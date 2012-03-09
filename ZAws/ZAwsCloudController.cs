using Amazon.EC2;
using Amazon;
using System;
using Amazon.EC2.Model;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
namespace ZAws
{
    class ZAwsEc2Controller
    {




        string awsAccessKey = System.Configuration.ConfigurationManager.AppSettings["AWSAccessKey"];
        string awsSecretKey = System.Configuration.ConfigurationManager.AppSettings["AWSSecretKey"];
        string awsZoneUrl = "https://eu-west-1.ec2.amazonaws.com";

        AmazonEC2 ec2 = null;
        
        public class ZAwsNewObjectEventArgs : EventArgs
        {
            public ZAwsNewObjectEventArgs(ZAwsObject arg)
            { NewObject = arg; }

            public readonly ZAwsObject NewObject;
        }

        public event EventHandler<ZAwsNewObjectEventArgs> NewObject;

        private bool RunMonitoring = false;
        private object Ec2Lock = new object();
        private Thread MonitoringThread = null;
        List<ZAwsEc2> currentStatus = new List<ZAwsEc2>();

        public void Connect()
        {
            if(ec2 != null)
            {
                throw new ZAwsEWrongState("Controller is already open");
            }

            Debug.Assert(!RunMonitoring);
            Debug.Assert(MonitoringThread == null);

            ec2 = AWSClientFactory.CreateAmazonEC2Client(awsAccessKey, awsSecretKey,
                        new AmazonEC2Config().WithServiceURL(awsZoneUrl));

            //Start the thread
            MonitoringThread = new Thread(new ThreadStart(MonitorFunction));
            RunMonitoring = true;
            MonitoringThread.Start();
        }
        public void Disconnect()
        {
            if (ec2 != null)
            {
                Debug.Assert(RunMonitoring);
                Debug.Assert(MonitoringThread != null);

                bool killedTheThread = false;
                //Shut the thread
                lock (Ec2Lock)
                {
                    RunMonitoring = false;
                }
                if (!MonitoringThread.Join(5000))
                {
                    MonitoringThread.Abort();
                    killedTheThread = true;
                }
                MonitoringThread = null;

                ec2.Dispose();
                ec2 = null;

                if (killedTheThread)
                {
                    throw new ZAwsException("Connection failure, could not close connection gracefully. Might require restart.");
                }
            }
            else
            {
                Debug.Assert(!RunMonitoring);
                Debug.Assert(MonitoringThread == null);
            }
        }



        void MonitorFunction()
        {
            while (true)
            {
                DescribeInstancesResponse resp;
                List<ZAwsEc2> zawsInstanceList = new List<ZAwsEc2>();
                lock (Ec2Lock)
                {
                    if (!RunMonitoring)
                    {
                        return;
                    }
                }
                resp = GetRunningInstances();
                lock (Ec2Lock)
                {
                    if (!RunMonitoring)
                    {
                        return;
                    }
                }
                foreach (Amazon.EC2.Model.Reservation res in resp.DescribeInstancesResult.Reservation)
                {
                    bool found = false;
                    foreach (ZAwsEc2 oldI in this.currentStatus)
                    {
                        if (res.RunningInstance[0].InstanceId == oldI.InstanceId)
                        {
                            oldI.Update(res);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        var NewObj = new ZAwsEc2(this, res);
                        currentStatus.Add(NewObj);
                        if (NewObject != null)
                        {
                            NewObject(this, new ZAwsNewObjectEventArgs(NewObj));
                        }
                    }
                }

                //Now do check for terminated ones!
                List<ZAwsEc2> toDeleteList = new List<ZAwsEc2>();
                foreach (ZAwsEc2 oldI in this.currentStatus)
                {
                    bool found = false;
                    foreach (Amazon.EC2.Model.Reservation res in resp.DescribeInstancesResult.Reservation)
                    {
                        if (res.RunningInstance[0].InstanceId == oldI.InstanceId)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        toDeleteList.Add(oldI);
                    }
                }
                foreach (ZAwsEc2 oldToDelete in toDeleteList)
                {
                    this.currentStatus.Remove(oldToDelete);
                    oldToDelete.Delete();
                }


                //Give ther threads a chance, and also allow user to smoothly disconnect
                Thread.Sleep(200);
            }
        }

        /*
        bool ChangeExists(ZAwsEc2[] newStatus)
        {
            //TODO:
            foreach (ZAwsEc2 newI in newStatus)
            {
                bool found = false;
                foreach (ZAwsEc2 oldI in this.currentStatus)
                {
                    if (newI.InstanceId == oldI.InstanceId)
                    {
                        if (newI.Name != oldI.Name
                            || newI.StatusCode != oldI.StatusCode
                            )
                        {
                            currentStatus = newStatus;
                            return true;                                       // at least this one has changed
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    currentStatus = newStatus;
                    return true;                                                //there is a new one
                }
            }
            foreach (ZAwsEc2 oldI in this.currentStatus)
            {
                bool found = false;
                foreach (ZAwsEc2 newI in newStatus)
                {
                    if (newI.InstanceId == oldI.InstanceId)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    currentStatus = newStatus;
                    return true;                                                //there is at least one removed
                }
            }
            return false;
        }
        */
        public DescribeInstancesResponse GetRunningInstances()
        {
            DescribeInstancesRequest ec2Request = new DescribeInstancesRequest();
            DescribeInstancesResponse ec2Response = ec2.DescribeInstances(ec2Request);
            return ec2Response;
        }

        public bool StartInstance(ZAwsEc2 zAwsEc2Instance)
        {
            StartInstancesResponse resp = ec2.StartInstances(new StartInstancesRequest()
                    .WithInstanceId(zAwsEc2Instance.InstanceId));

            return true;
        }
        public bool StopInstance(ZAwsEc2 zAwsEc2Instance)
        {
            StopInstancesResponse resp = ec2.StopInstances(new StopInstancesRequest()
                    .WithInstanceId(zAwsEc2Instance.InstanceId));

            return true;
        }

        private void GetInfo()
        {
            //TODO: this is all just sample code to figure our the syntax! Do not use like this, it will not work.
            Amazon.CloudWatch.AmazonCloudWatchClient w = new Amazon.CloudWatch.AmazonCloudWatchClient();

            var resp = w.GetMetricStatistics(new Amazon.CloudWatch.Model.GetMetricStatisticsRequest());
            double b = resp.GetMetricStatisticsResult.Datapoints[0].Average;
        }
    }
}
