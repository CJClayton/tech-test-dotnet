using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountRepository _accountRepository;

        public PaymentService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public PaymentService()
            : this(new AccountRepository())
        {
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var account = _accountRepository.GetAccount(request.DebtorAccountNumber);

            var result = new MakePaymentResult
            {
                Success = IsPaymentAllowed(request, account)
            };

            if (!result.Success)
            {
                return result;
            }

            account.Balance -= request.Amount;

            _accountRepository.UpdateAccount(account);

            return result;
        }

        private static bool IsPaymentAllowed(MakePaymentRequest request, Account account)
        {
            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    return CanProcessBacs(account);

                case PaymentScheme.FasterPayments:
                    return CanProcessFasterPayments(account, request);

                case PaymentScheme.Chaps:
                    return CanProcessChaps(account);

                default:
                    return true;
            }
        }

        private static bool CanProcessBacs(Account account)
        {
            return account != null &&
                   account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);
        }

        private static bool CanProcessFasterPayments(Account account, MakePaymentRequest request)
        {
            return account != null &&
                   account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments) &&
                   account.Balance >= request.Amount;
        }

        private static bool CanProcessChaps(Account account)
        {
            return account != null &&
                   account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) &&
                   account.Status == AccountStatus.Live;
        }
    }
}
