using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Globalization;
using Frayte.WebApi.MessageHandler;

namespace Frayte.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
           // GlobalConfiguration.Configuration.MessageHandlers.Add(new APIKeyMessageHandler());
            
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //key for reomve extra text from barcode image
            Spire.Barcode.BarcodeSettings.ApplyKey("HGH0O-CU80Z-7P9IA-L1DFW-XNO5O");

            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Culture = new CultureInfo(string.Empty)
            //{
            //    NumberFormat = new NumberFormatInfo
            //    {
            //        CurrencyDecimalDigits = 2
            //    }
            //};
        }
    }
}
