using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorageLibrary;
using AzureStorageLibrary.Models;

namespace MvcWebApp.Controllers
{
    public class TableStoragesController : Controller
    {
        private INoSqlStorage<Product> _noSqlStorage;

        public TableStoragesController(INoSqlStorage<Product> noSqlStorage)
        {
            _noSqlStorage = noSqlStorage;
        }

        public IActionResult Index()
        {
            //Tablo üzerindeki tüm satırları çekmek
            ViewBag.products = _noSqlStorage.All().ToList();


            return View();
        }

        

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.RowKey = Guid.NewGuid().ToString();
            product.PartitionKey = "Kalemler";

            await _noSqlStorage.Add(product);

            return RedirectToAction("Index");
        }

    }
}
