using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

using Amazon;
using Amazon.Route53;
using Amazon.Route53.Model;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;


namespace ZAws
{
    class ZAwsEc2Controller
    {
        string awsAccessKey = System.Configuration.ConfigurationManager.AppSettings["AWSAccessKey"];
        string awsSecretKey = System.Configuration.ConfigurationManager.AppSettings["AWSSecretKey"];
        string awsEc2ZoneUrl = "https://eu-west-1.ec2.amazonaws.com";
        string awsCloudWatchZoneUrl = "https://eu-west-1.monitoring.amazonaws.com/";
        string awsS3ZoneUrl = "s3.amazonaws.com";
        string awsRoute53ZoneUrl = "https://route53.amazonaws.com";

        internal AmazonEC2 ec2 {get; private set;}
        internal AmazonRoute53 route53 { get; private set; }
        internal AmazonS3 s3 { get; private set; }
        internal AmazonCloudWatch CloudWatch;

        public ZAwsEc2Controller()
        {
            ec2 = null;
            route53 = null;
            s3 = null;
        }
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
        List<ZAwsEc2> currentStatusEc2 = new List<ZAwsEc2>();
        List<ZAwsElasticIp> currentStatusElIps = new List<ZAwsElasticIp>();
        List<ZAwsHostedZone> currentHostedZones = new List<ZAwsHostedZone>();
        List<ZAwsS3> currentS3Buckets = new List<ZAwsS3>();
        List<ZAwsSecGroup> currentSecGroups = new List<ZAwsSecGroup>();
        List<ZAwsKeyPair> currentKeyPairs = new List<ZAwsKeyPair>();
        List<ZAwsSnapshot> currentSnapshots = new List<ZAwsSnapshot>();
        List<ZAwsAmi> currentAmis = new List<ZAwsAmi>();
        List<ZAwsEbsVolume> currentEbsVolumes = new List<ZAwsEbsVolume>();
        List<ZAwsSpotRequest> currentSpotRequests = new List<ZAwsSpotRequest>();

        public ZAwsEc2[] CurrentEc2s { get { return currentStatusEc2.ToArray(); } }
        public ZAwsElasticIp[] CurrentElasticIps { get { return currentStatusElIps.ToArray(); } }
        public ZAwsHostedZone[] CurrentHostedZones { get { return currentHostedZones.ToArray(); } }
        public ZAwsS3[] CurrentS3Buckets { get { return currentS3Buckets.ToArray(); } }
        public ZAwsSecGroup[] CurrentSecGroups { get { return currentSecGroups.ToArray(); } }
        public ZAwsKeyPair[] CurrentKeyPairs { get { return currentKeyPairs.ToArray(); } }
        public ZAwsSnapshot[] CurrentSnapshots { get { return currentSnapshots.ToArray(); } }
        public ZAwsAmi[] CurrentAmis { get { return currentAmis.ToArray(); } }
        public ZAwsEbsVolume[] CurrentEbsVolumes { get { return currentEbsVolumes.ToArray(); } }
        public ZAwsSpotRequest[] CurrentSpotRequests { get { return currentSpotRequests.ToArray(); } }


        public ZAwsEc2 GetEc2(string InstanceId)
        {
            foreach (var i in currentStatusEc2)
            {
                if (i.InstanceId == InstanceId)
                {
                    return i;
                }
            }
            throw new ZAwsEInstanceNotFound("Cannot find an instance with ID " + InstanceId);
        }

        public ZAwsElasticIp GetElasticIp(string PublicIp)
        {
            foreach (var z in CurrentElasticIps)
            {
                if (z.Name == PublicIp)
                {
                    return z;
                }
            }
            throw new ZAwsEInstanceNotFound("Cannot find a zone with IP " + PublicIp);
        }


        public void Connect()
        {
            if (ec2 != null)
            {
                throw new ZAwsEWrongState("Controller is already open");
            }

            Debug.Assert(route53 == null);
            Debug.Assert(s3 == null);

            Debug.Assert(!RunMonitoring);
            Debug.Assert(MonitoringThread == null);

            ec2 = AWSClientFactory.CreateAmazonEC2Client(awsAccessKey, awsSecretKey,
                        new AmazonEC2Config().WithServiceURL(awsEc2ZoneUrl));

            route53 = AWSClientFactory.CreateAmazonRoute53Client(awsAccessKey, awsSecretKey,
                            new AmazonRoute53Config() { ServiceURL = awsRoute53ZoneUrl });

            s3 = AWSClientFactory.CreateAmazonS3Client(awsAccessKey, awsSecretKey,
                            new AmazonS3Config().WithServiceURL(awsS3ZoneUrl));

            CloudWatch = AWSClientFactory.CreateAmazonCloudWatchClient(awsAccessKey, awsSecretKey,
                        new AmazonCloudWatchConfig() { ServiceURL = awsCloudWatchZoneUrl });

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
                Debug.Assert(route53 != null);
                Debug.Assert(s3 != null);
                Debug.Assert(CloudWatch != null);

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

                route53.Dispose();
                route53 = null;

                s3.Dispose();
                s3 = null;

                CloudWatch.Dispose();
                CloudWatch = null;

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
        #region Monitor query functions
        void MonitorFunction()
        {
            while (true)
            {
                //TEST ZONE
                {
                }
                //Now continues normally


                lock (Ec2Lock) { if (!RunMonitoring) { return; } }
                DescribeInstancesResponse respEc2 = GetRunningInstances();
                UpdateClassOfObjects(currentStatusEc2, respEc2.DescribeInstancesResult.Reservation);

                lock (Ec2Lock) { if (!RunMonitoring) { return; } }
                DescribeAddressesResponse respElasitIp = GetElasticIps();
                UpdateClassOfObjects(currentStatusElIps, respElasitIp.DescribeAddressesResult.Address);

                lock (Ec2Lock) { if (!RunMonitoring) { return; } }
                ListHostedZonesResponse route53Zones = GetHostedZones();
                UpdateClassOfObjects(currentHostedZones, route53Zones.ListHostedZonesResult.HostedZones);

                lock (Ec2Lock) { if (!RunMonitoring) { return; } }
                ListBucketsResponse s3Buckects = GetBuckets();
                UpdateClassOfObjects(currentS3Buckets, s3Buckects.Buckets);

                lock (Ec2Lock) { if (!RunMonitoring) { return; } }
                DescribeSnapshotsResponse respEc2Snapshots = GetSnapshots();
                UpdateClassOfObjects(currentSnapshots, respEc2Snapshots.DescribeSnapshotsResult.Snapshot);

                lock (Ec2Lock) { if (!RunMonitoring) { return; } }
                DescribeKeyPairsResponse respKeyPairs = GetKeyPairs();
                UpdateClassOfObjects(currentKeyPairs, respKeyPairs.DescribeKeyPairsResult.KeyPair);

                lock (Ec2Lock) { if (!RunMonitoring) { return; } }
                DescribeSecurityGroupsResponse respSecGroups = GetSecurityGroups();
                UpdateClassOfObjects(currentSecGroups, respSecGroups.DescribeSecurityGroupsResult.SecurityGroup);

                lock (Ec2Lock) { if (!RunMonitoring) { return; } }
                DescribeImagesResponse respAmis = GetAmis();
                UpdateClassOfObjects(currentAmis, respAmis.DescribeImagesResult.Image);

                lock (Ec2Lock) { if (!RunMonitoring) { return; } }
                DescribeVolumesResponse respEbsVolumes = GetEbsVolumes();
                UpdateClassOfObjects(currentEbsVolumes, respEbsVolumes.DescribeVolumesResult.Volume);
                
                lock (Ec2Lock) { if (!RunMonitoring) { return; } }
                DescribeSpotInstanceRequestsResponse respSpotRequests = GetSpotRequests();
                UpdateClassOfObjects(currentSpotRequests, respSpotRequests.DescribeSpotInstanceRequestsResult.SpotInstanceRequest);


                foreach (ZAwsEc2 ec2Instance in CurrentEc2s)
                {
                    lock (Ec2Lock) { if (!RunMonitoring) { return; } }
                    ec2Instance.UpdateInfo();
                }

                foreach (ZAwsHostedZone zone in CurrentHostedZones)
                {
                    lock (Ec2Lock) { if (!RunMonitoring) { return; } }
                    zone.UpdateInfo();
                }

                lock (Ec2Lock) { if (!RunMonitoring) { return; } }

                //Give ther threads a chance, and also allow user to smoothly disconnect
                Thread.Sleep(200);
            }
        }
        void UpdateClassOfObjects<T, U>(List<T> ListToUpdate, List<U> ListOfResponses) where T : ZAwsObject
        {
            //UpdateClassOfObjects(currentStatus, resp.DescribeInstancesResult.Reservation);
            foreach (U res in ListOfResponses)
            {
                bool found = false;
                foreach (T oldI in ListToUpdate)
                {
                    if (oldI.EqualsData(res))
                    {
                        oldI.Update(res);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    T NewObj = (T)Activator.CreateInstance(typeof(T), this, res);
                    ListToUpdate.Add(NewObj);
                    if (NewObject != null)
                    {
                        NewObject(this, new ZAwsNewObjectEventArgs(NewObj));
                    }
                }
            }

            //Now do check for terminated ones!
            List<T> toDeleteList = new List<T>();
            foreach (T oldI in ListToUpdate)
            {
                bool found = false;
                foreach (U res in ListOfResponses)
                {
                    if (oldI.EqualsData(res))
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
            foreach (T oldToDelete in toDeleteList)
            {
                ListToUpdate.Remove(oldToDelete);
                oldToDelete.Delete();
            }
        }
        private DescribeInstancesResponse GetRunningInstances()
        {
            DescribeInstancesRequest request = new DescribeInstancesRequest();
            DescribeInstancesResponse resp = ec2.DescribeInstances(request);
            return resp;
        }
        private DescribeAddressesResponse GetElasticIps()
        {
            DescribeAddressesRequest request = new DescribeAddressesRequest();
            DescribeAddressesResponse resp = ec2.DescribeAddresses(request);
            return resp;
        }
        private ListHostedZonesResponse GetHostedZones()
        {
            ListHostedZonesResponse resp = route53.ListHostedZones();
            return resp;
        }
        private ListBucketsResponse GetBuckets()
        {
            ListBucketsResponse resp = s3.ListBuckets();
            return resp;
        }
        private DescribeSecurityGroupsResponse GetSecurityGroups()
        {
            DescribeSecurityGroupsRequest request = new DescribeSecurityGroupsRequest();
            DescribeSecurityGroupsResponse resp = ec2.DescribeSecurityGroups(request);
            return resp;
        }
        private DescribeSnapshotsResponse GetSnapshots()
        {
            DescribeSnapshotsRequest request = new DescribeSnapshotsRequest()
                            .WithOwner("self");
            DescribeSnapshotsResponse resp = ec2.DescribeSnapshots(request);
            return resp;
        }
        private DescribeKeyPairsResponse GetKeyPairs()
        {
            DescribeKeyPairsRequest request = new DescribeKeyPairsRequest();
            DescribeKeyPairsResponse resp = ec2.DescribeKeyPairs(request);
            return resp;
        }
        private DescribeVolumesResponse GetEbsVolumes()
        {
            DescribeVolumesRequest request = new DescribeVolumesRequest();
            DescribeVolumesResponse resp = ec2.DescribeVolumes(request);
            return resp;
        }
        private DescribeImagesResponse GetAmis()
        {
            DescribeImagesRequest request = new DescribeImagesRequest()
                .WithOwner("self");
            DescribeImagesResponse resp = ec2.DescribeImages(request);
            return resp;
        }
        private DescribeSpotInstanceRequestsResponse GetSpotRequests()
        {
            DescribeSpotInstanceRequestsRequest request = new DescribeSpotInstanceRequestsRequest();
            DescribeSpotInstanceRequestsResponse resp = ec2.DescribeSpotInstanceRequests(request);
            return resp;
        }
        
        #endregion

        internal string AllocateIp()
        {
            AllocateAddressResponse resp = ec2.AllocateAddress(new AllocateAddressRequest());
            return resp.AllocateAddressResult.PublicIp;
        }

        internal void CreatedHostedZone(string p)
        {
            CreateHostedZoneResponse resp = route53.CreateHostedZone(new CreateHostedZoneRequest()
                                                                            .WithName(p)
                                                                            .WithCallerReference("zawscc" + new Random().Next(1000).ToString())
                                                                            );
        }

        List<ZAwsAmi.NewApp> runningApps = new List<ZAwsAmi.NewApp>();

        internal void RegisterNewApps(ZAwsAmi.NewApp[] NewInstalledApps)
        {
            runningApps.AddRange(NewInstalledApps);
        }

        internal void HandleNewEc2Instance(ZAwsEc2 newEc2)
        {
            //Check if you should set a name to yourself
            foreach (var n in NamesForSpotInstance)
            {
                if (n.NewInstanceId == newEc2.InstanceId)
                {
                    newEc2.SetName(n.NewInstanceName);
                }
            }

            //Now check if additional action is needed with this EC2
            foreach (var app in runningApps)
            {
                if (app.DeployedOnInstanceId == newEc2.InstanceId)
                {
                    app.DeployedOnInstance = newEc2;
                    ConfigureAppIfNeeded(app);
                }
            }
        }

        internal void HandleNewElasticIp(ZAwsElasticIp zAwsElasticIp)
        {
            foreach (var app in runningApps)
            {
                if ((!zAwsElasticIp.Associated) && (app.AssignedIpValue == zAwsElasticIp.Name) && (app.DeployedOnInstance != null))
                {
                    zAwsElasticIp.Associate(app.DeployedOnInstance);
                }
            }
        }

        private void ConfigureAppIfNeeded(ZAwsAmi.NewApp app)
        {
            if (!app.CreateUrlRecords)
            {
                return;
            }

            //IP assignment
            if (app.DeployedOnInstance != null && string.IsNullOrWhiteSpace(app.AssignedIpValue))
            {
                //First check if some IP has already been associated with another app running on the same instance - this is the case
                // when more then one app is downloaded to that instance
                foreach (var app2 in runningApps)
                {
                    if (app2.DeployedOnInstanceId == app.DeployedOnInstanceId && !string.IsNullOrWhiteSpace(app2.AssignedIpValue))
                    {
                        app.AssignedIpValue = app2.AssignedIpValue;
                        //Call the same function to assign the URL records
                        ConfigureAppIfNeeded(app);
                        return;
                    }
                }

                //Try to find one unassigned IP, and assign it
                foreach (var ip in CurrentElasticIps)
                {
                    if (!ip.Associated)
                    {
                        ip.Associate(app.DeployedOnInstance);
                        app.AssignedIpValue = ip.Name;
                        //Call the same function to assign the URL records
                        ConfigureAppIfNeeded(app);
                        return;
                    }
                }
                //There was no unassigned IPs, create a new one
                app.AssignedIpValue = AllocateIp();
            }

            if (!string.IsNullOrWhiteSpace(app.AssignedIpValue) && app.AssignedToHostedZone == null)
            {
                //Now try to find the hosted zone
                foreach (var zone in currentHostedZones)
                {
                    //Name of the hosted zone must match the ending of the app URL. 
                    if (app.AppUrl.TrimEnd('.').IndexOf(zone.Name.TrimEnd('.')) == app.AppUrl.TrimEnd('.').Length - zone.Name.TrimEnd('.').Length)
                    {
                        //Now add the record
                        zone.AddRecord(new Amazon.Route53.Model.ResourceRecordSet()
                                                            .WithName(app.AppUrl)
                                                            .WithType("A")
                                                            .WithTTL(800)
                                                            .WithResourceRecords(new ResourceRecord()
                                                                                        .WithValue(app.AssignedIpValue)));

                    }
                }
            }
        }

        class NameForSpotInstance
        {
            public string SpotInstanceRequestId;
            public string NewInstanceName;
            public string NewInstanceId = "";
        }
        List<NameForSpotInstance> NamesForSpotInstance = new List<NameForSpotInstance>();

        internal void RememberNameForSpotInstance(string spotInstanceRequestId, string newInstanceName)
        {
            NamesForSpotInstance.Add(new NameForSpotInstance()
                                        {
                                            NewInstanceName = newInstanceName,
                                            SpotInstanceRequestId = spotInstanceRequestId
                                        });
        }

        internal void AssignInstanceToSpotRequest(string SpotInstanceRequestId,string InstanceId)
        {
            foreach (var n in NamesForSpotInstance)
            {
                if (n.SpotInstanceRequestId == SpotInstanceRequestId)
                {
                    n.NewInstanceId = InstanceId;
                }
            }

            foreach (var app in this.runningApps)
            {
                if (app.DeployedOnInstanceId == SpotInstanceRequestId)
                {
                    app.DeployedOnInstanceId = InstanceId;
                    
                }
            }
            //now check if that instance is already there, and register it
            foreach (var i in CurrentEc2s)
            {
                if (i.InstanceId == InstanceId)
                {
                    HandleNewEc2Instance(i);
                }
            }
        }
    }
}
