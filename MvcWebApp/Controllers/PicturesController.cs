using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using AzureStorageLibrary.Services;
using Microsoft.AspNetCore.Http;
using MvcWebApp.Models;
using Newtonsoft.Json;

namespace MvcWebApp.Controllers
{
    public class PicturesController : Controller
    {
        //Table storage a datayı kaydedebilmek için üye varmış gibi tutucam
        public string UserId { get; set; } = "12345";//RowKey
        public string City { get; set; } = "Kocaeli";//PartitionKey


        private readonly INoSqlStorage<UserPicture> _noSqlStorage;
        private readonly IBlobStorage _blobStorage;

        public PicturesController(INoSqlStorage<UserPicture> noSqlStorage, IBlobStorage blobStorage)
        {
            _noSqlStorage = noSqlStorage;
            _blobStorage = blobStorage;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UserID = UserId;
            ViewBag.City = City;



            // Daha önce bir resim var mı varsa listeye ekle
            List<FileBlob> fileBlobs = new List<FileBlob>();

            // Storage'dan arama yapma işlemi
            var user = await _noSqlStorage.Get(UserId, City);


            if (user != null)
            {//veri tabanında user varsa

                user.Paths.ForEach(x =>
                {
                    fileBlobs.Add(new FileBlob { Name = x, Url = $"{_blobStorage.BlobUrl}/{EContainerName.pictures}/{x}" });
                });


            }

            ViewBag.fileBlobs = fileBlobs;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IEnumerable<IFormFile> pictures)//Birden fazla resim alabilicem
        {

            List<string> pictureList = new List<string>();//Eklenen resmin ismini ve uzantısını tutuyorum

            foreach (var item in pictures)
            {
                //rasgele dosya ismi. Sol taraf rasgele isim sağ taraf dosya uzantısı
                var newImageName = $"{Guid.NewGuid()}{Path.GetExtension(item.FileName)}";

                //resim kaydetme işlemi
                await _blobStorage.UploadAsync(item.OpenReadStream(), newImageName, EContainerName.pictures);

                pictureList.Add(newImageName);
            }

            //Table Storage'a ekleme işlemi önce kullanıcıya ait satır var mı onu bulmam lazım
            var isUser = await _noSqlStorage.Get(UserId, City);

            if (isUser != null)
            {
                pictureList.AddRange(isUser.Paths);//Böyle bir kullanıcı varsa eski resimleri de ekliyorum
                isUser.Paths = pictureList;// bunu burayada eklemem lazım tabloda user'a ait veri varsa üstüne eklme yaptığında yenileride göstersin
            }
            else
            {//yoksa sıfırdan oluşturdum var zaten üstüne ekledim eskiler artı yeniler şekilde
                isUser = new UserPicture();

                isUser.RowKey = UserId;
                isUser.PartitionKey = City;
                isUser.Paths = pictureList;//içersinde kakak.jpeg kakak2.jpeg
            }

            await _noSqlStorage.Add(isUser);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddWatermark(PictureWatermarkQueue pictureWatermarkQueue)
        {
            
            var jsonString = JsonConvert.SerializeObject(pictureWatermarkQueue);
            string jsonStringBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

            AzureQueue azureQueue = new AzureQueue("watermarkqueue");
            await azureQueue.SendMessageAsync(jsonStringBase64);

            return Ok();
        }
    }
}
