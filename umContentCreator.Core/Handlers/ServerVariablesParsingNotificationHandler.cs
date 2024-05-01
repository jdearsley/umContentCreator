using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
using umContentCreator.Core.Interfaces;

namespace umContentCreator.Core.Handlers
{
    public class ServerVariablesParsingNotificationHandler : INotificationHandler<ServerVariablesParsingNotification>
    {
        private ISettingsService _settingsService;
        public ServerVariablesParsingNotificationHandler(ISettingsService settingsService) {
            _settingsService = settingsService;
        }
        public void Handle(ServerVariablesParsingNotification notification)
        {
            var task = _settingsService.LoadSettingsAsync();
            var settings = task.Result;

            notification.ServerVariables.Add("umContentCreator", new
            {
                ApiKey = settings.ApiKey
            });
        }
    }
}
