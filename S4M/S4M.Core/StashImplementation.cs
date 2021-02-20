using System.Collections.Concurrent;
using Optional;
using Optional.Unsafe;

namespace S4M.Core
{
    internal class StashImplementation : IStash
    {
        private readonly ConcurrentQueue<object> _currentStash;
        private readonly ConcurrentQueue<object> _currentMailbox;
        private Option<object> _nextMessage = Option.None<object>();

        public StashImplementation(ConcurrentQueue<object> currentStash, ConcurrentQueue<object> currentMailbox)
        {
            _currentStash = currentStash;
            _currentMailbox = currentMailbox;
        }

        internal void SetNextMessage(Option<object> nextMessage)
        {
            _nextMessage = nextMessage;
        }

        public void Stash()
        {
            if (!_nextMessage.HasValue)
                return;

            _currentStash.Enqueue(_nextMessage.ValueOrFailure());

            // Hack: Throw a StashException so that it jumps out of the method
            // before anything else can be processed
            throw new StashException();
        }

        public void Unstash()
        {
            if (_currentStash.IsEmpty)
                return;

            // If Unstash is called, pull one message
            // off the stash and place it in the inbox
            if (_currentStash.TryDequeue(out var currentItem))
                _currentMailbox.Enqueue(currentItem);
        }

        public void UnstashAll()
        {
            // If UnstashAll was called, empty the stash back onto the inbox
            while (!_currentStash.IsEmpty)
            {
                if (_currentStash.TryDequeue(out var currentItem))
                    _currentMailbox.Enqueue(currentItem);
            }
        }
    }
}