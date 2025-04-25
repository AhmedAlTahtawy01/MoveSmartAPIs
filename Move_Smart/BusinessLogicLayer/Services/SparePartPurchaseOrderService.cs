using BusinessLayer.Services;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class SparePartPurchaseOrderService
    {
        private readonly appDBContext _appDbContext;
        private readonly ApplicationService _applicationService;
        public SparePartPurchaseOrderService(appDBContext appDBContext, ApplicationService application )
        {
            _appDbContext = appDBContext;
            _applicationService = application;
        }
        public async Task AddSparePartsPurchaseOrder(Sparepartspurchaseorder order)
        {
            if (order == null || order.Application == null)
            {
                throw new ArgumentNullException("Order or Application data is missing.");
            }

            // Create Application via ApplicationService
            //int appId = await _applicationService.CreateApplicationAsync(order.Application);
            //order.ApplicationId = appId;

            // Check if order already exists
            var existing = await _appDbContext.Sparepartspurchaseorders
                .FirstOrDefaultAsync(x => x.OrderId == order.OrderId);

            if (existing != null)
            {
                throw new InvalidOperationException("Spare part order already exists.");
            }

            order.Application.Status = enStatus.Pending;

            _appDbContext.Sparepartspurchaseorders.Add(order);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task<List<Sparepartspurchaseorder>> GetAllSparePartPurchaseOrder()
        {
            return await _appDbContext.Sparepartspurchaseorders
                    .AsNoTracking()
                    .Include(sp => sp.Application)
                    //.Include(sp => sp.RequiredItemNavigation)
                    .ToListAsync();


        }
        public async Task DeleteSparePartsPurchaseOrderAsync(int orderId)
        {
            var order = await _appDbContext.Sparepartspurchaseorders
                .Include(o => o.Application)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);


            if (order == null)
            {
                throw new InvalidOperationException("Spare part order not found.");
            }
            // Remove the order itself
            _appDbContext.Sparepartspurchaseorders.Remove(order);

            // Commit the changes
            await _appDbContext.SaveChangesAsync();
            // Remove the Application if it exists
            if (order.Application != null)
            {
                await _applicationService.DeleteApplicationAsync(order.Application.ApplicationId);
            }

           
        }
        public async Task UpdateSparePartsPurchaseOrderAsync(int orderId, Sparepartspurchaseorder updatedOrder)
        {
            var existingOrder = await _appDbContext.Sparepartspurchaseorders
                .Include(o => o.Application)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (existingOrder == null)
                throw new InvalidOperationException("Order not found.");

            // Update SparePartPurchaseOrder fields
            existingOrder.ApprovedByGeneralSupervisor = updatedOrder.ApprovedByGeneralSupervisor;
            existingOrder.ApprovedByGeneralManager = updatedOrder.ApprovedByGeneralManager;
            existingOrder.RequiredItem = updatedOrder.RequiredItem;
            existingOrder.RequiredQuantity = updatedOrder.RequiredQuantity;

            // Update Application fields via service
            if (existingOrder.Application != null && updatedOrder.Application != null)
            {
                updatedOrder.Application.ApplicationId = existingOrder.Application.ApplicationId; // Ensure the ID is set
                await _applicationService.UpdateApplicationAsync(updatedOrder.Application);
            }

            await _appDbContext.SaveChangesAsync();
        }

        public async Task<Sparepartspurchaseorder?> GetSparePartPurchaseOrderByID(int orderId)
        {
            return await _appDbContext.Sparepartspurchaseorders
                .AsNoTracking()
                .Include(o => o.Application)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }



    }
}
