namespace TeamCitySharp.ActionTypes
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using TeamCitySharp.Connection;
    using TeamCitySharp.DomainEntities;

    internal class ServerInformation : IServerInformation
    {
        private const string ServerUrlPrefix = "/app/rest/server";
        private readonly ITeamCityCaller _caller;

        internal ServerInformation(ITeamCityCaller caller)
        {
            _caller = caller;
        }

        public async Task<Server> ServerInfo()
        {
            var server = await _caller.Get<Server>(ServerUrlPrefix);
            return server;
        }

        public async Task<List<Plugin>> AllPlugins()
        {
            var pluginWrapper = await _caller.Get<PluginWrapper>(ServerUrlPrefix + "/plugins");
            return pluginWrapper.Plugin;
        }

        public async Task<string> TriggerServerInstanceBackup(BackupOptions backupOptions)
        {
            var backupOptionsUrlPart = this.BuildBackupOptionsUrl(backupOptions);
            var url = string.Concat(ServerUrlPrefix, "/backup?", backupOptionsUrlPart);
            return await _caller.StartBackup(url);
        }

        public async Task<string> GetBackupStatus()
        {
            var url = string.Concat(ServerUrlPrefix, "/backup");
            return await _caller.Get<string>(url);
        }

        private string BuildBackupOptionsUrl(BackupOptions backupOptions)
        {
            return new StringBuilder()
                .Append("fileName=").Append(backupOptions.Filename)
                .Append("&includeBuildLogs=").Append(backupOptions.IncludeBuildLogs)
                .Append("&includeConfigs=").Append(backupOptions.IncludeConfigurations)
                .Append("&includeDatabase=").Append(backupOptions.IncludeDatabase)
                .Append("&includePersonalChanges=").Append(backupOptions.IncludePersonalChanges)
                .ToString();
        }
    }
}