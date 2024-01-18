using MediatR;
using UrlShortener.Domain.Interfaces;

namespace ReferralPartnerServices.Domain.UseCases
{
    public class GetUrlMessage : IRequest<string>
    {
        public required string UrlId { get; set; }
    }

    public class GetUrl : IRequestHandler<GetUrlMessage, string>
    {
        private readonly ICosmosRepository _cosmosRepository;
        private readonly IShortUrlRepository _shortUrlRepository; 

        public GetUrl(ICosmosRepository cosmosRepository, IShortUrlRepository shortUrlRepository)
        {
            _cosmosRepository = cosmosRepository;
            _shortUrlRepository = shortUrlRepository;
        }

        public async Task<string> Handle(GetUrlMessage request, CancellationToken cancellationToken)
        {
            int index = _shortUrlRepository.RestoreSeedFromString(request.UrlId);
            try 
            {
                ShortUrlDto retrievedDto = await _cosmosRepository.GetShortUrlDtoAsync(index.ToString(), cancellationToken);
                return retrievedDto.url;
            }
            catch 
            {
                throw new KeyNotFoundException("Invalid url has been provided.");
            }
        }
    }
}