using System;
using System.Linq;
using System.Threading.Tasks;
using GestaoResiduos.API.Data;
using GestaoResiduos.API.Models;
using GestaoResiduos.API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestaoResiduos.API.Services
{
    public interface IScheduledCollectionService
    {
        Task<PaginatedResponse<ScheduledCollectionViewModel>> GetAllAsync(int page, int pageSize);
        Task<ScheduledCollectionViewModel?> GetByIdAsync(int id);
        Task<ScheduledCollectionViewModel> CreateAsync(CreateScheduledCollectionViewModel model);
        Task<ScheduledCollectionViewModel?> UpdateAsync(int id, UpdateScheduledCollectionViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<ScheduledCollectionViewModel?> CompleteCollectionAsync(int id, CompleteCollectionViewModel model);
    }
    
    public class ScheduledCollectionService : IScheduledCollectionService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        
        public ScheduledCollectionService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }
        
        public async Task<PaginatedResponse<ScheduledCollectionViewModel>> GetAllAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;
            
            var totalItems = await _context.ScheduledCollections.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var collections = await _context.ScheduledCollections
                .Include(sc => sc.Residue)
                .Include(sc => sc.CollectionPoint)
                .OrderByDescending(sc => sc.ScheduledDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(sc => new ScheduledCollectionViewModel
                {
                    Id = sc.Id,
                    ResidueId = sc.ResidueId,
                    ResidueName = sc.Residue.Name,
                    CollectionPointId = sc.CollectionPointId,
                    CollectionPointName = sc.CollectionPoint.Name,
                    CollectionPointLocation = sc.CollectionPoint.Location,
                    ScheduledDate = sc.ScheduledDate,
                    Status = sc.Status,
                    EstimatedQuantity = sc.EstimatedQuantity,
                    ActualQuantity = sc.ActualQuantity,
                    CreatedAt = sc.CreatedAt,
                    CompletedAt = sc.CompletedAt,
                    Notes = sc.Notes
                })
                .ToListAsync();
                
            return new PaginatedResponse<ScheduledCollectionViewModel>
            {
                Items = collections,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
        
        public async Task<ScheduledCollectionViewModel?> GetByIdAsync(int id)
        {
            var collection = await _context.ScheduledCollections
                .Include(sc => sc.Residue)
                .Include(sc => sc.CollectionPoint)
                .FirstOrDefaultAsync(sc => sc.Id == id);
                
            if (collection == null) return null;
            
            return new ScheduledCollectionViewModel
            {
                Id = collection.Id,
                ResidueId = collection.ResidueId,
                ResidueName = collection.Residue.Name,
                CollectionPointId = collection.CollectionPointId,
                CollectionPointName = collection.CollectionPoint.Name,
                CollectionPointLocation = collection.CollectionPoint.Location,
                ScheduledDate = collection.ScheduledDate,
                Status = collection.Status,
                EstimatedQuantity = collection.EstimatedQuantity,
                ActualQuantity = collection.ActualQuantity,
                CreatedAt = collection.CreatedAt,
                CompletedAt = collection.CompletedAt,
                Notes = collection.Notes
            };
        }
        
        public async Task<ScheduledCollectionViewModel> CreateAsync(CreateScheduledCollectionViewModel model)
        {
            // Verificar se o resíduo existe
            var residue = await _context.Residues.FindAsync(model.ResidueId);
            if (residue == null)
                throw new ArgumentException("Resíduo não encontrado", nameof(model.ResidueId));
                
            // Verificar se o ponto de coleta existe
            var collectionPoint = await _context.CollectionPoints.FindAsync(model.CollectionPointId);
            if (collectionPoint == null)
                throw new ArgumentException("Ponto de coleta não encontrado", nameof(model.CollectionPointId));
                
            // Verificar se o ponto de coleta aceita este tipo de resíduo
            var acceptedCategories = collectionPoint.AcceptedCategories
                .Split(',', StringSplitOptions.RemoveEmptyEntries);
                
            if (acceptedCategories.Length > 0 && !acceptedCategories.Contains(residue.Category))
                throw new ArgumentException($"O ponto de coleta não aceita resíduos da categoria '{residue.Category}'");
                
            var collection = new ScheduledCollection
            {
                ResidueId = model.ResidueId,
                CollectionPointId = model.CollectionPointId,
                ScheduledDate = model.ScheduledDate,
                Status = "Pending", // Status padrão
                EstimatedQuantity = model.EstimatedQuantity,
                CreatedAt = DateTime.Now,
                Notes = model.Notes
            };
            
            _context.ScheduledCollections.Add(collection);
            await _context.SaveChangesAsync();
            
            // Criar notificação para nova coleta agendada
            await _notificationService.CreateAsync(new CreateNotificationViewModel
            {
                Title = "Nova Coleta Agendada",
                Message = $"Uma nova coleta para o resíduo {residue.Name} foi agendada para {model.ScheduledDate:dd/MM/yyyy HH:mm}.",
                NotificationType = "ScheduledCollection",
                ResidueId = model.ResidueId,
                CollectionPointId = model.CollectionPointId
            });
            
            // Carregar relacionamentos para o ViewModel
            await _context.Entry(collection).Reference(sc => sc.Residue).LoadAsync();
            await _context.Entry(collection).Reference(sc => sc.CollectionPoint).LoadAsync();
            
            return new ScheduledCollectionViewModel
            {
                Id = collection.Id,
                ResidueId = collection.ResidueId,
                ResidueName = collection.Residue.Name,
                CollectionPointId = collection.CollectionPointId,
                CollectionPointName = collection.CollectionPoint.Name,
                CollectionPointLocation = collection.CollectionPoint.Location,
                ScheduledDate = collection.ScheduledDate,
                Status = collection.Status,
                EstimatedQuantity = collection.EstimatedQuantity,
                ActualQuantity = collection.ActualQuantity,
                CreatedAt = collection.CreatedAt,
                CompletedAt = collection.CompletedAt,
                Notes = collection.Notes
            };
        }
        
        public async Task<ScheduledCollectionViewModel?> UpdateAsync(int id, UpdateScheduledCollectionViewModel model)
        {
            var collection = await _context.ScheduledCollections
                .Include(sc => sc.Residue)
                .Include(sc => sc.CollectionPoint)
                .FirstOrDefaultAsync(sc => sc.Id == id);
                
            if (collection == null) return null;
            
            // Não permitir atualização de coletas completas
            if (collection.Status == "Completed")
                throw new InvalidOperationException("Não é possível atualizar uma coleta já concluída");
                
            if (model.ScheduledDate.HasValue) collection.ScheduledDate = model.ScheduledDate.Value;
            if (model.Status != null) collection.Status = model.Status;
            if (model.EstimatedQuantity.HasValue) collection.EstimatedQuantity = model.EstimatedQuantity.Value;
            if (model.ActualQuantity.HasValue) collection.ActualQuantity = model.ActualQuantity.Value;
            if (model.Notes != null) collection.Notes = model.Notes;
            
            // Se o status está sendo alterado para "Completed", atualizar a data de conclusão
            if (model.Status == "Completed" && collection.Status != "Completed")
            {
                collection.CompletedAt = DateTime.Now;
                
                // Atualizar quantidade do resíduo e status de alerta
                await UpdateResidueAfterCollection(collection);
                
                // Criar notificação para coleta concluída
                await _notificationService.CreateAsync(new CreateNotificationViewModel
                {
                    Title = "Coleta Concluída",
                    Message = $"A coleta do resíduo {collection.Residue.Name} foi concluída com sucesso.",
                    NotificationType = "CompletedCollection",
                    ResidueId = collection.ResidueId,
                    CollectionPointId = collection.CollectionPointId
                });
            }
            
            await _context.SaveChangesAsync();
            
            return new ScheduledCollectionViewModel
            {
                Id = collection.Id,
                ResidueId = collection.ResidueId,
                ResidueName = collection.Residue.Name,
                CollectionPointId = collection.CollectionPointId,
                CollectionPointName = collection.CollectionPoint.Name,
                CollectionPointLocation = collection.CollectionPoint.Location,
                ScheduledDate = collection.ScheduledDate,
                Status = collection.Status,
                EstimatedQuantity = collection.EstimatedQuantity,
                ActualQuantity = collection.ActualQuantity,
                CreatedAt = collection.CreatedAt,
                CompletedAt = collection.CompletedAt,
                Notes = collection.Notes
            };
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var collection = await _context.ScheduledCollections.FindAsync(id);
            if (collection == null) return false;
            
            // Não permitir exclusão de coletas completas
            if (collection.Status == "Completed")
                throw new InvalidOperationException("Não é possível excluir uma coleta já concluída");
                
            _context.ScheduledCollections.Remove(collection);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<ScheduledCollectionViewModel?> CompleteCollectionAsync(int id, CompleteCollectionViewModel model)
        {
            var collection = await _context.ScheduledCollections
                .Include(sc => sc.Residue)
                .Include(sc => sc.CollectionPoint)
                .FirstOrDefaultAsync(sc => sc.Id == id);
                
            if (collection == null) return null;
            
            if (collection.Status == "Completed")
                throw new InvalidOperationException("A coleta já está concluída");
                
            if (collection.Status == "Cancelled")
                throw new InvalidOperationException("Não é possível concluir uma coleta cancelada");
                
            collection.Status = "Completed";
            collection.ActualQuantity = model.ActualQuantity;
            collection.CompletedAt = DateTime.Now;
            if (model.Notes != null)
                collection.Notes = model.Notes;
            
            // Atualizar quantidade do resíduo e status de alerta
            await UpdateResidueAfterCollection(collection);
            
            await _context.SaveChangesAsync();
            
            // Criar notificação para coleta concluída
            await _notificationService.CreateAsync(new CreateNotificationViewModel
            {
                Title = "Coleta Concluída",
                Message = $"A coleta do resíduo {collection.Residue.Name} foi concluída com sucesso.",
                NotificationType = "CompletedCollection",
                ResidueId = collection.ResidueId,
                CollectionPointId = collection.CollectionPointId
            });
            
            return new ScheduledCollectionViewModel
            {
                Id = collection.Id,
                ResidueId = collection.ResidueId,
                ResidueName = collection.Residue.Name,
                CollectionPointId = collection.CollectionPointId,
                CollectionPointName = collection.CollectionPoint.Name,
                CollectionPointLocation = collection.CollectionPoint.Location,
                ScheduledDate = collection.ScheduledDate,
                Status = collection.Status,
                EstimatedQuantity = collection.EstimatedQuantity,
                ActualQuantity = collection.ActualQuantity,
                CreatedAt = collection.CreatedAt,
                CompletedAt = collection.CompletedAt,
                Notes = collection.Notes
            };
        }
        
        private async Task UpdateResidueAfterCollection(ScheduledCollection collection)
        {
            // Atualizar quantidade do resíduo e status de alerta
            var residue = collection.Residue;
            residue.CurrentQuantity -= collection.ActualQuantity ?? collection.EstimatedQuantity;
            if (residue.CurrentQuantity < 0) residue.CurrentQuantity = 0;
            residue.LastCollectionDate = DateTime.Now;
            residue.AlertActive = residue.CurrentQuantity >= residue.AlertThreshold;
            
            await _context.SaveChangesAsync();
        }
    }
}
