using System;
using System.IO;
using System.Threading.Tasks;

namespace TeamCitySharp.ActionTypes
{
    public interface IBuildArtifacts
    {
        //Task<List<string>> DownloadArtifactsByBuildId(string path, string buildId);
        ArtifactWrapper ByBuildConfigId(string buildConfigId);
    }
}