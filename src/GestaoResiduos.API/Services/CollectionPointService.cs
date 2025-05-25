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
    public interface ICollectionPointService
    {
        Task<PaginatedResponse<CollectionPointViewModel>> GetAllAsync(int page, int pageSize);
        Task<CollectionPointViewModel?> GetByIdAsync(int id);
        Task<CollectionPointViewModel> CreateAsync(CreateCollectionPointViewModel model);
        Task<CollectionPointViewModel?> UpdateAsync(int id, UpdateCollectionPointViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<List<CollectionPointViewModel>> FindNearbyPointsAsync(double latitude, double longitude, double radiusKm);
    }
    
    public class CollectionPointService : ICollectionPointService
    {
        private readonly ApplicationDbContext _context;
        
        public CollectionPointService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<PaginatedResponse<CollectionPointViewModel>> GetAllAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;
            
            var totalItems = await _context.CollectionPoints.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var points = await _context.CollectionPoints
                .OrderBy(cp => cp.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(cp => new CollectionPointViewModel
                {
                    Id = cp.Id,
                    Name = cp.Name,
                    Location = cp.Location,
                    Latitude = cp.Latitude,
                    Longitude = cp.Longitude,
                    ResponsiblePerson = cp.ResponsiblePerson,
                    Contact = cp.Contact,
                    IsActive = cp.IsActive,
                    AcceptedCategories = cp.AcceptedCategories,
                    CreatedAt = cp.CreatedAt
                })
                .ToListAsync();
                
            return new PaginatedResponse<CollectionPointViewModel>
            {
                Items = points,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
        
        public async Task<CollectionPointViewModel?> GetByIdAsync(int id)
        {
            var point = await _context.CollectionPoints.FindAsync(id);
            if (point == null) return null;
            
            return new CollectionPointViewModel
            {
                Id = point.Id,
                Name = point.Name,
                Location = point.Location,
                Latitude = point.Latitude,
                Longitude = point.Longitude,
                ResponsiblePerson = point.ResponsiblePerson,
                Contact = point.Contact,
                IsActive = point.IsActive,
                AcceptedCategories = point.AcceptedCategories,
                CreatedAt = point.CreatedAt
            };
        }
        
        public async Task<CollectionPointViewModel> CreateAsync(CreateCollectionPointViewModel model)
        {
            var point = new CollectionPoint
            {
                Name = model.Name,
                Location = model.Location,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                ResponsiblePerson = model.ResponsiblePerson,
                Contact = model.Contact,
                IsActive = true,
                AcceptedCategories = model.AcceptedCategories,
                CreatedAt = DateTime.Now
            };
            
            _context.CollectionPoints.Add(point);
            await _context.SaveChangesAsync();
            
            return new CollectionPointViewModel
            {
                Id = point.Id,
                Name = point.Name,
                Location = point.Location,
                Latitude = point.Latitude,
                Longitude = point.Longitude,
                ResponsiblePerson = point.ResponsiblePerson,
                Contact = point.Contact,
                IsActive = point.IsActive,
                AcceptedCategories = point.AcceptedCategories,
                CreatedAt = point.CreatedAt
            };
        }
        
        public async Task<CollectionPointViewModel?> UpdateAsync(int id, UpdateCollectionPointViewModel model)
        {
            var point = await _context.CollectionPoints.FindAsync(id);
            if (point == null) return null;
            
            if (model.Name != null) point.Name = model.Name;
            if (model.Location != null) point.Location = model.Location;
            if (model.Latitude.HasValue) point.Latitude = model.Latitude.Value;
            if (model.Longitude.HasValue) point.Longitude = model.Longitude.Value;
            if (model.ResponsiblePerson != null) point.ResponsiblePerson = model.ResponsiblePerson;
            if (model.Contact != null) point.Contact = model.Contact;
            if (model.IsActive.HasValue) point.IsActive = model.IsActive.Value;
            if (model.AcceptedCategories != null) point.AcceptedCategories = model.AcceptedCategories;
            
            await _context.SaveChangesAsync();
            
            return new CollectionPointViewModel
            {
                Id = point.Id,
                Name = point.Name,
                Location = point.Location,
                Latitude = point.Latitude,
                Longitude = point.Longitude,
                ResponsiblePerson = point.ResponsiblePerson,
                Contact = point.Contact,
                IsActive = point.IsActive,
                AcceptedCategories = point.AcceptedCategories,
                CreatedAt = point.CreatedAt
            };
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var point = await _context.CollectionPoints.FindAsync(id);
            if (point == null) return false;
            
            _context.CollectionPoints.Remove(point);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<List<CollectionPointViewModel>> FindNearbyPointsAsync(double latitude, double longitude, double radiusKm)
        {
            // Cálculo aproximado de distância (funciona para pequenas distâncias)
            // Uma implementação mais precisa usaria a fórmula de Haversine
            double latRad = latitude * (Math.PI / 180);
            
            // 1 grau de latitude ≈ 111 km
            double latDiff = radiusKm / 111.0;
            
            // Longitude varia com o cosseno da latitude
            double lonDiff = radiusKm / (111.0 * Math.Cos(latRad));
            
            var points = await _context.CollectionPoints
                .Where(cp => cp.IsActive &&
                           cp.Latitude >= latitude - latDiff &&
                           cp.Latitude <= latitude + latDiff &&
                           cp.Longitude >= longitude - lonDiff &&
                           cp.Longitude <= longitude + lonDiff)
                .Select(cp => new CollectionPointViewModel
                {
                    Id = cp.Id,
                    Name = cp.Name,
                    Location = cp.Location,
                    Latitude = cp.Latitude,
                    Longitude = cp.Longitude,
                    ResponsiblePerson = cp.ResponsiblePerson,
                    Contact = cp.Contact,
                    IsActive = cp.IsActive,
                    AcceptedCategories = cp.AcceptedCategories,
                    CreatedAt = cp.CreatedAt
                })
                .ToListAsync();
                
            return points;
        }
    }
}
