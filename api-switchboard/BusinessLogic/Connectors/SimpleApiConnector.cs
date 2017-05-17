using System.Collections.Generic;
using System.Threading.Tasks;

namespace api_switchboard.BusinessLogic
{
    /// <summary>
    /// This abstract implementation of the AbstractApiConnector implements empty method
    /// studs for all optional callbacks.
    /// </summary>
    public abstract class SimpleApiConnector : AbstractApiConnector
    {
        public override Task OnSetFlagsOutgoing(IDictionary<string, object> outVals)
        {
            return Task.FromResult(false);
        }

        public override Task OnSetFlagsIncoming(IDictionary<string, object> outVals)
        {
            return Task.FromResult(false);
        }
    }
}