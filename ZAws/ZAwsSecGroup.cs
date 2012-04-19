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

        protected override void DoDeleteObject()
        {
            Amazon.EC2.Model.DeleteSecurityGroupResponse resp = myController.ec2.DeleteSecurityGroup(
                                                        new Amazon.EC2.Model.DeleteSecurityGroupRequest()
                                                                    .WithGroupId(this.ResponseData.GroupId));
        }

        public override string Description
        {
            get
            {
                string s = "";
                string p = "";
                string r = "";
                foreach (var i in this.ResponseData.IpPermission)
                {
                    string portnumber = ((int)i.FromPort).ToString();
                    if(i.FromPort != i.ToPort)
                    {
                        portnumber += "-" + ((int)i.ToPort).ToString();
                    }
                    if(i.IpRange.Count != 1 || i.IpRange[0] != "0.0.0.0/0")
                    {
                        portnumber = "(" + portnumber + ")";
                    }
                    switch (i.IpProtocol)
                    {
                        case "tcp": if (s.Length == 0) { s = "tcp: "; } else { s += ", "; } s += portnumber; break;
                        case "udp": if (p.Length == 0) { p = "\nudp: "; } else { p += ", "; } p += portnumber; break;
                        case "icmp": if (r.Length == 0) { r = "\nicmp: "; } else { r += ", "; } r += portnumber; break;
                    }
                }
                return s + p;
            }
        }
    }
}
