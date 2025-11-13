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
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Transaction, TransactionDto>().ReverseMap();
            CreateMap<User, UserDto>();
            CreateMap<User, UserResponseDto>();
        }
    }
}
