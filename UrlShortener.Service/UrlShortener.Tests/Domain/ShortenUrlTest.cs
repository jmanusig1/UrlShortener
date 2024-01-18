using Moq;
using ReferralPartnerServices.Domain.UseCases;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Tests.Domain
{
    public class ShortenUrlTests
    {

        private Mock<ICosmosRepository> _cosmosRepository;
        private Mock<IShortUrlRepository> _shortUrlRepository;

        public ShortenUrlTests()
        {
            _cosmosRepository = new Mock<ICosmosRepository>();
            _shortUrlRepository = new Mock<IShortUrlRepository>();
        }

        [Fact]
        public async void ShortenUrl_Valid()
        {
            string expected = "http://localhost:5000/test";

            ShortenUrlMessage request = new ShortenUrlMessage()
            {
                UrlToShorten = "someurl.com"
            }; 

            _shortUrlRepository.Setup(repo => repo.GenerateShortString(It.IsAny<int>()))
                .Returns("test");
            _cosmosRepository.Setup(repo => repo.VerifyIdAsync("test"))
                .ReturnsAsync(true);
            _cosmosRepository.Setup(repo => repo.InsertNewShortUrlAsync(It.IsAny<string>(), It.IsAny<ShortUrlDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("inserted into db");

            ShortenUrl handler = new ShortenUrl(_cosmosRepository.Object, _shortUrlRepository.Object);
            string actual = await handler.Handle(request, new CancellationToken());

            Assert.Equal(actual, expected);
        }
    }
}