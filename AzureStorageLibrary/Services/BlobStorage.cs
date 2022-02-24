using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

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

        public async Task SetLogAsync(string text, string fileName)
        {//Yazma operasyonu
            //Hangi  Container olduğunu biliyoruz o yüzden sadece logu alsak yeter
            var containerClient = _blobServiceClient.GetBlobContainerClient(EContainerName.Logs.ToString());

            var appendBlobClient = containerClient.GetAppendBlobClient(fileName);

            //append blob'ın olup olmadığını tespt ekmek gerekir. Eğer yoksa oluşsun
            await appendBlobClient.CreateIfNotExistsAsync();

            using (MemoryStream ms = new MemoryStream())
            {
                //parametreden gelen text 'i Stream'e çevirecek bir StreamWriter oluşturalım
                using (StreamWriter sw = new StreamWriter(ms))
                {//ms (MemoryStream) yi yazıcam 
                    sw.Write($"{DateTime.Now}:{text}/n");

                    //yazma işleminden sonra temizleniyor.
                    sw.Flush();
                    ms.Position = 0; // Yazma işlemine başlarken sıfırdan başlasın. Diyelimki text=Ali SArı ise A'dan başlasın

                    await appendBlobClient.AppendBlockAsync(ms);
                }
            }
        }

        public async Task<List<string>> GetLogAsync(string fileName)
        {
            //satır satır okumak için öncelikle list<string> oluşturuldu
            List<string> logs = new List<string>();
            //Hangi  Container olduğunu biliyoruz o yüzden sadece logu alsak yeter
            var containerClient = _blobServiceClient.GetBlobContainerClient(EContainerName.Logs.ToString());

            //Loglama işleminde AppendBlob işlemi kullanıyoruz
            var appendContainerClient = containerClient.GetAppendBlobClient(fileName);

            // Yoksa oluşsun boş dahi olsa oluşsun ki okuma işlemini gerçekleştirelim
            await appendContainerClient.CreateIfNotExistsAsync();

            var info = await appendContainerClient.DownloadAsync();
            //satır satır okuma işlemi yapılması gerekiyor.
            using (StreamReader sr = new StreamReader(info.Value.Content))
            {
                string line = string.Empty;

                while ((line = sr.ReadLine()) != null)//SATIR OKU null olmayana kadar oku
                {
                    logs.Add(line);
                }
            }

            return logs;
        }

        public List<string> GetNames(EContainerName eContainerName)
        {//Blobları alacağımız metod
            List<string> blobNames = new List<string>();
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());

            var blobs = containerClient.GetBlobs();//Tüm bloblar geldi
            blobs.ToList().ForEach(x =>
            {
                blobNames.Add(x.Name);
            });

            return blobNames;
        }
    }
}
