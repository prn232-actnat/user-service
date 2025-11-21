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
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        public QuestionRepository(UserServiceContext context) : base(context)
        {

        }
    }
}
