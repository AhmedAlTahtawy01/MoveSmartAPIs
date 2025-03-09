using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Vacation
    {
        public enum enMode { Add, Update };
        public enMode mode = enMode.Add;
        public int? VacationID { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int VacationOwnerID { get; set; }
        public int SubstituteDriverID { get; set; }

        public VacationDTO VacationDTO => new VacationDTO(
            VacationID,
            StartDate,
            EndDate,
            VacationOwnerID,
            SubstituteDriverID
            );

        public Vacation(VacationDTO vacationDTO, enMode mode = enMode.Add)
        {
            this.VacationID = vacationDTO.VacationID;
            this.StartDate = vacationDTO.StartDate;
            this.EndDate = vacationDTO.EndDate;
            this.VacationOwnerID = vacationDTO.VacationOwnerID;
            this.SubstituteDriverID = vacationDTO.SubstituteDriverID;
            this.mode = mode;
        }

        private async Task<bool> _AddNewAsync()
        {
            if (this.StartDate.CompareTo(this.EndDate) > 0)
                return false;

            if (this.StartDate.CompareTo(DateTime.Now.Date) < 0)
                return false;

            this.VacationID = await VacationRepo.AddNewVacationAsync(VacationDTO);

            return this.VacationID.HasValue;
        }

        private async Task<bool> _UpdateAsync()
        {
            if (this.StartDate.CompareTo(this.EndDate) > 0)
                return false;

            if (this.StartDate.CompareTo(DateTime.Now.Date) < 0)
                return false;

            return await VacationRepo.UpdateVacationAsync(VacationDTO);
        }

        public static async Task<List<Vacation>> GetAllVacationsAsync()
        {
            List<VacationDTO> vacationDTOs = await VacationRepo.GetAllVacationsAsync();

            List<Vacation> vacations = new List<Vacation>();

            foreach(VacationDTO vacationDTO in vacationDTOs)
            {
                vacations.Add(new Vacation(vacationDTO, enMode.Update));
            }

            return vacations;
        }

        public static async Task<List<Vacation>> GetAllVacationsForDriverAsync(int driverID)
        {
            List<VacationDTO> vacationDTOs = await VacationRepo.GetAllVacationsForDriverAsync(driverID);

            List<Vacation> vacations = new List<Vacation>();

            foreach (VacationDTO vacationDTO in vacationDTOs)
            {
                vacations.Add(new Vacation(vacationDTO, enMode.Update));
            }

            return vacations;
        }

        public static async Task<List<Vacation>> GetAllFutureVacationsForDriverAsync(int driverID)
        {
            List<VacationDTO> vacationDTOs = await VacationRepo.GetAllFutureVacationsForDriverAsync(driverID);

            List<Vacation> vacations = new List<Vacation>();

            foreach (VacationDTO vacationDTO in vacationDTOs)
            {
                vacations.Add(new Vacation(vacationDTO, enMode.Update));
            }

            return vacations;
        }

        public static async Task<Vacation?> GetVacationByIDAsync(int vacationID)
        {
            VacationDTO vacationDTO = await VacationRepo.GetVacationByIDAsync(vacationID);

            return vacationDTO != null ? new Vacation(vacationDTO, enMode.Update) : null;
        }

        public static async Task<bool> IsDriverInVacationAsync(int driverID)
        {
            return await VacationRepo.IsDriverinVacationAsync(driverID);
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
            return this.VacationID.HasValue ? await VacationRepo.DeleteVacationAsync(this.VacationID.Value) : false;
        }
    }
}
