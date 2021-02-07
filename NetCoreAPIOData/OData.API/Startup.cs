using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OData.API.Models;
using OData.API.Models.DataTransferObjects;
using System.Linq;

namespace OData.API
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
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Default"));
            });

            services.AddOData(); //OData Eklenmesi
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var builder = new ODataConventionModelBuilder();

            //builder.EntitySet<EntityName>("ControllerName");
            //[entity set name]Controller
            builder.EntitySet<Category>("Categories");
            builder.EntitySet<Product>("Products");

            #region Actions
            //Custom (Body Parametresiz)
            //...../odata/categories(1)/totalproductprice (Body Parametresiz acton)
            builder.EntityType<Category>().Action("totalproductprice").Returns<int>();

            //Custom Path (Body Parametreli)
            //odata/category/totalproductprice (Body Parametreli acton)
            var totalProduct = builder.EntityType<Category>().Collection.Action("TotalProductPriceWithParameter").Returns<int>();
            totalProduct.Parameter<int>("categoryId");
            totalProduct.Parameter<int>("minimumStock");

            //Custom Path (Complext Type)
            builder.EntityType<Product>().Collection.Action("Login").Returns<string>().Parameter<LoginDto>("LoginModel");
            #endregion

            #region Functions
            //Parametresiz Function
            builder.EntityType<Category>().Collection.Function("CategoryCount").Returns<int>();

            //Parametreli Function
            var multipleFunction = builder.EntityType<Product>().Collection.Function("Multiple").Returns<int>();
            multipleFunction.Parameter<int>("parameter1");
            multipleFunction.Parameter<int>("parameter2");
            multipleFunction.Parameter<int>("parameter3");
            #endregion

            #region UnBound
            builder.Function("GetKdv").Returns<int>();
            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Select()
                         .Expand()
                         .OrderBy()
                         .MaxTop(null)
                         .Count()
                         .Filter(); //OData'ya select, expand, orderby kullanýlacaðýný bildiriyoruz.

                endpoints.MapODataRoute("odata", "odata", builder.GetEdmModel());
                endpoints.MapControllers();
            });
        }
    }
}
