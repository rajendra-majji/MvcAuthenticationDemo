using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcTwitterAuthentication.Data;

namespace MvcTwitterAuthentication
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddRazorPages()
                .AddRazorPagesOptions(config => {
                    config.Conventions.AuthorizePage("/Razor/secured");
                });

            // Failing with an error:
            // https://localhost:44312/Identity/Account/ExternalLogin?returnUrl=%2F
            // HttpRequestException: Response status code does not indicate success: 401 (Authorization Required).
            services.AddAuthentication()
                .AddTwitter(twitterOptions =>
                {
                    twitterOptions.ConsumerKey = "9pEl0DC5sez0XR3Xr48AunW0d"; // Configuration["Authentication:Twitter:ConsumerAPIKey"];
                    twitterOptions.ConsumerSecret = "KnGESC2JmiWxmnYGMAxE9kKISDuD53Fiiqqoku2uzOBn6tlFcQ"; // Configuration["Authentication:Twitter:ConsumerSecret"];
                    twitterOptions.RetrieveUserDetails = true;
                });


            //services.AddAuthentication().AddTwitter(twitterOptions =>
            //    {
            //        twitterOptions.ConsumerKey = "9pEl0DC5sez0XR3Xr48AunW0d"; // Configuration["Authentication:Twitter:ConsumerAPIKey"];
            //        twitterOptions.ConsumerSecret = "KnGESC2JmiWxmnYGMAxE9kKISDuD53Fiiqqoku2uzOBn6tlFcQ"; // Configuration["Authentication:Twitter:ConsumerSecret"];
            //        twitterOptions.RetrieveUserDetails = true;
            //        twitterOptions.BackchannelCertificateValidator
            //        new CertificateSubjectKeyIdentifierValidator(new[]
            //            {
            //                "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
            //                "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
            //                "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
            //                "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
            //                "5168FF90AF0207753CCCD9656462A212B859723B", //DigiCert SHA2 High Assurance Server C‎A 
            //                "B13EC36903F8BF4701D498261A0802EF63642BC3" //DigiCert High Assurance EV Root CA
            //            });
            //    });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
