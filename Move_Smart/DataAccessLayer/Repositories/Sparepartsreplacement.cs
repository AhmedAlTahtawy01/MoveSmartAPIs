﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public partial class Sparepartsreplacement
    {
        public int ReplacementId { get; set; }

        public int MaintenanceId { get; set; }

        public short SparePartId { get; set; }

        public virtual Sparepart SparePart { get; set; }
        public Sparepartsreplacement() { }

    }
   public partial class SparePartsReplacement
    {
        private readonly appDBContext _appDBContext;
        public SparePartsReplacement(appDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }
        public async Task AddSparePartsReplacement(Sparepartsreplacement order)
        {
            var check = await _appDBContext.Sparepartsreplacements.FirstOrDefaultAsync(x => x.ReplacementId == order.ReplacementId);
            if (check != null)
            {
                throw new InvalidOperationException(" Cannot be null");
            }
            _appDBContext.Sparepartsreplacements.Add(order);
            await _appDBContext.SaveChangesAsync();

        }

        public async Task DeleteSparePartsReplacement(int ApplicationID)
        {
            var sparepart = await _appDBContext.Sparepartsreplacements.AsNoTracking().FirstOrDefaultAsync(id => id.ReplacementId == ApplicationID);
            if (sparepart == null)
            {
                throw new InvalidOperationException(" Cannot be null");
            }
            _appDBContext.Sparepartsreplacements.Remove(sparepart);
            await _appDBContext.SaveChangesAsync();
        }

        public async Task<List<Sparepartsreplacement>> GetAllSparePartsReplacement()
        {
            return await _appDBContext.Sparepartsreplacements.AsNoTracking().ToListAsync();
        }

        public async Task<Sparepartsreplacement> GetSparePartsReplacementByID(int WithdrawApplicationID)
        {
           var data =  await _appDBContext.Sparepartsreplacements.AsNoTracking().FirstAsync(id => WithdrawApplicationID == id.ReplacementId);
            if(data == null)
            {
                throw new InvalidOperationException("مفيش حد يالاسم دا");
            }
            return data;
        }

        public async Task UpdateSparePartsReplacement(Sparepartsreplacement order)
        {
            _appDBContext.Sparepartsreplacements.Update(order);
            await _appDBContext.SaveChangesAsync();
        }
        public async Task<int> CountAllOrdersAsync()
        {
            return await _appDBContext.Sparepartsreplacements.CountAsync();
        }

    }

}