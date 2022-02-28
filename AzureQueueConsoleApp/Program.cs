using System;
using System.Text;
using System.Threading.Tasks;
using AzureStorageLibrary.Services;

namespace AzureQueueConsoleApp
{
    class Program
    {
       async static Task Main(string[] args)
        {
            AzureStorageLibrary.ConnectionStrings.AzureStorageConnectionString =
                "DefaultEndpointsProtocol=https;AccountName=realstorageaccount41;AccountKey=Z1g8K87KaicSESnwlbjfinZicQ09CtyKLjWnzVs/r6IF5kD7z1c9lFonrLmc/ydyJv6rk+63Krm1r0pCGhq3+w==;EndpointSuffix=core.windows.net";

            AzureQueue queue = new AzureQueue("ornekkuyruk");

            //Mesaj gönderelim Base64 işlemi yapmak gerekir türkçe karakterler veya service için anlamlı olabilecek karakter olabilir.Encoding ettikten sonra Decodding işlemi ypaılacak
            string base64Message = Convert.ToBase64String(Encoding.UTF8.GetBytes("ali sarı"));//"ali sarı" mesajı yazıyorum


            //queue.SendMessageAsync(base64Message).Wait(); // Wait() metodu ile senkrona çevrildi 

            // Kaydedilen mesajı okuyoruz
            var queueMessage = queue.RetrieveNextMessageAsync().Result;//Geriye dönen message'ı result üzerinden alabiliriz

            // eğer bu işlemi yapmazsak gelen Mesaj: YWxpIHNhcsSx olur 
            //string text = Encoding.UTF8.GetString(Convert.FromBase64String(queueMessage.MessageText));

            //Console.WriteLine("Mesaj: " + text);

            //Silme işlemleri

            await queue.DeleteMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt);
            Console.WriteLine("Yapmak istediğimiz işleri yaptık farz ediyoruz... Mesaj Silindi.");



        }
    }
}
