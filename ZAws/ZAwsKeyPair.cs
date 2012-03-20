using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ZAws
{
    class ZAwsKeyPair : ZAwsObject
    {
        public Amazon.EC2.Model.KeyPair ResponseData { get; private set; }

        public ZAwsKeyPair(ZAwsEc2Controller controller, Amazon.EC2.Model.KeyPair res)
            : base(controller)
        {
            Update(res);
        }

        public override string Name
        {
            get
            {
                return ResponseData.KeyName;
            }
        }

        protected override bool DoUpdate(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.KeyPair), "Wrong data passed to the object for update.");
            ResponseData = (Amazon.EC2.Model.KeyPair)responseData;
            return true;
        }

        internal override bool EqualsData(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.KeyPair), "Wrong data passed to the object for update.");
            return string.Equals(ResponseData.KeyFingerprint, ((Amazon.EC2.Model.KeyPair)responseData).KeyFingerprint);
        }

        protected override void DoDeleteObject()
        {
            Amazon.EC2.Model.DeleteKeyPairResponse resp = myController.ec2.DeleteKeyPair(new Amazon.EC2.Model.DeleteKeyPairRequest()
                                                                    .WithKeyName(this.ResponseData.KeyName));
        }
    }
}
