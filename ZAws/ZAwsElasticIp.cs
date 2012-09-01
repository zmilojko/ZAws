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
using Amazon.EC2.Model;

namespace ZAws
{
    class ZAwsElasticIp : ZAwsObject
    {
        public ZAwsElasticIp(ZAwsEc2Controller controller, Amazon.EC2.Model.Address res)
            : base(controller)
        {
            Update(res);

            //myController.HandleNewElasticIp(this);
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

        
        public bool Associated
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ResponseData.InstanceId);
            }
        }

        public ZAwsEc2 AssociatedEc2
        {
            get
            {
                return myController.GetEc2(ResponseData.InstanceId);
            }
        }

        protected override void DoDeleteObject()
        {
            //Check if associated
            if (Associated)
            {
                throw new Exception("Cannot delete IP when associated. Disassociate first.");
            }

            foreach (ZAwsHostedZone zone in myController.CurrentHostedZones)
            {
                foreach (var r in zone.currentRecordSet)
                {
                    foreach (var t in r.ResourceRecords)
                    {
                        if (t.Value.Contains(this.Name))
                        {
                            throw new Exception("This IP features in a record of the hosted zone " + zone.Name + ". Remove that record first. ");
                        }
                    }
                }
            }


            Amazon.EC2.Model.ReleaseAddressResponse resp = myController.ec2.ReleaseAddress(new Amazon.EC2.Model.ReleaseAddressRequest()
                                                                    .WithPublicIp(this.ResponseData.PublicIp));
        }

        internal void Associate(ZAwsEc2 ec2)
        {
            Amazon.EC2.Model.AssociateAddressResponse resp = myController.ec2.AssociateAddress(new AssociateAddressRequest()
                                                .WithPublicIp(this.Name)
                                                .WithInstanceId(ec2.InstanceId));
        }

        internal void Disassociate()
        {
            Amazon.EC2.Model.DisassociateAddressResponse resp = myController.ec2.DisassociateAddress(new DisassociateAddressRequest()
                                                .WithPublicIp(this.Name));
        }
    }
}
