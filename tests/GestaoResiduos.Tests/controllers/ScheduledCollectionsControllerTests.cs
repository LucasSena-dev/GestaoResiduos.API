using GestaoResiduos.API.Controllers;
using GestaoResiduos.API.Services;
using GestaoResiduos.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GestaoResiduos.Tests
{
    public class ScheduledCollectionsControllerTests
    {
        private readonly Mock<IScheduledCollectionService> _mockService;
        private readonly ScheduledCollectionsController _controller;

        public ScheduledCollectionsControllerTests()
        {
            _mockService = new Mock<IScheduledCollectionService>();
            _controller = new ScheduledCollectionsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithStatusCode200()
        {
            // Arrange
            var mockResponse = new PaginatedResponse<ScheduledCollectionViewModel>
            {
                Items = new List<ScheduledCollectionViewModel>
                {
                    new ScheduledCollectionViewModel
                    {
                        Id = 1,
                        ResidueId = 1,
                        ResidueName = "Papel Teste",
                        CollectionPointId = 1,
                        CollectionPointName = "Ecoponto Teste",
                        CollectionPointLocation = "Rua Teste, 123",
                        ScheduledDate = DateTime.Now.AddDays(1),
                        Status = "Pending",
                        EstimatedQuantity = 50,
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
