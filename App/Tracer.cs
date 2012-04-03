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

namespace ZAws.Console
{
    class NewTraceEventArgs : EventArgs
    {
        public NewTraceEventArgs(string t)
        {
            Trace = t;
        }
        public readonly string Trace;
    }

    class Tracer
    {
        public event EventHandler<NewTraceEventArgs> NewTrace;
        object lockObject = new object();

        public void TraceLine(string line, Exception ex, params object[] line_parameters)
        {
            lock (lockObject)
            {
                TraceLine(true, line + " (see trace file for full exception, log entry " + (new Random()).Next().ToString() + ")", line_parameters);
                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }
        }
        public void TraceLine(string line, params object[] line_parameters)
        {
            TraceLine(true, line, line_parameters);
        }
        public void TraceLine(bool includeTimeSTamp, string line, params object[] line_parameters)
        {
            Trace(includeTimeSTamp, line + "\r\n", line_parameters);
        }

        internal void Trace(string line, params object[] line_parameters)
        {
            Trace(true, line, line_parameters);
        }
        internal void Trace(bool includeTimeSTamp, string line, params object[] line_parameters)
        {
            lock (lockObject)
            {
                string s;
                if (line_parameters == null || line_parameters.Length == 0)
                {
                    s = line;
                }
                else
                {
                    try
                    {
                        s = (includeTimeSTamp ? DateTime.UtcNow.ToString("hh:mm:ss") + "> " : "") + string.Format(line, line_parameters);
                    }
                    catch (FormatException)
                    {
                        System.Diagnostics.Trace.WriteLine("Problem formatting following string: " + line);
                        s = (includeTimeSTamp ? DateTime.UtcNow.ToString("hh:mm:ss") + "> " : "") + line;
                    }
                }
                System.Diagnostics.Trace.WriteLine(s);
                if (NewTrace != null)
                {
                    NewTrace(null, new NewTraceEventArgs(s));
                }
            }
        }
    }
}
