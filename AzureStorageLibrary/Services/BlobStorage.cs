using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace AzureStorageLibrary.Services
{
    public class BlobStorage : IBlobStorage
    {
        private readonly BlobServiceClient _blobServiceClient;//Bu client üzerinden haberleşme yapıcam

        public BlobStorage()
        {
            _blobServiceClient = new BlobServiceClient(ConnectionStrings.AzureStorageConnectionString);
        }

        public string BlobUrl { get; set; }
        public async Task UploadAsync(Stream fileStream, string fileName, EContainerName eContainerName)
        {
            //Bir blob kaydedebilmek için önce bir ServiceContainer oluşturmak gerekir.
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());


            //Container elde ettikten sonra Container var mı yok mu bakılıyor.
            await containerClient.CreateIfNotExistsAsync();//Bu container yoksa oluşsun
            //Erişim izni vermem gerekiyor. Hangi seviyede dış dünyaya açılsın Container seviyesinde dış dünyaya açılmasını istiyorum
            //Dış dünyaya açtım artık Url üzerinden erişebilicem. Yani web sitemde kolayca gösterebilcem
            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);


            //Önce bir serviceClient sonra containerClient sonrasında da BlobClinet
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream);
        }

        public async Task<Stream> DownloadAsync(string fileName, EContainerName eContainerName)
        {//Azure Storage'da herhangi bir blob'ı  indirme oprasyonu
            //Bir blob kaydedebilmek için önce bir ServiceContainer oluşturmak gerekir.
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());

            var blobClient = containerClient.GetBlobClient(fileName);

            var info = await blobClient.DownloadAsync();
            return info.Value.Content;
        }

        public async Task DeleteAsync(string fileName, EContainerName eContainerName)
        {//Azure Container'ı silme
            //Bir blob kaydedebilmek için önce bir ServiceContainer oluşturmak gerekir.
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());

            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteAsync();
        }

        public Task SetLogAsync(string text, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetLogAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public List<string> GetNames(EContainerName eContainerName)
        {
            throw new NotImplementedException();
        }
    }
}
