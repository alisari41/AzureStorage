using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace AzureStorageLibrary.Services
{
    public class AzureQueue
    {
        private readonly QueueClient _queueClient;

        public AzureQueue(string queueName)
        {
            _queueClient = new QueueClient(ConnectionStrings.AzureStorageConnectionString, queueName);

            //Kuyruk mevcut değilse oluşsun
            _queueClient.CreateIfNotExists();
        }

        public async Task SendMessageAsync(string message)//Async metot olduğu için isimlendirirken sonuna Async yazılır.
        {
            // visibilityTimeout : Kuyrukta ne kadar görünmez olduğunu belirtmek için.Mesela kuyruktan mesaj aldım bunu 30 saniyede işleyebiliyorsam kuyrukta görünmez olası lazımki başka clientlar bu mesajı almasın. Default olarak 30 saniyedir.
            // timeToLive : Yaşam süresi default olarak 7 gündür. 
            await _queueClient.SendMessageAsync(message, default, TimeSpan.FromDays(8));
        }


        //Mesaj alma işlemi
        public async Task<QueueMessage> RetrieveNextMessageAsync()
        {
            // Kuyrukta bir mesaj var  mı yok mu
            QueueProperties properties = await _queueClient.GetPropertiesAsync();

            if (properties.ApproximateMessagesCount > 0)
            {
                // ReceiveMessagesAsync çoğul geliyor fakat biz 1 mesaj okumak istiyoruz o yüzden 1 veriyoruz görümezlik zamanında 1 dakika verdim Eğer 1 dakika içersinde silemezsem başkları bu mesajı okuyabilir kuyrukta yine mevcut olacağından dolayı başklar bu mesajı okuyabilir çift okuma olur       dikkat et bir mesajı aşağı yukarı ne kadar sürede işlenebiliniyorsa ona yakın süre verilmelidir.
                QueueMessage[] queueMessages = await _queueClient.ReceiveMessagesAsync(1, TimeSpan.FromMinutes(1));

                if (queueMessages.Any())//Herhangi bir data varsa
                {
                    return queueMessages[0];//Zaten bir tane aldığımdan dolayı
                }
            }

            return null;//kuyrukta mesaj kalmamış 
        }


        //Mesaj silme işlemi .  popReceipt : eğerki kuyruktan silerken başarısız olurasak başka clientlar o mesajı siliyor olacak
        public async Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            // popReceipt : Yani silmede başarısız olursam başkaları silecek
            await _queueClient.DeleteMessageAsync(messageId, popReceipt); 
        }



    }
}
