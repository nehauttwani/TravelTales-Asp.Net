using System.Threading.Tasks;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.ViewModels;
namespace Travel_Agency___Data.Services
{
    public interface IWalletService
    {
        Task<decimal> GetWalletBalanceAsync(int customerId);
        Task<bool> DeductFundsAsync(int customerId, decimal amount);
        Task<bool> AddFundsAsync(int customerId, decimal amount);
    }
}
