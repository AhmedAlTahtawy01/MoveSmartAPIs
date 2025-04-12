using BusinessLayer.Services;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class MissionsVehicleService
    {
        private readonly MissionsVehicleRepo _repo;
        private readonly ILogger<MissionsVehicleService> _logger;

        public MissionsVehicleService(MissionsVehicleRepo repo, ILogger<MissionsVehicleService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "Data access layer cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }
    }
}
