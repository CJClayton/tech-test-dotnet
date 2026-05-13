using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Tests.Fakes;
using ClearBank.DeveloperTest.Types;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services;

public class PaymentServiceTests
{
    [Fact]
    public void MakePayment_returns_success_for_valid_bacs_payment()
    {
        // Arrange
        var account = CreateAccount(
            AllowedPaymentSchemes.Bacs,
            balance: 100m);

        var repository = new FakeAccountRepository
        {
            Account = account
        };

        var service = new PaymentService(repository);

        var request = CreateRequest(PaymentScheme.Bacs, amount: 25m);

        // Act
        var result = service.MakePayment(request);

        //Assert
        Assert.True(result.Success);
    }

    [Fact]
    public void MakePayment_returns_failure_when_bacs_account_not_found()
    {
        // Arrange
        var repository = new FakeAccountRepository();

        var service = new PaymentService(repository);

        var request = CreateRequest(PaymentScheme.Bacs);

        // Act
        var result = service.MakePayment(request);

        //Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void MakePayment_returns_failure_when_bacs_not_supported()
    {
        // Arrange
        var account = CreateAccount(
            AllowedPaymentSchemes.FasterPayments,
            balance: 100m);

        var repository = new FakeAccountRepository
        {
            Account = account
        };

        var service = new PaymentService(repository);

        var request = CreateRequest(PaymentScheme.Bacs);

        // Act
        var result = service.MakePayment(request);

        //Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void MakePayment_returns_success_for_valid_faster_payment()
    {
        // Arrange
        var account = CreateAccount(
            AllowedPaymentSchemes.FasterPayments,
            balance: 100m);

        var repository = new FakeAccountRepository
        {
            Account = account
        };

        var service = new PaymentService(repository);

        var request = CreateRequest(PaymentScheme.FasterPayments, amount: 50m);

        // Act
        var result = service.MakePayment(request);

        //Assert
        Assert.True(result.Success);
    }

    [Fact]
    public void MakePayment_returns_failure_when_balance_insufficient()
    {
        // Arrange
        var account = CreateAccount(
            AllowedPaymentSchemes.FasterPayments,
            balance: 10m);

        var repository = new FakeAccountRepository
        {
            Account = account
        };

        var service = new PaymentService(repository);

        var request = CreateRequest(PaymentScheme.FasterPayments, amount: 50m);

        // Act
        var result = service.MakePayment(request);

        //Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void MakePayment_returns_failure_when_faster_payments_not_supported()
    {
        // Arrange
        var account = CreateAccount(
            AllowedPaymentSchemes.Bacs,
            balance: 100m);

        var repository = new FakeAccountRepository
        {
            Account = account
        };

        var service = new PaymentService(repository);

        var request = CreateRequest(PaymentScheme.FasterPayments, amount: 50m);

        // Act
        var result = service.MakePayment(request);

        //Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void MakePayment_returns_success_for_live_chaps_account()
    {
        // Arrange
        var account = CreateAccount(
            AllowedPaymentSchemes.Chaps,
            balance: 100m,
            status: AccountStatus.Live);

        var repository = new FakeAccountRepository
        {
            Account = account
        };

        var service = new PaymentService(repository);

        var request = CreateRequest(PaymentScheme.Chaps, amount: 50m);

        // Act
        var result = service.MakePayment(request);

        //Assert
        Assert.True(result.Success);
    }

    [Fact]
    public void MakePayment_returns_failure_for_non_live_chaps_account()
    {
        // Arrange
        var account = CreateAccount(
            AllowedPaymentSchemes.Chaps,
            balance: 100m,
            status: AccountStatus.Disabled);

        var repository = new FakeAccountRepository
        {
            Account = account
        };

        var service = new PaymentService(repository);

        var request = CreateRequest(PaymentScheme.Chaps, amount: 50m);

        // Act
        var result = service.MakePayment(request);

        //Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void MakePayment_returns_failure_when_chaps_not_supported()
    {
        // Arrange
        var account = CreateAccount(
            AllowedPaymentSchemes.Bacs,
            balance: 100m,
            status: AccountStatus.Live);

        var repository = new FakeAccountRepository
        {
            Account = account
        };

        var service = new PaymentService(repository);

        var request = CreateRequest(PaymentScheme.Chaps, amount: 50m);

        // Act
        var result = service.MakePayment(request);

        //Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void MakePayment_updates_account_when_payment_succeeds()
    {
        // Arrange
        var account = CreateAccount(
            AllowedPaymentSchemes.FasterPayments,
            balance: 100m);

        var repository = new FakeAccountRepository
        {
            Account = account
        };

        var service = new PaymentService(repository);

        var request = CreateRequest(PaymentScheme.FasterPayments, amount: 25m);

        // Act
        service.MakePayment(request);

        //Assert
        Assert.True(repository.UpdateAccountCalled);
        Assert.Equal(75m, repository.Account.Balance);
    }

    [Fact]
    public void MakePayment_does_not_update_account_when_payment_fails()
    {
        // Arrange
        var account = CreateAccount(
            AllowedPaymentSchemes.FasterPayments,
            balance: 10m);

        var repository = new FakeAccountRepository
        {
            Account = account
        };

        var service = new PaymentService(repository);

        var request = CreateRequest(PaymentScheme.FasterPayments, amount: 50m);

        // Act
        service.MakePayment(request);

        //Assert
        Assert.False(repository.UpdateAccountCalled);
        Assert.Equal(10m, repository.Account.Balance);
    }

    private static MakePaymentRequest CreateRequest(
            PaymentScheme paymentScheme,
            decimal amount = 10m)
    {
        return new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            Amount = amount,
            PaymentScheme = paymentScheme
        };
    }

    private static Account CreateAccount(
        AllowedPaymentSchemes allowedPaymentSchemes,
        decimal balance,
        AccountStatus status = AccountStatus.Live)
    {
        return new Account
        {
            AllowedPaymentSchemes = allowedPaymentSchemes,
            Balance = balance,
            Status = status
        };
    }
}
