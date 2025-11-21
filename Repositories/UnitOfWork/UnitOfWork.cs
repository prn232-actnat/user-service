using DataAccess.GenericRepository;
using Repositories.IRepository;
using Repositories.IRepository.Repositories.IRepository;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
    namespace DataAccess.UnitOfWork
    {
        public class UnitOfWork : IUnitOfWork
        {
            private readonly UserServiceContext _context;

            public IUserRepository Users { get; }
            public ITransactionRepository Transactions { get; }
            public IOrderRepository Orders { get; }
            public IQuestionRepository Questions { get; }
            public ICommentRepository Comments { get; }
            public IQuestionLikeRepository QuestionLikes { get; } 
            public ICommentLikeRepository CommentLikes { get; }
            public UnitOfWork(
                UserServiceContext context,
                IUserRepository userRepository,
                ITransactionRepository transactionRepository,
                IOrderRepository orderRepository,
                IQuestionRepository questionRepository,
                ICommentRepository commentRepository,
                IQuestionLikeRepository questionLikeRepository,
                ICommentLikeRepository commentLikeRepository)
            {
                _context = context;
                Users = userRepository;
                Transactions = transactionRepository;
                Orders = orderRepository;
                Questions = questionRepository;
                Comments = commentRepository;
                QuestionLikes = questionLikeRepository;
                CommentLikes = commentLikeRepository;
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
}
