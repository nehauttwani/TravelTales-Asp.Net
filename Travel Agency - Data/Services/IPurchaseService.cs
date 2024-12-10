using System.Threading.Tasks;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.Services
{
    public interface IPurchaseService
    {
        Task<Package> GetPackageAsync(int packageId);
        Task SavePurchaseAsync(Purchase purchase);
    }
}
