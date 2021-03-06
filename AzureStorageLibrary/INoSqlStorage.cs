using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary
{
    public interface INoSqlStorage<TEntity>
    {//Bir tablo ile ilgili en sık kullanılan metodlar tanımlanıcak
        Task<TEntity> Add(TEntity entity);
        Task Delete(string rowKey, string partitionKey);
        Task<TEntity> Update(TEntity entity);
        Task<TEntity> Get(string rowKey, string partitionKey);//En verimli iki alanda arama yapıcaz
        IQueryable<TEntity> All();
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> query);//Example : productTable.query(x=>x.price>100)
    }
}
