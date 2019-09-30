using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.ParcelHub
{
    public static class ParcelHubTestRequestUrl
    {
        public static string TestStartXmlTag = "http://api.parcelhub.net/schemas/api/parcelhub-api-v0.4.xsd";
        public static string TestAuthenticationUrl = "https://api.test.parcelhub.net/1.0/token";
        public static string TestGetServicesUrl = "https://api.test.parcelhub.net/1.0/Service?AccountId";
        public static string TestFinalUrl = "https://api.test.parcelhub.net/1.0/Shipment?RequestedLabelSize=6&RequestedLabelFormat=PNG";
    }

    public static class ParcelHubLiveRequestUrl
    {
        public static string LiveStartXmlTag = "http://api.parcelhub.net/schemas/api/parcelhub-api-v0.4.xsd";
        public static string LiveAuthenticationUrl = "https://api.parcelhub.net/1.0/token";
        public static string LiveGetServicesUrl = "https://api.parcelhub.net/1.0/Service?AccountId";
        public static string LiveFinalUrl = "https://api.parcelhub.net/1.0/Shipment?RequestedLabelSize=6&RequestedLabelFormat=PNG";
    }

    public class TokenValue
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }
        public string issued { get; set; }
        public string expires { get; set; }
    }

    public class GetServices
    {
        public GetServicesResponse GetServicesResponse { get; set; }
    }

    public class GetServicesResponse
    {
        public Services Services { get; set; }
        public UnsuitableServiceProviders UnsuitableServiceProviders { get; set; }
    }

    public class Services
    {
        public List<Service> Service { get; set; }
    }

    public class Service
    {
        public ServiceIds ServiceIds { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDesc { get; set; }
        public string ServiceProviderName { get; set; }
        public ServiceCountryCodes ServiceCountryCodes { get; set; }
        public AllowableDimensions AllowableDimensions { get; set; }
        public bool MultiPackages { get; set; }
        public bool TimedService { get; set; }
        public bool WeekendService { get; set; }
        public bool BusinessAddresses { get; set; }
        public bool ResidentialAddresses { get; set; }
        public bool ServiceOutOfArea { get; set; }
        public AvailableLabelSpec AvailableLabelSpec { get; set; }
    }

    public class ServiceIds
    {
        public string ServiceId { get; set; }
        public string ServiceCustomerUID { get; set; }
        public string ServiceProviderId { get; set; }
    }

    public class ServiceCountryCodes
    {
        public string ServiceCountryCode { get; set; }
    }

    public class AllowableDimensions
    {
        public decimal MinWeight { get; set; }
        public decimal MaxWeight { get; set; }
        public decimal MinVolume { get; set; }
        public decimal MaxVolume { get; set; }
        public decimal MaxLength { get; set; }
        public decimal MaxGirth { get; set; }
    }

    public class AvailableLabelSpec
    {
        public List<LabelSpec> LabelSpec { get; set; }
    }

    public class LabelSpec
    {
        public string LabelSize { get; set; }
        public string LabelFormat { get; set; }
    }

    public class UnsuitableServiceProviders
    {
        public List<UnsuitableServiceProvider> UnsuitableServiceProvider { get; set; }
    }

    public class UnsuitableServiceProvider
    {
        public string ServiceProviderId { get; set; }
        public string UnsuitableServiceProviderMessage { get; set; }
        public string Reason { get; set; }
    }

    public class ParcelResponse
    {
        public List<ImageData> Bytes { get; set; }
        public CourierTrackingNumber CourierNumber { get; set; }
        public List<PackageTrackingNumber> PackageNumber { get; set; }
    }

    public class ImageData
    {
        public string ImageBytes { get; set; }
    }

    public class CourierTrackingNumber
    {
        public string TrackingNumber { get; set; }
    }

    public class PackageTrackingNumber
    {
        public string CourierTrackingNumber { get; set; }
    }
}
