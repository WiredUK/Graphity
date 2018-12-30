using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Graphity.Tests.Fixtures;
using Graphity.Tests.Fixtures.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Graphity.Tests.Middleware
{
    [Collection("Middleware Tests")]
    public class ComplexMiddlewareTests : IClassFixture<CustomWebApplicationFactory<ComplexStartup>>
    {
        private readonly WebApplicationFactory<ComplexStartup> _factory;

        public ComplexMiddlewareTests(CustomWebApplicationFactory<ComplexStartup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder => builder.UseStartup<ComplexStartup>());
        }

        [Fact]
        public async Task Can_retrieve_top_level_entities_with_renamed_type()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{filteredAnimals {name}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Equal(3, result.Data["filteredAnimals"].Count());
            var item = result.Data["filteredAnimals"].First();
            Assert.Equal("Cat", item.Name);
            Assert.Null(item.LivesIn);
        }

        [Fact]
        public async Task Can_retrieve_second_level_entities()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{filteredAnimals {name livesIn { name }}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Equal(3, result.Data["filteredAnimals"].Count());

            var item = result.Data["filteredAnimals"].First();
            Assert.Equal("Cat", item.Name);
            Assert.Equal("France", item.LivesIn.Name);
        }

        [Fact]
        public async Task Calling_field_without_auth_will_return_bad_request()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{countryProperties { population }}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<CountryProperties>>();

            Assert.Null(result.Data);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task Calling_field_with_func_policy_will_fail_first_time()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{country { name }}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            //Call it again...
            response = await client.PostAsJsonAsync("/api/graph", query);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Country>>();

            Assert.NotEmpty(result.Data["country"]);
            Assert.Null(result.Errors);
        }
    }
}