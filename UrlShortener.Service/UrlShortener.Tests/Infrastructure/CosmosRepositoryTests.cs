using Microsoft.Azure.Cosmos;
using Moq;
using UrlShortener.Infrastructure.Repositories;

namespace UrlShortener.Tests.Infrastructure
{
    public class CosmosRepositoryTests
    {

        private Mock<Database> _database;
        private Mock<Container> _container;

        public CosmosRepositoryTests()
        {
            _database = new Mock<Database>();
            _container = new Mock<Container>(); 
        }

        [Fact]
        public async void GetShortUrlDtoAsync_Valid()
        {
            string expected = "test id";
            var mockItemResponse = new Mock<ItemResponse<ShortUrlDto>>();

            mockItemResponse.Setup(x => x.Resource)
                .Returns(new ShortUrlDto(){id = "test id"});
            
            _container.Setup(c => c.ReadItemAsync<ShortUrlDto>(It.IsAny<string>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockItemResponse.Object);
           
            _database.Setup(db => db.GetContainer(It.IsAny<string>()))
                .Returns(_container.Object);

            CosmosRepository cosmosRepository = new CosmosRepository(_database.Object);

            var actual = await cosmosRepository.GetShortUrlDtoAsync("test", new CancellationToken());

            Assert.Equal(expected, actual.id);
        }

        [Fact]
        public async void GetShortUrlDtoAsync_Throws_Not_Found()
        {
            string expected = "No Short URL ID of test found in CosmosDB";
            var mockItemResponse = new Mock<ItemResponse<ShortUrlDto>>();
            
            _container.Setup(c => c.ReadItemAsync<ShortUrlDto>(It.IsAny<string>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CosmosException("id not found", System.Net.HttpStatusCode.NotFound, 0, "", 0));
           
            _database.Setup(db => db.GetContainer(It.IsAny<string>()))
                .Returns(_container.Object);

            CosmosRepository cosmosRepository = new CosmosRepository(_database.Object);

            KeyNotFoundException actualException = await Assert.ThrowsAsync<KeyNotFoundException>(() => cosmosRepository.GetShortUrlDtoAsync("test", new CancellationToken()));

            Assert.Equal(expected, actualException.Message);
        }

        [Fact]
        public async void InsertNewShortUrlAsync_Valid()
        {
            string expected = "test id";
            var mockItemResponse = new Mock<ItemResponse<ShortUrlDto>>();

            mockItemResponse.Setup(x => x.Resource)
                .Returns(new ShortUrlDto(){id = "test id"});
            
            _container.Setup(c => c.CreateItemAsync(It.IsAny<ShortUrlDto>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockItemResponse.Object);
           
            _database.Setup(db => db.GetContainer(It.IsAny<string>()))
                .Returns(_container.Object);

            CosmosRepository cosmosRepository = new CosmosRepository(_database.Object);

            var actual = await cosmosRepository.InsertNewShortUrlAsync("test", new ShortUrlDto(), new CancellationToken());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void InsertNewShortUrlAsync_Throws()
        {
            string expected = "could not create item.";
            
            _container.Setup(c => c.CreateItemAsync(It.IsAny<ShortUrlDto>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CosmosException("could not create item.", System.Net.HttpStatusCode.NotFound, 0, "", 0));
           
            _database.Setup(db => db.GetContainer(It.IsAny<string>()))
                .Returns(_container.Object);

            CosmosRepository cosmosRepository = new CosmosRepository(_database.Object);

            CosmosException actualException = await Assert.ThrowsAsync<CosmosException>(() => cosmosRepository.InsertNewShortUrlAsync("test", new ShortUrlDto(), new CancellationToken()));

            Assert.Equal(expected, actualException.Message);
        }

        [Fact]
        public async void VerifyIdAsync_True()
        {
            bool expected = true; 
            var mockResponseMessage = new Mock<ResponseMessage>();

            mockResponseMessage.Setup(x => x.StatusCode)
                .Returns(System.Net.HttpStatusCode.NotFound);
            
            _container.Setup(c => c.ReadItemStreamAsync(It.IsAny<string>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponseMessage.Object);
           
            _database.Setup(db => db.GetContainer(It.IsAny<string>()))
                .Returns(_container.Object);

            CosmosRepository cosmosRepository = new CosmosRepository(_database.Object);

            var actual = await cosmosRepository.VerifyIdAsync("test");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void VerifyIdAsync_False()
        {
            bool expected = false; 
            var mockResponseMessage = new Mock<ResponseMessage>();

            mockResponseMessage.Setup(x => x.StatusCode)
                .Returns(System.Net.HttpStatusCode.Found);
            
            _container.Setup(c => c.ReadItemStreamAsync(It.IsAny<string>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponseMessage.Object);
           
            _database.Setup(db => db.GetContainer(It.IsAny<string>()))
                .Returns(_container.Object);

            CosmosRepository cosmosRepository = new CosmosRepository(_database.Object);

            var actual = await cosmosRepository.VerifyIdAsync("test");

            Assert.Equal(expected, actual);
        }
    }
}