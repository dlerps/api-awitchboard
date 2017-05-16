namespace api_switchboard.BusinessLogic
{
    /// <summary>
    /// Enum to determine which HTTP method the outgoing API call should use.
    /// </summary>
    public enum ApiConnectorMethod
    {
        Post = 1,
        Put = 2,
        Patch = 3
    }
}