using System.Threading;
using System.Threading.Tasks;

namespace S4M.Core
{
    public interface ICanTellAsync
    {
        Task TellAsync(object message, CancellationToken cancellationToken);   
    }
}