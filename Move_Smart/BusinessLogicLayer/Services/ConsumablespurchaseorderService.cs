using BusinessLayer.Services;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Move_Smart.Hubs;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ConsumablespurchaseorderService
    {
        private readonly appDBContext _appDBContext;
        private readonly ApplicationService _applicationService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserRepo _userRepo;
        public ConsumablespurchaseorderService(ApplicationService applicationService,appDBContext appDBContext, IHubContext<NotificationHub> hubContext, UserRepo userRepo)
        {
            _appDBContext = appDBContext;
            _applicationService = applicationService;
            _hubContext = hubContext;
            _userRepo = userRepo;
        }
        public async Task<List<Consumablespurchaseorder>> GetAllConsumablesPurchaseoOrder()
        {
            return await _appDBContext.Consumablespurchaseorders
                    .AsNoTracking()
                    .Include(sp => sp.Application)
                    .ToListAsync();
        }
        public async Task<Consumablespurchaseorder> GetConsumablePurchaseOrderByID(int ID)
        {
            var order=  await _appDBContext.Consumablespurchaseorders
                .AsNoTracking()
                .Include(o => o.Application)
                .FirstOrDefaultAsync(o => o.OrderId == ID);
            if(order == null)
            {
                throw new ArgumentNullException("Order is not found!");
            }
            return order;
        }
        public async Task DeleteConsumablePurchaseOrderAsync(int orderId)
        {
            var order = await _appDBContext.Consumablespurchaseorders
                .Include(o => o.Application)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);


            if (order == null)
            {
                throw new InvalidOperationException(" order not found!");
            }
            // Remove the order itselff
            _appDBContext.Consumablespurchaseorders.Remove(order);

            // Commit the changes
            await _appDBContext.SaveChangesAsync();
            // Remove the Application if it exists
            if (order.Application != null)
            {
                await _applicationService.DeleteApplicationAsync(order.Application.ApplicationId);
            }
            
            await _appDBContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("NewOrderCreated", $"A new Consumable Purchase Order has been created with ID: {order.OrderId}");
        }
            public async Task AddConsumablePurchaseOrderAsync(Consumablespurchaseorder order)
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

                // Now link the ApplicationId to the order
                //order.ApplicationId = application.ApplicationId;

                // Ensure the order doesn’t already exist
                var existingOrder = await _appDBContext.Consumablespurchaseorders
                    .FirstOrDefaultAsync(x => x.OrderId == order.OrderId);

                if (existingOrder != null)
                {
                    throw new InvalidOperationException("Order already exists.");
                }

                // the problem of navi ....
                order.Application = null;

                _appDBContext.Consumablespurchaseorders.Add(order);
                await _appDBContext.SaveChangesAsync();

                await SendNotificationBasedOnRole(order.Application.CreatedByUserID, order.OrderId);

            }
            public async Task DeleteConsumablePurchaseOrder(int ID)
            {
                var order = await _appDBContext.Consumablespurchaseorders
                    .Include(o => o.Application)
                    .FirstOrDefaultAsync(o => o.OrderId == ID);


                if (order == null)
                {
                    throw new InvalidOperationException(" Order not found.");
                }
                // Remove the order itself
                _appDBContext.Consumablespurchaseorders.Remove(order);

                // Commit the changes
                await _appDBContext.SaveChangesAsync();
                // Remove the Application if it exists
                if (order.Application != null)
                {
                    await _applicationService.DeleteApplicationAsync(order.Application.ApplicationId);
                }
                await _appDBContext.SaveChangesAsync();

            }
        public async Task UpdateConsumablePurchaseOrderAsync(Consumablespurchaseorder updatedOrder)
        {
            var existingOrder = await _appDBContext.Consumablespurchaseorders
                .Include(o => o.Application)
                .FirstOrDefaultAsync(o => o.OrderId == updatedOrder.OrderId);

            if (existingOrder == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            // now the Orderss
            existingOrder.RequiredItem = updatedOrder.RequiredItem;
            existingOrder.RequiredQuantity = updatedOrder.RequiredQuantity;
            existingOrder.ApprovedByGeneralSupervisor = updatedOrder.ApprovedByGeneralSupervisor;
            existingOrder.ApprovedByGeneralManager = updatedOrder.ApprovedByGeneralManager;

            // app from hamdy
            if (existingOrder.Application != null && updatedOrder.Application != null)
            {
                // t3ala hena ya fallah
                await _applicationService.UpdateApplicationAsync(updatedOrder.Application);
                
            }

            await _appDBContext.SaveChangesAsync();
        }
        public async Task<int> CountAllConsumablePurchaseOrdersAsync()
        {
            return await _appDBContext.Consumablespurchaseorders.CountAsync();
        }
        private async Task SendNotificationBasedOnRole(int userId, int orderId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null) return;

            var message = $"Order #{orderId} notification.";

            var target = user.Role switch
            {
                EnUserRole.GeneralSupervisor => "GeneralSupervisorNotification",
                EnUserRole.HospitalManager => "HospitalManagerNotification",
                _ => "GeneralNotification"
            };

            await _hubContext.Clients.User(userId.ToString())
                .SendAsync(target, message);
        }

    }
}
