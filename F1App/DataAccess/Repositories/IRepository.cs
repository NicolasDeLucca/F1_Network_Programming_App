using System.Collections.Generic;

namespace DataAccess.Repositories
{
    public interface IRepository<T>
    {
        int Store(T t);
        T Get(int id);
        List<T> GetAll();
        void Delete(int id);
    }
}
