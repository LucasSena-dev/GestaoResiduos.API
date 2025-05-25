using GestaoResiduos.API.Controllers;
using GestaoResiduos.API.Services;
using GestaoResiduos.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GestaoResiduos.Tests
{
    public class ResiduesControllerTests
    {
        private readonly Mock<IResidueService> _mockService;
        private readonly ResiduesController _controller;

        public ResiduesControllerTests()
        {
            _mockService = new Mock<IResidueService>();
            _controller = new ResiduesController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithStatusCode200()
        {
            // Arrange
            var mockResponse = new PaginatedResponse<ResidueViewModel>
            {
                Items = new List<ResidueViewModel>
                {
                    new ResidueViewModel 
                    { 
                        Id = 1, 
                        Name = "Teste", 
                        Category = "Papel",
                        Description = "Teste Desc",
                        CurrentQuantity = 50,
                        AlertThreshold = 100,
                        AlertActive = false,
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
