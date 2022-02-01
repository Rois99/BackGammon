using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Backgammon.Services.Identity.Infra.Data.Contexts;

namespace Backgammon.Services.Identity.Api.IntegrationTests
{
    public abstract class IntegrationTest
    {
        protected readonly HttpClient TestClient;
        private readonly IServiceProvider serviceProvider;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(config => {
                    config.ConfigureServices(services =>
                    {
                        services.RemoveAll<DbContextOptionsBuilder<DataContext>>();
                        services.RemoveAll<DbContextOptions<DataContext>>();
                        services.RemoveAll<DataContext>();

                        services.AddDbContext<DataContext>(options =>
                        {
                            options.UseLazyLoadingProxies()
                            .UseInMemoryDatabase(databaseName: "Test");
                        });
                    });
                });

            serviceProvider = appFactory.Services;

            TestClient = appFactory.CreateClient();
        }

        [TestInitialize]
        public async virtual Task SetUp()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                using var context = scope.ServiceProvider.GetService<DataContext>();
                await context.Database.EnsureCreatedAsync();
            }
        }

        [TestCleanup]
        public async virtual Task CleanUp()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                using var context = scope.ServiceProvider.GetService<DataContext>();
                await context.Database.EnsureDeletedAsync();
            }

        }
    }
}
