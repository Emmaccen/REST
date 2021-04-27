using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CourseLibrary.API
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
            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
            }).AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                // create a problem details object
                var detailsFactory = context.HttpContext.RequestServices
                .GetRequiredService<ProblemDetailsFactory>();
                    var problemDetails = detailsFactory.CreateValidationProblemDetails(
                        context.HttpContext, context.ModelState);

                // add additional info not added by default
                problemDetails.Detail = "See errors for additional details";
                    problemDetails.Instance = context.HttpContext.Request.Path;

                //find out which status code to use
                var actionExecutingContext = context as ActionExecutingContext;

                // if there are model state errors and all argument are correctly parsed/found
                // that means we're dealing with a vidation error so...
                if ((context.ModelState.ErrorCount > 0) &&
            (actionExecutingContext?.ActionArguments.Count ==
            context.ActionDescriptor.Parameters.Count))
                    {
                        problemDetails.Title = "One or more validation errors occured.";
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        problemDetails.Type = "https://courseLibrary.com/modelValidation";

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };

                /*if one of the arguments couldn't be found or cannot be parsed that means
                    * we're dealing with null/unparseable inputs/values
                    */
                    problemDetails.Title = "One or more validation errors occured";
                    problemDetails.Status = StatusCodes.Status400BadRequest;

                    return new UnprocessableEntityObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });

            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected error happened. Please do try again later :)");
                    });
                });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
