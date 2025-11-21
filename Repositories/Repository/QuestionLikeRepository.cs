using DataAccess.GenericRepository;
using Repositories.IRepository;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class QuestionLikeRepository : GenericRepository<QuestionLike>, IQuestionLikeRepository
    {
        public QuestionLikeRepository(UserServiceContext context) : base(context)
        {
        }
    }
}
