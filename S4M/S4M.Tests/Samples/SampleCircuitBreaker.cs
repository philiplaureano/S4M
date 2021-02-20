using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using S4M.Core;

namespace S4M.Tests.Samples
{
    /// <summary>
    /// A sample circuit breaker that slams wide open if a call fails, and closes itself if a call succeeds while in an open state. Definitely not for production use.
    /// </summary>
    public class SampleCircuitBreaker : StateMachine
    {
        private readonly Action<object> _commandHandler;
        private readonly ConcurrentBag<object> _handledMessages = new ConcurrentBag<object>();
        private readonly ConcurrentBag<Exception> _exceptionsThrown = new ConcurrentBag<Exception>();

        public SampleCircuitBreaker(Action<object> commandHandler)
        {
            _commandHandler = commandHandler;
            CurrentState = CircuitBreakerState.Closed;

            Become(Closed);
        }

        private void Closed()
        {
            Receive<object>(msg =>
            {
                try
                {
                    _commandHandler(msg);
                    _handledMessages.Add(msg);
                }
                catch (Exception e)
                {
                    _exceptionsThrown.Add(e);
                    
                    // Something happened - trip the circuit breaker
                    CurrentState = CircuitBreakerState.Open;
                    Become(Open);
                }
            });
        }

        private void Open()
        {
            Receive<object>(msg =>
            {
                try
                {
                    _commandHandler(msg);
                    _handledMessages.Add(msg);
                    
                    // If the call worked, let's close the circuit breaker again 
                    CurrentState = CircuitBreakerState.Closed;
                    Become(Closed);
                }
                catch (Exception e)
                {
                    // Ignore the error and keep the circuit breaker open since errors are still occurring
                    _exceptionsThrown.Add(e);
                }
            });
        }

        public CircuitBreakerState CurrentState { get; private set; }

        public IEnumerable<object> HandledMessages => _handledMessages;
        public IEnumerable<Exception> ExceptionsThrown => _exceptionsThrown;
    }
}