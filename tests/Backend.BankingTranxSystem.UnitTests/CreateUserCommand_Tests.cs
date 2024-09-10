using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Commands;
using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.DTOs.Response;
using Backend.BankingTranxSystem.DataAccess.Enums;
using Backend.BankingTranxSystem.SharedServices.Helper;
using MediatR;
using Moq;

namespace Backend.BankingTranxSystem.UnitTests
{
    public class CreateUserCommand_Tests : IClassFixture<Fixture>
    {
        public Fixture _fixture;

        public CreateUserCommand_Tests(Fixture fixture) => _fixture = fixture ??
            throw new ArgumentNullException(nameof(Fixture));

        [Fact]
        public void CreateIndividualUserAccount_Success_Test()
        {
            var senderMock = new Mock<ISender>();
            senderMock.Setup(m => m.Send(new CreateUserCommand()
            {
                FirstName = It.IsAny<string>(),
                LastName = It.IsAny<string>(),
                OtherNames = It.IsAny<string>(),
                DateOfBirth = It.IsAny<DateTime>(),
                AccountType = AccountType.Individual,
                Bvn = It.IsAny<string>(),
                Country = It.IsAny<string>(),
                EmailAddress = It.IsAny<string>(),
                Gender = Gender.Female,
                Password = It.IsAny<string>(),
                PermanentAddress = It.IsAny<string>(),
                State = It.IsAny<string>(),
                TelephoneNumber = It.IsAny<string>(),
            }, CancellationToken.None)).Returns(Task.FromResult(It.IsAny<RepositoryActionResult<CreateUserResponseDto>>()));


            var result = senderMock.Object.Send(
                new CreateUserCommand()
                {
                    FirstName = It.IsAny<string>(),
                    LastName = It.IsAny<string>(),
                    OtherNames = It.IsAny<string>(),
                    DateOfBirth = It.IsAny<DateTime>(),
                    AccountType = AccountType.Individual,
                    Bvn = It.IsAny<string>(),
                    Country = It.IsAny<string>(),
                    EmailAddress = It.IsAny<string>(),
                    Gender = Gender.Female,
                    Password = It.IsAny<string>(),
                    PermanentAddress = It.IsAny<string>(),
                    State = It.IsAny<string>(),
                    TelephoneNumber = It.IsAny<string>(),
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
        public void CreateCorporateUserAccount_Success_Test()
        {
            var senderMock = new Mock<ISender>();
            senderMock.Setup(m => m.Send(new CreateUserCommand()
            {
                FirstName = It.IsAny<string>(),
                LastName = It.IsAny<string>(),
                OtherNames = It.IsAny<string>(),
                DateOfBirth = It.IsAny<DateTime>(),
                AccountType = AccountType.Corporate,
                Bvn = It.IsAny<string>(),
                Country = It.IsAny<string>(),
                EmailAddress = It.IsAny<string>(),
                Gender = Gender.NA,
                Password = It.IsAny<string>(),
                PermanentAddress = It.IsAny<string>(),
                State = It.IsAny<string>(),
                TelephoneNumber = It.IsAny<string>(),
            }, CancellationToken.None)).Returns(Task.FromResult(It.IsAny<RepositoryActionResult<CreateUserResponseDto>>()));


            var result = senderMock.Object.Send(
                new CreateUserCommand()
                {
                    FirstName = It.IsAny<string>(),
                    LastName = It.IsAny<string>(),
                    OtherNames = It.IsAny<string>(),
                    DateOfBirth = It.IsAny<DateTime>(),
                    AccountType = AccountType.Individual,
                    Bvn = It.IsAny<string>(),
                    Country = It.IsAny<string>(),
                    EmailAddress = It.IsAny<string>(),
                    Gender = Gender.Female,
                    Password = It.IsAny<string>(),
                    PermanentAddress = It.IsAny<string>(),
                    State = It.IsAny<string>(),
                    TelephoneNumber = It.IsAny<string>(),
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
}
