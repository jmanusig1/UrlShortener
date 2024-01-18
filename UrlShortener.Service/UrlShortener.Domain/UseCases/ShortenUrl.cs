using MediatR;
using UrlShortener.Domain.Interfaces;

namespace ReferralPartnerServices.Domain.UseCases
{
    public class ShortenUrlMessage : IRequest<string>
    {
        public required string UrlToShorten { get; set; }
    }

    public class ShortenUrl : IRequestHandler<ShortenUrlMessage, string>
    {

        private readonly ICosmosRepository _cosmosRepository;
        private readonly IShortUrlRepository _shortUrlRepository; 
        private readonly string _domainName = "http://localhost:5000/";

        public ShortenUrl(ICosmosRepository cosmosRepository, IShortUrlRepository shortUrlRepository)
        {
            _cosmosRepository = cosmosRepository;
            _shortUrlRepository = shortUrlRepository;
        }

        public async Task<string> Handle(ShortenUrlMessage request, CancellationToken cancellationToken)
        {
            //generate random id (eventually want this to be a counter) --> spin multiple instances --> use distributed coordinator (like zookeeper) --> set to certain int ranges
            Random random = new Random();
            int randomNo = random.Next(int.MaxValue);
            string shortString = _shortUrlRepository.GenerateShortString(randomNo);
            bool validateShortString = await _cosmosRepository.VerifyIdAsync(shortString);

            //just keep retrying until we get a valid url 
            while(!validateShortString)
            {
                randomNo = random.Next(int.MaxValue);
                shortString = _shortUrlRepository.GenerateShortString(randomNo);
                validateShortString = await _cosmosRepository.VerifyIdAsync(shortString);
            }
            
            //insert new short url DTO into database
            ShortUrlDto addDto = new ShortUrlDto()
            {
                id = randomNo.ToString(),
                url = request.UrlToShorten,
                shortUrl = _domainName + shortString,
                timesClicked = 0,
                dateCreated = DateTime.Now,
                expirationDate = DateTime.Now.AddDays(30)
            };
    
            await _cosmosRepository.InsertNewShortUrlAsync(randomNo.ToString(), addDto, cancellationToken);
            return _domainName + shortString;
        }
    }
}