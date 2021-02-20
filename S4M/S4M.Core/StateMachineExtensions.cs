using System.Threading;
using System.Threading.Tasks;

namespace S4M.Core
{
    public static class StateMachineExtensions
    {
        public static async Task TellAsync(this IStateMachine stateMachine, object message)
        {
            await stateMachine.TellAsync(message, CancellationToken.None);
        }
    }
}