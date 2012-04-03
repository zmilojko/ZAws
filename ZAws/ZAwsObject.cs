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
