using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace AzureStorageLibrary.Models
{
    public class Product : TableEntity
    {//Table Storage da tutulacak olan Entity yani satırım
        // 4 tane sutun buradan gelicek 3 tanede default olarak gelicek
        // RowKey = Birincil anahtar gibi
        // PartitionKey = Satır Gruplama işlemi yaparak daha hızlı listelenmesini sağlar.Mesela kullanıcı şehirlerini tutuyoruz şehirleri gruplayabiliriz.
        // TimeStamp = Zaman dalgası

        public string Name { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public string Color { get; set; }
    }
}
