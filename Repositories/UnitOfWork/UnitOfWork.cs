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
        public ITransactionRepository Transactions { get; }
        public IOrderRepository Orders { get; }

        public UnitOfWork(
            UserServiceContext context,
            IUserRepository userRepository,
            ITransactionRepository transactionRepository,
            IOrderRepository orderRepository)
        {
            _context = context;
            Users = userRepository;
            Transactions = transactionRepository;
            Orders = orderRepository;
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
