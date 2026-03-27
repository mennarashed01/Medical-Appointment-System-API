using SW_Project.Models;

namespace SW_Project.Repositories.IRepository
{
    public interface ISecretaryRepository
    {
        void Add(Secretary secertary);
        Secretary GetByUserId(int userId);
        void Save();
        void Delete(int id);
        Secretary GetById(int id);
    }
}
