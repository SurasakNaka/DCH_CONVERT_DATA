using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace DCH_SendMail
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ClassLibrarySendMail.ClassLibrarySendMail classmail = new ClassLibrarySendMail.ClassLibrarySendMail();
            string Constr = System.Configuration.ConfigurationSettings.AppSettings["Constr"].ToString();
            StringBuilder result = new StringBuilder();
            int j = 0;
            int k = 0;
            try
            {
                //string sql = @"SELECT System,SMTP,Mail_From,Mail_To,sSubject,sBody,battachment,sServer,sUser,sPassword,sSql,sPath,sPath_Bak,bActive,sDataBase,sPreFix
                //                FROM POD_31.dbo.TMConfig with(nolock) Where bActive = 1 ";

                string sql = @"SELECT System,SMTP,Mail_From,Mail_To,sSubject,sBody,battachment,sServer,sUser,sPassword,sSql,sPath,sPath_Bak,bActive,sDataBase,sPreFix
                                FROM dbo.TMConfig with(nolock) Where bActive = 1 ";

                SqlDataAdapter da = new SqlDataAdapter(sql, Constr);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count != 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        // Data Source=10.15.52.23;Initial Catalog=POD_31;Persist Security Info=True;User ID=sa;Password=sasa
                        string sql_Connstr = @"Data Source= "+ dt.Rows[i]["sServer"].ToString() + ";Initial Catalog= " + dt.Rows[i]["sDataBase"].ToString() + ";Persist Security Info=True;User ID= " + dt.Rows[i]["sUser"].ToString() + ";Password= " + dt.Rows[i]["sPassword"].ToString() + "";
                        string Sql_data = dt.Rows[i]["sSql"].ToString();

                        #region Create Directory
                        bool exists = System.IO.Directory.Exists(dt.Rows[i]["sPath"].ToString());
                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(dt.Rows[i]["sPath"].ToString());
                        }

                        exists = System.IO.Directory.Exists(dt.Rows[i]["sPath_Bak"].ToString());
                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(dt.Rows[i]["sPath_Bak"].ToString());
                        }


                        #endregion
                        DataSet ds = new DataSet();
                        da = new SqlDataAdapter(Sql_data, sql_Connstr);
                        da.Fill(ds);

                        //da = new SqlDataAdapter(Sql_data, sql_Connstr);
                        //DataTable dt_da = new DataTable();
                        //da.Fill(dt_da);
                        string FileName = string.Empty;
                        if (ds.Tables.Count != 0)
                        {
                            bool bAttch = Convert.ToBoolean((dt.Rows[i]["battachment"].ToString().Trim()));
                            if (bAttch)
                            {
                                for (int M = 0; M < ds.Tables.Count; M++)
                                {
                                    result = new StringBuilder();
                                    #region Get Name Column
                                    for (j = 0; j < ds.Tables[M].Columns.Count - 1; j++)
                                    {
                                        result.Append(ds.Tables[M].Columns[j].ColumnName + "|");
                                    }
                                    result.Append(ds.Tables[M].Columns[j].ColumnName);
                                    result.AppendLine();
                                    #endregion

                                    #region Detail
                                    foreach (DataRow row in ds.Tables[M].Rows)
                                    {
                                        object[] array = row.ItemArray;

                                        for (k = 0; k < array.Length - 1; k++)
                                        {
                                            result.Append(array[k].ToString() + "|");
                                        }
                                        result.Append(array[k].ToString());
                                        result.AppendLine();
                                    }
                                    #endregion

                                    #region Create Text file
                                    FileName = dt.Rows[i]["sPreFix"].ToString() + "_" + DateTime.Now.ToString("yyyyMMddhhmmss", (new System.Globalization.CultureInfo("en-US")));
                                    StreamWriter objWriter = new StreamWriter(dt.Rows[i]["sPath"].ToString().Trim() + @"\\" + FileName + ".txt", false);
                                    objWriter.WriteLine(result.ToString());
                                    objWriter.Close();
                                    System.Threading.Thread.Sleep(1000);
                                    #endregion
                                }
                            }
                           

                            //#region Get Name Column
                            //for (j = 0; j < dt_da.Columns.Count - 1; j++)
                            //{
                            //    result.Append(dt_da.Columns[j].ColumnName + "|");
                            //}
                            //result.Append(dt_da.Columns[j].ColumnName);
                            //result.AppendLine();
                            //#endregion

                            //#region Detail
                            //foreach (DataRow row in dt_da.Rows)
                            //{
                            //    object[] array = row.ItemArray;

                            //    for (k = 0; k < array.Length - 1; k++)
                            //    {
                            //        result.Append(array[k].ToString() + "|");
                            //    }
                            //    result.Append(array[k].ToString());
                            //    result.AppendLine();
                            //}
                            //#endregion


                            classmail.Sendmail(dt.Rows[i]["Mail_To"].ToString().Trim(), dt.Rows[i]["SMTP"].ToString().Trim(), dt.Rows[i]["sBody"].ToString().Trim(), dt.Rows[i]["Mail_From"].ToString().Trim(), dt.Rows[i]["sSubject"].ToString().Trim(), bAttch, dt.Rows[i]["sPath"].ToString().Trim(), FileName + ".ZIP", false, dt.Rows[i]["sPath_Bak"].ToString().Trim(), ds);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                this.Dispose();
            }
        }
    }
}
