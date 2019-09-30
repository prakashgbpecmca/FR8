using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;

namespace Frayte.Services.Business
{
    public class TermAndConditionRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public int GetTermAndConditionDetail()
        {
            TermAndCondition result = new TermAndCondition();
            var data = dbContext.TermAndConditions.OrderByDescending(p => p.TermAndConditionId).FirstOrDefault();
            if (data != null)
            {
                result.TermAndConditionId = data.TermAndConditionId;
                result.Detail = data.Detail;
                result.CreatedOn = data.CreatedOn;
            }

            return result.TermAndConditionId;

        }

        public TermAndCondition GetLatestTermAndCondition(int OperationZoneId, string TermAndCondtionType, string shortCode)
        {
            TermAndCondition result = new TermAndCondition();
            var data = (from TAC in dbContext.TermAndConditions
                        where TAC.OperationZoneId == OperationZoneId &&
                              TAC.TermAndConditionType == TermAndCondtionType
                              && TAC.ConpanyCode == (!string.IsNullOrEmpty(shortCode) ? shortCode : null)
                        select new
                        {
                            TAC.TermAndConditionId,
                            TAC.Detail,
                            TAC.CreatedOn,
                            TAC.OperationZoneId,
                            TAC.TermAndConditionType
                        }).OrderByDescending(p => p.TermAndConditionId).FirstOrDefault(); //dbContext.TermAndConditions.OrderByDescending(p => p.TermAndConditionId).FirstOrDefault();
            if (data != null)
            {
                result.TermAndConditionId = data.TermAndConditionId;
                result.Detail = data.Detail;
                result.CreatedOn = data.CreatedOn;
                result.OperationZoneId = data.OperationZoneId;
                result.TermAndConditionType = data.TermAndConditionType;
            }

            return result;

        }

        public TermAndCondition GetTermAndCondition(int id)
        {
            TermAndCondition result = new TermAndCondition();
            var data = dbContext.TermAndConditions.Where(p => p.TermAndConditionId == id).FirstOrDefault();
            if (data != null)
            {
                result.TermAndConditionId = data.TermAndConditionId;
                result.Detail = data.Detail;
                result.CreatedOn = data.CreatedOn;
                result.OperationZoneId = data.OperationZoneId;
                result.TermAndConditionType = data.TermAndConditionType;
            }

            return result;

        }

        public List<TermAndCondition> GetAllTermAndCondition(int OperationZoneId, int userId)
        {
            TermAndCondition result = new TermAndCondition();
            var data = dbContext.TermAndConditions.Where(p => p.TermAndConditionType == "Public" && p.OperationZoneId == OperationZoneId && p.CreatedBy == userId).OrderByDescending(p => p.TermAndConditionId).ToList();
            return data;
        }

        public TermAndCondition SaveTermAndCondition(TermAndCondition termAndCondition)
        {
            //For Term And Condition, we always have to create new term and condition in database.

            string shortCode = string.Empty;
            var customerCompanyDetail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == termAndCondition.CreatedBy).FirstOrDefault();

            if (customerCompanyDetail != null)
            {
                shortCode = customerCompanyDetail.CompanyName.ToLower().Contains("mex") ? "MEX" : "";
            }

            var OperationZone = UtilityRepository.GetOperationZone();

            TermAndCondition newTermAndCondition = new TermAndCondition();
            newTermAndCondition.TermAndConditionId = 0;
            newTermAndCondition.Detail = termAndCondition.Detail;
            newTermAndCondition.CreatedOn = DateTime.UtcNow;
            newTermAndCondition.OperationZoneId = termAndCondition.OperationZoneId;
            newTermAndCondition.TermAndConditionType = termAndCondition.TermAndConditionType;
            newTermAndCondition.CreatedBy = termAndCondition.CreatedBy;
            newTermAndCondition.ConpanyCode = shortCode;
            dbContext.TermAndConditions.Add(newTermAndCondition);
            dbContext.SaveChanges();

            return newTermAndCondition;
        }
    }
}
