using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services
{
    public static class CommonConversion
    {
        private const string USER_KEY = "PlAf";

        public static string GetNewRandonNumber()
        {
            var rng = new Random();
            return rng.Next(1000, 9999).ToString();
        }

        public static string GetNewFrayteNumber()
        {
            var rng = new Random();
            return rng.Next(10000000, 99999999).ToString();
        }

        public static string GetNewSpecialCharacter()
        {
            Random _rng = new Random();
            string _chars = "*$?!%";

            char[] buffer = new char[2];

            for (int i = 0; i < 2; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        public static string ConvertToString(DataRow objRow, string colmnName)
        {
            if (objRow != null && objRow.Table.Columns.Contains(colmnName))
            {
                if (objRow[colmnName] == null)
                    return "";
                else if (objRow[colmnName].GetType() == typeof(System.Decimal))
                {
                    decimal decVal = 0;
                    decimal.TryParse(objRow[colmnName].ToString(), out decVal);
                    return decVal.ToString("N2");
                }
                else if (objRow[colmnName].GetType() == typeof(System.DateTime))
                {
                    DateTime dtVal = new DateTime();
                    DateTime.TryParse(objRow[colmnName].ToString(), out dtVal);
                    if (dtVal.Equals(new DateTime()))
                        return "";

                    return dtVal.ToShortDateString();
                }
                else
                    return objRow[colmnName].ToString().Trim();
            }
            else
                return "";
        }

        public static string ConvertToStrings(DataRow objRow, string colmnName)
        {
            if (objRow != null && objRow.Table.Columns.Contains(colmnName))
            {
                if (objRow[colmnName] == null)
                    return "";
                else if (objRow[colmnName].GetType() == typeof(System.Decimal))
                {
                    decimal decVal = 0;
                    decimal.TryParse(objRow[colmnName].ToString(), out decVal);
                    return decVal.ToString("N2");
                }
                else if (objRow[colmnName].GetType() == typeof(System.DateTime))
                {
                    DateTime dtVal = new DateTime();
                    DateTime.TryParse(objRow[colmnName].ToString(), out dtVal);
                    if (dtVal.Equals(new DateTime()))
                        return "";

                    return dtVal.ToString();
                }
                else
                    return objRow[colmnName].ToString().Trim();
            }
            else
                return "";
        }

        public static bool ConvertToBool(DataRow objRow, string colmnName)
        {
            if (objRow.Table.Columns.Contains(colmnName))
            {
                if (objRow[colmnName] == null)
                    return false;
                else
                {
                    bool retVal = false;
                    bool.TryParse(objRow[colmnName].ToString().Trim(), out retVal);
                    return retVal;
                }
            }
            else
                return false;
        }

        public static bool ConvertToBool(string stringValue)
        {
            bool retVal = false;
            bool.TryParse(stringValue, out retVal);
            return retVal;
        }

        public static DateTime ConvertToDateTime(DataRow objRow, string colmnName)
        {
            DateTime DateTimeValue = DateTime.Today;
            DateTime.TryParse(ConvertToString(objRow, colmnName), out DateTimeValue);
            return DateTimeValue;
        }

        public static DateTime ConvertToDateTimes(DataRow objRow, string colmnName)
        {
            DateTime DateTimeValue = DateTime.Today;
            DateTime.TryParse(ConvertToStrings(objRow, colmnName), out DateTimeValue);
            return DateTimeValue;
        }

        public static DateTime? ConvertToDateTimeNullable(object objValue)
        {
            if (objValue == null || string.IsNullOrEmpty(objValue.ToString()))
                return null;
            else
            {
                DateTime DateTimeValue = DateTime.Today;
                DateTime.TryParse(objValue.ToString(), out DateTimeValue);
                return DateTimeValue;
            }
        }

        public static DateTime ConvertToDateTime(string objValue)
        {
            if (objValue == null || string.IsNullOrEmpty(objValue.ToString()))
                return DateTime.UtcNow;
            else
            {
                string[] date = objValue.Split('/');
                return new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[0]), Convert.ToInt32(date[1]));
            }
        }

        public static int ConvertToInt(DataRow objRow, string colmnName)
        {
            int intValue = 0;
            int.TryParse(ConvertToString(objRow, colmnName), out intValue);
            return intValue;
        }

        public static int ConvertToInt(string stringValue)
        {
            int intValue = 0;
            int.TryParse(stringValue, out intValue);
            return intValue;
        }

        public static decimal ConvertToDecimal(DataRow objRow, string colmnName)
        {
            decimal decimalValue = 0;
            decimal.TryParse(ConvertToString(objRow, colmnName), out decimalValue);
            return decimalValue;
        }

        public static decimal ConvertToDecimal(string stringValue)
        {
            decimal decimalValue = 0;
            decimal.TryParse(stringValue, out decimalValue);
            return decimalValue;
        }

        public static double ConvertToDouble(DataRow objRow, string colmnName)
        {
            double doubleValue = 0;
            double.TryParse(ConvertToString(objRow, colmnName), out doubleValue);
            return doubleValue;
        }

        public static double ConvertToDouble(string stringVal)
        {
            double doubleValue = 0;
            double.TryParse(stringVal, out doubleValue);
            return doubleValue;
        }

        public static string ConvertToShortDateString(DataRow objRow, string colmnName)
        {
            if (objRow.Table.Columns.Contains(colmnName))
            {
                if (objRow[colmnName] == null)
                    return "";
                else
                {
                    DateTime dt;
                    DateTime.TryParse(objRow[colmnName].ToString(), out dt);
                    if (dt.Equals(DateTime.MinValue))
                        return "";
                    else
                        return dt.ToShortDateString();
                }
            }
            else
                return "";
        }

        public static string ValidateStringLength(string fieldValue, int fieldLimit)
        {
            fieldValue = fieldValue.Replace("''", "'");
            fieldValue = fieldValue.Length > fieldLimit ? fieldValue.Substring(0, fieldLimit) : fieldValue;
            fieldValue = fieldValue.Replace("'", "''");
            return fieldValue;
        }

        public static string CreateFormattedTime()
        {
            string hours;
            string minutes;
            string seconds;
            hours = minutes = seconds = string.Empty;

            hours = DateTime.Now.TimeOfDay.Hours < 10 ? "0" + DateTime.Now.TimeOfDay.Hours.ToString() : DateTime.Now.TimeOfDay.Hours.ToString();
            minutes = DateTime.Now.Minute < 10 ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString();
            seconds = DateTime.Now.Second < 10 ? "0" + DateTime.Now.Second.ToString() : DateTime.Now.Second.ToString();
            return String.Format("{0}:{1}:{2}", hours, minutes, seconds);
        }

        internal static object ConvertToInt(int p)
        {
            throw new NotImplementedException();
        }

        public static decimal RoundDecimalValue(decimal Number)
        {
            if (Number < (decimal)2.50)
            {
                return (decimal)2.50;
            }
            else
            {
                string[] num = Number.ToString().Split('.');
                if (ConvertToDouble(num[1]) == 00)
                {
                    return (decimal)ConvertToDouble(num[0]);
                }
                else if (ConvertToDouble(num[1]) > 49)
                {
                    return (decimal)ConvertToDouble(num[0]) + 1;
                }
                else
                {
                    return (decimal)ConvertToDouble(num[0]) + (decimal).50;
                }
            }
        }

        public static string ConvertFirstLetterCaps(string value)
        {
            string service = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                service = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
            }
            return service;
        }

        public static string DisplayName(string CourierName)
        {
            string Name = string.Empty;
            if (Frayte.Services.Models.FrayteCourierCompany.DHLExpress == CourierName)
            {
                Name = "DHL Express";
            }
            else if (Frayte.Services.Models.FrayteCourierCompany.FedEx == CourierName)
            {
                Name = "Fed Ex";
            }
            else if (Frayte.Services.Models.FrayteCourierCompany.TNT == CourierName)
            {
                Name = "TNT Express";
            }
            return Name;
        }

        public static TimeSpan ConvertStringToTime(string Time)
        {
            if (!string.IsNullOrEmpty(Time))
            {
                Time = Time.Contains(":") ? Time.Replace(":", "") : Time;
                string sub = Time.Substring(0, 2) + ":" + Time.Substring(2, 2);
                return TimeSpan.Parse(sub);
            }
            else
            {
                return TimeSpan.Parse(DateTime.Now.ToString("hh:MM"));
            }
        }
    }
}
