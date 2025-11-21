using DataAccess.GenericRepository;
using Repositories.IRepository.Repositories.IRepository;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(UserServiceContext context) : base(context)
        {

        }
    }
}
