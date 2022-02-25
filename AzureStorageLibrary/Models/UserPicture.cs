using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace AzureStorageLibrary.Models
{
    public class UserPicture : TableEntity
    {
        public string RawPaths { get; set; }//Bloblara kaydedilen resimlerin isimlerini tutucam 

        [IgnoreProperty]//Bu sayedi serialize edilmeyecek 
        public List<string> Paths
        {
            get => RawPaths == null ? null : JsonConvert.DeserializeObject<List<string>>(RawPaths);
            set => RawPaths = JsonConvert.SerializeObject(value);

        }


        public string WatermarkRawPaths { get; set; }//Resim eklenmiş olan

        [IgnoreProperty]
        public List<string> WatermarkPaths
        {
            //Eğer WatermarkRawPaths null ise null dönsün değilse diğer taraf dönsün
            get => WatermarkRawPaths == null ? null : JsonConvert.DeserializeObject<List<string>>(WatermarkRawPaths);
            set => WatermarkRawPaths = JsonConvert.SerializeObject(value);

        }
    }
}
