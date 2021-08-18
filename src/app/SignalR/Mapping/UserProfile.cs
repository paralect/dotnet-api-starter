using AutoMapper;
using Common.Models;
using SignalR.Models;

namespace SignalR.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<IUser, UserViewModel>();
        }
    }
}
