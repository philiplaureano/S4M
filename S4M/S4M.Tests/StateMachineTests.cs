using System;
using System.Linq;
using System.Threading.Tasks;
using S4M.Tests.Samples;
using Xunit;

namespace S4M.Tests
{
    public class StateMachineTests
    {
        [Fact(DisplayName = "The state machine should allow its derived classes to stash messages")]
        public async Task ShouldBeAbleToStashMessages()
        {
            var numberOfMessagesToStash = 42;
            var numberOfMessagesToSend = 1;

            var messagesToSend = Enumerable.Range(0, numberOfMessagesToSend).Select(_ => Guid.NewGuid()).ToArray();
            var messagesToStash = Enumerable.Range(0, numberOfMessagesToStash).Select(_ => Guid.NewGuid()).ToArray();

            var stasher = new SampleStasher();

            // Add the messages that should be handled normally
            foreach (var msg in messagesToSend)
            {
                await stasher.TellAsync(msg);
            }
            
            // Enable stashing and ensure that the correct number of messages have been handled and stashed
            stasher.EnableStashing = true;
            foreach (var msg in messagesToStash)
            {
                await stasher.TellAsync(msg);
            }
            
            // Ensure that both lists match the expected results
            foreach (var msg in messagesToSend)
            {
                Assert.Contains(msg, stasher.MessagesHandled);
            }
            
            foreach (var msg in messagesToStash)
            {
                Assert.Contains(msg, stasher.MessagesStashed);
            }
        }

        [Fact(DisplayName =
            "The state machine should allow its derived classes to unstash all of the stashed messages")]
        public async Task ShouldBeAbleToUnstashAllStashedMessages()
        {
            throw new NotImplementedException("TODO: Implement the ShouldBeAbleToUnstashAllStashedMessages test");
        }

        [Fact(DisplayName =
            "The state machine should handle stashed messages after they are unsta shed by a derived class")]
        public async Task ShouldHandleStashedMessagesAfterUnstashingThem()
        {
            throw new NotImplementedException(
                "TODO: Implement the ShouldHandleStashedMessagesAfterUnstashingThem test");
        }

        [Fact(DisplayName = "The state machine should allow its derived classes to change state")]
        public async Task ShouldAllowDerivedClassesToChangeState()
        {
            Action<object> faultyHandler = msg => throw new InvalidOperationException("Something bad happened");

            var sampleCircuitBreaker = new SampleCircuitBreaker(faultyHandler);

            // Verify the starting state of the sample derived class
            Assert.Equal(CircuitBreakerState.Closed, sampleCircuitBreaker.CurrentState);

            // Pass a random message to the circuit breaker to trigger the faulty handler, which will cause
            // an exception
            await sampleCircuitBreaker.TellAsync(Guid.NewGuid());

            Assert.Equal(CircuitBreakerState.Open, sampleCircuitBreaker.CurrentState);
            Assert.Single(sampleCircuitBreaker.ExceptionsThrown);
        }
    }
}