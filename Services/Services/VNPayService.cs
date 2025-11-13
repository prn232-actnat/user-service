using DataAccess.UnitOfWork;
using DTOs.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Models;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Services.Services
{
    public class VNPayService : IVNPayService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public VNPayService(IConfiguration config, IUnitOfWork unitOfWork)
        {
            _config = config;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> BuyPackageAsync(Guid userId, decimal amount, string ipAddress)
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                UserId = userId,
                Amount = amount,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Orders.AddAsync(order);

            string txnRef = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                OrderId = order.OrderId,
                TxnRef = txnRef,
                Amount = amount,
                TransactionStatus = "PENDING",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Transactions.AddAsync(transaction);

            await _unitOfWork.SaveChangesAsync();

            var vnpUrl = _config["VNPay:vnp_Url"];
            var returnUrl = _config["VNPay:vnp_ReturnUrl"];
            var tmnCode = _config["VNPay:vnp_TmnCode"];
            var hashSecret = _config["VNPay:vnp_HashSecret"];

            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var nowVN = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

            string createDate = nowVN.ToString("yyyyMMddHHmmss");
            string expireDate = nowVN.AddMinutes(15).ToString("yyyyMMddHHmmss");

            string orderInfo = $"Thanh toan don hang {order.OrderId}"
                                .Replace("đ", "d")
                                .Replace("Đ", "D");

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", tmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_TxnRef", txnRef); 
            vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnpay.AddRequestData("vnp_IpAddr", ipAddress);
            vnpay.AddRequestData("vnp_CreateDate", createDate);
            vnpay.AddRequestData("vnp_ExpireDate", expireDate);

            string paymentUrl = vnpay.CreateRequestUrl(vnpUrl, hashSecret);

            return paymentUrl;
        }

        public async Task<PaymentReturnDto> ProcessReturnAsync(IQueryCollection query)
        {

            var hashSecret = _config["VNPay:vnp_HashSecret"];
            if (string.IsNullOrEmpty(hashSecret))
            {
                throw new MyCustomException("Lỗi cấu hình thanh toán.");
            }

            var vnp_SecureHash = query["vnp_SecureHash"].ToString();

            bool isValid = vnp_SecureHash.Equals(vnp_SecureHash, StringComparison.OrdinalIgnoreCase);

            if (!isValid)
            {
                return new PaymentReturnDto
                {
                    IsSuccess = false,
                    Message = "Chữ ký không hợp lệ."
                };
            }

            string responseCode = query["vnp_ResponseCode"];
            string transactionStatus = query["vnp_TransactionStatus"];
            string txnRef = query["vnp_TxnRef"];
            string vnpayTranId = query["vnp_TransactionNo"]; 
            string amountRaw = query["vnp_Amount"];

            try
            {
                var transaction = await _unitOfWork.Transactions.GetAsync(x => x.TxnRef == txnRef);
                if (transaction == null)
                {
                    throw new MyCustomException("Không tìm thấy giao dịch.");
                }

                var order = await _unitOfWork.Orders.GetByIdAsync(transaction.OrderId);
                if (order == null)
                {
                    throw new MyCustomException("Không tìm thấy đơn hàng.");
                }

                decimal amount = decimal.Parse(amountRaw) / 100;

                transaction.Amount = amount;
                transaction.ResponseCode = responseCode;
                transaction.TransactionStatus = (responseCode == "00" && transactionStatus == "00") ? "SUCCESS" : "FAILED";
                transaction.BankCode = query["vnp_BankCode"];
                transaction.CardType = query["vnp_CardType"];
                transaction.SecureHash = vnp_SecureHash;
                transaction.RawData = string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));
                transaction.PayDate = DateTime.UtcNow; 

                if (responseCode == "00" && transactionStatus == "00")
                {
                    order.Status = "PAID";

                    var user = await _unitOfWork.Users.GetByIdAsync(order.UserId);
                    if (user != null)
                    {
                        user.Premium = true;
                        user.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Users.Update(user);
                    }
                }
                else
                {
                    order.Status = "FAILED";
                }

                order.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Transactions.Update(transaction);
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();

                return new PaymentReturnDto
                {
                    IsSuccess = (responseCode == "00" && transactionStatus == "00"),
                    Message = (responseCode == "00" && transactionStatus == "00")
                        ? "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ."
                        : $"Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: {responseCode}",
                };
            }
            catch (Exception ex)
            {
                throw new MyCustomException("Có lỗi xảy ra trong quá trình xử lý.");
            }
        }
    }
}