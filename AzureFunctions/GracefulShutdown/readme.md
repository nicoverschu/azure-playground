#  Azure Functions - Graceful shutdown

## Testing Gracefull shutdown behaviour

### Prerequisites

You'll need
* An Azure Account
* A service bus client, such as Service Bus Explorer

### Setup infrastructure

* Deploy the resource group
   - Open Powershell prompt in `src/GracefulShutdown.Infrastructure/`
   - Login to your Azure Account using `Login-AzureRmAccount`
   - Deploy by running `.\Deploy-AzureResourceGroup.ps1 -ResourceGroupName "whatever"`

### Testing local behaviour

#### With an HTTP trigger Azure Function

* Open Visual Studio
* Run project `MyFunction.Http`
* Issue an HTTP request similar to the following, with Fiddler/Postman/whatever, to simulate a 30s running-task
```
POST http://localhost:7071/api/wait-for-shutdown/30/4 HTTP/1.1
```
* press `Ctrl + C` in the host console to trigger a shutdown

Once shutdown has been triggered, function user code will wait for 4 seconds before responding an HTTP 503

#### With a Service Bus trigger Azure Function

* Open Visual Studio
* Run project `MyFunction.ServiceBus`
* In order to simulate a 30s long-processing task, post a message to the service bus with a body similar to this:

```
{
    "nSecondsWait": 30,
    "nSecondsShutdownDuration": 2
}
```
* press `Ctrl + C` in the host console to trigger a shutdown


### On Azure

#### With an HTTP trigger Azure Function
* Deploy the HTTP trigger Azure Function on `whatever-http`

TO BE TESTED

#### With a Service Bus trigger Azure Function
* Deploy project `MyFunction.ServiceBus` on the Service Azure Function (`whatever-svcbus`)
* In order to simulate a 30s long-processing task, post a message to the service bus with a body similar to this:

```
{
    "nSecondsWait": 30,
    "nSecondsShutdownDuration": 2
}
```

* trigger a function app restart from the portal, for example

What i want to achieve is to save the messages that were being processed to another queue, so that they are handled later on.
I want to avoid messages in the dead-letter queue (potential duplicates).
I've set the Max delivery count to 1 on the queue which triggers the function.
So far i have some messages in the `retryhandling` queue and some messages in the dead-letter.

Next steps: 
* setup a queue which keeps the messages that were processed without any trouble.
* increase the max delivery count of `queue` in order to see if i end up with actual duplicates.


