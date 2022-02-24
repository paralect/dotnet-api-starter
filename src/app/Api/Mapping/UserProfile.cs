using Api.Models;
using Api.Models.User;
using AutoMapper;
using Common.DAL;
using Common.DAL.Documents.User;

namespace Api.Mapping
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
