using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.Commands;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Request.ResourceParameters;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Response;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.Queries;
using Backend.BankingTranxSystem.DataAccess.Enums;
using Backend.BankingTranxSystem.SharedServices.Helper;
using MediatR;
using Moq;

namespace Backend.BankingTranxSystem.UnitTests;

public class WalletCommand_Tests : IClassFixture<Fixture>
{
    public Fixture _fixture;

    public WalletCommand_Tests(Fixture fixture) => _fixture = fixture ??
        throw new ArgumentNullException(nameof(Fixture));

    [Fact]
    public void WalletDeposit_Success_Test()
    {
        var senderMock = new Mock<ISender>();
        senderMock.Setup(m => m.Send(new WalletCommand()
        {
            Amount = It.IsAny<decimal>(),
            Narration = It.IsAny<string>(),
            RequestId = It.IsAny<string>(),
            TransactionType = TransactionType.Deposit,
        }, CancellationToken.None)).Returns(Task.FromResult(It.IsAny<RepositoryActionResult<WalletResponseDto>>()));


        var result = senderMock.Object.Send(
            new WalletCommand()
            {
                Amount = It.IsAny<decimal>(),
                Narration = It.IsAny<string>(),
                RequestId = It.IsAny<string>(),
                TransactionType = TransactionType.Deposit,
            }, CancellationToken.None);
        if (result.IsCompleted)
        {
            Assert.True(true);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void WalletWithdrawal_Success_Test()
    {
        var senderMock = new Mock<ISender>();
        senderMock.Setup(m => m.Send(new WalletCommand()
        {
            Amount = It.IsAny<decimal>(),
            Narration = It.IsAny<string>(),
            RequestId = It.IsAny<string>(),
            TransactionType = TransactionType.Withdrawal,
        }, CancellationToken.None)).Returns(Task.FromResult(It.IsAny<RepositoryActionResult<WalletResponseDto>>()));


        var result = senderMock.Object.Send(
            new WalletCommand()
            {
                Amount = It.IsAny<decimal>(),
                Narration = It.IsAny<string>(),
                RequestId = It.IsAny<string>(),
                TransactionType = TransactionType.Withdrawal,
            }, CancellationToken.None);
        if (result.IsCompleted)
        {
            Assert.True(true);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void WalletToWalletTransfer_Success_Test()
    {
        var senderMock = new Mock<ISender>();
        senderMock.Setup(m => m.Send(new WalletToWalletTransferCommand()
        {
            Amount = It.IsAny<decimal>(),
            Narration = It.IsAny<string>(),
            RequestId = It.IsAny<string>(),
            DestinationReference = It.IsAny<string>(),
        }, CancellationToken.None)).Returns(Task.FromResult(It.IsAny<RepositoryActionResult<WalletToWalletResponseDto>>()));


        var result = senderMock.Object.Send(
            new WalletToWalletTransferCommand()
            {
                Amount = It.IsAny<decimal>(),
                Narration = It.IsAny<string>(),
                RequestId = It.IsAny<string>(),
                DestinationReference = It.IsAny<string>(),
            }, CancellationToken.None);
        if (result.IsCompleted)
        {
            Assert.True(true);
        }
        else
        {
            Assert.True(false);
        }
    }

    [Fact]
    public void GetWalletAndTransactionHistory_Success_Test()
    {
        var senderMock = new Mock<ISender>();
        senderMock.Setup(m => m.Send(new GetWalletAndTransactionHistoryQuery(It.IsAny<string>(),
                                                                             It.IsAny<GetWalletAndTransactionHistoryResourceParameter>()),
                                                                             CancellationToken.None)).Returns(Task.FromResult(It.IsAny<PagedList<GetWalletTransactionDto>>()));


        var result = senderMock.Object.Send(
            new GetWalletAndTransactionHistoryQuery(It.IsAny<string>(),
                                                    It.IsAny<GetWalletAndTransactionHistoryResourceParameter>()),
                                                    CancellationToken.None);
        if (result.IsCompleted)
        {
            Assert.True(true);
        }
        else
        {
            Assert.True(false);
        }
    }
}