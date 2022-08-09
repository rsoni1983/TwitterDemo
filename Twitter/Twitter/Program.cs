


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Twitter.Models;
using Twitter.Services;

namespace Training09
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            var configuration = builder.Configuration;
            var connectionString = configuration.GetConnectionString("appConnectionString");

            builder.Services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITweetService, TweetService>();
            builder.Services.AddScoped<ITweetReactionService, TweetReactionService>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                           .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
                       ValidateIssuer = false,
                       ValidateAudience = false
                   };
               });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddApiVersioning(options =>
            {
                // ReportApiVersions will return the "api-supported-versions" and "api-deprecated-versions" headers.
                options.ReportApiVersions = true;

                // Set a default version when it's not provided,
                // e.g., for backward compatibility when applying versioning on existing APIs
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // Combine (or not) API Versioning Mechanisms:
                options.ApiVersionReader = ApiVersionReader.Combine(
                    // Read from url segment
                    new UrlSegmentApiVersionReader(),
                    // The versioning mechanism which reads the API version from the "api-version" Query String paramater.
                    new QueryStringApiVersionReader("api-version"),
                    // Use the following, if you would like to specify the version as a custom HTTP Header.
                    new HeaderApiVersionReader("Accept-Version"),
                    // Use the following, if you would like to specify the version as a Media Type Header.
                    new MediaTypeApiVersionReader("api-version")
                );
            });

            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard authorization heading required using bearer scheme",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        } 
    } 
}