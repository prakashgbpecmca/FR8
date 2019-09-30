using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frayteWinApp.Utility
{
    public static class AppSettings
    {
        public static string WebApiURL 
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["WebApiURL"].ToString()) ? "" : ConfigurationManager.AppSettings["WebApiURL"].ToString();
            }
        }

        public static string WebApiRateCardURL
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["WebApiRateCardURL"].ToString()) ? "" : ConfigurationManager.AppSettings["WebApiRateCardURL"].ToString();
            }
        }

        public static string WebApiDirectShipment
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["WebApiDirectShipment"].ToString()) ? "" : ConfigurationManager.AppSettings["WebApiDirectShipment"].ToString();
            }
        }

        public static string WebApiFuelSurCharge
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["WebApiFuelSurCharge"].ToString()) ? "" : ConfigurationManager.AppSettings["WebApiFuelSurCharge"].ToString();
            }
        }

        public static string WebApiExchangeRate
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["WebApiExchangeRate"].ToString()) ? "" : ConfigurationManager.AppSettings["WebApiExchangeRate"].ToString();
            }
        }

        public static string RateCard
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["RateCardTime"].ToString()) ? "" : ConfigurationManager.AppSettings["RateCardTime"].ToString();
            }
        }

        public static string FuelSurCharge
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["FuelSurChargeTime"].ToString()) ? "" : ConfigurationManager.AppSettings["FuelSurChargeTime"].ToString();
            }
        }

        public static string CustomerCurrency
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["CurrencyTime"].ToString()) ? "" : ConfigurationManager.AppSettings["CurrencyTime"].ToString();
            }
        }

        public static string FuelSurChargeDay
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["FuelSurChargeDay"].ToString()) ? "" : ConfigurationManager.AppSettings["FuelSurChargeDay"].ToString();
            }
        }
    }
}
