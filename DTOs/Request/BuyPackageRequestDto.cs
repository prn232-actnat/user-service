using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request
{
    public class BuyPackageRequestDto
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
    }

}
