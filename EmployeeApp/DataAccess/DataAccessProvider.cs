using EmployeeApp.Model;

namespace EmployeeApp.DataAccess
{
    public class DataAccessProvider : IDataAccessProvider
    {
        private readonly PostgreSqlContext _context;

        public DataAccessProvider(PostgreSqlContext context)
        {
            _context = context;
        }

        public void AddEmployeeRecord(Employee employee)
        {
            _context.Employee.Add(employee);
            _context.SaveChanges();
        }

        public List<Employee> GetEmployeeRecords()
        {
            var record = _context.Employee.ToList();
            return record;
        }
    }
}
