using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace api_switchboard.BusinessLogic
{
    public interface IApiConnector
    {
         /// <summary>
         /// Translates the payload from the incoming model to the outgoing one.
         /// </summary>
         /// <param name="incomingValues"></param>
         /// <returns></returns>
         Task<IDictionary<string, object>> OnMapModel(IDictionary<string, object> incomingValues);

         /// <summary>
         /// Connects the two APIs by translating the model and calling the outgoing API.
         /// </summary>
         /// <param name="incomingValues"></param>
         /// <returns></returns>
         Task Connect(IDictionary<string, object> incomingValues);

         /// <summary>
         /// Gets the headers for the subsequent API call.
         /// </summary>
         /// <param name="headers"></param>
         /// <returns></returns>
         IDictionary<string, string> OnGetHeaders();

         /// <summary>
         /// Returns the outgoing URI of the API which is called.
         /// </summary>
         /// <returns></returns>
         string OnDetermineUri();

         /// <summary>
         /// Tells which HTTP method is used to do the outgoing API call.
         /// </summary>
         /// <returns></returns>
         ApiConnectorMethod OnDetermineMethod();

         /// <summary>
         /// Callback to set flags depending on the incoming values.
         /// 
         /// This method is called first after the values have been parsed.
         /// </summary>
         /// <param name="incomingValues"></param>
         /// <returns></returns>
         Task OnSetFlagsIncoming(IDictionary<string, object> incomingValues);

         /// <summary>
         /// Callback to set flags depending on the outgoing values.
         /// 
         /// This method is called directly after the incoming values have been mapped to the
         /// outgoing ones.
         /// </summary>
         /// <param name="outgoingValues"></param>
         /// <returns></returns>
         Task OnSetFlagsOutgoing(IDictionary<string, object> outgoingValues);
    }
}