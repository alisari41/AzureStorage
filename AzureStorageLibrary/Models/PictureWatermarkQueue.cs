using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Models
{
    public class PictureWatermarkQueue
    {
        //Mesaj kime ait onu tutumam lazım
        public string UserId { get; set; } //RowKey
        public string City { get; set; } //PartitionKey

        //Watermark eklencek resimleri bir dizi şeklinde tutuluyor
        public List<string> WatermarkPictures { get; set; }

        public string ConnectionId { get; set; }

        public string WatermarkText { get; set; } //Resme eklemek istediği yazı
    }
}
