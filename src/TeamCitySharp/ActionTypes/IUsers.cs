using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCitySharp.DomainEntities;

namespace TeamCitySharp.ActionTypes
{
    public interface IUsers
    {
        Task<List<User>> All();
        Task<User> Details(string userName);
        Task<List<Role>> AllRolesByUserName(string userName);
        Task<List<Group>> AllGroupsByUserName(string userName);
        Task<List<Group>> AllUserGroups();
        Task<List<User>> AllUsersByUserGroup(string userGroupName);
        Task<List<Role>> AllUserRolesByUserGroup(string userGroupName);
        Task<bool> Create(string username, string name, string email, string password);
        Task<bool> AddPassword(string username, string password);
    }
}