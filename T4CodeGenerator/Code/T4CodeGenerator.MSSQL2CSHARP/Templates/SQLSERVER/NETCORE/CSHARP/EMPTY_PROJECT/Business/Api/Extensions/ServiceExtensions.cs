using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Core;
using Core.Services;
using Data;
using Services;

namespace Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        public static void ConfigureDependencyServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IAuditLogService, AuditLogService>();

            services.AddScoped<Core.Miscellaneous.ICryptography, Miscellaneous.Cryptography>();

            ApiAuth.Extensions.ServiceExtensions.ConfigureDependencyServices(services);
        }

        public static void ConfigureSqlServerl(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<DbContextInstance>(
                options =>
                    options.UseSqlServer(configuration.GetConnectionString("Default"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly("Data");
                        sqlOptions.EnableRetryOnFailure(3);
                    }
                )
            );

            services.AddDbContextPool<DataAuth.AuthDbContext>(
                options =>
                   options.UseSqlServer(configuration.GetConnectionString("Default"),
                   sqlOptions =>
                   {
                       sqlOptions.MigrationsAssembly("DataAuth");
                       sqlOptions.EnableRetryOnFailure(3);
                   }
               )
           );
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration config)
        {

            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = config.GetSection("Auth:Issuer").Value,
                    ValidAudience = config.GetSection("Auth:Audience").Value,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Auth:Key").Value))
                };
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            // Agregado para trabajar con Swagger de manera más sencilla, evitando eror 406
            services.AddControllers(config => { config.ReturnHttpNotAcceptable = false; });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Template API", Version = "v1" });


                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                //c.OperationFilter<SecurityRequirementsOperationFilter>();
                //c.OperationFilter<AddHeaderOperationFilter>("authorization", "Authorization for the request", false); // adds any string you like to the request headers - in this case, a correlation id


                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });


                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }

        internal static void ConfigureServicesCommon(this IServiceCollection services)
        {
            services.ConfigureCors();

            services.AddControllers(setupActions =>
            {
                setupActions.ReturnHttpNotAcceptable = true;
            })
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "http://",
                            Title = "One or more model validation errors ocurred.",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "See the errors property for details.",
                            Instance = context.HttpContext.Request.Path
                        };

                        problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problmem+json" }
                        };
                    };
                });
        }
    }
}
