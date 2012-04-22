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
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using ZAws.Console;
using System.Threading;

namespace ZAws
{
    class ZAwsAmi : ZAwsObject
    {
        public Amazon.EC2.Model.Image ResponseData { get; private set; }

        public ZAwsAmi(ZAwsEc2Controller controller, Amazon.EC2.Model.Image res)
            : base(controller)
        {
            Update(res);
        }

        public override string Name
        {
            get
            {
                return ResponseData.Name;
            }
        }
        public override string Description
        {
            get
            {
                return "(" + ResponseData.ImageId + ")\n" + ResponseData.Description;
            }
        }

        protected override bool DoUpdate(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.Image), "Wrong data passed to the object for update.");
            ResponseData = (Amazon.EC2.Model.Image)responseData;
            return true;
        }

        internal override bool EqualsData(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.Image), "Wrong data passed to the object for update.");
            return string.Equals(ResponseData.ImageId, ((Amazon.EC2.Model.Image)responseData).ImageId);
        }

        internal string Launch(string InstanceSize, ZAwsSecGroup SecGroup, ZAwsKeyPair KeyPair, string Name, string StartupScript, NewApp[] AppsToInstall, decimal MaxBid)
        {
            //Prepare the StartupScript, using SETTINGS and FILE features.
            //Now search for all SETTINGS and replace with settings
            int settings_location = -1;
            while ((settings_location = StartupScript.IndexOf("{SETTING=")) != -1)
            {
                string keyname = StartupScript.Substring(settings_location + "{SETTING=".Length, StartupScript.IndexOf("}", settings_location) - settings_location - "{SETTING=".Length);

                string s = StartupScript.Substring(0, settings_location);
                s += System.Configuration.ConfigurationManager.AppSettings[keyname];
                s += StartupScript.Substring(StartupScript.IndexOf("}", settings_location) + 1);
                StartupScript = s;
            }
            settings_location = -1;
            while ((settings_location = StartupScript.IndexOf("{FILE=")) != -1)
            {
                string keyname = StartupScript.Substring(settings_location + "{FILE=".Length, StartupScript.IndexOf("}", settings_location) - settings_location - "{FILE=".Length);

                StreamReader streamReader = new StreamReader(System.Configuration.ConfigurationManager.AppSettings[keyname]);
                string svalue = streamReader.ReadToEnd();
                streamReader.Close();

                string s = StartupScript.Substring(0, settings_location);
                s += svalue;
                s += StartupScript.Substring(StartupScript.IndexOf("}", settings_location) + 1);
                StartupScript = s;
            }

            //Input the project downlaod scripts

            
            if (AppsToInstall != null && AppsToInstall.Length > 0)
            {
                StreamReader streamReader = new StreamReader("app_addscript");
                string appLoadScript = streamReader.ReadToEnd();
                streamReader.Close();

                string sApps = "";
                foreach (var app in AppsToInstall)
                {
                    sApps += string.Format(appLoadScript,
                                    app.AppName,
                                    app.AppLocation,
                                    app.AppUrl,
                                    app.TypeIsRails ? "/public" : "",
                                    app.DefaultServerApp ? "_" : "");
                }

                //Now insert
                int startLoc = StartupScript.IndexOf("#{INSTALL_APP_START}") + "#{INSTALL_APP_START}".Length;

                string s = StartupScript.Substring(0, startLoc) + "\n";
                s += sApps + "\n";
                s += StartupScript.Substring(startLoc);
                StartupScript = s;
            }

            // {0} = app name and directory, for example b1
            // {1} = git repository URL, for example git@github.com:zmilojko/b1.git
            // {2} = app URL, for example b1.z-ware.fi
            // {3} = for rails application, "/public", otherwise ""
            // {4} = for default application, "_", toherwise ""


            //Copy the script to the clipboard - to help debugging
            Clipboard.SetText(StartupScript);

            string NewInstanceId = "";

            lock (myController.MonitoringThreadLock)
            {

                if (MaxBid > 0)
                {


                    //For spot instances
                    var req2 = new Amazon.EC2.Model.RequestSpotInstancesRequest()
                        .WithSpotPrice(MaxBid.ToString(CultureInfo.InvariantCulture))
                        .WithLaunchSpecification(new Amazon.EC2.Model.LaunchSpecification()
                            .WithImageId(this.ResponseData.ImageId)
                            .WithInstanceType(InstanceSize)
                            .WithKeyName(KeyPair.Name)
                            .WithSecurityGroupId(SecGroup.ResponseData.GroupId)
                            .WithUserData(Convert.ToBase64String(Encoding.UTF8.GetBytes(StartupScript.Replace("\r", "")))));

                    Amazon.EC2.Model.RequestSpotInstancesResponse resp2 = myController.ec2.RequestSpotInstances(req2);

                    NewInstanceId = resp2.RequestSpotInstancesResult.SpotInstanceRequest[0].SpotInstanceRequestId;
                    myController.myTaskQueue.AddTask(new ZAwsTaskNewSpotRequestHandling(this.myController,resp2.RequestSpotInstancesResult.SpotInstanceRequest[0].SpotInstanceRequestId, Name));
                    //myController.RememberNameForSpotInstance(NewInstanceId, Name);
                }
                else
                {
                    var req = new Amazon.EC2.Model.RunInstancesRequest()
                      .WithImageId(this.ResponseData.ImageId)
                      .WithInstanceType(InstanceSize)
                      .WithKeyName(KeyPair.Name)
                      .WithSecurityGroupId(SecGroup.ResponseData.GroupId)
                      .WithMinCount(1)
                      .WithMaxCount(1)
                      .WithUserData(Convert.ToBase64String(Encoding.UTF8.GetBytes(StartupScript.Replace("\r", ""))));

                    Amazon.EC2.Model.RunInstancesResponse response = myController.ec2.RunInstances(req);

                    NewInstanceId = response.RunInstancesResult.Reservation.RunningInstance[0].InstanceId;

                    int errCounter = 0;
                    while (true)
                    {
                        try
                        {
                            Amazon.EC2.Model.CreateTagsResponse response2 = myController.ec2.CreateTags(new Amazon.EC2.Model.CreateTagsRequest()
                                                                .WithResourceId(NewInstanceId)
                                                                .WithTag(new Amazon.EC2.Model.Tag().WithKey("Name").WithValue(Name)));
                            break;
                        }
                        catch(Exception ex)
                        {
                            errCounter++;
                            if (errCounter > 2)
                            {
                                Program.TraceLine("Run Instance request sent OK, but cannot set instance name.", ex);
                                break;
                            }
                            Thread.Sleep(errCounter * 2000);
                        }
                    }

                }

                if (AppsToInstall != null && AppsToInstall.Length > 0)
                {
                    foreach (var appToInstall in AppsToInstall)
                    {
                        appToInstall.DeployedOnInstanceId = NewInstanceId;
                    }
                    myController.RegisterNewApps(AppsToInstall);
                }
            }
            return NewInstanceId;
        }

        internal class NewApp
        {
            public string AppName;
            public string AppLocation;
            public string AppUrl;
            public bool CreateUrlRecords;
            public bool DefaultServerApp;
            public bool TypeIsRails;
            public string DeployedOnInstanceId = "";
            public ZAwsEc2 DeployedOnInstance = null;
            public string AssignedIpValue = "";
            public ZAwsHostedZone AssignedToHostedZone = null;
        }

        protected override void DoDeleteObject()
        {
            Amazon.EC2.Model.DeregisterImageResponse resp = myController.ec2.DeregisterImage(
                                                            new Amazon.EC2.Model.DeregisterImageRequest()
                                                                    .WithImageId(this.ResponseData.ImageId));
        }
    }
}
