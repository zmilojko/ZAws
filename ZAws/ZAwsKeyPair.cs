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
    class ZAwsKeyPair : ZAwsObject
    {
        public Amazon.EC2.Model.KeyPair ResponseData { get; private set; }

        public ZAwsKeyPair(ZAwsEc2Controller controller, Amazon.EC2.Model.KeyPair res)
            : base(controller)
        {
            Update(res);
        }

        public override string Name
        {
            get
            {
                return ResponseData.KeyName;
            }
        }

        protected override bool DoUpdate(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.KeyPair), "Wrong data passed to the object for update.");
            ResponseData = (Amazon.EC2.Model.KeyPair)responseData;
            return true;
        }

        internal override bool EqualsData(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.EC2.Model.KeyPair), "Wrong data passed to the object for update.");
            return string.Equals(ResponseData.KeyFingerprint, ((Amazon.EC2.Model.KeyPair)responseData).KeyFingerprint);
        }

        public bool Available
        {
            get
            {
                string awsKeyPath = System.Configuration.ConfigurationManager.AppSettings["SSHPrivateKeysDir"];
                return System.IO.File.Exists(awsKeyPath + this.Name + ".pem")
                    && System.IO.File.Exists(awsKeyPath + this.Name + ".ssh2")
                    && System.IO.File.Exists(awsKeyPath + this.Name + ".ppk");
            }
        }


        protected override void DoDeleteObject()
        {
            Amazon.EC2.Model.DeleteKeyPairResponse resp = myController.ec2.DeleteKeyPair(new Amazon.EC2.Model.DeleteKeyPairRequest()
                                                                    .WithKeyName(this.ResponseData.KeyName));
        }
    }
}
