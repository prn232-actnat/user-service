using DataAccess.GenericRepository;
using Repositories.IRepository;
using Repositories.IRepository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ITransactionRepository Transactions { get; }
        IOrderRepository Orders { get; }
        IQuestionRepository Questions { get; }
        ICommentRepository Comments { get; }

        IQuestionLikeRepository QuestionLikes { get; }
        ICommentLikeRepository CommentLikes { get; }

        Task<int> SaveChangesAsync();
    }
}
