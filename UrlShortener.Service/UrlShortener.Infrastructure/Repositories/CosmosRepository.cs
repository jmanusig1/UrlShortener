using Microsoft.Azure.Cosmos;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Infrastructure.Repositories
{
    public class CosmosRepository : ICosmosRepository
    {
        private static Container _container;
        
        public CosmosRepository(Database database)
        {
            _container = database.GetContainer("ShortUrl");
        }
        
        public async Task<ShortUrlDto> GetShortUrlDtoAsync(string id, CancellationToken cancellationToken)
        {
            try 
            {
                return await _container.ReadItemAsync<ShortUrlDto>(id, new PartitionKey(id));
            }
            catch (CosmosException ex)
            {
                if(ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException($"No Short URL ID of {id} found in CosmosDB");
                }

                throw;
            }
        }

        public async Task<string> InsertNewShortUrlAsync(string urlId, ShortUrlDto shortUrlDto, CancellationToken cancellationToken)
        {
            try 
            {
                ItemResponse<ShortUrlDto> response = await _container.CreateItemAsync(shortUrlDto, new PartitionKey(urlId), null, cancellationToken);
                return response.Resource.id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> VerifyIdAsync(string id)
        {
            using (ResponseMessage responseMessage = await _container.ReadItemStreamAsync(
                partitionKey: new PartitionKey(id),
                id: id))
            {
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return true; 
                }
            }

            return false; 
        }
    }
}
