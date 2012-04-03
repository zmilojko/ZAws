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
