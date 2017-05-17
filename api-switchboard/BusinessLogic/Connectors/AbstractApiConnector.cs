using System.Collections.Generic;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api_switchboard.BusinessLogic
{
    /// <summary>
    /// This abstract implementation of the IApiConnector includes an implementation for the 
    /// Connect() method.
    /// </summary>
    public abstract class AbstractApiConnector : IApiConnector
    {
        private Dictionary<string, object> _flags;

        /// <summary>
        /// Initialises a new Abstract Api Connector.
        /// </summary>
        public AbstractApiConnector()
        {
            _flags = new Dictionary<string, object>();
        }

        /// <summary>
        /// Connects the incoming API call with the outgoing API!
        /// 
        /// Passes in the payload of the incoming call, maps it to the outgoing model and
        /// uses it as the content for the call.
        /// 
        /// Uses the information provided by the abstract methods. In order for this
        /// method to work these need to be implemented correctly.
        /// </summary>
        /// <param name="incomingValues"></param>
        /// <returns></returns>
        public async Task Connect(IDictionary<string, object> incomingValues)
        {
            if(incomingValues == null)
                throw new ApiConnectorException("No such incoming values");

            await OnSetFlagsIncoming(incomingValues);

            // map incoming values to the outgoing model
            IDictionary<string, object> outgoingValues = await OnMapModel(incomingValues);

            await OnSetFlagsOutgoing(outgoingValues);

            // initialising the outgoing http client & call
            HttpClient http = new HttpClient();
            ApiConnectorMethod method = OnDetermineMethod();
            string address = OnDetermineUri();

            SetHeaders(http);

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

#region Helpers

        /// <summary>
        /// Sets a value in the internal flags dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected AbstractApiConnector SetFlag<FType>(string key, FType value)
        {
            _flags.Add(key, value);
            return this;
        }

        /// <summary>
        /// Gets a value from the internal flags dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected FType GetFlag<FType>(string key, FType defaultValue = default(FType))
        {
            if(_flags.ContainsKey(key) && _flags[key] is FType)
                return (FType) _flags[key];

            return defaultValue;
        }

        /// <summary>
        /// Sets the headers for the API call.
        /// </summary>
        /// <param name="http"></param>
        private void SetHeaders(HttpClient http)
        {
            IDictionary<string, string> headers = OnGetHeaders();

            if(headers.Any())
            {
                foreach(KeyValuePair<string, string> headerPair in headers)
                {
                    http.DefaultRequestHeaders.Add(headerPair.Key, headerPair.Value);
                }
            }
        }

#endregion

#region Abstract Callbacks

        public abstract ApiConnectorMethod OnDetermineMethod();
        public abstract string OnDetermineUri();
        public abstract Task<IDictionary<string, object>> OnMapModel(IDictionary<string, object> incomingValues);
        public abstract IDictionary<string, string> OnGetHeaders();
        public abstract Task OnSetFlagsIncoming(IDictionary<string, object> incomingValues);
        public abstract Task OnSetFlagsOutgoing(IDictionary<string, object> outgoingValues);

#endregion

    }
}