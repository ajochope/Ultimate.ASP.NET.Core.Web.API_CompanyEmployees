using Entities.Models;

namespace Contracts
{
    public interface ICompanyRepository
    {
        // Async Code
        Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges);
        Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges);
        Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
        void CreateCompany(Company company);

        // Sync Code
        IEnumerable<Company> GetAllCompanies(bool trackChanges);
        Company GetCompany(Guid companyId, bool trackChanges);
        IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges);

        //void CreateCompany(Company company);
        void DeleteCompany(Company company);
    }
}
