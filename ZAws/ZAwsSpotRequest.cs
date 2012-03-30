using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ZAws
{
    class ZAwsSpotRequest : ZAwsObject
    {
        public Amazon.EC2.Model.SpotInstanceRequest ResponseData { get; private set; }

        public ZAwsSpotRequest(ZAwsEc2Controller controller, Amazon.EC2.Model.SpotInstanceRequest res)
            : base(controller)
        {
            Update(res);
        }

        public override string Name
        {
            get
            {
                return ResponseData.SpotInstanceRequestId;
            }
        }

        public string InstanceId
        {
            get
            {
                return ResponseData.InstanceId;
            }
        }

        

        protected override bool DoUpdate(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.SpotInstanceRequest), "Wrong data passed to the object for update.");
            if(!string.IsNullOrWhiteSpace(((Amazon.EC2.Model.SpotInstanceRequest)responseData).InstanceId)
                &&(ResponseData == null
                        || ResponseData.InstanceId != ((Amazon.EC2.Model.SpotInstanceRequest)responseData).InstanceId))
            {
                myController.AssignInstanceToSpotRequest(((Amazon.EC2.Model.SpotInstanceRequest)responseData).SpotInstanceRequestId,
                     ((Amazon.EC2.Model.SpotInstanceRequest)responseData).InstanceId);
            }
            ResponseData = (Amazon.EC2.Model.SpotInstanceRequest)responseData;

            

            return true;
        }

        internal override bool EqualsData(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.SpotInstanceRequest), "Wrong data passed to the object for update.");
            return (string.Equals(Name, ((Amazon.EC2.Model.SpotInstanceRequest)responseData).SpotInstanceRequestId));
        }

        protected override void DoDeleteObject()
        {
            Amazon.EC2.Model.CancelSpotInstanceRequestsResponse resp = myController.ec2.CancelSpotInstanceRequests(new Amazon.EC2.Model.CancelSpotInstanceRequestsRequest()
                                                                    .WithSpotInstanceRequestId(Name));
        }
    }
}
