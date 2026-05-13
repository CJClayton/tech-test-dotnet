using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Tests.Fakes;

public class FakeAccountRepository : IAccountRepository
{
    public Account Account { get; set; }

    public bool UpdateAccountCalled { get; private set; }

    public Account GetAccount(string accountNumber)
    {
        return Account;
    }

    public void UpdateAccount(Account account)
    {
        UpdateAccountCalled = true;
        Account = account;
    }
}
