namespace AzureIoTPortal.SMS
{
    using Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Data.Odbc;

    public partial class chart_master
    {

        [Key]
        public int chart_master_id { get; set; }
        public string chart_master_code { get; set; }
        public string chart_master_name { get; set; }
        [Display(Name = "Type")]
        public int chart_master_type_id { get; set; }

        [Display(Name = "RechargeTo")]
        public int? chart_master_recharge_to_id { get; set; }

        [Display(Name = "Parent")]
        public int? chart_master_parent_id { get; set; }

        [Display(Name = "Account")]
        public int? chart_master_account_id { get; set; }

        [Display(Name = "No Tax")]
        public bool chart_master_notax { get; set; }

        [Display(Name = "Bank Account")]
        public bool chart_master_bank_account { get; set; }

        [Display(Name = "Trust Account")]
        public bool chart_master_trust_account { get; set; }

        [Display(Name = "Levy Base")]
        public bool chart_master_levy_base { get; set; }

        [Display(Name = "Inactive")]
        public bool chart_master_inactive { get; set; }

        public string chart_master_dsn { get; set; }





        private string constr;
        private Odbc odbc = null;
        public chart_master()
        {
        }
        public chart_master(string constr)
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

        public void LoadData(int chart_master_id)
        {
            Odbc mydb = null;
            try
            {
                this.chart_master_id = chart_master_id;
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
                string sql = "SELECT * FROM `chart_master` WHERE `chart_master_id`=" + chart_master_id;
                OdbcDataReader dr = mydb.Reader(sql);
                if (dr.Read())
                {
                    #region Initial Properties
                    this.chart_master_code = DBSafeUtils.DBStrToStr(dr["chart_master_code"]);
                    this.chart_master_name = DBSafeUtils.DBStrToStr(dr["chart_master_name"]);
                    this.chart_master_type_id = Convert.ToInt32(dr["chart_master_type_id"]);
                    this.chart_master_recharge_to_id = DBSafeUtils.DBIntToIntN(dr["chart_master_recharge_to_id"]);
                    this.chart_master_account_id = DBSafeUtils.DBIntToIntN(dr["chart_master_account_id"]);
                    this.chart_master_notax = dr["chart_master_notax"].ToString() == "0" ? false : true;
                    this.chart_master_bank_account = DBSafeUtils.DBBoolToBool(dr["chart_master_bank_account"]);
                    this.chart_master_trust_account = DBSafeUtils.DBBoolToBool(dr["chart_master_trust_account"]);
                    this.chart_master_levy_base = DBSafeUtils.DBBoolToBool(dr["chart_master_levy_base"]);
                    this.chart_master_inactive = DBSafeUtils.DBBoolToBool(dr["chart_master_inactive"]);
                    this.chart_master_parent_id = DBSafeUtils.DBIntToIntN(dr["chart_master_parent_id"]);
                    this.chart_master_dsn = DBSafeUtils.DBStrToStr(dr["chart_master_dsn"]);
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
        public void LoadData(string chart_code)
        {
            Odbc mydb = null;
            try
            {
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                }
                else
                {
                    mydb = odbc;
                }
                string sql = "SELECT * FROM `chart_master` WHERE `chart_master_code`=" + DBSafeUtils.StrToQuoteSQL(chart_code);
                OdbcDataReader dr = mydb.Reader(sql);
                if (dr.Read())
                {
                    #region Initial Properties
                    this.chart_master_id = Convert.ToInt32(dr["chart_master_id"]);
                    this.chart_master_code = DBSafeUtils.DBStrToStr(dr["chart_master_code"]);
                    this.chart_master_name = DBSafeUtils.DBStrToStr(dr["chart_master_name"]);
                    this.chart_master_type_id = Convert.ToInt32(dr["chart_master_type_id"]);
                    this.chart_master_recharge_to_id = DBSafeUtils.DBIntToIntN(dr["chart_master_recharge_to_id"]);
                    this.chart_master_account_id = DBSafeUtils.DBIntToIntN(dr["chart_master_account_id"]);
                    this.chart_master_notax = dr["chart_master_notax"].ToString() == "0" ? false : true;
                    this.chart_master_bank_account = DBSafeUtils.DBBoolToBool(dr["chart_master_bank_account"]);
                    this.chart_master_trust_account = DBSafeUtils.DBBoolToBool(dr["chart_master_trust_account"]);
                    this.chart_master_levy_base = DBSafeUtils.DBBoolToBool(dr["chart_master_levy_base"]);
                    this.chart_master_inactive = DBSafeUtils.DBBoolToBool(dr["chart_master_inactive"]);
                    this.chart_master_parent_id = DBSafeUtils.DBIntToIntN(dr["chart_master_parent_id"]);
                    this.chart_master_dsn = DBSafeUtils.DBStrToStr(dr["chart_master_dsn"]);
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





    }
}
