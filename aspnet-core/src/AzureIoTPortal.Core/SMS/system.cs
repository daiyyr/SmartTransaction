namespace AzureIoTPortal.SMS
{
    using Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Data.Odbc;

    public partial class system
    {

        [Key]
        public int system_id { get; set; }
        public string system_code { get; set; }
        public string system_value { get; set; }


        private string constr;
        private Odbc odbc = null;
        public system()
        {
        }
        public system(string constr)
        {
            try
            {
                this.constr = constr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LoadData(int system_id)
        {
            Odbc mydb = null;
            try
            {
                this.system_id = system_id;
                if (odbc == null)
                {
                    if (constr == null)
                    {
                        constr = AdFunction.conn_sms;
                    }
                    mydb = new Odbc(constr);
                }
                else
                {
                    mydb = odbc;
                }
                string sql = "SELECT * FROM `system` WHERE `system_id`=" + system_id;
                OdbcDataReader dr = mydb.Reader(sql);
                if (dr.Read())
                {
                    #region Initial Properties
                    system_code = DBSafeUtils.DBStrToStr(dr["system_code"]);
                    system_value = DBSafeUtils.DBStrToStr(dr["system_value"]);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (odbc == null)
                {
                    if (mydb != null) mydb.Close();
                }
            }
        }
        public void LoadData(string system_code)
        {
            Odbc mydb = null;
            try
            {
                if (odbc == null)
                {
                    if (constr == null)
                    {
                        constr = AdFunction.conn_sms;
                    }
                    mydb = new Odbc(constr);
                }
                else
                {
                    mydb = odbc;
                }
                string sql = "SELECT * FROM `system` WHERE `system_code`=" + DBSafeUtils.StrToQuoteSQL(system_code);
                OdbcDataReader dr = mydb.Reader(sql);
                if (dr.Read())
                {
                    #region Initial Properties
                    system_id = Convert.ToInt32(dr["system_id"]);
                    this.system_code = DBSafeUtils.DBStrToStr(dr["system_code"]);
                    system_value = DBSafeUtils.DBStrToStr(dr["system_value"]);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (odbc == null)
                {
                    if (mydb != null) mydb.Close();
                }
            }
        }
        public void SetOdbc(Odbc mydb)
        {
            try
            {
                this.odbc = mydb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public string GetNextNumber(string PREFIX_name, string PILOT_name, string DIGIT_name, string table, string column)
        {
            Odbc mydb = null;
            bool isnull = true;
            try
            {
                if (odbc == null)
                {
                    mydb = new Odbc(AdFunction.conn_sms);
                    this.odbc = mydb;
                    isnull = true;
                }
                else
                {
                    mydb = odbc;
                    isnull = false;
                }
                string INVOICEPREFIX = "";
                string INVOICEPILOT = "";
                string INVOICEDIGIT = "";
                this.LoadData(PREFIX_name);
                INVOICEPREFIX = this.system_value;
                this.LoadData(PILOT_name);
                INVOICEPILOT = this.system_value;
                this.LoadData(DIGIT_name);
                INVOICEDIGIT = this.system_value;
                int next_number = Convert.ToInt32(INVOICEPILOT) + 1;
                int digit = Convert.ToInt32(INVOICEDIGIT);
                string inv_number = next_number.ToString();
                if (inv_number.Length < digit)
                {
                    for (int i = 0; i < (digit - inv_number.Length); i++)
                    {
                        INVOICEPREFIX += "0";
                    }
                }
                inv_number = INVOICEPREFIX + inv_number;
                this.LoadData(PILOT_name);
                mydb.NonQuery("update system set system_value = '" + next_number.ToString() + "' where system_code ='" + PILOT_name + "'");
                if (CheckExist(table, column, inv_number))
                    return GetNextNumber(PREFIX_name, PILOT_name, DIGIT_name, table, column);
                else
                    return inv_number;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (isnull)
                {
                    if (mydb != null) mydb.Close();
                    odbc = null;
                }
            }
        }
        public bool CheckExist(string tablename, string column, string value)
        {
            Odbc mydb = null;
            bool isnull = true;
            try
            {
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                    this.odbc = mydb;
                    isnull = true;
                }
                else
                {
                    mydb = odbc;
                    isnull = false;
                }
                bool ret = false;
                string sql = "SELECT * FROM `" + tablename + "` WHERE `" + column + "`=" + DBSafeUtils.StrToQuoteSQL(value);
                DataTable dt = mydb.ReturnTable(sql, "temp");
                if (dt.Rows.Count > 0)
                    ret = true;
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (isnull)
                {
                    if (mydb != null) mydb.Close();
                    odbc = null;
                }
            }
        }


    }
}
