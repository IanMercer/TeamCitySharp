using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCitySharp.DomainEntities;
using TeamCitySharp.Locators;

namespace TeamCitySharp.ActionTypes
{
    public interface IVcsRoots
    {
        Task<List<VcsRoot>> All();
        Task<VcsRoot> ById(string vcsRootId);
        Task<VcsRoot> AttachVcsRoot(BuildTypeLocator locator, VcsRoot vcsRoot);
        void DetachVcsRoot(BuildTypeLocator locator, string vcsRootId);
        void SetVcsRootField<U>(VcsRoot vcsRoot, VcsRootField field, U value);
    }
}