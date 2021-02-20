using System;

namespace S4M.Core
{
    /// <summary>
    /// This is just an internal exception used to
    /// make an unconditional jump out of a handler method
    /// rather than executing the rest of the handler block
    /// </summary>
    internal class StashException : Exception
    {
        public StashException()
        {
        }
    }
}