using Default;
using System;
using System.Linq;

namespace ODataConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string serviceRoot = "https://localhost:44353/odata";

            Container container = new Container(new Uri(serviceRoot));

            var products = container.Products.ExecuteAsync().Result;

            products.ToList().ForEach(product =>
            {
                Console.WriteLine($"Product Name: {product.Name}");
            });

            var productsWithQuery = container.Products.AddQueryOption("$filter", "Id eq 2")
                                                      .AddQueryOption("$select", "Id,Name")
                                                      .ExecuteAsync().Result;

            var productsWithQuery2 = container.Products.Expand(p => p.Category).Execute();

            Console.ReadKey();
        }
    }
}
