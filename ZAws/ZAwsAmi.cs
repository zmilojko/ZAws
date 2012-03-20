using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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

        internal void Launch(ZAwsSecGroup SecGroup, ZAwsKeyPair KeyPair)
        {
            Amazon.EC2.Model.RunInstancesResponse response = myController.ec2.RunInstances(new Amazon.EC2.Model.RunInstancesRequest()
              .WithImageId(this.ResponseData.ImageId)
              .WithInstanceType("t1.micro")
              .WithKeyName(KeyPair.Name)
              .WithSecurityGroupId(SecGroup.ResponseData.GroupId)
              .WithMinCount(1)
              .WithMaxCount(1)
              .WithUserData(Convert.ToBase64String(Encoding.UTF8.GetBytes("ls".Replace("\r", ""))))
            );

        }

        protected override void DoDeleteObject()
        {
            Amazon.EC2.Model.DeregisterImageResponse resp = myController.ec2.DeregisterImage(
                                                            new Amazon.EC2.Model.DeregisterImageRequest()
                                                                    .WithImageId(this.ResponseData.ImageId));
        }
    }
}
