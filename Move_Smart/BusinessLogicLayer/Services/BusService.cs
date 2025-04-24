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

        protected async void _ValidateBusDTO(BusDTO dto)
        {
            if (dto == null)
            {
                _busLogger.LogError("BusDTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "BusDTO cannot be null.");
            }

            try
            {
                _ValidateVehicleDTO(dto.Vehicle);
            }
            catch (Exception ex)
            {
                _busLogger.LogError(ex, "Validation failed for VehicleDTO.");
                return;
            }

            if(dto.Capacity <= 0)
            {
                _busLogger.LogError("Capacity must be greater than 0.");
                throw new ArgumentException(nameof(dto), "Capacity must be greater than 0.");
            }

            if (dto.AvailableSpace > dto.Capacity)
            {
                _busLogger.LogError("AvailableSpace Can't Exceed Bus Capacity be null.");
                throw new ArgumentException(nameof(dto), "AvailableSpace Can't Exceed Bus Capacity be null.");
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
                return null;
            }

            if(await _busRepo.IsBusExistsAsync(dto.BusID ?? 0))
            {
                _busLogger.LogError($"Bus with PlateNumbers {dto.Vehicle.PlateNumbers} already exists.");
                return null;
            }

            if(!await IsVehicleExistsAsync(dto.VehicleID))
            {
                dto.VehicleID = await AddNewVehicleAsync(dto.Vehicle) ?? 0;

                if (dto.VehicleID == 0)
                {
                    _busLogger.LogError($"Failed to add vehicle for Bus with PlateNumbers {dto.Vehicle.PlateNumbers}.");
                    return null;
                }
            }

            dto.BusID = await _busRepo.AddNewBusAsync(dto);

            if (dto.BusID == null)
            {
                _busLogger.LogError($"Failed to add Bus with PlateNumbers {dto.Vehicle.PlateNumbers}.");
                
                if(await DeleteVehicleAsync(dto.VehicleID))
                {
                    _busLogger.LogError($"Faild to delete vehicle with ID [{dto.VehicleID}] after failing in adding new bus!");
                }
                
                return null;
            }

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
                _busLogger.LogError($"Bus with ID {dto.BusID} doesn't exist.");
                return false;
            }

            if (!await UpdateVehicleAsync(dto.Vehicle))
            {
                _busLogger.LogError($"Failed to update vehicle for Bus with PlateNumbers {dto.Vehicle.PlateNumbers}.");
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
            BusDTO? busDTO = await GetBusByIDAsync(busID);

            if (busDTO == null)
            {
                _busLogger.LogError($"Bus with ID {busID} doesn't exist.");
                return false;
            }

            if (!await _busRepo.DeleteBusAsync(busDTO.BusID ?? 0))
            {
                _busLogger.LogError($"Failed to delete Bus with ID [{busID}].");
                return false;
            }

            if(!await DeleteVehicleAsync(busDTO.VehicleID))
            {
                _busLogger.LogError($"Failed to delete vehicle for Bus... Vehicle ID [{busDTO.VehicleID}].");
                return false;
            }

            return await _busRepo.DeleteBusAsync(busID);
        }

        public async Task<bool> DeleteBusAsync(string plateNumbers)
        {
            BusDTO? busDTO = await GetBusByPlateNumbersAsync(plateNumbers);

            if (busDTO == null)
            {
                _busLogger.LogError($"Bus with plate numbers [{plateNumbers}] doesn't exist.");
                return false;
            }

            if (!await _busRepo.DeleteBusAsync(busDTO.Vehicle.PlateNumbers))
            {
                _busLogger.LogError($"Failed to delete Bus with plate numbers [{plateNumbers}].");
                return false;
            }

            if (!await DeleteVehicleAsync(plateNumbers))
            {
                _busLogger.LogError($"Failed to delete vehicle for Bus with plate numbers [{plateNumbers}].");
                return false;
            }

            return true;
        }
    }
}
