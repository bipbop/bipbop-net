using System;
using BipbopNet.Parser;
using Exception = BipbopNet.Parser.Exception;

namespace BipbopNet.Push
{
    public class PushStatus
    {
        public PushIdentifier? Push;
        
        public int? Version;
        public int? Tries;
        public int? Executions;
        public int? SuccessExecutions;
        public bool? HasException;
        public bool? Locked;
        public bool? Processing;
        public string? Callback;
        public Exception? Exception;
        public string? Machine;
        public int? Pid;

        public DateTime? LastSuccessRun;
        public DateTime? Created;
        public DateTime? NextJob;
        public DateTime? ProcessingAt;
        public DateTime? LastRun;
        public DateTime? Deleted;
        public DateTime? ExpectedNextJob;
    }
}