using DTOs.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IVNPayService
    {
        Task<string> BuyPackageAsync(Guid userId, decimal amount, string ipAddress);
        Task<PaymentReturnDto> ProcessReturnAsync(IQueryCollection query);
    }
}
