﻿using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
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

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile picture)
        {
            //Eklenecek resim ismini random oluşturma. = Sol tarafda random isim + sağ tarafta uzantısı jpeg mi pbg mi falan
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);


            await _blobStorage.UploadAsync(picture.OpenReadStream(), newFileName, EContainerName.pictures);

            return RedirectToAction("Index");
        }
    }
}
