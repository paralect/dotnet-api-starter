using Api.Core.DAL.Documents.User;
using Api.Models.User;
using AutoMapper;

namespace Api.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
        }
    }
}
