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
using System.IO;
using System.Linq;
using System.Text;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Amazon.S3;
using Amazon.S3.Model;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;

namespace ZAws.Console
{
    class Program
    {
        static RegistryKey appKey = null;
        public static RegistryKey AppRegKey
        {
            get
            {
                if (appKey != null)
                {
                    return appKey;
                }
                else
                {
                    appKey = Registry.CurrentUser.CreateSubKey(@"Software\ZWare\Zawscc");
                    return appKey;
                }
            }
        }

        public static string MonitorMessage
        {
            set
            {
                theMainView.SetMonitorMessage(value);
            }
        }


        public static Tracer Tracer = new Tracer();
        public static void TraceLine(string line, params object[] line_parameters)
        {
            Tracer.TraceLine(line, line_parameters);
        }
        public static void TraceLine(string line, Exception ex, params object[] line_parameters)
        {
            Tracer.TraceLine(line, ex, line_parameters);
        }

        static MainView theMainView;

        [STAThread]
        public static void Main(string[] args)
        {
            Stream TraceListenerStream = new FileStream(@"c:\zawscc.log", FileMode.Append, FileAccess.Write, FileShare.Delete | FileShare.ReadWrite | FileShare.Inheritable);
            Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(TraceListenerStream));
            Trace.AutoFlush = true;
            Trace.WriteLine("Trace file: " + @"c:\zawscc.log");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(theMainView = new ZAws.Console.MainView());
        }

        static void c_ReceivedString(object sender, ZawsSshClient.ReceivedStringEventArgs e)
        {
            Trace.WriteLine(e.ReceivedString);
        }

        public static string GetServiceOutput()
        {
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                sr.WriteLine("===========================================");
                sr.WriteLine("Welcome to the AWS .NET SDK!");
                sr.WriteLine("===========================================");

                
                // Print the number of Amazon EC2 instances.
                AmazonEC2 ec2 = AWSClientFactory.CreateAmazonEC2Client(
                        System.Configuration.ConfigurationManager.AppSettings["AWSAccessKey"],
                        System.Configuration.ConfigurationManager.AppSettings["AWSSecretKey"],
                        new AmazonEC2Config().WithServiceURL("https://eu-west-1.ec2.amazonaws.com"));
                
                
                
                DescribeInstancesRequest ec2Request = new DescribeInstancesRequest();
                

                try
                {
                    DescribeInstancesResponse ec2Response = ec2.DescribeInstances(ec2Request);
                    int numInstances = 0;
                    numInstances = ec2Response.DescribeInstancesResult.Reservation.Count;
                    sr.WriteLine("You have " + numInstances + " Amazon EC2 instance(s) running in the US-East (Northern Virginia) region.");

                }
                catch (AmazonEC2Exception ex)
                {
                    if (ex.ErrorCode != null && ex.ErrorCode.Equals("AuthFailure"))
                    {
                        sr.WriteLine("The account you are using is not signed up for Amazon EC2.");
                        sr.WriteLine("You can sign up for Amazon EC2 at http://aws.amazon.com/ec2");
                    }
                    else
                    {
                        sr.WriteLine("Caught Exception: " + ex.Message);
                        sr.WriteLine("Response Status Code: " + ex.StatusCode);
                        sr.WriteLine("Error Code: " + ex.ErrorCode);
                        sr.WriteLine("Error Type: " + ex.ErrorType);
                        sr.WriteLine("Request ID: " + ex.RequestId);
                        sr.WriteLine("XML: " + ex.XML);
                    }
                }
                sr.WriteLine();

                // Print the number of Amazon SimpleDB domains.
                AmazonSimpleDB sdb = AWSClientFactory.CreateAmazonSimpleDBClient();
                ListDomainsRequest sdbRequest = new ListDomainsRequest();

                try
                {
                    ListDomainsResponse sdbResponse = sdb.ListDomains(sdbRequest);

                    if (sdbResponse.IsSetListDomainsResult())
                    {
                        int numDomains = 0;
                        numDomains = sdbResponse.ListDomainsResult.DomainName.Count;
                        sr.WriteLine("You have " + numDomains + " Amazon SimpleDB domain(s) in the US-East (Northern Virginia) region.");
                    }
                }
                catch (AmazonSimpleDBException ex)
                {
                    if (ex.ErrorCode != null && ex.ErrorCode.Equals("AuthFailure"))
                    {
                        sr.WriteLine("The account you are using is not signed up for Amazon SimpleDB.");
                        sr.WriteLine("You can sign up for Amazon SimpleDB at http://aws.amazon.com/simpledb");
                    }
                    else
                    {
                        sr.WriteLine("Caught Exception: " + ex.Message);
                        sr.WriteLine("Response Status Code: " + ex.StatusCode);
                        sr.WriteLine("Error Code: " + ex.ErrorCode);
                        sr.WriteLine("Error Type: " + ex.ErrorType);
                        sr.WriteLine("Request ID: " + ex.RequestId);
                        sr.WriteLine("XML: " + ex.XML);
                    }
                }
                sr.WriteLine();

                // Print the number of Amazon S3 Buckets.
                AmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client();

                try
                {
                    ListBucketsResponse response = s3Client.ListBuckets();
                    int numBuckets = 0;
                    if (response.Buckets != null &&
                        response.Buckets.Count > 0)
                    {
                        numBuckets = response.Buckets.Count;
                    }
                    sr.WriteLine("You have " + numBuckets + " Amazon S3 bucket(s) in the US Standard region.");
                }
                catch (AmazonS3Exception ex)
                {
                    if (ex.ErrorCode != null && (ex.ErrorCode.Equals("InvalidAccessKeyId") ||
                        ex.ErrorCode.Equals("InvalidSecurity")))
                    {
                        sr.WriteLine("Please check the provided AWS Credentials.");
                        sr.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                    }
                    else
                    {
                        sr.WriteLine("Caught Exception: " + ex.Message);
                        sr.WriteLine("Response Status Code: " + ex.StatusCode);
                        sr.WriteLine("Error Code: " + ex.ErrorCode);
                        sr.WriteLine("Request ID: " + ex.RequestId);
                        sr.WriteLine("XML: " + ex.XML);
                    }
                }
                sr.WriteLine("Press any key to continue...");
            }
            return sb.ToString();
        }

        internal static void OpenWebBrowser(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        static List<ZAwsPopupForm> activePopupForms = new List<ZAwsPopupForm>();
        
        internal static void LaunchPopupForm<F>(ZAwsObject obj) where F : ZAwsPopupForm
        {
            foreach (var f in activePopupForms)
            {
                if (f.GetType() == typeof(F) && f.ServedType == obj.GetType() && obj == f.MyObj)
                {
                    f.Select();
                    f.Focus();
                    return;
                }
            }

            F newPopup = (F)Activator.CreateInstance(typeof(F), obj);
            newPopup.Show();
            activePopupForms.Add(newPopup);
        }
        internal static void UnrgisterPopupForm(ZAwsPopupForm zAwsPopupForm)
        {
            foreach (var f in activePopupForms)
            {
                if (f == zAwsPopupForm)
                {
                    activePopupForms.Remove(f);
                    return;
                }
            }
        }
    }
}