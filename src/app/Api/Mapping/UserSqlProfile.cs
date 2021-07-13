using Api.Models.User;
using AutoMapper;
using Common.DALSql.Entities;

namespace Api.Mapping
{
    public class UserSqlProfile : Profile
    {
        public UserSqlProfile()
        {
            CreateMap<User, UserViewModel>();
        }
    }
}