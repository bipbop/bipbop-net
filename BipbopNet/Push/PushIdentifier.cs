using System;

namespace BipbopNet.Push
{
    public class PushIdentifier
    {
        public string? Id;
        public string? Label;
    }

    public class PushException : Exception
    {
        public PushException(string empty) : base(empty)
        {
        }
    }
}