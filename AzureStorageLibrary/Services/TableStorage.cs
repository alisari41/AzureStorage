using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace AzureStorageLibrary.Services
{
    public class TableStorage<TEntity> : INoSqlStorage<TEntity> where TEntity : TableEntity, new()//Buraya geçilebilen class mutlaka nesne örneği alınabilien olsun yani new() bize burada abstract veya static alamaz diyor
    {//
        private readonly CloudTableClient _cloudTableClient;//Azure table storage larda işlem yapabilicem. Tüm tablolar üstünde işlem yapmak
        private readonly CloudTable _table;// Tek bir tabloda işlem yapmak

        public TableStorage()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionStrings.AzureStorageConnectionString);//Bağlantı yapmamı sağlıcak
            _cloudTableClient = storageAccount.CreateCloudTableClient();
            _table = _cloudTableClient.GetTableReference(typeof(TEntity).Name);//Tablo ismini aldım

            _table.CreateIfNotExists();//Eğer tablo yoksa oluştur.
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            var operation = TableOperation.InsertOrMerge(entity);//var dememim sebebi Türünü otomatik belirtmesi mesela TableOperation operation yazmak yerine var yazarak çözüyoruz
            var execute = await _table.ExecuteAsync(operation);

            return execute.Result as TEntity;
        }

        public async Task Delete(string rowKey, string partitionKey)
        {
            var entity = await Get(rowKey, partitionKey);
            var operation = TableOperation.Delete(entity);

            await _table.ExecuteAsync(operation);//Silme işlemi gerçekleşecek


        }

        public async Task<TEntity> Update(TEntity entity)
        {
            var operation = TableOperation.Replace(entity);

            var execute = await _table.ExecuteAsync(operation);

            return execute.Result as TEntity;
        }

        public async Task<TEntity> Get(string rowKey, string partitionKey)
        {
            var operation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);

            var execute = await _table.ExecuteAsync(operation);

            return execute.Result as TEntity;
        }

        public IQueryable<TEntity> All()
        {
            return _table.CreateQuery<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return _table.CreateQuery<TEntity>().Where(query);
        }
    }
}
