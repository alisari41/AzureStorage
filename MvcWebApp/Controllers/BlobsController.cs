using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorageLibrary;
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
                Url = $"{blobUrl}/x"
            }).ToList();

            return View(); 
        }
    }
}
