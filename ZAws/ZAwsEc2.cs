using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Amazon.EC2.Model;
using Amazon.CloudWatch.Model;

namespace ZAws
{
    class ZAwsEc2 : ZAwsObject
    {
        public Amazon.EC2.Model.Reservation Reservation { get; private set; }

        public ZAwsEc2(ZAwsEc2Controller controller, Amazon.EC2.Model.Reservation res)
            : base(controller)
        {
            Update(res);

            StatisticsValid = false;

            LatestBootConsoleTimestamp = "";
            ConsoleOutput = "";

            myController.HandleNewEc2Instance(this);
        }

        public override string Name
        {
            get
            {
                //find the instance name among the tags
                string InstanceName = "not set";
                foreach (Amazon.EC2.Model.Tag t in Reservation.RunningInstance[0].Tag)
                {
                    if (t.Key.ToLower() == "name")
                    {
                        InstanceName = t.Value;
                    }
                }
                return InstanceName;
            }
        }

        /// <summary>
        /// Returns one of the following: 0 for Running, 1 for Stopped, 2 for Stopping, 3 for Terminated, 
        /// 4 for Shutting-down and 5 for Pending.
        /// </summary>
        public Ec2Status Status
        {
            get
            {
                switch (((int)(Math.Truncate(Reservation.RunningInstance[0].InstanceState.Code))) % 256)
                {
                    case 16: return Ec2Status.Running;
                    case 32: return Ec2Status.ShuttingDown;
                    case 48: return Ec2Status.Terminated;
                    case 64: return Ec2Status.Stopping;
                    case 80: return Ec2Status.Stopped;
                    default: return Ec2Status.Pending;
                }
            }

        }

        public enum Ec2Status
        {
            Running = 0,
            Stopped = 1,
            Stopping = 2,
            Terminated = 3,
            ShuttingDown = 4,
            Pending = 5,
        }


        public string InstanceId
        {
            get
            {
                return Reservation.RunningInstance[0].InstanceId;
            }
        }

        protected override bool DoUpdate(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.Reservation), "Wrong data passed to the object for update.");
            Reservation = (Amazon.EC2.Model.Reservation)responseData;
            return true;
        }

        internal override bool EqualsData(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.Reservation), "Wrong data passed to the object for update.");
            return string.Equals(InstanceId, ((Amazon.EC2.Model.Reservation)responseData).RunningInstance[0].InstanceId);
        }

        internal void Start()
        {
            StartInstancesResponse resp = myController.ec2.StartInstances(new StartInstancesRequest()
                    .WithInstanceId(InstanceId));
        }

        internal void Stop()
        {
            StopInstancesResponse resp = myController.ec2.StopInstances(new StopInstancesRequest()
                    .WithInstanceId(InstanceId));
        }
        internal void Reboot()
        {
            RebootInstancesResponse resp = myController.ec2.RebootInstances(new RebootInstancesRequest()
                    .WithInstanceId(InstanceId));
        }

        protected override void DoDeleteObject()
        {
            if (Status != Ec2Status.Terminated)
            {
                TerminateInstancesResponse resp = myController.ec2.TerminateInstances(new TerminateInstancesRequest()
                                                        .WithInstanceId(InstanceId));
            }
        }

        internal void StartTerminal()
        {
            string awsKeyPath = System.Configuration.ConfigurationManager.AppSettings["SSHPrivateKeysDir"];
            string awsTerminalApp = System.Configuration.ConfigurationManager.AppSettings["SSHTerminalApp"];
            string awsTerminalCommandLines = string.Format(System.Configuration.ConfigurationManager.AppSettings["SSHTerminaAppArgs"],
                this.Reservation.RunningInstance[0].PublicDnsName,
                awsKeyPath + this.Reservation.RunningInstance[0].KeyName + ".ppk");

            Process p = Process.Start(awsTerminalApp, awsTerminalCommandLines);
        }

        internal void StartSshFileBrowser()
        {
            string awsKeyPath = System.Configuration.ConfigurationManager.AppSettings["SSHPrivateKeysDir"];
            string awsTerminalApp = System.Configuration.ConfigurationManager.AppSettings["SSHFileBrowserApp"];
            string awsTerminalCommandLines = string.Format(System.Configuration.ConfigurationManager.AppSettings["SSHFileBrowserAppArgs"],
                this.Reservation.RunningInstance[0].PublicDnsName,
                awsKeyPath + this.Reservation.RunningInstance[0].KeyName + ".ppk");

            Process p = Process.Start(awsTerminalApp, awsTerminalCommandLines);
        }

        public bool StatisticsValid { get; private set; }
        public int CPUUtilizationMax { get; private set; }
        public int CPUUtilizationAvg { get; private set; }
        public int NetworkOutRecent5Min { get; private set; }
        public string NetworkOutRecent5MinString
        {
            get
            {
                if (NetworkOutRecent5Min < 1000)
                {
                    return NetworkOutRecent5Min.ToString() + "B";
                }
                if (NetworkOutRecent5Min < 10000)
                {
                    return (NetworkOutRecent5Min / 1000).ToString() + "." + ((NetworkOutRecent5Min % 1000) / 100).ToString() + "KB";
                }
                if (NetworkOutRecent5Min < 500000)
                {
                    return (NetworkOutRecent5Min / 1000).ToString() + "KB";
                }
                if (NetworkOutRecent5Min < 10000000)
                {
                    return (NetworkOutRecent5Min / 1000000).ToString() + "." + ((NetworkOutRecent5Min % 1000000) / 100000).ToString() + "MB";
                }
                return (NetworkOutRecent5Min / 1000000).ToString() + "MB";
            }
        }


        internal void UpdateInfo()
        {
            bool Change = false;
            /*
            var resp = myController.CloudWatch.ListMetrics(new Amazon.CloudWatch.Model.ListMetricsRequest()
            
                                                                .WithNamespace("AWS/EC2")
                                                                
                                                                .WithDimensions(new DimensionFilter()
                                                                                            .WithName("InstanceId")
                                                                                            .WithValue(this.InstanceId)));
            */
            var resp2 = myController.CloudWatch.GetMetricStatistics(new GetMetricStatisticsRequest()
                                                                .WithNamespace("AWS/EC2")
                                                                
                                                                .WithDimensions(new Dimension()
                                                                                            .WithName("InstanceId")
                                                                                            .WithValue(this.InstanceId))
                                                                 
                                                                .WithStartTime((DateTime.Now - TimeSpan.FromHours(4)).ToUniversalTime())
                                                                .WithEndTime(DateTime.Now.ToUniversalTime())
                                                                .WithPeriod(300)
                                                                .WithMetricName("CPUUtilization")
                                                                .WithUnit("Percent")
                                                                .WithStatistics("Average","Maximum"));

            if (resp2.GetMetricStatisticsResult.Datapoints.Count > 0)
            {

                int CPUUtilizationTemp = (int)resp2.GetMetricStatisticsResult.Datapoints[resp2.GetMetricStatisticsResult.Datapoints.Count - 1].Maximum;
                if (CPUUtilizationTemp != CPUUtilizationMax)
                {
                    CPUUtilizationMax = CPUUtilizationTemp;
                    Change = true;
                } 
                CPUUtilizationTemp = (int)resp2.GetMetricStatisticsResult.Datapoints[resp2.GetMetricStatisticsResult.Datapoints.Count - 1].Average;
                if (CPUUtilizationTemp != CPUUtilizationAvg)
                {
                    CPUUtilizationAvg = CPUUtilizationTemp;
                    Change = true;
                }
                StatisticsValid = true;
            }

            var resp3 = myController.CloudWatch.GetMetricStatistics(new GetMetricStatisticsRequest()
                                                                .WithNamespace("AWS/EC2")

                                                                .WithDimensions(new Dimension()
                                                                                            .WithName("InstanceId")
                                                                                            .WithValue(this.InstanceId))

                                                                .WithStartTime((DateTime.Now - TimeSpan.FromHours(4)).ToUniversalTime())
                                                                .WithEndTime(DateTime.Now.ToUniversalTime())
                                                                .WithPeriod(300)
                                                                .WithMetricName("NetworkOut")
                                                                .WithUnit("Bytes")
                                                                .WithStatistics("Sum"));
            if (resp3.GetMetricStatisticsResult.Datapoints.Count > 0)
            {
                int NetworkOutRecent5MinTemp = (int)resp3.GetMetricStatisticsResult.Datapoints[resp3.GetMetricStatisticsResult.Datapoints.Count - 1].Sum;
                if (NetworkOutRecent5MinTemp != NetworkOutRecent5Min)
                {
                    NetworkOutRecent5Min = NetworkOutRecent5MinTemp;
                    Change = true;
                }
                StatisticsValid = true;
            }

            if (Change)
            {
                this.TriggerStatusChanged();
            }

            //check terminal

            Amazon.EC2.Model.GetConsoleOutputResponse resp =
                myController.ec2.GetConsoleOutput(new Amazon.EC2.Model.GetConsoleOutputRequest()
                                        .WithInstanceId(this.InstanceId));

            if (LatestBootConsoleTimestamp != resp.GetConsoleOutputResult.ConsoleOutput.Timestamp)
            {
                LatestBootConsoleTimestamp = resp.GetConsoleOutputResult.ConsoleOutput.Timestamp;


                ConsoleOutput = Encoding.UTF8.GetString(Convert.FromBase64String(resp.GetConsoleOutputResult.ConsoleOutput.Output));
 
                if(ConsoleUpdate != null)
                {
                    ConsoleUpdate(this, new NewConceolOutputEventArgs(ConsoleOutput, LatestBootConsoleTimestamp));
                }
            }
        }

        public class NewConceolOutputEventArgs : EventArgs
        {
            public NewConceolOutputEventArgs(string output, string timestamp)
            { Output = output; Timestamp = timestamp; }

            public readonly string Timestamp;
            public readonly string Output;
        }

        public event EventHandler<NewConceolOutputEventArgs> ConsoleUpdate;

        public string LatestBootConsoleTimestamp { get; private set; }
        public string ConsoleOutput { get; private set; }

        internal void SetName(string p)
        {
             Amazon.EC2.Model.CreateTagsResponse response2 = myController.ec2.CreateTags(new Amazon.EC2.Model.CreateTagsRequest()
                                                .WithResourceId(this.InstanceId)
                                                .WithTag(new Amazon.EC2.Model.Tag().WithKey("Name").WithValue(Name)));
        }
    }
}
