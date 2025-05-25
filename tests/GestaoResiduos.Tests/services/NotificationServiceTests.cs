using GestaoResiduos.API.Data;
using GestaoResiduos.API.Models;
using GestaoResiduos.API.Services;
using GestaoResiduos.API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestaoResiduos.Tests
{
    public class NotificationServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _service;

        public NotificationServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new NotificationService(_context);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var residue = new Residue
            {
                Id = 1,
                Name = "Papel Teste",
                Description = "Papel de escritório",
                Category = "Papel",
                CurrentQuantity = 50,
                AlertThreshold = 100,
                AlertActive = false,
                CreatedAt = DateTime.Now
            };

            var collectionPoint = new CollectionPoint
            {
                Id = 1,
                Name = "Ecoponto Teste",
                Location = "Rua Teste, 123",
                Latitude = -23.5505,
                Longitude = -46.6333,
                ResponsiblePerson = "João Silva",
                Contact = "(11) 99999-9999",
                IsActive = true,
                AcceptedCategories = "Papel",
                CreatedAt = DateTime.Now
            };

            var notifications = new List<Notification>
            {
                new Notification
                {
                    Id = 1,
                    Title = "Alerta de Coleta",
                    Message = "Resíduo atingiu limite",
                    NotificationType = "CollectionAlert",
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    ResidueId = 1
                },
                new Notification
                {
                    Id = 2,
                    Title = "Coleta Agendada",
                    Message = "Nova coleta foi agendada",
                    NotificationType = "ScheduledCollection",
                    IsRead = true,
                    CreatedAt = DateTime.Now.AddMinutes(-30),
                    CollectionPointId = 1
                }
            };

            _context.Residues.Add(residue);
            _context.CollectionPoints.Add(collectionPoint);
            _context.Notifications.AddRange(notifications);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResults()
        {
            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalItems.Should().Be(2);
            result.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnNotification()
        {
            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Title.Should().Be("Alerta de Coleta");
            result.NotificationType.Should().Be("CollectionAlert");
            result.IsRead.Should().BeFalse();
            result.ResidueName.Should().Be("Papel Teste");
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_ShouldCreateNotification()
        {
            // Arrange
            var model = new CreateNotificationViewModel
            {
                Title = "Nova Notificação",
                Message = "Mensagem de teste",
                NotificationType = "General",
                ResidueId = 1
            };

            // Act
            var result = await _service.CreateAsync(model);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Nova Notificação");
            result.Message.Should().Be("Mensagem de teste");
            result.IsRead.Should().BeFalse(); // Deve ser criada como não lida
            result.ResidueName.Should().Be("Papel Teste");
            
            var savedNotification = await _context.Notifications.FindAsync(result.Id);
            savedNotification.Should().NotBeNull();
        }

        [Fact]
        public async Task MarkAsReadAsync_WithValidId_ShouldMarkAsRead()
        {
            // Act
            var result = await _service.MarkAsReadAsync(1);

            // Assert
            result.Should().BeTrue();
            
            var notification = await _context.Notifications.FindAsync(1);
            notification!.IsRead.Should().BeTrue();
        }

        [Fact]
        public async Task MarkAsReadAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Act
            var result = await _service.MarkAsReadAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetUnreadCountAsync_ShouldReturnCorrectCount()
        {
            // Act
            var result = await _service.GetUnreadCountAsync();

            // Assert
            result.Should().Be(1); // Apenas 1 notificação não lida nos dados de teste
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteNotification()
        {
            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            result.Should().BeTrue();
            
            var deletedNotification = await _context.Notifications.FindAsync(1);
            deletedNotification.Should().BeNull();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
