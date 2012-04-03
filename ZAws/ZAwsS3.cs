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
    class ZAwsS3 : ZAwsObject
    {
        public Amazon.S3.Model.S3Bucket ResponseData { get; private set; }

        public ZAwsS3(ZAwsEc2Controller controller, Amazon.S3.Model.S3Bucket res)
            : base(controller)
        {
            Update(res);
        }

        public override string Name
        {
            get
            {
                return ResponseData.BucketName;
            }
        }

        protected override bool DoUpdate(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.S3.Model.S3Bucket), "Wrong data passed to the object for update.");
            ResponseData = (Amazon.S3.Model.S3Bucket)responseData;
            return true;
        }

        internal override bool EqualsData(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(Amazon.S3.Model.S3Bucket), "Wrong data passed to the object for update.");
            return string.Equals(Name, ((Amazon.S3.Model.S3Bucket)responseData).BucketName);
        }

        protected override void DoDeleteObject()
        {
            Amazon.S3.Model.DeleteBucketResponse resp = myController.s3.DeleteBucket(new Amazon.S3.Model.DeleteBucketRequest()
                                                                    .WithBucketName(Name));
        }
    }
}
