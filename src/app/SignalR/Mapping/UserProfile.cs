﻿using AutoMapper;
using Common.DAL.Documents.User;
using SignalR.Models;

namespace SignalR.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
        }
    }
}
