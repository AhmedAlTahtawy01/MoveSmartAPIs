//using DataAccessLayer;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static DataAccessLayer.EmployeeDTO;

//namespace BusinessLayer
//{
//    public class Employee
//    {
//        public enum enMode { Add, Update };
//        public enMode mode = enMode.Add;
//        public int? EmployeeID { get; set; }
//        public string NationalNo { get; set; }
//        public string Name { get; set; }
//        public string JobTitle { get; set; }
//        public string Phone { get; set; }
//        public enTransportationSubscriptionStatus TransportationSubscriptionStatus { get; set; }

//        public EmployeeDTO EmployeeDTO => new EmployeeDTO(
//            EmployeeID,
//            NationalNo,
//            Name,
//            JobTitle,
//            Phone,
//            TransportationSubscriptionStatus
//            );

//        public Employee(EmployeeDTO employeeDTO, enMode mode = enMode.Add)
//        {
//            this.EmployeeID = employeeDTO.EmployeeID;
//            this.NationalNo = employeeDTO.NationalNo;
//            this.Name = employeeDTO.Name;
//            this.JobTitle = employeeDTO.JobTitle;
//            this.Phone = employeeDTO.Phone;
//            this.TransportationSubscriptionStatus = employeeDTO.TransportationSubscriptionStatus;
//            this.mode = mode;
//        }

//        private async Task<bool> _AddNewAsync()
//        {
//            if(string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(NationalNo) || string.IsNullOrWhiteSpace(JobTitle) || string.IsNullOrWhiteSpace(Phone))
//                return false;

//            if (this.NationalNo.Length != 14)
//                return false;

//            if (this.Phone.Length != 11)
//                return false;

//            this.EmployeeID = await EmployeeRepo.AddNewEmployeeAsync(EmployeeDTO);

//            return this.EmployeeID.HasValue;
//        }

//        private async Task<bool> _UpdateAsync()
//        {
//            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(NationalNo) || string.IsNullOrWhiteSpace(JobTitle) || string.IsNullOrWhiteSpace(Phone))
//                return false;

//            if (this.NationalNo.Length != 14)
//                return false;

//            if (this.Phone.Length != 11)
//                return false;

//            return await EmployeeRepo.UpdateEmployeeAsync(EmployeeDTO);
//        }

//        public static async Task<List<Employee>> GetAllEmployeesAsync()
//        {
//            List<EmployeeDTO> employeeDTOs = await EmployeeRepo.GetAllEmployeesAsync();

//            List<Employee> employees = new List<Employee>();

//            foreach(EmployeeDTO employeeDTO in employeeDTOs)
//            {
//                employees.Add(new Employee(employeeDTO, enMode.Update));
//            }

//            return employees;
//        }

//        public static async Task<List<Employee>> GetAllEmployeesWhoAreUsingBusAsync(byte busID)
//        {
//            List<EmployeeDTO> employeeDTOs = await EmployeeRepo.GetAllEmployeesWhoAreUsingBusAsync(busID);

//            List<Employee> employees = new List<Employee>();

//            foreach (EmployeeDTO employeeDTO in employeeDTOs)
//            {
//                employees.Add(new Employee(employeeDTO, enMode.Update));
//            }

//            return employees;
//        }

//        public static async Task<Employee?> GetEmployeeByIDAsync(int employeeID)
//        {
//            EmployeeDTO employeeDTO = await EmployeeRepo.GetEmployeeByIDAsync(employeeID);

//            return employeeDTO != null ? new Employee(employeeDTO, enMode.Update) : null;
//        }

//        public static async Task<Employee?> GetEmployeeByNationalNoAsync(string nationalNo)
//        {
//            EmployeeDTO employeeDTO = await EmployeeRepo.GetEmployeeByNationalNoAsync(nationalNo);

//            return employeeDTO != null ? new Employee(employeeDTO, enMode.Update) : null;
//        }

//        public static async Task<Employee?> GetEmployeeByPhoneAsync(string phone)
//        {
//            EmployeeDTO employeeDTO = await EmployeeRepo.GetEmployeeByPhoneAsync(phone);

//            return employeeDTO != null ? new Employee(employeeDTO, enMode.Update) : null;
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
//            return this.EmployeeID.HasValue ? await EmployeeRepo.DeleteEmployeeAsync(this.EmployeeID.Value) : false;
//        }

//        public async Task<bool> IsTransportationSubscriptionValidAsync()
//        {
//            return this.EmployeeID.HasValue ? await EmployeeRepo.IsEmployeeTransportationSubscriptionValidAsync(this.EmployeeID.Value) : false;
//        }
//    }
//}
