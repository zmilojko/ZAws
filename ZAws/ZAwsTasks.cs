using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZAws.Console;

namespace ZAws
{
    abstract class ZAwsTask
    {
        protected ZAwsEc2Controller myController;

        public readonly string Taskname;

        public string StartMessage { get; protected set; }
        public string SuccessMessage { get; protected set; } // {0} - ID
        public string ExceptionMessage { get; protected set; } //{0} - exception

        public int DelayBeforeTaskStart { get; protected set; } ///Seconds!

        protected ZAwsTask(ZAwsEc2Controller controller, string taskname)
        {
            Taskname = taskname;

            myController = controller;

            StartMessage = string.Format("Starting task {0}", taskname);
            SuccessMessage = string.Format("Task executed succesfully.");
            ExceptionMessage = "Error executing task "+taskname+", reason: {1}";

            DelayBeforeTaskStart = 0;
        }

        abstract internal void DoExecute(ZAwsObject TargetObject);
        abstract internal bool WillHandle(ZAwsObject TargetObject);

        internal void SetQueue(ZAwsTaskHandler q)
        {
            MyQueue = q;
        }


        protected ZAwsTaskHandler MyQueue
        {
            get;
            private set;
        }
    }

    class ZAwsTaskHandler
    {
        readonly ZAwsEc2Controller myController;
        Thread handlerThread;
        bool runner = true;
        EventWaitHandle runnerEver = new EventWaitHandle(false, EventResetMode.ManualReset);
        EventWaitHandle sleepEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        List<ZAwsTask> myTaskList = new List<ZAwsTask>();
        Queue<ZAwsObject> myTagetObjects = new Queue<ZAwsObject>();
        object myLock = new object();


        internal ZAwsTaskHandler(ZAwsEc2Controller controller)
        {
            myController = controller;
            handlerThread = new Thread(new ThreadStart(delegate
                                            {
                                                while (runner)
                                                {
                                                    ZAwsObject currentObject = null;
                                                    lock (myLock)
                                                    {
                                                        try
                                                        {
                                                            currentObject = myTagetObjects.Dequeue();
                                                        }
                                                        catch(System.InvalidOperationException)
                                                        {

                                                                if (!runner)
                                                                {
                                                                    return;
                                                                }
                                                                runnerEver.Reset();
                                                        }
                                                    }

                                                    if (currentObject == null)
                                                    {
                                                        //Now wait for the next object to be added
                                                        runnerEver.WaitOne();
                                                        continue;
                                                    }

                                                    while (true)
                                                    {
                                                        ZAwsTask currentTask = null;
                                                        lock (myLock)
                                                        {
                                                            for (int i = 0; i < myTaskList.Count; i++)
                                                            {
                                                                if (myTaskList[i].WillHandle(currentObject))
                                                                {
                                                                    //remove this task from list
                                                                    currentTask = myTaskList[i];
                                                                    myTaskList.RemoveAt(i);
                                                                    break;
                                                                }
                                                            }
                                                        }

                                                        if (currentTask == null)
                                                        {
                                                            break;
                                                        }

                                                        if (sleepEvent.WaitOne(currentTask.DelayBeforeTaskStart * 1000))
                                                        {
                                                            //This event is only set by the Close function, so if this returns true, you should close right away
                                                            return;
                                                        }
                                                        Program.TraceLine(currentTask.StartMessage);
                                                        try
                                                        {
                                                            currentTask.DoExecute(currentObject);
                                                            Program.TraceLine(currentTask.SuccessMessage);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Program.TraceLine(currentTask.ExceptionMessage, ex);
                                                        }
                                                    }
                                                }
                                            }));
            handlerThread.Start();
        }
        internal void Close()
        {
            lock (myLock)
            {
                runner = false;
                runnerEver.Set();
                sleepEvent.Set();
            }
            if(!handlerThread.Join(10000))
            {
                handlerThread.Abort();
            }
            handlerThread = null;
        }

        public void AddTask(ZAwsTask newTask)
        {
            lock (myLock)
            {
                newTask.SetQueue(this);
                myTaskList.Add(newTask);
                runnerEver.Set();
            }
        }

        public void HandleNewObject(ZAwsObject TargetObject)
        {
            lock (myLock)
            {
                myTagetObjects.Enqueue(TargetObject);
                runnerEver.Set();
            }
        }
    }

    class ZAwsTaskNewSpotRequestHandling :ZAwsTask 
    {
        string SpotRequestId;
        string Ec2NewName;

        public ZAwsTaskNewSpotRequestHandling(ZAwsEc2Controller controller, string spotRequestId, string Name)
            : base(controller, "New Spot Request handling.")
        {
            SpotRequestId = spotRequestId;
            Ec2NewName = Name;

            StartMessage = "";
            SuccessMessage = "Succesfully handled new Spot Request.";
            ExceptionMessage = "Error handling Spot Request, reason: {0}";

            DelayBeforeTaskStart = 0;
        }

        internal override bool WillHandle(ZAwsObject TargetObject)
        {
            return TargetObject.GetType() == typeof(ZAwsSpotRequest) && TargetObject.Id == SpotRequestId && !string.IsNullOrWhiteSpace(((ZAwsSpotRequest)TargetObject).InstanceId);
        }

        internal override void DoExecute(ZAwsObject TargetObject)
        {
            MyQueue.AddTask(new ZAwsTaskHandleNewSpotRequestBasedEc2Instance((ZAwsSpotRequest)TargetObject, Ec2NewName));
        }
    }

    class ZAwsTaskHandleNewSpotRequestBasedEc2Instance : ZAwsTask 
    {
        ZAwsSpotRequest SpotRequest;
        string Ec2NewName;

        public ZAwsTaskHandleNewSpotRequestBasedEc2Instance(ZAwsSpotRequest spotRequest, string SupposedInstanceName)
            : base(spotRequest.myController, "New EC2 based on Spot Request handling.")
        {
            SpotRequest = spotRequest;
            Ec2NewName = SupposedInstanceName;

            StartMessage = "";
            SuccessMessage = "Succesfully set the name " + Ec2NewName + " to new EC2 instance.";
            ExceptionMessage = "Error setting name to the new instance, reason: {0}";

            DelayBeforeTaskStart = 10;
        }

        internal override void DoExecute(ZAwsObject TargetObject)
        {
            ((ZAwsEc2)TargetObject).SetName(Ec2NewName);
        }

        internal override bool WillHandle(ZAwsObject TargetObject)
        {
            return TargetObject.GetType() == typeof(ZAwsEc2) && TargetObject.Id == SpotRequest.InstanceId && ((ZAwsEc2)TargetObject).Status == ZAwsEc2.Ec2Status.Running;
        }
    }
}
