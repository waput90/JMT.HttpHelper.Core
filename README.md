# A simple HTTP Helper for .NET Core 3.x

this was built with love and community friendly in mind library for requesting API, and this was built based on IHttpClientFactory to make sure its properly disposed


## Usage

1. Register the service to startup
```
using JMT.HttpHelper.Core;

public void ConfigureServices(IServiceCollection services)
{
  // some code here
  services.RegisterHttpHelper();
}
```

2. HttpTypes available for the request
```
  POST,
  GET,
  PATCH,
  DELETE,
  PUT
```

3. MimeType current available for the request
```
  JSON,
  FORM_URL_ENCODED
```

3. Sample request
```
using JMT.HttpHelper.Core;

// initialize request
var exchangeRate = await new HttpHelper()
     // set your URL here
    .SetBaseUrl("YOUR STRING URL HERE")

    // set request method refer on step number 2.
    .SetMethod(HttpType.GET)

    // optional but not required for passing client factory
    .SetHttpClientFactory(_clientFactory) 

    // set your url route here
    .SetRoute($"/charts-fetch.php?c1={exchange_to}&c2={exchange_from}&t=1")

    // mime type either JSON or FORM_URL but you can still pass string data here if mimetype are not listed
    .SetAppType(MimeType.JSON)
    
    // mime type either JSON or FORM_URL but you can still pass string data here if mimetype are not listed
    .SetMediaTypeHeader(MimeType.JSON) 
    
    // multiple custom header you can set more than one custom header if refer example below
    .SetCustomHeader("HeaderKey1", "HeaderValue1")
    .SetCustomHeader("HeaderKey2", "HeaderValue2")
    
    // you can mapped directly the object that you want to pass on the request
    .RequestDeserialize<TAnyDTO>();
    
    // this is still the same with RequestDeserialize but only returns string instead of strongly typed objects
    .Request() 

```

