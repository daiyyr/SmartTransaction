using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace AzureIoTPortal.Web.Controllers
{
    public class Spider
    {

        public static string version = "4.3";
        public static int timeoutTime = 1000 * 60 * 5;
        public static int delayTime = 400;
        public static decimal serverForbidenDelayMinites = 0.6m;
        public static int mostForbbidenTimes = 4;
        public static int mostReadPageFailedTimes = 4;
        public static int retry = 1;

        public static string connstr = "dsn=paymark_spider;UID=root;PWD=onlyoffice;  OPTION=3; Pooled=True;";
        //public static string connstr = "dsn=paymark_spider;UID=root;PWD=rrRXJI5NJg;  OPTION=3; Pooled=True;";

        public static string AgentImageFolder = "";
        public static string PropertyImageFolder = "";
        public static Regex standstrFilter = new Regex("[^a-zA-Z0-9 -]");

        //   public static string gHost = "search.bartercard.co.nz";
        public static string gHost = "sec.paymentexpress.com";
        //  public static string gHost = "210.86.1.61:81"; //portal


        public static bool printDtailsLog = true;

        //public static string gAccept = "application/json";
        //public static string requestContentType = "application/json";

        //for paymark
        // public static string gAccept = "application/vnd.paymark_api+json";
        // public static string requestContentType = "application/vnd.paymark_api+json";

        //for transtrack.paymark.co.nz/
        public static string gAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        public static string requestContentType = "application/x-www-form-urlencoded";

        //for paymentexpress
        //public static string gAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        //public static string requestContentType = "multipart/form-data; boundary=----WebKitFormBoundarynTPzP7J0sAo0CBUW";

        //for bartercard
        //public static string gAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        //public static string requestContentType = "application/x-www-form-urlencoded";


        public string gusername = "";
        public string gpassword = "";

        public string BearerKey = "";

        //     public static string gUserAgent = "okhttp/2.7.5";
        public static string gUserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";
        public string gKey = "dGxkN0RHanpRdjNWRzI2RlcxdkxiMUFzd3hiU0tQYm06MTQ5OTIxMTgxMA==";
        public string gDigest = "f94baf95401d373e13363dbde8c0e4459007b83d";
        public string gUdid = "ec56eec2ba9657a2";
        public static string whiteFile = "WhiteList.txt";
        public static string blackList = "BlackList.txt";
        public static string eGiftValidList = "eGiftValid.xml";
        public static string soldList = "SoldList.txt";
        public string whileFilePath = "";
        public string blackFilePath = "";
        public string initFilePath = "";
        public string paymarkFilePath = "";
        public static int keepSessionRequestDelay = 2; //minites

        public bool stop = false;

        #region Util Functions
        public static string ReplaceEverythingBeforeAndOnLastOccurrence(string Source, string Find)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(0, place + Find.Length);
            return result;
        }
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
        public static string DateToSQL(object date)
        {
            try
            {
                if (date == null || date == DBNull.Value || Convert.ToString(date) == "" || Convert.ToString(date).ToLower() == "null") return "null";
                else
                {
                    DateTime dateObj = DateTime.MinValue;
                    dateObj = Convert.ToDateTime(date);
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
                    dateObj = Convert.ToDateTime(datetime);
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

        public void setRequest(HttpWebRequest req, CookieCollection cookies)
        {
            req.Timeout = timeoutTime;
            req.Host = gHost;
            //       req.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.10; rv:40.0) Gecko/20100101 Firefox/40.0";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            req.AllowAutoRedirect = false;
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.PerDomainCapacity = 40;
            if (cookies != null)
            {
                req.CookieContainer.Add(cookies);
            }
            /*
            if (xmlRequest)
            {
                req.ContentType = "text/xml; encoding='utf-8'";
            }
            else
            {
                req.ContentType = "application/x-www-form-urlencoded";
            }
            */

            //for voucher
            //req.Headers.Add("X-Auth-Key", gKey);
            //req.Headers.Add("X-Auth-Digest", gDigest);

            req.ContentType = requestContentType;
            req.UserAgent = gUserAgent;
            req.Accept = gAccept;


            //for paymark.co.nz
            /*
            req.UseDefaultCredentials = true;
            req.PreAuthenticate = true;
            req.Credentials = CredentialCache.DefaultCredentials;
            req.Headers.Add("Authorization", "Bearer " + BearerKey);
            if(req.RequestUri.ToString() == "https://api.paymark.nz/identity/authorisation/")
            {
                req.Headers.Add("Origin", "https://account.paymark.co.nz");
                req.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                req.Headers.Add("Accept-Language", "en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7");
            }
            */

            req.KeepAlive = true;

            req.Headers.Add("Cache-Control", "max-age=0");
            req.Headers.Add("Upgrade-Insecure-Requests", "1");
            req.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            req.Headers.Add("Accept-Language", "en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7");


            //for payment express
            req.AutomaticDecompression = DecompressionMethods.GZip;

        }
        public int writePostData(HttpWebRequest req, string postData, bool xmlRequest = false, bool jsonRequest = true)
        {
            byte[] postBytes = Encoding.UTF8.GetBytes(postData);
            //           (xmlRequest ? Encoding.ASCII.GetBytes(postData) : Encoding.UTF8.GetBytes(postData));

            if (xmlRequest)
            {
                req.ContentLength = postBytes.Length;
            }


            if (jsonRequest)
            {
                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    streamWriter.Write(postData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            else
            {
                //req.ContentLength = postBytes.Length;  // cause InvalidOperationException: 写入开始后不能设置此属性。
                Stream postDataStream = null;
                try
                {
                    postDataStream = req.GetRequestStream();
                    postDataStream.Write(postBytes, 0, postBytes.Length);
                }
                catch (WebException webEx)
                {
                    return -1;
                }
                postDataStream.Close();
            }

            return 1;
        }
        public string resp2html(HttpWebResponse resp)
        {
            try
            {
                if (resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.Found)
                {
                    if (resp.CharacterSet == "GZip")
                    {
                        //???
                    }
                    StreamReader stream = new StreamReader(resp.GetResponseStream());
                    return stream.ReadToEnd();
                }
                else
                {
                    return resp.StatusDescription;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        private byte[] GetBytes(WebResponse response)
        {
            var length = (int)response.ContentLength;
            byte[] data;

            using (var memoryStream = new MemoryStream())
            {
                var buffer = new byte[0x100];
                try
                {
                    using (var rs = response.GetResponseStream())
                    {
                        for (var i = rs.Read(buffer, 0, buffer.Length); i > 0; i = rs.Read(buffer, 0, buffer.Length))
                        {
                            memoryStream.Write(buffer, 0, i);
                        }
                    }
                }
                catch (Exception e)
                {

                }


                data = memoryStream.ToArray();
            }

            return data;
        }
        private string GetEncodingFromBody(byte[] buffer)
        {
            var regex = new Regex(@"<meta(\s+)http-equiv(\s*)=(\s*""?\s*)content-type(\s*""?\s+)content(\s*)=(\s*)""text/html;(\s+)charset(\s*)=(\s*)(?<charset>[a-zA-Z0-9-]+?)""(\s*)(/?)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var str = Encoding.ASCII.GetString(buffer);
            var regMatch = regex.Match(str);
            if (regMatch.Success)
            {
                var charSet = regMatch.Groups["charset"].Value;
                return charSet;
            }

            return Encoding.ASCII.BodyName;
        }
        public string resp2html(HttpWebResponse resp, string charSet)
        {
            var buffer = GetBytes(resp);
            if (resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.Found)
            {
                if (String.IsNullOrEmpty(charSet) || string.Compare(charSet, "ISO-8859-1") == 0)
                {
                    charSet = GetEncodingFromBody(buffer);
                }

                try
                {
                    var encoding = Encoding.GetEncoding(charSet);  //Shift_JIS
                    var str = encoding.GetString(buffer);

                    return str;
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
            else
            {
                return resp.StatusDescription;
            }

        }
        public string sendRequest(string url, string method, string referer, bool allowAutoRedirect, string postData, ref CookieCollection cookies, string host, bool responseInUTF8, string accept = "", bool NotPrintProtocalError = false)
        {
            for (int i = 0; i < retry; i++)
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = null;
                setRequest(req, cookies);
                if (accept != "")
                {
                    req.Accept = accept;
                }
                req.Method = method;
                req.Referer = referer;
                if (allowAutoRedirect)
                {
                    req.AllowAutoRedirect = true;
                }
                req.Host = host;



                if (url.Contains("reportid=OverdueInvoiceReport"))
                    req.Timeout = 1000 * 60 * 60 * 12;          //wait at most 12 hours for over due invoice


                if (method.Equals("POST"))
                {
                    if (writePostData(req, postData) < 0)
                    {
                        continue;
                    }
                }
                string respHtml = "";
                string errorMessage = "";

                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException webEx)
                {
                    if (webEx.Status == WebExceptionStatus.ConnectionClosed)
                    {
                        //return "wrong address"; //地址错误
                        throw webEx;
                    }
                    if (webEx.Status == WebExceptionStatus.ProtocolError)
                    {
                        return "";
                    }
                    /*
                    if (webEx.Status == WebExceptionStatus.ProtocolError)
                    {
                        //return "ProtocolError";
                        throw webEx;
                    }
                    */
                    bool AcceptableException = false;
                    if (webEx.Message.Contains("(400)"))
                    {
                        AcceptableException = true;
                        if (webEx.Response != null)
                        {
                            if (webEx.Response != null)
                            {
                                using (var errorResponse = (HttpWebResponse)webEx.Response)
                                {
                                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                    {
                                        errorMessage = reader.ReadToEnd();
                                        //TODO: use JSON.net to parse this string and look at the error message
                                    }
                                }
                            }
                        }
                    }
                    if (!AcceptableException)
                    {
                        if (i < retry - 1)
                        {
                            continue;
                        }
                        else
                        {
                            saveLog(webEx.Message);
                        }
                    }
                }
                if (errorMessage != "")
                {
                    return errorMessage;
                }
                else if (resp != null)
                {
                    if (responseInUTF8)
                    {
                        respHtml = resp2html(resp);
                    }
                    else
                    {
                        respHtml = resp2html(resp, resp.CharacterSet); // like  Shift_JIS
                    }
                    if (respHtml.Equals(""))
                    {
                        continue;
                    }
                    cookies = req.CookieContainer.GetCookies(req.RequestUri);
                    resp.Close();
                    return respHtml;
                }
                else
                {
                    continue;
                }
            }
            return "";
        }
        public HttpWebResponse sendDownloadRequest(string url, string method, string referer, bool allowAutoRedirect, string postData, ref CookieCollection cookies, string host, bool responseInUTF8, string accept = "", bool NotPrintProtocalError = false)
        {
            for (int i = 0; i < retry; i++)
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = null;
                setRequest(req, cookies);
                if (accept != "")
                {
                    req.Accept = accept;
                }
                req.Method = method;
                req.Referer = referer;
                if (allowAutoRedirect)
                {
                    req.AllowAutoRedirect = true;
                }
                req.Host = host;
                if (method.Equals("POST"))
                {
                    if (writePostData(req, postData) < 0)
                    {
                        continue;
                    }
                }
                string respHtml = "";
                string errorMessage = "";
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException webEx)
                {
                    if (webEx.Status == WebExceptionStatus.ConnectionClosed)
                    {
                        //return "wrong address"; //地址错误
                        throw webEx;
                    }
                    if (webEx.Status == WebExceptionStatus.ProtocolError)
                    {
                        return null;
                    }
                    /*
                    if (webEx.Status == WebExceptionStatus.ProtocolError)
                    {
                        //return "ProtocolError";
                        throw webEx;
                    }
                    */
                    bool AcceptableException = false;
                    if (webEx.Message.Contains("(400)"))
                    {
                        AcceptableException = true;
                        if (webEx.Response != null)
                        {
                            if (webEx.Response != null)
                            {
                                using (var errorResponse = (HttpWebResponse)webEx.Response)
                                {
                                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                    {
                                        errorMessage = reader.ReadToEnd();
                                        //TODO: use JSON.net to parse this string and look at the error message
                                    }
                                }
                            }
                        }
                    }
                    if (!AcceptableException)
                    {
                        if (i < retry - 1)
                        {
                            continue;
                        }
                        else
                        {
                            saveLog(webEx.Message);
                        }
                    }
                }

                if (resp != null)
                {
                    return resp;
                }
                else
                {
                    continue;
                }
            }
            return null;
        }


        public static void DataTableToCsv(DataTable table, string file)
        {
            string title = "";
            FileStream fs = new FileStream(file, FileMode.Create);
            StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                title += table.Columns[i].ColumnName + ",";
            }
            title = title.Substring(0, title.Length - 1) + "\r\n";
            sw.Write(title);

            foreach (DataRow row in table.Rows)
            {
                string line = "";
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    line += "\"" + row[i].ToString().Replace("\"", "\"\"") + "\",";
                }
                line = line.Substring(0, line.Length - 1) + "\r\n";
                sw.Write(line);
            }

            sw.Close();
            fs.Close();
        }
        public static DataTable CsvToDataTable(string strFileName)
        {

            if (File.Exists(strFileName))
            {

                string[] Lines;
                string CSVFilePathName = strFileName;

                Lines = File.ReadAllLines(CSVFilePathName);
                while (Lines[0].EndsWith(","))
                {
                    Lines[0] = Lines[0].Remove(Lines[0].Length - 1);
                }
                string[] Fields;
                Fields = Lines[0].Split(new char[] { ',' });
                int Cols = Fields.GetLength(0);
                DataTable dt = new DataTable();
                //1st row must be column names; force lower case to ensure matching later on.
                for (int i = 0; i < Cols; i++)
                    dt.Columns.Add(Fields[i], typeof(string));
                DataRow Row;
                int rowcount = 0;
                try
                {
                    string[] ToBeContinued = new string[] { };
                    bool lineToBeContinued = false;
                    for (int i = 1; i < Lines.GetLength(0); i++)
                    {
                        if (!Lines[i].Equals(""))
                        {
                            Fields = Lines[i].Split(new char[] { ',' });
                            string temp0 = string.Join("", Fields).Replace("\"\"", "");
                            int quaotCount0 = temp0.Count(c => c == '"');
                            if (Fields.GetLength(0) < Cols || lineToBeContinued || quaotCount0 % 2 != 0)
                            {
                                if (ToBeContinued.GetLength(0) > 0)
                                {
                                    ToBeContinued[ToBeContinued.Length - 1] += "\n" + Fields[0];
                                    Fields = Fields.Skip(1).ToArray();
                                }
                                string[] newArray = new string[ToBeContinued.Length + Fields.Length];
                                Array.Copy(ToBeContinued, newArray, ToBeContinued.Length);
                                Array.Copy(Fields, 0, newArray, ToBeContinued.Length, Fields.Length);
                                ToBeContinued = newArray;
                                string temp = string.Join("", ToBeContinued).Replace("\"\"", "");
                                int quaotCount = temp.Count(c => c == '"');
                                if (ToBeContinued.GetLength(0) >= Cols && quaotCount % 2 == 0)
                                {
                                    Fields = ToBeContinued;
                                    ToBeContinued = new string[] { };
                                    lineToBeContinued = false;
                                }
                                else
                                {
                                    lineToBeContinued = true;
                                    continue;
                                }
                            }

                            //modified by teemo @2016 09 13
                            //handle ',' and '"'
                            //Deserialize CSV following Excel's rule:
                            // 1: If there is commas in a field, quote the field.
                            // 2: Two consecutive quotes indicate a user's quote.

                            List<int> singleLeftquota = new List<int>();
                            List<int> singleRightquota = new List<int>();

                            //combine fileds if number of commas match
                            if (Fields.GetLength(0) > Cols)
                            {
                                bool lastSingleQuoteIsLeft = true;
                                for (int j = 0; j < Fields.GetLength(0); j++)
                                {
                                    bool leftOddquota = false;
                                    bool rightOddquota = false;
                                    if (Fields[j].StartsWith("\""))
                                    {
                                        int numberOfConsecutiveQuotes = 0;
                                        foreach (char c in Fields[j]) //start with how many "
                                        {
                                            if (c == '"')
                                            {
                                                numberOfConsecutiveQuotes++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        if (numberOfConsecutiveQuotes % 2 == 1)//start with odd number of quotes indicate system quote
                                        {
                                            leftOddquota = true;
                                        }
                                    }

                                    if (Fields[j].EndsWith("\""))
                                    {
                                        int numberOfConsecutiveQuotes = 0;
                                        for (int jj = Fields[j].Length - 1; jj >= 0; jj--)
                                        {
                                            if (Fields[j].Substring(jj, 1) == "\"") // end with how many "
                                            {
                                                numberOfConsecutiveQuotes++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        if (numberOfConsecutiveQuotes % 2 == 1)//end with odd number of quotes indicate system quote
                                        {
                                            rightOddquota = true;
                                        }
                                    }
                                    if (leftOddquota && !rightOddquota)
                                    {
                                        singleLeftquota.Add(j);
                                        lastSingleQuoteIsLeft = true;
                                    }
                                    else if (!leftOddquota && rightOddquota)
                                    {
                                        singleRightquota.Add(j);
                                        lastSingleQuoteIsLeft = false;
                                    }
                                    else if (Fields[j] == "\"") //only one quota in a field
                                    {
                                        if (lastSingleQuoteIsLeft)
                                        {
                                            singleRightquota.Add(j);
                                        }
                                        else
                                        {
                                            singleLeftquota.Add(j);
                                        }
                                    }
                                }
                                if (singleLeftquota.Count == singleRightquota.Count)
                                {
                                    int insideCommas = 0;
                                    for (int indexN = 0; indexN < singleLeftquota.Count; indexN++)
                                    {
                                        insideCommas += singleRightquota[indexN] - singleLeftquota[indexN];
                                    }
                                    if (Fields.GetLength(0) - Cols >= insideCommas) //probabaly matched
                                    {
                                        int validFildsCount = insideCommas + Cols; //(Fields.GetLength(0) - insideCommas) may be exceed the Cols
                                        String[] temp = new String[validFildsCount];
                                        int totalOffSet = 0;
                                        for (int iii = 0; iii < validFildsCount - totalOffSet; iii++)
                                        {
                                            bool combine = false;
                                            int storedIndex = 0;
                                            for (int iInLeft = 0; iInLeft < singleLeftquota.Count; iInLeft++)
                                            {
                                                if (iii + totalOffSet == singleLeftquota[iInLeft])
                                                {
                                                    combine = true;
                                                    storedIndex = iInLeft;
                                                    break;
                                                }
                                            }
                                            if (combine)
                                            {
                                                int offset = singleRightquota[storedIndex] - singleLeftquota[storedIndex];
                                                for (int combineI = 0; combineI <= offset; combineI++)
                                                {
                                                    temp[iii] += Fields[iii + totalOffSet + combineI] + ",";
                                                }
                                                temp[iii] = temp[iii].Remove(temp[iii].Length - 1, 1);
                                                totalOffSet += offset;
                                            }
                                            else
                                            {
                                                temp[iii] = Fields[iii + totalOffSet];
                                            }
                                        }
                                        Fields = temp;
                                    }
                                }
                            }
                            Row = dt.NewRow();
                            for (int f = 0; f < Cols; f++)
                            {
                                Fields[f] = Fields[f].Replace("\"\"", "\""); //Two consecutive quotes indicate a user's quote
                                if (Fields[f].StartsWith("\""))
                                {
                                    if (Fields[f].EndsWith("\""))
                                    {
                                        Fields[f] = Fields[f].Remove(0, 1);
                                        if (Fields[f].Length > 0)
                                        {
                                            Fields[f] = Fields[f].Remove(Fields[f].Length - 1, 1);
                                        }
                                    }
                                }
                                Row[f] = Fields[f];
                            }
                            dt.Rows.Add(Row);
                            rowcount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("row: " + (rowcount + 2) + ", " + ex.Message);
                }
                //OleDbConnection connection = new OleDbConnection(string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}; Extended Properties=""text;HDR=Yes;FMT=Delimited"";", FilePath + FileName));
                //OleDbCommand command = new OleDbCommand("SELECT * FROM " + FileName, connection);
                //OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                //DataTable dt = new DataTable();
                //adapter.Fill(dt);
                //adapter.Dispose();
                return dt;
            }
            else
                return null;

            //OleDbConnection connection = new OleDbConnection(string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}; Extended Properties=""text;HDR=Yes;FMT=Delimited"";", strFilePath));
            //OleDbCommand command = new OleDbCommand("SELECT * FROM " + strFileName, connection);
            //OleDbDataAdapter adapter = new OleDbDataAdapter(command);
            //DataTable dt = new DataTable();
            //adapter.Fill(dt);
            //return dt;
        }
        #endregion

        public static void saveLog(string text)
        {
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "\\SendingDailyEmailLog.txt";
            File.AppendAllText(logPath, global::System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + text + Environment.NewLine);
        }




    }

}
