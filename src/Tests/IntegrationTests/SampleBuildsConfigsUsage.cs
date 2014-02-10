using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using TeamCitySharp.Locators;
using System.Net.Http;
using System.Security.Authentication;

namespace TeamCitySharp.IntegrationTests
{
    [TestFixture]
    public class when_interations_to_get_build_configuration_details
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
        public void it_throws_exception_when_no_url_passed()
        {
            var client = new TeamCityClient(null);

            //Assert: Exception
        }

        [Test]
        public void it_throws_exception_when_host_does_not_exist()
        {
            var client = new TeamCityClient("test:81");
            client.Connect("teamcitysharpuser", "qwerty");

            Assert.That(async () => await client.BuildConfigs.All(), Throws.Exception.TypeOf<HttpRequestException>());
        }

        [Test]
        public void it_throws_authentication_exception_when_no_connection_formed()
        {
            var client = new TeamCityClient("teamcity.codebetter.com");

            Assert.That(async () => await client.BuildConfigs.All(), Throws.Exception.TypeOf<AuthenticationException>());
        }

        [Test]
        public void it_returns_all_build_types()
        {
            var buildConfigs = _client.BuildConfigs.All().Result;

            Assert.That(buildConfigs.Any(), "No build types were found in this server");
        }

        [Test]
        public void it_returns_build_config_details_by_configuration_id()
        {
            string buildConfigId = "bt437";
            var buildConfig = _client.BuildConfigs.ByConfigurationId(buildConfigId);

            Assert.That(buildConfig != null, "Cannot find a build type for that buildId");
        }

        [Test]
        [Ignore("No permissions")]
        public void it_pauses_configuration()
        {
            string buildConfigId = "bt437";
            var buildLocator = BuildTypeLocator.WithId(buildConfigId);
            _client.BuildConfigs.SetConfigurationPauseStatus(buildLocator, true);
            var status = _client.BuildConfigs.GetConfigurationPauseStatus(buildLocator).Result;
            Assert.That(status == true, "Build not paused");
        }

        [Test]
        [Ignore("No permissions")]
        public void it_unpauses_configuration()
        {
            string buildConfigId = "bt437";
            var buildLocator = BuildTypeLocator.WithId(buildConfigId);
            _client.BuildConfigs.SetConfigurationPauseStatus(buildLocator, false);
            var status = _client.BuildConfigs.GetConfigurationPauseStatus(buildLocator).Result;
            Assert.That(status == false, "Build not unpaused");
        }

        [Test]
        public void it_returns_build_config_details_by_configuration_name()
        {
            string buildConfigName = "Release Build";
            var buildConfig = _client.BuildConfigs.ByConfigurationName(buildConfigName);

            Assert.That(buildConfig != null, "Cannot find a build type for that buildName");
        }

        [Test]
        public void it_returns_build_configs_by_project_id()
        {
            string projectId = "project137";
            var buildConfigs = _client.BuildConfigs.ByProjectId(projectId).Result;

            Assert.That(buildConfigs.Any(), "Cannot find a build type for that projectId");
        }

        [Test]
        public void it_returns_build_configs_by_project_name()
        {
            string projectName = "YouTrackSharp";
            var buildConfigs = _client.BuildConfigs.ByProjectName(projectName).Result;

            Assert.That(buildConfigs.Any(), "Cannot find a build type for that projectName");
        }
    }
}