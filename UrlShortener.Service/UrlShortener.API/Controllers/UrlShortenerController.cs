
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReferralPartnerServices.Domain.UseCases;

namespace UrlShortenerServices.API.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public class UrlShortenerController : ControllerBase 
    {
        private readonly IMediator _mediator;

        public UrlShortenerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Shortens a given url
        /// </summary>
        /// <param name="request">the url requested to be shortened</param>
        /// <returns>the link to the shortened url or a bad request if an invalid url is given</returns>
        [HttpPost]
        [Route("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            //validate the url
            Uri uriResult;
            if(!Uri.TryCreate(request.url, UriKind.Absolute, out uriResult))
            {
                return BadRequest("Invalid url has been provided.");
            }

            string ret = await _mediator.Send(new ShortenUrlMessage { UrlToShorten = request.url});
            return Ok(ret);
        }

        /// <summary>
        /// Redirects a generated short url link to the full link from database. Returns a bad request if the short link is not in database.
        /// </summary>
        /// <param name="id">id of the short url to query the database</param>
        /// <returns>Redirect or Bad Request if there is an invalid link</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> RedirectFromShortLink(string id)
        {
            try 
            {
                string ret = await _mediator.Send(new GetUrlMessage { UrlId = id});
                return Redirect(ret);
            }
            catch
            {
                return BadRequest("Invalid url has been provided.");
            }
        }
    }
}