using EmployeeApp.DataAccess;
using EmployeeApp.Model;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApp.Controller
{
    public class EmployeeController : ControllerBase
    {
        private readonly IDataAccessProvider _dataAccessProvider;

        public EmployeeController(IDataAccessProvider dataAccessProvider)
        {
            _dataAccessProvider = dataAccessProvider;
        }

        [Route("~/api/employees")]
        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            return _dataAccessProvider.GetEmployeeRecords();
        }

        [Route("~/api/addemployee")]
        [HttpPost]
        public IActionResult Create([FromBody] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _dataAccessProvider.AddEmployeeRecord(employee);
                return Ok();
            }
            return BadRequest();
        }
    }
}
