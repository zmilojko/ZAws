using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace ZAws.Console
{
    class ZAwsPopupForm : Form
    {
        public readonly ZAwsObject MyObj;
        protected ZAwsEc2Controller MyControler
        {
            get
            {
                return MyObj.myController;
            }
        }
        /// <summary>
        /// This should never be used - it is here only for the FOrms designer!
        /// </summary>
        public ZAwsPopupForm()
            : base()
        {
        }

        public ZAwsPopupForm(ZAwsObject obj)
            : this()
        {
            MyObj = obj;
            Debug.Assert(MyObj != null);
            Debug.Assert(MyControler != null);

            this.FormClosed += new FormClosedEventHandler(HandleFormClosed);
        }

        protected virtual void HandleFormClosed(object sender, FormClosedEventArgs e)
        {
            Program.UnrgisterPopupForm(this);
        }

        public Type ServedType
        {
            get
            {
                return MyObj.GetType();
            }
        }

        
    }
}
