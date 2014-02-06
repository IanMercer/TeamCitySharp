using System.Security.Authentication;
using NUnit.Framework;

namespace TeamCitySharp.IntegrationTests
{
    [TestFixture]
    public class when_connecting_to_the_teamcity_server
    {
        private ITeamCityClient _client;

        [SetUp]
        public void SetUp()
        {
            _client = new TeamCityClient("localhost:81");
           
        }

        [Test]
        public void it_will_authenticate_a_known_user()
        {
            _client.Connect("admin", "qwerty");
            
            Assert.That(_client.Authenticate().Result);
        }

        [Test]
        public void it_will_throw_an_exception_for_an_unknown_user()
        {
            _client.Connect("smithy", "smithy");
            Assert.That(async () => await _client.Authenticate(), Throws.Exception.TypeOf<AuthenticationException>());

            //Assert.Throws Exception
        }

    }
}
