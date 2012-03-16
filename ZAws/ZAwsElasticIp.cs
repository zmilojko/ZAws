using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Amazon.EC2.Model;

namespace ZAws
{
    class ZAwsElasticIp : ZAwsObject
    {
        public ZAwsElasticIp(ZAwsEc2Controller controller, Amazon.EC2.Model.Address res)
            : base(controller)
        {
            Update(res);
        }
        Amazon.EC2.Model.Address ResponseData;

        public override string Name
        {
            get { return ResponseData.PublicIp; }
        }

        protected override bool DoUpdate(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Address), "Wrong data passed to the object for update.");
            ResponseData = (Address)responseData;
            return true;
        }

        internal override bool EqualsData(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Address), "Wrong data passed to the object for update.");
            return string.Equals(((Address)responseData).PublicIp, ResponseData.PublicIp);
        }
    }
}
