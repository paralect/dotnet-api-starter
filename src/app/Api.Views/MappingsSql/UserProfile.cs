using Api.Views.Models.View.User;
using AutoMapper;
using Common.DalSql.Entities;

namespace Api.Sql.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserViewModel>();
    }
}
