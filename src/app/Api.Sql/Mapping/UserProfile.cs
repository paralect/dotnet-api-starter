using Api.Sql.Models;
using Api.Sql.Models.User;
using AutoMapper;
using Common.DalSql;
using Common.DalSql.Entities;

namespace Api.Sql.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<Page<User>, PageModel<UserViewModel>>();
        }
    }
}
