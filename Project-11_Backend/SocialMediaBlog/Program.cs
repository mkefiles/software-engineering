using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SocialMediaBlog.Account;
using SocialMediaBlog.CommonInterfaces;
using Microsoft.Extensions.Configuration;
using SocialMediaBlog.DatabaseContext;
using SocialMediaBlog.Messages;

namespace SocialMediaBlog;

public class Program {

    public static void Main(string[] args) {
        // DESC: Initialize the Web App. Builder
        // NOTE: This is used to configure the Web App. prior to
        // ... initializing it

        WebApplicationBuilder appConfigurationBuilder = WebApplication.CreateBuilder(args);
        
        // DESC: Add Connection-String for Database
        // NOTE: The Connection String can be defined in the `appsettings.json` file
        var connectionString = appConfigurationBuilder
            .Configuration.GetConnectionString("SocialMediaBlog") ?? "Data Source=SocialMediaBlog.db";

        appConfigurationBuilder.Services.AddSqlite<SocialMediaBlogDbCtx>(connectionString);
        // DESC: Enable the use of Traditional Controllers
        // NOTE: This is necessary if not using Minimal API
        appConfigurationBuilder.Services.AddControllers();
        
        // DESC: Register Services for Dependency Injection
        // NOTE: PT02 - D.I. - Register Services with D.I. Container
        // NOTE: Including the Interface is NOT required, however it helps when
        // ... there are multiple impl.'s of it, when it comes to unit-testing and
        // ... if you want to promote loose coupling and maintainability
        
        /*
         * CALL OUT: It is problematic to use the following syntax when ONE Interface is
         * ... implemented by MULTIPLE services as the last registration overwrites the
         * ... previous registration(s):
         * `appConfigurationBuilder.Services.AddScoped<ICommonContract, AccountService>();`
         * https://touseefkhan4pk.medium.com/registering-multiple-services-with-a-single-interface-in-net-core-e6e4a1a0ec04
         */
        
        appConfigurationBuilder.Services.AddKeyedScoped<ICommonContract, AccountService>("Acct. DI w/ Interface");
        appConfigurationBuilder.Services.AddScoped<IAccountService, AccountService>();
        
        appConfigurationBuilder.Services.AddKeyedScoped<ICommonContract, MessagesService>("Msgs. DI w/ Interface");
        appConfigurationBuilder.Services.AddScoped<IMessagesService, MessagesService>();

        // DESC: Enable the use of OpenAPI (for SwaggerUI)
        appConfigurationBuilder.Services.AddOpenApi();

        // DESC: Enable Logging for LINQ errors
        // NOTE: This is for Development ONLY
        appConfigurationBuilder.Services.AddDbContext<SocialMediaBlogDbCtx>((options) => {
                options.EnableSensitiveDataLogging();
            }
        );
        
        // DESC: Initialize the actual App (a.k.a., HTTP Pipeline)
        // NOTE: It is referred to as a "Pipeline" because it
        // ... represents a series of components that an HTTP request
        // ... passes through up to and including the actual response
        WebApplication app = appConfigurationBuilder.Build();

        // DESC: Map the OpenAPI end-points IF in Development
        if (app.Environment.IsDevelopment()) {
            app.MapOpenApi();
        }
        
        // DESC: Establish necessary middle-wares
        app.UseHttpsRedirection();
        
        // NOTE: This tells ASP.NET Core to discover and register
        // ... end-points defined by the `[Route]` attribute
        // NOTE: Routing is auto-configured, by WebApp.Bulder, in
        // ... that it wraps the middle-ware added here with
        // ... `UseRouting` and `UseEndpoints` middle-ware 
        app.MapControllers();
        
        // DESC: Run the App
        app.Run();
    }
}