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


namespace ZAws
{
    class ZAwsEc2Controller
    {
        string awsAccessKey = System.Configuration.ConfigurationManager.AppSettings["AWSAccessKey"];
        string awsSecretKey = System.Configuration.ConfigurationManager.AppSettings["AWSSecretKey"];
        string awsEc2ZoneUrl = "https://eu-west-1.ec2.amazonaws.com";

        AmazonEC2 ec2 = null;
        AmazonRoute53 route53 = null;
        AmazonS3 s3 = null;

        public ZAwsEc2Controller()
        {
            ObjectFactory = new ZAwsObjectFactory() { ZAwsEc2Controller = this };
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

            route53 = AWSClientFactory.CreateAmazonRoute53Client(awsAccessKey, awsSecretKey);

            s3 = AWSClientFactory.CreateAmazonS3Client(awsAccessKey, awsSecretKey);

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
                lock (Ec2Lock)
                {
                    if (!RunMonitoring)
                    {
                        return;
                    }
                }
                DescribeInstancesResponse respEc2 = GetRunningInstances();
                DescribeAddressesResponse respElasitIp = GetElasticIps();
                ListHostedZonesResponse route53Zones = GetHostedZones();
                ListBucketsResponse s3Buckects = GetBuckets();
                lock (Ec2Lock)
                {
                    if (!RunMonitoring)
                    {
                        return;
                    }
                }

                UpdateClassOfObjects(currentStatusEc2, respEc2.DescribeInstancesResult.Reservation);
                UpdateClassOfObjects(currentStatusElIps, respElasitIp.DescribeAddressesResult.Address);
                UpdateClassOfObjects(currentHostedZones, route53Zones.ListHostedZonesResult.HostedZones);
                UpdateClassOfObjects(currentS3Buckets, s3Buckects.Buckets);

                //Give ther threads a chance, and also allow user to smoothly disconnect
                Thread.Sleep(200);
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
            ListHostedZonesRequest request = new ListHostedZonesRequest();
            ListHostedZonesResponse resp = route53.ListHostedZones();
            return resp;
        }
        private ListBucketsResponse GetBuckets()
        {
            ListBucketsRequest request = new ListBucketsRequest();
            ListBucketsResponse resp = s3.ListBuckets();
            return resp;
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
                    T NewObj = (T)ObjectFactory.CreateZawsObject(typeof(T), res);
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


        /*
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
        }*/

        class ZAwsObjectFactory
        {
            public ZAwsEc2Controller ZAwsEc2Controller;
            public ZAwsObject CreateZawsObject(Type ZAwsType, Object ResponseData)
            {
                if (ZAwsType == typeof(ZAwsEc2))
                {
                    Debug.Assert(ResponseData.GetType() == typeof(Reservation), "Wrong data passed to the object factory.");
                    return new ZAwsEc2(ZAwsEc2Controller, (Reservation)ResponseData);
                }
                if (ZAwsType == typeof(ZAwsElasticIp))
                {
                    Debug.Assert(ResponseData.GetType() == typeof(Address), "Wrong data passed to the object factory.");
                    return new ZAwsElasticIp(ZAwsEc2Controller, (Address)ResponseData);
                } 
                if (ZAwsType == typeof(ZAwsHostedZone))
                {
                    Debug.Assert(ResponseData.GetType() == typeof(HostedZone), "Wrong data passed to the object factory.");
                    return new ZAwsHostedZone(ZAwsEc2Controller, (HostedZone)ResponseData);
                } 
                if (ZAwsType == typeof(ZAwsS3))
                {
                    Debug.Assert(ResponseData.GetType() == typeof(S3Bucket), "Wrong data passed to the object factory.");
                    return new ZAwsS3(ZAwsEc2Controller, (S3Bucket)ResponseData);
                }
                Debug.Assert(false);
                throw new ArgumentException("Unknown ZAWS Object class: " + ResponseData.GetType().ToString());
            }
        };
        ZAwsObjectFactory ObjectFactory;
    }
}
