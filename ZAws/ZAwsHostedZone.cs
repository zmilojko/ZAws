using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ZAws
{
    class ZAwsHostedZone : ZAwsObject
    {
        public Amazon.Route53.Model.HostedZone ResponseData { get; private set; }

        public ZAwsHostedZone(ZAwsEc2Controller controller, Amazon.Route53.Model.HostedZone res)
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
            Debug.Assert(responseData.GetType() == typeof(Amazon.Route53.Model.HostedZone), "Wrong data passed to the object for update.");
            ResponseData = (Amazon.Route53.Model.HostedZone)responseData;
            return true;
        }

        internal override bool EqualsData(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.Route53.Model.HostedZone), "Wrong data passed to the object for update.");
            return string.Equals(Name, ((Amazon.Route53.Model.HostedZone)responseData).Name);
        }

        protected override void DoDeleteObject()
        {
            Amazon.Route53.Model.DeleteHostedZoneResponse resp = myController.route53.DeleteHostedZone(
                                                new Amazon.Route53.Model.DeleteHostedZoneRequest()
                                                                    .WithId(this.ResponseData.Id));
        }
    }
}
