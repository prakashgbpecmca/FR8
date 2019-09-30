using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Models.BreakBulk;
using Frayte.Services.Models.Tradelane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    [AllowAnonymous]
    public class BreakBulkController : ApiController
    {
        #region GetInitial Call

        [HttpGet]
        public HttpResponseMessage GetInitials(int userId)
        {
            //Get Country list
            List<FrayteCountryCode> lstCountry = new List<FrayteCountryCode>();
            lstCountry = new CountryRepository().lstCountry();

            //Get Shipment Hanler Methods
            var lstShipmentHandlerMethods = new BreakBulkRepository().GetShipmentHandlerMethod();

            //Get Country PhoneCode
            List<FrayteCountryPhoneCode> lstCountryPhones = new List<FrayteCountryPhoneCode>();

            lstCountryPhones = new CountryRepository().GetCountryPhoneCodeList();

            //Get all Incoterm
            var lstIncoterm = new BreakBulkRepository().GetIncoterms();

            //Get all PaymentParty
            List<TradelaneCustomer> lstCustomer = new List<TradelaneCustomer>();
            lstCustomer = new BreakBulkRepository().GetCustomers(userId);

            //Get all Factory
            var lstFactory = new BreakBulkRepository().GetFactories();

            //Get all Custom field
            var lstCustomField = new BreakBulkRepository().GetCustomField(userId);

            return this.Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    Countries = lstCountry,
                    ShipmentMethods = lstShipmentHandlerMethods,
                    CountryPhoneCodes = lstCountryPhones,
                    Incoterm = lstIncoterm,
                    Factories = lstFactory,
                    PaymentParty = lstCustomer,
                    CustomFields = lstCustomField
                });
        }
        #endregion

        #region PlaceOrder

        [HttpPost]
        public BreakBulkShipmentDetail SavePurchaseOrderData(BreakBulkShipmentDetail shipment)
        {
            BreakBulkShipmentDetail model = new BreakBulkRepository().SavePurchaseOrderData(shipment);
            return model;
        }

        [HttpPost]
        public List<HubService> BreakBulkHubServices(BreakBulkServiceModel serviceObj)
        {
            List<HubService> services = new BreakBulkRepository().GetBreakBulkServices(serviceObj);
            return services;
        }

        [HttpGet]
        public IHttpActionResult GetHubAddress(int countryId, string postcode, string state)
        {
            try
            {
                return Ok(new BreakBulkRepository().GetHubAddress(countryId, postcode, state));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IHttpActionResult AirPortList(int countryId)
        {
            try
            {
                var listAirlines = new BreakBulkRepository().GetAirlines(countryId);
                return Ok(listAirlines);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IHttpActionResult GenerateConsignmentNumber(int userId)
        {
            var consignmentNumber = new BreakBulkRepository().GenerateConsignmentNumber(userId);
            return Ok(consignmentNumber);
        }

        #endregion

        #region CustomField

        [HttpPost]
        public IHttpActionResult SaveCustomerCustomField(customerCustomFieldModel CustomField)
        {
            return Ok(new BreakBulkRepository().SaveCustomerCustomField(CustomField));
        }

        #endregion

        #region DefaultShipmentType

        [HttpPost]
        public bool SaveCustomerShipmentType(int customerId, string DefaultShipmentType)
        {
            return new BreakBulkRepository().SaveCustomerShipmentType(customerId, DefaultShipmentType);
        }

        #endregion

        #region POJOb View

        [HttpGet]
        public BreakBulkShipmentDetail GetBreakBulkBookingDetail(int PurchaseOrderId)
        {
            return new BreakBulkRepository().GetBreakBulkBookingDetail(PurchaseOrderId); ;
        }

        [HttpPost]
        public List<FraytePOviewPurchaseOrder> GetPOPurchaseOrderD(FraytePurchaseOrderTrack track)
        {
            return new BreakBulkRepository().TrackPOPurchaseOrder(track);
        }

        [HttpPost]
        public List<FrayteJobViewPurchaseOrder> GetJobPurchaseOrderD(FraytePurchaseOrderTrack track)
        {
            return new BreakBulkRepository().TrackJobPurchaseOrder(track);
        }

        #endregion

        #region POJobStatus

        [HttpGet]
        public List<BBKShipmentStatus> GetBBKShipmentStatusList(string BookingType)
        {
            return new BreakBulkRepository().GetBBKShipmentStatusList(BookingType);
        }

        #endregion
        
    }
}
