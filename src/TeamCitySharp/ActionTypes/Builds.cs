using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamCitySharp.Connection;
using TeamCitySharp.DomainEntities;
using TeamCitySharp.Locators;

namespace TeamCitySharp.ActionTypes
{
    internal class Builds : IBuilds
    {
        private readonly TeamCityCaller _caller;

        internal Builds(TeamCityCaller caller)
        {
            _caller = caller;
        }

        public async Task<List<Build>> ByBuildLocator(BuildLocator locator)
        {
            var buildWrapper = await _caller.GetFormat<BuildWrapper>("/app/rest/builds?locator={0}", locator);
            if (int.Parse(buildWrapper.Count) > 0)
            {
                return buildWrapper.Build;
            }
            return new List<Build>();
        }

        public async Task<Build> LastBuildByAgent(string agentName)
        {
            var builds = await ByBuildLocator(BuildLocator.WithDimensions(agentName: agentName,
                maxResults: 1));
            return builds.SingleOrDefault();
        }

        public void Add2QueueBuildByBuildConfigId(string buildConfigId)
        {
            _caller.GetFormat<string>("/action.html?add2Queue={0}", buildConfigId);
        }

        public Task<List<Build>> SuccessfulBuildsByBuildConfigId(string buildConfigId)
        {
            return ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                status: BuildStatus.SUCCESS));
        }

        public async Task<Build> LastSuccessfulBuildByBuildConfigId(string buildConfigId)
        {
            var builds = await ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                status: BuildStatus.SUCCESS, maxResults: 1));
            return builds != null ? builds.FirstOrDefault() : new Build();
        }

        public async Task<List<Build>> FailedBuildsByBuildConfigId(string buildConfigId)
        {
            return await ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                status: BuildStatus.FAILURE));
        }

        public async Task<Build> LastFailedBuildByBuildConfigId(string buildConfigId)
        {
            var builds = await ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                status: BuildStatus.FAILURE, maxResults: 1));
            return builds != null ? builds.FirstOrDefault() : new Build();
        }

        public async Task<Build> LastBuildByBuildConfigId(string buildConfigId)
        {
            var builds = await ByBuildLocator(BuildLocator.WithDimensions(
                BuildTypeLocator.WithId(buildConfigId), maxResults: 1));
            return builds != null ? builds.FirstOrDefault() : new Build();
        }

        public async Task<List<Build>> ErrorBuildsByBuildConfigId(string buildConfigId)
        {
            return await ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                status: BuildStatus.ERROR));
        }

        public async Task<Build> LastErrorBuildByBuildConfigId(string buildConfigId)
        {
            var builds = await ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                status: BuildStatus.ERROR, maxResults: 1));
            return builds != null ? builds.FirstOrDefault() : new Build();
        }

        public async Task<List<Build>> ByBuildConfigId(string buildConfigId)
        {
            return await ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId)));
        }

        public async Task<List<Build>> ByConfigIdAndTag(string buildConfigId, string tag)
        {
            return await ByConfigIdAndTag(buildConfigId, new[] { tag });
        }

        public async Task<List<Build>> ByConfigIdAndTag(string buildConfigId, string[] tags)
        {
            return await ByBuildLocator(BuildLocator.WithDimensions(BuildTypeLocator.WithId(buildConfigId),
                tags: tags));
        }

        public async Task<List<Build>> ByUserName(string userName)
        {
            return await ByBuildLocator(BuildLocator.WithDimensions(user: UserLocator.WithUserName(userName)));
        }

        public async Task<List<Build>> AllSinceDate(DateTime date)
        {
            return await ByBuildLocator(BuildLocator.WithDimensions(sinceDate: date));
        }

        public async Task<List<Build>> ByBranch(string branchName)
        {
            return await ByBuildLocator(BuildLocator.WithDimensions(branch: branchName));
        } 

        public async Task<List<Build>> AllBuildsOfStatusSinceDate(DateTime date, BuildStatus buildStatus)
        {
            return await ByBuildLocator(BuildLocator.WithDimensions(sinceDate: date, status: buildStatus));
        }

        public async Task<List<Build>> NonSuccessfulBuildsForUser(string userName)
        {
            var builds = await ByUserName(userName);
            if (builds == null)
            {
                return null;
            }
            return builds.Where(b => b.Status != "SUCCESS").ToList();
        }
    }
}