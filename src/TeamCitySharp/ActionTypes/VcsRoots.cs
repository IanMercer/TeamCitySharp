using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using TeamCitySharp.Connection;
using TeamCitySharp.DomainEntities;
using TeamCitySharp.Locators;

namespace TeamCitySharp.ActionTypes
{
    internal class VcsRoots: IVcsRoots
    {
        private readonly TeamCityCaller _caller;

        internal VcsRoots(TeamCityCaller caller)
        {
            _caller = caller;
        }

        public async Task<List<VcsRoot>> All()
        {
            var vcsRootWrapper = await _caller.Get<VcsRootWrapper>("/app/rest/vcs-roots");
            return vcsRootWrapper.VcsRoot;
        }

        public async Task<VcsRoot> ById(string vcsRootId)
        {
            var vcsRoot = await _caller.GetFormat<VcsRoot>("/app/rest/vcs-roots/id:{0}", vcsRootId);
            return vcsRoot;
        }

        [DataContract(Name="vcs-root-entry")]
        private class VcsRootEntry
        {
            [DataMember(Name = "id")]
            public string id { get; set; }
        }

        public async Task<VcsRoot> AttachVcsRoot(BuildTypeLocator locator, VcsRoot vcsRoot)
        {
            var vcsRootEntry = new VcsRootEntry { id = vcsRoot.Id };
            //var xml = string.Format(@"<vcs-root-entry><vcs-root id=""{0}""/></vcs-root-entry>", vcsRoot.Id);
            return await _caller.PostFormat<VcsRootEntry, VcsRoot>(vcsRootEntry, "application/xml", "/app/rest/buildTypes/{0}/vcs-root-entries", locator);
        }

        public void DetachVcsRoot(BuildTypeLocator locator, string vcsRootId)
        {
            _caller.DeleteFormat("/app/rest/buildTypes/{0}/vcs-root-entries/{1}", locator, vcsRootId);
        }

        public void SetVcsRootField<U>(VcsRoot vcsRoot, VcsRootField field, U value)
        {
            _caller.PutFormat<U, string>(value, "text/xml", "/app/rest/vcs-roots/id:{0}/{1}", vcsRoot.Id, ToCamelCase(field.ToString()));
        }

        private static string ToCamelCase(string s)
        {
            return Char.ToLower(s.ToCharArray()[0]) + s.Substring(1);
        }
    }
}