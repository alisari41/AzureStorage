using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureStorageLibrary;
using Microsoft.AspNetCore.Http;
using MvcWebApp.Models;

namespace MvcWebApp.Controllers
{
    public class BlobsController : Controller
    {
        private readonly IBlobStorage _blobStorage;//Bağımlılığımız geçtik

        public BlobsController(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public async Task<IActionResult> Index()
        {

            var names = _blobStorage.GetNames(EContainerName.pictures);//İçersindeki isimleri alınıyor.

            //{URL yazılacak}/{Container eklenecek}
            string blobUrl = $"{_blobStorage.BlobUrl}/{EContainerName.pictures.ToString()}";

            ViewBag.blobs = names.Select(x => new FileBlob
            {
                Name = x,
                //{blobUrl} /{x=Blob'ın ismi "png mi jpeg mi neyse o "}
                Url = $"{blobUrl}/{x}"
            }).ToList();

            ViewBag.logs = await _blobStorage.GetLogAsync("controller.txt");

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile picture)
        {
            //Hangi dosyaya ne yazsın
            await _blobStorage.SetLogAsync("Upload metot'una giriş yapıldı.", "controller.txt");


            //Eklenecek resim ismini random oluşturma. = Sol tarafda random isim + sağ tarafta uzantısı jpeg mi pbg mi falan
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);
            await _blobStorage.UploadAsync(picture.OpenReadStream(), newFileName, EContainerName.pictures);


            await _blobStorage.SetLogAsync("Upload metot'undan çıkış yapıldı.", "controller.txt");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            var stream = await _blobStorage.DownloadAsync(fileName, EContainerName.pictures);

            return File(stream, "application/octet-stream", fileName);//tipi ni "octet-stream" vermemim nedeni tipini bilmediğimden 
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string fileName)
        {
            await _blobStorage.DeleteAsync(fileName, EContainerName.pictures);
            return RedirectToAction("Index");
        }
    }
}
