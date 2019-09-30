using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;

namespace Frayte.Services.Utility
{
    public static class FrayteSurchargeRule
    {
        //public static bool WeightSurcharge(DirectBookingService WeightFrom, LogisticServiceSurchargeDetail surchargeDetail)
        //{
        //    switch (surchargeDetail.Condition)
        //    {
        //        case "Greater":
        //        //if (WeightFrom.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
        //            return WeightFrom.Weight > decimal.Parse(surchargeDetail.Unit);
        //        //else if (WeightFrom.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
        //        //    return (WeightFrom.Weight * 0.453592m) > decimal.Parse(surchargeDetail.Unit);
        //        case "GreaterEqual":
        //        //if (WeightFrom.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
        //            return WeightFrom.Weight >= decimal.Parse(surchargeDetail.Unit);
        //        //else if (WeightFrom.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
        //        //    return (WeightFrom.Weight * 0.453592m) >= decimal.Parse(surchargeDetail.Unit);
        //        case "Lesser":
        //        //if (WeightFrom.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
        //            return WeightFrom.Weight < decimal.Parse(surchargeDetail.Unit);
        //        //else if (WeightFrom.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
        //        //  return (WeightFrom.Weight * 0.453592m) < decimal.Parse(surchargeDetail.Unit);
        //        //break;
        //        case "LesserEqual":
        //        //if (WeightFrom.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
        //            return WeightFrom.Weight <= decimal.Parse(surchargeDetail.Unit);
        //        //else if (WeightFrom.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
        //        //    return (WeightFrom.Weight * 0.453592m) <= decimal.Parse(surchargeDetail.Unit);
        //        //break;
        //        case "Equal":
        //        //if (WeightFrom.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
        //            return WeightFrom.Weight == decimal.Parse(surchargeDetail.Unit);
        //        //else if (WeightFrom.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
        //        //    return (WeightFrom.Weight * 0.453592m) == decimal.Parse(surchargeDetail.Unit);
        //        //break;
        //    }

        //    return false;
        //}

        //public static bool DimensionSurcharge(DirectBookingService Dimension, LogisticServiceSurchargeDetail surchargeDetail)
        //{
        //    switch (surchargeDetail.Condition)
        //    {
        //        case "Greater":
        //            if (Dimension.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
        //                return (Dimension.Length * Dimension.Width * Dimension.Height) > decimal.Parse(surchargeDetail.Unit);
        //            else if (Dimension.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
        //                return ((Dimension.Length * (decimal)2.54) * (Dimension.Width * (decimal)2.54) * (Dimension.Height * (decimal)2.54)) > decimal.Parse(surchargeDetail.Unit);
        //            break;
        //        case "GreaterEqual":
        //            break;
        //        case "Lesser":
        //            break;
        //        case "LesserEqual":
        //            break;
        //        case "Equal":
        //            break;
        //    }

        //    return false;
        //}

        //public static bool LengthSurcharge(DirectBookingService Dimension, LogisticServiceSurchargeDetail surchargeDetail)
        //{
        //    switch (surchargeDetail.Condition)
        //    {
        //        case "Greater":
        //            if (Dimension.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
        //                return Dimension.Length > decimal.Parse(surchargeDetail.Unit);
        //            else if (Dimension.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
        //                return ((Dimension.Length * (decimal)2.54) > decimal.Parse(surchargeDetail.Unit));
        //            break;
        //        case "GreaterEqual":
        //            break;
        //        case "Lesser":
        //            break;
        //        case "LesserEqual":
        //            break;
        //        case "Equal":
        //            break;
        //    }

        //    return false;
        //}

        //public static bool WidthSurcharge(DirectBookingService Dimension, LogisticServiceSurchargeDetail surchargeDetail)
        //{
        //    switch (surchargeDetail.Condition)
        //    {
        //        case "Greater":
        //            if (Dimension.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
        //                return Dimension.Width > decimal.Parse(surchargeDetail.Unit);
        //            else if (Dimension.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
        //                return ((Dimension.Width * (decimal)2.54) > decimal.Parse(surchargeDetail.Unit));
        //            break;
        //        case "GreaterEqual":
        //            break;
        //        case "Lesser":
        //            break;
        //        case "LesserEqual":
        //            break;
        //        case "Equal":
        //            break;
        //    }

        //    return false;
        //}

        //public static bool HeightSurcharge(DirectBookingService Dimension, LogisticServiceSurchargeDetail surchargeDetail)
        //{
        //    switch (surchargeDetail.Condition)
        //    {
        //        case "Greater":
        //            if (Dimension.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
        //                return Dimension.Height > decimal.Parse(surchargeDetail.Unit);
        //            else if (Dimension.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
        //                return ((Dimension.Height * (decimal)2.54) > decimal.Parse(surchargeDetail.Unit));
        //            break;
        //        case "GreaterEqual":
        //            break;
        //        case "Lesser":
        //            break;
        //        case "LesserEqual":
        //            break;
        //        case "Equal":
        //            break;
        //    }

        //    return false;
        //}

        //public static bool DaySurcharge(DirectBookingService SatDay, LogisticServiceSurchargeDetail surchargeDetail)
        //{
        //    switch (surchargeDetail.Condition)
        //    {
        //        case "Greater":
        //            break;
        //        case "GreaterEqual":
        //            break;
        //        case "Lesser":
        //            break;
        //        case "LesserEqual":
        //            break;
        //        case "Equal":
        //            switch (surchargeDetail.Unit)
        //            {
        //                case FrayteSurchargeRuleUnittype.ServiceName:
        //                    return SatDay.LogisticDescription == FrayteSurchargeRuleUnittype.ServiceName;
        //                case FrayteSurchargeRuleUnittype.ServiceBy9:
        //                    return SatDay.LogisticDescription == FrayteSurchargeRuleUnittype.ServiceBy9;
        //                case FrayteSurchargeRuleUnittype.ServiceBy10:
        //                    return SatDay.LogisticDescription == FrayteSurchargeRuleUnittype.ServiceBy10;
        //            }
        //            break;
        //    }

        //    return false;
        //}
    }
}