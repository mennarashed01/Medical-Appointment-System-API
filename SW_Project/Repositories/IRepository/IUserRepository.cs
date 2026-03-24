using SW_Project.Models;

namespace SW_Project.Repositories.IRepository
{
    public interface IUserRepository
    {
        User GetByEmail(string email);
        User GetById(int id);
        void Add(User user);
        void Update(User user);
        void Save();
    }
}
