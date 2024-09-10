using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Backend.BankingTranxSystem.UnitTests;

public class Fixture : IDisposable
{
     public ServiceProvider ServiceProviderDi { get; }
     public Dictionary<string, long> keyValues1 = default;
     //public Dictionary<string, FriendsInEntityWithOutCount> GetFriendsInRespectiveGroupsOutput = default;
     //public Dictionary<string, FriendsInEntityWithCount> GetFriendsInRespectiveNetworkOrGroupOutput = default;
     //public Dictionary<string, SqlUserBelongsToGroup> GetUserGroupMemberOrAdminStatusOutput = default;
     //public (string, User)[] GetNationalNetworkForGroupOutput = default;

     private static IConfiguration Configuration { set; get; }
     public Fixture()
     {

         IServiceCollection services = new ServiceCollection();
         var Configuration = new ConfigurationBuilder()
              .AddInMemoryCollection(
                    new Dictionary<string, string>())
              .Build();
         services.AddSingleton<IConfiguration>(Configuration);

         services.AddSingleton(app =>
         {
             var mock = new Mock<ILogger>();
             mock.Setup(logger => logger.Log(
                 It.IsAny<LogLevel>(),
                 It.IsAny<EventId>(),
                 It.IsAny<Object>(),
                 It.IsAny<Exception>(),
                 It.IsAny<Func<object, Exception, string>>())
             ).Callback(() => { });
             return mock.Object;
         });
         ServiceProviderDi = services.BuildServiceProvider();
     }

     /// <summary>
     /// Create New Mock Logger
     /// </summary>
     /// <typeparam name="T"></typeparam>
     /// <returns></returns>
     private static Mock<ILogger<T>> CreateMockLogger<T>()
     {
         var mockLogger = new Mock<ILogger<T>>();
         mockLogger.Setup(logger => logger.Log(
             It.IsAny<Microsoft.Extensions.Logging.LogLevel>(),
             It.IsAny<EventId>(),
             It.IsAny<Object>(),
             It.IsAny<Exception>(),
             It.IsAny<Func<object, Exception, string>>())
         ).Callback(() => { });
         return mockLogger;
     }

     /// <summary>
     /// mock httprequest
     /// </summary>
     /// <returns>mock http request</returns>
     public HttpRequest HttpRequestGetSetup(string body)
     {
         var reqMock = new Mock<HttpRequest>();
         var stream = new MemoryStream();
         var writer = new StreamWriter(stream);
         writer.Write(body);
         writer.Flush();
         stream.Position = 0;
         reqMock.Setup(req => req.Body).Returns(stream);
         reqMock.Setup(req => req.Method).Returns("GET");
         var queryCollection = new QueryCollection(new Dictionary<string, StringValues>()
         {
             { "zipCode", "10001" }
         });
         reqMock.Setup(req => req.Query).Returns(queryCollection);
         HeaderDictionary keyValuePairs = new HeaderDictionary
         {
             { "x-caller-id", "12121CWeHSfV5xcgPcfmWbLQ2qlflU5D3|F" },
         };
         reqMock.Setup(req => req.Headers).Returns(keyValuePairs);
         return reqMock.Object;
     }

     /// <summary>
     /// mock httprequest
     /// </summary>
     /// <returns>mock http request</returns>
     public HttpRequest HttpRequestGetSetupNull(string body)
     {
         var reqMock = new Mock<HttpRequest>();
         var stream = new MemoryStream();
         var writer = new StreamWriter(stream);
         writer.Write(body);
         writer.Flush();
         stream.Position = 0;
         reqMock.Setup(req => req.Body).Returns(stream);
         reqMock.Setup(req => req.Method).Returns("GET");
         HeaderDictionary keyValuePairs = new HeaderDictionary
         {
             { "x-caller-id", "12121CWeHSfV5xcgPcfmWbLQ2qlflU5D3|F" },
         };
         reqMock.Setup(req => req.Headers).Returns(keyValuePairs);
         return reqMock.Object;
     }
     public void Dispose()
     {

     }
}