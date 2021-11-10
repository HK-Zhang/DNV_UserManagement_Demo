using DNVGL.Authorization.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyRoleUser.AspCore3.EFCore5
{
    public class PermissionBook : IPermissionMatrix
    {
        public enum NormalPermission
        {
            [PermissionValue(id: "7", key: "ManageWeatherForecast", name: "Manage WeatherForecast", group: "Normal", description: "Manage WeatherForecast")]
            ManageWeatherForecast,

            [PermissionValue(id: "8", key: "ViewWeatherForecast", name: "View WeatherForecast", group: "Normal", description: "View WeatherForecast")]
            ViewWeatherForecast,
        }

        public enum SpecialPermission
        {
            [PermissionValue(id: "9", key: "CreateReport", name: "Create Report", group: "Special", description: "CreateReport")]
            CreateReport,
        }
    }
}
