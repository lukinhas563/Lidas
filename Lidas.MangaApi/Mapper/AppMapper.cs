using AutoMapper;
using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Models.InputModels;
using Lidas.MangaApi.Models.ViewModels;

namespace Lidas.MangaApi.Mapper;

public class AppMapper: Profile
{
    public AppMapper()
    {
        // Views
        CreateMap<Manga, MangaView>();
        CreateMap<Manga, MangaViewList>();

        CreateMap<Category, CategoryView>();
        CreateMap<Category, CategoryViewList>();

        CreateMap<Author, AuthorView>();
        CreateMap<Author, AuthorViewList>();

        CreateMap<Chapter, ChapterView>();

        CreateMap<Role, RoleView>();
        CreateMap<Role, RoleViewList>();

        // Inputs
        CreateMap<MangaInput, Manga>();

        CreateMap<CategoryInput, Category>();

        CreateMap<AuthorInput, Author>();

        CreateMap<ChapterInput, Chapter>();

        CreateMap<RoleInput, Role>();
    }
}
