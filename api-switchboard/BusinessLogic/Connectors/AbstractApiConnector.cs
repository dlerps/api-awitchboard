using System.Collections.Generic;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api_switchboard.BusinessLogic
{
    public abstract class AbstractApiConnector : IApiConnector
    {

#region Abstract Methods

        public abstract ApiConnectorMethod GetMethod();
        public abstract string GetOutgoingUrl();
        public abstract Task<IDictionary<string, object>> Map(IDictionary<string, object> incomingValues);
        public abstract IDictionary<string, string> GetOutgoingHeaders();

#endregion

        public async Task Connect(IDictionary<string, object> incomingValues)
        {
            if(incomingValues == null)
                throw new ApiConnectorException("No such incoming values");

            HttpClient http = new HttpClient();
            ApiConnectorMethod method = GetMethod();
            string address = GetOutgoingUrl();

            SetHeaders(http);

            // map incoming values to the outgoing model
            IDictionary<string, object> outgoingValues = await Map(incomingValues);

            // jsonfying the outgoing values
            StringContent payload = null;

            try 
            {
                string stringValues = JsonConvert.SerializeObject(outgoingValues, Formatting.None);
                payload = new StringContent(stringValues);
            }
            catch(Exception e)
            {
                throw new ApiConnectorException("Unable to serialise payload", e);
            }

            Func<Task<HttpResponseMessage>> apiCall = null;

            if(method == ApiConnectorMethod.Post)
                apiCall = async () => await http.PostAsync(address, payload);
            else if(method == ApiConnectorMethod.Put)
                apiCall = async () => await http.PutAsync(address, payload);
            else if(method == ApiConnectorMethod.Patch)
                throw new NotImplementedException("No Patch methods supported yet");

            HttpResponseMessage response = await apiCall.Invoke();

            // throw exception in case of unsuccessful call
            if(!response.IsSuccessStatusCode)
            {
                string code = response.StatusCode.ToString();
                string message = await response.Content.ReadAsStringAsync();

                throw new ApiConnectorException(String.Format("{0} - {1}", code, message));
            }
        }

        private void SetHeaders(HttpClient http)
        {
            IDictionary<string, string> headers = GetOutgoingHeaders();

            if(headers.Any())
            {
                foreach(KeyValuePair<string, string> headerPair in headers)
                {
                    http.DefaultRequestHeaders.Add(headerPair.Key, headerPair.Value);
                }
            }
        }

    }
}