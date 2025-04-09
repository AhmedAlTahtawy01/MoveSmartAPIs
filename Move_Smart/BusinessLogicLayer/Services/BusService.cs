//using DataAccessLayer;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BusinessLayer
//{
//    public class Bus : Vehicle
//    {
//        public new enum enMode { Add, Update };
//        public new enMode mode = enMode.Add;
//        public byte? BusID { get; set; }
//        public byte Capacity { get; set; }
//        public byte AvailableSpace { get; set; }

//        public BusDTO BusDTO => new BusDTO(BusID, Capacity, AvailableSpace, VehicleID ?? 0);

//        public Bus(BusDTO busDTO, VehicleDTO vehicleDTO, enMode mode = enMode.Add)
//            : base(vehicleDTO, Vehicle.enMode.Update)
//        {
//            this.BusID = busDTO.BusID;
//            this.Capacity = busDTO.Capacity;
//            this.AvailableSpace = busDTO.AvailableSpace;
//            this.mode = mode;
//        }

//        private async Task<bool> _AddNewAsync()
//        {
//            this.BusID = await BusRepo.AddNewBusAsync(BusDTO);

//            return this.BusID.HasValue;
//        }

//        private async Task<bool> _UpdateAsync()
//        {
//            return await BusRepo.UpdateBusAsync(BusDTO);
//        }

//        public static async Task<List<Bus>> GetAllBusesAsync()
//        {
//            List<BusDTO> busDTOs = await BusRepo.GetAllBusesAsync();

//            List<Bus> buses = new List<Bus>();

//            foreach(BusDTO busDTO in busDTOs)
//            {
//                buses.Add(new Bus(busDTO, await VehicleRepo.GetVehicleByIDAsync(busDTO.VehicleID), enMode.Update));
//            }

//            return buses;
//        }

//        public static async Task<List<Bus>> GetBusesByCapacityAsync(byte capacity)
//        {
//            List<BusDTO> busDTOs = await BusRepo.GetBusesByCapacityAsync(capacity);

//            List<Bus> buses = new List<Bus>();

//            foreach (BusDTO busDTO in busDTOs)
//            {
//                buses.Add(new Bus(busDTO, await VehicleRepo.GetVehicleByIDAsync(busDTO.VehicleID), enMode.Update));
//            }

//            return buses;
//        }

//        public static async Task<List<Bus>> GetBusesByAvailableSpaceAsync(byte availableSpace)
//        {
//            List<BusDTO> busDTOs = await BusRepo.GetBusesByAvailableSpaceAsync(availableSpace);

//            List<Bus> buses = new List<Bus>();

//            foreach (BusDTO busDTO in busDTOs)
//            {
//                buses.Add(new Bus(busDTO, await VehicleRepo.GetVehicleByIDAsync(busDTO.VehicleID), enMode.Update));
//            }

//            return buses;
//        }

//        public static async Task<Bus?> GetBusByIDAsync(byte id)
//        {
//            BusDTO busDTO = await BusRepo.GetBusByIDAsync(id);
//            VehicleDTO vehicleDTO = await VehicleRepo.GetVehicleByIDAsync(busDTO.VehicleID);

//            return vehicleDTO != null && busDTO != null ? new Bus(busDTO, vehicleDTO, enMode.Update) : null;
//        }

//        public override async Task<bool> SaveAsync()
//        {
//            if(!await base.SaveAsync())
//                return false;

//            switch(mode)
//            {
//                case enMode.Add:
//                    if (await _AddNewAsync())
//                    {
//                        mode = enMode.Update;
//                        return true;
//                    }
//                    else
//                        return false;

//                case enMode.Update:
//                    return await _UpdateAsync();
//            }

//            return false;
//        }

//        public async Task<bool> DeleteAsync()
//        {
//            return this.BusID.HasValue ? await BusRepo.DeleteBusAsync(this.BusID.Value) : false;
//        }
//    }
//}
