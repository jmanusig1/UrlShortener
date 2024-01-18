
namespace UrlShortener.Domain.Interfaces
{
    public interface IShortUrlRepository
    {
        string GenerateShortString(int seed);
        int RestoreSeedFromString(string str);
    }
}