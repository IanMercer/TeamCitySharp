using System;

namespace TeamCitySharp.DomainEntities
{
    public class Server
    {
        public string VersonMajor { get; set; }
        public string Version { get; set; }
        public string BuildNumber { get; set; }
        public DateTimeOffset CurrentTime { get; set; }
        public DateTimeOffset StartTime { get; set; }
    }
}