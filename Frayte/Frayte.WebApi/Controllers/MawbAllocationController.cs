using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Models.Tradelane;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    public class MawbAllocationController : ApiController
    {
        [HttpPost]
        public FratyteError SaveMawbAllocation(List<MawbAllocationModel> MAList)
        {
            var File = new TradelaneShipmentsController().CreateDocument(MAList.FirstOrDefault().TradelaneId, 0, FrayteTradelaneShipmentDocumentEnum.CoLoadForm, "");
            var FilePath = AppSettings.UploadFolderPath + "/Tradelane" + "/" + MAList.FirstOrDefault().TradelaneId + "/" + File.FileName;
            if (File.FileName != null && File.FileName != "")
            {

            }
            else
            {
                FilePath = "";
            }
            return new MawbAllocationRepository().SaveMawbAllocation(MAList, FilePath, "Tradelane");
        }

        [HttpGet]
        public List<MawbAllocationModel> GetMawbAllocation(int TradelaneShipmentId)
        {
            return new MawbAllocationRepository().GetMawbAllocation(TradelaneShipmentId, "");
        }

        [HttpGet]
        public List<DirectBookingCustomer> GetAgents()
        {
            return new MawbAllocationRepository().GetAgents();
        }

        [HttpGet]
        public string GetMawbDocumentName(int TradelaneShipmentId)
        {
            return new MawbAllocationRepository().GetMawbDocumentName(TradelaneShipmentId);
        }

        [HttpGet]
        public ShipmentRoute GetShipmentHandlerId(int TradelaneshipmentId)
        {
            return new MawbAllocationRepository().GetShipmentHandlerId(TradelaneshipmentId);
        }

        [HttpGet]
        public FrayteResult DeleteMawbAllocation(int AllocationId)
        {
            return new MawbAllocationRepository().DeleteMawbAllocation(AllocationId);
        }

        [HttpGet]
        public List<TradelaneAirline> GetAirlines()
        {
            return new MasterDataRepository().GetAirlines();
        }

        [HttpGet]
        public string GetTimeZoneName(int TradelaneShipmentId)
        {
            return new MawbAllocationRepository().GetTimeZoneName(TradelaneShipmentId);
        }
    }
}
