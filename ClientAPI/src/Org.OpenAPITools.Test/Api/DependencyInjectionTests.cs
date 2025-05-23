/*
 * Web
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Cryptography;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Extensions;
using Xunit;

namespace Org.OpenAPITools.Test.Api
{
    /// <summary>
    ///  Tests the dependency injection.
    /// </summary>
    public class DependencyInjectionTest
    {
        private readonly IHost _hostUsingConfigureWithoutAClient =
            Host.CreateDefaultBuilder([]).ConfigureApi((context, services, options) =>
            {

            })
            .Build();

        private readonly IHost _hostUsingConfigureWithAClient =
            Host.CreateDefaultBuilder([]).ConfigureApi((context, services, options) =>
            {

                options.AddApiHttpClients(client => client.BaseAddress = new Uri(ClientUtils.BASE_ADDRESS));
            })
            .Build();

        private readonly IHost _hostUsingAddWithoutAClient =
            Host.CreateDefaultBuilder([]).ConfigureServices((host, services) =>
            {
                services.AddApi(options =>
                {

                });
            })
            .Build();

        private readonly IHost _hostUsingAddWithAClient =
            Host.CreateDefaultBuilder([]).ConfigureServices((host, services) =>
            {
                services.AddApi(options =>
                {

                    options.AddApiHttpClients(client => client.BaseAddress = new Uri(ClientUtils.BASE_ADDRESS));
                });
            })
            .Build();

        /// <summary>
        /// Test dependency injection when using the configure method
        /// </summary>
        [Fact]
        public void ConfigureApiWithAClientTest()
        {
            var aprendizApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IAprendizApi>();
            Assert.True(aprendizApi.HttpClient.BaseAddress != null);

            var aprendizProcessInstructorApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IAprendizProcessInstructorApi>();
            Assert.True(aprendizProcessInstructorApi.HttpClient.BaseAddress != null);

            var aprendizProgramApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IAprendizProgramApi>();
            Assert.True(aprendizProgramApi.HttpClient.BaseAddress != null);

            var centerApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<ICenterApi>();
            Assert.True(centerApi.HttpClient.BaseAddress != null);

            var conceptApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IConceptApi>();
            Assert.True(conceptApi.HttpClient.BaseAddress != null);

            var enterpriseApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IEnterpriseApi>();
            Assert.True(enterpriseApi.HttpClient.BaseAddress != null);

            var formApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IFormApi>();
            Assert.True(formApi.HttpClient.BaseAddress != null);

            var formModuleApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IFormModuleApi>();
            Assert.True(formModuleApi.HttpClient.BaseAddress != null);

            var instructorApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IInstructorApi>();
            Assert.True(instructorApi.HttpClient.BaseAddress != null);

            var instructorProgramApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IInstructorProgramApi>();
            Assert.True(instructorProgramApi.HttpClient.BaseAddress != null);

            var moduleApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IModuleApi>();
            Assert.True(moduleApi.HttpClient.BaseAddress != null);

            var personApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IPersonApi>();
            Assert.True(personApi.HttpClient.BaseAddress != null);

            var processApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IProcessApi>();
            Assert.True(processApi.HttpClient.BaseAddress != null);

            var programApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IProgramApi>();
            Assert.True(programApi.HttpClient.BaseAddress != null);

            var regionalApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IRegionalApi>();
            Assert.True(regionalApi.HttpClient.BaseAddress != null);

            var registerySofiaApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IRegisterySofiaApi>();
            Assert.True(registerySofiaApi.HttpClient.BaseAddress != null);

            var rolApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IRolApi>();
            Assert.True(rolApi.HttpClient.BaseAddress != null);

            var rolFormApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IRolFormApi>();
            Assert.True(rolFormApi.HttpClient.BaseAddress != null);

            var rolUserApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IRolUserApi>();
            Assert.True(rolUserApi.HttpClient.BaseAddress != null);

            var sedeApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<ISedeApi>();
            Assert.True(sedeApi.HttpClient.BaseAddress != null);

            var stateApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IStateApi>();
            Assert.True(stateApi.HttpClient.BaseAddress != null);

            var typeModalityApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<ITypeModalityApi>();
            Assert.True(typeModalityApi.HttpClient.BaseAddress != null);

            var userApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IUserApi>();
            Assert.True(userApi.HttpClient.BaseAddress != null);

            var userRolApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IUserRolApi>();
            Assert.True(userRolApi.HttpClient.BaseAddress != null);

            var userSedeApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IUserSedeApi>();
            Assert.True(userSedeApi.HttpClient.BaseAddress != null);

            var verificationApi = _hostUsingConfigureWithAClient.Services.GetRequiredService<IVerificationApi>();
            Assert.True(verificationApi.HttpClient.BaseAddress != null);
        }

        /// <summary>
        /// Test dependency injection when using the configure method
        /// </summary>
        [Fact]
        public void ConfigureApiWithoutAClientTest()
        {
            var aprendizApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IAprendizApi>();
            Assert.True(aprendizApi.HttpClient.BaseAddress != null);

            var aprendizProcessInstructorApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IAprendizProcessInstructorApi>();
            Assert.True(aprendizProcessInstructorApi.HttpClient.BaseAddress != null);

            var aprendizProgramApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IAprendizProgramApi>();
            Assert.True(aprendizProgramApi.HttpClient.BaseAddress != null);

            var centerApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<ICenterApi>();
            Assert.True(centerApi.HttpClient.BaseAddress != null);

            var conceptApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IConceptApi>();
            Assert.True(conceptApi.HttpClient.BaseAddress != null);

            var enterpriseApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IEnterpriseApi>();
            Assert.True(enterpriseApi.HttpClient.BaseAddress != null);

            var formApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IFormApi>();
            Assert.True(formApi.HttpClient.BaseAddress != null);

            var formModuleApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IFormModuleApi>();
            Assert.True(formModuleApi.HttpClient.BaseAddress != null);

            var instructorApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IInstructorApi>();
            Assert.True(instructorApi.HttpClient.BaseAddress != null);

            var instructorProgramApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IInstructorProgramApi>();
            Assert.True(instructorProgramApi.HttpClient.BaseAddress != null);

            var moduleApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IModuleApi>();
            Assert.True(moduleApi.HttpClient.BaseAddress != null);

            var personApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IPersonApi>();
            Assert.True(personApi.HttpClient.BaseAddress != null);

            var processApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IProcessApi>();
            Assert.True(processApi.HttpClient.BaseAddress != null);

            var programApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IProgramApi>();
            Assert.True(programApi.HttpClient.BaseAddress != null);

            var regionalApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IRegionalApi>();
            Assert.True(regionalApi.HttpClient.BaseAddress != null);

            var registerySofiaApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IRegisterySofiaApi>();
            Assert.True(registerySofiaApi.HttpClient.BaseAddress != null);

            var rolApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IRolApi>();
            Assert.True(rolApi.HttpClient.BaseAddress != null);

            var rolFormApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IRolFormApi>();
            Assert.True(rolFormApi.HttpClient.BaseAddress != null);

            var rolUserApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IRolUserApi>();
            Assert.True(rolUserApi.HttpClient.BaseAddress != null);

            var sedeApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<ISedeApi>();
            Assert.True(sedeApi.HttpClient.BaseAddress != null);

            var stateApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IStateApi>();
            Assert.True(stateApi.HttpClient.BaseAddress != null);

            var typeModalityApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<ITypeModalityApi>();
            Assert.True(typeModalityApi.HttpClient.BaseAddress != null);

            var userApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IUserApi>();
            Assert.True(userApi.HttpClient.BaseAddress != null);

            var userRolApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IUserRolApi>();
            Assert.True(userRolApi.HttpClient.BaseAddress != null);

            var userSedeApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IUserSedeApi>();
            Assert.True(userSedeApi.HttpClient.BaseAddress != null);

            var verificationApi = _hostUsingConfigureWithoutAClient.Services.GetRequiredService<IVerificationApi>();
            Assert.True(verificationApi.HttpClient.BaseAddress != null);
        }

        /// <summary>
        /// Test dependency injection when using the add method
        /// </summary>
        [Fact]
        public void AddApiWithAClientTest()
        {
            var aprendizApi = _hostUsingAddWithAClient.Services.GetRequiredService<IAprendizApi>();
            Assert.True(aprendizApi.HttpClient.BaseAddress != null);
            
            var aprendizProcessInstructorApi = _hostUsingAddWithAClient.Services.GetRequiredService<IAprendizProcessInstructorApi>();
            Assert.True(aprendizProcessInstructorApi.HttpClient.BaseAddress != null);
            
            var aprendizProgramApi = _hostUsingAddWithAClient.Services.GetRequiredService<IAprendizProgramApi>();
            Assert.True(aprendizProgramApi.HttpClient.BaseAddress != null);
            
            var centerApi = _hostUsingAddWithAClient.Services.GetRequiredService<ICenterApi>();
            Assert.True(centerApi.HttpClient.BaseAddress != null);
            
            var conceptApi = _hostUsingAddWithAClient.Services.GetRequiredService<IConceptApi>();
            Assert.True(conceptApi.HttpClient.BaseAddress != null);
            
            var enterpriseApi = _hostUsingAddWithAClient.Services.GetRequiredService<IEnterpriseApi>();
            Assert.True(enterpriseApi.HttpClient.BaseAddress != null);
            
            var formApi = _hostUsingAddWithAClient.Services.GetRequiredService<IFormApi>();
            Assert.True(formApi.HttpClient.BaseAddress != null);
            
            var formModuleApi = _hostUsingAddWithAClient.Services.GetRequiredService<IFormModuleApi>();
            Assert.True(formModuleApi.HttpClient.BaseAddress != null);
            
            var instructorApi = _hostUsingAddWithAClient.Services.GetRequiredService<IInstructorApi>();
            Assert.True(instructorApi.HttpClient.BaseAddress != null);
            
            var instructorProgramApi = _hostUsingAddWithAClient.Services.GetRequiredService<IInstructorProgramApi>();
            Assert.True(instructorProgramApi.HttpClient.BaseAddress != null);
            
            var moduleApi = _hostUsingAddWithAClient.Services.GetRequiredService<IModuleApi>();
            Assert.True(moduleApi.HttpClient.BaseAddress != null);
            
            var personApi = _hostUsingAddWithAClient.Services.GetRequiredService<IPersonApi>();
            Assert.True(personApi.HttpClient.BaseAddress != null);
            
            var processApi = _hostUsingAddWithAClient.Services.GetRequiredService<IProcessApi>();
            Assert.True(processApi.HttpClient.BaseAddress != null);
            
            var programApi = _hostUsingAddWithAClient.Services.GetRequiredService<IProgramApi>();
            Assert.True(programApi.HttpClient.BaseAddress != null);
            
            var regionalApi = _hostUsingAddWithAClient.Services.GetRequiredService<IRegionalApi>();
            Assert.True(regionalApi.HttpClient.BaseAddress != null);
            
            var registerySofiaApi = _hostUsingAddWithAClient.Services.GetRequiredService<IRegisterySofiaApi>();
            Assert.True(registerySofiaApi.HttpClient.BaseAddress != null);
            
            var rolApi = _hostUsingAddWithAClient.Services.GetRequiredService<IRolApi>();
            Assert.True(rolApi.HttpClient.BaseAddress != null);
            
            var rolFormApi = _hostUsingAddWithAClient.Services.GetRequiredService<IRolFormApi>();
            Assert.True(rolFormApi.HttpClient.BaseAddress != null);
            
            var rolUserApi = _hostUsingAddWithAClient.Services.GetRequiredService<IRolUserApi>();
            Assert.True(rolUserApi.HttpClient.BaseAddress != null);
            
            var sedeApi = _hostUsingAddWithAClient.Services.GetRequiredService<ISedeApi>();
            Assert.True(sedeApi.HttpClient.BaseAddress != null);
            
            var stateApi = _hostUsingAddWithAClient.Services.GetRequiredService<IStateApi>();
            Assert.True(stateApi.HttpClient.BaseAddress != null);
            
            var typeModalityApi = _hostUsingAddWithAClient.Services.GetRequiredService<ITypeModalityApi>();
            Assert.True(typeModalityApi.HttpClient.BaseAddress != null);
            
            var userApi = _hostUsingAddWithAClient.Services.GetRequiredService<IUserApi>();
            Assert.True(userApi.HttpClient.BaseAddress != null);
            
            var userRolApi = _hostUsingAddWithAClient.Services.GetRequiredService<IUserRolApi>();
            Assert.True(userRolApi.HttpClient.BaseAddress != null);
            
            var userSedeApi = _hostUsingAddWithAClient.Services.GetRequiredService<IUserSedeApi>();
            Assert.True(userSedeApi.HttpClient.BaseAddress != null);
            
            var verificationApi = _hostUsingAddWithAClient.Services.GetRequiredService<IVerificationApi>();
            Assert.True(verificationApi.HttpClient.BaseAddress != null);
        }

        /// <summary>
        /// Test dependency injection when using the add method
        /// </summary>
        [Fact]
        public void AddApiWithoutAClientTest()
        {
            var aprendizApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IAprendizApi>();
            Assert.True(aprendizApi.HttpClient.BaseAddress != null);

            var aprendizProcessInstructorApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IAprendizProcessInstructorApi>();
            Assert.True(aprendizProcessInstructorApi.HttpClient.BaseAddress != null);

            var aprendizProgramApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IAprendizProgramApi>();
            Assert.True(aprendizProgramApi.HttpClient.BaseAddress != null);

            var centerApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<ICenterApi>();
            Assert.True(centerApi.HttpClient.BaseAddress != null);

            var conceptApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IConceptApi>();
            Assert.True(conceptApi.HttpClient.BaseAddress != null);

            var enterpriseApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IEnterpriseApi>();
            Assert.True(enterpriseApi.HttpClient.BaseAddress != null);

            var formApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IFormApi>();
            Assert.True(formApi.HttpClient.BaseAddress != null);

            var formModuleApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IFormModuleApi>();
            Assert.True(formModuleApi.HttpClient.BaseAddress != null);

            var instructorApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IInstructorApi>();
            Assert.True(instructorApi.HttpClient.BaseAddress != null);

            var instructorProgramApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IInstructorProgramApi>();
            Assert.True(instructorProgramApi.HttpClient.BaseAddress != null);

            var moduleApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IModuleApi>();
            Assert.True(moduleApi.HttpClient.BaseAddress != null);

            var personApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IPersonApi>();
            Assert.True(personApi.HttpClient.BaseAddress != null);

            var processApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IProcessApi>();
            Assert.True(processApi.HttpClient.BaseAddress != null);

            var programApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IProgramApi>();
            Assert.True(programApi.HttpClient.BaseAddress != null);

            var regionalApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IRegionalApi>();
            Assert.True(regionalApi.HttpClient.BaseAddress != null);

            var registerySofiaApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IRegisterySofiaApi>();
            Assert.True(registerySofiaApi.HttpClient.BaseAddress != null);

            var rolApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IRolApi>();
            Assert.True(rolApi.HttpClient.BaseAddress != null);

            var rolFormApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IRolFormApi>();
            Assert.True(rolFormApi.HttpClient.BaseAddress != null);

            var rolUserApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IRolUserApi>();
            Assert.True(rolUserApi.HttpClient.BaseAddress != null);

            var sedeApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<ISedeApi>();
            Assert.True(sedeApi.HttpClient.BaseAddress != null);

            var stateApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IStateApi>();
            Assert.True(stateApi.HttpClient.BaseAddress != null);

            var typeModalityApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<ITypeModalityApi>();
            Assert.True(typeModalityApi.HttpClient.BaseAddress != null);

            var userApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IUserApi>();
            Assert.True(userApi.HttpClient.BaseAddress != null);

            var userRolApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IUserRolApi>();
            Assert.True(userRolApi.HttpClient.BaseAddress != null);

            var userSedeApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IUserSedeApi>();
            Assert.True(userSedeApi.HttpClient.BaseAddress != null);

            var verificationApi = _hostUsingAddWithoutAClient.Services.GetRequiredService<IVerificationApi>();
            Assert.True(verificationApi.HttpClient.BaseAddress != null);
        }
    }
}
