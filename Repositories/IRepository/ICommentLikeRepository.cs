using DataAccess.GenericRepository;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepository
{
    public interface ICommentLikeRepository : IGenericRepository<CommentLike>
    {
    }
}
