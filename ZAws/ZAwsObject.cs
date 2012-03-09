using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZAws
{
    abstract class ZAwsObject
    {
        protected readonly ZAwsEc2Controller myController;
        protected ZAwsObject(ZAwsEc2Controller controller)
        {
            myController = controller;
        }

        public event EventHandler StatusChanged;
        public event EventHandler ObjectDeleted;

        protected void TriggerStatusChanged()
        {
            if(StatusChanged != null)
            {
                StatusChanged(this, EventArgs.Empty);
            }
        }
        protected void TriggerObjectDeleted()
        {
            if (ObjectDeleted != null)
            {
                ObjectDeleted(this, EventArgs.Empty);
            }
        }

        public abstract string Name { get; }
    }
}
