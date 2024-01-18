using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using ReferralPartnerServices.Domain.UseCases;
using UrlShortenerServices.API.Controllers;

namespace UrlShortener.Tests.Api
{
    public class UrlShortenerControllerTests
    {
        private Mock<IMediator> _mediator;

        public UrlShortenerControllerTests()
        {
            _mediator = new Mock<IMediator>();
        }

        [Fact]
        public async void ShortenUrl_Valid()
        {
            UrlShortenerController controller = new UrlShortenerController(_mediator.Object);

            _mediator.Setup(repo => repo.Send(It.IsAny<ShortenUrlMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("test");

            IActionResult actual = await controller.ShortenUrl(new ShortenUrlRequest(){url = "https://www.google.com/"});
            var actualResult = actual as OkObjectResult;

            Assert.Equal(((IStatusCodeActionResult) actual).StatusCode , 200);
            Assert.Equal(actualResult?.Value , "test");
        }

        [Fact]
        public async void ShortenUrl_Throws_Bad_Uri()
        {
            UrlShortenerController controller = new UrlShortenerController(_mediator.Object);

            _mediator.Setup(repo => repo.Send(It.IsAny<ShortenUrlMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("test");

            IActionResult actual = await controller.ShortenUrl(new ShortenUrlRequest(){url = "baduridotcom"});
            var actualResult = actual as BadRequestObjectResult;

            Assert.Equal(((IStatusCodeActionResult) actual).StatusCode , 400);
            Assert.Equal(actualResult?.Value , "Invalid url has been provided.");
        }

        [Fact]
        public async void RedirectFromShortLink_Valid()
        {
            UrlShortenerController controller = new UrlShortenerController(_mediator.Object);

            _mediator.Setup(repo => repo.Send(It.IsAny<GetUrlMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("test");

            IActionResult actual = await controller.RedirectFromShortLink("https://www.short.com/12345");
            var actualResult = actual as RedirectResult;

            Assert.Equal("test", actualResult?.Url);
        }

        [Fact]
        public async void RedirectFromShortLink_Bad_URL()
        {
            UrlShortenerController controller = new UrlShortenerController(_mediator.Object);

            _mediator.Setup(repo => repo.Send(It.IsAny<GetUrlMessage>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new KeyNotFoundException("Invalid url has been provided."));

            IActionResult actual = await controller.RedirectFromShortLink("https://www.short.com/12345");
            var actualResult = actual as BadRequestObjectResult;

            Assert.Equal(((IStatusCodeActionResult) actual).StatusCode , 400);
            Assert.Equal(actualResult?.Value , "Invalid url has been provided.");
        }
    }
}