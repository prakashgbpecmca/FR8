using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.SKYPOSTAL
{
    public class SkyPostalTrackingModel
    {
        public string Key { get; set; }
        public string method { get; set; }
        public string extr_nmr { get; set; }
        public int copa_id { get; set; }
        public string lang_cdg { get; set; }


    }
    public class TrackingResponse
    {
        public string TRDE_FCH_USER { get; set; }
        public string TPSH_CDG { get; set; }
        public string SHIP_MERCHANT_NAME { get; set; }
        public string SHIP_CONTENT { get; set; }
        public decimal SHIP_PRICE_VALUE { get; set; }
        public decimal SHIP_QUANTITY_PIECE { get; set; }
        public string CTRY_NAME { get; set; }
        public string CITY_NAME { get; set; }
        public string TRCK_NMR_FOL { get; set; }
        public decimal SHIP_CHARGE_WEIGHT { get; set; }
        public decimal SHIP_PHYSICAL_WEIGHT { get; set; }
        public decimal SHIP_VOLUMETRIC_WEIGHT { get; set; }
        public decimal SHIP_SUBTOTAL_CUSTOM { get; set; }
        public DateTime CUSTOM_DATE_PROCESS { get; set; }
        public DateTime SHIP_DATE_PROCESS { get; set; }
        public DateTime SHIP_DATE_CHARGE { get; set; }
        public decimal SHIP_RATE { get; set; }
        public decimal SHIP_DISCOUNT { get; set; }
        public decimal SHIP_INSURANCE_VALUE { get; set; }
        public decimal SHIP_TOTAL { get; set; }
        public string SHIP_SWT_TYPE_CHARGE_WEIGTH { get; set; }
        public string STATUS { get; set; }
        public string LOCALITY { get; set; }
        public string CHCK_CDG { get; set; }
        public string TRDE_OBS { get; set; }
        public string EXT_TRACK { get; set; }
        public string CHCK_ORDER { get; set; }
        public string TRCK_ID_UNIQUE { get; set; }

    }
    public class SkyPostalTrackingResponseModel
    {
        public int success { get; set; }
        public List<TrackingResponse> response { get; set; }
    }
}

