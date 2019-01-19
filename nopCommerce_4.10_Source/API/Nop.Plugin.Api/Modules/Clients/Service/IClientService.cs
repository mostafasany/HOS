using System.Collections.Generic;
using Nop.Plugin.Api.Models;

namespace Nop.Plugin.Api.Services
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