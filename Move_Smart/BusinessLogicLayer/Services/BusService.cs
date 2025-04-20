using DataAccessLayer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class BusService : VehicleService
    {
        protected readonly BusRepo _busRepo;
        protected readonly ILogger<BusService> _busLogger;

        public BusService(BusRepo busRepo, ILogger<BusService> busLogger, VehicleRepo vehicleRepo, ILogger<VehicleService> vehicleLogger)
            : base(vehicleRepo, vehicleLogger)
        {
            _busRepo = busRepo ?? throw new ArgumentNullException(nameof(busRepo), "Data access layer cannot be null.");
            _busLogger = busLogger ?? throw new ArgumentNullException(nameof(busLogger), "Logger cannot be null.");
        }

        private async void _ValidateBusDTO(BusDTO dto)
        {
            if (dto == null)
            {
                _busLogger.LogError("BusDTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "BusDTO cannot be null.");
            }

            if(dto.AvailableSpace > dto.Capacity)
            {
                _busLogger.LogError("AvailableSpace Can't Exceed Bus Capacity be null.");
                throw new ArgumentException(nameof(dto), "AvailableSpace Can't Exceed Bus Capacity be null.");
            }

            if(!await _vehicleRepo.IsVehicleExistsAsync(dto.VehicleID))
            {
                _busLogger.LogError($"Vehicle No[{dto.VehicleID}] Doesn't Existes.");
                throw new ArgumentException(nameof(dto), $"Vehicle No[{dto.VehicleID}] Doesn't Existes.");
            }
        }

        public async Task<byte?> AddNewBusAsync(BusDTO dto)
        {
            try
            {
                _ValidateBusDTO(dto);
            }
            catch (Exception ex)
            {
                _busLogger.LogError(ex, "Validation Failed For BusDTO");
            }

            if(await _busRepo.IsBusExistsAsync(dto.BusID ?? 0))
            {
                _busLogger.LogError($"Bus with PlateNumbers {dto.Vehicle.PlateNumbers} already exists.");
                return null;
            }

            dto.BusID = await _busRepo.AddNewBusAsync(dto);
            return dto.BusID;
        }

        public async Task<bool> UpdateBusAsync(BusDTO dto)
        {
            try
            {
                _ValidateBusDTO(dto);
            }
            catch (Exception ex)
            {
                _busLogger.LogError(ex, "Validation Failed For BusDTO");
                return false;
            }

            if(!await _busRepo.IsBusExistsAsync(dto.BusID ?? 0))
            {
                _busLogger.LogError($"Bus with PlateNumbers {dto.Vehicle.PlateNumbers} doesn't exist.");
                return false;
            }

            return await _busRepo.UpdateBusAsync(dto);
        }

        public async Task<List<BusDTO>> GetAllBusesAsync()
        {
            return await _busRepo.GetAllBusesAsync();
        }

        public async Task<List<BusDTO>> GetBusesByCapacityAsync(byte capacity)
        {
            return await _busRepo.GetBusesByCapacityAsync(capacity);
        }

        public async Task<List<BusDTO>> GetBusesByAvailableSpaceAsync(byte availableSpace)
        {
            return await _busRepo.GetBusesByAvailableSpaceAsync(availableSpace);
        }

        public async Task<BusDTO?> GetBusByIDAsync(byte id)
        {
            return await _busRepo.GetBusByIDAsync(id);
        }

        public async Task<BusDTO?> GetBusByPlateNumbersAsync(string plateNumbers)
        {
            return await _busRepo.GetBusByPlateNumbersAsync(plateNumbers);
        }

        public async Task<bool> IsBusExists(byte busID)
        {
            return await _busRepo.IsBusExistsAsync(busID);
        }

        public async Task<bool> IsBusExists(string plateNumbers)
        {
            return await _busRepo.IsBusExistsAsync(plateNumbers);
        }

        public async Task<bool> DeleteBusAsync(byte busID)
        {
            return await _busRepo.DeleteBusAsync(busID);
        }

        public async Task<bool> DeleteBusAsync(string plateNumbers)
        {
            return await _busRepo.DeleteBusAsync(plateNumbers);
        }
    }
}
