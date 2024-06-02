using System.Collections.Generic;
using System.Data;
using BuildingMaterialsStoreManagement.Internal.Models;

namespace BuildingMaterialsStoreManagement.Internal.Database.Abstract
{
    public interface IRepository<in T> where T : class
    {
        void Add(T entity);
        void Delete(int id);
        DataTable GetAll();
        void Update(T entity);
    }
    
    public interface IRepositoryWithItems<in T> : IRepository<T> where T : class
    {
        DataTable GetAllItems(int id);
        void AddItem(Item item);
        void UpdateItem(Item entity);
        void DeleteItem(int id);
    }
}