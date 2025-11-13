using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string TxnRef { get; set; }

        public decimal? Amount { get; set; }

        public string BankCode { get; set; }

        public string CardType { get; set; }

        public DateTime? PayDate { get; set; }

        public string ResponseCode { get; set; }

        public string TransactionStatus { get; set; }

        public string SecureHash { get; set; }

        public string RawData { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

}
