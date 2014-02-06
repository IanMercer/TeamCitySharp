using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Xml;
using TeamCitySharp.Connection;
using TeamCitySharp.DomainEntities;
using TeamCitySharp.Locators;

namespace TeamCitySharp.ActionTypes
{
    internal class BuildConfigs : IBuildConfigs
    {
        private readonly TeamCityCaller _caller;

        internal BuildConfigs(TeamCityCaller caller)
        {
            _caller = caller;
        }

        public async Task<List<BuildConfig>> All()
        {
            var buildType = await _caller.Get<BuildTypeWrapper>("/app/rest/buildTypes");
            return buildType.BuildType;
        }

        public async Task<BuildConfig> ByConfigurationName(string buildConfigName)
        {
            var build = await _caller.GetFormat<BuildConfig>("/app/rest/buildTypes/name:{0}", buildConfigName);
            return build;
        }

        public async Task<BuildConfig> ByConfigurationId(string buildConfigId)
        {
            var build = await _caller.GetFormat<BuildConfig>("/app/rest/buildTypes/id:{0}", buildConfigId);
            return build;
        }

        public async Task<BuildConfig> ByProjectNameAndConfigurationName(string projectName, string buildConfigName)
        {
            var build = await _caller.Get<BuildConfig>(string.Format("/app/rest/projects/name:{0}/buildTypes/name:{1}", projectName, buildConfigName));
            return build;
        }

        public async Task<BuildConfig> ByProjectNameAndConfigurationId(string projectName, string buildConfigId)
        {
            var build = await _caller.Get<BuildConfig>(string.Format("/app/rest/projects/name:{0}/buildTypes/id:{1}", projectName, buildConfigId));
            return build;
        }

        public async Task<BuildConfig> ByProjectIdAndConfigurationName(string projectId, string buildConfigName)
        {
            var build = await _caller.Get<BuildConfig>(string.Format("/app/rest/projects/id:{0}/buildTypes/name:{1}", projectId, buildConfigName));
            return build;
        }

        public async Task<BuildConfig> ByProjectIdAndConfigurationId(string projectId, string buildConfigId)
        {
            var build = await _caller.Get<BuildConfig>(string.Format("/app/rest/projects/id:{0}/buildTypes/id:{1}", projectId, buildConfigId));
            return build;
        }

        public async Task<List<BuildConfig>> ByProjectId(string projectId)
        {
            var buildWrapper = await _caller.GetFormat<BuildTypeWrapper>("/app/rest/projects/id:{0}/buildTypes", projectId);
            if (buildWrapper == null || buildWrapper.BuildType == null) return new List<BuildConfig>();
            return buildWrapper.BuildType;
        }

        public async Task<List<BuildConfig>> ByProjectName(string projectName)
        {
            var buildWrapper = await _caller.GetFormat<BuildTypeWrapper>("/app/rest/projects/name:{0}/buildTypes", projectName);

            if (buildWrapper == null || buildWrapper.BuildType == null) return new List<BuildConfig>();
            return buildWrapper.BuildType;
        }

        public async Task<BuildConfig> CreateConfiguration(string projectName, string configurationName)
        {
            return await _caller.PostFormat<string, BuildConfig>(configurationName, "text/plain", "application/json", "/app/rest/projects/name:{0}/buildTypes", projectName);
        }

        public void SetConfigurationSetting(BuildTypeLocator locator, string settingName, string settingValue)
        {
            _caller.PutFormat<string, string>(settingValue, "text/plain", "/app/rest/buildTypes/{0}/settings/{1}", locator, settingName);
        }

        public async Task<bool> GetConfigurationPauseStatus(BuildTypeLocator locator)
        {
             return await _caller.GetFormat<bool>("/app/rest/buildTypes/{0}/paused/", locator);
        }

        public void SetConfigurationPauseStatus(BuildTypeLocator locator, bool isPaused)
        {
            _caller.PutFormat<bool, string>(isPaused, "text/plain", "/app/rest/buildTypes/{0}/paused/", locator);
        }

        public void PostRawArtifactDependency(BuildTypeLocator locator, string rawXml)
        {
            _caller.PostFormat<string, ArtifactDependency>(rawXml, "application/xml", string.Empty, "/app/rest/buildTypes/{0}/artifact-dependencies", locator);
        }

        public void PostRawBuildStep(BuildTypeLocator locator, string rawXml)
        {
            _caller.PostFormat<string, BuildConfig>(rawXml, "application/xml", string.Empty, "/app/rest/buildTypes/{0}/steps", locator);
        }

        public void PostRawBuildTrigger(BuildTypeLocator locator, string rawXml)
        {
            _caller.PostFormat<string, string>(rawXml, "application/xml", "/app/rest/buildTypes/{0}/triggers", locator);
        }

        public void SetConfigurationParameter(BuildTypeLocator locator, string key, string value)
        {
            _caller.PutFormat<string,string>(value, "text/plain", "/app/rest/buildTypes/{0}/parameters/{1}", locator, key);
        }

        public void DeleteConfiguration(BuildTypeLocator locator)
        {
            _caller.DeleteFormat("/app/rest/buildTypes/{0}", locator);
        }

        public void DeleteAllBuildTypeParameters(BuildTypeLocator locator)
        {
            _caller.DeleteFormat("/app/rest/buildTypes/{0}/parameters", locator);
        }

        public void PutAllBuildTypeParameters(BuildTypeLocator locator, IDictionary<string, string> parameters)
        {
            if(locator == null)
                throw new ArgumentNullException("locator");
            if(parameters == null)
                throw new ArgumentNullException("parameters");

            StringWriter sw = new StringWriter();
            using(XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.WriteStartElement("properties");
                foreach(var parameter in parameters)
                {
                    writer.WriteStartElement("property");
                    writer.WriteAttributeString("name", parameter.Key);
                    writer.WriteAttributeString("value", parameter.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            _caller.PutFormat<string,string>(sw.ToString(), "application/xml", "/app/rest/buildTypes/{0}/parameters", locator);
        }

        public void DownloadConfiguration(BuildTypeLocator locator, Action<string> downloadHandler)
        {
            _caller.GetDownloadFormat(downloadHandler, "/app/rest/buildTypes/{0}", locator);
        }

        public void PostRawAgentRequirement(BuildTypeLocator locator, string rawXml)
        {
            _caller.PostFormat<string,string>(rawXml, "application/xml", "/app/rest/buildTypes/{0}/agent-requirements", locator);
        }

        public void DeleteBuildStep(BuildTypeLocator locator, string buildStepId)
        {
            _caller.DeleteFormat("/app/rest/buildTypes/{0}/steps/{1}", locator, buildStepId);
        }

        public void DeleteArtifactDependency(BuildTypeLocator locator, string artifactDependencyId)
        {
            _caller.DeleteFormat("/app/rest/buildTypes/{0}/artifact-dependencies/{1}", locator, artifactDependencyId);
        }

        public void DeleteAgentRequirement(BuildTypeLocator locator, string agentRequirementId)
        {
            _caller.DeleteFormat("/app/rest/buildTypes/{0}/agent-requirements/{1}", locator, agentRequirementId);
        }

        public void DeleteParameter(BuildTypeLocator locator, string parameterName)
        {
            _caller.DeleteFormat("/app/rest/buildTypes/{0}/parameters/{1}", locator, parameterName);
        }

        public void DeleteBuildTrigger(BuildTypeLocator locator, string buildTriggerId)
        {
            _caller.DeleteFormat("/app/rest/buildTypes/{0}/triggers/{1}", locator, buildTriggerId);
        }

        public void SetBuildTypeTemplate(BuildTypeLocator locatorBuildType, BuildTypeLocator locatorTemplate)
        {
            _caller.PutFormat<string,string>(locatorTemplate.ToString(), "text/plain", "/app/rest/buildTypes/{0}/template", locatorBuildType);
        }

        public void DeleteSnapshotDependency(BuildTypeLocator locator, string snapshotDependencyId)
        {
            _caller.DeleteFormat("/app/rest/buildTypes/{0}/snapshot-dependencies/{1}", locator, snapshotDependencyId);
        }

        public void PostRawSnapshotDependency(BuildTypeLocator locator, XmlElement rawXml)
        {
            _caller.PostFormat<string,string>(rawXml.OuterXml, "application/xml", "/app/rest/buildTypes/{0}/snapshot-dependencies", locator);
        }

        public async Task<BuildConfig> BuildType(BuildTypeLocator locator)
        {
            var build = await _caller.GetFormat<BuildConfig>("/app/rest/buildTypes/{0}", locator);
            return build;
        }
    }
}