using System;
using System.Threading.Tasks;
using S4M.Core;
using S4M.Tests.Samples;
using Xunit;

namespace S4M.Tests
{
    public class StateMachineTests
    {
        [Fact(DisplayName = "The state machine should allow its derived classes to stash messages")]
        public async Task ShouldBeAbleToStashMessages()
        {
            throw new NotImplementedException("TODO: Implement the ShouldBeAbleToStashMessages test");
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