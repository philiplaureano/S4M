using System.Threading;
using System.Threading.Tasks;

namespace S4M.Core
{
    public interface IStateMachine
    {
        Task TellAsync(object message, CancellationToken cancellationToken);
    }
}