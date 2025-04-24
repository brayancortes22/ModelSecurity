# Org.OpenAPITools.Api.RegisterySofiaApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiRegisterySofiaGet**](RegisterySofiaApi.md#apiregisterysofiaget) | **GET** /api/RegisterySofia |  |
| [**ApiRegisterySofiaIdDelete**](RegisterySofiaApi.md#apiregisterysofiaiddelete) | **DELETE** /api/RegisterySofia/{id} |  |
| [**ApiRegisterySofiaIdGet**](RegisterySofiaApi.md#apiregisterysofiaidget) | **GET** /api/RegisterySofia/{id} |  |
| [**ApiRegisterySofiaIdPatch**](RegisterySofiaApi.md#apiregisterysofiaidpatch) | **PATCH** /api/RegisterySofia/{id} |  |
| [**ApiRegisterySofiaIdPut**](RegisterySofiaApi.md#apiregisterysofiaidput) | **PUT** /api/RegisterySofia/{id} |  |
| [**ApiRegisterySofiaIdSoftDelete**](RegisterySofiaApi.md#apiregisterysofiaidsoftdelete) | **DELETE** /api/RegisterySofia/{id}/soft |  |
| [**ApiRegisterySofiaPost**](RegisterySofiaApi.md#apiregisterysofiapost) | **POST** /api/RegisterySofia |  |

<a id="apiregisterysofiaget"></a>
# **ApiRegisterySofiaGet**
> List&lt;RegisterySofiaDto&gt; ApiRegisterySofiaGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiRegisterySofiaGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RegisterySofiaApi(config);

            try
            {
                List<RegisterySofiaDto> result = apiInstance.ApiRegisterySofiaGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiRegisterySofiaGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<RegisterySofiaDto>> response = apiInstance.ApiRegisterySofiaGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;RegisterySofiaDto&gt;**](RegisterySofiaDto.md)

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

<a id="apiregisterysofiaiddelete"></a>
# **ApiRegisterySofiaIdDelete**
> void ApiRegisterySofiaIdDelete (int id)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiRegisterySofiaIdDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RegisterySofiaApi(config);
            var id = 56;  // int | 

            try
            {
                apiInstance.ApiRegisterySofiaIdDelete(id);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaIdDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiRegisterySofiaIdDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiRegisterySofiaIdDeleteWithHttpInfo(id);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaIdDeleteWithHttpInfo: " + e.Message);
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

<a id="apiregisterysofiaidget"></a>
# **ApiRegisterySofiaIdGet**
> RegisterySofiaDto ApiRegisterySofiaIdGet (int id)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiRegisterySofiaIdGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RegisterySofiaApi(config);
            var id = 56;  // int | 

            try
            {
                RegisterySofiaDto result = apiInstance.ApiRegisterySofiaIdGet(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaIdGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiRegisterySofiaIdGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<RegisterySofiaDto> response = apiInstance.ApiRegisterySofiaIdGetWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaIdGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **int** |  |  |

### Return type

[**RegisterySofiaDto**](RegisterySofiaDto.md)

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

<a id="apiregisterysofiaidpatch"></a>
# **ApiRegisterySofiaIdPatch**
> RegisterySofiaDto ApiRegisterySofiaIdPatch (int id, RegisterySofiaDto registerySofiaDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiRegisterySofiaIdPatchExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RegisterySofiaApi(config);
            var id = 56;  // int | 
            var registerySofiaDto = new RegisterySofiaDto(); // RegisterySofiaDto |  (optional) 

            try
            {
                RegisterySofiaDto result = apiInstance.ApiRegisterySofiaIdPatch(id, registerySofiaDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaIdPatch: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiRegisterySofiaIdPatchWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<RegisterySofiaDto> response = apiInstance.ApiRegisterySofiaIdPatchWithHttpInfo(id, registerySofiaDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaIdPatchWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **int** |  |  |
| **registerySofiaDto** | [**RegisterySofiaDto**](RegisterySofiaDto.md) |  | [optional]  |

### Return type

[**RegisterySofiaDto**](RegisterySofiaDto.md)

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

<a id="apiregisterysofiaidput"></a>
# **ApiRegisterySofiaIdPut**
> RegisterySofiaDto ApiRegisterySofiaIdPut (int id, RegisterySofiaDto registerySofiaDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiRegisterySofiaIdPutExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RegisterySofiaApi(config);
            var id = 56;  // int | 
            var registerySofiaDto = new RegisterySofiaDto(); // RegisterySofiaDto |  (optional) 

            try
            {
                RegisterySofiaDto result = apiInstance.ApiRegisterySofiaIdPut(id, registerySofiaDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaIdPut: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiRegisterySofiaIdPutWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<RegisterySofiaDto> response = apiInstance.ApiRegisterySofiaIdPutWithHttpInfo(id, registerySofiaDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaIdPutWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **int** |  |  |
| **registerySofiaDto** | [**RegisterySofiaDto**](RegisterySofiaDto.md) |  | [optional]  |

### Return type

[**RegisterySofiaDto**](RegisterySofiaDto.md)

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

<a id="apiregisterysofiaidsoftdelete"></a>
# **ApiRegisterySofiaIdSoftDelete**
> void ApiRegisterySofiaIdSoftDelete (int id)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiRegisterySofiaIdSoftDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RegisterySofiaApi(config);
            var id = 56;  // int | 

            try
            {
                apiInstance.ApiRegisterySofiaIdSoftDelete(id);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaIdSoftDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiRegisterySofiaIdSoftDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiRegisterySofiaIdSoftDeleteWithHttpInfo(id);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaIdSoftDeleteWithHttpInfo: " + e.Message);
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

<a id="apiregisterysofiapost"></a>
# **ApiRegisterySofiaPost**
> RegisterySofiaDto ApiRegisterySofiaPost (RegisterySofiaDto registerySofiaDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiRegisterySofiaPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RegisterySofiaApi(config);
            var registerySofiaDto = new RegisterySofiaDto(); // RegisterySofiaDto |  (optional) 

            try
            {
                RegisterySofiaDto result = apiInstance.ApiRegisterySofiaPost(registerySofiaDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiRegisterySofiaPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<RegisterySofiaDto> response = apiInstance.ApiRegisterySofiaPostWithHttpInfo(registerySofiaDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RegisterySofiaApi.ApiRegisterySofiaPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **registerySofiaDto** | [**RegisterySofiaDto**](RegisterySofiaDto.md) |  | [optional]  |

### Return type

[**RegisterySofiaDto**](RegisterySofiaDto.md)

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

