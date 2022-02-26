using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            ConnectionStrings.AzureStorageConnectionString = "***REMOVED***";

            IBlobStorage blobStorage = new BlobStorage();
            // Veritabanýnda güncelleme iþlemleri
            INoSqlStorage<UserPicture> noSqlStorage = new TableStorage<UserPicture>();


            foreach (var item in myQueueItem.WatermarkPictures)
            {
                // Download iþlemi yapýlýyor
                using var stream = await blobStorage.DownloadAsync(item, EContainerName.pictures);

                using var memortStream = AddWaterMark(myQueueItem.WatermarkText, stream);

                await blobStorage.UploadAsync(memortStream, item, EContainerName.watermarkpictures);

                var logger = context.GetLogger("Function1");
                logger.LogInformation($"{item} resmine watermark eklenmiþtir.");

            }

            var userPicture = await noSqlStorage.Get(myQueueItem.UserId, myQueueItem.City);

            if (userPicture.WatermarkRawPaths != null)
            {
                myQueueItem.WatermarkPictures.AddRange(userPicture.WatermarkPaths);
            }

            userPicture.WatermarkPaths = myQueueItem.WatermarkPictures;//Yazýsý eklenmiþ olan resimler

            await noSqlStorage.Add(userPicture);//Dinamik olarak tabloya yeni kolon ekliyorum



        }


        public static MemoryStream AddWaterMark(string watermarkText, Stream pictureStream)
        {
            MemoryStream ms = new MemoryStream();

            using (Image image = Bitmap.FromStream(pictureStream))
            {
                using (Bitmap tempBitmap = new Bitmap(image.Width, image.Height))//Bitmap þuan boþ bir resim gibi düþünebiliriz
                {//resim boyutlandýrmasý ypaýldý
                    using (Graphics graphics = Graphics.FromImage(tempBitmap))//Grafik nesnesi oluþturuluyor. Bu nesne üzerinden resmin üzerine yazý yazýlacak
                    {

                        graphics.DrawImage(image, 0, 0);//Resmi çizmek için baþlangýç verildi

                        var font = new Font(FontFamily.GenericSansSerif, 22, FontStyle.Bold);//Yazý tipi belirlendi

                        var color = Color.FromArgb(255, 0, 0);//Kýrmýzý

                        var brush = new SolidBrush(color);

                        var point = new Point(20, y: (int)image.Height - 50);//Resim yazýsýnýn konumu

                        graphics.DrawString(watermarkText, font, brush, point);

                        tempBitmap.Save(ms, ImageFormat.Png);
                    }
                }
            }

            ms.Position = 0;//Stram sýfýrdan baþlasýn resim yazma iþlemi

            return ms;
        }
    }
}
