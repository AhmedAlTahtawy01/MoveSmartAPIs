using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class ApplicationType
    {

        private readonly ApplicationTypeRepo _dal;

        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public enum enMode
        {
            AddNew = 0,
            Update = 1
        };
        public enMode Mode { get; private set; } = enMode.AddNew;

        public ApplicationTypeDTO AppTypeDTO => new ApplicationTypeDTO(this.TypeId, this.TypeName);


        public ApplicationType(ApplicationTypeDTO appTypeDTO, enMode mode = enMode.AddNew)
        {
            TypeId = appTypeDTO.TypeId;
            TypeName = appTypeDTO.TypeName;
            _dal = new ApplicationTypeRepo();

            Mode = mode;
        }

        private async Task<bool> _CreateApplicationTypeAsync()
        {
            if (await _dal.ApplicationTypeExistsAsync(TypeName))
                throw new Exception($"Application type with ID {TypeName} already exists.");

            TypeId = await _dal.CreateApplicationTypeAsync(AppTypeDTO.TypeName);
            if (TypeId > 0)
            {
                Mode = enMode.Update;
                return true;
            }

            return false;
        }
    
        private async Task<bool> _UpdateApplicationTypeAsync()
        {
            if (!await _dal.ApplicationTypeExistsAsync(TypeName))
                throw new Exception($"Application type with ID {TypeName} does not exist.");

            return await _dal.UpdateApplicationTypeAsync(AppTypeDTO);
        }
    
        public async Task<List<ApplicationTypeDTO>> GetAllApplicationTypesAsync()
        {
            return await _dal.GetAllApplicationTypesAsync();
        }

        public async Task<ApplicationType> GetApplicationTypeByIdAsync(int typeId)
        {
            var appTypeDTO = await _dal.GetApplicationTypeByIdAsync(typeId);
            return appTypeDTO != null ? new ApplicationType(appTypeDTO, enMode.Update) : null;
        }
    
        public async Task<bool> Save()
        {
            return Mode switch
            { enMode.AddNew => await _CreateApplicationTypeAsync(),
                enMode.Update => await _UpdateApplicationTypeAsync(),
                _ => throw new Exception("Invalid mode.")
            };
        }

        public async Task<bool> DeleteApplicationType()
        {
            if (!await _dal.ApplicationTypeExistsAsync(TypeName))
                throw new Exception($"Application type with ID {TypeName} does not exist.");

            return await _dal.DeleteApplicationTypeAsync(AppTypeDTO);
        }
    }
}
