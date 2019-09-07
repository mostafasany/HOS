using System.Collections.Generic;
using Nop.Plugin.Api.Menu.Dto;

namespace Nop.Plugin.Api.Menu.Service
{
    public interface IMenuApiService
    {
        MenuDto GetMenu();
        List<MenuDto2> GetNewMenu();
    }
}