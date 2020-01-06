using System;
using Exception = BipbopNet.Parser.Exception;

namespace BipbopNet.Push
{
    public class PushStatus
    {
        public string? Callback;
        public DateTime? Created;
        public DateTime? Deleted;
        public Exception? Exception;
        public int? Executions;
        public DateTime? ExpectedNextJob;
        public bool? HasException;
        public DateTime? LastRun;

        public DateTime? LastSuccessRun;
        public bool? Locked;
        public string? Machine;
        public DateTime? NextJob;
        public int? Pid;
        public bool? Processing;
        public DateTime? ProcessingAt;
        public PushIdentifier? Push;
        public int? SuccessExecutions;
        public int? Tries;

        public int? Version;
    }
}