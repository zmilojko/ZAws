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

namespace ZAws.Console
{
    static public class Do
    {
        static public EventHandler HandleInZawsUi(EventHandler target, string SuccessComment, string FailComment)
        {
            Handler h = new Handler(target, SuccessComment, FailComment);
            return new EventHandler(h.Handle);
        }

        class Handler
        {
            public Handler(EventHandler t, string SuccessComment, string FailComment)
            {
                target = t;
                successComment = SuccessComment;
                failComment = FailComment;
            }
            EventHandler target;
            string successComment;
            string failComment;

            public void Handle(object sender, EventArgs e)
            {
                try
                {
                    target(sender, e);
                    Program.TraceLine(successComment);
                }
                catch (NoCommentException)
                {
                }
                catch (Exception ex)
                {
                    Program.TraceLine(failComment, ex, ex.Message);
                }
            }
        }
    }
}
