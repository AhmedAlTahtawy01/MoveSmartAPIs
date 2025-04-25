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
    public class CosumableWithdawApplicationService
    {
        private readonly appDBContext _appDBContext;
        private readonly ApplicationService _applicationService;
        public CosumableWithdawApplicationService(appDBContext appDBContext,ApplicationService applicationService)
        {
            _appDBContext = appDBContext;
            _applicationService = applicationService;  
        }
        public async Task<List<Consumableswithdrawapplication>> GetAllConsumableWithdrawApplications()
        {
            return await _appDBContext.Consumableswithdrawapplications
                   .AsNoTracking()
                   .Include(sp => sp.Application)
                   //.Include(sp => sp.RequiredItemNavigation)
                   .ToListAsync();
        } 
        public async Task<Consumableswithdrawapplication>   GetConsumableWithdrawApplicationByID(int id)
        {
            var order = await _appDBContext.Consumableswithdrawapplications
               .AsNoTracking()
               .Include(o => o.Application)
               .FirstOrDefaultAsync(o => o.WithdrawApplicationId == id);
            if (order == null)
            {
                throw new ArgumentNullException("Order is not found!");
            }
            return order;
        }
        public async Task AddConsumableWithdrawApplicationAsync(Consumableswithdrawapplication order)
        {
            if (order == null || order.Application == null)
            {
                throw new ArgumentNullException("Order or Application data is not found.");
            }
            // add and save the Application
            var application = order.Application;
            application.Status = enStatus.Pending;

            int appId = await _applicationService.CreateApplicationAsync(application);
            order.ApplicationId = appId;

            
            var existingOrder = await _appDBContext.Consumableswithdrawapplications
                .FirstOrDefaultAsync(x => x.WithdrawApplicationId == order.WithdrawApplicationId);

            if (existingOrder != null)
            {
                throw new InvalidOperationException("Order already exists.");
            }

            // the problem of navi ....
            order.Application = null;

            _appDBContext.Consumableswithdrawapplications.Add(order);
            await _appDBContext.SaveChangesAsync();
        }
        public async Task DeleteWithdrawOrder(int ID)
        {
            var order = await _appDBContext.Consumableswithdrawapplications
                .Include(o => o.Application)
                .FirstOrDefaultAsync(o => o.WithdrawApplicationId == ID);
            if (order == null)
            {
                throw new InvalidOperationException(" Order not found.");
            }
            // remove the order itself
            _appDBContext.Consumableswithdrawapplications.Remove(order);

            await _appDBContext.SaveChangesAsync();
            if (order.Application != null)
            {
                await _applicationService.DeleteApplicationAsync(order.Application.ApplicationId);
            }
            await _appDBContext.SaveChangesAsync();

        }
        public async Task UpdateConsumableWithdrawAsync(Consumableswithdrawapplication updatedOrder)
        {
            var existingOrder = await _appDBContext.Consumableswithdrawapplications
                .Include(o => o.Application)
                .FirstOrDefaultAsync(o => o.WithdrawApplicationId == updatedOrder.WithdrawApplicationId);

            if (existingOrder == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            existingOrder.ApprovedByGeneralSupervisor = updatedOrder.ApprovedByGeneralSupervisor;
            existingOrder.ApprovedByGeneralManager = updatedOrder.ApprovedByGeneralManager;

            // app from hamdy
            if (existingOrder.Application != null && updatedOrder.Application != null)
            {
                await _applicationService.UpdateApplicationAsync(updatedOrder.Application);
            }

            await _appDBContext.SaveChangesAsync();
        }
        public async Task<int> CountAllOrdersAsync()
        {
            return await _appDBContext.Consumableswithdrawapplications.CountAsync();
        }
    }
}
