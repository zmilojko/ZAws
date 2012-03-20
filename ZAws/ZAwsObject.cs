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

        internal void Update(Object ResponseData)
        {
            if (DoUpdate(ResponseData))
            {
                TriggerStatusChanged();
            }
        }

        internal virtual void Delete()
        {
            TriggerObjectDeleted();
        }

        public abstract string Name { get; }
        protected abstract bool DoUpdate(Object responseData);
        protected abstract void DoDeleteObject();
        internal abstract bool EqualsData(Object responseData);

        public void DeleteObject()
        {
            DoDeleteObject();
        }
    }
}
