using Api.Sql.Models.User;
using AutoMapper;

namespace Api.Sql.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserSignInModel, UserViewModel>();
        }
    }
}
