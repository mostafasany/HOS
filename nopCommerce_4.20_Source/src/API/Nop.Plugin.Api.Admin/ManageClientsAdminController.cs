﻿using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Admin.Model;
using Nop.Plugin.Api.Admin.Service;
using Nop.Plugin.Api.Common.Constants;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Api.Admin
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [Route("admin/manageClientsAdmin/")]
    public class ManageClientsAdminController : BasePluginController
    {
        private readonly IClientService _clientService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public ManageClientsAdminController(ILocalizationService localizationService, IClientService clientService,
            INotificationService notificationService)
        {
            _localizationService = localizationService;
            _clientService = clientService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Route("list")]
        public ActionResult List()
        {
            return View(ViewNames.AdminApiClientsList);
        }

        [HttpPost]
        [Route("list")]
        public ActionResult List(object command)
        {
            var gridModel = _clientService.GetAllClients();

//            var grids = new DataSourceResult()
//            {
//                Data = gridModel,
//                Total = gridModel.Count()
//            };

            return Json(null);
        }

        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            var clientModel = new ClientApiModel
            {
                Enabled = true,
                ClientSecret = Guid.NewGuid().ToString(),
                ClientId = Guid.NewGuid().ToString(),
                AccessTokenLifetime = Configurations.DefaultAccessTokenExpiration,
                RefreshTokenLifetime = Configurations.DefaultRefreshTokenExpiration
            };

            return View(ViewNames.AdminApiClientsCreate, clientModel);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [Route("create")]
        public ActionResult Create(ClientApiModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var clientId = _clientService.InsertClient(model);

                _notificationService.SuccessNotification(
                    _localizationService.GetResource("Plugins.Api.Admin.Client.Created"));
                return continueEditing ? RedirectToAction("Edit", new {id = clientId}) : RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var clientModel = _clientService.FindClientByIdAsync(id);

            return View(ViewNames.AdminApiClientsEdit, clientModel);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [Route("edit/{id}")]
        public IActionResult Edit(ClientApiModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                _clientService.UpdateClient(model);

                _notificationService.SuccessNotification(
                    _localizationService.GetResource("Plugins.Api.Admin.Client.Edit"));
                return continueEditing ? RedirectToAction("Edit", new {id = model.Id}) : RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        [ActionName("Delete")]
        [Route("delete/{id}")]
        public IActionResult DeleteConfirmed(int id)
        {
            _clientService.DeleteClient(id);

            _notificationService.SuccessNotification(
                _localizationService.GetResource("Plugins.Api.Admin.Client.Deleted"));
            return RedirectToAction("List");
        }


//        [HttpGet]
//        [Route("list")]
//        public ActionResult List() => View(ViewNames.AdminApiClientsList);

        //TODO:4.2 Migration
        //[HttpPost]
        //[Route("list")]
        //public ActionResult List(DataSourceRequest command)
        //{
        //    IList<ClientApiModel> gridModel = _clientService.GetAllClients();

        //    var grids = new DataSourceResult
        //    {
        //        Data = gridModel,
        //        Total = gridModel.Count()
        //    };

        //    return Json(grids);
        //}
//
//        [HttpPost]
//        [Route("list")]
//        public ActionResult List(object command)
//        {
//            IList<ClientApiModel> gridModel = _clientService.GetAllClients();
//
//            //var grids = new DataSourceResult
//            //{
//            //    Data = gridModel,
//            //    Total = gridModel.Count()
//            //};
//
//            // return Json(grids);
//            return Json("");
//        }
    }
}