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
            var numberOfMessagesToStash = 42;
            var messagesToStash = Enumerable.Range(0, numberOfMessagesToStash).Select(_ => Guid.NewGuid()).ToArray();

            var unstasher = new SampleUnstasher();

            // Push the messages and have the unstasher stash every message coming in
            foreach (var msg in messagesToStash)
            {
                await unstasher.TellAsync(msg);
            }

            // Match the number of messages stashed
            Assert.Equal(numberOfMessagesToStash, unstasher.MessagesStashed.Count());

            // Enable the message processing, which should trigger the UnstashAll() call that will
            // pop all the stashed messages into the mailbox
            unstasher.StartHandlingMessages();

            // Tell the unstasher to handle one message, which should trigger the message handling for
            // the previously stashed messages
            await unstasher.TellAsync(Guid.NewGuid());

            var expectedNumberOfHandledMessages = numberOfMessagesToStash + 1;
            Assert.Equal(expectedNumberOfHandledMessages, unstasher.MessagesHandled.Count());
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