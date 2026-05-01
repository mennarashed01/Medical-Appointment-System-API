using SW_Project.Data;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;


namespace SW_Project.Repositories.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(User user)
        {
            _context.Users.Add(user);
        }
        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public User GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
             
        }

        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
