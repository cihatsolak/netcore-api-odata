﻿using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using OData.API.Models;
using System.Linq;

namespace OData.API.Controllers
{
    [ODataRoutePrefix("Categories")]
    public class CategoriesController : ODataController
    {
        private readonly AppDbContext _dbContext;
        public CategoriesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_dbContext.Categories);
        }

        //[ODataRoute("Categories({id})")]
        [ODataRoute("({id})")]
        [EnableQuery]
        [HttpGet]
        public IActionResult GetCategoryById([FromODataUri] int id)
        {
            //Url -> [FromODataUri]
            //Where -> IQueryable

            return Ok(_dbContext.Categories.Where(p => p.Id == id));
        }

        //[EnableQuery]
        //[HttpGet]
        //public IActionResult Get([FromODataUri] int key) //Key zorunlu
        //{
        //    //Url -> [FromODataUri]
        //    //Where -> IQueryable

        //    return Ok(_dbContext.Categories.Where(p => p.Id == key));
        //}

        [ODataRoute("({categoryId})/Products({productId})")]
        [EnableQuery]
        [HttpGet]
        public IActionResult ProductById([FromODataUri] int categoryId, [FromODataUri] int productId)
        {
            return Ok(_dbContext.Products.Where(p => p.CategoryId == categoryId && p.Id == productId));
        }

        /// <summary>
        /// Custom Path (Parametresiz Action-> Body)
        /// </summary>
        /// <param name="key">Category Id</param>
        /// <returns>Toplam Ürün Fiyatı</returns>
        [HttpPost]
        public IActionResult TotalProductPrice([FromODataUri] int key) //key: odata
        {
            var total = _dbContext.Products.Where(p => p.CategoryId == key).Sum(p => p.Price);

            return Ok($"Total: {total}");
        }

        /// <summary>
        /// Custom Path (Parametreli Action-> Body)
        /// </summary>
        /// <param name="parameters">parameters</param>
        /// <returns>Toplam Ürün Fiyatı</returns>
        [HttpPost]
        public IActionResult TotalProductPriceWithParameter(ODataActionParameters parameters)
        {
            int categoryId = (int)parameters["categoryId"];
            int minimumStock = (int)parameters["minimumStock"];

            var total = _dbContext.Products.Where(p => p.CategoryId == categoryId && p.Stock >= minimumStock).Sum(p => p.Price);

            return Ok($"Total: {total}");
        }

        /// <summary>
        /// Custom Path (Uri parametresiz)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CategoryCount()
        {
            int count = _dbContext.Categories.Count();

            return Ok($"Category Count: {count}");
        }
    }
}
