using DataAccess.GenericRepository;
using Repositories.IRepository;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserServiceContext _context;

        public IUserRepository Users { get; }

        public UnitOfWork(
            UserServiceContext context,
            IUserRepository userRepository)
        {
            _context = context;
            Users = userRepository;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
