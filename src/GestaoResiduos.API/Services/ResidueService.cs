using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestaoResiduos.API.Data;
using GestaoResiduos.API.Models;
using GestaoResiduos.API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestaoResiduos.API.Services
{
    public interface IResidueService
    {
        Task<PaginatedResponse<ResidueViewModel>> GetAllAsync(int page, int pageSize);
        Task<ResidueViewModel?> GetByIdAsync(int id);
        Task<ResidueViewModel> CreateAsync(CreateResidueViewModel model);
        Task<ResidueViewModel?> UpdateAsync(int id, UpdateResidueViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<bool> CheckAndUpdateAlertStatusesAsync();
    }
    
    public class ResidueService : IResidueService
    {
        private readonly ApplicationDbContext _context;
        
        public ResidueService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<PaginatedResponse<ResidueViewModel>> GetAllAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;
            
            var totalItems = await _context.Residues.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var residues = await _context.Residues
                .OrderBy(r => r.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ResidueViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Category = r.Category,
                    CurrentQuantity = r.CurrentQuantity,
                    AlertThreshold = r.AlertThreshold,
                    AlertActive = r.AlertActive,
                    CreatedAt = r.CreatedAt,
                    LastCollectionDate = r.LastCollectionDate
                })
                .ToListAsync();
                
            return new PaginatedResponse<ResidueViewModel>
            {
                Items = residues,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
        
        public async Task<ResidueViewModel?> GetByIdAsync(int id)
        {
            var residue = await _context.Residues.FindAsync(id);
            if (residue == null) return null;
            
            return new ResidueViewModel
            {
                Id = residue.Id,
                Name = residue.Name,
                Description = residue.Description,
                Category = residue.Category,
                CurrentQuantity = residue.CurrentQuantity,
                AlertThreshold = residue.AlertThreshold,
                AlertActive = residue.AlertActive,
                CreatedAt = residue.CreatedAt,
                LastCollectionDate = residue.LastCollectionDate
            };
        }
        
        public async Task<ResidueViewModel> CreateAsync(CreateResidueViewModel model)
        {
            var residue = new Residue
            {
                Name = model.Name,
                Description = model.Description,
                Category = model.Category,
                CurrentQuantity = model.CurrentQuantity,
                AlertThreshold = model.AlertThreshold,
                AlertActive = model.CurrentQuantity >= model.AlertThreshold,
                CreatedAt = DateTime.Now
            };
            
            _context.Residues.Add(residue);
            await _context.SaveChangesAsync();
            
            // Se quantidade já está acima do limite, cria uma notificação
            if (residue.AlertActive)
            {
                await CreateAlertNotification(residue);
            }
            
            return new ResidueViewModel
            {
                Id = residue.Id,
                Name = residue.Name,
                Description = residue.Description,
                Category = residue.Category,
                CurrentQuantity = residue.CurrentQuantity,
                AlertThreshold = residue.AlertThreshold,
                AlertActive = residue.AlertActive,
                CreatedAt = residue.CreatedAt,
                LastCollectionDate = residue.LastCollectionDate
            };
        }
        
        public async Task<ResidueViewModel?> UpdateAsync(int id, UpdateResidueViewModel model)
        {
            var residue = await _context.Residues.FindAsync(id);
            if (residue == null) return null;
            
            bool previousAlertStatus = residue.AlertActive;
            
            if (model.Name != null) residue.Name = model.Name;
            if (model.Description != null) residue.Description = model.Description;
            if (model.Category != null) residue.Category = model.Category;
            if (model.CurrentQuantity.HasValue) residue.CurrentQuantity = model.CurrentQuantity.Value;
            if (model.AlertThreshold.HasValue) residue.AlertThreshold = model.AlertThreshold.Value;
            
            // Recalcula o status do alerta
            residue.AlertActive = residue.CurrentQuantity >= residue.AlertThreshold;
            
            await _context.SaveChangesAsync();
            
            // Se o status do alerta mudou para ativo, cria uma notificação
            if (!previousAlertStatus && residue.AlertActive)
            {
                await CreateAlertNotification(residue);
            }
            
            return new ResidueViewModel
            {
                Id = residue.Id,
                Name = residue.Name,
                Description = residue.Description,
                Category = residue.Category,
                CurrentQuantity = residue.CurrentQuantity,
                AlertThreshold = residue.AlertThreshold,
                AlertActive = residue.AlertActive,
                CreatedAt = residue.CreatedAt,
                LastCollectionDate = residue.LastCollectionDate
            };
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var residue = await _context.Residues.FindAsync(id);
            if (residue == null) return false;
            
            _context.Residues.Remove(residue);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> CheckAndUpdateAlertStatusesAsync()
        {
            var residuesToUpdate = await _context.Residues
                .Where(r => (r.CurrentQuantity >= r.AlertThreshold && !r.AlertActive) ||
                            (r.CurrentQuantity < r.AlertThreshold && r.AlertActive))
                .ToListAsync();
                
            if (!residuesToUpdate.Any()) return false;
            
            foreach (var residue in residuesToUpdate)
            {
                bool previousStatus = residue.AlertActive;
                residue.AlertActive = residue.CurrentQuantity >= residue.AlertThreshold;
                
                if (!previousStatus && residue.AlertActive)
                {
                    await CreateAlertNotification(residue);
                }
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        private async Task CreateAlertNotification(Residue residue)
        {
            var notification = new Notification
            {
                Title = "Alerta de Coleta de Resíduos",
                Message = $"O resíduo {residue.Name} atingiu a quantidade limite para coleta.",
                NotificationType = "CollectionAlert",
                IsRead = false,
                CreatedAt = DateTime.Now,
                ResidueId = residue.Id
            };
            
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
    }
}
