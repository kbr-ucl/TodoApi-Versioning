using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TodoApplication.Api.Models;

namespace TodoApplication.Api
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TodoContext>(opt =>
                opt.UseInMemoryDatabase("TodoList"));
            services.AddControllers();

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1.0", new OpenApiInfo {Title = "ToDo service", Version = "v1.0"});
            //    c.SwaggerDoc("v2.0", new OpenApiInfo { Title = "ToDo service", Version = "v2.0" });
            //    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //    //c.IncludeXmlComments(xmlPath);
            //});

            services.AddApiVersioningAndExplorer();

            services.AddSwaggerGeneration();


            // https://github.com/microsoft/aspnet-api-versioning/wiki/API-Version-Reader
            // http://sundeepkamath.in/posts/rest-api-versioning-in-aspnet-core-part-1/
            // https://github.com/sundeepkamath/RESTAPIVersioning

            //Check https://github.com/Microsoft/aspnet-api-versioning/wiki/API-Versioning-Options for details on these properties
            //services.AddApiVersioning(options =>
            //{
            //    options.ReportApiVersions = true;
            //    options.AssumeDefaultVersionWhenUnspecified = true;
            //    options.DefaultApiVersion = new ApiVersion(1, 0);

            //    //Query String Approach - Default - api-version
            //    //options.ApiVersionReader = new QueryStringApiVersionReader();

            //    //Query String Approach - Custom
            //    //options.ApiVersionReader = new QueryStringApiVersionReader("v1");

            //    //Http Header Approach - No default, only custom
            //    //options.ApiVersionReader = new HeaderApiVersionReader("api-version");

            //    //Media type approach - Default - v
            //    options.ApiVersionReader = new MediaTypeApiVersionReader();
            //    options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);


            //    //Media type approach - custom
            //    //options.ApiVersionReader = new MediaTypeApiVersionReader("version");
            //});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            // app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUIAndAddApiVersionEndPointBuilder(provider);

            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "ToDo service V1.0");
            //    c.SwaggerEndpoint("/swagger/v2.0/swagger.json", "ToDo service V2.0");
            //    c.RoutePrefix = string.Empty;
            //});

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }

    public static class SwaggerGenerationSetupExtensionMethods
    {
        public static void AddSwaggerGeneration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Resolve the IApiVersionDescriptionProvider service
                // Note: that we have to build a temporary service provider here because one has not been created yet
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    var provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

                    // Add a swagger document for each discovered API version
                    // Note: you might choose to skip or document deprecated API versions differently
                    foreach (var description in provider.ApiVersionDescriptions)
                        options.SwaggerDoc(description.GroupName, description.CreateInfoForApiVersion());
                }

                // Add a custom operation filter which sets default values if you are using a separate header for version
                //options.OperationFilter<SwaggerDefaultValues>();

                // Integrate xml comments, add this when we have come that far.
                //options.IncludeXmlComments(XmlCommentsFilePath);

                // Describe all enums as strings instead of integers.
                //options.DescribeAllEnumsAsStrings();
            });
        }

        public static OpenApiInfo CreateInfoForApiVersion(this ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Title = $"Test API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "This is the swagger base info for API.",
                Contact = new OpenApiContact {Name = "Contact Name", Email = "some@email.com"},
                //TermsOfService = "UnComment and put the URI here to the terms of service.",
                License = new OpenApiLicense {Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT")}
            };

            if (description.IsDeprecated) info.Description += " This API version has been deprecated.";

            return info;
        }

        public static void UseSwaggerUIAndAddApiVersionEndPointBuilder(this IApplicationBuilder app,
            IApiVersionDescriptionProvider provider)
        {
            app.UseSwaggerUI(c =>
            {
                // Build a swagger endpoint for each discovered API version
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                    c.RoutePrefix = string.Empty;
                }
            });
        }
    }

    public static class ApiVersionSetupExtensionMethods
    {
        public static void AddApiVersioningAndExplorer(this IServiceCollection services)
        {
            // Add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // Note: the specified format code will format the version as "'v'major[.minor][-status]"
            services.AddVersionedApiExplorer(
                options => { options.GroupNameFormat = "'v'VVV"; });

            services.AddApiVersioning(
                options =>
                {
                    options.ReportApiVersions = true;

                    // Use this if you would like a new separate header to set the version in.
                    // Eg: Header 'api-version: 1.0'
                    //options.ApiVersionReader = new HeaderApiVersionReader("api-version");

                    // Use this if you would like to use the MediaType version header.
                    // Eg: Header 'Accept: application/json; v=1.0'
                    options.ApiVersionReader = new MediaTypeApiVersionReader();

                    // This is set to true so that we can set what version to select (default version)
                    // when no version has been selected.
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    // And this is where we set how to select the default version. 
                    options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
                });
        }
    }
}