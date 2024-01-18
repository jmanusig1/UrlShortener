using UrlShortener.Infrastructure.Repositories;

namespace UrlShortener.Tests.Infrastructure
{
    public class ShortUrlRepositoryTests
    {
        [Fact]
        public void GenerateShortString_Valid()
        {
            string expected = "t4";
            ShortUrlRepository shortUrlRepository = new ShortUrlRepository();

            string actual = shortUrlRepository.GenerateShortString(1234);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RestoreSeedFromString_Valid()
        {
            int expected = 1234;
            ShortUrlRepository shortUrlRepository = new ShortUrlRepository();

            int actual = shortUrlRepository.RestoreSeedFromString("t4");

            Assert.Equal(expected, actual);
        }
    }
}