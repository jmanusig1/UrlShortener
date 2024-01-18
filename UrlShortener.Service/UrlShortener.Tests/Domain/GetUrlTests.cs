using Microsoft.Azure.Cosmos;
using Moq;
using ReferralPartnerServices.Domain.UseCases;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Tests.Domain
{
    public class GetUrlTests
    {

        private Mock<ICosmosRepository> _cosmosRepository;
        private Mock<IShortUrlRepository> _shortUrlRepository;

        public GetUrlTests()
        {
            _cosmosRepository = new Mock<ICosmosRepository>();
            _shortUrlRepository = new Mock<IShortUrlRepository>();
        }

        [Fact]
        public async void GetUrl_Valid()
        {
            ShortUrlDto expectedDto = new ShortUrlDto()
            {
                url = "test.com"
            };

            GetUrlMessage request = new GetUrlMessage()
            {
                UrlId = "test"
            }; 

            _shortUrlRepository.Setup(repo => repo.RestoreSeedFromString(It.IsAny<string>()))
                .Returns(123);
            _cosmosRepository.Setup(repo => repo.GetShortUrlDtoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            GetUrl handler = new GetUrl(_cosmosRepository.Object, _shortUrlRepository.Object);
            string actual = await handler.Handle(request, new CancellationToken());

            Assert.Equal(actual, expectedDto.url);
        }

        [Fact]
        public async void GetUrl_Throws_Bad_Url()
        {
            string expected = "Invalid url has been provided.";

            GetUrlMessage request = new GetUrlMessage()
            {
                UrlId = "test"
            }; 

            _shortUrlRepository.Setup(repo => repo.RestoreSeedFromString(It.IsAny<string>()))
                .Returns(123);
            _cosmosRepository.Setup(repo => repo.GetShortUrlDtoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CosmosException("ex", System.Net.HttpStatusCode.BadRequest, 0, "", 0));

            GetUrl handler = new GetUrl(_cosmosRepository.Object, _shortUrlRepository.Object);
            KeyNotFoundException actual = await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(request, new CancellationToken()));

            Assert.Equal(expected, actual.Message);
        }
    }
}