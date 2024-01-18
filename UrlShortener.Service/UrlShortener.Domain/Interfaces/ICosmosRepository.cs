
namespace UrlShortener.Domain.Interfaces
{
    public interface ICosmosRepository
    {
        Task<ShortUrlDto> GetShortUrlDtoAsync(string id, CancellationToken cancellationToken);
        Task<string> InsertNewShortUrlAsync(string urlId, ShortUrlDto shortUrlDto, CancellationToken cancellationToken);
        Task<bool> VerifyIdAsync(string id);
    }
}