using System;

namespace BipbopNet.Push
{
    [Serializable]
    public class JobIdentifier : IEquatable<JobIdentifier>
    {
        /// <summary>
        ///     ID do PUSH
        /// </summary>
        public string Id;

        /// <summary>
        ///     Label do PUSH
        /// </summary>
        public string Label;

        public bool Equals(JobIdentifier other)
        {
            if (other == null) return false;
            if (other.Id != null && Id != null) return other.Id == Id;
            if (other.Label != null && Label != null) return other.Label == Label;
            return false;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}