using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        //Async code
        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges) =>
                     await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                     .OrderBy(e => e.Name).ToListAsync();
        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
                     await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
                     .SingleOrDefaultAsync();


        // Sync Code
        public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges) =>
                     FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                     .OrderBy(e => e.Name).ToList();
        public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges) =>
                     FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
                     .SingleOrDefault();
        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }
        public void DeleteEmployee(Employee employee) => Delete(employee);
    }
}
