using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OData.API.Models;
using OData.API.Models.DataTransferObjects;
using System.Linq;

namespace OData.API.Controllers
{
    [ODataRoutePrefix("Products")]
    public class ProductsController : ODataController
    {
        private readonly AppDbContext _dbContext;
        public ProductsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //[EnableQuery]
        [EnableQuery(PageSize = 2, AllowedLogicalOperators = AllowedLogicalOperators.And)]
        [HttpGet]
        public IActionResult Get()
        {
            /*
             * DbSet : IQueryable<>
             * ToList(): IEnumarable<>
             */
            return Ok(_dbContext.Products);
        }

        //[ODataRoute("Products({id})")]
        [ODataRoute("({id})")]
        [EnableQuery]
        [HttpGet]
        public IActionResult GetProductById([FromODataUri] int id)
        {
            //Url -> [FromODataUri]
            //Where -> IQueryable

            return Ok(_dbContext.Products.Where(p => p.Id == id));
        }

        //[EnableQuery]
        //[HttpGet]
        //public IActionResult Get([FromODataUri] int key) //Key zorunlu
        //{
        //    //Url -> [FromODataUri]
        //    //Where -> IQueryable

        //    return Ok(_dbContext.Products.Where(p => p.Id == key));
        //}

        [ODataRoute("")]
        [HttpPost]
        public IActionResult Post([FromBody] Product product) //isimlendirme önemli. (product)
        {
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            return Created<Product>(product);  // <!-- Best Practices -->
        }

        [HttpPut]
        public IActionResult Put([FromODataUri] int key, [FromBody] Product product) //isimlendirme önemli. (key),(product)
        {
            product.Id = key;

            _dbContext.Entry(product).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return NoContent(); // <!-- Best Practices -->
        }

        [ODataRoute("({id})")]
        [HttpDelete]
        public IActionResult Delete([FromODataUri] int id)
        {
            var products = _dbContext.Products.Find(id);
            if (products == null)
                return NotFound("No products associated with key found");

            _dbContext.Remove(products);
            _dbContext.SaveChanges();

            return NoContent(); // <!-- Best Practices -->
        }

        [HttpPost]
        public IActionResult Login(ODataActionParameters parameters)
        {
            LoginDto login = parameters["LoginModel"] as LoginDto;
            if (login == null)
                return NotFound();

            return Ok($"Email: {login.Email} - Password: {login.Password}");
        }

        /// <summary>
        /// Parametreleri function
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="parameter3"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Multiple([FromODataUri]int parameter1, [FromODataUri] int parameter2, [FromODataUri] int parameter3)
        {
            int result = parameter1 * parameter2 * parameter3;

            return Ok($"Multiple result: {result}");
        }
    }
}
