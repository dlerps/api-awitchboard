[ ![Codeship Status for dlerps/api-switchboard](https://app.codeship.com/projects/94ba3e70-1ec8-0135-646f-16c2125e48fe/status?branch=master)](https://app.codeship.com/projects/220850)

# api-switchboard
A RESTful API which can receive calls and redirects them like a proxy to other REST APIs. Great for unlisted and incompatible IFTTT services or anything else you can come up with.


# Classes
The main logic is located in the ``BusinessLogic/Connectors`` directory.

## Interfaces
The main interface is the ``IApiConnector``. It defines the main method ``Connect(IDictionary<string, object>)`` which takes the payload of the incoming call (in key-value pairs) and does the subsequent outgoong API call.

## Abstract Classes

### AbstractApiCpnnector
The main abstract implementation of the IApiConnector intercae is the ``AbstractApiConnector`` which already includes an implementation of the ``Connect(..)`` method that calls all necessary callbacks in order to do a successful outgoing API call.
#### Callback Execution Order
1. ``OnSetFlagsIncoming(IDictionary<string, object>)`` 
  This callback is intended to set certain flags (e.g. depending on tokens, ...) which are necessary for subsequent calls to make certain decisions. The parameter which is passed in are the incoming API call values.

2. ``OnMapModel(IDictionary<string, object>)``
  In this callback the incoming values are mapped to the outgoing ones. Put your logic whoch translates your call here.

3. ``OnSetFlagsOutgoing(IDictionary<string, object>)``
  Similar to the flags determined by the incoming values, this callback can set them based on the mapped values for the outgoing call.

4. ``OnDetermineMethod()``
  This callback is supposed to return the HTTP method type (as the built-in enum) which is intended to be used for the outgoing API call.

5. ``OnDetermineUri()``
  This callback is supposed to return the URI for the ourgoing API call.

6. ``OnGetHeaders()``
  The return value of this callback should be a dictionary of all necessary headers which need to be set for the outgoing API call.

### SimpleApiConnector
This abstract class is a sub-class of the AbstractApiConnector. This is a convenience implementation because it contains empty methods for all optional callbacks so they do not need to be implemented, but can be overriden if necessary.

# Run the code
``dotnet run``
