using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using TeamCitySharp.DomainEntities;
using System.Net.Http;

namespace TeamCitySharp.IntegrationTests
{
    [TestFixture]
    public class when_interacting_to_get_agent_details
    {
        private ITeamCityClient _client;

        [SetUp]
        public void SetUp()
        {
            _client = new TeamCityClient("teamcity.codebetter.com");
            _client.Connect("teamcitysharpuser", "qwerty");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void it_throws_exception_when_no_host()
        {
            var client = new TeamCityClient(null);

            //Assert: Exception
        }

        [Test]
        public void it_throws_exception_when_host_url_invalid()
        {
            var client = new TeamCityClient("teamcity:81");
            client.Connect("teamcitysharpuser", "qwerty");

            Assert.That(async () => await client.Agents.All(), Throws.Exception.TypeOf<HttpRequestException>());
        }

        [Test]
        public void it_throws_exception_when_no_client_connection_made()
        {
            var client = new TeamCityClient("teamcity.codebetter.com");

            Assert.That(async () => await client.Agents.All(), Throws.Exception.TypeOf<HttpRequestException>());
        }

        [Test]
        public void it_returns_all_agents()
        {
            List<Agent> agents = _client.Agents.All().Result;

            Assert.That(agents.Any(), "No agents were found");
        }

        [TestCase("agent01")]
        public void it_returns_last_build_status_for_agent(string agentName)
        {
            Build lastBuild = _client.Builds.LastBuildByAgent(agentName).Result;

            Assert.That(lastBuild != null, "No build information found for the last build on the specified agent");
        }
    }
}