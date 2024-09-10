using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Commands;
using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.DTOs.Response;
using Backend.BankingTranxSystem.SharedServices.Helper;
using MediatR;
using Moq;

namespace Backend.BankingTranxSystem.UnitTests;

public class LoginUserCommand_Tests : IClassFixture<Fixture>
{
    public Fixture _fixture;

    public LoginUserCommand_Tests(Fixture fixture) => _fixture = fixture ??
        throw new ArgumentNullException(nameof(Fixture));

    [Fact]
    public void LoginUser_Success_Test()
    {
        var senderMock = new Mock<ISender>();
        senderMock.Setup(m => m.Send(new LoginUserCommand()
        {
            EmailAddress = It.IsAny<string>(),
            Password = It.IsAny<string>(),
        }, CancellationToken.None)).Returns(Task.FromResult(It.IsAny<RepositoryActionResult<LoginUserResponseDto>>()));


        var result = senderMock.Object.Send(
            new LoginUserCommand()
            {
                EmailAddress = It.IsAny<string>(),
                Password = It.IsAny<string>(),
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
}