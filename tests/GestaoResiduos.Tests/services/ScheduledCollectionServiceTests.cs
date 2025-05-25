using GestaoResiduos.API.Data;
using GestaoResiduos.API.Models;
using GestaoResiduos.API.Services;
using GestaoResiduos.API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestaoResiduos.Tests
{
    public class ScheduledCollectionServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly ScheduledCollectionService _service;

        public ScheduledCollectionServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockNotificationService = new Mock<INotificationService>();
            _service = new ScheduledCollectionService(_context, _mockNotificationService.Object);

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
                CurrentQuantity = 150,
                AlertThreshold = 100,
                AlertActive = true,
                CreatedAt = DateTime.Now
            };

            var collectionPoint = new CollectionPoint
            {
                Id = 1,
                Name = "Ecoponto Centro",
                Location = "Rua Principal, 123",
                Latitude = -23.5505,
                Longitude = -46.6333,
                ResponsiblePerson = "João Silva",
                Contact = "(11) 99999-9999",
                IsActive = true,
                AcceptedCategories = "Papel,Plástico",
                CreatedAt = DateTime.Now
            };

            var collection = new ScheduledCollection
            {
                Id = 1,
                ResidueId = 1,
                CollectionPointId = 1,
                ScheduledDate = DateTime.Now.AddDays(1),
                Status = "Pending",
                EstimatedQuantity = 50,
                CreatedAt = DateTime.Now
            };

            _context.Residues.Add(residue);
            _context.CollectionPoints.Add(collectionPoint);
            _context.ScheduledCollections.Add(collection);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResults()
        {
            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalItems.Should().Be(1);
            result.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnScheduledCollection()
        {
            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.ResidueName.Should().Be("Papel Teste");
            result.CollectionPointName.Should().Be("Ecoponto Centro");
            result.Status.Should().Be("Pending");
            result.EstimatedQuantity.Should().Be(50);
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_ShouldCreateScheduledCollection()
        {
            // Arrange
            var model = new CreateScheduledCollectionViewModel
            {
                ResidueId = 1,
                CollectionPointId = 1,
                ScheduledDate = DateTime.Now.AddDays(2),
                EstimatedQuantity = 75,
                Notes = "Coleta de teste"
            };

            // Act
            var result = await _service.CreateAsync(model);

            // Assert
            result.Should().NotBeNull();
            result.ResidueId.Should().Be(1);
            result.CollectionPointId.Should().Be(1);
            result.Status.Should().Be("Pending");
            result.EstimatedQuantity.Should().Be(75);
            result.Notes.Should().Be("Coleta de teste");
            
            // Verificar se notificação foi criada
            _mockNotificationService.Verify(
                x => x.CreateAsync(It.IsAny<CreateNotificationViewModel>()), 
                Times.Once
            );
        }

        [Fact]
        public async Task CreateAsync_WithIncompatibleCategory_ShouldThrowException()
        {
            // Arrange - Criar resíduo de categoria não aceita pelo ponto
            var organicResidue = new Residue
            {
                Id = 2,
                Name = "Resíduo Orgânico",
                Description = "Restos de comida",
                Category = "Orgânico",
                CurrentQuantity = 100,
                AlertThreshold = 80,
                AlertActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Residues.Add(organicResidue);
            await _context.SaveChangesAsync();

            var model = new CreateScheduledCollectionViewModel
            {
                ResidueId = 2, // Resíduo orgânico
                CollectionPointId = 1, // Ponto que só aceita Papel e Plástico
                ScheduledDate = DateTime.Now.AddDays(1),
                EstimatedQuantity = 30
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(model)
            );
            exception.Message.Should().Contain("não aceita resíduos da categoria 'Orgânico'");
        }

        [Fact]
        public async Task CompleteCollectionAsync_WithValidData_ShouldCompleteCollection()
        {
            // Arrange
            var model = new CompleteCollectionViewModel
            {
                ActualQuantity = 45,
                Notes = "Coleta realizada com sucesso"
            };

            // Act
            var result = await _service.CompleteCollectionAsync(1, model);

            // Assert
            result.Should().NotBeNull();
            result!.Status.Should().Be("Completed");
            result.ActualQuantity.Should().Be(45);
            result.CompletedAt.Should().NotBeNull();
            result.Notes.Should().Be("Coleta realizada com sucesso");

            // Verificar se a quantidade do resíduo foi atualizada
            var residue = await _context.Residues.FindAsync(1);
            residue!.CurrentQuantity.Should().Be(105); // 150 - 45 = 105
            residue.LastCollectionDate.Should().NotBeNull();

            // Verificar se notificação foi criada
            _mockNotificationService.Verify(
                x => x.CreateAsync(It.IsAny<CreateNotificationViewModel>()), 
                Times.Once
            );
        }

        [Fact]
        public async Task CompleteCollectionAsync_AlreadyCompleted_ShouldThrowException()
        {
            // Arrange - Completar a coleta primeiro
            var collection = await _context.ScheduledCollections.FindAsync(1);
            collection!.Status = "Completed";
            await _context.SaveChangesAsync();

            var model = new CompleteCollectionViewModel
            {
                ActualQuantity = 30
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CompleteCollectionAsync(1, model)
            );
            exception.Message.Should().Contain("já está concluída");
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteCollection()
        {
            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            result.Should().BeTrue();
            
            var deletedCollection = await _context.ScheduledCollections.FindAsync(1);
            deletedCollection.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_CompletedCollection_ShouldThrowException()
        {
            // Arrange - Marcar como concluída
            var collection = await _context.ScheduledCollections.FindAsync(1);
            collection!.Status = "Completed";
            await _context.SaveChangesAsync();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.DeleteAsync(1)
            );
            exception.Message.Should().Contain("Não é possível excluir uma coleta já concluída");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
