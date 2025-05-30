﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{

    public partial class Consumablespurchaseorder
    {
        public int OrderId { get; set; }

        public int ApplicationId { get; set; }

        public int RequiredItem { get; set; }

        public short RequiredQuantity { get; set; }

        public ulong ApprovedByGeneralSupervisor { get; set; }

        public ulong ApprovedByGeneralManager { get; set; }
        public virtual ApplicationDTO Application{ get; set; }

        public virtual Vehicleconsumable RequiredItemNavigation { get; set; }
        public Consumablespurchaseorder() { }
    }
    //public partial class consumablespurchaseorderRepo
    //{
    //    private readonly appDBContext _appDbContext;
    //    private readonly ApplicationRepo _iapplication;

    //    public consumablespurchaseorderRepo(appDBContext appDbContext, ApplicationRepo iapplication)
    //    {
    //        _appDbContext = appDbContext;
    //        _iapplication = iapplication;
    //    }
    //    public async Task AddConsumablesPurchaseOrder(Consumablespurchaseorder order)
    //    {
    //        var check = await _appDbContext.Consumablespurchaseorders.FirstOrDefaultAsync(x => x.OrderId == order.OrderId);

    //        if (check != null)
    //        {
    //            throw new InvalidOperationException(" Cannot be null");
    //        }
    //        if (order.RequiredQuantity == 0 && order.RequiredItem == 0)
    //        {
    //            throw new InvalidOperationException(" Cannot be null");
    //        }
    //        _appDbContext.Consumablespurchaseorders.Add(order);
    //        await _appDbContext.SaveChangesAsync();


    //    }

    //    public async Task ApproveRequestAsync(int OrderID)
    //    {
    //        //var app = await GetConsumablesPurchaseOrderByID(OrderID);
    //        //if (app == null)
    //        //{
    //        //    throw new KeyNotFoundException($"Application Request with ID {OrderID} not found.");
    //        //}

    //        //if (app.Status != "Pending")
    //        //{
    //        //    throw new InvalidOperationException("Only pending applications can be approved.");
    //        //}

    //        //app.Status = "Approved";
    //        //await _iapplication.UpdateApplication(app);
    //        throw new NotImplementedException();
    //    }

    //    //public async Task CancelRequestAsync(int OrderID)
    //    //{
    //    //    var request = await GetConsumablesPurchaseOrderByID(OrderID);
    //    //    if (request == null)
    //    //    {
    //    //        throw new KeyNotFoundException($"Checkout Request with ID {OrderID} not found.");
    //    //    }

    //    //    if (request.Application.Status != "Pending")
    //    //    {
    //    //        throw new InvalidOperationException("Only pending requests can be rejected.");
    //    //    }
    //    //    request.Application.Status = "Cancelled";
    //    //    await _iapplication.UpdateApplication(request.Application);

    //    //    // Update vehicle is available

    //    //    //var vehicle = await _appDBContext.GetVehicleByIdAsync(request.VehicleId);
    //    //    //if (vehicle == null)
    //    //    //{
    //    //    //    throw new KeyNotFoundException($"Vehicle with ID {request.id of the vehicle} not found.");
    //    //    //}
    //    //    //vehicle.IsAvailable = false;
    //    //    //await _vehicleRepository.UpdateVehicleAsync(vehicle);


    //    //    //Update the avaibleilty of the driver

    //    //}

    //    public async Task CompleteRequestAsync(int OrderID, DateTime endDate)
    //    {
    //        var request = await GetConsumablesPurchaseOrderByID(OrderID);
    //        if (request == null)
    //        {
    //            throw new KeyNotFoundException($"Checkout Request with ID {OrderID} not found.");
    //        }

    //        //if (request.Application.Status != "Approved")
    //        //{
    //        //    throw new InvalidOperationException("Only approved requests can be completed.");
    //        //}

    //        //request.Application.Status = "Completed";
    //        await UpdateConsumablesPurchaseOrder(request);
    //        //request.= endDate;

    //        //await _appDbContext.UpdateCheckoutRequestAsync(request);

    //        //var vehicle = await _appDbContext.GetVehicleByIdAsync(request.VehicleId);
    //        //if (vehicle == null)
    //        //{
    //        //    throw new KeyNotFoundException($"Vehicle with ID {request.VehicleId} not found.");
    //        //}

    //        //vehicle.IsAvailable = true;
    //        //vehicle.Mileage = endMileage;

    //        //await _vehicleRepository.UpdateVehicleAsync(vehicle);
    //    }

    //    public async Task DeleteConsumablesPurchaseOrder(int OrderID)
    //    {
    //        var order = await _appDbContext.Consumablespurchaseorders.AsNoTracking().FirstOrDefaultAsync(id => id.OrderId == OrderID);
    //        if (order == null)
    //        {
    //            throw new InvalidOperationException(" Cannot be null");
    //        }
    //        _appDbContext.Consumablespurchaseorders.Remove(order);
    //        await _appDbContext.SaveChangesAsync();

    //    }

    //    public async Task<List<Consumablespurchaseorder>> GetAllConsumablesPurchaseOrder()
    //    {
    //        return await _appDbContext.Consumablespurchaseorders.AsNoTracking().ToListAsync();
    //    }

    //    public async Task<Consumablespurchaseorder> GetConsumablesPurchaseOrderByID(int OrderID)
    //    {
    //        return await _appDbContext.Consumablespurchaseorders.Include(c => c.ApplicationId).AsNoTracking().FirstAsync(id => OrderID == id.OrderId);
    //    }

    //    //public async Task RejectRequestAsync(int OrderID)
    //    //{
    //    //    var request = await GetConsumablesPurchaseOrderByID(OrderID);
    //    //    if (request == null)
    //    //    {
    //    //        throw new KeyNotFoundException($"Checkout Request with ID {OrderID} not found.");
    //    //    }

    //    //    if (request.Application.Status != "Pending")
    //    //    {
    //    //        throw new InvalidOperationException($"Only pending requests can be rejected.current status: {request.Application.Status} and the id => {OrderID} ");

    //    //    }
    //    //    request.Application.Status = "Rejected";

    //    //    await _iapplication.UpdateApplication(request.Application);

    //    //}

    //    //public async Task UpdateStatusAsync(int OrderID, string status)
    //    //{
    //    //    var request = await GetConsumablesPurchaseOrderByID(OrderID);
    //    //    if (request == null)
    //    //    {
    //    //        throw new KeyNotFoundException($"Checkout Request with ID {OrderID} not found.");
    //    //    }

    //    //    if (request.Application.Status != "Pending")
    //    //    {
    //    //        throw new InvalidOperationException($"Only pending requests can be rejected.current status: {request.Application.Status} and the id => {OrderID} ");

    //    //    }
    //    //    request.Application.Status = status;

    //    //    await _iapplication.UpdateApplication(request.Application);

    //    //}

    //    public async Task UpdateConsumablesPurchaseOrder(Consumablespurchaseorder order)
    //    {
    //        _appDbContext.Consumablespurchaseorders.Update(order);
    //        await _appDbContext.SaveChangesAsync();
    //    }
    //    //public async Task<List<Consumablespurchaseorder>> GetConsumablesPurchaseOrderByStatus(string Status)
    //    //{
    //    //    return await _appDbContext.Consumablespurchaseorders
    //    //        .Include(c => c.ApplicationId)
    //    //        .Where(c => c.Application.Status == Status)
    //    //        .AsNoTracking()
    //    //        .ToListAsync();
    //    //}

    //    //public async Task<List<Consumablespurchaseorder>> GetConsumablesPurchaseOrderByUser(int UserId)
    //    //{

    //    //    return await _appDbContext.Consumablespurchaseorders
    //    //        .Include(c => c.ApplicationId)
    //    //        .Where(c => c.Application.CreatedByUserId == UserId)
    //    //        .AsNoTracking()
    //    //        .ToListAsync();
    //    //}

    //}

}