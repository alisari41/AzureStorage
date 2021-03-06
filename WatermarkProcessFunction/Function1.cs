using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using AzureStorageLibrary.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace WatermarkProcessFunction
{
    public static class Function1
    {
        [Function("Function1")]
        public async static Task Run([QueueTrigger("watermarkqueue")] PictureWatermarkQueue myQueueItem, FunctionContext context)
        {

            ConnectionStrings.AzureStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=realstorageaccount41;AccountKey=Z1g8K87KaicSESnwlbjfinZicQ09CtyKLjWnzVs/r6IF5kD7z1c9lFonrLmc/ydyJv6rk+63Krm1r0pCGhq3+w==;EndpointSuffix=core.windows.net";

            IBlobStorage blobStorage = new BlobStorage();
            // Veritaban?nda g?ncelleme i?lemleri
            INoSqlStorage<UserPicture> noSqlStorage = new TableStorage<UserPicture>();


            foreach (var item in myQueueItem.WatermarkPictures)
            {
                // Download i?lemi yap?l?yor
                using var stream = await blobStorage.DownloadAsync(item, EContainerName.pictures);

                using var memortStream = AddWaterMark(myQueueItem.WatermarkText, stream);

                await blobStorage.UploadAsync(memortStream, item, EContainerName.watermarkpictures);


                var logger = context.GetLogger("Function1");
                logger.LogInformation($"{item} resmine watermark eklenmi?tir.");

            }

            var userPicture = await noSqlStorage.Get(myQueueItem.UserId, myQueueItem.City);

            if (userPicture.WatermarkRawPaths != null)
            {
                myQueueItem.WatermarkPictures.AddRange(userPicture.WatermarkPaths);
            }

            userPicture.WatermarkPaths = myQueueItem.WatermarkPictures;//Yaz?s? eklenmi? olan resimler

            await noSqlStorage.Add(userPicture);//Dinamik olarak tabloya yeni kolon ekliyorum

            //Haber verme i?lemi SignalR 
            var endPoint = "https://192.168.0.1/service";
            var httpHandler = new HttpClientHandler();

            httpHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            {
                return true;
            };
            var httpClient = new HttpClient(httpHandler);
            //await ekleyince kuyru?a mesaj dahi gitmiyor
            var response = await httpClient.GetAsync("https://localhost:44389/api/Notifications/CompleteWatermarkProcess/" + myQueueItem.ConnectionId);

            var logger2 = context.GetLogger("Function1");
            logger2.LogInformation($"Client ({myQueueItem.ConnectionId}) bilgilendirilmi?tir.");





        }


        public static MemoryStream AddWaterMark(string watermarkText, Stream pictureStream)
        {
            MemoryStream ms = new MemoryStream();

            using (Image image = Bitmap.FromStream(pictureStream))
            {
                using (Bitmap tempBitmap = new Bitmap(image.Width, image.Height))//Bitmap ?uan bo? bir resim gibi d???nebiliriz
                {//resim boyutland?rmas? ypa?ld?
                    using (Graphics graphics = Graphics.FromImage(tempBitmap))//Grafik nesnesi olu?turuluyor. Bu nesne ?zerinden resmin ?zerine yaz? yaz?lacak
                    {

                        graphics.DrawImage(image, 0, 0);//Resmi ?izmek i?in ba?lang?? verildi

                        var font = new Font(FontFamily.GenericSansSerif, 22, FontStyle.Bold);//Yaz? tipi belirlendi

                        var color = Color.FromArgb(255, 0, 0);//K?rm?z?

                        var brush = new SolidBrush(color);

                        var point = new Point(20, y: (int)image.Height - 50);//Resim yaz?s?n?n konumu

                        graphics.DrawString(watermarkText, font, brush, point);

                        tempBitmap.Save(ms, ImageFormat.Png);
                    }
                }
            }

            ms.Position = 0;//Stram s?f?rdan ba?las?n resim yazma i?lemi

            return ms;
        }
    }
}
