using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AzureIoTPortal.Data
{
    public class AdFunction
    {
        //public static string conn = (
        //    ConfigurationManager.ConnectionStrings["SmsContext"].ConnectionString.Contains("dsn=") ?
        //    ConfigurationManager.ConnectionStrings["SmsContext"].ConnectionString :
        //    "dsn=sapp_sms;" + ConfigurationManager.ConnectionStrings["SmsContext"].ConnectionString
        //    );
        //public static string constr_general = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static string conn_sms = "";
        public static string conn_iot = "";


        public class displayDetail
        {
            public string name { get; set; }
            public bool fk = false;
            public Type datetype = null;
            public displayDetail(string name, bool foreign_key = false, Type datetype = null)
            {
                this.name = name;
                fk = foreign_key;
                this.datetype = datetype;
            }
        }
        public static string GeneratePageTable(SortedDictionary<int, displayDetail> displayDetailList)
        {
            int i = 0;
            string html = "<table class=\"details\">";
            foreach (var details in displayDetailList)
            {
                if (i % 2 == 0)
                {
                    html += "<tr>";
                }
                html += "<tr><td><label class=\"label-sms\">" + details.Value.name + ":</label></td>";
                if (!details.Value.fk)
                {
                    if (details.Value.datetype == typeof(Nullable<DateTime>))
                    {
                        html += "<td>@if(Model." + details.Value.name + ".HasValue){@Model." + details.Value.name + ".Value.ToString(\"dd/MM/yyyy\")}</td>";
                    }
                    else if (details.Value.datetype == typeof(DateTime))
                    {
                        html += "<td>@Model." + details.Value.name + ".ToString(\"dd/MM/yyyy\")</td>";
                    }
                    else
                    {
                        html += "<td>@Html.DisplayFor(model => model." + details.Value.name + ")</td>";
                    }
                }
                else
                {
                    html += "<td>@ViewBag." + details.Value.name + "</td>";
                }
                if (i % 2 == 1)
                {
                    html += "</tr>";
                }
                i++;
            }
            html += "</table>";
            return html;
        }

        public class displayDetail2
        {
            public string name { get; set; }
            public string value { get; set; }
            public displayDetail2(string name, string value)
            {
                this.name = name;
                this.value = value;
            }
        }
        public static string GeneratePageTable2(SortedDictionary<int, displayDetail2> displayDetailList2)
        {
            int i = 0;
            string html = "<table class=\"details\">";
            foreach (var details in displayDetailList2)
            {
                if (i % 2 == 0)
                {
                    html += "<tr>";
                }
                html += "<td><label class=\"label-sms\">" + details.Value.name + ":</label></td>";
                html += "<td>";
                if (details.Value.value == "True")
                {
                    html += "<input type=\"checkbox\" id=\"" + details.Value.name + "\" disabled=\"disabled\" checked=\"checked\">";
                }
                else if (details.Value.value == "False")
                {
                    html += "<input type=\"checkbox\" id=\"" + details.Value.name + "\" disabled=\"disabled\" >";
                }
                else
                {
                    html += details.Value.value;
                }
                html += "</td>";
                if (i % 2 == 1)
                {
                    html += "</tr>";
                }
                i++;
            }
            html += "</table>";
            return html;
        }

        public static string StringArray_String(string[] s)
        {
            string result = "";
            for (int i = 0; i < s.Length; i++)
            {
                result += s[i] + ",";
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }


        #region decimal
        public static decimal Rounded(string v, int places = 2)
        {
            try
            {
                decimal d = 0;
                decimal.TryParse(v, out d);
                string temp = d.ToString("f" + places.ToString());
                d = decimal.Parse(temp);
                return d;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static decimal Rounded(decimal v, int places = 2)
        {
            return decimal.Parse(v.ToString("f" + places.ToString()));
        }
        public static string Ordinal(int number)
        {
            string suffix = String.Empty;

            int ones = number % 10;
            int tens = (int)Math.Floor(number / 10M) % 10;

            if (tens == 1)
            {
                suffix = "th";
            }
            else
            {
                switch (ones)
                {
                    case 1:
                        suffix = "st";
                        break;

                    case 2:
                        suffix = "nd";
                        break;

                    case 3:
                        suffix = "rd";
                        break;

                    default:
                        suffix = "th";
                        break;
                }
            }
            return String.Format("{0}{1}", number, suffix);
        }
        #endregion

        #region Word And PDF
        public static int GetPdfPageCount(string filePath)
        {
            using (PdfDocument pdfDoc = PdfReader.Open(filePath, PdfDocumentOpenMode.Import))
            {
                return pdfDoc.PageCount;
            }
        }
        public static void CombinePDF(List<string> sourceFileNameList, string targetFile)
        {
            using (PdfDocument outPdf = new PdfDocument())
            {
                foreach (string sourceFile in sourceFileNameList)
                {
                    using (PdfDocument one = PdfReader.Open(sourceFile, PdfDocumentOpenMode.Import))
                    {
                        CopyPages(one, outPdf);
                    }
                }
                outPdf.Save(targetFile);
            }
        }
        public static void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }
        public static void dotx2docx(string sourceFile, string targetFile)
        {
            MemoryStream documentStream;
            using (Stream tplStream = File.OpenRead(sourceFile))
            {
                documentStream = new MemoryStream((int)tplStream.Length);
                CopyStream(tplStream, documentStream);
                documentStream.Position = 0L;
            }

            using (WordprocessingDocument template = WordprocessingDocument.Open(documentStream, true))
            {
                template.ChangeDocumentType(DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
                MainDocumentPart mainPart = template.MainDocumentPart;
                mainPart.DocumentSettingsPart.AddExternalRelationship("http://schemas.openxmlformats.org/officeDocument/2006/relationships/attachedTemplate",
                   new Uri(targetFile, UriKind.Absolute));

                mainPart.Document.Save();
            }
            File.WriteAllBytes(targetFile, documentStream.ToArray());
        }
        public static void CopyStream(Stream source, Stream target)
        {
            if (source != null)
            {
                MemoryStream mstream = source as MemoryStream;
                if (mstream != null) mstream.WriteTo(target);
                else
                {
                    byte[] buffer = new byte[2048];
                    int length = buffer.Length, size;
                    while ((size = source.Read(buffer, 0, length)) != 0)
                        target.Write(buffer, 0, size);
                }
            }
        }
        public static void Mailmerge(string templatePath, string DestinatePath, DataRow dr, DataColumnCollection columns)
        {
            dotx2docx(templatePath, DestinatePath);
            using (WordprocessingDocument doc = WordprocessingDocument.Open(DestinatePath, true))
            {
                var allParas = doc.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>();
                Text PreItem = null;
                string PreItemConstant = null;
                bool FindSingleAnglebrackets = false;
                bool breakFlag = false;
                List<Text> breakedFiled = new List<Text>();
                foreach (Text item in allParas)
                {
                    foreach (DataColumn cl in columns)
                    {
                        //<Today>
                        if (item.Text.Contains("«" + cl.ColumnName + "»") || item.Text.Contains("<" + cl.ColumnName + ">"))
                        {
                            item.Text = item.Text.Replace("<" + cl.ColumnName + ">", dr[cl.ColumnName].ToString())
                                                 .Replace("«" + cl.ColumnName + "»", dr[cl.ColumnName].ToString());
                            FindSingleAnglebrackets = false;
                            breakFlag = false;
                            breakedFiled.Clear();
                        }
                        else if //<Today
                        (item.Text != null
                            && (
                                    (item.Text.Contains("<") && !item.Text.Contains(">"))
                                    || (item.Text.Contains("«") && !item.Text.Contains("»"))
                                )
                            && (item.Text.Contains(cl.ColumnName))
                        )
                        {
                            FindSingleAnglebrackets = true;
                            item.Text = global::System.Text.RegularExpressions.Regex.Replace(item.Text, @"\<" + cl.ColumnName + @"(?!\w)", dr[cl.ColumnName].ToString());
                            item.Text = global::System.Text.RegularExpressions.Regex.Replace(item.Text, @"\«" + cl.ColumnName + @"(?!\w)", dr[cl.ColumnName].ToString());
                        }
                        else if //Today> or Today
                        (
                            PreItemConstant != null
                            && (
                                    (PreItemConstant.Contains("<") && !PreItemConstant.Contains(">"))
                                    || (PreItemConstant.Contains("«") && !PreItemConstant.Contains("»"))
                                )
                            && (item.Text.Contains(cl.ColumnName))
                        )
                        {
                            if (item.Text.Contains(">") || item.Text.Contains("»"))
                            {
                                FindSingleAnglebrackets = false;
                                breakFlag = false;
                                breakedFiled.Clear();
                            }
                            else
                            {
                                FindSingleAnglebrackets = true;
                            }
                            if (PreItemConstant == "<" || PreItemConstant == "«")
                            {
                                PreItem.Text = "";
                            }
                            else
                            {
                                PreItem.Text = global::System.Text.RegularExpressions.Regex.Replace(PreItemConstant, @"\<" + cl.ColumnName + @"(?!\w)", dr[cl.ColumnName].ToString());
                                PreItem.Text = global::System.Text.RegularExpressions.Regex.Replace(PreItemConstant, @"\«" + cl.ColumnName + @"(?!\w)", dr[cl.ColumnName].ToString());
                            }
                            if (item.Text.Contains(cl.ColumnName + ">") || item.Text.Contains(cl.ColumnName + "»"))
                            {
                                item.Text = global::System.Text.RegularExpressions.Regex.Replace(item.Text, @"(?<!\w)" + cl.ColumnName + @"\>", dr[cl.ColumnName].ToString());
                                item.Text = global::System.Text.RegularExpressions.Regex.Replace(item.Text, @"(?<!\w)" + cl.ColumnName + @"\»", dr[cl.ColumnName].ToString());

                            }
                            else
                            {
                                item.Text = global::System.Text.RegularExpressions.Regex.Replace(item.Text, @"(?<!\w)" + cl.ColumnName + @"(?!\w)", dr[cl.ColumnName].ToString());
                            }
                        }
                        else if (FindSingleAnglebrackets && (item.Text.Contains("»") || item.Text.Contains(">")))
                        {
                            item.Text = global::System.Text.RegularExpressions.Regex.Replace(item.Text, @"(?<!\w)" + cl.ColumnName + @"\>", dr[cl.ColumnName].ToString());
                            item.Text = global::System.Text.RegularExpressions.Regex.Replace(item.Text, @"(?<!\w)" + cl.ColumnName + @"\»", dr[cl.ColumnName].ToString());
                            item.Text = global::System.Text.RegularExpressions.Regex.Replace(item.Text, @"^\s*\>", "");
                            item.Text = global::System.Text.RegularExpressions.Regex.Replace(item.Text, @"^\s*\»", "");
                            FindSingleAnglebrackets = false;
                            breakFlag = false;
                            breakedFiled.Clear();
                        }
                    } //end of each columns
                    PreItem = item;
                    PreItemConstant = item.Text;
                    if (breakFlag
                        || (item.Text.Contains("<") && !item.Text.Contains(">"))
                        || (item.Text.Contains("«") && !item.Text.Contains("»"))
                       )
                    {
                        breakFlag = true;
                        breakedFiled.Add(item);
                        string combinedfiled = "";
                        foreach (Text t in breakedFiled)
                        {
                            combinedfiled += t.Text;
                        }
                        foreach (DataColumn cl in columns)
                        {
                            //<Today>
                            if (combinedfiled.Contains("«" + cl.ColumnName + "»") || combinedfiled.Contains("<" + cl.ColumnName + ">"))
                            {
                                //for the first part, remove the last '<' and tailing content
                                breakedFiled[0].Text = global::System.Text.RegularExpressions.Regex.Replace(breakedFiled[0].Text, @"<\w*$", "");
                                breakedFiled[0].Text = global::System.Text.RegularExpressions.Regex.Replace(breakedFiled[0].Text, @"<\w*$", "");

                                //remove middle parts
                                foreach (Text t in breakedFiled)
                                {
                                    if (!t.Text.Contains("<") && !t.Text.Contains("«") && !t.Text.Contains(">") && !t.Text.Contains("»"))
                                    {
                                        t.Text = "";
                                    }
                                }

                                //for the last part(as current item), remove leading content till the first '>' 
                                item.Text = global::System.Text.RegularExpressions.Regex.Replace(item.Text, @"^\s*\>", dr[cl.ColumnName].ToString());
                                item.Text = global::System.Text.RegularExpressions.Regex.Replace(item.Text, @"^\s*\»", dr[cl.ColumnName].ToString());

                                FindSingleAnglebrackets = false;
                                breakFlag = false;
                                breakedFiled.Clear();
                                break;
                            }
                        }
                    }
                }//end of each item
                #region go through footer
                MainDocumentPart mainPart = doc.MainDocumentPart;
                foreach (FooterPart footerPart in mainPart.FooterParts)
                {
                    Footer footer = footerPart.Footer;
                    var allFooterParas = footer.Descendants<Text>();
                    foreach (Text item in allFooterParas)
                    {
                        foreach (DataColumn cl in columns)
                        {
                            if (item.Text.Contains("«" + cl.ColumnName + "»") || item.Text.Contains("<" + cl.ColumnName + ">"))
                            {
                                item.Text = (string.IsNullOrEmpty(dr[cl.ColumnName].ToString()) ? " " : dr[cl.ColumnName].ToString());
                                FindSingleAnglebrackets = false;
                            }
                            else if (PreItem != null && (PreItem.Text == "<" || PreItem.Text == "«") && (item.Text.Trim() == cl.ColumnName))
                            {
                                FindSingleAnglebrackets = true;
                                PreItem.Text = "";
                                item.Text = (string.IsNullOrEmpty(dr[cl.ColumnName].ToString()) ? " " : dr[cl.ColumnName].ToString());
                            }
                            else if (FindSingleAnglebrackets && (item.Text == "»" || item.Text == ">"))
                            {
                                item.Text = "";
                                FindSingleAnglebrackets = false;
                            }
                        }
                        PreItem = item;
                    }
                }
                #endregion

                doc.MainDocumentPart.Document.Save();
            }
        }
        public static void MergeDocuments(params string[] filepaths)
        {

            //filepaths = new[] { "D:\\one.docx", "D:\\two.docx", "D:\\three.docx", "D:\\four.docx", "D:\\five.docx" };
            if (filepaths != null && filepaths.Length > 1)

                using (WordprocessingDocument myDoc = WordprocessingDocument.Open(@filepaths[0], true))
                {
                    MainDocumentPart mainPart = myDoc.MainDocumentPart;

                    for (int i = 1; i < filepaths.Length; i++)
                    {
                        string altChunkId = "AltChunkId" + i;
                        AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(
                            AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                        using (FileStream fileStream = File.Open(@filepaths[i], FileMode.Open))
                        {
                            chunk.FeedData(fileStream);
                        }
                        DocumentFormat.OpenXml.Wordprocessing.AltChunk altChunk = new DocumentFormat.OpenXml.Wordprocessing.AltChunk();
                        altChunk.Id = altChunkId;
                        //new page, if you like it...
                        mainPart.Document.Body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
                        //next document
                        mainPart.Document.Body.InsertAfter(altChunk, mainPart.Document.Body.Elements<Paragraph>().Last());
                    }
                    mainPart.Document.Save();
                    myDoc.Close();
                }
        }
        /*
        public static void doc2PDF(string inputFile, string outputFile)
        {
            SautinSoft.UseOffice u = new SautinSoft.UseOffice();
            if (u.InitWord() == 0)
            {
                //convert Word (RTF, DOC, DOCX to PDF)
                u.ConvertFile(inputFile, outputFile, SautinSoft.UseOffice.eDirection.DOC_to_PDF);
            }
            u.CloseOffice();

            //need to do something to remove Watermark
            
            
        }
        */
        #endregion

        #region report fast mode function

        /****** 
         ******  This function is on apply based
         ******/
        public static decimal GetBCBalance(int bodycorp_id, DateTime start, DateTime end, string chartMasterId = null, Odbc myOdbc = null)
        {
            decimal total = 0;
            Odbc o = myOdbc;
            if (o == null)
            {
                o = new Odbc(AdFunction.conn_sms);
            }
            if(chartMasterId == null)
            {
                string sql_bc_coa = "select bodycorp_account_id from bodycorps where bodycorp_id = " + bodycorp_id;
                var dtBcCoa = o.ReturnTable(sql_bc_coa, "bccoa");
                if(dtBcCoa.Rows.Count > 0)
                {
                    chartMasterId = dtBcCoa.Rows[0][0].ToString();
                }
                else
                {
                    throw new Exception("Failed to find Bodycorp Account");
                }
            }


            #region Opening Balance

            string sql = "SELECT SUM(`invoice_master_gross`) as total FROM `invoice_master` WHERE "
                    + " (`invoice_master_date`<" + DBSafeUtils.DateToSQL(start.AddYears(-20)) + ") and invoice_master_bodycorp_id=" + bodycorp_id;
            OdbcDataReader dr1 = o.Reader(sql);
            if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
            {
                total += decimal.Parse(dr1["total"].ToString());
            }

            sql = "SELECT SUM(`receipt_gross`) as total FROM `receipts` WHERE (`receipt_date`<" + DBSafeUtils.DateToSQL(start.AddYears(-20))
                + ") and receipt_bodycorp_id=" + bodycorp_id;
            dr1 = o.Reader(sql);
            if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
            {
                total -= decimal.Parse(dr1["total"].ToString());
            }

            sql = "SELECT SUM(`gl_transaction_net`) as total "
                + " FROM `gl_transactions`  "
                + "       LEFT JOIN `chart_master` ON `gl_transaction_chart_id`=`chart_master_id`  "
                + "       LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                + "                  FROM gl_transactions, gl_tran_gls "
                + "                  WHERE gl_transaction_bodycorp_id = " + bodycorp_id
                + "                    AND gl_transaction_type_id = 3 "
                + "                    AND gl_transaction_ref_type_id = 3 "
                + "                    AND gl_transaction_rev = 0 "
                + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                + "  WHERE `gl_transaction_date` < " + DBSafeUtils.DateToSQL(start.AddYears(-20))
                + "   AND gl_transaction_chart_id in (" + chartMasterId + ")  AND (`gl_transaction_type_id`=6) and gl_transaction_bodycorp_id = " + bodycorp_id;
            dr1 = o.Reader(sql);
            if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
            {
                total -= decimal.Parse(dr1["total"].ToString());
            }
            #endregion
            #region income
            sql = "SELECT sum(invoice_master_gross) as total FROM `invoice_master` WHERE "
                + "( (`invoice_master_apply` is not null and `invoice_master_apply` BETWEEN "
                    + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") "
                + " or (`invoice_master_apply` is null and `invoice_master_date` BETWEEN "
                    + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + " )"
                + " ) and invoice_master_bodycorp_id=" + bodycorp_id;

            dr1 = o.Reader(sql);
            if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
            {
                total += decimal.Parse(dr1["total"].ToString());
            }
            #endregion
            #region deposit
            sql = "SELECT sum(receipt_gross) as total FROM `receipts` WHERE (`receipt_date` BETWEEN "
                    + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") and receipt_bodycorp_id =" + bodycorp_id;
            dr1 = o.Reader(sql);
            if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
            {
                total -= decimal.Parse(dr1["total"].ToString());
            }
            #endregion
            #region journal
            sql = "SELECT sum(gl_transaction_net) as total "
                    + " FROM gl_transactions "
                    + "      LEFT JOIN chart_master ON gl_transaction_chart_id = chart_master_id "
                    + "      LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                    + "                  FROM gl_transactions, gl_tran_gls "
                    + "                  WHERE gl_transaction_bodycorp_id = " + bodycorp_id
                    + "                    AND gl_transaction_type_id = 3 "
                    + "                    AND gl_transaction_ref_type_id = 3 "
                    + "                    AND gl_transaction_rev = 0 "
                    + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                    + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                    + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                    + " WHERE gl_transaction_bodycorp_id = " + bodycorp_id
                    + "   AND chart_master_id in (" + chartMasterId + ") "
                    + "   AND gl_transaction_type_id IN (6, 7, 8) "
                    + "   AND gl_transaction_date BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end);
            dr1 = o.Reader(sql);
            if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
            {
                total -= decimal.Parse(dr1["total"].ToString());
            }
            #endregion

            return total;
        }

        /****** 
 ******  This function is on apply based
 ******/
        public static decimal GetUnitBalance(string unit_id, DateTime start, DateTime end, Odbc o = null)
        {
            if (o == null)
            {
                o = new Odbc(AdFunction.conn_sms);
            }
            SMS.system s = new SMS.system(AdFunction.conn_sms);
            s.SetOdbc(o);
            s.LoadData("GENERALDEBTOR");
            string[] debtor_chart_ids;
            string[] gcodes = s.system_value.Split('|');
            debtor_chart_ids = new string[gcodes.Length];
            for (int i = 0; i < gcodes.Length; i++)
            {
                string temp = gcodes[i];
                SMS.chart_master cm = new SMS.chart_master();
                cm.SetOdbc(o);
                cm.LoadData(temp);
                debtor_chart_ids[i] = cm.chart_master_code;
            }
            decimal total = 0;
            string sql = "";
            #region income
            sql = "SELECT sum(invoice_master_gross) as total FROM `invoice_master` WHERE "
                + "( (`invoice_master_apply` is not null and `invoice_master_apply` BETWEEN "
                    + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") "
                + " or (`invoice_master_apply` is null and `invoice_master_date` BETWEEN "
                    + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + " )"
                + " ) and invoice_master_unit_id=" + unit_id;

            OdbcDataReader dr1 = o.Reader(sql);
            if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
            {
                total += decimal.Parse(dr1["total"].ToString());
            }
            #endregion
            #region deposit
            sql = "SELECT sum(receipt_gross) as total FROM `receipts` WHERE (`receipt_date` BETWEEN "
                    + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") and receipt_unit_id =" + unit_id;
            dr1 = o.Reader(sql);
            if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
            {
                total -= decimal.Parse(dr1["total"].ToString());
            }
            #endregion
            #region journal
            sql = "SELECT sum(gl_transaction_net) as total "
                    + " FROM gl_transactions "
                    + "      LEFT JOIN chart_master ON gl_transaction_chart_id = chart_master_id "
                    + "      LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                    + "                  FROM gl_transactions, gl_tran_gls "
                    + "                  WHERE gl_transaction_unit_id = " + unit_id
                    + "                    AND gl_transaction_type_id = 3 "
                    + "                    AND gl_transaction_ref_type_id = 3 "
                    + "                    AND gl_transaction_rev = 0 "
                    + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                    + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                    + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                    + " WHERE gl_transaction_unit_id = " + unit_id
                    + " AND gl_transaction_chart_id in (" + AdFunction.StringArray_String(debtor_chart_ids) + ")    AND gl_transaction_type_id IN (6, 7, 8) "
                    + "   AND gl_transaction_date BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end);
            dr1 = o.Reader(sql);
            if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
            {
                total -= decimal.Parse(dr1["total"].ToString());
            }
            #endregion

            return total;
        }


        public static DataTable GetUnitActivity(string bodycorp_id, DateTime start, DateTime end, string ChartMasterId = "All", string BaseDateKbn = "DueDate", int unit_master_id = 0, Odbc o = null)
        {
            Odbc mydb = o;
            try
            {
                if (mydb == null)
                {
                    mydb = new Odbc(AdFunction.conn_sms);
                }

                DataTable dt = new DataTable("activities");
                dt.Columns.Add("ID");
                dt.Columns.Add("InvDate");
                dt.Columns.Add("DueDate");
                dt.Columns.Add("order", typeof(DateTime));
                dt.Columns.Add("Ref");
                dt.Columns.Add("Description");
                dt.Columns.Add("Invoice");
                dt.Columns.Add("Receipt");
                dt.Columns.Add("Balance");
                dt.Columns.Add("Journal");
                dt.Columns.Add("Type");
                dt.Columns.Add("Rev");
                dt.Columns.Add("Rec");
                dt.Columns.Add("UnitID");   // Add 07/06/2016
                dt.Columns.Add("DebtorID"); // Add 07/06/2016
                dt.Columns.Add("Allocated"); // Add 20/06/2016 (Inv. Paid, Rec. Allocated, Jou. gl_transaction_net)

                #region Load System Chart ID

                SMS.system s = new SMS.system(AdFunction.conn_sms);
                s.SetOdbc(mydb);
                s.LoadData("GST Input");
                SMS.chart_master cm = new SMS.chart_master();
                cm.SetOdbc(mydb);
                cm.LoadData(s.system_value);
                string InputGstID = cm.chart_master_id.ToString();
                s.LoadData("GST Output");
                cm.LoadData(s.system_value);
                string OutputGstID = cm.chart_master_id.ToString();
                s.LoadData("GENERALTAX");
                cm.LoadData(s.system_value);
                string gstid = cm.chart_master_id.ToString();
                s.LoadData("GENERALDEBTOR");
                string[] debtor_chart_ids;
                if (ChartMasterId.Equals("All"))
                {
                    string[] gcodes = s.system_value.Split('|');
                    debtor_chart_ids = new string[gcodes.Length];
                    for (int i = 0; i < gcodes.Length; i++)
                    {
                        string temp = gcodes[i];
                        cm.LoadData(temp);
                        debtor_chart_ids[i] = cm.chart_master_id.ToString();
                    }
                }
                else
                {
                    debtor_chart_ids = new string[1];
                    debtor_chart_ids[0] = ChartMasterId;
                }



                s.LoadData("GENERALCREDITOR");
                cm.LoadData(s.system_value);
                string creditorID = cm.chart_master_id.ToString();
                s.LoadData("DISCOUNTCHARCODE");
                cm.LoadData(s.system_value);
                string discountID = cm.chart_master_id.ToString();

                #endregion
                if (unit_master_id > 0)
                {

                    string sql = "";
                    #region Opening Balance
                    decimal balance = 0;
                    decimal receipt = 0;
                    decimal invoice = 0;
                    decimal paid_balance = 0;   // Add 20/06/2016
                    int bcid = int.Parse(bodycorp_id);
                    {

                        sql = "SELECT SUM(`invoice_master_gross`), SUM(invoice_master_paid) FROM `invoice_master` WHERE (`invoice_master_unit_id`=" + unit_master_id
                        + ") AND (`invoice_master_date`<" + DBSafeUtils.DateToSQL(start) + ") and invoice_master_bodycorp_id=" + bodycorp_id;

                        OdbcDataReader reader = mydb.Reader(sql);

                        if (reader.Read())
                        {
                            object bal = reader[0];
                            if (bal != DBNull.Value)
                            {
                                invoice += Convert.ToDecimal(bal);
                                balance = Convert.ToDecimal(bal);
                            }

                            object paid_bal = reader[1];
                            if (paid_bal != DBNull.Value)
                            {
                                paid_balance += Convert.ToDecimal(paid_bal);
                            }
                        }

                        sql = "SELECT SUM(`receipt_gross`), SUM(receipt_allocated) FROM `receipts` WHERE (`receipt_unit_id`=" + unit_master_id + ") AND (`receipt_date`<" + DBSafeUtils.DateToSQL(start) + ") and receipt_bodycorp_id=" + bodycorp_id;

                        reader = mydb.Reader(sql);

                        if (reader.Read())
                        {
                            object bal = reader[0];
                            if (bal != DBNull.Value)
                            {
                                receipt += Convert.ToDecimal(bal);
                                balance -= Convert.ToDecimal(bal);
                            }

                            object paid_bal = reader[1];
                            if (paid_bal != DBNull.Value)
                            {
                                paid_balance -= Convert.ToDecimal(paid_bal);
                            }
                        }


                        sql = "SELECT SUM(`gl_transaction_net`), SUM(journal_allocated) "
                            + " FROM `gl_transactions`  "
                            + "       LEFT JOIN `chart_master` ON `gl_transaction_chart_id`=`chart_master_id`  "
                            + "       LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                            + "                  FROM gl_transactions, gl_tran_gls "
                            + "                  WHERE gl_transaction_bodycorp_id = " + bodycorp_id
                            + "                    AND gl_transaction_type_id = 3 "
                            + "                    AND gl_transaction_ref_type_id = 3 "
                            + "                    AND gl_transaction_rev = 0 "
                            + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                            + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                            + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                            + "  WHERE (`gl_transaction_unit_id`= " + unit_master_id + ") AND `gl_transaction_date` < " + DBSafeUtils.DateToSQL(start)
                            + "  AND (`chart_master_type_id`=3) AND (`gl_transaction_type_id`=6) and gl_transaction_bodycorp_id = " + bodycorp_id;

                        reader = mydb.Reader(sql);

                        if (reader.Read())
                        {
                            object bal = reader[0];
                            if (bal != DBNull.Value)
                            {
                                receipt += Convert.ToDecimal(bal);
                                balance -= Convert.ToDecimal(bal);
                            }

                            object paid_bal = reader[1];
                            if (paid_bal != DBNull.Value)
                            {
                                paid_balance -= Convert.ToDecimal(paid_bal);
                            }
                        }
                    }
                    DataRow nr2 = dt.NewRow();
                    nr2["ID"] = "0";
                    nr2["InvDate"] = start.AddDays(-1).ToString("dd/MM/yyyy");
                    nr2["order"] = start.AddDays(-1);
                    //nr2["DueDate"] = new DateTime();
                    nr2["DueDate"] = start.AddDays(-1).ToString("dd/MM/yyyy");  // Update 26/05/2016
                    nr2["Ref"] = "OB";
                    nr2["Description"] = "Opening Balance";

                    nr2["Invoice"] = invoice;
                    nr2["Receipt"] = receipt;
                    nr2["Balance"] = balance.ToString("0.00");
                    nr2["Allocated"] = paid_balance.ToString("0.00");  // Add 20/06/2016

                    dt.Rows.Add(nr2);
                    #endregion

                    #region INCOME

                    sql = "SELECT * FROM `invoice_master` WHERE (`invoice_master_unit_id`=" + unit_master_id + ") AND  (`invoice_master_date` BETWEEN "
                        + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ")";
                    OdbcDataReader dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        nr["ID"] = dr["invoice_master_id"].ToString();
                        nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["invoice_master_date"]).ToString("dd/MM/yyyy");
                        nr["order"] = DBSafeUtils.DBDateToDate(dr["invoice_master_date"]);

                        // Update 14/05/2016
                        if ("ApplyDate".Equals(BaseDateKbn))
                        {
                            if (dr["invoice_master_apply"].ToString().Equals(""))
                            {
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(nr["InvDate"]).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["invoice_master_apply"]).ToString("dd/MM/yyyy");
                            }
                        }
                        else
                        {
                            if (dr["invoice_master_due"].ToString().Equals(""))
                            {
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(nr["InvDate"]).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["invoice_master_due"]).ToString("dd/MM/yyyy");
                            }
                        }

                        nr["Ref"] = dr["invoice_master_num"].ToString();
                        nr["Description"] = dr["invoice_master_description"].ToString();
                        nr["Invoice"] = Convert.ToDecimal(dr["invoice_master_gross"]).ToString("0.00");
                        nr["Receipt"] = "";
                        nr["Balance"] = balance + decimal.Parse(dr["invoice_master_gross"].ToString());
                        balance = balance + decimal.Parse(dr["invoice_master_gross"].ToString());
                        nr["Type"] = "Invoice";
                        nr["UnitID"] = dr["invoice_master_unit_id"].ToString();     //Add 07/06/2016
                        nr["DebtorID"] = dr["invoice_master_debtor_id"].ToString(); //Add 07/06/2016
                        nr["Allocated"] = Convert.ToDecimal(dr["invoice_master_paid"]).ToString("0.00");  // Add 20/06/2016
                        dt.Rows.Add(nr);
                    }
                    #endregion

                    #region DEPOSIT
                    sql = "SELECT * FROM `receipts` WHERE (`receipt_unit_id`=" + unit_master_id + ") AND (`receipt_date` BETWEEN "
                        + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") and receipt_bodycorp_id =" + bodycorp_id;
                    dr = mydb.Reader(sql);
                    DataTable recrpitPayTypeDT = o.ReturnTable("select * from payment_types ", "payment_types");
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        nr["ID"] = dr["receipt_id"].ToString();
                        nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["receipt_date"]).ToString("dd/MM/yyyy");
                        nr["order"] = DBSafeUtils.DBDateToDate(dr["receipt_date"]);
                        nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["receipt_date"]).ToString("dd/MM/yyyy");
                        nr["Ref"] = dr["receipt_ref"].ToString();
                        //nr["Description"] = ReportDT.GetDataByColumn(recrpitPayTypeDT, "payment_type_id", dr["receipt_payment_type_id"].ToString(), "payment_type_name");
                        nr["Description"] = dr["receipt_notes"].ToString();
                        nr["Invoice"] = "";
                        nr["Receipt"] = Convert.ToDecimal(dr["receipt_gross"]).ToString("0.00");
                        nr["Balance"] = balance - decimal.Parse(dr["receipt_gross"].ToString());
                        balance = balance - decimal.Parse(dr["receipt_gross"].ToString());
                        nr["Type"] = "Receipt";
                        nr["Rev"] = "";
                        if (dr["receipt_reconciled"].ToString().Equals("1"))
                            nr["Rec"] = "Rec";
                        nr["UnitID"] = dr["receipt_unit_id"].ToString();     //Add 07/06/2016
                        nr["DebtorID"] = dr["receipt_debtor_id"].ToString(); //Add 07/06/2016
                        nr["Allocated"] = (-Convert.ToDecimal(dr["receipt_allocated"])).ToString("0.00");  // Add 20/06/2016
                        dt.Rows.Add(nr);
                    }
                    #endregion

                    #region JOURNAL
                    // Update 20/06/2016
                    //sql = "SELECT * FROM `gl_transactions` LEFT JOIN `chart_master` ON `gl_transaction_chart_id`=`chart_master_id`"
                    //    + " WHERE (`gl_transaction_unit_id`=" + unit_master_id + ") AND (`gl_transaction_date`BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ")"
                    //    + " AND (`chart_master_id` in (" + AdFunction.StringArray_String(debtor_chart_ids) + ")) AND (`gl_transaction_type_id`=6 or `gl_transaction_type_id`=7 or `gl_transaction_type_id`=8) and gl_transaction_bodycorp_id	=" + bodycorp_id;
                    sql = "SELECT distinct * "
                        + " FROM gl_transactions "
                        + "      LEFT JOIN chart_master ON gl_transaction_chart_id = chart_master_id "
                        + "      LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                        + "                  FROM gl_transactions, gl_tran_gls "
                        + "                  WHERE gl_transaction_bodycorp_id = " + bodycorp_id
                        + "                    AND gl_transaction_type_id = 3 "
                        + "                    AND gl_transaction_ref_type_id = 3 "
                        + "                    AND gl_transaction_rev = 0 "
                        + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                        + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                        + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                        + " WHERE gl_transaction_unit_id = " + unit_master_id
                        + "   AND gl_transaction_bodycorp_id = " + bodycorp_id
                        + "   AND chart_master_id in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") "
                        + "   AND gl_transaction_type_id IN (6, 7, 8) "
                        + "   AND gl_transaction_date BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end);

                    dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        if (dr["gl_transaction_rev"].ToString().Equals("1"))
                            nr["Rev"] = "Rev";
                        if (dr["gl_transaction_rec"].ToString().Equals("1"))
                            nr["Rec"] = "Rec";
                        nr["ID"] = dr["gl_transaction_id"].ToString();
                        nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]).ToString("dd/MM/yyyy");
                        nr["order"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]);
                        nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]).ToString("dd/MM/yyyy");
                        nr["Ref"] = dr["gl_transaction_ref"].ToString();
                        nr["Description"] = dr["gl_transaction_description"].ToString();
                        // Update 06/04/2016
                        if (Convert.ToDecimal(dr["gl_transaction_net"]) >= 0)
                        {
                            nr["Invoice"] = "";
                            nr["Receipt"] = (Convert.ToDecimal(dr["gl_transaction_net"])).ToString("0.00");
                        }
                        else
                        {
                            nr["Invoice"] = (-Convert.ToDecimal(dr["gl_transaction_net"])).ToString("0.00");
                            nr["Receipt"] = "";
                        }
                        nr["Balance"] = balance - decimal.Parse(dr["gl_transaction_net"].ToString());
                        balance = balance - decimal.Parse(dr["gl_transaction_net"].ToString());
                        nr["Type"] = "Journal";
                        if ("".Equals(dr["journal_allocated"].ToString()))
                        {
                            nr["Allocated"] = "0.00";
                        }
                        else
                        {
                            nr["Allocated"] = (-Convert.ToDecimal(dr["journal_allocated"])).ToString("0.00");  // Add 20/06/2016
                        }

                        //DataRow tempdr = ReportDT.GetDataRowByColumn(dt, "Ref", dr["gl_transaction_ref"].ToString());
                        //if (tempdr == null)
                        dt.Rows.Add(nr);
                        //else
                        //{
                        //    tempdr["Receipt"] = AdFunction.Rounded(nr["Receipt"].ToString(), 2) + AdFunction.Rounded(tempdr["Receipt"].ToString(), 2);
                        //    tempdr["Balance"] = balance + decimal.Parse(dr["gl_transaction_net"].ToString());
                        //    balance = balance + decimal.Parse(dr["gl_transaction_net"].ToString());
                        //}
                    }

                    {
                        sql = "SELECT * FROM `gl_transactions` WHERE  (`gl_transaction_unit_id`=" + unit_master_id + ") AND (`gl_transaction_date` BETWEEN "
+ DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") and gl_transaction_chart_id not in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") and (gl_transaction_type_id=6 or gl_transaction_type_id=7 or gl_transaction_type_id=8) and `gl_transaction_net`>0 group by gl_transaction_ref and gl_transaction_bodycorp_id =" + bodycorp_id;
                        dr = mydb.Reader(sql);
                        while (dr.Read())
                        {
                            bool exit = false;
                            foreach (DataRow searchRef in dt.Rows)
                            {
                                if (searchRef["Ref"].ToString() == dr["gl_transaction_ref"].ToString())
                                {
                                    exit = true;
                                    break;
                                }
                            }
                            if (exit)
                            {
                                continue;
                            }
                            DataRow nr = dt.NewRow();
                            nr["ID"] = dr["gl_transaction_id"].ToString();
                            nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]);
                            nr["order"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]);
                            nr["Ref"] = dr["gl_transaction_ref"].ToString();

                            nr["Description"] = dr["gl_transaction_description"].ToString();
                            sql = "SELECT sum(gl_transaction_net) FROM `gl_transactions` WHERE  (`gl_transaction_unit_id`=" + unit_master_id + ") AND (`gl_transaction_date` BETWEEN "
+ DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") and gl_transaction_chart_id not in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") and (gl_transaction_type_id=6 or gl_transaction_type_id=7 or gl_transaction_type_id=8) and `gl_transaction_net`>0 group by gl_transaction_ref and gl_transaction_bodycorp_id =" + bodycorp_id;
                            DataTable sumdt = mydb.ReturnTable(sql, "t1");
                            nr["Journal"] = sumdt.Rows[0][0].ToString();

                            //modified by teemo @20160825
                            nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]).ToString("dd/MM/yyyy");

                            nr["Type"] = "Journal";
                            if (dr["gl_transaction_rev"].ToString().Equals("1"))
                            {
                                nr["Rev"] = "Rev";
                                nr["Journal"] = 0;
                            }
                            if (dr["gl_transaction_rec"].ToString().Equals("1"))
                                nr["Rec"] = "Rec";
                            dt.Rows.Add(nr);

                        }
                    }

                    dt.DefaultView.Sort = "order";
                    dt = dt.DefaultView.ToTable();
                    decimal b = 0;
                    foreach (DataRow dr2 in dt.Rows)
                    {
                        decimal i = 0;
                        decimal r = 0;
                        decimal.TryParse(dr2["Invoice"].ToString(), out i);
                        decimal.TryParse(dr2["Receipt"].ToString(), out r);
                        b += i - r;
                        dr2["Balance"] = b;

                    }


                    #endregion
                }
                else if (unit_master_id == 0)
                {
                    #region Opening Balance
                    decimal balance = 0;
                    decimal receipt = 0;
                    decimal invoice = 0;
                    decimal paid_balance = 0;   // Add 20/06/2016

                    // Update 07/06/2016; 20/06/2016 
                    //string sql = "SELECT SUM(`invoice_master_gross`) FROM `invoice_master` WHERE (`invoice_master_unit_id`=" + unit_master_id
                    string sql = "SELECT SUM(`invoice_master_gross`), SUM(invoice_master_paid) FROM `invoice_master` WHERE (`invoice_master_unit_id` IS NULL "
                        + ") AND (`invoice_master_date`<" + DBSafeUtils.DateToSQL(start) + ") and invoice_master_bodycorp_id=" + bodycorp_id;
                    //object bal = mydb.ExecuteScalar(sql);
                    OdbcDataReader reader = mydb.Reader(sql);

                    if (reader.Read())
                    {
                        object bal = reader[0];
                        if (bal != DBNull.Value)
                        {
                            invoice += Convert.ToDecimal(bal);
                            balance = Convert.ToDecimal(bal);
                        }

                        object paid_bal = reader[1];
                        if (paid_bal != DBNull.Value)
                        {
                            paid_balance += Convert.ToDecimal(paid_bal);
                        }
                    }

                    // Update 20/06/2016 Add journal allocated
                    sql = "SELECT SUM(`receipt_gross`), SUM(receipt_allocated) FROM `receipts` WHERE (`receipt_unit_id`is null) AND (`receipt_date`<" + DBSafeUtils.DateToSQL(start) + ") and receipt_bodycorp_id=" + bodycorp_id;
                    //bal = mydb.ExecuteScalar(sql);
                    reader = mydb.Reader(sql);

                    if (reader.Read())
                    {
                        object bal = reader[0];
                        if (bal != DBNull.Value)
                        {
                            receipt += Convert.ToDecimal(bal);
                            balance -= Convert.ToDecimal(bal);
                        }

                        object paid_bal = reader[1];
                        if (paid_bal != DBNull.Value)
                        {
                            paid_balance -= Convert.ToDecimal(paid_bal);
                        }
                    }

                    // Update 20/06/2016 Add journal allocated
                    //sql = "SELECT SUM(`gl_transaction_net`) FROM `gl_transactions` LEFT JOIN `chart_master` ON `gl_transaction_chart_id`=`chart_master_id`"
                    //    + " WHERE (`gl_transaction_unit_id`is null) AND (`gl_transaction_date`<" + DBSafeUtils.DateToSQL(start) + ")"
                    //    + " AND (`chart_master_type_id`=3) AND (`gl_transaction_type_id`=6) and `gl_transaction_chart_id`in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") and `gl_transaction_bodycorp_id`=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value;
                    sql = "SELECT SUM(`gl_transaction_net`), SUM(journal_allocated) "
                        + " FROM `gl_transactions`  "
                        + "       LEFT JOIN `chart_master` ON `gl_transaction_chart_id`=`chart_master_id`  "
                        + "       LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                        + "                  FROM gl_transactions, gl_tran_gls "
                        + "                  WHERE gl_transaction_bodycorp_id = " + bodycorp_id
                        + "                    AND gl_transaction_type_id = 3 "
                        + "                    AND gl_transaction_ref_type_id = 3 "
                        + "                    AND gl_transaction_rev = 0 "
                        + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                        + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                        + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                        + "  WHERE (`gl_transaction_unit_id` IS NULL) AND `gl_transaction_date` < " + DBSafeUtils.DateToSQL(start)
                        + "  AND chart_master_id in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") AND (`gl_transaction_type_id`=6) and `gl_transaction_chart_id`in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") and gl_transaction_bodycorp_id = " + bodycorp_id;
                    //bal = mydb.ExecuteScalar(sql);
                    reader = mydb.Reader(sql);

                    if (reader.Read())
                    {
                        object bal = reader[0];
                        if (bal != DBNull.Value)
                        {
                            receipt += Convert.ToDecimal(bal);
                            balance -= Convert.ToDecimal(bal);
                        }

                        object paid_bal = reader[1];
                        if (paid_bal != DBNull.Value)
                        {
                            paid_balance -= Convert.ToDecimal(paid_bal);
                        }
                    }

                    DataRow nr2 = dt.NewRow();
                    nr2["ID"] = "0";
                    nr2["InvDate"] = start.AddDays(-1).ToString("dd/MM/yyyy");
                    nr2["DueDate"] = start.AddDays(-1).ToString("dd/MM/yyyy");
                    nr2["Ref"] = "OB";
                    nr2["Description"] = "Opening Balance";
                    nr2["Invoice"] = invoice;
                    nr2["Receipt"] = receipt;
                    nr2["Balance"] = balance.ToString("0.00");
                    nr2["Allocated"] = "0.00";  // Add 20/06/2016
                    dt.Rows.Add(nr2);
                    #endregion

                    #region INCOME
                    sql = "SELECT * FROM `invoice_master` WHERE (`invoice_master_unit_id`is null) AND  (`invoice_master_date` BETWEEN "
                        + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") and invoice_master_bodycorp_id=" + bodycorp_id;
                    OdbcDataReader dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        nr["ID"] = dr["invoice_master_id"].ToString();
                        nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["invoice_master_date"]).ToString("dd/MM/yyyy");

                        // Update 14/05/2016
                        if ("ApplyDate".Equals(BaseDateKbn))
                        {
                            if (dr["invoice_master_apply"].ToString().Equals(""))
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(nr["InvDate"]).ToString("dd/MM/yyyy");
                            else
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["invoice_master_apply"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            if (dr["invoice_master_due"].ToString().Equals(""))
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(nr["InvDate"]).ToString("dd/MM/yyyy");
                            else
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["invoice_master_due"]).ToString("dd/MM/yyyy");
                        }

                        nr["Ref"] = dr["invoice_master_num"].ToString();
                        nr["Description"] = dr["invoice_master_description"].ToString();
                        nr["Invoice"] = Convert.ToDecimal(dr["invoice_master_gross"]).ToString("0.00");
                        nr["Receipt"] = "";
                        nr["Balance"] = balance + decimal.Parse(dr["invoice_master_gross"].ToString());
                        balance = balance + decimal.Parse(dr["invoice_master_gross"].ToString());
                        nr["Type"] = "Invoice";
                        nr["UnitID"] = dr["invoice_master_unit_id"].ToString();     //Add 07/06/2016
                        nr["DebtorID"] = dr["invoice_master_debtor_id"].ToString(); //Add 07/06/2016
                        nr["Allocated"] = Convert.ToDecimal(dr["invoice_master_paid"]).ToString("0.00");  // Add 20/06/2016
                        dt.Rows.Add(nr);
                    }
                    #endregion

                    #region DEPOSIT
                    sql = "SELECT * FROM `receipts` WHERE (`receipt_unit_id`is null) AND (`receipt_date` BETWEEN "
                        + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") and receipt_bodycorp_id=" + bodycorp_id;
                    dr = mydb.Reader(sql);
                    DataTable recrpitPayTypeDT = o.ReturnTable("select * from payment_types", "payment_types");
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        nr["ID"] = dr["receipt_id"].ToString();
                        nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["receipt_date"]).ToString("dd/MM/yyyy");
                        nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["receipt_date"]).ToString("dd/MM/yyyy");
                        nr["Ref"] = dr["receipt_ref"].ToString();
                        //nr["Description"] = ReportDT.GetDataByColumn(recrpitPayTypeDT, "payment_type_id", dr["receipt_payment_type_id"].ToString(), "payment_type_name");
                        nr["Description"] = dr["receipt_notes"].ToString();
                        nr["Invoice"] = "";
                        nr["Receipt"] = Convert.ToDecimal(dr["receipt_gross"]).ToString("0.00");
                        nr["Balance"] = balance - decimal.Parse(dr["receipt_gross"].ToString());
                        balance = balance - decimal.Parse(dr["receipt_gross"].ToString());
                        nr["Type"] = "Receipt";
                        nr["UnitID"] = dr["receipt_unit_id"].ToString();     //Add 07/06/2016
                        nr["DebtorID"] = dr["receipt_debtor_id"].ToString(); //Add 07/06/2016
                        nr["Allocated"] = (-Convert.ToDecimal(dr["receipt_allocated"])).ToString("0.00");  // Add 20/06/2016
                        dt.Rows.Add(nr);
                    }
                    #endregion

                    #region JOURNAL
                    //s.LoadData("GENERALDEBTOR");
                    //string c = s.system_value;
                    //ChartMaster ch = new ChartMaster(constr);
                    //ch.LoadData(c);

                    // Update 20/06/2016
                    //sql = "SELECT * FROM `gl_transactions` WHERE `gl_transaction_bodycorp_id`=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value + " and `gl_transaction_chart_id`in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") and  (`gl_transaction_type_id`=6 or `gl_transaction_type_id`=7) and (`gl_transaction_unit_id`is null) AND (`gl_transaction_date`BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") AND gl_transaction_description not like '%RCPT%'";
                    sql = "SELECT * "
                        + " FROM gl_transactions "
                        + "      LEFT JOIN chart_master ON gl_transaction_chart_id = chart_master_id "
                        + "      LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                        + "                  FROM gl_transactions, gl_tran_gls "
                        + "                  WHERE gl_transaction_bodycorp_id = " + bodycorp_id
                        + "                    AND gl_transaction_type_id = 3 "
                        + "                    AND gl_transaction_ref_type_id = 3 "
                        + "                    AND gl_transaction_rev = 0 "
                        + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                        + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                        + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                        + " WHERE gl_transaction_unit_id IS NULL "
                        + "   AND gl_transaction_bodycorp_id = " + bodycorp_id
                        + "   AND chart_master_id in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") "
                        + "   AND gl_transaction_type_id IN (6, 7) "
                        + "   AND gl_transaction_date BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end)
                        + "   AND gl_transaction_description NOT LIKE '%RCPT%' ";

                    dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        nr["ID"] = dr["gl_transaction_id"].ToString();
                        nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]).ToString("dd/MM/yyyy");
                        nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]).ToString("dd/MM/yyyy");
                        nr["Ref"] = dr["gl_transaction_ref"].ToString();
                        nr["Description"] = dr["gl_transaction_description"].ToString();
                        // Update 06/04/2016
                        if (Convert.ToDecimal(dr["gl_transaction_net"]) >= 0)
                        {
                            nr["Invoice"] = "";
                            nr["Receipt"] = (Convert.ToDecimal(dr["gl_transaction_net"])).ToString("0.00");
                        }
                        else
                        {
                            nr["Invoice"] = (-Convert.ToDecimal(dr["gl_transaction_net"])).ToString("0.00");
                            nr["Receipt"] = "";
                        }
                        nr["Balance"] = balance - decimal.Parse(dr["gl_transaction_net"].ToString());
                        balance = balance - decimal.Parse(dr["gl_transaction_net"].ToString());
                        nr["Type"] = "Journal";
                        if ("".Equals(dr["journal_allocated"].ToString()))
                        {
                            nr["Allocated"] = "0.00";
                        }
                        else
                        {
                            nr["Allocated"] = (-Convert.ToDecimal(dr["journal_allocated"])).ToString("0.00");  // Add 20/06/2016
                        }

                        dt.Rows.Add(nr);
                    }
                    dt.DefaultView.Sort = "InvDate";
                    dt = dt.DefaultView.ToTable();
                    decimal b = 0;
                    foreach (DataRow dr2 in dt.Rows)
                    {
                        decimal i = 0;
                        decimal r = 0;
                        decimal.TryParse(dr2["Invoice"].ToString(), out i);
                        decimal.TryParse(dr2["Receipt"].ToString(), out r);
                        b += i - r;
                        dr2["Balance"] = b;

                    }


                    #endregion
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (mydb != null && o == null) mydb.Close();
            }
        }

        public static DataTable GetUnitActivity_sms(string bodycorp_id, DateTime start, DateTime end, string ChartMasterId = "All", string BaseDateKbn = "DueDate", int unit_master_id = 0, Odbc o = null)
        {
            Odbc mydb = o;
            try
            {
                if (mydb == null)
                {
                    mydb = new Odbc(AdFunction.conn_sms);
                }
                DataTable dt = new DataTable("activities");
                dt.Columns.Add("ID");
                dt.Columns.Add("InvDate", typeof(DateTime));
                dt.Columns.Add("DueDate", typeof(DateTime));
                dt.Columns.Add("Ref");
                dt.Columns.Add("Description");
                dt.Columns.Add("Invoice");
                dt.Columns.Add("Receipt");
                dt.Columns.Add("Balance");
                dt.Columns.Add("Journal");
                dt.Columns.Add("Type");
                dt.Columns.Add("Rev");
                dt.Columns.Add("Rec");
                dt.Columns.Add("UnitID");   // Add 07/06/2016
                dt.Columns.Add("DebtorID"); // Add 07/06/2016
                dt.Columns.Add("Allocated"); // Add 20/06/2016 (Inv. Paid, Rec. Allocated, Jou. gl_transaction_net)

                #region Load System Chart ID

                SMS.system s = new SMS.system(AdFunction.conn_sms);
                s.SetOdbc(mydb);
                SMS.chart_master ch = new SMS.chart_master();
                ch.SetOdbc(mydb);
                s.LoadData("GST Input");
                ch.LoadData(s.system_value);
                string InputGstID = ch.chart_master_id.ToString();
                s.LoadData("GST Output");
                ch.LoadData(s.system_value);
                string OutputGstID = ch.chart_master_id.ToString();
                s.LoadData("GENERALTAX");
                ch.LoadData(s.system_value);
                string gstid = ch.chart_master_id.ToString();
                s.LoadData("GENERALDEBTOR");
                string[] debtor_chart_ids;
                if (ChartMasterId.Equals("All"))
                {
                    string[] gcodes = s.system_value.Split('|');
                    debtor_chart_ids = new string[gcodes.Length];
                    for (int i = 0; i < gcodes.Length; i++)
                    {
                        ch.LoadData(gcodes[i]);
                        debtor_chart_ids[i] = ch.chart_master_id.ToString();
                    }
                }
                else
                {
                    debtor_chart_ids = new string[1];
                    debtor_chart_ids[0] = ChartMasterId;
                }



                s.LoadData("GENERALCREDITOR");
                ch.LoadData(s.system_value);
                string creditorID = ch.chart_master_id.ToString();
                s.LoadData("DISCOUNTCHARCODE");
                ch.LoadData(s.system_value);
                string discountID = ch.chart_master_id.ToString();

                #endregion
                if (unit_master_id > 0)
                {

                    string sql = "";
                    #region Opening Balance
                    decimal balance = 0;
                    decimal receipt = 0;
                    decimal invoice = 0;
                    decimal paid_balance = 0;   // Add 20/06/2016
                    {
                        {
                            string dateRestrict = "";
                            if ("ApplyDate".Equals(BaseDateKbn))
                            {
                                dateRestrict = " and ( (`invoice_master_apply` is not null and `invoice_master_apply` < "
                                + DBSafeUtils.DateToSQL(start) + ") "
                                + " or (`invoice_master_apply` is null and `invoice_master_date` < "
                                    + DBSafeUtils.DateToSQL(start) + " )"
                                + " ) ";
                            }
                            else
                            {
                                dateRestrict = " AND (`invoice_master_date`<" + DBSafeUtils.DateToSQL(start) + " ) ";
                            }
                            sql = "SELECT SUM(`invoice_master_gross`), SUM(invoice_master_paid) FROM `invoice_master` WHERE (`invoice_master_unit_id`=" + unit_master_id + " ) "
                           + dateRestrict;
                        }


                        OdbcDataReader reader = mydb.Reader(sql);

                        if (reader.Read())
                        {
                            object bal = reader[0];
                            if (bal != DBNull.Value)
                            {
                                invoice += Convert.ToDecimal(bal);
                                balance = Convert.ToDecimal(bal);
                            }

                            object paid_bal = reader[1];
                            if (paid_bal != DBNull.Value)
                            {
                                paid_balance += Convert.ToDecimal(paid_bal);
                            }
                        }

                        {
                            sql = "SELECT SUM(`receipt_gross`), SUM(receipt_allocated) FROM `receipts` WHERE (`receipt_unit_id`=" + unit_master_id + ") AND (`receipt_date`<" + DBSafeUtils.DateToSQL(start) + ") ";
                        }
                        reader = mydb.Reader(sql);

                        if (reader.Read())
                        {
                            object bal = reader[0];
                            if (bal != DBNull.Value)
                            {
                                receipt += Convert.ToDecimal(bal);
                                balance -= Convert.ToDecimal(bal);
                            }

                            object paid_bal = reader[1];
                            if (paid_bal != DBNull.Value)
                            {
                                paid_balance -= Convert.ToDecimal(paid_bal);
                            }
                        }

                        {
                            sql = "SELECT SUM(`gl_transaction_net`), SUM(journal_allocated) "
                                + " FROM `gl_transactions`  "
                                + "       LEFT JOIN `chart_master` ON `gl_transaction_chart_id`=`chart_master_id`  "
                                + "       LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                                + "                  FROM gl_transactions, gl_tran_gls "
                                + "                  WHERE  gl_transaction_type_id = 3 "
                                + "                    AND gl_transaction_ref_type_id = 3 "
                                + "                    AND gl_transaction_rev = 0 "
                                + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                                + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                                + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                                + "  WHERE (`gl_transaction_unit_id`= " + unit_master_id + ") AND `gl_transaction_date` < " + DBSafeUtils.DateToSQL(start)
                                + "  AND (chart_master_id in (" + AdFunction.StringArray_String(debtor_chart_ids) + ")  ) AND (`gl_transaction_type_id`=6) ";
                        }
                        reader = mydb.Reader(sql);

                        if (reader.Read())
                        {
                            object bal = reader[0];
                            if (bal != DBNull.Value)
                            {
                                receipt += Convert.ToDecimal(bal);
                                balance -= Convert.ToDecimal(bal);
                            }

                            object paid_bal = reader[1];
                            if (paid_bal != DBNull.Value)
                            {
                                paid_balance -= Convert.ToDecimal(paid_bal);
                            }
                        }
                    }
                    DataRow nr2 = dt.NewRow();
                    nr2["ID"] = "0";
                    nr2["InvDate"] = start.AddDays(-1).ToString("dd/MM/yyyy");
                    //nr2["DueDate"] = new DateTime();
                    nr2["DueDate"] = start.AddDays(-1).ToString("dd/MM/yyyy");  // Update 26/05/2016
                    nr2["Ref"] = "OB";
                    nr2["Description"] = "Opening Balance";
                   
                    {
                        nr2["Invoice"] = invoice;
                        nr2["Receipt"] = receipt;
                        nr2["Balance"] = balance.ToString("0.00");
                        nr2["Allocated"] = paid_balance.ToString("0.00");  // Add 20/06/2016
                    }
                    dt.Rows.Add(nr2);
                    #endregion

                    #region INCOME
                    string dateRestrict1 = "";
                    if ("ApplyDate".Equals(BaseDateKbn))
                    {
                        dateRestrict1 = " and ( (`invoice_master_apply` is not null and `invoice_master_apply` BETWEEN "
                        + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") "
                        + " or (`invoice_master_apply` is null and `invoice_master_date` BETWEEN "
                            + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + " )"
                        + " ) ";
                    }
                    else
                    {
                        dateRestrict1 = " AND  (`invoice_master_date` BETWEEN "
                        + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ")";
                    }

                    sql = "SELECT * FROM `invoice_master` WHERE (`invoice_master_unit_id`=" + unit_master_id + ") "
                        + dateRestrict1;

                    OdbcDataReader dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        nr["ID"] = dr["invoice_master_id"].ToString();
                        nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["invoice_master_date"]).ToString("dd/MM/yyyy");

                        // Update 14/05/2016
                        if ("ApplyDate".Equals(BaseDateKbn))
                        {
                            if (dr["invoice_master_apply"].ToString().Equals(""))
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(nr["InvDate"]).ToString("dd/MM/yyyy");
                            else
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["invoice_master_apply"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            if (dr["invoice_master_due"].ToString().Equals(""))
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(nr["InvDate"]).ToString("dd/MM/yyyy");
                            else
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["invoice_master_due"]).ToString("dd/MM/yyyy");
                        }

                        nr["Ref"] = dr["invoice_master_num"].ToString();
                        nr["Description"] = dr["invoice_master_description"].ToString();
                        nr["Invoice"] = Convert.ToDecimal(dr["invoice_master_gross"]).ToString("0.00");
                        nr["Receipt"] = "";
                        nr["Balance"] = balance + decimal.Parse(dr["invoice_master_gross"].ToString());
                        balance = balance + decimal.Parse(dr["invoice_master_gross"].ToString());
                        nr["Type"] = "Invoice";
                        nr["UnitID"] = dr["invoice_master_unit_id"].ToString();     //Add 07/06/2016
                        nr["DebtorID"] = dr["invoice_master_debtor_id"].ToString(); //Add 07/06/2016
                        nr["Allocated"] = Convert.ToDecimal(dr["invoice_master_paid"]).ToString("0.00");  // Add 20/06/2016
                        dt.Rows.Add(nr);
                    }
                    #endregion

                    #region DEPOSIT
                    sql = "SELECT * FROM `receipts` WHERE (`receipt_unit_id`=" + unit_master_id + ") AND (`receipt_date` BETWEEN "
                        + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") ";
                    dr = mydb.Reader(sql);

                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        nr["ID"] = dr["receipt_id"].ToString();
                        nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["receipt_date"]).ToString("dd/MM/yyyy");
                        nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["receipt_date"]).ToString("dd/MM/yyyy");
                        nr["Ref"] = dr["receipt_ref"].ToString();
                        //nr["Description"] = ReportDT.GetDataByColumn(recrpitPayTypeDT, "payment_type_id", dr["receipt_payment_type_id"].ToString(), "payment_type_name");
                        nr["Description"] = dr["receipt_notes"].ToString();
                        nr["Invoice"] = "";
                        nr["Receipt"] = Convert.ToDecimal(dr["receipt_gross"]).ToString("0.00");
                        nr["Balance"] = balance - decimal.Parse(dr["receipt_gross"].ToString());
                        balance = balance - decimal.Parse(dr["receipt_gross"].ToString());
                        nr["Type"] = "Receipt";
                        nr["Rev"] = "";
                        if (dr["receipt_reconciled"].ToString().Equals("1"))
                            nr["Rec"] = "Rec";
                        nr["UnitID"] = dr["receipt_unit_id"].ToString();     //Add 07/06/2016
                        nr["DebtorID"] = dr["receipt_debtor_id"].ToString(); //Add 07/06/2016
                        nr["Allocated"] = (-Convert.ToDecimal(dr["receipt_allocated"])).ToString("0.00");  // Add 20/06/2016
                        dt.Rows.Add(nr);
                    }
                    #endregion

                    #region JOURNAL
                    // Update 20/06/2016
                    //sql = "SELECT * FROM `gl_transactions` LEFT JOIN `chart_master` ON `gl_transaction_chart_id`=`chart_master_id`"
                    //    + " WHERE (`gl_transaction_unit_id`=" + unit_master_id + ") AND (`gl_transaction_date`BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ")"
                    //    + " AND (`chart_master_id` in (" + AdFunction.StringArray_String(debtor_chart_ids) + ")) AND (`gl_transaction_type_id`=6 or `gl_transaction_type_id`=7 or `gl_transaction_type_id`=8) and gl_transaction_bodycorp_id	=" + bodycorp_id;
                    sql = "SELECT distinct * "
                        + " FROM gl_transactions "
                        + "      LEFT JOIN chart_master ON gl_transaction_chart_id = chart_master_id "
                        + "      LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                        + "                  FROM gl_transactions, gl_tran_gls "
                        + "                  WHERE  gl_transaction_type_id = 3 "
                        + "                    AND gl_transaction_ref_type_id = 3 "
                        + "                    AND gl_transaction_rev = 0 "
                        + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                        + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                        + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                        + " WHERE gl_transaction_unit_id = " + unit_master_id
                        + "   AND chart_master_id in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") "
                        + "   AND gl_transaction_type_id IN (6, 7, 8) "
                        + "   AND gl_transaction_date BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end);

                    dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        if (dr["gl_transaction_rev"].ToString().Equals("1"))
                            nr["Rev"] = "Rev";
                        if (dr["gl_transaction_rec"].ToString().Equals("1"))
                            nr["Rec"] = "Rec";
                        nr["ID"] = dr["gl_transaction_id"].ToString();
                        nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]).ToString("dd/MM/yyyy");
                        nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]).ToString("dd/MM/yyyy");
                        nr["Ref"] = dr["gl_transaction_ref"].ToString();
                        nr["Description"] = dr["gl_transaction_description"].ToString();
                        // Update 06/04/2016
                        if (Convert.ToDecimal(dr["gl_transaction_net"]) >= 0)
                        {
                            nr["Invoice"] = "";
                            nr["Receipt"] = (Convert.ToDecimal(dr["gl_transaction_net"])).ToString("0.00");
                        }
                        else
                        {
                            nr["Invoice"] = (-Convert.ToDecimal(dr["gl_transaction_net"])).ToString("0.00");
                            nr["Receipt"] = "";
                        }
                        nr["Balance"] = balance - decimal.Parse(dr["gl_transaction_net"].ToString());
                        balance = balance - decimal.Parse(dr["gl_transaction_net"].ToString());
                        nr["Type"] = "Journal";
                        if ("".Equals(dr["journal_allocated"].ToString()))
                        {
                            nr["Allocated"] = "0.00";
                        }
                        else
                        {
                            nr["Allocated"] = (-Convert.ToDecimal(dr["journal_allocated"])).ToString("0.00");  // Add 20/06/2016
                        }

                        //DataRow tempdr = ReportDT.GetDataRowByColumn(dt, "Ref", dr["gl_transaction_ref"].ToString());
                        //if (tempdr == null)
                        dt.Rows.Add(nr);
                        //else
                        //{
                        //    tempdr["Receipt"] = AdFunction.Rounded(nr["Receipt"].ToString(), 2) + AdFunction.Rounded(tempdr["Receipt"].ToString(), 2);
                        //    tempdr["Balance"] = balance + decimal.Parse(dr["gl_transaction_net"].ToString());
                        //    balance = balance + decimal.Parse(dr["gl_transaction_net"].ToString());
                        //}
                    }

                    {
                        sql = "SELECT * FROM `gl_transactions` WHERE  (`gl_transaction_unit_id`=" + unit_master_id + ") AND (`gl_transaction_date` BETWEEN "
+ DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") and gl_transaction_chart_id not in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") and (gl_transaction_type_id=6 or gl_transaction_type_id=7 or gl_transaction_type_id=8) and `gl_transaction_net`>0 group by gl_transaction_ref ";
                        dr = mydb.Reader(sql);
                        while (dr.Read())
                        {
                            bool exit = false;
                            foreach (DataRow searchRef in dt.Rows)
                            {
                                if (searchRef["Ref"].ToString() == dr["gl_transaction_ref"].ToString())
                                {
                                    exit = true;
                                    break;
                                }
                            }
                            if (exit)
                            {
                                continue;
                            }
                            DataRow nr = dt.NewRow();
                            nr["ID"] = dr["gl_transaction_id"].ToString();
                            nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]);
                            nr["Ref"] = dr["gl_transaction_ref"].ToString();

                            nr["Description"] = dr["gl_transaction_description"].ToString();
                            sql = "SELECT sum(gl_transaction_net) FROM `gl_transactions` WHERE  (`gl_transaction_unit_id`=" + unit_master_id + ") AND (`gl_transaction_date` BETWEEN "
+ DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") and gl_transaction_chart_id not in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") and (gl_transaction_type_id=6 or gl_transaction_type_id=7 or gl_transaction_type_id=8) and `gl_transaction_net`>0 group by gl_transaction_ref ";
                            DataTable sumdt = mydb.ReturnTable(sql, "t1");
                            nr["Journal"] = sumdt.Rows[0][0].ToString();

                            //modified by teemo @20160825
                            nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]).ToString("dd/MM/yyyy");

                            nr["Type"] = "Journal";
                            if (dr["gl_transaction_rev"].ToString().Equals("1"))
                            {
                                nr["Rev"] = "Rev";
                                nr["Journal"] = 0;
                            }
                            if (dr["gl_transaction_rec"].ToString().Equals("1"))
                                nr["Rec"] = "Rec";
                            dt.Rows.Add(nr);

                        }
                    }

                    dt.DefaultView.Sort = "DueDate";
                    dt = dt.DefaultView.ToTable();
                    decimal b = 0;
                    foreach (DataRow dr2 in dt.Rows)
                    {
                        decimal i = 0;
                        decimal r = 0;
                        decimal.TryParse(dr2["Invoice"].ToString(), out i);
                        decimal.TryParse(dr2["Receipt"].ToString(), out r);
                        b += i - r;
                        dr2["Balance"] = b;

                    }


                    #endregion
                }
                else if (unit_master_id == 0)
                {

                    #region Opening Balance
                    decimal balance = 0;
                    decimal receipt = 0;
                    decimal invoice = 0;
                    decimal paid_balance = 0;   // Add 20/06/2016

                    // Update 07/06/2016; 20/06/2016 
                    //string sql = "SELECT SUM(`invoice_master_gross`) FROM `invoice_master` WHERE (`invoice_master_unit_id`=" + unit_master_id

                    string dateRestrict = "";
                    if ("ApplyDate".Equals(BaseDateKbn))
                    {
                        dateRestrict = " and ( (`invoice_master_apply` is not null and `invoice_master_apply` < "
                        + DBSafeUtils.DateToSQL(start) + ") "
                        + " or (`invoice_master_apply` is null and `invoice_master_date` < "
                            + DBSafeUtils.DateToSQL(start) + " )"
                        + " ) ";
                    }
                    else
                    {
                        dateRestrict = " AND (`invoice_master_date`<" + DBSafeUtils.DateToSQL(start) + " ) ";
                    }

                    string sql = "SELECT SUM(`invoice_master_gross`), SUM(invoice_master_paid) FROM `invoice_master` WHERE (`invoice_master_unit_id` IS NULL ) "
                        + dateRestrict
                        + " and invoice_master_bodycorp_id=" + bodycorp_id;
                    //object bal = mydb.ExecuteScalar(sql);
                    OdbcDataReader reader = mydb.Reader(sql);

                    if (reader.Read())
                    {
                        object bal = reader[0];
                        if (bal != DBNull.Value)
                        {
                            invoice += Convert.ToDecimal(bal);
                            balance = Convert.ToDecimal(bal);
                        }

                        object paid_bal = reader[1];
                        if (paid_bal != DBNull.Value)
                        {
                            paid_balance += Convert.ToDecimal(paid_bal);
                        }
                    }

                    // Update 20/06/2016 Add journal allocated
                    sql = "SELECT SUM(`receipt_gross`), SUM(receipt_allocated) FROM `receipts` WHERE (`receipt_unit_id`is null) AND (`receipt_date`<" + DBSafeUtils.DateToSQL(start) + ") and receipt_bodycorp_id=" + bodycorp_id;
                    //bal = mydb.ExecuteScalar(sql);
                    reader = mydb.Reader(sql);

                    if (reader.Read())
                    {
                        object bal = reader[0];
                        if (bal != DBNull.Value)
                        {
                            receipt += Convert.ToDecimal(bal);
                            balance -= Convert.ToDecimal(bal);
                        }

                        object paid_bal = reader[1];
                        if (paid_bal != DBNull.Value)
                        {
                            paid_balance -= Convert.ToDecimal(paid_bal);
                        }
                    }

                    // Update 20/06/2016 Add journal allocated
                    //sql = "SELECT SUM(`gl_transaction_net`) FROM `gl_transactions` LEFT JOIN `chart_master` ON `gl_transaction_chart_id`=`chart_master_id`"
                    //    + " WHERE (`gl_transaction_unit_id`is null) AND (`gl_transaction_date`<" + DBSafeUtils.DateToSQL(start) + ")"
                    //    + " AND (`chart_master_type_id`=3) AND (`gl_transaction_type_id`=6) and `gl_transaction_chart_id`in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") and `gl_transaction_bodycorp_id`=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value;
                    sql = "SELECT SUM(`gl_transaction_net`), SUM(journal_allocated) "
                        + " FROM `gl_transactions`  "
                        + "       LEFT JOIN `chart_master` ON `gl_transaction_chart_id`=`chart_master_id`  "
                        + "       LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                        + "                  FROM gl_transactions, gl_tran_gls "
                        + "                  WHERE gl_transaction_bodycorp_id = " + bodycorp_id
                        + "                    AND gl_transaction_type_id = 3 "
                        + "                    AND gl_transaction_ref_type_id = 3 "
                        + "                    AND gl_transaction_rev = 0 "
                        + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                        + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                        + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                        + "  WHERE (`gl_transaction_unit_id` IS NULL) AND `gl_transaction_date` < " + DBSafeUtils.DateToSQL(start)
                        + "  AND chart_master_id in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") AND (`gl_transaction_type_id`=6) and `gl_transaction_chart_id`in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") and gl_transaction_bodycorp_id = " + bodycorp_id;
                    //bal = mydb.ExecuteScalar(sql);
                    reader = mydb.Reader(sql);

                    if (reader.Read())
                    {
                        object bal = reader[0];
                        if (bal != DBNull.Value)
                        {
                            receipt += Convert.ToDecimal(bal);
                            balance -= Convert.ToDecimal(bal);
                        }

                        object paid_bal = reader[1];
                        if (paid_bal != DBNull.Value)
                        {
                            paid_balance -= Convert.ToDecimal(paid_bal);
                        }
                    }

                    DataRow nr2 = dt.NewRow();
                    nr2["ID"] = "0";
                    nr2["InvDate"] = start.AddDays(-1).ToString("dd/MM/yyyy");
                    nr2["DueDate"] = start.AddDays(-1).ToString("dd/MM/yyyy");
                    nr2["Ref"] = "OB";
                    nr2["Description"] = "Opening Balance";
                    nr2["Invoice"] = invoice;
                    nr2["Receipt"] = receipt;
                    nr2["Balance"] = balance.ToString("0.00");
                    nr2["Allocated"] = "0.00";  // Add 20/06/2016
                    dt.Rows.Add(nr2);
                    #endregion

                    #region INCOME
                    string dateRestrict1 = "";
                    if ("ApplyDate".Equals(BaseDateKbn))
                    {
                        dateRestrict1 = " and ( (`invoice_master_apply` is not null and `invoice_master_apply` BETWEEN "
                        + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") "
                        + " or (`invoice_master_apply` is null and `invoice_master_date` BETWEEN "
                            + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + " )"
                        + " ) ";
                    }
                    else
                    {
                        dateRestrict1 = " AND  (`invoice_master_date` BETWEEN "
                        + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ")";
                    }

                    sql = "SELECT * FROM `invoice_master` WHERE (`invoice_master_unit_id`is null) "
                        + dateRestrict1
                        + " and invoice_master_bodycorp_id=" + bodycorp_id;
                    OdbcDataReader dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        nr["ID"] = dr["invoice_master_id"].ToString();
                        nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["invoice_master_date"]).ToString("dd/MM/yyyy");

                        // Update 14/05/2016
                        if ("ApplyDate".Equals(BaseDateKbn))
                        {
                            if (dr["invoice_master_apply"].ToString().Equals(""))
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(nr["InvDate"]).ToString("dd/MM/yyyy");
                            else
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["invoice_master_apply"]).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            if (dr["invoice_master_due"].ToString().Equals(""))
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(nr["InvDate"]).ToString("dd/MM/yyyy");
                            else
                                nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["invoice_master_due"]).ToString("dd/MM/yyyy");
                        }

                        nr["Ref"] = dr["invoice_master_num"].ToString();
                        nr["Description"] = dr["invoice_master_description"].ToString();
                        nr["Invoice"] = Convert.ToDecimal(dr["invoice_master_gross"]).ToString("0.00");
                        nr["Receipt"] = "";
                        nr["Balance"] = balance + decimal.Parse(dr["invoice_master_gross"].ToString());
                        balance = balance + decimal.Parse(dr["invoice_master_gross"].ToString());
                        nr["Type"] = "Invoice";
                        nr["UnitID"] = dr["invoice_master_unit_id"].ToString();     //Add 07/06/2016
                        nr["DebtorID"] = dr["invoice_master_debtor_id"].ToString(); //Add 07/06/2016
                        nr["Allocated"] = Convert.ToDecimal(dr["invoice_master_paid"]).ToString("0.00");  // Add 20/06/2016
                        dt.Rows.Add(nr);
                    }
                    #endregion

                    #region DEPOSIT
                    sql = "SELECT * FROM `receipts` WHERE (`receipt_unit_id`is null) AND (`receipt_date` BETWEEN "
                        + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") and receipt_bodycorp_id=" + bodycorp_id;
                    dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        nr["ID"] = dr["receipt_id"].ToString();
                        nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["receipt_date"]).ToString("dd/MM/yyyy");
                        nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["receipt_date"]).ToString("dd/MM/yyyy");
                        nr["Ref"] = dr["receipt_ref"].ToString();
                        //nr["Description"] = ReportDT.GetDataByColumn(recrpitPayTypeDT, "payment_type_id", dr["receipt_payment_type_id"].ToString(), "payment_type_name");
                        nr["Description"] = dr["receipt_notes"].ToString();
                        nr["Invoice"] = "";
                        nr["Receipt"] = Convert.ToDecimal(dr["receipt_gross"]).ToString("0.00");
                        nr["Balance"] = balance - decimal.Parse(dr["receipt_gross"].ToString());
                        balance = balance - decimal.Parse(dr["receipt_gross"].ToString());
                        nr["Type"] = "Receipt";
                        nr["UnitID"] = dr["receipt_unit_id"].ToString();     //Add 07/06/2016
                        nr["DebtorID"] = dr["receipt_debtor_id"].ToString(); //Add 07/06/2016
                        nr["Allocated"] = (-Convert.ToDecimal(dr["receipt_allocated"])).ToString("0.00");  // Add 20/06/2016
                        dt.Rows.Add(nr);
                    }
                    #endregion

                    #region JOURNAL
                    //s.LoadData("GENERALDEBTOR");
                    //string c = s.system_value;
                    //ChartMaster ch = new ChartMaster(constr);
                    //ch.LoadData(c);

                    // Update 20/06/2016
                    //sql = "SELECT * FROM `gl_transactions` WHERE `gl_transaction_bodycorp_id`=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value + " and `gl_transaction_chart_id`in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") and  (`gl_transaction_type_id`=6 or `gl_transaction_type_id`=7) and (`gl_transaction_unit_id`is null) AND (`gl_transaction_date`BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") AND gl_transaction_description not like '%RCPT%'";
                    sql = "SELECT * "
                        + " FROM gl_transactions "
                        + "      LEFT JOIN chart_master ON gl_transaction_chart_id = chart_master_id "
                        + "      LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                        + "                  FROM gl_transactions, gl_tran_gls "
                        + "                  WHERE gl_transaction_bodycorp_id = " + bodycorp_id
                        + "                    AND gl_transaction_type_id = 3 "
                        + "                    AND gl_transaction_ref_type_id = 3 "
                        + "                    AND gl_transaction_rev = 0 "
                        + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                        + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                        + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                        + " WHERE gl_transaction_unit_id IS NULL "
                        + "   AND gl_transaction_bodycorp_id = " + bodycorp_id
                        + "   AND chart_master_id in (" + AdFunction.StringArray_String(debtor_chart_ids) + ") "
                        + "   AND gl_transaction_type_id IN (6, 7) "
                        + "   AND gl_transaction_date BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end)
                        + "   AND gl_transaction_description NOT LIKE '%RCPT%' ";

                    dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        nr["ID"] = dr["gl_transaction_id"].ToString();
                        nr["InvDate"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]).ToString("dd/MM/yyyy");
                        nr["DueDate"] = DBSafeUtils.DBDateToDate(dr["gl_transaction_date"]).ToString("dd/MM/yyyy");
                        nr["Ref"] = dr["gl_transaction_ref"].ToString();
                        nr["Description"] = dr["gl_transaction_description"].ToString();
                        // Update 06/04/2016
                        if (Convert.ToDecimal(dr["gl_transaction_net"]) >= 0)
                        {
                            nr["Invoice"] = "";
                            nr["Receipt"] = (Convert.ToDecimal(dr["gl_transaction_net"])).ToString("0.00");
                        }
                        else
                        {
                            nr["Invoice"] = (-Convert.ToDecimal(dr["gl_transaction_net"])).ToString("0.00");
                            nr["Receipt"] = "";
                        }
                        nr["Balance"] = balance - decimal.Parse(dr["gl_transaction_net"].ToString());
                        balance = balance - decimal.Parse(dr["gl_transaction_net"].ToString());
                        nr["Type"] = "Journal";
                        if ("".Equals(dr["journal_allocated"].ToString()))
                        {
                            nr["Allocated"] = "0.00";
                        }
                        else
                        {
                            nr["Allocated"] = (-Convert.ToDecimal(dr["journal_allocated"])).ToString("0.00");  // Add 20/06/2016
                        }

                        dt.Rows.Add(nr);
                    }
                    dt.DefaultView.Sort = "InvDate";
                    dt = dt.DefaultView.ToTable();
                    decimal b = 0;
                    foreach (DataRow dr2 in dt.Rows)
                    {
                        decimal i = 0;
                        decimal r = 0;
                        decimal.TryParse(dr2["Invoice"].ToString(), out i);
                        decimal.TryParse(dr2["Receipt"].ToString(), out r);
                        b += i - r;
                        dr2["Balance"] = b;

                    }


                    #endregion
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (mydb != null && o == null) mydb.Close();
            }
        }




        public static DataTable GetDonut(string bodycorp_id, DateTime start, DateTime end, string ChartMasterId = "All", string BaseDateKbn = "DueDate", int unit_master_id = 0, Odbc o = null)
        {
            Odbc mydb = o;
            try
            {
                if (mydb == null)
                {
                    mydb = new Odbc(AdFunction.conn_sms);
                }
                SMS.system s = new SMS.system(AdFunction.conn_sms);
                s.SetOdbc(mydb);
                s.LoadData("GST Input");
                SMS.chart_master cm = new SMS.chart_master();
                cm.SetOdbc(mydb);
                cm.LoadData(s.system_value);
                string InputGstID = cm.chart_master_id.ToString();
                s.LoadData("GST Output");
                cm.LoadData(s.system_value);
                string OutputGstID = cm.chart_master_id.ToString();
                s.LoadData("GENERALTAX");
                cm.LoadData(s.system_value);
                string gstid = cm.chart_master_id.ToString();
                s.LoadData("GENERALDEBTOR");
                string[] debtor_chart_ids;
                if (ChartMasterId.Equals("All"))
                {
                    string[] gcodes = s.system_value.Split('|');
                    debtor_chart_ids = new string[gcodes.Length];
                    for (int i = 0; i < gcodes.Length; i++)
                    {
                        string temp = gcodes[i];
                        cm.LoadData(temp);
                        debtor_chart_ids[i] = cm.chart_master_id.ToString();
                    }
                }
                else
                {
                    debtor_chart_ids = new string[1];
                    debtor_chart_ids[0] = ChartMasterId;
                }


                string sql_incomeAccount = @"
select 
sum(gl_transaction_net) as cc, chart_master_id, chart_master_name
 from gl_transactions 
join chart_master on chart_master_id = gl_transaction_chart_id
where 
gl_transaction_unit_id = " + unit_master_id + @"
and gl_transaction_type_id <>3 and gl_transaction_type_id <>4
and gl_transaction_net > 0
and gl_transaction_date >= "+ DBSafeUtils.DateToSQL(start) + @" and gl_transaction_date < " + DBSafeUtils.DateToSQL(end) + @"
 AND chart_master_id not in ("
+ InputGstID + ","
+ OutputGstID + ","
+ gstid + ","
+ AdFunction.StringArray_String(debtor_chart_ids) + @" ) 
group by gl_transaction_chart_id
";
                return mydb.ReturnTable(sql_incomeAccount, "income");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (mydb != null && o == null) mydb.Close();
            }
        }

        public static string GetCleanCode(string anyCode)
        {
            return global::System.Text.RegularExpressions.Regex.Replace(anyCode, @"[^a-zA-Z0-9]", "");
        }


        public static DataTable GetUnitBalanceAndAllocation(string unit_id, DateTime report_start_date, DateTime start, DateTime end, string BaseDateKbn = "DueDate")
        {
            SMS.system s = new SMS.system(AdFunction.conn_sms);
            s.LoadData("GENERALDEBTOR");
            string[] debtor_chart_ids;
            string[] gcodes = s.system_value.Split('|');
            debtor_chart_ids = new string[gcodes.Length];

            for (int i = 0; i < gcodes.Length; i++)
            {
                string temp = gcodes[i];
                SMS.chart_master cm = new SMS.chart_master();
                cm.LoadData(temp);
                debtor_chart_ids[i] = cm.chart_master_code;
            }

            decimal total = 0;
            decimal Allocated = 0;
            Odbc o = new Odbc(AdFunction.conn_sms);
            string sql = "";
            OdbcDataReader dr1 = null;

            #region opening balance
            if (start < report_start_date)
            {
                sql = "SELECT SUM(`invoice_master_gross`) as total,sum(invoice_master_paid) as Allocated FROM `invoice_master` WHERE "
                    + " (`invoice_master_date`<" + DBSafeUtils.DateToSQL(report_start_date) + ") and `invoice_master_unit_id`=" + unit_id;
                dr1 = o.Reader(sql);
                if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
                {
                    total += decimal.Parse(dr1["total"].ToString());
                    Allocated += decimal.Parse(dr1["Allocated"].ToString());
                }

                sql = "SELECT SUM(`receipt_gross`) as total, sum(receipt_allocated) as Allocated FROM `receipts` WHERE (`receipt_date`<" + DBSafeUtils.DateToSQL(report_start_date)
                    + ") and `receipt_unit_id`=" + unit_id;
                dr1 = o.Reader(sql);
                if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
                {
                    total -= decimal.Parse(dr1["total"].ToString());
                    Allocated -= decimal.Parse(dr1["Allocated"].ToString());
                }

                sql = "SELECT SUM(`gl_transaction_net`) as total,journal_allocated "
                    + " FROM `gl_transactions`  "
                    + "       LEFT JOIN `chart_master` ON `gl_transaction_chart_id`=`chart_master_id`  "
                    + "       LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                    + "                  FROM gl_transactions, gl_tran_gls "
                    + "                  WHERE gl_transaction_unit_id = " + unit_id
                    + "                    AND gl_transaction_type_id = 3 "
                    + "                    AND gl_transaction_ref_type_id = 3 "
                    + "                    AND gl_transaction_rev = 0 "
                    + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                    + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                    + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                    + "  WHERE `gl_transaction_date` < " + DBSafeUtils.DateToSQL(report_start_date)
                    + "  AND (`chart_master_type_id`=3) AND (`gl_transaction_type_id`=6) and `gl_transaction_unit_id`=" + unit_id;
                dr1 = o.Reader(sql);
                if (dr1.Read())
                {
                    if (!String.IsNullOrEmpty(dr1["total"].ToString()))
                    {
                        total -= decimal.Parse(dr1["total"].ToString());
                    }
                    if (!string.IsNullOrEmpty(dr1["journal_allocated"].ToString()))
                    {
                        Allocated -= decimal.Parse(dr1["journal_allocated"].ToString());
                    }
                }
            }

            #endregion

            #region income
            sql = "SELECT sum(invoice_master_gross) as total, sum(invoice_master_paid) as Allocated FROM `invoice_master` WHERE "
                + " invoice_master_unit_id=" + unit_id
                + " and ";
            if ("ApplyDate".Equals(BaseDateKbn))
            {
                sql += "( (`invoice_master_apply` is not null "
                    + (start > DateTime.Parse("1950-01-01") ? "and `invoice_master_apply` > " + DBSafeUtils.DateToSQL(start) : "")
                    + " AND `invoice_master_apply` <= " + DBSafeUtils.DateToSQL(end) + ") "
                    + " or (`invoice_master_apply` is null "
                    + (start > DateTime.Parse("1950-01-01") ? "and `invoice_master_date` > " + DBSafeUtils.DateToSQL(start) : "")
                    + " AND `invoice_master_date` <= " + DBSafeUtils.DateToSQL(end) + " )"
                    + " ) ";
            }
            else
            {
                sql += "( (`invoice_master_due` is not null "
                    + (start > DateTime.Parse("1950-01-01") ? "and `invoice_master_due` > " + DBSafeUtils.DateToSQL(start) : "")
                    + " AND `invoice_master_due` <= " + DBSafeUtils.DateToSQL(end) + ") "
                    + " or (`invoice_master_due` is null "
                    + (start > DateTime.Parse("1950-01-01") ? "and `invoice_master_date` > " + DBSafeUtils.DateToSQL(start) : "")
                    + " AND `invoice_master_date` <= " + DBSafeUtils.DateToSQL(end) + " )"
                    + " ) ";
            }

            dr1 = o.Reader(sql);
            if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
            {
                total += decimal.Parse(dr1["total"].ToString());
                Allocated += decimal.Parse(dr1["Allocated"].ToString());
            }
            #endregion
            #region deposit
            sql = "SELECT sum(receipt_gross) as total, sum(receipt_allocated) as Allocated FROM `receipts` WHERE  "
                    + " `receipt_date` <=" + DBSafeUtils.DateToSQL(end) + " and receipt_unit_id =" + unit_id 
                    + (start > DateTime.Parse("1950-01-01") ? " and `receipt_date` > " + DBSafeUtils.DateToSQL(start) : "");
            dr1 = o.Reader(sql);
            if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
            {
                total -= decimal.Parse(dr1["total"].ToString());
                Allocated -= decimal.Parse(dr1["Allocated"].ToString());
            }
            #endregion
            #region journal
            sql = "SELECT sum(gl_transaction_net) as total, journal_allocated "
                    + " FROM gl_transactions "
                    + "      LEFT JOIN chart_master ON gl_transaction_chart_id = chart_master_id "
                    + "      LEFT JOIN (SELECT gl_tran_gl_parent_id, SUM(gl_transaction_net) AS journal_allocated "
                    + "                  FROM gl_transactions, gl_tran_gls "
                    + "                  WHERE gl_transaction_unit_id = " + unit_id
                    + "                    AND gl_transaction_type_id = 3 "
                    + "                    AND gl_transaction_ref_type_id = 3 "
                    + "                    AND gl_transaction_rev = 0 "
                    + "                    AND gl_transaction_id = gl_tran_gl_offset_id "
                    + "                  GROUP BY gl_tran_gl_parent_id) AS T1 "
                    + "                ON T1.gl_tran_gl_parent_id = gl_transaction_id "
                    + " WHERE gl_transaction_unit_id = " + unit_id 
                    + " AND gl_transaction_chart_id in (" + AdFunction.StringArray_String(debtor_chart_ids) + ")  AND gl_transaction_type_id IN (6, 7, 8) "
                    + (start > DateTime.Parse("1950-01-01") ? " and `gl_transaction_date` > " + DBSafeUtils.DateToSQL(start) : "")
                    + " AND gl_transaction_date <=" + DBSafeUtils.DateToSQL(end);
            dr1 = o.Reader(sql);
            if (dr1.Read())
            {
                if (!String.IsNullOrEmpty(dr1["total"].ToString()))
                {
                    total -= decimal.Parse(dr1["total"].ToString());
                }
                if (!string.IsNullOrEmpty(dr1["journal_allocated"].ToString()))
                {
                    Allocated -= decimal.Parse(dr1["journal_allocated"].ToString());
                }
            }
            #endregion
            DataTable dt = new DataTable();
            dt.Columns.Add("Balance");
            dt.Columns.Add("Allocated");
            DataRow nr = dt.NewRow();
            nr["Balance"] = total;
            nr["Allocated"] = Allocated;
            dt.Rows.Add(nr);
            return dt;
        }

        public static decimal getBMPFIcomOExpenses(int bodycorp_id, DateTime start, DateTime end, string chartMasterId, int close_off_gl_type_id)
        {
            decimal result = 0;
            Odbc o = new Odbc(AdFunction.conn_sms);
            #region bcs
            /*
            ChartMaster chart = new ChartMaster(constr);
            chart.SetOdbc(o);
            Sapp.SMS.System system = new System(constr);
            system.LoadData("GST Input");
            chart.LoadData(system.SystemValue);
            int GENERALTAX = chart.ChartMasterId;
            string sql_incomes = " SELECT sum(GT8.gl_transaction_net) as income"
                               + " FROM gl_transactions AS GT8  "
                               + "      LEFT JOIN chart_master AS CM2 ON GT8.gl_transaction_chart_id = CM2.chart_master_id  "
                               + "      LEFT JOIN budget_master AS BM1 ON GT8.gl_transaction_chart_id = BM1.budget_master_chart_id AND GT8.gl_transaction_bodycorp_id = BM1.budget_master_bodycorp_id "
                               + " WHERE GT8.gl_transaction_type_id = 6 "
                               + "   AND GT8.gl_transaction_bodycorp_id = " + bodycorp_id
                               + "   AND GT8.gl_transaction_id IN  "
                               + "           (  "
                               + "             SELECT GT7.gl_transaction_id_leg "
                               + "             FROM  "
                               + "               ( SELECT GT5.gl_transaction_id AS gl_transaction_id_leg, GT6.gl_transaction_id, GT5.gl_transaction_chart_id, GT6.legs  "
                               + "                 FROM (   SELECT GT4.gl_transaction_id, GT4.gl_transaction_ref, GT3.legs "
                               + "                          FROM ( SELECT GT1.gl_transaction_id, GT1.gl_transaction_ref "
                               + "                                 FROM gl_transactions AS GT1  "
                               + "                                      LEFT JOIN invoice_gls ON GT1.gl_transaction_id=invoice_gl_gl_id "
                               + "                                      LEFT JOIN invoice_master ON invoice_gl_invoice_id=invoice_master_id "
                               + "                                 WHERE GT1.gl_transaction_chart_id = " + chartMasterId
                               + "                                   AND GT1.gl_transaction_type_id = 6 "
                               + "                                   AND GT1.gl_transaction_bodycorp_id = " + bodycorp_id
                               + "                                   AND ( ( invoice_master_apply IS NULL  "
                               + "                                           AND GT1.gl_transaction_date>='" + start.ToString("yyyy-MM-dd") + "' AND GT1.gl_transaction_date<='" + end.ToString("yyyy-MM-dd") + "')  "
                               + "                                         OR ( invoice_master_apply IS NOT NULL  "
                               + "                                              AND invoice_master_apply>='" + start.ToString("yyyy-MM-dd") + "' AND invoice_master_apply<='" + end.ToString("yyyy-MM-dd") + "'))  "
                               + "                               ) AS GT4 "
                               + "                               LEFT JOIN  "
                               + "                               ( SELECT GT2.gl_transaction_ref, COUNT(GT2.gl_transaction_ref) AS legs  "
                               + "                                 FROM gl_transactions AS GT2 "
                               + "                                 WHERE GT2.gl_transaction_type_id = 6 "
                               + "                                   AND GT2.gl_transaction_bodycorp_id = " + bodycorp_id
                               + "                                 GROUP BY GT2.gl_transaction_ref  "
                               + "                               ) AS GT3 ON GT4.gl_transaction_ref = GT3.gl_transaction_ref "
                               + "                      ) AS GT6,  "
                               + "                      gl_transactions AS GT5 "
                               + "                 WHERE GT5.gl_transaction_ref = GT6.gl_transaction_ref "
                               + "                   AND GT5.gl_transaction_id <> GT6.gl_transaction_id "
                               + "               ) AS GT7  "
                               + "               LEFT JOIN chart_master AS CM1 ON GT7.gl_transaction_chart_id = CM1.chart_master_id "
                               + "             WHERE GT7.legs = 2 "
                               + "               AND CM1.chart_master_bank_account = 1 "
                               + "             GROUP BY GT7.gl_transaction_id "
                               + "           ) ";

            string sql_expenses = " SELECT sum(GT13.gl_transaction_net) AS expense "
                                + " FROM ( "
                                + "   SELECT GT12.gl_transaction_id, GT12.gl_transaction_chart_id, GT12.gl_transaction_description, CM3.chart_master_name, IFNULL(GT12.gl_transaction_net, 0) AS gl_transaction_net, IFNULL(BM2.budget_master_amound, 0) AS budget_master_amound "
                                + "   FROM ( "
                                + "        SELECT GT11.gl_transaction_id AS gl_transaction_id_leg, GT11.gl_transaction_chart_id, GT11.gl_transaction_description, GT10.gl_transaction_id, GT10.gl_transaction_chart_id AS gl_transaction_chart_id_old, GT10.gl_transaction_net "
                                + "        FROM "
                                + "          (  "
                                + "               SELECT GT9.gl_transaction_id, GT9.gl_transaction_chart_id, GT9.gl_transaction_description, GT9.gl_transaction_net, CG1.cinvoice_gl_cinvoice_id  "
                                + "               FROM gl_transactions AS GT9  "
                                + "                    LEFT JOIN invoice_gls ON GT9.gl_transaction_id=invoice_gl_gl_id "
                                + "                    LEFT JOIN invoice_master ON invoice_gl_invoice_id=invoice_master_id "
                                + "                    LEFT JOIN cinvoice_gls AS CG1 ON GT9.gl_transaction_id=CG1.cinvoice_gl_gl_id "
                                + "               WHERE GT9.gl_transaction_chart_id = " + chartMasterId
                                + "                 AND GT9.gl_transaction_type_id = 2 "
                                + "                 AND GT9.gl_transaction_bodycorp_id = " + bodycorp_id
                                + "                 AND ( ( invoice_master_apply IS NULL  "
                                + "                         AND GT9.gl_transaction_date>='" + start.ToString("yyyy-MM-dd") + "' AND GT9.gl_transaction_date<='" + end.ToString("yyyy-MM-dd") + "')  "
                                + "                       OR ( invoice_master_apply IS NOT NULL  "
                                + "                            AND invoice_master_apply>='" + start.ToString("yyyy-MM-dd") + "' AND invoice_master_apply<='" + end.ToString("yyyy-MM-dd") + "')) "
                                + "           ) AS GT10, "
                                + "           cinvoice_gls AS CG2, "
                                + "           gl_transactions AS GT11 "
                                + "        WHERE GT11.gl_transaction_bodycorp_id = " + bodycorp_id
                                + "          AND CG2.cinvoice_gl_cinvoice_id = GT10.cinvoice_gl_cinvoice_id "
                                + "          AND CG2.cinvoice_gl_gl_id = GT11.gl_transaction_id "
                                + "          AND GT11.gl_transaction_type_id <> 3 AND  GT11.gl_transaction_type_id <> 4  AND `gl_transaction_type_id`<>" + close_off_gl_type_id
                                + "          AND GT11.gl_transaction_chart_id <> " + GENERALTAX
                                + "          AND GT11.gl_transaction_id <> GT10.gl_transaction_id "
                                + "        GROUP BY gl_transaction_id "
                                + "      ) AS GT12 "
                                + "      LEFT JOIN chart_master AS CM3 ON GT12.gl_transaction_chart_id = CM3.chart_master_id  "
                                + "      LEFT JOIN budget_master AS BM2 ON ( GT12.gl_transaction_chart_id = BM2.budget_master_chart_id AND BM2.budget_master_bodycorp_id = 4 ) "
                                + "   UNION  "
                                + "   SELECT GT8.gl_transaction_id, GT8.gl_transaction_chart_id, GT8.gl_transaction_description, CM2.chart_master_name, IFNULL(GT8.gl_transaction_net, 0) AS gl_transaction_net, IFNULL(BM1.budget_master_amound, 0) AS budget_master_amound "
                                + "   FROM gl_transactions AS GT8  "
                                + "        LEFT JOIN chart_master AS CM2 ON GT8.gl_transaction_chart_id = CM2.chart_master_id  "
                                + "        LEFT JOIN budget_master AS BM1 ON ( GT8.gl_transaction_chart_id = BM1.budget_master_chart_id AND GT8.gl_transaction_bodycorp_id = BM1.budget_master_bodycorp_id ) "
                                + "   WHERE GT8.gl_transaction_bodycorp_id = " + bodycorp_id
                                + "     AND GT8.gl_transaction_type_id IN (2, 6) "
                                + "     AND GT8.gl_transaction_id IN  "
                                + "             (  "
                                + "               SELECT GT7.gl_transaction_id "
                                + "               FROM  "
                                + "                 ( SELECT GT5.gl_transaction_id AS gl_transaction_id_leg, GT6.gl_transaction_id, GT5.gl_transaction_ref, GT5.gl_transaction_chart_id, GT6.legs  "
                                + "                   FROM (   SELECT GT4.gl_transaction_id, GT4.gl_transaction_ref, GT3.legs "
                                + "                            FROM ( SELECT GT1.gl_transaction_id, GT1.gl_transaction_ref "
                                + "                                   FROM gl_transactions AS GT1  "
                                + "                                        LEFT JOIN invoice_gls ON GT1.gl_transaction_id=invoice_gl_gl_id "
                                + "                                        LEFT JOIN invoice_master ON invoice_gl_invoice_id=invoice_master_id "
                                + "                                   WHERE GT1.gl_transaction_chart_id = " + chartMasterId
                                + "                                     AND GT1.gl_transaction_type_id = 6 "
                                + "                                     AND GT1.gl_transaction_bodycorp_id = " + bodycorp_id
                                + "                                     AND ( ( invoice_master_apply IS NULL  "
                                + "                                             AND GT1.gl_transaction_date>='" + start.ToString("yyyy-MM-dd") + "' AND GT1.gl_transaction_date<='" + end.ToString("yyyy-MM-dd") + "')  "
                                + "                                           OR ( invoice_master_apply IS NOT NULL  "
                                + "                                                AND invoice_master_apply>='" + start.ToString("yyyy-MM-dd") + "' AND invoice_master_apply<='" + end.ToString("yyyy-MM-dd") + "'))  "
                                + "                                 ) AS GT4 "
                                + "                                 LEFT JOIN  "
                                + "                                 ( SELECT GT2.gl_transaction_ref, COUNT(GT2.gl_transaction_ref) AS legs  "
                                + "                                   FROM gl_transactions AS GT2 "
                                + "                                   WHERE GT2.gl_transaction_type_id = 6 "
                                + "                                     AND GT2.gl_transaction_bodycorp_id = " + bodycorp_id
                                + "                                   GROUP BY GT2.gl_transaction_ref  "
                                + "                                 ) AS GT3 ON GT4.gl_transaction_ref = GT3.gl_transaction_ref "
                                + "                        ) AS GT6, "
                                + "                        gl_transactions AS GT5 "
                                + "                   WHERE GT5.gl_transaction_ref = GT6.gl_transaction_ref "
                                + "                     AND GT5.gl_transaction_id <> GT6.gl_transaction_id "
                                + "                 ) AS GT7  "
                                + "                 LEFT JOIN chart_master AS CM1 ON GT7.gl_transaction_chart_id = CM1.chart_master_id "
                                + "               WHERE GT7.legs = 2 "
                                + "                 AND CM1.chart_master_bank_account <> 1 "
                                + "               GROUP BY GT7.gl_transaction_id "
                                + "             ) "
                                + "   ) AS GT13 ";
            OdbcDataReader dr1 = o.Reader(sql_incomes);
            if (dr1.Read())
            {
                if (!string.IsNullOrEmpty(dr1["income"].ToString()))
                {
                    result -= decimal.Parse(dr1["income"].ToString());
                }
            }
            dr1 = o.Reader(sql_expenses);
            if (dr1.Read())
            {
                if (!string.IsNullOrEmpty(dr1["expense"].ToString()))
                {
                    result += decimal.Parse(dr1["expense"].ToString());
                }
            }
            */
            #endregion

            #region NOT bcs
            string gl_transation_sql = "SELECT SUM(gl_transaction_net) as total"
                + " FROM ((((`gl_transactions` LEFT JOIN `invoice_gls` ON gl_transactions.`gl_transaction_id`=invoice_gls.invoice_gl_gl_id)"
                + " LEFT JOIN `invoice_master` ON invoice_gls.invoice_gl_invoice_id=invoice_master.`invoice_master_id`)"
                + " LEFT JOIN `cinvoice_gls` ON cinvoice_gls.cinvoice_gl_gl_id=gl_transactions.`gl_transaction_id`)"
                + " LEFT JOIN `cinvoices` ON cinvoice_gls.cinvoice_gl_cinvoice_id=cinvoices.`cinvoice_id`)"
                + " join chart_master on chart_master_id = gl_transaction_chart_id"
                + " join chart_types on chart_type_id = chart_master_type_id"
                + " WHERE `gl_transaction_type_id`<>3 AND `gl_transaction_type_id`<>4 AND `gl_transaction_type_id`<>" + close_off_gl_type_id
                + " AND ((`invoice_master_apply` IS NULL AND `cinvoice_apply` IS NULL AND `gl_transaction_date`>='"
                + start.ToString("yyyy-MM-dd") + "' AND `gl_transaction_date`<='" + end.ToString("yyyy-MM-dd") + "') OR (`invoice_master_apply` IS NOT NULL AND `invoice_master_apply`>='"
                + start.ToString("yyyy-MM-dd") + "' AND `invoice_master_apply`<='" + end.ToString("yyyy-MM-dd") + "') OR (`cinvoice_apply` IS NOT NULL AND `cinvoice_apply`>='"
                + start.ToString("yyyy-MM-dd") + "' AND `cinvoice_apply`<='" + end.ToString("yyyy-MM-dd") + "' )) and gl_transaction_bodycorp_id=" + bodycorp_id
                + " and ( chart_type_class_id= 4 or chart_type_class_id= 5)"
                + " and chart_master_parent_id = " + chartMasterId;
            OdbcDataReader dr1 = o.Reader(gl_transation_sql);
            if (dr1.Read() && !String.IsNullOrEmpty(dr1["total"].ToString()))
            {
                result = decimal.Parse(dr1["total"].ToString());
            }
            #endregion
            return result;
        }

        public static decimal GetDueWithGrossAndPaid(decimal gross = 0, decimal paid = 0)
        {
            decimal due = Math.Abs(gross) - paid;
            return due;
        }


        #endregion

    }
}
