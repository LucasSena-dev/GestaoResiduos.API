using GestaoResiduos.API.Data;
using GestaoResiduos.API.Models;
using GestaoResiduos.API.Services;
using GestaoResiduos.API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestaoResiduos.Tests
{
    public class CollectionPointServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CollectionPointService _service;

        public CollectionPointServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new CollectionPointService(_context);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var points = new List<CollectionPoint>
            {
                new CollectionPoint
                {
                    Id = 1,
                    Name = "Ecoponto Centro",
                    Location = "Rua Principal, 123 - Centro",
                    Latitude = -23.5505,
                    Longitude = -46.6333,
                    ResponsiblePerson = "João Silva",
                    Contact = "(11) 99999-9999",
                    IsActive = true,
                    AcceptedCategories = "Papel,Plástico,Metal",
                    CreatedAt = DateTime.Now
                },
                new CollectionPoint
                {
                    Id = 2,
                    Name = "Ecoponto Norte",
                    Location = "Avenida Norte, 456",
                    Latitude = -23.5200,
                    Longitude = -46.6100,
                    ResponsiblePerson = "Maria Santos",
                    Contact = "(11) 88888-8888",
                    IsActive = false,
                    AcceptedCategories = "Vidro,Orgânico",
                    CreatedAt = DateTime.Now
                }
            };

            _context.CollectionPoints.AddRange(points);
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
        public async Task GetByIdAsync_WithValidId_ShouldReturnCollectionPoint()
        {
            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Ecoponto Centro");
            result.Location.Should().Be("Rua Principal, 123 - Centro");
            result.ResponsiblePerson.Should().Be("João Silva");
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
        public async Task CreateAsync_WithValidModel_ShouldCreateCollectionPoint()
        {
            // Arrange
            var model = new CreateCollectionPointViewModel
            {
                Name = "Ecoponto Novo",
                Location = "Rua Nova, 789",
                Latitude = -23.5300,
                Longitude = -46.6200,
                ResponsiblePerson = "Pedro Costa",
                Contact = "(11) 77777-7777",
                AcceptedCategories = "Papel,Vidro"
            };

            // Act
            var result = await _service.CreateAsync(model);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Ecoponto Novo");
            result.IsActive.Should().BeTrue(); // Deve ser criado como ativo
            result.AcceptedCategories.Should().Be("Papel,Vidro");
            
            var savedPoint = await _context.CollectionPoints.FindAsync(result.Id);
            savedPoint.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateCollectionPoint()
        {
            // Arrange
            var model = new UpdateCollectionPointViewModel
            {
                Name = "Ecoponto Centro Atualizado",
                Contact = "(11) 99999-0000",
                IsActive = false
            };

            // Act
            var result = await _service.UpdateAsync(1, model);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Ecoponto Centro Atualizado");
            result.Contact.Should().Be("(11) 99999-0000");
            result.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteCollectionPoint()
        {
            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            result.Should().BeTrue();
            
            var deletedPoint = await _context.CollectionPoints.FindAsync(1);
            deletedPoint.Should().BeNull();
        }

        [Fact]
        public async Task FindNearbyPointsAsync_ShouldReturnPointsInRadius()
        {
            // Arrange - Coordenadas próximas ao ponto 1
            double searchLatitude = -23.5500;
            double searchLongitude = -46.6300;
            double radiusKm = 1.0;

            // Act
            var result = await _service.FindNearbyPointsAsync(searchLatitude, searchLongitude, radiusKm);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCountGreaterThan(0);
            result.Should().Contain(p => p.Name == "Ecoponto Centro");
            
            // Verificar que apenas pontos ativos são retornados
            result.Should().OnlyContain(p => p.IsActive);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
