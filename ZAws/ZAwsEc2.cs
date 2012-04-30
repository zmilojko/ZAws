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
using Amazon.Route53.Model;

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

            /*
            Trace.WriteLine("Now will see to configure new applications.");
            ConfigureAppsWhenBootingComplete = myController.HandleNewEc2Instance(this) ? 3 : 0;
            Trace.WriteLine("ConfigureAppsWhenBootingComplete = " + ConfigureAppsWhenBootingComplete.ToString());
             * */
        }

        //int ConfigureAppsWhenBootingComplete = 0;

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

        public override string Id
        {
            get
            {
                return InstanceId;
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
            ConnectApp(System.Configuration.ConfigurationManager.AppSettings["SSHTerminalApp"],
                System.Configuration.ConfigurationManager.AppSettings["SSHTerminaAppArgs"]);

            /*
            string awsTerminalApp = System.Configuration.ConfigurationManager.AppSettings["SSHTerminalApp"];
            string awsTerminalCommandLines = string.Format(System.Configuration.ConfigurationManager.AppSettings["SSHTerminaAppArgs"],
                this.Reservation.RunningInstance[0].PublicDnsName,
                PrivateKeyFile + ".ppk");

            Process p = Process.Start(awsTerminalApp, awsTerminalCommandLines);
             * */
        }

        internal void StartSshFileBrowser()
        {
            ConnectApp(System.Configuration.ConfigurationManager.AppSettings["SSHFileBrowserApp"],
                System.Configuration.ConfigurationManager.AppSettings["SSHFileBrowserAppArgs"]);
            /*
            string awsKeyPath = System.Configuration.ConfigurationManager.AppSettings["SSHPrivateKeysDir"];
            string awsTerminalApp = System.Configuration.ConfigurationManager.AppSettings["SSHFileBrowserApp"];
            string awsTerminalCommandLines = string.Format(System.Configuration.ConfigurationManager.AppSettings["SSHFileBrowserAppArgs"],
                this.Reservation.RunningInstance[0].PublicDnsName,
                awsKeyPath + this.Reservation.RunningInstance[0].KeyName + ".ppk");

            Process p = Process.Start(awsTerminalApp, awsTerminalCommandLines);
             * */
        }

        internal void ConnectApp(string appLoc, string argPattern, params string[] command_line_arguments)
        {
            Process p = Process.Start(appLoc, string.Format(argPattern.Replace("{PublicDNS}", this.Reservation.RunningInstance[0].PublicDnsName)
                .Replace("{PublicIP}", this.Reservation.RunningInstance[0].IpAddress)
                .Replace("{PublicKeyFile}", PrivateKeyFile)
                , command_line_arguments));
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

        public class DataSample
        {
            public DateTime Time;
            public decimal Value;
        }

        public class Dimension
        {
            public DataSample[] Values = null;
        }

        /// <summary>
        /// Unit is %.
        /// </summary>
        public Dimension StatisticCPUUtiliationMax = new Dimension();
        /// <summary>
        /// Unit is %.
        /// </summary>
        public Dimension StatisticCPUUtiliationAvg = new Dimension();
        /// <summary>
        /// Unit is Kbps.
        /// </summary>
        public Dimension StatisticNetworkOut = new Dimension();

        DateTime timeSpanRequired = DateTime.Now - TimeSpan.FromDays(1);

        DateTime monitoringSegmentEnd;
        bool monitoringSegmentEndSet = false;
        TimeSpan MonitoringSpan = TimeSpan.FromDays(1);

        public DateTime MonitoringSegmentStart
        {
            get
            {
                return MonitoringSegmentEnd - MonitoringSpan;
            }
        }

        int Period
        {
            get
            {
                return Math.Min((int)(MonitoringSpan.TotalSeconds / 288), 300);
            }
        }

        public DateTime MonitoringSegmentEnd
        {
            set
            {
                if (DateTime.Now - value < TimeSpan.FromHours(1))
                {
                    monitoringSegmentEndSet = false;
                }
                else
                {
                    monitoringSegmentEndSet = true;
                    monitoringSegmentEnd = value;
                }
            }
            get
            {
                if (monitoringSegmentEndSet)
                {
                    return monitoringSegmentEnd;
                }
                else
                {
                    return DateTime.Now;
                }
            }
        }


        internal void UpdateInfo()
        {
            bool Change = false;

            if (this.Status == Ec2Status.Running)
            {
                //this will give us 5 minute interval on 1 day 
                

                var resp2 = myController.CloudWatch.GetMetricStatistics(new GetMetricStatisticsRequest()
                                                                    .WithNamespace("AWS/EC2")

                                                                    .WithDimensions(new Amazon.CloudWatch.Model.Dimension()
                                                                                                .WithName("InstanceId")
                                                                                                .WithValue(this.InstanceId))

                                                                    .WithStartTime(MonitoringSegmentStart.ToUniversalTime())
                                                                    .WithEndTime(MonitoringSegmentEnd.ToUniversalTime())
                                                                    .WithPeriod(Period)
                                                                    .WithMetricName("CPUUtilization")
                                                                    .WithUnit("Percent")
                                                                    .WithStatistics("Average", "Maximum"));

                if (resp2.GetMetricStatisticsResult.Datapoints.Count > 0)
                {

                    StatisticCPUUtiliationMax.Values = new DataSample[resp2.GetMetricStatisticsResult.Datapoints.Count];
                    StatisticCPUUtiliationAvg.Values = new DataSample[resp2.GetMetricStatisticsResult.Datapoints.Count];
                    for (int i = 0; i < resp2.GetMetricStatisticsResult.Datapoints.Count; i++)
                    {
                        StatisticCPUUtiliationMax.Values[i] = new DataSample()
                        {
                            Value = (decimal)resp2.GetMetricStatisticsResult.Datapoints[i].Maximum,
                            Time = resp2.GetMetricStatisticsResult.Datapoints[i].Timestamp
                        }; 
                        StatisticCPUUtiliationAvg.Values[i] = new DataSample()
                        {
                            Value = (decimal)resp2.GetMetricStatisticsResult.Datapoints[i].Average,
                            Time = resp2.GetMetricStatisticsResult.Datapoints[i].Timestamp
                        };
                    }
                    

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

                                                                    .WithDimensions(new Amazon.CloudWatch.Model.Dimension()
                                                                                                .WithName("InstanceId")
                                                                                                .WithValue(this.InstanceId))

                                                                    .WithStartTime(MonitoringSegmentStart.ToUniversalTime())
                                                                    .WithEndTime(MonitoringSegmentEnd.ToUniversalTime())
                                                                    .WithPeriod(Period)
                                                                    .WithMetricName("NetworkOut")
                                                                    .WithUnit("Bytes")
                                                                    .WithStatistics("Sum"));
                if (resp3.GetMetricStatisticsResult.Datapoints.Count > 0)
                {
                    StatisticNetworkOut.Values = new DataSample[resp3.GetMetricStatisticsResult.Datapoints.Count];
                    for (int i = 0; i < resp3.GetMetricStatisticsResult.Datapoints.Count; i++)
                    {
                        StatisticNetworkOut.Values[i] = new DataSample()
                        {
                            Value = (decimal)resp3.GetMetricStatisticsResult.Datapoints[i].Sum / 1000,
                            Time = resp3.GetMetricStatisticsResult.Datapoints[i].Timestamp
                        };
                    }

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
                /*
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
                 * */
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
            //ConfigureAppsWhenBootingComplete = 0;
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
            /*
            Amazon.EC2.Model.DeleteTagsResponse response = myController.ec2.DeleteTags(new Amazon.EC2.Model.DeleteTagsRequest()
                                               .WithResourceId(this.InstanceId)
                                               .WithTag(new Amazon.EC2.Model.DeleteTags().WithKey("Name")));
             */
            Amazon.EC2.Model.CreateTagsResponse response2 = myController.ec2.CreateTags(new Amazon.EC2.Model.CreateTagsRequest()
                                    .WithResourceId(this.InstanceId)
                                    .WithTag(new Amazon.EC2.Model.Tag().WithKey("Name").WithValue(p)));
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

        public enum ApplicationType { GENERIC, RAILS_APP };

        public class Application
        {
            internal Application(ZAwsEc2 server, string name, string url, string repo, string reponame, ApplicationType type)
            {
                Server = server;
                Name = name;
                URL = url;
                Repo = repo;
                RepoName = reponame;
                AppType = type;
            }
            public readonly string Name;
            public readonly string URL;
            public readonly string Repo;
            public readonly string RepoName;
            public readonly ApplicationType AppType;
            public readonly ZAwsEc2 Server;

            public void Update()
            {
                Server.SshClient.SendLine("sudo su", true);
                Program.Tracer.Trace(false, Server.SshClient.GetResponse().Replace("\n", "\nssh>"));
                Server.SshClient.SendLine(string.Format("cd /var/{0}/{1}", this.AppType == ApplicationType.GENERIC ? "web_apps" : "rails_apps",
                    this.Name), true);
                Program.Tracer.Trace(false, Server.SshClient.GetResponse().Replace("\n", "\nssh>"));
                Server.SshClient.SendLine(string.Format("git pull {0} master", this.RepoName), true);
                Program.Tracer.Trace(false, Server.SshClient.GetResponse().Replace("\n", "\nssh>"));
            }
        }

        List<Application> installedApplications = null;

        public Application[] GetInstalledApps()
        {
            return GetInstalledApps(false);
        }
        public Application[] GetInstalledApps(bool forceRefresh)
        {
            if (installedApplications == null || forceRefresh)
            {
                installedApplications = new List<Application>();
                this.SshClient.SendLine("sudo su", true);
                this.SshClient.GetResponse();
                this.SshClient.SendLine("ls /etc/httpd/virtual_hosts/", true);
                string resp;
                Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));
                string[] lsResp = resp.Split(new string[] { " ", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 2; i < lsResp.Length - 2; i++)
                {
                    this.SshClient.SendLine("cat /etc/httpd/virtual_hosts/" + lsResp[i], true);
                    Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));
                    string[] lsResp2 = resp.Split(new string[] { " ", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] pathSegments = lsResp2[7].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    ApplicationType t;
                    string name;
                    string path = lsResp2[7];
                    
                    if(pathSegments[pathSegments.Length - 1] == "public")
                    {
                        t = ApplicationType.RAILS_APP;
                        name = pathSegments[pathSegments.Length - 2];
                        path = path.Substring(0, path.LastIndexOf("/public"));
                    }
                    else
                    {
                        t = ApplicationType.GENERIC;
                        name = pathSegments[pathSegments.Length - 1];
                    }
                    this.SshClient.SendLine("cd " + path, true);
                    Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));
                    this.SshClient.SendLine("git remote -v", true);
                    Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));
                    string gitrepo = "" ;
                    string gitreponame = "";
                    if (!resp.ToLower().Contains("Not a git repository".ToLower()))
                    {
                        gitreponame = resp.Split(new string[] { " ", "\n", "\r", "\t" }, StringSplitOptions.RemoveEmptyEntries)[3];
                        gitrepo = resp.Split(new string[] { " ", "\n", "\r", "\t" }, StringSplitOptions.RemoveEmptyEntries)[4];
                    }
                    installedApplications.Add(new Application(this, name, lsResp2[5], gitrepo, gitreponame, t));
                }
            }
            return installedApplications.ToArray();
        }

        public void RebootWebServer()
        {
            this.SshClient.SendLine("sudo service httpd restart", true);
            string resp;
            Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));
            if (resp.Contains("Starting httpd") && resp.Substring(resp.IndexOf("Starting httpd") + 14).Contains("OK"))
            {
                return;
            }
            //Lets try to solve this problem: it might be that 
            Program.Tracer.TraceLine(false, "Problem restarting Apache. Trying to kill the process brutaly and start it.");
            this.SshClient.SendLine("sudo netstat -tulpn | grep 80", true);
            Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));
            var resp2 = resp.Split(new char[] { ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);
            for(int i=0; i< resp2.Length;i++)
            {
                if (resp2[i] == "httpd" && i>0)
                {
                    this.SshClient.SendLine("sudo kill " + resp2[i-1], true);
                    Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));
                    this.SshClient.SendLine("sudo service httpd start", true);
                    Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));
                    if (resp.Contains("Starting httpd") && resp.Substring(resp.IndexOf("Starting httpd") + 14).Contains("OK"))
                    {
                        return;
                    }
                    break;
                }
            }
            throw new Exception("Not received OK response from server.");
        }

        internal void InstallApp(string name, string url, string repo_location, ApplicationType applicationType, bool createUrlDnsEntry, bool defaultApp)
        {
            string resp;
            this.SshClient.SendLine("sudo su", true);
            Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));

            //Step 1: Craete a folder and git pull everything in it
            this.SshClient.SendLine("cd /var/" + (applicationType == ApplicationType.GENERIC ? "web_apps" : "rails_apps"), true);
            Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));

            this.SshClient.SendLine("mkdir " + name, true);
            Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));
            this.SshClient.SendLine("cd " + name, true);
            Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));

            this.SshClient.SendLine("git init", true);
            Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));

            string cmd = string.Format("git remote add origin {0}", repo_location);
            this.SshClient.SendLine(cmd, true);
            Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));

            this.SshClient.SendLine("git pull origin master", true);
            Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));

            //Step 2: Execute README bash script            
            this.SshClient.SendLine("bash STARTME", true);
            Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));

            //Step 3: Create virtual host
            this.SshClient.SendLine("cd /etc/httpd/virtual_hosts/", true);
            Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));

            string config_text = string.Format(ZAws.Properties.Resources.ConfigText, name, url,
                applicationType == ApplicationType.GENERIC ? "" : "/public", defaultApp ? "_" : "");
            this.SshClient.SendLine(config_text, true);
            Program.Tracer.TraceLine(false, "ssh>" + (resp = this.SshClient.GetResponse()).Replace("\n", "\nssh>"));

            //Step 4: If needed, assign URl A record to this EC2
            if (!string.IsNullOrWhiteSpace(url) && createUrlDnsEntry)
            {
                this.PointUrl(url);
            }


        }

        public ZAwsElasticIp AssociatedIP
        {
            get
            {
                foreach (var ip in myController.CurrentElasticIps)
                {
                    if (ip.Associated && ip.AssociatedEc2.Id == Id)
                    {
                        return ip;
                    }
                }
                return null;
            }
        }

        public bool AssociateIp()
        {
            if (AssociatedIP != null)
            {
                return true;
            }
            foreach (var ip in myController.CurrentElasticIps)
            {
                if (!ip.Associated)
                {
                    ip.Associate(this);
                    return true;
                }
            }
            myController.AllocateIp();
            myController.myTaskQueue.AddTask(new ZAwsTaskAssociateNewIpToEc2(this));
            return false;
        }

        public void PointUrl(string url)
        {
            if (!AssociateIp())
            {
                myController.myTaskQueue.AddTask(new ZAwsTaskPointARecordToEc2(this, url));
                return;
            }
            Program.TraceLine(">>>Searchign for a hosted zone to set following URL: {0}", url);
            foreach (var zone in myController.CurrentHostedZones)
            {
                //Name of the hosted zone must match the ending of the app URL. 
                if (url.TrimEnd('.').IndexOf(zone.Name.TrimEnd('.')) == url.TrimEnd('.').Length - zone.Name.TrimEnd('.').Length)
                {
                    Program.TraceLine(">>>Adding following record: {0} to hosted zone {1}", url, zone.Name);
                    //Now add the record
                    zone.AddRecord(new Amazon.Route53.Model.ResourceRecordSet()
                                                        .WithName(url)
                                                        .WithType("A")
                                                        .WithTTL(800)
                                                        .WithResourceRecords(new ResourceRecord()
                                                                                    .WithValue(AssociatedIP.Name)));
                }
            }
        }
    }
}
