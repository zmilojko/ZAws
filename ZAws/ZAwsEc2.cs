///////////////////////////////////////////////////////////////////////////////
//   Copyright 2012 Z-Ware Ltd.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Amazon.EC2.Model;
using Amazon.CloudWatch.Model;
using ZAws.Console;
using System.Threading;

namespace ZAws
{
    class ZAwsEc2 : ZAwsObject
    {
        public Amazon.EC2.Model.Reservation Reservation { get; private set; }

        public ZAwsEc2(ZAwsEc2Controller controller, Amazon.EC2.Model.Reservation res)
            : base(controller)
        {
            StatisticsValid = false;

            LatestBootConsoleTimestamp = "";
            ConsoleOutput = "";

            Update(res);

            Trace.WriteLine("Now will see to configure new applications.");
            ConfigureAppsWhenBootingComplete = myController.HandleNewEc2Instance(this) ? 3 : 0;
            Trace.WriteLine("ConfigureAppsWhenBootingComplete = " + ConfigureAppsWhenBootingComplete.ToString());
        }

        int ConfigureAppsWhenBootingComplete = 0;

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

            /*
            //First time public DNS is received, configure apps
            if (Reservation != null && string.IsNullOrWhiteSpace(InstanceId)
                && !string.IsNullOrWhiteSpace(((Amazon.EC2.Model.Reservation)responseData).RunningInstance[0].InstanceId))
            {
                Trace.Wri
                ConfigureAppsWhenBootingComplete = myController.HandleNewEc2Instance(this);
            }
            */
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

        internal string PrivateKeyFile
        {
            get
            {
                string awsKeyPath = System.Configuration.ConfigurationManager.AppSettings["SSHPrivateKeysDir"];
                return awsKeyPath + this.Reservation.RunningInstance[0].KeyName;
            }
        }

        internal void StartTerminal()
        {
            
            string awsTerminalApp = System.Configuration.ConfigurationManager.AppSettings["SSHTerminalApp"];
            string awsTerminalCommandLines = string.Format(System.Configuration.ConfigurationManager.AppSettings["SSHTerminaAppArgs"],
                this.Reservation.RunningInstance[0].PublicDnsName,
                PrivateKeyFile + ".ppk");

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

            if (this.Status == Ec2Status.Running)
            {

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
                                                                    .WithStatistics("Average", "Maximum"));

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
            }
            //check terminal


            if (this.Status != Ec2Status.Running || string.IsNullOrWhiteSpace(this.ConsoleOutput))
            {
                Amazon.EC2.Model.GetConsoleOutputResponse resp =
                    myController.ec2.GetConsoleOutput(new Amazon.EC2.Model.GetConsoleOutputRequest()
                                            .WithInstanceId(this.InstanceId));

                if (LatestBootConsoleTimestamp != resp.GetConsoleOutputResult.ConsoleOutput.Timestamp)
                {
                    LatestBootConsoleTimestamp = resp.GetConsoleOutputResult.ConsoleOutput.Timestamp;


                    ConsoleOutput = Encoding.UTF8.GetString(Convert.FromBase64String(resp.GetConsoleOutputResult.ConsoleOutput.Output));

                    if (ConsoleUpdate != null)
                    {
                        ConsoleUpdate(this, new NewConceolOutputEventArgs(ConsoleOutput, LatestBootConsoleTimestamp));
                    }
                }
                if (this.ConfigureAppsWhenBootingComplete > 0)
                {
                    try
                    {
                        ConfigureAppsOnBootComplete();
                    }
                    catch (Exception ex)
                    {
                        Program.TraceLine("Error while configuring apps on boot complete, reason: ", ex);
                        ConfigureAppsWhenBootingComplete--;
                    }
                }
            }
        }

        private void ConfigureAppsOnBootComplete()
        {
            if (string.IsNullOrWhiteSpace(this.Reservation.RunningInstance[0].PublicDnsName)
                || string.IsNullOrWhiteSpace(this.ConsoleOutput))
            {
                return;
            }

            //We will hardcode this for a while
            Program.TraceLine("Configuring apps through SSH");
            Program.Tracer.Trace(false, "ssh>" + SshClient.GetResponse().Replace("\n", "\nssh>"));
            //Program.Tracer.TraceLine(false, "sudo su");
            this.SshClient.SendLine("sudo su", true);
            Program.Tracer.Trace(false, SshClient.GetResponse().Replace("\n", "\nssh>"));
            foreach (var app in this.myController.runningApps)
            {
                if (app.DeployedOnInstanceId == this.InstanceId)
                {
                    //Program.Tracer.TraceLine(false, "bash /var/rails_apps/{0}/STARTME > /var/log/app_init_{0}", app.AppName);
                    this.SshClient.SendLine(string.Format("bash /var/rails_apps/{0}/STARTME" /* > /var/log/app_init_{0}"*/, app.AppName), true, 10 * 60 * 1000);
                    Program.Tracer.Trace(false, SshClient.GetResponse().Replace("\n", "\nssh>"));
                }
            }
            this.SshClient.SendLine("reboot");
            Thread.Sleep(1000);
            Program.Tracer.Trace(false, SshClient.GetResponse().Replace("\n", "\nssh>"));
            this.SshClient.Close();
            Program.Tracer.TraceLine("\nssh>SSH DISCONNECTED.");
            ConfigureAppsWhenBootingComplete = 0;
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

        public void SetName(string p)
        {
             Amazon.EC2.Model.CreateTagsResponse response2 = myController.ec2.CreateTags(new Amazon.EC2.Model.CreateTagsRequest()
                                                .WithResourceId(this.InstanceId)
                                                .WithTag(new Amazon.EC2.Model.Tag().WithKey("Name").WithValue(Name)));
        }

        ZawsSshClient sshClient = null;
        void CloseSshClient()
        {
            if (sshClient != null)
            {
                Program.TraceLine("Closing SSH interface to {0}", this.Reservation.RunningInstance[0].PublicDnsName);
                sshClient.Close();
                sshClient = null;
            }
        }
        public ZawsSshClient SshClient
        {
            get
            {
                if (sshClient == null)
                {
                    Program.TraceLine("Opening SSH interface to {0}", this.Reservation.RunningInstance[0].PublicDnsName);
                    sshClient = new ZawsSshClient(this.Reservation.RunningInstance[0].PublicDnsName, "ec2-user", "", PrivateKeyFile + ".ssh2");
                }
                return sshClient;
            }
        }
    }
}
