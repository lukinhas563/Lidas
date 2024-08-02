using Lidas.LikeApi.Models.View;
using Refit;

namespace Lidas.LikeApi.Interfaces;

public interface IRequestService
{
    [Get("/api/manga/list")]
    Task<List<MangaViewList>> GetAll([Query(CollectionFormat.Multi)] List<Guid> mangaIds);
}
