using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ZAws
{
    class ZAwsSnapshot : ZAwsObject
    {
        public Amazon.EC2.Model.Snapshot ResponseData { get; private set; }

        public ZAwsSnapshot(ZAwsEc2Controller controller, Amazon.EC2.Model.Snapshot res)
            : base(controller)
        {
            Update(res);
        }

        public override string Name
        {
            get
            {
                return ResponseData.SnapshotId;
            }
        }

        protected override bool DoUpdate(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.Snapshot), "Wrong data passed to the object for update.");
            ResponseData = (Amazon.EC2.Model.Snapshot)responseData;
            return true;
        }

        internal override bool EqualsData(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.Snapshot), "Wrong data passed to the object for update.");
            return string.Equals(Name, ((Amazon.EC2.Model.Snapshot)responseData).SnapshotId);
        }

        protected override void DoDeleteObject()
        {
            Amazon.EC2.Model.DeleteSnapshotResponse resp = myController.ec2.DeleteSnapshot(new Amazon.EC2.Model.DeleteSnapshotRequest()
                                                                    .WithSnapshotId(this.ResponseData.SnapshotId));
        }
    }
}
