# Project Naming Rules and Task Assignments

## Naming Rules

- **Variables**: camelCase (e.g., `userName`, `vehicleCost`)
- **Functions**: PascalCase (e.g., `GetCarById()`, `GetUserData()`)
- **Classes**: PascalCase (e.g., `UserRepo`, `VehicleService`)
- **Classes Properties**: PascalCase (e.g, `UserId`, `AccessRight`)
- **Files in Data Access Layer**: PascalCaseRepo (e.g., `VehicleRepo.cs`, `UserRepo.cs`)
- **Files in Business Layer**: PascalCaseService (e.g., `VehicleService.cs`, `UserService.cs`)
- **Files in Presentation Layer**: PascalCaseAPIController (e.g., `VehicleAPIController.cs`, `UserAPIController.cs`)

## Task Assignments

- **Hamdy**:

  - **Level 1**: Users, Applications.
  - **Level 2**: JobOrder.
  - **Level 3**: Maintenance, Missions.
  - **Level 4**: MissionsJobOrders, MissionsVehicles.

  ##

- **Abd-Elrahman**:

  - **Level 1**: Vehicles.
  - **Level 2**: Buses, Drivers.
  - **Level 3**: Employees, Patrols, Vacations, MaintenanceApplications, MissionsNotes, PatrolsSubscriptions.

  ##

- **Kamal**:
  - **Level 1**: VehicleConsumables, SpareParts.
  - **Level 3**: ConsumablesPurchaseOrders, ConsumablesWithdrawApplications, SparePartsPurchaseOrders, SparePartsWithdrawApplications.
  - **Level 5**: ConsumablesReplacements, SparePartsReplacements.


## Adding ConnectionString and Jwt key to your environment variable
- **ConnectionString**: 
- Write this code in your cmd: setx ConnectionStrings__DefaultConnection "Your_Connection_String_Here"

- **Jwt Key**:
- Write this code in your cmd: setx JWT__Key "The_Key_Here"
- **If not worked:**
- **Write this commands:**
- 1- dotnet user-secrets init
- 2- dotnet user-secrets set "JWT:Key" "YourSuperSecretKey"



## General Guidelines

- Follow clean code practices.
- Write meaningful names.
- Write clear and descriptive commit messages.
- Don't merge your code with master unless it was reviewed.
