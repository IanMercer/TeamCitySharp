using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using TeamCitySharp.Connection;

namespace TeamCitySharp.ActionTypes
{
    internal class BuildArtifacts : IBuildArtifacts
    {
        private readonly TeamCityCaller _caller;

        public BuildArtifacts(TeamCityCaller caller)
        {
            _caller = caller;
        }

        //public async Task<bool> DownloadArtifactsByBuildId(string path, string buildId)
        //{
        //    return await _caller.GetDownloadFormat(path, "/downloadArtifacts.html?buildId={0}", buildId);
        //}

        public ArtifactWrapper ByBuildConfigId(string buildConfigId)
        {
            return new ArtifactWrapper(_caller, buildConfigId);
        }
    }

    public class ArtifactWrapper
    {
        private readonly TeamCityCaller _caller;
        private readonly string _buildConfigId;

        internal ArtifactWrapper(TeamCityCaller caller, string buildConfigId)
        {
            _caller = caller;
            _buildConfigId = buildConfigId;
        }

        public async Task<ArtifactCollection> LastFinished()
        {
            return await Specification(".lastFinished");
        }

        public async Task<ArtifactCollection> LastPinned()
        {
            return await Specification(".lastPinned");
        }

        public async Task<ArtifactCollection> LastSuccessful()
        {
            return await Specification(".lastSuccessful");
        }

        public async Task<ArtifactCollection> Tag(string tag)
        {
            return await Specification(tag + ".tcbuildtag");
        }

        public async Task<ArtifactCollection> Specification(string buildSpecification)
        {
            var xml = await _caller.GetRaw(string.Format("/repository/download/{0}/{1}/teamcity-ivy.xml", _buildConfigId, buildSpecification));

            var document = new XmlDocument();
            document.LoadXml(xml);
            var artifactNodes = document.SelectNodes("//artifact");
            if (artifactNodes == null)
            {
                return null;
            }
            var list = new List<string>();
            foreach (XmlNode node in artifactNodes)
            {
                var nameNode = node.SelectSingleNode("@name");
                var extensionNode = node.SelectSingleNode("@ext");
                var artifact = string.Empty;
                if (nameNode != null)
                {
                    artifact = nameNode.Value;
                }
                if (extensionNode != null)
                {
                    artifact += "." + extensionNode.Value;
                }
                list.Add(string.Format("/repository/download/{0}/{1}/{2}", _buildConfigId, buildSpecification, artifact));
            }
            return new ArtifactCollection(_caller, list);
        }
    }

    public class ArtifactCollection
    {
        private readonly TeamCityCaller _caller;
        private readonly List<string> _urls;

        internal ArtifactCollection(TeamCityCaller caller, List<string> urls)
        {
            _caller = caller;
            _urls = urls;
        }

        /// <summary>
        /// Takes a list of artifact urls and downloads them, see ArtifactsBy* methods.
        /// </summary>
        /// <param name="destinationDirectory">
        /// Destination directory for downloaded artifacts.  Must NOT exist.
        /// </param>
        /// <returns>
        /// True if the download was successful, otherwise throws an error
        /// </returns>
        /// <remarks>
        /// Changed API from original.  Artifacts should be treated as an atomic unit
        /// fetched as a whole and delivered complete (or not at all).
        /// </remarks>
        public async Task<bool> Download(string destinationDirectory, TimeSpan timeout = default(TimeSpan))
        {
            if (string.IsNullOrWhiteSpace(destinationDirectory)) throw new ArgumentNullException("directory");
            if (File.Exists(destinationDirectory)) throw new ArgumentException(destinationDirectory + " is a file, cannot overwrite");
            if (Directory.Exists(destinationDirectory)) throw new ArgumentException(destinationDirectory + " already exists; must specify a new location each time");

            // Download to a temp directory, not directly to the target directory
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            foreach (var url in _urls)
            {
                // user probably didn't use to artifact url generating functions
                Debug.Assert(url.StartsWith("/repository/download/"));

                // figure out local filename
                // TODO: Fix the somewhat risky assumption that there are 5 parts before it
                var parts = url.Split('/').Skip(5).ToArray();
                string fileName = string.Join(Path.DirectorySeparatorChar.ToString(), parts);
                string destination = Path.Combine(tempDirectory, fileName);

                // create directories that do not exist
                var directoryName = Path.GetDirectoryName(destination);
                if (directoryName != null && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                var ok = await _caller.GetDownloadFormat(destination, timeout, url);
            }

            // finally, in one atomic action, move the downloaded artifacts over
            if (_urls.Any())
            {
                Directory.Move(tempDirectory, destinationDirectory);
            }
            else
            {
                // No artifacts, create a blank directory
                Directory.CreateDirectory(destinationDirectory);
            }
            return true;
        }
    }
}