using Refit;

namespace Lidas.MangaApi.Interfaces;

public interface IRequestService
{
    [Get("/api/like/count/{mangaId}")]
    Task<int> GetCount(Guid mangaId);
}
