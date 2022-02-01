using Backgammon.Services.Chat.Api.Hubs;
using Backgammon.Services.Chat.Application;
using Backgammon.Services.Chat.Application.Interfaces;
using Backgammon.Services.Chat.Infra.Data;
using Backgammon.Services.Chat.Infra.Ioc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddData(Configuration);
            services.AddAppServices();
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
                            (path.StartsWithSegments("/hubs/chat")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowedSpecificOrigin", builder =>
                {
                    builder.WithOrigins(new[] { "http://localhost:4200" })
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();                      
                });
            });

            services.AddSignalR();

            services.AddAuthorization();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backgammon.Services.Chat.Api", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,DataContext data,IChatService chatService)
        {
            data.Database.EnsureDeleted();
            data.Database.EnsureCreated();
            chatService.CloseAllConnections();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backgammon.Services.Chat.Api v1"));
            }

            app.UseCors("AllowedSpecificOrigin");

            app.UseRouting();         

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/hubs/chat");
                endpoints.MapControllers();
            });
        }
    }
}
