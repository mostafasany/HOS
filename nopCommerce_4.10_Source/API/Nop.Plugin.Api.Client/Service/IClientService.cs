using System.Collections.Generic;
using Nop.Plugin.Api.Modules.Client.Model;

namespace Nop.Plugin.Api.Modules.Client.Service
{
    public interface IClientService
    {
        void DeleteClient(int id);
        ClientApiModel FindClientByClientId(string clientId);
        ClientApiModel FindClientByIdAsync(int id);
        IList<ClientApiModel> GetAllClients();
        int InsertClient(ClientApiModel model);
        void UpdateClient(ClientApiModel model);
    }
}