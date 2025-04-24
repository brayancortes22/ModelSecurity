# Org.OpenAPITools.Api.AprendizProgramApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiAprendizProgramGet**](AprendizProgramApi.md#apiaprendizprogramget) | **GET** /api/AprendizProgram |  |
| [**ApiAprendizProgramIdDelete**](AprendizProgramApi.md#apiaprendizprogramiddelete) | **DELETE** /api/AprendizProgram/{id} |  |
| [**ApiAprendizProgramIdGet**](AprendizProgramApi.md#apiaprendizprogramidget) | **GET** /api/AprendizProgram/{id} |  |
| [**ApiAprendizProgramIdPatch**](AprendizProgramApi.md#apiaprendizprogramidpatch) | **PATCH** /api/AprendizProgram/{id} |  |
| [**ApiAprendizProgramIdPut**](AprendizProgramApi.md#apiaprendizprogramidput) | **PUT** /api/AprendizProgram/{id} |  |
| [**ApiAprendizProgramIdSoftDelete**](AprendizProgramApi.md#apiaprendizprogramidsoftdelete) | **DELETE** /api/AprendizProgram/{id}/soft |  |
| [**ApiAprendizProgramPost**](AprendizProgramApi.md#apiaprendizprogrampost) | **POST** /api/AprendizProgram |  |

<a id="apiaprendizprogramget"></a>
# **ApiAprendizProgramGet**
> List&lt;AprendizProgramDto&gt; ApiAprendizProgramGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiAprendizProgramGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new AprendizProgramApi(config);

            try
            {
                List<AprendizProgramDto> result = apiInstance.ApiAprendizProgramGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAprendizProgramGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<AprendizProgramDto>> response = apiInstance.ApiAprendizProgramGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;AprendizProgramDto&gt;**](AprendizProgramDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="apiaprendizprogramiddelete"></a>
# **ApiAprendizProgramIdDelete**
> void ApiAprendizProgramIdDelete (int id)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiAprendizProgramIdDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new AprendizProgramApi(config);
            var id = 56;  // int | 

            try
            {
                apiInstance.ApiAprendizProgramIdDelete(id);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramIdDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAprendizProgramIdDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiAprendizProgramIdDeleteWithHttpInfo(id);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramIdDeleteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **int** |  |  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **204** | No Content |  -  |
| **400** | Bad Request |  -  |
| **404** | Not Found |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="apiaprendizprogramidget"></a>
# **ApiAprendizProgramIdGet**
> AprendizProgramDto ApiAprendizProgramIdGet (int id)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiAprendizProgramIdGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new AprendizProgramApi(config);
            var id = 56;  // int | 

            try
            {
                AprendizProgramDto result = apiInstance.ApiAprendizProgramIdGet(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramIdGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAprendizProgramIdGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<AprendizProgramDto> response = apiInstance.ApiAprendizProgramIdGetWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramIdGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **int** |  |  |

### Return type

[**AprendizProgramDto**](AprendizProgramDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **400** | Bad Request |  -  |
| **404** | Not Found |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="apiaprendizprogramidpatch"></a>
# **ApiAprendizProgramIdPatch**
> void ApiAprendizProgramIdPatch (int id, AprendizProgramDto aprendizProgramDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiAprendizProgramIdPatchExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new AprendizProgramApi(config);
            var id = 56;  // int | 
            var aprendizProgramDto = new AprendizProgramDto(); // AprendizProgramDto |  (optional) 

            try
            {
                apiInstance.ApiAprendizProgramIdPatch(id, aprendizProgramDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramIdPatch: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAprendizProgramIdPatchWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiAprendizProgramIdPatchWithHttpInfo(id, aprendizProgramDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramIdPatchWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **int** |  |  |
| **aprendizProgramDto** | [**AprendizProgramDto**](AprendizProgramDto.md) |  | [optional]  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **400** | Bad Request |  -  |
| **404** | Not Found |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="apiaprendizprogramidput"></a>
# **ApiAprendizProgramIdPut**
> void ApiAprendizProgramIdPut (int id, AprendizProgramDto aprendizProgramDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiAprendizProgramIdPutExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new AprendizProgramApi(config);
            var id = 56;  // int | 
            var aprendizProgramDto = new AprendizProgramDto(); // AprendizProgramDto |  (optional) 

            try
            {
                apiInstance.ApiAprendizProgramIdPut(id, aprendizProgramDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramIdPut: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAprendizProgramIdPutWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiAprendizProgramIdPutWithHttpInfo(id, aprendizProgramDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramIdPutWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **int** |  |  |
| **aprendizProgramDto** | [**AprendizProgramDto**](AprendizProgramDto.md) |  | [optional]  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **400** | Bad Request |  -  |
| **404** | Not Found |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="apiaprendizprogramidsoftdelete"></a>
# **ApiAprendizProgramIdSoftDelete**
> void ApiAprendizProgramIdSoftDelete (int id)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiAprendizProgramIdSoftDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new AprendizProgramApi(config);
            var id = 56;  // int | 

            try
            {
                apiInstance.ApiAprendizProgramIdSoftDelete(id);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramIdSoftDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAprendizProgramIdSoftDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiAprendizProgramIdSoftDeleteWithHttpInfo(id);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramIdSoftDeleteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **int** |  |  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **204** | No Content |  -  |
| **400** | Bad Request |  -  |
| **404** | Not Found |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="apiaprendizprogrampost"></a>
# **ApiAprendizProgramPost**
> AprendizProgramDto ApiAprendizProgramPost (AprendizProgramDto aprendizProgramDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiAprendizProgramPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new AprendizProgramApi(config);
            var aprendizProgramDto = new AprendizProgramDto(); // AprendizProgramDto |  (optional) 

            try
            {
                AprendizProgramDto result = apiInstance.ApiAprendizProgramPost(aprendizProgramDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAprendizProgramPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<AprendizProgramDto> response = apiInstance.ApiAprendizProgramPostWithHttpInfo(aprendizProgramDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AprendizProgramApi.ApiAprendizProgramPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **aprendizProgramDto** | [**AprendizProgramDto**](AprendizProgramDto.md) |  | [optional]  |

### Return type

[**AprendizProgramDto**](AprendizProgramDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **201** | Created |  -  |
| **400** | Bad Request |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

