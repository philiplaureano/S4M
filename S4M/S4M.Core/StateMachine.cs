using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Optional;

namespace S4M.Core
{
    public abstract class StateMachine : IStateMachine
    {
        private ConcurrentDictionary<Func<object, bool>, Action<object>> _currentState =
            new ConcurrentDictionary<Func<object, bool>, Action<object>>();

        private readonly ConcurrentQueue<(Func<object, bool>, Action<object>)> _queuedHandlers =
            new ConcurrentQueue<(Func<object, bool>, Action<object>)>();

        private readonly ConcurrentQueue<object> _mailbox = new ConcurrentQueue<object>();
        private readonly ConcurrentQueue<object> _stash = new ConcurrentQueue<object>();

        private readonly StashImplementation _stashImplementation;
        private readonly object _stateBuilderLock = new object();
        private bool _isBuildingState = false;
        private bool _hasStateBeenInitialized = false;

        protected StateMachine()
        {
            _stashImplementation = new StashImplementation(_stash, _mailbox);
        }

        public async Task TellAsync(object message, CancellationToken cancellationToken)
        {
            // Block the task/thread if we're in the middle of a state change
            var waitUntilDispatchTableBuilt = Task.Run(() =>
            {
                while (_isBuildingState && !cancellationToken.IsCancellationRequested)
                {
                    // Spin until the state is built
                }
            }, cancellationToken);
            
            if (!_hasStateBeenInitialized)
            {
                await BuildNewDispatchTableAsync();
                _hasStateBeenInitialized = true;
            }

            await waitUntilDispatchTableBuilt;
            await DispatchPendingMailboxMessages(cancellationToken);
            
            // Handle the current message
            await DispatchAsync(message, cancellationToken);
        }

        private async Task DispatchPendingMailboxMessages(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _mailbox.TryDequeue(out var nextMessage))
            {
                await DispatchAsync(nextMessage, cancellationToken);
            }
        }

        private async Task DispatchAsync(object message, CancellationToken cancellationToken)
        {
            foreach (var predicate in _currentState.Keys)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                
                if (!predicate(message))
                    continue;

                var currentAction = _currentState[predicate];
                try
                {
                    // Give the stash handler the next message to potentially stash
                    _stashImplementation.SetNextMessage(Option.Some(message));
                    currentAction?.Invoke(message);
                }
                catch (UnstashAllException)
                {
                    // Ignore the error here since the exception means that the caller
                    // wants to pop the contents of the mailbox onto the stack
                    // and process the stashed messages
                    
                    // Handle the unstashed messages first
                    await DispatchPendingMailboxMessages(cancellationToken);
                    
                    // Push the current message onto the mailbox
                    await DispatchAsync(message, cancellationToken);
                }
                catch (StashException)
                {
                    // Ignore it since the handler
                    // has requested that the message be stashed
                    // rather than calling the handler
                }
                catch (Exception ex)
                {
                    OnExceptionThrown(ex);
                }
            }
        }

        protected IStash Stash => _stashImplementation;

        protected async Task Become(Action configureHandlers)
        {
            if (Monitor.TryEnter(_stateBuilderLock))
            {
                // Change the internal state and start inboxing
                // incoming messages while the new state is being built
                _isBuildingState = true;

                // Queue up the new handlers
                configureHandlers();

                await BuildNewDispatchTableAsync();

                _isBuildingState = false;

                if (!_hasStateBeenInitialized)
                    _hasStateBeenInitialized = true;

                Monitor.Exit(_stateBuilderLock);
            }
        }

#pragma warning disable 1998
        private async Task BuildNewDispatchTableAsync()
#pragma warning restore 1998
        {
            // Build the new dispatch table
            var newState = new ConcurrentDictionary<Func<object, bool>, Action<object>>();
            while (!_queuedHandlers.IsEmpty)
            {
                if (!_queuedHandlers.TryDequeue(out var kvp))
                    continue;

                var (predicate, handler) = kvp;
                newState[predicate] = handler;
            }

            // Replace the old table with the new one
            Interlocked.Exchange(ref _currentState, newState);
        }

        protected void Receive<TMessage>(Action<TMessage> handler)
        {
            Func<object, bool> predicate = (msg => msg is TMessage);
            Action<object> adapter = msg => handler((TMessage) msg);
            _queuedHandlers.Enqueue((predicate, adapter));
        }

        protected void Receive<TMessage>(Func<TMessage, bool> messageFilter, Action<TMessage> handler)
        {
            Func<object, bool> predicate = msg => msg is TMessage message && messageFilter(message);
            Action<object> adapter = msg => handler((TMessage) msg);
            _queuedHandlers.Enqueue((predicate, adapter));
        }

        protected virtual void OnExceptionThrown(Exception ex)
        {
            throw ex;
        }
    }
}