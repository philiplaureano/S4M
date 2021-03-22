using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using S4M.Core;
using Xunit;

namespace S4M.Tests
{
    public class PipeToTests
    {
        [Fact(DisplayName = @"Task<T> results should be able to be redirected to ICanTellAsync instances")]
        public void ShouldBeAbleToPipeResultsToSelf()
        {
            var expectedResult = Guid.NewGuid();
            var fakeReceiver = A.Fake<ICanTellAsync>();

            var fakeTask = Task.FromResult(expectedResult);
            fakeTask.PipeTo(fakeReceiver);

            A.CallTo(() => fakeReceiver.TellAsync(expectedResult, CancellationToken.None)).MustHaveHappened();
        }
    }
}