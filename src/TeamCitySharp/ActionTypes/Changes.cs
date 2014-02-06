using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamCitySharp.Connection;
using TeamCitySharp.DomainEntities;

namespace TeamCitySharp.ActionTypes
{
    internal class Changes : IChanges
    {
        private readonly TeamCityCaller _caller;

        internal Changes(TeamCityCaller caller)
        {
            _caller = caller;
        }

        public async Task<List<Change>> All()
        {
            var changeWrapper = await _caller.Get<ChangeWrapper>("/app/rest/changes");
            return changeWrapper.Change;
        }

        public async Task<Change> ByChangeId(string id)
        {
            var change = await _caller.GetFormat<Change>("/app/rest/changes/id:{0}", id);
            return change;
        }

        public async Task<List<Change>> ByBuildConfigId(string buildConfigId)
        {
            var changeWrapper = await _caller.GetFormat<ChangeWrapper>("/app/rest/changes?buildType={0}", buildConfigId);
            return changeWrapper.Change;
        }

        public async Task<Change> LastChangeDetailByBuildConfigId(string buildConfigId)
        {
            var changes = await ByBuildConfigId(buildConfigId);
            return changes.FirstOrDefault();
        }

    }
}