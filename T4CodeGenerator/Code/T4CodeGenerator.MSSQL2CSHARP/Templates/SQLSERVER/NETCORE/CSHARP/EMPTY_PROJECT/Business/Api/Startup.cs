using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Api.Extensions;
using Api.Resources;
using Core.Services;
using Services;

namespace Api
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(setupActions =>
            {
                setupActions.ReturnHttpNotAcceptable = true;
            })
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        return new BadRequestObjectResult(context.ModelState);
                    };
                });

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddMvcCore(x =>
            {
                var authenticatedUserPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
                x.Filters.Add(new AuthorizeFilter(authenticatedUserPolicy));
            });

            services.ConfigureSqlServerl(Configuration);

            services.ConfigureDependencyServices();

            services.ConfigureCors();

            services.ConfigureSwagger();

            services.ConfigureJwt(Configuration);

            services.AddAutoMapper(typeof(Startup));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(config =>
                {
                    config.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            var ex = error.Error;

                            await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResource()
                            {
                                StatusCode = 500,
                                ErrorMessage = ex.Message
                            }));
                        }
                    });
                });
            }

            app.UseSwagger();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty;
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Template API");
            });
        }
    }
}

