using Newtonsoft.Json;

using System.Collections.Generic;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Mvc;

using api_switchboard.BusinessLogic.Utils;

namespace api_switchboard.BusinessLogic
{
    /// <summary>
    /// This abstract implementation of the IApiConnector includes an implementation for the 
    /// Connect() method.
    /// </summary>
    public abstract class AbstractApiConnector : IApiConnector
    {
        private Dictionary<string, object> _flags;
        private List<string> _errors;

        /// <summary>
        /// Initialises a new Abstract Api Connector.
        /// </summary>
        public AbstractApiConnector()
        {
            _errors = new List<string>();
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
            outgoingValues.PrintDictionary();

            await OnSetFlagsOutgoing(outgoingValues);

            // initialising the outgoing http client & call
            HttpClient http = new HttpClient();
            ApiConnectorMethod method = OnDetermineMethod();
            string address = OnDetermineUri();

            SetHeaders(http);
            http.Timeout = TimeSpan.FromSeconds(5);

            StringContent payload = null;

            try 
            {
                // jsonfying the outgoing values
                string stringValues = JsonConvert.SerializeObject(outgoingValues, Formatting.None);

                payload = new StringContent(stringValues);
                payload.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            catch(Exception)
            {
                AddError("Unable to serialise payload");
            }
            
            // only do the outgoing api call if there haven't been any errors
            if(!IsInErrorState())
            {
                Func<Task<HttpResponseMessage>> apiCall = null;

                try
                {
                    if(method == ApiConnectorMethod.Post)
                        apiCall = async () => await http.PostAsync(address, payload);
                    else if(method == ApiConnectorMethod.Put)
                        apiCall = async () => await http.PutAsync(address, payload);
                    else if(method == ApiConnectorMethod.Patch)
                        throw new NotImplementedException("No Patch methods supported yet");
                }
                catch(Exception e)
                {
                    // Log the error
                    AddError(e);
                }

                HttpResponseMessage response = await apiCall.Invoke();
                string code = response.StatusCode.ToString();
                string message = await response.Content.ReadAsStringAsync();

                // record error in case of unsuccessful call
                if(!response.IsSuccessStatusCode)
                    AddError(String.Format("{0} - {1}", code, message));
            }

            // if there have been errors -> throw them in an exception
            if(IsInErrorState())
                throw new ApiConnectorException(GetFormattedErros());
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
            return _flags.ParseValue<FType>(key, defaultValue);
        }

        /// <summary>
        /// Adds an error which occured to the list.
        /// </summary>
        /// <param name="err"></param>
        protected void AddError(string err)
        {
            if(!String.IsNullOrEmpty(err))
                _errors.Add(err);
        }

        /// <summary>
        /// Adds an exception which occured to the list of errors.
        /// </summary>
        /// <param name="exception"></param>
        protected void AddError(Exception exception)
        {
            if(exception != null)
                _errors.Add(exception.Message);
        }

        /// <summary>
        /// Gets the list of execution errors.
        /// </summary>
        /// <returns></returns>
        protected List<string> GetErrors()
        {
            return _errors;
        }

        /// <summary>
        /// Gets a formatted string of all errors.
        /// </summary>
        /// <returns></returns>
        protected string GetFormattedErros()
        {
            return _errors.Any() ? String.Join("\n", _errors) : String.Empty;
        }

        /// <summary>
        /// Tells if there have been any errors logged so far.
        /// </summary>
        /// <returns></returns>
        protected bool IsInErrorState()
        {
            return _errors.Any();
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