using DataAccess.GenericRepository;
using Repositories.IRepository;
using Repositories.IRepository.Repositories.IRepository;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class CommentLikeRepository : GenericRepository<CommentLike>, ICommentLikeRepository
    {
        public CommentLikeRepository(UserServiceContext context) : base(context)
        {
        }
    }
}
