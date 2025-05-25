using GestaoResiduos.API.Controllers;
using GestaoResiduos.API.Services;
using GestaoResiduos.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GestaoResiduos.Tests
{
    public class NotificationsControllerTests
    {
        private readonly Mock<INotificationService> _mockService;
        private readonly NotificationsController _controller;

        public NotificationsControllerTests()
        {
            _mockService = new Mock<INotificationService>();
            _controller = new NotificationsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithStatusCode200()
        {
            // Arrange
            var mockResponse = new PaginatedResponse<NotificationViewModel>
            {
                Items = new List<NotificationViewModel>
                {
                    new NotificationViewModel
                    {
                        Id = 1,
                        Title = "Alerta de Coleta",
                        Message = "Resíduo atingiu limite",
                        NotificationType = "CollectionAlert",
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                        ResidueId = 1,
                        ResidueName = "Papel Teste"
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
