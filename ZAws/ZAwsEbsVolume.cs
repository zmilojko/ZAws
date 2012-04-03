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
