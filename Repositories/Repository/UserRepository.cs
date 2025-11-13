using DataAccess.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Repositories.IRepository;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(UserServiceContext context) : base(context)
        {

        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                                 .FirstOrDefaultAsync(a => a.Email == email);
        }
    }
}
