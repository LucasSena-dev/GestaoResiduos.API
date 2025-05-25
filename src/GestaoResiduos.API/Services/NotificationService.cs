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
    public interface INotificationService
    {
        Task<PaginatedResponse<NotificationViewModel>> GetAllAsync(int page, int pageSize);
        Task<NotificationViewModel?> GetByIdAsync(int id);
        Task<NotificationViewModel> CreateAsync(CreateNotificationViewModel model);
        Task<NotificationViewModel?> UpdateAsync(int id, UpdateNotificationViewModel model);
        Task<bool> MarkAsReadAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<int> GetUnreadCountAsync();
    }
    
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        
        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<PaginatedResponse<NotificationViewModel>> GetAllAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;
            
            var totalItems = await _context.Notifications.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var notifications = await _context.Notifications
                .Include(n => n.Residue)
                .Include(n => n.CollectionPoint)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    NotificationType = n.NotificationType,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    ResidueId = n.ResidueId,
                    ResidueName = n.Residue != null ? n.Residue.Name : null,
                    CollectionPointId = n.CollectionPointId,
                    CollectionPointName = n.CollectionPoint != null ? n.CollectionPoint.Name : null
                })
                .ToListAsync();
                
            return new PaginatedResponse<NotificationViewModel>
            {
                Items = notifications,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
        
        public async Task<NotificationViewModel?> GetByIdAsync(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.Residue)
                .Include(n => n.CollectionPoint)
                .FirstOrDefaultAsync(n => n.Id == id);
                
            if (notification == null) return null;
            
            return new NotificationViewModel
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                NotificationType = notification.NotificationType,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ResidueId = notification.ResidueId,
                ResidueName = notification.Residue?.Name,
                CollectionPointId = notification.CollectionPointId,
                CollectionPointName = notification.CollectionPoint?.Name
            };
        }
        
        public async Task<NotificationViewModel> CreateAsync(CreateNotificationViewModel model)
        {
            var notification = new Notification
            {
                Title = model.Title,
                Message = model.Message,
                NotificationType = model.NotificationType,
                IsRead = false,
                CreatedAt = DateTime.Now,
                ResidueId = model.ResidueId,
                CollectionPointId = model.CollectionPointId
            };
            
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            
            // Carregando relacionamentos para o ViewModel
            if (notification.ResidueId.HasValue)
                await _context.Entry(notification).Reference(n => n.Residue).LoadAsync();
                
            if (notification.CollectionPointId.HasValue)
                await _context.Entry(notification).Reference(n => n.CollectionPoint).LoadAsync();
            
            return new NotificationViewModel
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                NotificationType = notification.NotificationType,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ResidueId = notification.ResidueId,
                ResidueName = notification.Residue?.Name,
                CollectionPointId = notification.CollectionPointId,
                CollectionPointName = notification.CollectionPoint?.Name
            };
        }
        
        public async Task<NotificationViewModel?> UpdateAsync(int id, UpdateNotificationViewModel model)
        {
            var notification = await _context.Notifications
                .Include(n => n.Residue)
                .Include(n => n.CollectionPoint)
                .FirstOrDefaultAsync(n => n.Id == id);
                
            if (notification == null) return null;
            
            if (model.IsRead.HasValue) notification.IsRead = model.IsRead.Value;
            
            await _context.SaveChangesAsync();
            
            return new NotificationViewModel
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                NotificationType = notification.NotificationType,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ResidueId = notification.ResidueId,
                ResidueName = notification.Residue?.Name,
                CollectionPointId = notification.CollectionPointId,
                CollectionPointName = notification.CollectionPoint?.Name
            };
        }
        
        public async Task<bool> MarkAsReadAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return false;
            
            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return false;
            
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<int> GetUnreadCountAsync()
        {
            return await _context.Notifications.CountAsync(n => !n.IsRead);
        }
    }
}
