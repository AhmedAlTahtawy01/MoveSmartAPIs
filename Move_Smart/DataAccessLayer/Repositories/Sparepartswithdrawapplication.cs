﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public class Sparepartswithdrawapplication
    {
        public int WithdrawApplicationId { get; set; }

        public int ApplicationId { get; set; }

        public short SparePartId { get; set; }

        public short VehicleId { get; set; }

        public virtual ApplicationDTO Application { get; set; }
        public ulong ApprovedByGeneralSupervisor { get; set; }

        public ulong ApprovedByGeneralManager { get; set; }

        public virtual Sparepart SparePart { get; set; }
    }
}