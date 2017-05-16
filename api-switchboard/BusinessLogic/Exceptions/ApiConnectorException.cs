using System;

namespace api_switchboard.BusinessLogic
{
    public class ApiConnectorException : Exception
    {
        public ApiConnectorException() : base() {}
        public ApiConnectorException(string msg) : base(msg) {}
        public ApiConnectorException(string msg, Exception e) : base(msg, e) {}
    }
}