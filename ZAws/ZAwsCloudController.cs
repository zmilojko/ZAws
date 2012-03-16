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

        public void Connect()
        {
            if (ec2 != null)
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
                lock (Ec2Lock)
                {
                    if (!RunMonitoring)
                    {
                        return;
                    }
                }
                DescribeInstancesResponse respEc2 = GetRunningInstances();
                DescribeAddressesResponse respElasitIp = GetElasticIps();
                lock (Ec2Lock)
                {
                    if (!RunMonitoring)
                    {
                        return;
                    }
                }

                UpdateClassOfObjects(currentStatusEc2, respEc2.DescribeInstancesResult.Reservation);
                UpdateClassOfObjects(currentStatusElIps, respElasitIp.DescribeAddressesResult.Address);

                //Give ther threads a chance, and also allow user to smoothly disconnect
                Thread.Sleep(200);
            }
        }



        private DescribeInstancesResponse GetRunningInstances()
        {
            DescribeInstancesRequest ec2Request = new DescribeInstancesRequest();
            DescribeInstancesResponse ec2Response = ec2.DescribeInstances(ec2Request);
            return ec2Response;
        }
        private DescribeAddressesResponse GetElasticIps()
        {
            DescribeAddressesRequest ec2Request = new DescribeAddressesRequest();
            DescribeAddressesResponse ec2Response = ec2.DescribeAddresses(ec2Request);
            return ec2Response;
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
                return null;
            }
        };
        ZAwsObjectFactory ObjectFactory;
    }
}
