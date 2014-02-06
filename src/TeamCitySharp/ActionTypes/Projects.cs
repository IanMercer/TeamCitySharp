using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCitySharp.Connection;
using TeamCitySharp.DomainEntities;

namespace TeamCitySharp.ActionTypes
{
    internal class Projects : IProjects
    {
        private readonly TeamCityCaller _caller;

        internal Projects(TeamCityCaller caller)
        {
            _caller = caller;
        }

        public async Task<List<Project>> All()
        {
            var projectWrapper = await _caller.Get<ProjectWrapper>("/app/rest/projects");
            return projectWrapper.Project;
        }

        public async Task<Project> ByName(string projectLocatorName)
        {
            var project = await _caller.GetFormat<Project>("/app/rest/projects/name:{0}", projectLocatorName);
            return project;
        }

        public async Task<Project> ById(string projectLocatorId)
        {
            var project = await _caller.GetFormat<Project>("/app/rest/projects/id:{0}", projectLocatorId);
            return project;
        }

        public async Task<Project> Details(Project project)
        {
            return await ById(project.Id);
        }

        public async Task<Project> Create(string projectName)
        {
            return await _caller.Post<string, Project>(projectName, "application/xml", "/app/rest/projects/");
        }

        public void Delete(string projectName)
        {
            _caller.DeleteFormat("/app/rest/projects/name:{0}", projectName);
        }

        public void DeleteProjectParameter(string projectName, string parameterName)
        {
            _caller.DeleteFormat("/app/rest/projects/name:{0}/parameters/{1}", projectName, parameterName);
        }

        public void SetProjectParameter(string projectName, string settingName, string settingValue)
        {
            _caller.PutFormat<string, string>(settingValue, "/app/rest/projects/name:{0}/parameters/{1}", projectName, settingName);
        }
    }
}