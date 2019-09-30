using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Frayte.Services.Utility
{
    public static class AppSettings
    {
        public static string ProfileImagePath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ProfileImagePath"].ToString()) ? "" : ConfigurationManager.AppSettings["ProfileImagePath"].ToString();
            }
        }

        public static string eCommerceUploadShipmentLabelFolder
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["eCommerceUploadShipmentLabelFolder"].ToString()) ? "" : ConfigurationManager.AppSettings["eCommerceUploadShipmentLabelFolder"].ToString();
            }
        }

        public static string eCommerceUploadLabelFolder
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["eCommerceUploadLabelFolder"].ToString()) ? "" : ConfigurationManager.AppSettings["eCommerceUploadLabelFolder"].ToString();
            }
        }

        public static string eCommerceUploadShipmentBatchProcess
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["eCommerceUploadShipmentBatchProcess"].ToString()) ? "" : ConfigurationManager.AppSettings["eCommerceUploadShipmentBatchProcess"].ToString();
            }
        }

        public static string ControllerDeptEmail
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ControllerDeptEmail"].ToString()) ? "" : ConfigurationManager.AppSettings["ControllerDeptEmail"].ToString();
            }
        }
        public static string ControllerDeptName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ControllerDeptName"].ToString()) ? "" : ConfigurationManager.AppSettings["ControllerDeptName"].ToString();
            }
        }

        public static string ControlDeptFolderPath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ControlDeptFolderPath"].ToString()) ? "" : ConfigurationManager.AppSettings["ControlDeptFolderPath"].ToString();
            }
        }

        public static string eCommerceManifestBatchProcess
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["eCommerceManifestBatchProcess"].ToString()) ? "" : ConfigurationManager.AppSettings["eCommerceManifestBatchProcess"].ToString();
            }
        }
        public static string PaymentUrl
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["PaymentUrl"].ToString()) ? "" : ConfigurationManager.AppSettings["PaymentUrl"].ToString();
            }
        }
        public static string ManifestFolderPath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ManifestFolderPath"].ToString()) ? "" : ConfigurationManager.AppSettings["ManifestFolderPath"].ToString();
            }
        }

        public static string ApplicationMode
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ApplicationMode"].ToString()) ? "" : ConfigurationManager.AppSettings["ApplicationMode"].ToString();
            }
        }

        public static string HostName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["HostName"].ToString()) ? "" : ConfigurationManager.AppSettings["HostName"].ToString();
            }
        }
        public static string TrackingUrl
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["TrackUrl"].ToString()) ? "" : ConfigurationManager.AppSettings["TrackUrl"].ToString();
            }
        }
        public static string TrackingUrlHKG
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["TrackUrlHKG"].ToString()) ? "" : ConfigurationManager.AppSettings["TrackUrl"].ToString();
            }
        }

        public static string WebApiPath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["WebApiPath"].ToString()) ? "" : ConfigurationManager.AppSettings["WebApiPath"].ToString();
            }
        }


        public static string LabelVirtualPath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["LabelVirtualPath"].ToString()) ? "" : ConfigurationManager.AppSettings["LabelVirtualPath"].ToString();
            }
        }

        public static string LabelSave
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["LabelReadFrom"].ToString()) ? "" : ConfigurationManager.AppSettings["LabelReadFrom"].ToString();
            }
        }

        public static string MailHostName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["MailHostName"].ToString()) ? "" : ConfigurationManager.AppSettings["MailHostName"].ToString();
            }
        }

        public static string SMTPport
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTPport"].ToString()) ? "" : ConfigurationManager.AppSettings["SMTPport"].ToString();
            }
        }

        public static string SMTPUserName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTPUserName"].ToString()) ? "" : ConfigurationManager.AppSettings["SMTPUserName"].ToString();
            }
        }

        public static string SMTPPassword
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTPPassword"].ToString()) ? "" : ConfigurationManager.AppSettings["SMTPPassword"].ToString();
            }
        }

        public static string FromMail
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["FromMail"].ToString()) ? "" : ConfigurationManager.AppSettings["FromMail"].ToString();
            }
        }

        public static string EnableSsl
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableSsl"].ToString()) ? "" : ConfigurationManager.AppSettings["EnableSsl"].ToString();
            }
        }

        public static string MailSentTo
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["MailSentTo"].ToString()) ? "" : ConfigurationManager.AppSettings["MailSentTo"].ToString();
            }
        }

        public static string BCC
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["BCC"].ToString()) ? "" : ConfigurationManager.AppSettings["BCC"].ToString();
            }
        }

        public static string EmailServicePath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailServicePath"].ToString()) ? "" : ConfigurationManager.AppSettings["EmailServicePath"].ToString();
            }
        }

        public static string TOCC
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["TOCC"].ToString()) ? "" : ConfigurationManager.AppSettings["TOCC"].ToString();
            }
        }
        public static string EasyPostKey
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["EasyPostKey"].ToString()) ? "" : ConfigurationManager.AppSettings["EasyPostKey"].ToString();
            }
        }

        public static string OperationZone
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["OpearationZone"].ToString()) ? "" : ConfigurationManager.AppSettings["OpearationZone"].ToString();
            }
        }

        public static string UserName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ParcelHubUserName"].ToString()) ? "" : ConfigurationManager.AppSettings["ParcelHubUserName"].ToString();
            }
        }

        public static string Password
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ParcelHUbPassword"].ToString()) ? "" : ConfigurationManager.AppSettings["ParcelHUbPassword"].ToString();
            }
        }

        public static string LabelFolder
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["LabelFolder"].ToString()) ? "" : ConfigurationManager.AppSettings["LabelFolder"].ToString();
            }
        }

        public static string eCommerceLabelFolder
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["eCommerceLabelFolder"].ToString()) ? "" : ConfigurationManager.AppSettings["eCommerceLabelFolder"].ToString();
            }
        }
        public static string eCommerceShipmentLabel
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["eCommerceShipmentLabel"].ToString()) ? "" : ConfigurationManager.AppSettings["eCommerceShipmentLabel"].ToString();
            }
        }
        public static string eCommerceManifest
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["eCommerceManifest"].ToString()) ? "" : ConfigurationManager.AppSettings["eCommerceManifest"].ToString();
            }
        }
        public static string eCommerceBag
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["eCommerceBag"].ToString()) ? "" : ConfigurationManager.AppSettings["eCommerceBag"].ToString();
            }
        }
        public static string ParcelhubAPIIntegrationURL
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ParcelhubAPIBaseURL"].ToString()) ? "" : ConfigurationManager.AppSettings["ParcelhubAPIBaseURL"].ToString();
            }
        }

        public static string UserAgent
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UserAgent"].ToString()) ? "" : ConfigurationManager.AppSettings["UserAgent"].ToString();
            }
        }

        public static string ParcelHubEmail
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ParcelHubEmail"].ToString()) ? "" : ConfigurationManager.AppSettings["ParcelHubEmail"].ToString();
            }
        }

        public static string EasyPostEmail
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["EasyPostEmail"].ToString()) ? "" : ConfigurationManager.AppSettings["EasyPostEmail"].ToString();
            }
        }

        public static string StripeApiKey
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["StripeApiKey"].ToString()) ? "" : ConfigurationManager.AppSettings["StripeApiKey"].ToString();
            }
        }

        public static string ReportFolder
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReportFolder"].ToString()) ? "" : ConfigurationManager.AppSettings["ReportFolder"].ToString();
            }
        }

        public static string eCommerceReportFolder
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["eCommerceReportFolder"].ToString()) ? "" : ConfigurationManager.AppSettings["eCommerceReportFolder"].ToString();
            }
        }
        public static string HKHostName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["HKHostName"].ToString()) ? "" : ConfigurationManager.AppSettings["HKHostName"].ToString();
            }
        }

        public static string HKport
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["HKport"].ToString()) ? "" : ConfigurationManager.AppSettings["HKport"].ToString();
            }
        }

        public static string HKName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["HKUserName"].ToString()) ? "" : ConfigurationManager.AppSettings["HKUserName"].ToString();
            }
        }

        public static string HKPassword
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["HKPassword"].ToString()) ? "" : ConfigurationManager.AppSettings["HKPassword"].ToString();
            }
        }

        public static string HKFromMail
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["HKFromMail"].ToString()) ? "" : ConfigurationManager.AppSettings["HKFromMail"].ToString();
            }
        }

        public static string HKEnableSsl
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["HKEnableSsl"].ToString()) ? "" : ConfigurationManager.AppSettings["HKEnableSsl"].ToString();
            }
        }

        public static string HKBCC
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["HKBCC"].ToString()) ? "" : ConfigurationManager.AppSettings["HKBCC"].ToString();
            }
        }

        public static string UKHostName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UKHostName"].ToString()) ? "" : ConfigurationManager.AppSettings["UKHostName"].ToString();
            }
        }

        public static string UKport
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UKport"].ToString()) ? "" : ConfigurationManager.AppSettings["UKport"].ToString();
            }
        }

        public static string UKName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UKUserName"].ToString()) ? "" : ConfigurationManager.AppSettings["UKUserName"].ToString();
            }
        }

        public static string UKPassword
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UKPassword"].ToString()) ? "" : ConfigurationManager.AppSettings["UKPassword"].ToString();
            }
        }

        public static string UKFromMail
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UKFromMail"].ToString()) ? "" : ConfigurationManager.AppSettings["UKFromMail"].ToString();
            }
        }

        public static string UKEnableSsl
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UKEnableSsl"].ToString()) ? "" : ConfigurationManager.AppSettings["UKEnableSsl"].ToString();
            }
        }

        public static string UKBCC
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UKBCC"].ToString()) ? "" : ConfigurationManager.AppSettings["UKBCC"].ToString();
            }
        }

        public static string DisplayName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTPDisplayName"].ToString()) ? "" : ConfigurationManager.AppSettings["SMTPDisplayName"].ToString();
            }
        }

        public static string HKDisplayName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["HKDisplayName"].ToString()) ? "" : ConfigurationManager.AppSettings["HKDisplayName"].ToString();
            }
        }

        public static string UKDisplayName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UKDisplayName"].ToString()) ? "" : ConfigurationManager.AppSettings["UKDisplayName"].ToString();
            }
        }

        public static string HKUrl
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["HKUrl"].ToString()) ? "" : ConfigurationManager.AppSettings["HKUrl"].ToString();
            }
        }

        public static string UKUrl
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UKUrl"].ToString()) ? "" : ConfigurationManager.AppSettings["UKUrl"].ToString();
            }
        }

        public static string QuotationImages
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["QuotationImagePath"].ToString()) ? "" : ConfigurationManager.AppSettings["QuotationImagePath"].ToString();
            }
        }

        public static string ManifestFolder
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ManifestFolderPath"].ToString()) ? "" : ConfigurationManager.AppSettings["ManifestFolderPath"].ToString();
            }
        }

        // TNT Keys
        public static string TNTUserName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["TNTUserName"].ToString()) ? "" : ConfigurationManager.AppSettings["TNTUserName"].ToString();
            }
        }
        public static string TNTPassword
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["TNTPassword"].ToString()) ? "" : ConfigurationManager.AppSettings["TNTPassword"].ToString();
            }
        }
        public static string TNTAppId
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["TNTAppId"].ToString()) ? "" : ConfigurationManager.AppSettings["TNTAppId"].ToString();
            }
        }
        public static string TNTAppVersion
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["TNTAppVersion"].ToString()) ? "" : ConfigurationManager.AppSettings["TNTAppVersion"].ToString();
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

        public static string FilePath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["FilePath"].ToString()) ? "" : ConfigurationManager.AppSettings["FilePath"].ToString();
            }
        }

        public static string AssetFolderPath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["AssetsFolderPath"].ToString()) ? "" : ConfigurationManager.AppSettings["AssetsFolderPath"].ToString();
            }
        }

        public static string ClientFolderPath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ClientFolderPath"].ToString()) ? "" : ConfigurationManager.AppSettings["ClientFolderPath"].ToString();
            }
        }

        public static string PrinterName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["PrinterName"].ToString()) ? "" : ConfigurationManager.AppSettings["PrinterName"].ToString();
            }
        }
        public static string UploadFolderPath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["UploadFolderPath"].ToString()) ? "" : ConfigurationManager.AppSettings["UploadFolderPath"].ToString();
            }
        }

        public static string AWBImagePath
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["AWBImagePath"].ToString()) ? "" : ConfigurationManager.AppSettings["AWBImagePath"].ToString();
            }
        }
    }
}