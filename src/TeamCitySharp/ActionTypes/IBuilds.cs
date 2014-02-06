using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCitySharp.DomainEntities;
using TeamCitySharp.Locators;

namespace TeamCitySharp.ActionTypes
{
    public interface IBuilds
    {
        Task<List<Build>> SuccessfulBuildsByBuildConfigId(string buildConfigId);
        Task<Build> LastSuccessfulBuildByBuildConfigId(string buildConfigId);
        Task<List<Build>> FailedBuildsByBuildConfigId(string buildConfigId);
        Task<Build> LastFailedBuildByBuildConfigId(string buildConfigId);
        Task<Build> LastBuildByBuildConfigId(string buildConfigId);
        Task<List<Build>> ErrorBuildsByBuildConfigId(string buildConfigId);
        Task<Build> LastErrorBuildByBuildConfigId(string buildConfigId);
        Task<List<Build>> ByBuildConfigId(string buildConfigId);
        Task<List<Build>> ByConfigIdAndTag(string buildConfigId, string tag);
        Task<List<Build>> ByUserName(string userName);
        Task<List<Build>> ByBuildLocator(BuildLocator locator);
        Task<List<Build>> AllSinceDate(DateTime date);
        Task<List<Build>> AllBuildsOfStatusSinceDate(DateTime date, BuildStatus buildStatus);
        Task<List<Build>> NonSuccessfulBuildsForUser(string userName);
        Task<List<Build>> ByBranch(string branchName);
        Task<Build> LastBuildByAgent(string agentName);
        void Add2QueueBuildByBuildConfigId(string buildConfigId);
    }
}