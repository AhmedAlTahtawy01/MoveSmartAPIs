//using DataAccessLayer;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BusinessLayer
//{
//    public class Patrol
//    {
//        public enum enMode { Add, Update };
//        public enMode mode = enMode.Add;
//        public short? PatrolID { get; set; }
//        public string Description { get; set; }
//        public TimeOnly MovingAt { get; set; }
//        public short ApproximatedTime { get; set; }
//        public byte BusID { get; set; }

//        public PatrolDTO PatrolDTO => new PatrolDTO(
//            PatrolID,
//            Description,
//            MovingAt,
//            ApproximatedTime,
//            BusID
//            );

//        public Patrol(PatrolDTO patrolDTO, enMode mode = enMode.Add)
//        {
//            this.PatrolID = patrolDTO.PatrolID;
//            this.Description = patrolDTO.Description;
//            this.MovingAt = patrolDTO.MovingAt;
//            this.ApproximatedTime = patrolDTO .ApproximatedTime;
//            this.BusID = patrolDTO.BusID;
//            this.mode = mode;
//        }

//        private async Task<bool> _AddNewAsync()
//        {
//            if (string.IsNullOrWhiteSpace(Description))
//                return false;

//            this.PatrolID = await PatrolRepo.AddNewPatrolAsync(PatrolDTO);

//            return this.PatrolID.HasValue;
//        }

//        private async Task<bool> _UpdateAsync()
//        {
//            if (string.IsNullOrWhiteSpace(Description))
//                return false;

//            return await PatrolRepo.UpdatePatrolAsync(PatrolDTO);
//        }

//        public static async Task<List<Patrol>> GetAllPatrolsAsync()
//        {
//            List<PatrolDTO> patrolDTOs = await PatrolRepo.GetAllPatrolsAsync();

//            List<Patrol> patrols = new List<Patrol>();

//            foreach(PatrolDTO patrolDTO in patrolDTOs)
//            {
//                patrols.Add(new Patrol(patrolDTO, enMode.Update));
//            }

//            return patrols;
//        }

//        public static async Task<Patrol?> GetPatrolByIDAsync(short patrolID)
//        {
//            PatrolDTO patrolDTO = await PatrolRepo.GetPatrolByIDAsync(patrolID);

//            return patrolDTO != null ? new Patrol(patrolDTO, enMode.Update) : null;
//        }

//        public async Task<bool> SaveAsync()
//        {
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
//            return this.PatrolID.HasValue ? await PatrolRepo.DeletePatrolAsync(this.PatrolID.Value) : false;
//        }
//    }
//}
