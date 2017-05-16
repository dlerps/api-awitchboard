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
         Task<IDictionary<string, object>> Map(IDictionary<string, object> incomingValues);

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
         IDictionary<string, string> GetOutgoingHeaders();

         /// <summary>
         /// Returns the outgoing URI of the API which is called.
         /// </summary>
         /// <returns></returns>
         string GetOutgoingUrl();

         /// <summary>
         /// Tells which HTTP method is used to do the outgoing API call.
         /// </summary>
         /// <returns></returns>
         ApiConnectorMethod GetMethod();
    }
}