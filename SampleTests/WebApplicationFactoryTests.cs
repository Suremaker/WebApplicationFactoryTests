using Microsoft.AspNetCore.Mvc.Testing;
using SampleApi;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SampleTests
{
    public class WebApplicationFactoryTests : IDisposable
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public WebApplicationFactoryTests()
        {
            Startup.NumberOfCalls = 0;
            _factory = new WebApplicationFactory<Startup>();
        }

        [Fact]
        public void WebApplicationFactory_should_initialize_Api_once()
        {
            for (int i = 0; i < 10; ++i)
                _factory.CreateDefaultClient();

            // This looks ok, the server is initialized once
            Assert.Equal(1, Startup.NumberOfCalls);
        }

        [Fact]
        public async Task WebApplicationFactory_Api_initialization_should_be_thread_safe()
        {
            // Requesting HttpClients in multiple tasks should end up in one TestServer instance creation
            await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => Task.Run(() => _factory.CreateDefaultClient())));

            // But it is not...
            Assert.Equal(1, Startup.NumberOfCalls);
        }

        [Fact]
        public void Server_should_be_available()
        {
            // How to initialize it then explicitly?
            Assert.NotNull(new WebApplicationFactory<Startup>().Server);
        }

        public void Dispose()
        {
            _factory?.Dispose();
        }
    }
}
