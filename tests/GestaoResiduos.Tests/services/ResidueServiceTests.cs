using GestaoResiduos.API.Data;
using GestaoResiduos.API.Models;
using GestaoResiduos.API.Services;
using GestaoResiduos.API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestaoResiduos.Tests
{
    public class ResidueServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ResidueService _service;

        public ResidueServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new ResidueService(_context);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var residues = new List<Residue>
            {
                new Residue
                {
                    Id = 1,
                    Name = "Papel Teste",
                    Description = "Papel de escritório",
                    Category = "Papel",
                    CurrentQuantity = 50,
                    AlertThreshold = 100,
                    AlertActive = false,
                    CreatedAt = DateTime.Now
                },
                new Residue
                {
                    Id = 2,
                    Name = "Plástico Teste",
                    Description = "Garrafas PET",
                    Category = "Plástico",
                    CurrentQuantity = 150,
                    AlertThreshold = 100,
                    AlertActive = true,
                    CreatedAt = DateTime.Now
                }
            };

            _context.Residues.AddRange(residues);
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
        public async Task GetByIdAsync_WithValidId_ShouldReturnResidue()
        {
            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Papel Teste");
            result.Category.Should().Be("Papel");
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
        public async Task CreateAsync_WithValidModel_ShouldCreateResidue()
        {
            // Arrange
            var model = new CreateResidueViewModel
            {
                Name = "Vidro Novo",
                Description = "Garrafas de vidro",
                Category = "Vidro",
                CurrentQuantity = 75,
                AlertThreshold = 50
            };

            // Act
            var result = await _service.CreateAsync(model);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Vidro Novo");
            result.AlertActive.Should().BeTrue(); // CurrentQuantity > AlertThreshold
            
            var savedResidue = await _context.Residues.FindAsync(result.Id);
            savedResidue.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateResidue()
        {
            // Arrange
            var model = new UpdateResidueViewModel
            {
                Name = "Papel Atualizado",
                CurrentQuantity = 200
            };

            // Act
            var result = await _service.UpdateAsync(1, model);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Papel Atualizado");
            result.CurrentQuantity.Should().Be(200);
            result.AlertActive.Should().BeTrue(); // 200 > 100
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteResidue()
        {
            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            result.Should().BeTrue();
            
            var deletedResidue = await _context.Residues.FindAsync(1);
            deletedResidue.Should().BeNull();
        }

        [Fact]
        public async Task CheckAndUpdateAlertStatusesAsync_ShouldUpdateAlertStatuses()
        {
            // Arrange - Criar um resíduo que precisa de atualização de alerta
            var residue = new Residue
            {
                Name = "Teste Alerta",
                Description = "Teste",
                Category = "Teste",
                CurrentQuantity = 150,
                AlertThreshold = 100,
                AlertActive = false, // Inconsistente - deveria ser true
                CreatedAt = DateTime.Now
            };
            _context.Residues.Add(residue);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.CheckAndUpdateAlertStatusesAsync();

            // Assert
            result.Should().BeTrue();
            
            var updatedResidue = await _context.Residues.FindAsync(residue.Id);
            updatedResidue!.AlertActive.Should().BeTrue();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}