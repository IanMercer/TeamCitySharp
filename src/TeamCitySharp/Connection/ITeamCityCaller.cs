namespace TeamCitySharp.Connection
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal interface ITeamCityCaller
    {
        void Connect(string userName, string password, bool actAsGuest);

        Task GetDownloadFormat(Action<string> downloadHandler, string urlPart, params object[] parts);

        Task<string> StartBackup(string urlPart);

        Task<T> Get<T>(string urlPart);
        Task<T> GetFormat<T>(string urlPart, params object[] parts);

        Task<U> Post<T, U>(T data, string contenttype, string urlPart);
        Task<U> PostFormat<T, U>(T data, string contenttype, string urlPart, params object[] parts);

        Task<U> Put<T, U>(T data, string contenttype, string urlPart);
        Task<U> PutFormat<T, U>(T data, string contenttype, string urlPart, params object[] parts);

        Task<bool> Authenticate(string urlPart);

        Task<bool> Delete(string urlPart);
        Task<bool> DeleteFormat(string urlPart, params object[] parts);
    }
}