using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.VehicleDTO;

namespace BusinessLayer
{
    public class Vehicle
    {
        public enum enMode { Add, Update };

        public enMode mode = enMode.Add;
        public short? VehicleID { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string PlateNumbers { get; set; }
        public enVehicleType VehicleType { get; set; }
        public string AssociatedHospital { get; set; }
        public string AssociatedTask { get; set; }
        public enVehicleStatus Status { get; set; }
        public int TotalKilometersMoved { get; set; }
        public enFuelType FuelType { get; set; }
        public byte FuelConsumptionRate { get; set; }
        public byte OilConsumptionRate { get; set; }

        public VehicleDTO VehicleDTO => new VehicleDTO(
            VehicleID,
            BrandName,
            ModelName,
            PlateNumbers,
            VehicleType,
            AssociatedHospital,
            AssociatedTask, Status,
            TotalKilometersMoved,
            FuelType,
            FuelConsumptionRate,
            OilConsumptionRate
            );

        public Vehicle(VehicleDTO vehicleDTO, enMode mode = enMode.Add)
        {
            this.VehicleID = vehicleDTO.VehicleID;
            this.BrandName = vehicleDTO.BrandName;
            this.ModelName = vehicleDTO.ModelName;
            this.PlateNumbers = vehicleDTO.PlateNumbers;
            this.VehicleType = vehicleDTO.VehicleType;
            this.AssociatedHospital = vehicleDTO.AssociatedHospital;
            this.AssociatedTask = vehicleDTO.AssociatedTask;
            this.Status = vehicleDTO.Status;
            this.TotalKilometersMoved = vehicleDTO.TotalKilometersMoved;
            this.FuelType = vehicleDTO.FuelType;
            this.FuelConsumptionRate = vehicleDTO.FuelConsumptionRate;
            this.OilConsumptionRate = vehicleDTO.OilConsumptionRate;
            this.mode = mode;
        }

        private async Task<bool> _AddNewAsync()
        {
            if (string.IsNullOrWhiteSpace(BrandName) || string.IsNullOrWhiteSpace(ModelName) || string.IsNullOrWhiteSpace(PlateNumbers) || string.IsNullOrWhiteSpace(AssociatedHospital) || string.IsNullOrWhiteSpace(AssociatedTask))
                return false;

            if (PlateNumbers.Length != 7)
                return false;

            this.VehicleID = await VehicleRepo.AddNewVehicleAsync(VehicleDTO);

            return this.VehicleID.HasValue;
        }

        private async Task<bool> _UpdateAsync()
        {
            if (string.IsNullOrWhiteSpace(BrandName) || string.IsNullOrWhiteSpace(ModelName) || string.IsNullOrWhiteSpace(PlateNumbers) || string.IsNullOrWhiteSpace(AssociatedHospital) || string.IsNullOrWhiteSpace(AssociatedTask))
                return false;

            if (PlateNumbers.Length != 7)
                return false;

            return await VehicleRepo.UpdateVehicleAsync(VehicleDTO);
        }

        public static async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            List<VehicleDTO> vehicleDTOs = await VehicleRepo.GetAllVehiclesAsync();

            List<Vehicle> vehicles = new List<Vehicle>();

            foreach (VehicleDTO vehicleDTO in vehicleDTOs)
            {
                vehicles.Add(new Vehicle(vehicleDTO, enMode.Update));
            }

            return vehicles;
        }

        public static async Task<List<Vehicle>> GetAllVehiclesOfTypeAsync(enVehicleType vehicleType)
        {
            List<VehicleDTO> vehicleDTOs = await VehicleRepo.GetVehiclesByVehicleTypeAsync(vehicleType);

            List<Vehicle> vehicles = new List<Vehicle>();

            foreach (VehicleDTO vehicleDTO in vehicleDTOs)
            {
                vehicles.Add(new Vehicle(vehicleDTO, enMode.Update));
            }

            return vehicles;
        }

        public static async Task<List<Vehicle>> GetAllVehiclesWithStatusAsync(enVehicleStatus status)
        {
            List<VehicleDTO> vehicleDTOs = await VehicleRepo.GetVehiclesByStatusAsync(status);

            List<Vehicle> vehicles = new List<Vehicle>();

            foreach (VehicleDTO vehicleDTO in vehicleDTOs)
            {
                vehicles.Add(new Vehicle(vehicleDTO, enMode.Update));
            }

            return vehicles;
        }

        public static async Task<List<Vehicle>> GetAllVehiclesUsingFuelTypeAsync(enFuelType fuelType)
        {
            List<VehicleDTO> vehicleDTOs = await VehicleRepo.GetVehiclesByFuelTypeAsync(fuelType);

            List<Vehicle> vehicles = new List<Vehicle>();

            foreach (VehicleDTO vehicleDTO in vehicleDTOs)
            {
                vehicles.Add(new Vehicle(vehicleDTO, enMode.Update));
            }

            return vehicles;
        }

        public static async Task<Vehicle?> GetVehicleByIDAsync(short vehicleID)
        {
            VehicleDTO vehicleDTO = await VehicleRepo.GetVehicleByIDAsync(vehicleID);

            return vehicleDTO != null ? new Vehicle(vehicleDTO, enMode.Update) : null;
        }

        public static async Task<Vehicle?> GetVehicleByPlateNumbersAsync(string plateNumbers)
        {
            VehicleDTO vehicleDTO = await VehicleRepo.GetVehicleByPlateNumbersAsync(plateNumbers);

            return vehicleDTO != null ? new Vehicle(vehicleDTO, enMode.Update) : null;
        }

        public static async Task<short> GetNumberOfVehiclesAsync()
        {
            return await VehicleRepo.GetNumbersVehiclesAsync();
        }

        public static async Task<short> GetNumberOfVehiclesWithStatusAsync(enVehicleStatus status)
        {
            return await VehicleRepo.GetNumbersVehiclesByStatusAsync(status);
        }

        public virtual async Task<bool> SaveAsync()
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

        public async Task<bool> DeleteVehicleAsync()
        {
            return await VehicleRepo.DeleteVehicleAsync(this.PlateNumbers);
        }
    }
}
