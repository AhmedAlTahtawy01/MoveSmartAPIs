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
    public class SparePartWithdrawApplicationService
    {
        private readonly appDBContext _appDBContext;
        private readonly ApplicationService _applicationService;
        public SparePartWithdrawApplicationService(appDBContext appDBContext , ApplicationService application)
        {
            _appDBContext = appDBContext;
            _applicationService = application;
        }
        public async Task<Sparepartswithdrawapplication> GetSparepartswithdrawapplicationByID(int id)
        {

            var order = await _appDBContext.Sparepartswithdrawapplications
                .AsNoTracking()
                .Include(o => o.Application)
                .FirstOrDefaultAsync(o => o.WithdrawApplicationId == id);
            if (order == null)
            {
                throw new ArgumentNullException("Order is not found!!");
            }
            return order;
        }
        public async Task<List<Sparepartswithdrawapplication>> GetAllSparePartWithdrawApplication()
        {
            return await _appDBContext.Sparepartswithdrawapplications
                   .AsNoTracking()
                   .Include(sp => sp.Application)
                   .ToListAsync();
        }
        public async Task DeleteSparePartWithdrawAppliactoinOrderAsync(int orderId)
        {
            var order = await _appDBContext.Sparepartswithdrawapplications
                .Include(o => o.Application)
                .FirstOrDefaultAsync(o => o.WithdrawApplicationId== orderId);


            if (order == null)
            {
                throw new InvalidOperationException(" order not found!");
            }
            // Remove the order itselff
            _appDBContext.Sparepartswithdrawapplications.Remove(order);

            await _appDBContext.SaveChangesAsync();
            if (order.Application != null)
            {
                await _applicationService.DeleteApplicationAsync(order.Application.ApplicationId);
            }

            await _appDBContext.SaveChangesAsync();
        }
        public async Task AddConsumablePurchaseOrderAsync(Sparepartswithdrawapplication order)
        {
            if (order == null || order.Application == null)
            {
                throw new ArgumentNullException("Order or Application data is missing.");
            }
            // First, add and save the Application
            var application = order.Application;
            application.Status = enStatus.Pending;

            int appId = await _applicationService.CreateApplicationAsync(application);
            order.ApplicationId = appId;
            var existingOrder = await _appDBContext.Sparepartswithdrawapplications
                .FirstOrDefaultAsync(x => x.WithdrawApplicationId == order.WithdrawApplicationId);

            if (existingOrder != null)
            {
                throw new InvalidOperationException("Order already exists.");
            }

            // the problem of navi ....
            order.Application = null;

            _appDBContext.Sparepartswithdrawapplications.Add(order);
            await _appDBContext.SaveChangesAsync();
        }
        public async Task UpdateSparePartsWithdrawApplicationAsync(Sparepartswithdrawapplication updatedOrder)
        {
            var existingOrder = await _appDBContext.Sparepartswithdrawapplications
                .Include(o => o.Application)
                .FirstOrDefaultAsync(o => o.WithdrawApplicationId == updatedOrder.WithdrawApplicationId);

            if (existingOrder == null)
            {
                throw new InvalidOperationException("Order not found.");
            }
            existingOrder.ApprovedByGeneralSupervisor = updatedOrder.ApprovedByGeneralSupervisor;
            existingOrder.ApprovedByGeneralManager = updatedOrder.ApprovedByGeneralManager;
            
            if (existingOrder.Application != null && updatedOrder.Application != null)
            {
                await _applicationService.UpdateApplicationAsync(updatedOrder.Application);
            }
            await _appDBContext.SaveChangesAsync();
        }
        public async Task<int> CountAllOrdersAsync()
        {
            return await _appDBContext.Sparepartswithdrawapplications.CountAsync();
        }
    }
}
