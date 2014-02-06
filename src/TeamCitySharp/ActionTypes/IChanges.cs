using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCitySharp.DomainEntities;

namespace TeamCitySharp.ActionTypes
{
    public interface IChanges
    {
        Task<List<Change>> All();
        Task<Change> ByChangeId(string id);
        Task<Change> LastChangeDetailByBuildConfigId(string buildConfigId);
        Task<List<Change>> ByBuildConfigId(string buildConfigId);
    }
}