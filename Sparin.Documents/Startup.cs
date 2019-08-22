using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Sparin.Documents.Configuration;
using Sparin.Documents.Services;
using Sparin.Documents.Services.Interfaces;
using Swashbuckle.AspNetCore.Swagger;

namespace Sparin.Documents
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string documentDbConnectionString = Configuration.GetConnectionString("DocumentDb");
            services.AddOptions();
            services.Configure<DocumentDbOptions>(Configuration.GetSection("DocumentDb"));

#if DEBUG
            services.AddCors();
#endif

            services.AddSingleton<MongoClient>(new MongoClient(documentDbConnectionString));
            services.AddSingleton<ICollectionService, CollectionService>();
            services.AddSingleton<IDocumentService, DocumentService>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v0.0.0", new Info { Title = "Documents API", Version = "v0.0.0" });
                c.DescribeAllEnumsAsStrings();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.CustomSchemaIds(x => x.FullName);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (env.IsDevelopment() || bool.Parse(Configuration["EnableSwagger"]))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v0.0.0/swagger.json", "Documents API");
                });
            }

#if DEBUG
            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:3000");
                //builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
            });
#endif

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
