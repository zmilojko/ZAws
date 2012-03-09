using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZAws
{
    class ZAwsEc2 : ZAwsObject
    {
        public Amazon.EC2.Model.Reservation Reservation { get; private set; }

        public ZAwsEc2(ZAwsEc2Controller controller, Amazon.EC2.Model.Reservation res)
            : base(controller)
        {
            Update(res);
        }

        public void Update(Amazon.EC2.Model.Reservation res)
        {
            Reservation = res;
            TriggerStatusChanged();
        }

        public override string Name
        {
            get
            {
                //find the instance name among the tags
                string InstanceName = "not set";
                foreach (Amazon.EC2.Model.Tag t in Reservation.RunningInstance[0].Tag)
                {
                    if (t.Key.ToLower() == "name")
                    {
                        InstanceName = t.Value;
                    }
                }
                return InstanceName;
            }
        }

        /// <summary>
        /// Returns one of the following: 0 for Running, 1 for Stopped, 2 for Stopping, 3 for Terminated, 
        /// 4 for Shutting-down and 5 for Pending.
        /// </summary>
        public int StatusCode
        {
            get
            {
                switch (((int)(Math.Truncate(Reservation.RunningInstance[0].InstanceState.Code))) % 256)
                {
                    case 16: return 0;
                    case 32: return 4;
                    case 48: return 3;
                    case 64: return 2;
                    case 80: return 1;
                    default: return 5;
                }
            }

        }


        public string InstanceId
        {
            get
            {
                return Reservation.RunningInstance[0].InstanceId;
            }
        }



        internal void Delete()
        {
            TriggerObjectDeleted();
        }
    }
}
