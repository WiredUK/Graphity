using System.Linq;
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
    }
}