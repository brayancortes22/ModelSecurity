using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Tests;

public class ApiClientTests
{
    private readonly HttpClient _httpClient;
    private readonly IServiceProvider _serviceProvider;
    
    public ApiClientTests()
    {
        // Configurar servicios necesarios
        var services = new ServiceCollection();
        
        // Configurar el HttpClient para conectarse a tu API
        services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7228"); // Ajusta el puerto según tu aplicación
        });
        
        // Agregar servicios necesarios para los clientes API generados
        services.AddLogging();
        services.AddSingleton<JsonSerializerOptionsProvider>();
        services.AddSingleton<PersonApiEvents>();
        services.AddSingleton<UserApiEvents>();
        
        _serviceProvider = services.BuildServiceProvider();
        
        // Configurar el HttpClient
        var factory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
        _httpClient = factory.CreateClient("ApiClient");
    }

    [Fact]
    public async Task TestPersonEndpoint()
    {
        // Arrange
        var logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<PersonApi>();
        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        var jsonOptions = _serviceProvider.GetRequiredService<JsonSerializerOptionsProvider>();
        var events = _serviceProvider.GetRequiredService<PersonApiEvents>();
        
        var apiClient = new PersonApi(logger, loggerFactory, _httpClient, jsonOptions, events);
        
        try
        {
            // Act
            // Ejemplo: obtener todas las personas
            var response = await apiClient.ApiPersonGetAsync();
            
            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsOk, "La respuesta no fue exitosa");
            
            // Si la respuesta es exitosa, verificamos los datos
            if (response.IsOk)
            {
                var persons = response.Ok();
                Assert.NotNull(persons);
            }
        }
        catch (Exception ex)
        {
            // En caso de error, la prueba fallará y mostrará el mensaje de error
            Assert.True(false, $"La prueba falló con la excepción: {ex.Message}");
        }
    }
    
    [Fact]
    public async Task TestUserEndpoint()
    {
        // Arrange
        var logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<UserApi>();
        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        var jsonOptions = _serviceProvider.GetRequiredService<JsonSerializerOptionsProvider>();
        var events = _serviceProvider.GetRequiredService<UserApiEvents>();
        
        var apiClient = new UserApi(logger, loggerFactory, _httpClient, jsonOptions, events);
        
        try
        {
            // Act
            // Ejemplo: obtener todos los usuarios
            var response = await apiClient.ApiUserGetAsync();
            
            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsOk, "La respuesta no fue exitosa");
            
            // Si la respuesta es exitosa, verificamos los datos
            if (response.IsOk)
            {
                var users = response.Ok();
                Assert.NotNull(users);
            }
        }
        catch (Exception ex)
        {
            Assert.True(false, $"La prueba falló con la excepción: {ex.Message}");
        }
    }
}
