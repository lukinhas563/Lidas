using Lidas.WishlistApi.Models.View;
using Refit;

namespace Lidas.WishlistApi.Interfaces;

public interface IRequestService
{
    [Get("/api/manga/list")]
    Task<List<MangaViewList>> GetAll([Query(CollectionFormat.Multi)] List<Guid> mangaIds);
}
