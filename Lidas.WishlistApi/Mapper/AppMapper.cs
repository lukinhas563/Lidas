using AutoMapper;
using Lidas.WishlistApi.Entities;
using Lidas.WishlistApi.Models.Input;
using Lidas.WishlistApi.Models.View;

namespace Lidas.WishlistApi.Mapper;

public class AppMapper: Profile
{
    public AppMapper()
    {
        // View
        CreateMap<Wishlist, WishlistView>();
        CreateMap<WishItem, WishitemView>();

        // Input
        CreateMap<WishitemInput, WishItem>();
    }
}
