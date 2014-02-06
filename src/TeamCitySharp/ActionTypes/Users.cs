using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TeamCitySharp.Connection;
using TeamCitySharp.DomainEntities;

namespace TeamCitySharp.ActionTypes
{
    internal class Users : IUsers
    {
        private readonly TeamCityCaller _caller;

        internal Users(TeamCityCaller caller)
        {
            _caller = caller;
        }

        public async Task<List<User>> All()
        {
            var userWrapper = await _caller.Get<UserWrapper>("/app/rest/users");
            return userWrapper.User;
        }

        public async Task<List<Role>> AllRolesByUserName(string userName)
        {
            var user = await _caller.GetFormat<User>("/app/rest/users/username:{0}", userName);
            return user.Roles == null ? null : user.Roles.Role;
        }

        public async Task<User> Details(string userName)
        {
            var user = await _caller.GetFormat<User>("/app/rest/users/username:{0}", userName);

            return user;
        }

        public async Task<List<Group>> AllGroupsByUserName(string userName)
        {
            var user = await _caller.GetFormat<User>("/app/rest/users/username:{0}", userName);
            return user.Groups.Group;
        }

        public async Task<List<Group>> AllUserGroups()
        {
            var userGroupWrapper = await _caller.Get<UserGroupWrapper>("/app/rest/userGroups");
            return userGroupWrapper.Group;
        }

        public async Task<List<User>> AllUsersByUserGroup(string userGroupName)
        {
            var group = await _caller.GetFormat<Group>("/app/rest/userGroups/key:{0}", userGroupName);
            return group.Users.User;
        }

        public async Task<List<Role>> AllUserRolesByUserGroup(string userGroupName)
        {
            var group = await _caller.GetFormat<Group>("/app/rest/userGroups/key:{0}", userGroupName);
            return group.Roles.Role;
        }

        public async Task<bool> Create(string username, string name, string email, string password)
        {
            string data = string.Format("<user name=\"{0}\" username=\"{1}\" email=\"{2}\" password=\"{3}\"/>", name, username, email, password);
            var createUserResponse = await _caller.Post<string, string>(data, "application/xml", "/app/rest/users");
            // Workaround, Create POST request fails to deserialize password field. See http://youtrack.jetbrains.com/issue/TW-23200
            // Also this does not return an accurate representation of whether it has worked or not
            return await AddPassword(username, password);
        }

        public async Task<bool> AddPassword(string username, string password)
        {
            var response = await _caller.Put<string, string>(password, "text/plain", string.Format("/app/rest/users/username:{0}/password", username));
            return true;
        }

    }
}