using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ZAws
{
    class ZAwsSecGroup : ZAwsObject
    {
        public Amazon.EC2.Model.SecurityGroup ResponseData { get; private set; }

        public ZAwsSecGroup(ZAwsEc2Controller controller, Amazon.EC2.Model.SecurityGroup res)
            : base(controller)
        {
            Update(res);
        }

        public override string Name
        {
            get
            {
                return ResponseData.GroupName;
            }
        }

        protected override bool DoUpdate(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.SecurityGroup), "Wrong data passed to the object for update.");
            ResponseData = (Amazon.EC2.Model.SecurityGroup)responseData;
            return true;
        }

        internal override bool EqualsData(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.SecurityGroup), "Wrong data passed to the object for update.");
            return string.Equals(ResponseData.GroupId, ((Amazon.EC2.Model.SecurityGroup)responseData).GroupId);
        }
    }
}
