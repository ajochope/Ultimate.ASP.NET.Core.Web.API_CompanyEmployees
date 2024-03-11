using Entities.Models;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        // Async Code
        Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges);
        Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);
        Task CreateEmployeeForCompanyAsync(Guid companyId, Employee employee);
        Task DeleteEmployeeAsync(Employee employee);

        // Sync Code
        IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges);
        Employee GetEmployee(Guid companyId, Guid id, bool trackChanges);
        void CreateEmployeeForCompany(Guid companyId, Employee employee);

        void DeleteEmployee(Employee employee);
    }
}
