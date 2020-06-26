## Testing Gracefull shutdown

### HTTP trigger behaviour

#### Locally

* Open in project in Visual Studio
* Run
* Issue an HTTP request similar to the following, with Fiddler/Postman/whatever

```
POST http://localhost:7071/api/wait-for-shutdown/30/4 HTTP/1.1
```

You have 30 seconds to press `Ctrl + C` in the host console to trigger a shutdown
Once shutdown has been triggered, function user code will wait for 4 seconds before responding an HTTP 503

### Service Bus trigger behaviour

#### Locally

