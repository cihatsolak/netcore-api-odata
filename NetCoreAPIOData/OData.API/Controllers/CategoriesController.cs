﻿using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using OData.API.Models;

namespace OData.API.Controllers
{
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
    }
}
