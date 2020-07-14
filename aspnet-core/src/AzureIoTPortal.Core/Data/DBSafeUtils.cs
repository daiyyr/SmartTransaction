
/****************************************************************************************
 * Author:          Philip
 * Date:            25/11/2011
 * Description:     Provide validation for database accesss.
 * Note:
 * Kowning Issues:
 * Last Modified:   25/11/2011
****************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureIoTPortal.Data
{
    public class DBSafeUtils
    {
        #region From input to SQL string
        public static string StrToQuoteSQL(string str)
        {
            try
            {
                str = str.Replace("\\", "\\\\");
                str = str.Replace("'", "\\'");
                str = "'" + str + "'";
                return str;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string QuoteSQLToStr(string str)
        {
            try
            {
                str = str.Substring(1, str.Length - 2);
                str = str.Replace("\\\\", "\\");
                str = str.Replace("\\'", "'");
                return str;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string StrToDQuoteSQL(string str)
        {
            try
            {
                str = str.Replace("\\", "\\\\");
                str = str.Replace("\"", "\\\"");
                str = "\"" + str + "\"";
                return str;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string DateToSQL(object date)
        {
            try
            {
                if (date == null || date == DBNull.Value || Convert.ToString(date) == "" || Convert.ToString(date).ToLower() == "null") return "null";
                else
                {
                    DateTime dateObj = DateTime.MinValue;
                    dateObj = DBSafeUtils.DBDateToDate(date);
                    if (dateObj == DateTime.MinValue) return "null";
                    else return "'" + dateObj.Year + "-" + dateObj.Month + "-" + dateObj.Day + "'";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string DateTimeToSQL(object datetime)
        {
            try
            {
                if (datetime == null || datetime == DBNull.Value || datetime == "") return "null";
                else
                {
                    DateTime dateObj = DateTime.MinValue;
                    dateObj = DBSafeUtils.DBDateToDate(datetime);
                    if (dateObj == DateTime.MinValue) return "null";
                    else return "'" + dateObj.Year + "-" + dateObj.Month + "-" + dateObj.Day
                        + " " + dateObj.Hour + ":" + dateObj.Minute + ":" + dateObj.Second + "'";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string TimeToSQL(object time)
        {
            try
            {
                if (time == null || time == DBNull.Value || time == "") return "null";
                TimeSpan ts = new TimeSpan();
                if (time != null) ts = TimeSpan.Parse(time.ToString());
                if (ts.Equals(TimeSpan.Zero)) return "null";
                else
                {
                    string ret = "'" + ts.ToString() + "'";
                    return ret;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    DateTime dt = DateTime.MinValue;
                    if (time != null) dt = DBSafeUtils.DBDateToDate(time);
                    if (dt == DateTime.MinValue) return "null";
                    else
                    {
                        string ret = "'" + dt.Hour + ":" + dt.Minute + ":" + dt.Second + "'";
                        return ret;
                    }
                }
                catch (Exception ex2)
                {
                    throw ex2;
                }
            }
        }
        public static string IntToSQL(int? num)
        {
            try
            {
                if (num == null) return "null";
                else return num.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string IntToSQL(string num)
        {
            try
            {
                if (num == "") return "null";
                else return Convert.ToInt32(num).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string IntToSQL(int? num, bool nullable)
        {
            try
            {
                if (nullable) return IntToSQL(num);
                else
                {
                    int ret = 0;
                    if (num == null) return "0";
                    else return num.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string IntToSQL(string num, bool nullable)
        {
            try
            {
                if (nullable) return IntToSQL(num);
                else
                {
                    if (num == "") return "0";
                    else return Convert.ToInt32(num).ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string DecimalToSQL(decimal? num)
        {
            try
            {
                if (num == null) return "null";
                else return num.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string DecimalToSQL(decimal? num, bool nullable)
        {
            try
            {
                if (nullable) return DecimalToSQL(num);
                else
                {
                    if (num == null) return "0";
                    else return num.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string DecimalToSQL(string num)
        {
            try
            {
                if (num == "") return "null";
                else return Convert.ToDecimal(num).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string DecimalToSQL(string num, bool nullable)
        {
            try
            {
                if (nullable) return DecimalToSQL(num);
                else
                {
                    if (num == "") return "0";
                    else return Convert.ToDecimal(num).ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string BoolToSQL(bool? b)
        {
            try
            {
                if (b == null) return "null";
                else if (b.Value) return "1";
                else return "0";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string BoolToSQL(string b)
        {
            try
            {
                if (b == "") return "null";
                else if (b == "true" || b == "True" || b == "TRUE") return "1";
                else if (b == "false" || b == "False" || b == "FALSE") return "0";
                else if (b == "1") return "1";
                else if (b == "0") return "0";
                else throw new Exception("Invalid input!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string BoolToSQL(bool b)
        {
            try
            {
                if (b) return "1";
                else return "0";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region DBValue to C# format
        public static object ValDBValue(object obj)
        {
            try
            {
                if (obj == DBNull.Value || obj == null) return null;
                else return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DateTime DBDateToDate(object obj)
        {
            try
            {
                if (obj == DBNull.Value || obj == null) return DateTime.MinValue;
                else
                {
                    if (obj.ToString().ToLower() == "null") return DateTime.MinValue;
                    else
                    {
                        try
                        {
                            return Convert.ToDateTime(obj);
                        }
                        catch
                        {
                            return DateTime.ParseExact(
                                obj.ToString(),
                                "dd/MM/yyyy",
                                System.Globalization.CultureInfo.InvariantCulture
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DateTime? DBDateToDateN(object obj)
        {
            try
            {
                if (obj == DBNull.Value || obj == null) return null;
                else
                {
                    if (obj.ToString().ToLower() == "null") return null;
                    else
                        return DBSafeUtils.DBDateToDate(obj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string DBDateToStr(object obj)
        {
            try
            {
                if (obj == DBNull.Value || obj == null) return "Null";
                else
                {
                    DateTime dateObj = DBSafeUtils.DBDateToDate(obj);
                    return dateObj.Day.ToString() + "/" + dateObj.Month.ToString() + "/" + dateObj.Year.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static TimeSpan DBTimeToTime(object obj)
        {
            try
            {
                string time = obj.ToString();
                string[] timespan = time.Split(':');
                TimeSpan ret = new TimeSpan(Convert.ToInt32(timespan[0]), Convert.ToInt32(timespan[1]), Convert.ToInt32(timespan[2]));
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static TimeSpan? DBTimeToTimeN(object obj)
        {
            try
            {
                if (obj == DBNull.Value || obj == null) return null;
                else
                {
                    string time = obj.ToString();
                    string[] timespan = time.Split(':');
                    TimeSpan ret = new TimeSpan(Convert.ToInt32(timespan[0]), Convert.ToInt32(timespan[1]), Convert.ToInt32(timespan[2]));
                    return ret;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static int? DBIntToIntN(object obj)
        {
            try
            {
                if (obj == DBNull.Value || obj == null) return null;
                else
                {
                    if (obj.ToString().ToLower() == "null") return null;
                    else
                        return Convert.ToInt32(obj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static decimal? DBDecimalToDecimalN(object obj)
        {
            try
            {
                if (obj == DBNull.Value || obj == null) return null;
                else
                {
                    return Convert.ToDecimal(obj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string DBStrToStr(object obj)
        {
            try
            {
                if (obj == DBNull.Value || obj == null) return "";
                else
                {
                    return obj.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool DBBoolToBool(object obj)
        {
            try
            {
                if (obj == DBNull.Value || obj == null) throw new Exception("Object cannot be Null!");
                else
                {
                    if (Convert.ToInt32(obj) == 1) return true;
                    else if (Convert.ToInt32(obj) == 0) return false;
                    else throw new Exception("Invalid value!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool? DBBoolToBoolN(object obj)
        {
            try
            {
                if (obj == DBNull.Value || obj == null || obj.Equals("")) return null;
                else
                {
                    if (Convert.ToInt32(obj) == 1) return true;
                    else if (Convert.ToInt32(obj) == 0) return false;
                    else throw new Exception("Invalid value!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string DBBoolToStr(object obj)
        {
            try
            {
                if (obj == DBNull.Value || obj == null) return "Null";
                else
                {
                    if (Convert.ToInt32(obj) == 1) return "Yes";
                    else if (Convert.ToInt32(obj) == 0) return "No";
                    else throw new Exception("Invalid value!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string DBObjToStr(object obj)
        {
            try
            {
                if (obj == DBNull.Value || obj == null) return "";
                else
                {
                    return obj.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
