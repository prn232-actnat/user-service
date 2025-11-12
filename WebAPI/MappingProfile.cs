using AutoMapper;
using DTOs;
using DTOs.Response;
using Repositories.Models;

namespace WebAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Payment, PaymentDto>();
            CreateMap<Transaction, TransactionDto>();
            CreateMap<User, UserDto>();
            CreateMap<User, UserResponseDto>();
        }
    }
}
