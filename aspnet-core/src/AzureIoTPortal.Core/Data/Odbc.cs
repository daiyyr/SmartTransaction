/****************************************************************************************
 * Author:          Philip
 * Date:            25/11/2011
 * Description:     DAL for odbc database.
 * Note:
 * Kowning Issues:
 * Last Modified:   25/11/2011
****************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Collections;
namespace AzureIoTPortal.Data
{
    public class Odbc : IDisposable
    {
        private OdbcConnection con;
        private bool opened;
        private string constr;
        private bool isrecorded = false;
        private int tran_count = 0;

        public Odbc(string constr)
        {
            try
            {
                this.constr = constr;
                con = new OdbcConnection(constr);
                opened = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //~Odbc()
        //{
        //    try
        //    {
        //        if (con != null)
        //            con.Close();
        //        //Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void Dispose()
        {
            try
            {
                if (con != null)
                {
                    con.Dispose();
                    con = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Close()
        {
            try
            {
                if (con != null)
                {
                    con.Close();
                    con = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public OdbcDataReader Reader(string sqlcommand)
        {
            try
            {
                OdbcCommand cmd;
                OdbcDataReader dr;
                cmd = new OdbcCommand(sqlcommand, con);
                if (!opened)
                {
                    con.Open();
                    opened = true;
                }
                dr = cmd.ExecuteReader();
                return dr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //
        public bool NonQuery(string sqlcommand)
        {
            try
            {
                OdbcCommand cmd = new OdbcCommand(sqlcommand, con);
                if (!opened)
                {
                    con.Open();
                    opened = true;
                }
                cmd.ExecuteNonQuery();
                //if (!isrecorded)
                //{
                //    Log log = new Log(constr);
                //    log.Add(DBSafeUtils.DBStrToStr(sqlcommand));
                //    isrecorded = true;
                //}
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ExecuteScalar(string sqlcommand)
        {
            try
            {
                OdbcCommand cmd = new OdbcCommand(sqlcommand, con);
                if (!opened)
                {
                    con.Open();
                    opened = true;
                }
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet FillDataSet(string sqlcommand, string tablename)
        {
            try
            {
                OdbcDataAdapter da = new OdbcDataAdapter(sqlcommand, con);
                DataSet ds = new DataSet();
                da.Fill(ds, tablename);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ReturnTable(string sqlcommand, string tablename)
        {
            try
            {
                OdbcDataAdapter da = new OdbcDataAdapter(sqlcommand, con);
                DataSet ds = new DataSet();
                da.Fill(ds, tablename);
                DataTable dt = ds.Tables[tablename];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //
        public bool ExecuteSQL(ArrayList sqlcommands)
        {
            try
            {
                bool ret = false;
                if (!opened)
                {
                    con.Open();
                    opened = true;
                }
                OdbcTransaction trans = con.BeginTransaction();
                OdbcCommand cmd = new OdbcCommand();
                cmd.Connection = con;
                cmd.Transaction = trans;

                try
                {
                    foreach (string sqlcommand in sqlcommands)
                    {
                        cmd.CommandText = sqlcommand;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    //foreach (string sqlcommand in sqlcommands)
                    //{
                    //    Log log = new Log(constr);
                    //    log.Add(sqlcommand);
                    //}
                }
                catch
                {
                    ret = false;
                    trans.Rollback();
                    throw;
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /*
        public bool ExecuteInsert(string tablename, Hashtable items)
        {
            string sql = "";
            try
            {
                if (items.Count <= 0)
                    return true;
                string fields = "";
                string values = "";
                int count = 0;
                foreach (DictionaryEntry item in items)
                {
                    if (count != 0)
                    {
                        fields += ", ";
                        values += ", ";
                    }
                    fields += "`" + item.Key.ToString() + "`";
                    string temp = item.Value.ToString();
                    //if (temp.StartsWith("'") && temp.EndsWith("'"))
                    //{
                    //    if (temp.Length - 1 > 0)
                    //    {
                    //        temp = temp.Substring(1, temp.Length - 2);
                    //        temp = temp.Replace("'", "''");
                    //        temp = "'" + temp + "'";
                    //    }
                    //}
                    values += temp;

                    count++;
                }
                if (tablename.Equals("gl_transactions"))
                {
                    try
                    {
                        string login = System.Web.HttpContext.Current.User.Identity.Name;
                        Sapp_General.Domain.User user = new Sapp_General.Domain.User(Sapp_Sms.Data.AdFunction.constr_general);
                        user.LoadData(login);

                        fields += ",gl_transaction_createdate,gl_transaction_user_id";
                        values += ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," + user.UserId;
                    }
                    catch (Exception)
                    {
                    }
                }
                //Log log = new Log(constr);
                //log.Add(log.GenerateContent("Insert", tablename, items));
                //isrecorded = true;

                sql = "INSERT INTO `" + tablename + "` (" + fields + ") VALUES (" + values + ")";
                return NonQuery(sql);
            }
            catch (Exception ex)
            {
                throw new Exception(sql + "||" + ex.Message);
            }
        }
        //
        public bool ExecuteInsertBatch(string tablename, ArrayList list)
        {
            try
            {
                bool ret = false;
                if (!opened)
                {
                    con.Open();
                    opened = true;
                }
                OdbcTransaction trans = con.BeginTransaction();
                OdbcCommand cmd = new OdbcCommand();
                cmd.Connection = con;
                cmd.Transaction = trans;
                try
                {
                    foreach (Hashtable items in list)
                    {
                        if (items.Count <= 0)
                            throw new Exception("Error: empty record!");
                        string fields = "";
                        string values = "";
                        int count = 0;
                        foreach (DictionaryEntry item in items)
                        {
                            if (count != 0)
                            {
                                fields += ", ";
                                values += ", ";
                            }
                            fields += "`" + item.Key.ToString() + "`";
                            values += item.Value.ToString();

                            count++;
                        }
                        if (tablename.Equals("gl_transactions"))
                        {
                            string login = System.Web.HttpContext.Current.User.Identity.Name;
                            Sapp.General.User user = new Sapp.General.User(Sapp.SMS.AdFunction.constr_general);
                            user.LoadData(login);

                            fields += ",gl_transaction_createdate,gl_transaction_user_id";
                            values += "," + DateTime.Now.ToString("yyyy-MM-dd") + "," + user.UserId;
                        }

                        string sql = "INSERT INTO `" + tablename + "` (" + fields + ") VALUES (" + values + ")";
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    //Log log = new Log(constr);
                    //foreach (Hashtable items in list)
                    //{
                    //    log.Add(log.GenerateContent("Insert", tablename, items));
                    //}
                }
                catch
                {
                    ret = false;
                    trans.Rollback();
                    throw;
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        */
        public bool ExecuteUpdate(string tablename, Hashtable items, string sqlwhere)
        {
            try
            {
                if (items.Count <= 0)
                    return true;
                string fields = "";
                int count = 0;
                foreach (DictionaryEntry item in items)
                {
                    if (count != 0)
                    {
                        fields += ", ";
                    }
                    fields += "`" + item.Key.ToString() + "`";
                    fields += "=";
                    fields += item.Value.ToString();
                    count++;
                }

                string sql = "UPDATE `" + tablename + "` SET " + fields + " " + sqlwhere;
                //Log log = new Log(constr);
                //log.Add(log.GenerateContent("Update", tablename, items, sqlwhere));
                //isrecorded = true;
                return NonQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void StartTransaction()
        {
            try
            {
                if (tran_count == 0)
                {
                    string sqlcommand = "START TRANSACTION";
                    OdbcCommand cmd = new OdbcCommand(sqlcommand, con);
                    if (!opened)
                    {
                        con.Open();
                        opened = true;
                    }
                    cmd.ExecuteNonQuery();
                    tran_count++;
                }
                else
                    tran_count++;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Commit()
        {
            try
            {
                if (tran_count == 1)
                {
                    string sqlcommand = "COMMIT";
                    OdbcCommand cmd = new OdbcCommand(sqlcommand, con);
                    if (!opened)
                    {
                        con.Open();
                        opened = true;
                    }
                    cmd.ExecuteNonQuery();
                    tran_count--;
                }
                else
                    tran_count--;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Rollback()
        {
            try
            {
                if (tran_count > 0)
                {
                    string sqlcommand = "ROLLBACK";
                    OdbcCommand cmd = new OdbcCommand(sqlcommand, con);
                    if (!opened)
                    {
                        con.Open();
                        opened = true;
                    }
                    cmd.ExecuteNonQuery();
                    tran_count = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LockTables(ArrayList list)
        {
            try
            {
                string sqlcommand = "LOCK TABLES ";
                int i = 0;
                foreach (Hashtable items in list)
                {
                    if (i != 0) sqlcommand += ", ";
                    sqlcommand += "`" + items["tablename"].ToString() + "`";
                    sqlcommand += " " + items["locktype"].ToString();
                    i++;
                }
                OdbcCommand cmd = new OdbcCommand(sqlcommand, con);
                if (!opened)
                {
                    con.Open();
                    opened = true;
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UnlockTables()
        {
            try
            {
                string sqlcommand = "UNLOCK TABLES ";
                OdbcCommand cmd = new OdbcCommand(sqlcommand, con);
                if (!opened)
                {
                    con.Open();
                    opened = true;
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetForeignKeyCheckOff()
        {
            try
            {
                string sqlcommand = "SET foreign_key_checks = 0";
                OdbcCommand cmd = new OdbcCommand(sqlcommand, con);
                if (!opened)
                {
                    con.Open();
                    opened = true;
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetForeignKeyCheckOn()
        {
            try
            {
                string sqlcommand = "SET foreign_key_checks = 1";
                OdbcCommand cmd = new OdbcCommand(sqlcommand, con);
                if (!opened)
                {
                    con.Open();
                    opened = true;
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool InsertLog(string sqlcommand)
        {
            try
            {
                OdbcCommand cmd = new OdbcCommand(sqlcommand, con);
                if (!opened)
                {
                    con.Open();
                    opened = true;
                }
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /*
        public void CreateTempTable(DataTable dt)
        {
            try
            {
                string sql = "CREATE TEMPORARY TABLE `" + dt.TableName + "`" +
                        "(";

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0) sql += ",";
                    if (dt.Columns[i].DataType != System.Type.GetType("System.DateTime"))
                        sql += "`" + dt.Columns[i].ColumnName + "` varchar(255)";
                    else sql += dt.Columns[i].ColumnName + " date";

                }
                sql += ")";
                NonQuery(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    Hashtable items = new Hashtable();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (dt.Columns[i].DataType != System.Type.GetType("System.DateTime")) items.Add(dt.Columns[i].ColumnName, DBSafeUtils.StrToQuoteSQL(dr[i].ToString()));
                        else items.Add(dt.Columns[i].ColumnName, DBSafeUtils.DateToSQL(dr[i]));
                    }
                    ExecuteInsert(dt.TableName, items);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void CreateTempTable(string tablename, ArrayList dataList)
        {
            try
            {
                DataTable dt = new DataTable(tablename);
                int index = 0;
                foreach (Hashtable items in dataList)
                {
                    if (index == 0)
                    {
                        foreach (DictionaryEntry entry in items)
                        {
                            dt.Columns.Add(entry.Key.ToString());
                        }
                    }
                    DataRow dr = dt.NewRow();
                    foreach (DictionaryEntry entry in items)
                    {
                        dr[entry.Key.ToString()] = entry.Value.ToString();
                    }
                    dt.Rows.Add(dr);
                }
                this.CreateTempTable(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        */
        public ArrayList Delete(string database, string tablename, Hashtable items, string sql_delete)
        {
            try
            {
                string sql = "SELECT `table_name`, `column_name`, `referenced_column_name` FROM INFORMATION_SCHEMA.key_column_usage WHERE `referenced_table_schema` = '" + database
                    + "' AND `referenced_table_name`= '" + tablename + "'";
                OdbcDataReader dr = this.Reader(sql);
                ArrayList ret_items = new ArrayList();
                while (dr.Read())
                {
                    string dependent_tablename = dr["table_name"].ToString();
                    string dependent_colname = dr["column_name"].ToString();
                    string referenced_colname = dr["referenced_column_name"].ToString();
                    string referenced_value = items[referenced_colname].ToString();
                    Hashtable dependents = Dependents(referenced_value, dependent_colname, dependent_tablename);
                    if (dependents.Count > 0)
                        ret_items.Add(dependents);
                }
                if (ret_items.Count == 0)
                    this.NonQuery(sql_delete);
                return ret_items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Hashtable Dependents(string referenced_value, string dependent_colname, string dependent_tablename)
        {
            try
            {
                string sql = "SELECT * FROM `" + dependent_tablename + "` WHERE `" + dependent_colname + "`=" + referenced_value;
                OdbcDataReader dr = this.Reader(sql);
                Hashtable items = new Hashtable();
                ArrayList dependents = new ArrayList();
                int i = 0;
                while (dr.Read())
                {
                    if (i == 0)
                    {
                        items.Add("dependent_tablename", dependent_tablename);
                        items.Add("dependent_colname", dependent_colname);
                        items.Add("referenced_value", referenced_value);
                    }
                    Hashtable dependent_value = new Hashtable();
                    for (int j = 0; j < dr.FieldCount; j++)
                    {
                        dependent_value.Add(dr.GetName(j), dr[j]);
                    }
                    dependents.Add(dependent_value);
                    i++;
                }
                if (items.Count > 0)
                    items.Add("dependents", dependents);
                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
