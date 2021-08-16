using Api.Models.User;
using AutoMapper;
using Common.DB.Postgres.DAL.Documents;

namespace Api.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
        }
    }
}
