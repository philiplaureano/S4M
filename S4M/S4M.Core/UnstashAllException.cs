using System;

namespace S4M.Core
{
    /// <summary>
    /// This is an exception that causes an unconditional jump
    /// out of a handler method for unstashing all messages. Internal use only.
    /// </summary>
    internal class UnstashAllException : Exception
    {
        internal UnstashAllException()
        {
        }
    }
}