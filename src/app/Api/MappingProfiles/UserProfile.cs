using Api.Models.User;
using AutoMapper;
using Common.DAL.Documents.User;

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
