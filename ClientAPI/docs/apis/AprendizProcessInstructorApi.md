# Org.OpenAPITools.Api.AprendizProcessInstructorApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiAprendizProcessInstructorGet**](AprendizProcessInstructorApi.md#apiaprendizprocessinstructorget) | **GET** /api/AprendizProcessInstructor |  |
| [**ApiAprendizProcessInstructorIdDelete**](AprendizProcessInstructorApi.md#apiaprendizprocessinstructoriddelete) | **DELETE** /api/AprendizProcessInstructor/{id} |  |
| [**ApiAprendizProcessInstructorIdGet**](AprendizProcessInstructorApi.md#apiaprendizprocessinstructoridget) | **GET** /api/AprendizProcessInstructor/{id} |  |
| [**ApiAprendizProcessInstructorIdPut**](AprendizProcessInstructorApi.md#apiaprendizprocessinstructoridput) | **PUT** /api/AprendizProcessInstructor/{id} |  |
| [**ApiAprendizProcessInstructorPost**](AprendizProcessInstructorApi.md#apiaprendizprocessinstructorpost) | **POST** /api/AprendizProcessInstructor |  |

<a id="apiaprendizprocessinstructorget"></a>
# **ApiAprendizProcessInstructorGet**
> List&lt;AprendizProcessInstructorDto&gt; ApiAprendizProcessInstructorGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiAprendizProcessInstructorGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new AprendizProcessInstructorApi(config);

            try
            {
                List<AprendizProcessInstructorDto> result = apiInstance.ApiAprendizProcessInstructorGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AprendizProcessInstructorApi.ApiAprendizProcessInstructorGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAprendizProcessInstructorGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<AprendizProcessInstructorDto>> response = apiInstance.ApiAprendizProcessInstructorGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AprendizProcessInstructorApi.ApiAprendizProcessInstructorGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;AprendizProcessInstructorDto&gt;**](AprendizProcessInstructorDto.md)

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

<a id="apiaprendizprocessinstructoriddelete"></a>
# **ApiAprendizProcessInstructorIdDelete**
> void ApiAprendizProcessInstructorIdDelete (int id)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiAprendizProcessInstructorIdDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new AprendizProcessInstructorApi(config);
            var id = 56;  // int | 

            try
            {
                apiInstance.ApiAprendizProcessInstructorIdDelete(id);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AprendizProcessInstructorApi.ApiAprendizProcessInstructorIdDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAprendizProcessInstructorIdDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiAprendizProcessInstructorIdDeleteWithHttpInfo(id);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AprendizProcessInstructorApi.ApiAprendizProcessInstructorIdDeleteWithHttpInfo: " + e.Message);
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

<a id="apiaprendizprocessinstructoridget"></a>
# **ApiAprendizProcessInstructorIdGet**
> AprendizProcessInstructorDto ApiAprendizProcessInstructorIdGet (int id)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiAprendizProcessInstructorIdGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new AprendizProcessInstructorApi(config);
            var id = 56;  // int | 

            try
            {
                AprendizProcessInstructorDto result = apiInstance.ApiAprendizProcessInstructorIdGet(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AprendizProcessInstructorApi.ApiAprendizProcessInstructorIdGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAprendizProcessInstructorIdGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<AprendizProcessInstructorDto> response = apiInstance.ApiAprendizProcessInstructorIdGetWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AprendizProcessInstructorApi.ApiAprendizProcessInstructorIdGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **int** |  |  |

### Return type

[**AprendizProcessInstructorDto**](AprendizProcessInstructorDto.md)

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

<a id="apiaprendizprocessinstructoridput"></a>
# **ApiAprendizProcessInstructorIdPut**
> AprendizProcessInstructorDto ApiAprendizProcessInstructorIdPut (int id, AprendizProcessInstructorDto aprendizProcessInstructorDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiAprendizProcessInstructorIdPutExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new AprendizProcessInstructorApi(config);
            var id = 56;  // int | 
            var aprendizProcessInstructorDto = new AprendizProcessInstructorDto(); // AprendizProcessInstructorDto |  (optional) 

            try
            {
                AprendizProcessInstructorDto result = apiInstance.ApiAprendizProcessInstructorIdPut(id, aprendizProcessInstructorDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AprendizProcessInstructorApi.ApiAprendizProcessInstructorIdPut: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAprendizProcessInstructorIdPutWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<AprendizProcessInstructorDto> response = apiInstance.ApiAprendizProcessInstructorIdPutWithHttpInfo(id, aprendizProcessInstructorDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AprendizProcessInstructorApi.ApiAprendizProcessInstructorIdPutWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **int** |  |  |
| **aprendizProcessInstructorDto** | [**AprendizProcessInstructorDto**](AprendizProcessInstructorDto.md) |  | [optional]  |

### Return type

[**AprendizProcessInstructorDto**](AprendizProcessInstructorDto.md)

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

<a id="apiaprendizprocessinstructorpost"></a>
# **ApiAprendizProcessInstructorPost**
> AprendizProcessInstructorDto ApiAprendizProcessInstructorPost (AprendizProcessInstructorDto aprendizProcessInstructorDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiAprendizProcessInstructorPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new AprendizProcessInstructorApi(config);
            var aprendizProcessInstructorDto = new AprendizProcessInstructorDto(); // AprendizProcessInstructorDto |  (optional) 

            try
            {
                AprendizProcessInstructorDto result = apiInstance.ApiAprendizProcessInstructorPost(aprendizProcessInstructorDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AprendizProcessInstructorApi.ApiAprendizProcessInstructorPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAprendizProcessInstructorPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<AprendizProcessInstructorDto> response = apiInstance.ApiAprendizProcessInstructorPostWithHttpInfo(aprendizProcessInstructorDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AprendizProcessInstructorApi.ApiAprendizProcessInstructorPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **aprendizProcessInstructorDto** | [**AprendizProcessInstructorDto**](AprendizProcessInstructorDto.md) |  | [optional]  |

### Return type

[**AprendizProcessInstructorDto**](AprendizProcessInstructorDto.md)

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

