using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCitySharp.DomainEntities;

namespace TeamCitySharp.ActionTypes
{
    public interface IProjects
    {
        Task<List<Project>> All();
        Task<Project> ByName(string projectLocatorName);
        Task<Project> ById(string projectLocatorId);
        Task<Project> Details(Project project);
        Task<Project> Create(string projectName);
        void Delete(string projectName);
        void DeleteProjectParameter(string projectName, string parameterName);
        void SetProjectParameter(string projectName, string settingName, string settingValue);
    }
}