namespace Lidas.MangaApi.Interfaces;

public interface IProvider
{
    public Task<string> UploadImage(IFormFile file);
}
