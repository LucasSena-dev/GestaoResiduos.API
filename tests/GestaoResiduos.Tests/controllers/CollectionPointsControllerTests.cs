using GestaoResiduos.API.Controllers;
using GestaoResiduos.API.Services;
using GestaoResiduos.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GestaoResiduos.Tests
{
    public class CollectionPointsControllerTests
    {
        private readonly Mock<ICollectionPointService> _mockService;
        private readonly CollectionPointsController _controller;

        public CollectionPointsControllerTests()
        {
            _mockService = new Mock<ICollectionPointService>();
            _controller = new CollectionPointsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithStatusCode200()
        {
            // Arrange
            var mockResponse = new PaginatedResponse<CollectionPointViewModel>
            {
                Items = new List<CollectionPointViewModel>
                {
                    new CollectionPointViewModel
                    {
                        Id = 1,
                        Name = "Ecoponto Teste",
                        Location = "Rua Teste, 123",
                        Latitude = -23.5505,
                        Longitude = -46.6333,
                        ResponsiblePerson = "João Silva",
                        Contact = "(11) 99999-9999",
                        IsActive = true,
                        AcceptedCategories = "Papel,Plástico",
                        CreatedAt = DateTime.Now
                    }
                },
                Page = 1,
                PageSize = 10,
                TotalItems = 1,
                TotalPages = 1
            };

            _mockService.Setup(s => s.GetAllAsync(1, 10))
                       .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAll(1, 10);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
        }
    }
}
