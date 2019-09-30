using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.SKYPOSTAL
{
    public class SkyPostalRequestModel
    {
        public string Key { get; set; }
        public string method { get; set; }
        public string include_label_data { get; set; }
        public string include_label_zpl { get; set; }
        public string zpl_encode_base64 { get; set; }
        public string include_label_base64_image { get; set; }
        public string label_pdf_rotate { get; set; }
        public Header header { get; set; }
        public List<PackageDetail> detail { get; set; }
    }

    public class Header
    {
        public string EXTR_TRACKING { get; set; }
        public string COUNTRY_CODE { get; set; }
        public int STATE_CODE { get; set; }
        public int CITY_CODE { get; set; }
        public object STATE_NAME { get; set; }
        public object CITY_NAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string ADDRESS_CONSIGNEE { get; set; }
        public string ADDRESS2 { get; set; }
        public string ZIPCODE { get; set; }
        public string PHONE { get; set; }
        public string MOBILE_PHONE { get; set; }
        public string EMAIL { get; set; }
        public string ID_NUMBER { get; set; }
        public string MERCHANT_NAME { get; set; }
        public int MERCHANT_NUMBER { get; set; }
        public string MERCHANT_BOX { get; set; }
        public object MERCHANT_CS_EMAIL { get; set; }
        public object MERCHANT_RETURN_ADDRESS { get; set; }
        public string MERCHANT_CS_NAME { get; set; }
        public string ORDER_NUMBER { get; set; }
        public decimal ORDER_AMOUNT { get; set; }
        public string ORDER_DATE { get; set; }
        public string INTERNAL_NUMBER { get; set; }
        public string MANIFEST_TYPE { get; set; }
        public int CONSOLIDATED { get; set; }
        public string CURRENCY_ISO_CODE { get; set; }
        public int SHIPMENT_FREIGHT { get; set; }
        public int SHIPMENT_INSURANCE { get; set; }
        public int SHIPMENT_DISCOUNT { get; set; }
    }

    public class PackageDetail
    {
        public string HSC { get; set; }
        public string FMPR_CDG { get; set; }
        public string CONTENT_OF_PRODUCT { get; set; }
        public decimal PHYSICAL_WEIGHT { get; set; }
        public string WEIGHT_TYPE { get; set; }
        public decimal DIMEN_LENGTH { get; set; }
        public decimal DIMEN_HEIGHT { get; set; }
        public decimal DIMEN_WIDTH { get; set; }
        public string DIMEN_UNIT { get; set; }
        public decimal MERCHANDISE_VALUE { get; set; }
        public int QUANTITY { get; set; }
    }

    //SkyPostal New API

    public class SkyPostalRequest
    {
        public UserInfo user_info { get; set; }
        public ShipmentInfo shipment_info { get; set; }
    }

    public class UserInfo
    {
        public int user_code { get; set; }
        public string user_key { get; set; }
        public string app_key { get; set; }
    }

    public class ShipmentInfo
    {
        public int copa_id { get; set; }
        public int ssa_copa_id { get; set; }
        public string box_id { get; set; }
        public Merchant merchant { get; set; }
        public Shipper shipper { get; set; }
        public Sender sender { get; set; }
        public Consignee consignee { get; set; }
        public Options options { get; set; }
        public Data data { get; set; }
    }


    public class Merchant
    {
        public string name { get; set; }
        public string email { get; set; }
        public MerchantAddress address { get; set; }
        public ReturnAddress return_address { get; set; }
        public List<MerchantPhone> phone { get; set; }
    }

    public class Shipper
    {
        public string name { get; set; }
        public string email { get; set; }
        public ShipperAddress shipperaddress { get; set; }
        public ReturnAddress return_address { get; set; }
        public List<ShipperPhone> phone { get; set; }
    }

    public class Sender
    {
        public string name { get; set; }
        public string email { get; set; }
        public SenderAddress SenderAddress { get; set; }
        public ReturnAddress return_address { get; set; }
        public List<ShipperPhone> phone { get; set; }
    }

    public class Consignee
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string id_number { get; set; }
        public ConsigneeAddress address { get; set; }
        public List<ConsigneePhone> phone { get; set; }
    }

    public class Options
    {
        public bool include_label_data { get; set; }
        public bool include_label_zpl { get; set; }
        public bool zpl_encode_base64 { get; set; }
        public bool include_label_image { get; set; }
        public string manifest_type { get; set; }
        public int insurance_code { get; set; }
        public bool generate_label_default { get; set; }
        public int rate_service_code { get; set; }

    }

    public class Data
    {
        public string external_tracking { get; set; }
        public string reference_date { get; set; }
        public string reference_number_01 { get; set; }
        public string reference_number_02 { get; set; }
        public string reference_number_03 { get; set; }
        public double tax { get; set; }
        public decimal value { get; set; }
        public double discount { get; set; }
        public double freight { get; set; }
        public string currency_iso_code { get; set; }
        public decimal dimension_01 { get; set; }
        public decimal dimension_02 { get; set; }
        public decimal dimension_03 { get; set; }
        public int insurance { get; set; }
        public string dimension_unit { get; set; }
        public decimal weight { get; set; }
        public string weight_unit { get; set; }
        public List<Item> items { get; set; }
    }


    public class SenderAddress
    {
        public int country_code { get; set; }
        public string country_iso_code { get; set; }
        public string country_name { get; set; }
        public int state_code { get; set; }
        public string state_name { get; set; }
        public int county_code { get; set; }
        public string county_name { get; set; }
        public int city_code { get; set; }
        public string city_name { get; set; }
        public string zip_code { get; set; }
        public string neighborhood { get; set; }
        public string address_01 { get; set; }
        public string address_02 { get; set; }
        public string address_03 { get; set; }
    }

    public class ShipperAddress
    {
        public int country_code { get; set; }
        public string country_iso_code { get; set; }
        public string country_name { get; set; }
        public int state_code { get; set; }
        public string state_name { get; set; }
        public int county_code { get; set; }
        public string county_name { get; set; }
        public int city_code { get; set; }
        public string city_name { get; set; }
        public string zip_code { get; set; }
        public string neighborhood { get; set; }
        public string address_01 { get; set; }
        public string address_02 { get; set; }
        public string address_03 { get; set; }
    }

    public class ShipperPhone
    {
        public int phone_type { get; set; }
        public string phone_number { get; set; }
    }

    public class ConsigneeAddress
    {
        public int country_code { get; set; }
        public string country_iso_code { get; set; }
        public string country_name { get; set; }
        public int state_code { get; set; }
        public string state_name { get; set; }
        public int county_code { get; set; }
        public string county_name { get; set; }
        public int city_code { get; set; }
        public string city_name { get; set; }
        public string zip_code { get; set; }
        public string neighborhood { get; set; }
        public string address_01 { get; set; }
        public string address_02 { get; set; }
        public string address_03 { get; set; }
    }

    public class ConsigneePhone
    {
        public int phone_type { get; set; }
        public string phone_number { get; set; }
    }

    public class Item
    {
        public string hs_code { get; set; }
        public string family_product { get; set; }
        public string serial_number { get; set; }
        public string imei_number { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public double tax { get; set; }
        public decimal value { get; set; }
        public decimal weight { get; set; }
    }

    public class MerchantAddress
    {
        public int country_code { get; set; }
        public string country_iso_code { get; set; }
        public string country_name { get; set; }
        public int state_code { get; set; }
        public string state_name { get; set; }
        public int county_code { get; set; }
        public string county_name { get; set; }
        public int city_code { get; set; }
        public string city_name { get; set; }
        public string zip_code { get; set; }
        public string neighborhood { get; set; }
        public string address_01 { get; set; }
        public string address_02 { get; set; }
        public string address_03 { get; set; }
    }

    public class ReturnAddress
    {
        public int country_code { get; set; }
        public string country_iso_code { get; set; }
        public string country_name { get; set; }
        public int state_code { get; set; }
        public string state_name { get; set; }
        public int county_code { get; set; }
        public string county_name { get; set; }
        public int city_code { get; set; }
        public string city_name { get; set; }
        public string zip_code { get; set; }
        public string neighborhood { get; set; }
        public string address_01 { get; set; }
        public string address_02 { get; set; }
        public string address_03 { get; set; }
    }

    public class MerchantPhone
    {
        public int phone_type { get; set; }
        public string phone_number { get; set; }
    }

    // response 

    public class Server
    {
        public object server_id { get; set; }
        public double server_time { get; set; }
    }

    public class AdditionalInfo
    {
        public List<object> @internal { get; set; }
        public Server server { get; set; }
    }

    public class Server2
    {
        public object server_id { get; set; }
        public int server_time { get; set; }
    }

    public class AdditionalInfo2
    {
        public List<object> @internal { get; set; }
        public Server2 server { get; set; }
    }

    public class LabelData
    {
        public object additional_info { get; set; }
        public object consignee { get; set; }
        public object destination { get; set; }
        public object error { get; set; }
        public object origin { get; set; }
        public object provider_service_type { get; set; }
        public object provider_settings { get; set; }
    }

    public class Datum
    {
        public bool _verify { get; set; }
        public AdditionalInfo2 additional_info { get; set; }
        public List<Error> error { get; set; }
        public LabelData label_data { get; set; }
        public object label_image { get; set; }
        public string label_invoice_url { get; set; }
        public string label_tracking_number_01 { get; set; }
        public string label_tracking_number_02 { get; set; }
        public object label_tracking_number_03 { get; set; }
        public string label_url { get; set; }
        public string label_url_pdf { get; set; }
        public string label_zpl { get; set; }
        public int? trck_nmr_fol { get; set; }
    }

    public class Error
    {
        public string error_description { get; set; }
        public string error_location { get; set; }
        public bool system_error { get; set; }
    }

    public class SkyPostalResponse
    {
        public AdditionalInfo additional_info { get; set; }
        public List<Datum> data { get; set; }
        public Error error { get; set; }
    } 
}
