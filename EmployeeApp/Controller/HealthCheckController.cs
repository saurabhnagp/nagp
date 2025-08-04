using EmployeeApp.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApp.Controller
{
    public class HealthCheckController : ControllerBase
    {
        private readonly IDataAccessProvider _dataAccessProvider;

        public HealthCheckController(IDataAccessProvider dataAccessProvider)
        {
            _dataAccessProvider = dataAccessProvider;
        }

        [Route("~/health")]
        [HttpGet]
        public IActionResult Health()
        {
            return Ok("Healthy");
        }

        [Route("~/ready")]
        [HttpGet]
        public IActionResult Ready()
        {
            try
            {
                // Test database connection
                var employees = _dataAccessProvider.GetEmployeeRecords();
                return Ok("Ready");
            }
            catch
            {
                return StatusCode(503, "Database Not Ready");
            }
        }
    }
}
