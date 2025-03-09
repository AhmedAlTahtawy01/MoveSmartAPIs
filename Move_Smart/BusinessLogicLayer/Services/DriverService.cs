using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.DriverDTO;

namespace BusinessLayer
{
    public class Driver
    {
        public enum enMode { Add, Update };
        public enMode mode = enMode.Add;
        public int? DriverID { get; set; }
        public string NationalNo { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public enDriverStatus Status { get; set; }
        public short VehicleID { get; set; }

        public DriverDTO DriverDTO => new DriverDTO(
            DriverID,
            NationalNo,
            Name,
            Phone,
            Status,
            VehicleID
            );

        private Driver(DriverDTO driverDTO, enMode mode = enMode.Add)
        {
            this.DriverID = driverDTO.DriverID;
            this.NationalNo = driverDTO.NationalNo;
            this.Name = driverDTO.Name;
            this.Phone = driverDTO.Phone;
            this.Status = driverDTO.Status;
            this.VehicleID = driverDTO.VehicleID;
        }

        private async Task<bool> _AddNewAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(NationalNo) || string.IsNullOrWhiteSpace(Phone))
                return false;

            if (NationalNo.Length != 14)
                return false;

            if (Phone.Length != 11)
                return false;

            this.DriverID = await DriverRepo.AddNewDriverAsync(DriverDTO);

            return this.DriverID.HasValue;
        }

        private async Task<bool> _UpdateAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(NationalNo) || string.IsNullOrWhiteSpace(Phone))
                return false;

            if (NationalNo.Length != 14)
                return false;

            if (Phone.Length != 11)
                return false;

            return await DriverRepo.UpdateDriverAsync(DriverDTO);
        }

        public static async Task<List<Driver>> GetAllDriversAsync()
        {
            List<DriverDTO> driverDTOs = await DriverRepo.GetAllDriversAsync();

            List<Driver> drivers = new List<Driver>();

            foreach (DriverDTO driverDTO in driverDTOs)
            {
                drivers.Add(new Driver(driverDTO, enMode.Update));
            }

            return drivers;
        }

        public static async Task<List<Driver>> GetAllDriversWorkingOnVehicleAsync(short vehicleID)
        {
            List<DriverDTO> driverDTOs = await DriverRepo.GetDriversByVehicleIDAsync(vehicleID);

            List<Driver> drivers = new List<Driver>();

            foreach (DriverDTO driverDTO in driverDTOs)
            {
                drivers.Add(new Driver(driverDTO, enMode.Update));
            }

            return drivers;
        }

        public static async Task<List<Driver>> GetAllDriversWorkingOnVehicleAsync(string plateNumbers)
        {
            List<DriverDTO> driverDTOs = await DriverRepo.GetDriversByVehiclePlateNumbersAsync(plateNumbers);

            List<Driver> drivers = new List<Driver>();

            foreach (DriverDTO driverDTO in driverDTOs)
            {
                drivers.Add(new Driver(driverDTO, enMode.Update));
            }

            return drivers;
        }

        public static async Task<List<Driver>> GetAllDriversWithStatusAsync(enDriverStatus status)
        {
            List<DriverDTO> driverDTOs = await DriverRepo.GetDriversByStatusAsync(status);

            List<Driver> drivers = new List<Driver>();

            foreach (DriverDTO driverDTO in driverDTOs)
            {
                drivers.Add(new Driver(driverDTO, enMode.Update));
            }

            return drivers;
        }

        public static async Task<Driver?> GetDriverByIDAsync(int driverID)
        {
            DriverDTO driverDTO = await DriverRepo.GetDriverByIDAsync(driverID);

            return driverDTO != null ? new Driver(driverDTO, enMode.Update) : null;
        }

        public static async Task<Driver?> GetDriverByNationalNoAsync(string nationalNo)
        {
            DriverDTO driverDTO = await DriverRepo.GetDriverByNationalNoAsync(nationalNo);

            return driverDTO != null ? new Driver(driverDTO, enMode.Update) : null;
        }

        public static async Task<Driver?> GetDriverByPhoneAsync(string phone)
        {
            DriverDTO driverDTO = await DriverRepo.GetDriverByPhoneAsync(phone);

            return driverDTO != null ? new Driver(driverDTO, enMode.Update) : null;
        }

        public static async Task<short> GetNumberOfDriversAsync()
        {
            return await DriverRepo.GetNumberOfDriversAsync();
        }

        public static async Task<short> GetNumberOfDriversByStatusAsync(enDriverStatus status)
        {
            return await DriverRepo.GetNumberOfDriversByStatusAsync(status);
        }

        public async Task<bool> SaveAsync()
        {
            switch(mode)
            {
                case enMode.Add:
                    if (await _AddNewAsync())
                    {
                        mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return await _UpdateAsync();
            }

            return false;
        }

        public async Task<bool> DeleteAsync()
        {
            return await DriverRepo.DeleteDriverAsync(this.NationalNo);
        }
    }
}
