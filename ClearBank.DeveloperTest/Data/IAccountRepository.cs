using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data;

public interface IAccountRepository
{
    Account GetAccount(string accountNumber);
    void UpdateAccount(Account account);
}
