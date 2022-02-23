using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using Microsoft.Azure.Cosmos.Table;

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

            ViewBag.isUpdate = false;

            return View();
        }

        //public IActionResult Create()
        //{
        //      Bu metodu kullanarak yeni sayfa içersinde oluşturmak istemiyorum Index sayfamda ekleme işlemi yapacağım için bu metoda gerek yok
        //
        //      return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.RowKey = Guid.NewGuid().ToString();
            product.PartitionKey = "Kalemler";

            await _noSqlStorage.Add(product);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(string rowKey, string partitionKey)
        {//index sayfası içersinde update metodu çalıştırmak için 
            //İndex sayfasında görüntüleme yapıcaz
            var product = await _noSqlStorage.Get(rowKey, partitionKey);


            //İndex sayfasına yönlendirme yapılıyorsa index metodundaki deperleri tekrar vermek gerekir
            ViewBag.products = _noSqlStorage.All().ToList();
            ViewBag.isUpdate = true;
            return View("Index", product);

        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            //ETag eşzamanlılık problemi
            //product.ETag = "*";//Diğer clinetlar güncelleme yaparken güncelleme yapılmış fakat işlenmemiş nesneyi ezebilir.
            ViewBag.isUpdate = true;
            await _noSqlStorage.Update(product);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string rowKey, string partitionKey)
        {
            await _noSqlStorage.Delete(rowKey, partitionKey);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Query(int price)
        {//Fiyat üzerinden sorgulama
            //İndex sayfasına yönlendirme yapılıyorsa index metodundaki deperleri tekrar vermek gerekir
            ViewBag.isUpdate = false;
            ViewBag.products = _noSqlStorage.Query(x => x.Price > price).ToList();

            return View("Index");

        }
    }
}
