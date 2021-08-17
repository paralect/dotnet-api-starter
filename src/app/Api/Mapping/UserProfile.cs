using Api.Models.User;
using AutoMapper;

namespace Api.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Common.DB.Postgres.DAL.Documents.User, UserViewModel>();
            CreateMap<Common.DB.Mongo.DAL.Documents.User.User, UserViewModel>();
        }
    }
}
