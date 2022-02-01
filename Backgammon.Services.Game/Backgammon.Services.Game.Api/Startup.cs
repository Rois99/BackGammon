using Backgammon.Services.Game.Api.HubConfig;
using Backgammon.Services.Game.App.Interfaces;
using Backgammon.Services.Game.App.Services;
using Backgammon.Services.Game.Domain.Interfaces;
using Backgammon.Services.Game.Infra;
using Backgammon.Services.Game.Infra.Extensions;
using Backgammon.Services.Game.Infra.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Api
{
    public class Startup
    {
        IConfiguration _configuration;
        IWebHostEnvironment _env;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<ICubeService, CubesService>();
            services.AddSingleton<IBoardManager, BoardsManager>();
            services.AddScoped<IPlayerService, PlayersService>();
            services.AddScoped<IRepository, Repository>();
            services.AddControllers();

            string conncetionString = _configuration.GetConnectionString("Def");
            services.AddDbContext<GameDataContext>(options => options.UseSqlServer(conncetionString));

            SecurityKey securityKey = null;

            services.AddSecrets(credProvider => securityKey = credProvider.GetSingnigKey());

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        //ValidIssuer = authSettings.Issuer,
                        //ValidAudiences = authSettings.Audiences,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = securityKey,
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            if (string.IsNullOrEmpty(accessToken))
                            {
                                var auth = context.Request.Headers["Authorization"];
                                if (!string.IsNullOrEmpty(auth))
                                    accessToken = auth.ToString().Split(' ')[1];
                            }
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/game")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
                    builder =>
                    {
                        builder.WithOrigins(new[] { "http://localhost:4200" })
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, GameDataContext gamesContext)
        {
            //DbConfiguration
            //gamesContext.Database.EnsureDeleted();
            //gamesContext.Database.EnsureCreated();

            app.UseCors("AllowAllHeaders");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }  


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Game}/{action=RollCubess}/{id?}");
                endpoints.MapHub<GameHub>("/game");
            });
        }
    }
}
