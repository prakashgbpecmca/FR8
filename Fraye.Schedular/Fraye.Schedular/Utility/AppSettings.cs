using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Schedular.Utility
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
        
        public static string ErrorLoger
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ErrorLoger"].ToString()) ? "" : ConfigurationManager.AppSettings["ErrorLoger"].ToString();
            }
        }
        public static string EmailSendTime
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailSendTime"].ToString()) ? "" : ConfigurationManager.AppSettings["EmailSendTime"].ToString();
            }
        }
        

        public static string HKEmail
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["HKEmail"].ToString()) ? "" : ConfigurationManager.AppSettings["HKEmail"].ToString();
            }
        }

        public static string UKEmail
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UKEmail"].ToString()) ? "" : ConfigurationManager.AppSettings["UKEmail"].ToString();
            }
        }

        public static string ManifestFolder
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ManifestFolderPath"].ToString()) ? "" : ConfigurationManager.AppSettings["ManifestFolderPath"].ToString();
            }
        }

        public static string ReportFolder
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReportFolder"].ToString()) ? "" : ConfigurationManager.AppSettings["ReportFolder"].ToString();
            }
        }

        public static string EmailServicePath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailServicePath"].ToString()) ? "" : ConfigurationManager.AppSettings["EmailServicePath"].ToString();
            }
        }
        public static string ShipmentCreatedFrom
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ShipmentCreatedFrom"].ToString()) ? "" : ConfigurationManager.AppSettings["ShipmentCreatedFrom"].ToString();
                }
                catch (Exception ex)
                {
                    return "";
                }

            }
        }
        public static string UploadFolderPath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UploadFolderPath"].ToString()) ? "" : ConfigurationManager.AppSettings["UploadFolderPath"].ToString();
            }
        }
    }
}
