/*
 * Web
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;


/* *********************************************************************************
*              Follow these manual steps to construct tests.
*              This file will not be overwritten.
*  *********************************************************************************
* 1. Navigate to ApiTests.Base.cs and ensure any tokens are being created correctly.
*    Take care not to commit credentials to any repository.
*
* 2. Mocking is coordinated by ApiTestsBase#AddApiHttpClients.
*    To mock the client, use the generic AddApiHttpClients.
*    To mock the server, change the client's BaseAddress.
*
* 3. Locate the test you want below
*      - remove the skip property from the Fact attribute
*      - set the value of any variables if necessary
*
* 4. Run the tests and ensure they work.
*
*/


namespace Org.OpenAPITools.Test.Api
{
    /// <summary>
    ///  Class for testing RegionalApi
    /// </summary>
    public sealed class RegionalApiTests : ApiTestsBase
    {
        private readonly IRegionalApi _instance;

        public RegionalApiTests(): base(Array.Empty<string>())
        {
            _instance = _host.Services.GetRequiredService<IRegionalApi>();
        }

        /// <summary>
        /// Test ApiRegionalGet
        /// </summary>
        [Fact (Skip = "not implemented")]
        public async Task ApiRegionalGetAsyncTest()
        {
            var response = await _instance.ApiRegionalGetAsync();
            var model = response.Ok();
            Assert.IsType<List<RegionalDto>>(model);
        }

        /// <summary>
        /// Test ApiRegionalIdDelete
        /// </summary>
        [Fact (Skip = "not implemented")]
        public async Task ApiRegionalIdDeleteAsyncTest()
        {
            int id = default!;
            await _instance.ApiRegionalIdDeleteAsync(id);
        }

        /// <summary>
        /// Test ApiRegionalIdGet
        /// </summary>
        [Fact (Skip = "not implemented")]
        public async Task ApiRegionalIdGetAsyncTest()
        {
            int id = default!;
            var response = await _instance.ApiRegionalIdGetAsync(id);
            var model = response.Ok();
            Assert.IsType<RegionalDto>(model);
        }

        /// <summary>
        /// Test ApiRegionalIdPatch
        /// </summary>
        [Fact (Skip = "not implemented")]
        public async Task ApiRegionalIdPatchAsyncTest()
        {
            int id = default!;
            Client.Option<RegionalDto> regionalDto = default!;
            var response = await _instance.ApiRegionalIdPatchAsync(id, regionalDto);
            var model = response.Ok();
            Assert.IsType<RegionalDto>(model);
        }

        /// <summary>
        /// Test ApiRegionalIdPut
        /// </summary>
        [Fact (Skip = "not implemented")]
        public async Task ApiRegionalIdPutAsyncTest()
        {
            int id = default!;
            Client.Option<RegionalDto> regionalDto = default!;
            var response = await _instance.ApiRegionalIdPutAsync(id, regionalDto);
            var model = response.Ok();
            Assert.IsType<RegionalDto>(model);
        }

        /// <summary>
        /// Test ApiRegionalIdSoftDelete
        /// </summary>
        [Fact (Skip = "not implemented")]
        public async Task ApiRegionalIdSoftDeleteAsyncTest()
        {
            int id = default!;
            await _instance.ApiRegionalIdSoftDeleteAsync(id);
        }

        /// <summary>
        /// Test ApiRegionalPost
        /// </summary>
        [Fact (Skip = "not implemented")]
        public async Task ApiRegionalPostAsyncTest()
        {
            Client.Option<RegionalDto> regionalDto = default!;
            var response = await _instance.ApiRegionalPostAsync(regionalDto);
            var model = response.Created();
            Assert.IsType<RegionalDto>(model);
        }
    }
}
