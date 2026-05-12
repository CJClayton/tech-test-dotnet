using ClearBank.DeveloperTest.Types;
using System.Configuration;

namespace ClearBank.DeveloperTest.Data;

public class AccountRepository : IAccountRepository
{
    private readonly string _dataStoreType;

    public AccountRepository()
    {
        _dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
    }

    public Account GetAccount(string accountNumber)
    {
        Account account;

        if (_dataStoreType == "Backup")
        {
            var accountDataStore = new BackupAccountDataStore();
            account = accountDataStore.GetAccount(accountNumber);
        }
        else
        {
            var accountDataStore = new AccountDataStore();
            account = accountDataStore.GetAccount(accountNumber);
        }

        return account;
    }

    public void UpdateAccount(Account account)
    {
        if (_dataStoreType == "Backup")
        {
            var accountDataStore = new BackupAccountDataStore();
            accountDataStore.UpdateAccount(account);
        }
        else
        {
            var accountDataStore = new AccountDataStore();
            accountDataStore.UpdateAccount(account);
        }
    }
}
