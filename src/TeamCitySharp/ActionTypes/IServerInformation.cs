using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCitySharp.DomainEntities;

namespace TeamCitySharp.ActionTypes
{
    public interface IServerInformation
    {
        Task<Server> ServerInfo();
        Task<List<Plugin>> AllPlugins();
        Task<string> TriggerServerInstanceBackup(BackupOptions backupOptions);
        Task<string> GetBackupStatus();
    }
}