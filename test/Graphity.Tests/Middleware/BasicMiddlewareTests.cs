using System;
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
    public class BasicMiddlewareTests : IClassFixture<CustomWebApplicationFactory<BasicStartup>>
    {
        private readonly WebApplicationFactory<BasicStartup> _factory;

        public BasicMiddlewareTests(CustomWebApplicationFactory<BasicStartup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder => builder.UseStartup<BasicStartup>());
        }

        [Fact]
        public async Task Can_retrieve_top_level_entities()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals {id name}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Equal(4, result.Data["animals"].Count());

            var item = result.Data["animals"].First();
            Assert.Equal(1, item.Id);
            Assert.Equal("Dog", item.Name);
            Assert.Null(item.LivesIn);
        }

        [Fact]
        public async Task Can_retrieve_second_level_entities()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals {id name livesIn { name }}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Equal(4, result.Data["animals"].Count());

            var item = result.Data["animals"].First();
            Assert.Equal(1, item.Id);
            Assert.Equal("Dog", item.Name);
            Assert.Equal("England", item.LivesIn.Name);
        }

        [Fact]
        public async Task Can_retrieve_guid_entity_property()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals {id name, handlerId}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Equal(4, result.Data["animals"].Count());

            var item = result.Data["animals"].First();
            Assert.Equal(1, item.Id);
            Assert.Equal("Dog", item.Name);
            Assert.NotEqual(Guid.Empty, item.HandlerId);
            Assert.Null(item.LivesIn);
        }

        [Fact]
        public async Task Can_retrieve_enum_property()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals {id name, animalType}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Equal(4, result.Data["animals"].Count());

            var item = result.Data["animals"].Last();
            Assert.Equal(4, item.Id);
            Assert.Equal("Snake", item.Name);
            Assert.Equal(AnimalType.Reptile, item.AnimalType);
            Assert.Null(item.LivesIn);
        }

        [Fact]
        public async Task Can_retrieve_nullable_enum_property()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals {id name, nullableAnimalType}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Equal(4, result.Data["animals"].Count());

            var item = result.Data["animals"].First();
            Assert.Equal(1, item.Id);
            Assert.Equal("Dog", item.Name);
            Assert.Null(item.NullableAnimalType);
            Assert.Null(item.LivesIn);
        }

        [Fact]
        public async Task Can_use_where_expression()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals(where: [{path: ""name"", comparison: equal, value: ""Snake""}]) {id name}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Single(result.Data["animals"]);

            var item = result.Data["animals"].First();
            Assert.Equal(4, item.Id);
            Assert.Equal("Snake", item.Name);
            Assert.Null(item.LivesIn);
        }

        [Fact]
        public async Task Can_use_basic_filter()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals(filter: ""name = `Snake`"") {id name}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Single(result.Data["animals"]);

            var item = result.Data["animals"].First();
            Assert.Equal(4, item.Id);
            Assert.Equal("Snake", item.Name);
            Assert.Null(item.LivesIn);
        }

        [Fact]
        public async Task Can_use_complex_filter()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals(filter: ""name = `Cat` or numberOfLegs = 2"") {id name}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Equal(2, result.Data["animals"].Count());

            var first = result.Data["animals"].First();
            Assert.Equal(2, first.Id);
            Assert.Equal("Cat", first.Name);
            Assert.Null(first.LivesIn);

            var last = result.Data["animals"].Last();
            Assert.Equal(3, last.Id);
            Assert.Equal("Sloth", last.Name);
            Assert.Null(last.LivesIn);
        }

        [Fact]
        public async Task Passing_an_invalid_filter_will_throw()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals(filter: ""turnip = `Tuesday`"") {id name}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Null(result.Data["animals"]);
            Assert.Single(result.Errors);
        }

        [Fact]
        public async Task Can_use_multiple_where_expressions()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals(where: [{path: ""id"", comparison: greaterThan, value: ""2""}, {path: ""id"", comparison: lessThan, value: ""4""}]) {id name}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Single(result.Data["animals"]);

            var item = result.Data["animals"].First();
            Assert.Equal(3, item.Id);
            Assert.Equal("Sloth", item.Name);
            Assert.Null(item.LivesIn);
        }

        [Fact]
        public async Task Can_use_skip()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals(skip: 2) {id name}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Equal(2, result.Data["animals"].Count());

            var item = result.Data["animals"].First();
            Assert.Equal(3, item.Id);
            Assert.Equal("Sloth", item.Name);
            Assert.Null(item.LivesIn);
        }

        [Fact]
        public async Task Can_use_take()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals(take: 2) {id name}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Equal(2, result.Data["animals"].Count());

            var item = result.Data["animals"].First();
            Assert.Equal(1, item.Id);
            Assert.Equal("Dog", item.Name);
            Assert.Null(item.LivesIn);
        }

        [Fact]
        public async Task Can_use_orderBy()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animals(orderBy: {path: ""livesIn.name""}) {id name livesIn {name}}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Equal(4, result.Data["animals"].Count());

            var item = result.Data["animals"].First();
            Assert.Equal(4, item.Id);
            Assert.Equal("Snake", item.Name);
            Assert.Equal("Australia", item.LivesIn.Name);
        }

        [Fact]
        public async Task Can_query_DbQuery_field()
        {
            var client = _factory.CreateClient();

            var query = new GraphQLQuery
            {
                Query = @"{animalsByCountry { countryId count}}"
            };

            var response = await client.PostAsJsonAsync("/api/graph", query);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GraphExecutionResult<Animal>>();

            Assert.Equal(4, result.Data["animalsByCountry"].Count());
        }
    }
}