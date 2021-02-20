using System.Collections.Concurrent;
using System.Collections.Generic;
using S4M.Core;

namespace S4M.Tests.Samples
{
    public class SampleStasher : StateMachine
    {
        private readonly ConcurrentBag<object> _messagesHandled = new();
        private readonly ConcurrentBag<object> _messagesStashed = new();

        private bool _stashing = false;

        public SampleStasher()
        {
            Become(NotStashing);
        }

        public bool EnableStashing
        {
            get => _stashing;
            set
            {
                // Skip setting the same value
                if (value == _stashing)
                    return;

                _stashing = value;

                if (_stashing)
                {
                    Become(StashEverything);
                    return;
                }

                Become(NotStashing);
            }
        }

        public IEnumerable<object> MessagesStashed => _messagesStashed;
        public IEnumerable<object> MessagesHandled => _messagesHandled;

        private void StashEverything()
        {
            _stashing = true;
            Receive<object>(msg =>
            {
                _messagesStashed.Add(msg);

                // Stash every message that comes in
                Stash.Stash();
            });
        }

        private void NotStashing()
        {
            _stashing = false;
            Receive<object>(msg =>
            {
                // Handle every message that comes in
                _messagesHandled.Add(msg);
            });
        }
    }
}