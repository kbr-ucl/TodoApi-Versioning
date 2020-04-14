using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace TodoApplication.Api.Extensions
{
    public static class ApiVersionSetupExtensionMethods
    {
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