using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ZAws
{
    class ZAwsEbsVolume : ZAwsObject
    {
        public Amazon.EC2.Model.Volume ResponseData { get; private set; }

        public ZAwsEbsVolume(ZAwsEc2Controller controller, Amazon.EC2.Model.Volume res)
            : base(controller)
        {
            Update(res);
        }

        public override string Name
        {
            get
            {
                return ResponseData.VolumeId;
            }
        }

        protected override bool DoUpdate(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.Volume), "Wrong data passed to the object for update.");
            ResponseData = (Amazon.EC2.Model.Volume)responseData;
            return true;
        }

        internal override bool EqualsData(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.Volume), "Wrong data passed to the object for update.");
            return string.Equals(ResponseData.VolumeId, ((Amazon.EC2.Model.Volume)responseData).VolumeId);
        }

        protected override void DoDeleteObject()
        {
            Amazon.EC2.Model.DeleteVolumeResponse resp = myController.ec2.DeleteVolume(new Amazon.EC2.Model.DeleteVolumeRequest()
                                                                    .WithVolumeId(this.ResponseData.VolumeId));
        }
    }
}
