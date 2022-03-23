using Api.Views.Models;
using Api.Views.Models.User;
using AutoMapper;
using Common.Dal;
using Common.Dal.Documents.User;

namespace Api.NoSql.Mapping
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
