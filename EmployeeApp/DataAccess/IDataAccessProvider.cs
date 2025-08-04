using EmployeeApp.Model;

namespace EmployeeApp.DataAccess
{
    public interface IDataAccessProvider
    {
        void AddEmployeeRecord(Employee employee);
        List<Employee> GetEmployeeRecords();
    }
}
