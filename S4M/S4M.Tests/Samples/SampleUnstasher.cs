using System.Collections.Concurrent;
using System.Collections.Generic;
using S4M.Core;

namespace S4M.Tests.Samples
{
    public class SampleUnstasher : StateMachine
    {
        private readonly ConcurrentBag<object> _messagesHandled = new();
        private readonly ConcurrentBag<object> _messagesStashed = new();
        
        public SampleUnstasher()
        {
            Become(StashEverything);
        }

        public void StartHandlingMessages()
        {
            Become(NotStashing);
        }
        
        private void StashEverything()
        {
            Receive<object>(msg =>
            {
                _messagesStashed.Add(msg);

                // Stash every message that comes in
                Stash.Stash();
            });
        }
        private void NotStashing()
        {
            Receive<object>(msg =>
            {
                // Unstash every message that wasn't handled
                Stash.UnstashAll();

                // Handle every message that comes in
                _messagesHandled.Add(msg);
            });
        }
        public IEnumerable<object> MessagesStashed => _messagesStashed;
        public IEnumerable<object> MessagesHandled => _messagesHandled;
    }
}