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
    public class MissionsJobOrderService
    {
        private readonly MissionsJobOrderRepo _repo;
        private readonly ILogger<MissionsJobOrderService> _logger;

        public MissionsJobOrderService(MissionsJobOrderRepo repo, ILogger<MissionsJobOrderService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "Data access layer cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }
    }
}
