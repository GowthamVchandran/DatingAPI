﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingAPI.Controllers.Helper;
using DatingAPI.Data;
using DatingAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace DatingAPI
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
            services
          .AddDbContext<DataContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
             services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
             .AddJsonOptions(opt => {opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;});
            services.AddCors();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddAutoMapper(typeof(DatingRepository));

            

            services.AddScoped<IAuthRepository,AuthReposity>();
            services.AddScoped<IDatingRepository,DatingRepository>();
            

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(x=>{
                  x.TokenValidationParameters = new TokenValidationParameters(){
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.
                      GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                      ValidateIssuer =false,
                      ValidateAudience = false
                  };
              });

              services.AddScoped<logUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
               // app.UseDeveloperExceptionPage();
            }
            else
            {
               app.UseExceptionHandler(builder => {

                   builder.Run( async context => 
                   {
                       context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                       var err =context.Features.Get<IExceptionHandlerFeature>();

                       if(err!=null)
                       {
                           context.Response.ApplicationError(err.Error.Message);
                           await context.Response.WriteAsync(err.Error.Message);
                       }
                   });
               });

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
               // app.UseHsts();
            }
            app.UseCors(s => s.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());           
            app.UseAuthentication();
             app.UseMvc();
            
        }
    }
}
