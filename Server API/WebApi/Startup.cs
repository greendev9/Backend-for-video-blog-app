using AutoMapper;
using DAL;
using Data;
using Data.Providers;
using Domain;
using Domain.Interfaces;
using Domain.Models.Notifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Services.Customers;
using Services.Localization;
using Services.Messages;
using Services.Notifications;
using Services.Posts;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebApi.Extensions;
using WebApi.Models;
using WebApi.Models.Poll;
using WebApi.Utils;
using WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using Hangfire;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
                        => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            // 'Accept-Language' parameter in a Header
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // (Hangfire) Background Task to change story status from Pending to Approval after 5h. story was posted
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc(options =>
            {
                options.InputFormatters.Add(new XmlSerializerInputFormatter());
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            }).AddJsonOptions(options =>
            {

                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }); 
            }
            )
            .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("he"),
                    //new CultureInfo("ru")
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddDistributedMemoryCache();

            services.AddScoped(typeof(IStagingRepository<>), typeof(BlaBlogRepository<>));
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddCors();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<IPollStoryBuilder, PollStoryBuilder>();
            services.AddTransient<IMessageService, MessageService>(); 
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<ILangDetectorProvider, LanguageDetectorProvider>(x => new
                LanguageDetectorProvider(Configuration["ApiKeys:DetectLanguage"]));
            services.AddTransient<ILocalizationService, LocalizationService>();

            // use Custom Null Ignore resolver 
            services.AddAutoMapper(); 


            services.AddTransient<INotificationTransport, EmailTransport>(x =>
                                new EmailTransport(
                                    Configuration["Smtp:username"],
                                    Configuration["Smtp:password"],
                                    Configuration["Smtp:from"],
                                    Configuration["Smtp:fromName"],
                                    Configuration["Smtp:host"]
                                ));


            services.AddDbContextPool<DataContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.AddSwaggerDocumentation();


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    // Ensure the token hasn't expired:
                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidIssuer = Configuration["JwtIssuer"],
                    ValidAudience = Configuration["JwtIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                    ClockSkew = TimeSpan.Zero // remove delay of token when expire  
                };
            });

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
                hubOptions.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
            });
            services.AddScoped<ScheduledTask>();

            //services.AddHttpsRedirection(options =>
            //{
            //    options.RedirectStatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status307TemporaryRedirect;
            //    options.HttpsPort = 44369;
            //});

        }

        /// <summary>
        /// Configure 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            //app.UseHttpsRedirection();

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            // Make sure the CORS middleware is ahead of SignalR, reCuptcha etc.
            app.UseCors(builder =>
            {
                builder.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowCredentials();
            });

            // (Hangfire) Background Task to change story status from Pending to Approval after 5h. story was posted
            app.UseHangfireServer();
            app.UseHangfireDashboard();
            RecurringJob.AddOrUpdate<ScheduledTask>(x => x.Check(), "0/1 * * * *");

            app.UseStaticFiles();
            app.UseSwaggerDocumentation();

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });

            app.UseMvc(config =>
            {



            });
        }



    }
}
