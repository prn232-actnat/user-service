using DataAccess.GenericRepository;
using DataAccess.UnitOfWork;
using DataAccess.UnitOfWork.DataAccess.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Repositories.IRepository;
using Repositories.IRepository.Repositories.IRepository;
using Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentLikeRepository, CommentLikeRepository>();
            services.AddScoped<IQuestionLikeRepository, QuestionLikeRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }
    }
}
