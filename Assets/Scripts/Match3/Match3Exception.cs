using System;

namespace Match3
{
    /// <summary>
    /// Special exception.
    /// </summary>
    public class Match3Exception : Exception
    {
        public Match3Exception(Exception innerException, string message)
            : base(message, innerException)
        { }

        public Match3Exception(string message)
            : this(null, message)
        { }
    }
}
