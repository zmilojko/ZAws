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
