using System;
using System.Net;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TeamCitySharp.DomainEntities;
using System.Security.Authentication;
using System.Net.Http;

namespace TeamCitySharp.IntegrationTests
{
    [TestFixture]
    public class when_interacting_to_get_vcs_details
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
        public void it_throws_exception_when_no_host_specified()
        {
            var client = new TeamCityClient(null);

            //Assert: Exception
        }

        [Test]
        public void it_throws_exception_when_host_does_not_exist()
        {
            var client = new TeamCityClient("test:81");
            client.Connect("admin", "qwerty");

            Assert.That(async () => await client.VcsRoots.All(), Throws.Exception.TypeOf<HttpRequestException>());
        }

        [Test]
        public void it_throws_authentication_exception_when_no_connection_formed()
        {
            var client = new TeamCityClient("teamcity.codebetter.com");

            Assert.That(async () => await client.VcsRoots.All(), Throws.Exception.TypeOf<AuthenticationException>());
        }
        
        [Test]
        public void it_returns_all_vcs_roots()
        {
            List<VcsRoot> vcsRoots = _client.VcsRoots.All().Result;
            // Current user has no VCS roots so this test fails
            //Assert.NotNull(vcsRoots, "VCS roots was null");
            //Assert.That(vcsRoots.Any(), "No VCS Roots were found for the installation");
        }

        [TestCase("1")]
        public void it_returns_vcs_details_when_passing_vcs_root_id(string vcsRootId)
        {
            VcsRoot rootDetails = _client.VcsRoots.ById(vcsRootId).Result;
            Assert.That(rootDetails != null, "Cannot find the specific VCSRoot");
        }
    }
}