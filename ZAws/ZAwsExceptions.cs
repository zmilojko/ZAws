using System;

namespace ZAws
{
    class ZAwsException : Exception
    { public ZAwsException(string msg) : base(msg) { } }

    class ZAwsEWrongState : ZAwsException
    { public ZAwsEWrongState(string msg) : base(msg) { } }
    class ZAwsEInstanceNotFound : ZAwsException
    { public ZAwsEInstanceNotFound(string msg) : base(msg) { } }
}
