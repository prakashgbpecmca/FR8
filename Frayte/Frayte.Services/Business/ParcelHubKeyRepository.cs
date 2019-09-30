using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class ParcelHubKeyRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public List<APIKeyDetail> GetParcelHubApiKeys()
        {
            var list = dbContext.APIKeyDetails.ToList();

            return list;

        }

        public FrayteResult SaveParcelHubKey(ParcelHubKeyModel ParcelHubKey)
        {
            FrayteResult result = new FrayteResult();
            APIKeyDetail parcelHubKey;

            if (ParcelHubKey.APIId == 0)
            {
                parcelHubKey = new APIKeyDetail();
                parcelHubKey.APIKey = ParcelHubKey.APIKey;
                parcelHubKey.APIName = ParcelHubKey.APIName;
                dbContext.APIKeyDetails.Add(parcelHubKey);
                dbContext.SaveChanges();
                result.Status = true;
            }
            else
            {
                parcelHubKey = dbContext.APIKeyDetails.Find(ParcelHubKey.APIId);
                if (parcelHubKey != null)
                {
                    parcelHubKey.APIKey = ParcelHubKey.APIKey;
                    parcelHubKey.APIName = ParcelHubKey.APIName;
                    dbContext.SaveChanges();
                    result.Status = true;
                }
                else
                {
                    result.Status = false;
                }
            }

            return result;
        }

        public FrayteResult DeleteParcelHubKey(int APIId)
        {
            FrayteResult result = new FrayteResult();
            APIKeyDetail parcelHubKey;
            try
            {
                parcelHubKey = dbContext.APIKeyDetails.Find(APIId);
                if (parcelHubKey != null)
                {
                    dbContext.APIKeyDetails.Remove(parcelHubKey);
                    dbContext.SaveChanges();
                    result.Status = true;
                }
                else
                {
                    result.Status = false;
                }
            }
            catch(Exception e)
            {
                result.Status = false;
            }
            return result;
        }
    }
}
