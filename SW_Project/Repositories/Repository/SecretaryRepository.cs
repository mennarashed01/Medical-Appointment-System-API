using SW_Project.Data;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;

namespace SW_Project.Repositories.Repository
{
    public class SecretaryRepository : ISecretaryRepository
    {
        private readonly ApplicationDbContext context;

        public SecretaryRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void Add(Secretary secertary)
        {
            context.Secretarys.Add(secertary);
        }

        public Secretary GetByUserId(int userId)
        {
            return context.Secretarys.FirstOrDefault(s => s.UserId == userId);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
