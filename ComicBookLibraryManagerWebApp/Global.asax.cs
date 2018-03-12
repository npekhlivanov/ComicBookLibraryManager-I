using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace ComicBookLibraryManagerWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
            // Globalization: Model binding DateTimes with ASP.Net MVC
            // http://www.hackered.co.uk/articles/globalization-model-binding-datetimes-with-asp-net-mvc
            var culture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = System.Configuration.ConfigurationManager.AppSettings["DateFormat"];
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Identifies and returns the default locale to use by mapping as close as possible from ASP.Nets culture to Globalize's locales
        /// </summary>
        /// <returns>The default locale to use for the current culture; eg "de"</returns>
        public string GetDefaultLocale()
        {
            const string localePattern = "~/Content/cldr-data/main/{0}"; // where cldr-data lives on disk
            var currentCulture = CultureInfo.CurrentCulture;
            var cultureToUse = "en-GB"; //Default regionalisation to use

            //Try to pick a more appropriate regionalisation
            if (Directory.Exists(HostingEnvironment.MapPath(string.Format(localePattern, currentCulture.Name))))
            {//First try for a en-GB style directory
                cultureToUse = currentCulture.Name;
            }
            else
            {
                if (Directory.Exists(HostingEnvironment.MapPath(string.Format(localePattern, currentCulture.TwoLetterISOLanguageName))))
                {
                    //That failed; now try for a en style directory
                    cultureToUse = currentCulture.TwoLetterISOLanguageName;
                }
            }

            return cultureToUse;
        }
    }
}
