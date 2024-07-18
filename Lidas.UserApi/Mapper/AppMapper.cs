using AutoMapper;
using Lidas.UserApi.Entities;
using Lidas.UserApi.Models.Input;
using Lidas.UserApi.Models.View;

namespace Lidas.UserApi.Mapper;

public class AppMapper: Profile
{
    public AppMapper()
    {
        // View
        CreateMap<User, UserView>();
        CreateMap<User, UserViewList>();

        CreateMap<Role, RoleView>();

        // Input
        CreateMap<UserInput, User>();
        CreateMap<RoleInput, Role>();
    }
}
