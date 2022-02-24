using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary
{
    public enum EContainerName
    {//Resimleri ve pdf ler tutmak için container tutuyorum istersek video falan diğer uzantılar için devam edebiliriz
        //Küçük harf kullanılmak zorunda bunu azure container oluştururken bu isimleri vermemiz gerekir. 
        pictures,
        pdf,
        logs,
    }
    public interface IBlobStorage
    {
        public string BlobUrl { get; }

        //Task geriye herhangi birşey döndürmeyecek bu bizim Void metodlara karşılık gelir
        Task UploadAsync(Stream fileStream, string fileName, EContainerName eContainerName);

        //Task<Strean> geriye Stream döndürecek
        Task<Stream> DownloadAsync(string fileName, EContainerName eContainerName);

        Task DeleteAsync(string fileName, EContainerName eContainerName);


        //Block Blob
        Task SetLogAsync(string text, string fileName);
        Task<List<string>> GetLogAsync(string fileName);



        //Bir Container içindeki tüm blobları alma işlemi
        List<string> GetNames(EContainerName eContainerName);
    }
}
