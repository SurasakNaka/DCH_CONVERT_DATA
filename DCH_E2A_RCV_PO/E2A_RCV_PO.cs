using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DCH_E2A_RCV_PO
{
    public partial class E2A_RCV_PO : Form
    {
        public E2A_RCV_PO()
        {
            InitializeComponent();
        }
        #region Getconfig
        public string GetPATH_SOURCE()
        {
            string sPath = System.Configuration.ConfigurationSettings.AppSettings["PATH_SOURCE"].ToString();
            return sPath;
        }

        public string GetPATH_BAK()
        {
            string sPath = System.Configuration.ConfigurationSettings.AppSettings["PATH_BAK"].ToString();
            return sPath;
        }

        public string GetPATH_XML()
        {
            string sPath = System.Configuration.ConfigurationSettings.AppSettings["PATH_TARGET"].ToString();
            return sPath;
        }
        public string[] GetPRE_FIX(string sPath)
        {
            string[] sFiles = null;
            try
            {
                string sPrefix = System.Configuration.ConfigurationSettings.AppSettings["PRE_FIX"].ToString();
                sFiles = Directory.GetFiles(sPath, sPrefix + "*.txt");
                return sFiles;
            }
            catch (Exception)
            {
                return sFiles;
            }
        }
 
        #endregion

        private DataSet AddCoulumn()
        {
            DataSet ds_output = new DataSet();
            DataTable dt = new DataTable("ERR");
            try
            {
                dt.Columns.Add("LINE_NO", typeof(String));
                dt.Columns.Add("PO_NO", typeof(String));
                dt.Columns.Add("TAG_NAME", typeof(String));
                dt.Columns.Add("VALUES", typeof(String));
                ds_output.Tables.Add(dt);
                return ds_output;
            }
            catch (Exception ex)
            {
                return ds_output;
            }

        }

        private DataSet AddCoulumn_reusultMail()
        {
            DataSet ds_output = new DataSet();
            DataTable dt = new DataTable("Mail");
            try
            {
                dt.Columns.Add("LINE_NO", typeof(String));
                dt.Columns.Add("MESSAGE", typeof(String));
                ds_output.Tables.Add(dt);
                return ds_output;
            }
            catch (Exception ex)
            {
                return ds_output;
            }
        }

        public DataTable ConvertToDataTable(string fileName, out DataTable dt_no)
        {
            dt_no = new DataTable();
            try
            {
                DataTable dt = new DataTable("Inbound");
                #region add column
                dt.Columns.Add("COMPANY_NUMBER");
                dt.Columns.Add("PO_NO");
                dt.Columns.Add("PO_TYPE");
                dt.Columns.Add("Supplier_Name");
                dt.Columns.Add("Supplier_Address_Line1");
                dt.Columns.Add("Supplier_Address_Line2");
                dt.Columns.Add("Supplier_Address_Line3");
                dt.Columns.Add("City");
                dt.Columns.Add("State");
                dt.Columns.Add("Zip");
                dt.Columns.Add("Phone");
                dt.Columns.Add("LineNumber");
                dt.Columns.Add("itemNumber");
                dt.Columns.Add("Qty");
                dt.Columns.Add("UOM");

                #endregion

                #region Sale order
                dt_no.Columns.Add("PO_NO");
                #endregion
                var lines = File.ReadAllLines(fileName, System.Text.Encoding.UTF8).Where(s => s.Trim() != string.Empty).ToArray();
                // reading rest of the data
                for (int i = 0; i < lines.Count(); i++)
                {
                    DataRow dr = dt.NewRow();
                    string[] values = lines[i].Split(new char[] { '|' });

                    dr["COMPANY_NUMBER"] = values[0];
                    dr["PO_NO"] = values[1];
                    dr["PO_TYPE"] = values[2];
                    dr["Supplier_Name"] = values[3];
                    dr["Supplier_Address_Line1"] = values[4];
                    dr["Supplier_Address_Line2"] = values[5];
                    dr["Supplier_Address_Line3"] = values[6];
                    dr["City"] = values[7];
                    dr["State"] = values[8];
                    dr["Zip"] = values[9];
                    dr["Phone"] = values[10];
                    dr["LineNumber"] = values[11];
                    dr["itemNumber"] = values[12];
                    dr["Qty"] = values[13];
                    dr["UOM"] = values[14];

                    dt.Rows.Add(dr);

                    DataRow dr_no = dt_no.NewRow();
                    dr_no["PO_NO"] = values[1];
                    dt_no.Rows.Add(dr_no);
                }
                dt_no = dt_no.DefaultView.ToTable(true, "PO_NO"); // removeDuplicatesRows
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void E2A_RCV_PO_Load(object sender, EventArgs e)
        {
            ClassLibrarySendMail.ClassLibrarySendMail classmail = new ClassLibrarySendMail.ClassLibrarySendMail();
            string MailFrom = System.Configuration.ConfigurationSettings.AppSettings["MailFrom"].ToString();
            string MailTo = System.Configuration.ConfigurationSettings.AppSettings["MailTo"].ToString();
            string smtp = System.Configuration.ConfigurationSettings.AppSettings["SMTP"].ToString();
            string PRE_Naming = System.Configuration.ConfigurationSettings.AppSettings["PRE_Naming"].ToString();
            string strSubject = System.Configuration.ConfigurationSettings.AppSettings["sSubjectmail"].ToString();

            string FtpSync_Inbound = System.Configuration.ConfigurationSettings.AppSettings["FtpSync_Inbound"].ToString();
            string WorkingDirectory = System.Configuration.ConfigurationSettings.AppSettings["WorkingDirectory"].ToString();
            try
            {
                string sPath_Source = GetPATH_SOURCE();
                string sPath_BAK = GetPATH_BAK();
                string GetPATHXML = GetPATH_XML();
                string[] sFiles = null;
                sFiles = GetPRE_FIX(sPath_Source);
                DataTable dt = new DataTable();
                DataTable dt_no = new DataTable();
                DataTable dtResult = new DataTable();

                StringBuilder result = new StringBuilder();
                StringBuilder result_error = new StringBuilder();
                string sPrefix = System.Configuration.ConfigurationSettings.AppSettings["PRE_FIX"].ToString();
                string SUPNUM = System.Configuration.ConfigurationSettings.AppSettings["SUPNUM"].ToString();
                string CLIENT_ID = System.Configuration.ConfigurationSettings.AppSettings["CLIENT_ID"].ToString();
                string WHSE_ID = System.Configuration.ConfigurationSettings.AppSettings["WHSE_ID"].ToString();
                string SHIP_TO_JDE_ADDRESS = "";
                string SALES_ORDER_NO = "";
                string SHIP_TO = "";
                bool bCheck = false;
                bool Count_error = false;
                int nCount = 0;
                int nCount_line = 0;

                System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                pProcess.StartInfo.WorkingDirectory = WorkingDirectory;
                pProcess.StartInfo.FileName = FtpSync_Inbound;
                pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                pProcess.Start();
                pProcess.WaitForExit();

                string Filename = string.Empty;
                foreach (string filepath in Directory.GetFiles(sPath_Source, sPrefix + "*.txt"))
                {
                    Filename = string.Empty;
                    bCheck = true;
                    dt = new DataTable();
                    dt_no = new DataTable();
                    dt = ConvertToDataTable(filepath, out dt_no);
                    Count_error = false;
                    string searchstring = string.Empty;
                    nCount = 0;
                    DataSet ds_result = new DataSet();
                    DataSet ds_mail = new DataSet();
                    DataRow dr;
                    Filename = Path.GetFileName(filepath);

                    ds_mail = new DataSet();
                    ds_mail = AddCoulumn_reusultMail();

                    for (int i = 0; i < dt_no.Rows.Count; i++) // Loop PO
                    {
                        nCount = nCount + 1;
                        if (searchstring != dt_no.Rows[i]["PO_NO"].ToString().Trim())
                        {
                            ds_result = AddCoulumn();
                        }

                        searchstring = dt_no.Rows[i]["PO_NO"].ToString();

                        dtResult = new DataTable();
                        dtResult = dt.Select("PO_NO LIKE '%" + searchstring + "%'").CopyToDataTable();

                        result = new StringBuilder();
                        result.Append(@"<?xml version='1.0' encoding= 'UTF-8'?>");

                        #region RA_INB_IFD  
                        result.Append(@"<RA_INB_IFD xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>");

                        #region CTRL_SEG
                        result.Append("<CTRL_SEG>");

                        result.Append("<TRNNAM>RA_TRAN</TRNNAM>");
                        //result.AppendLine();
                        result.Append("<TRNVER>8.3</TRNVER>");
                        //result.AppendLine();
                        result.Append(@"<TRNDTE>" + DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss") + "</TRNDTE>");
                        //result.AppendLine();
                        result.Append("<WHSE_ID>" + WHSE_ID + "</WHSE_ID>");

                        #region HEADER_SEG
                        result.Append("<HEADER_SEG>");

                        result.Append("<SEGNAM>HEADER_SEG</SEGNAM>");
                        result.Append("<TRNTYP>A</TRNTYP>");

                        string INVNUM = CLIENT_ID.Substring(0,5) + "PAHC-" + "PO" + dtResult.Rows[0]["PO_NO"].ToString().Trim();
                        if (dtResult.Rows[0]["PO_NO"].ToString().Trim() == string.Empty)
                        {
                            Count_error = true;
                            result.Append("<INVNUM/>"); //result_error

                            dr = ds_result.Tables["ERR"].NewRow();
                            dr["LINE_NO"] = nCount;
                            dr["PO_NO"] = searchstring;
                            dr["TAG_NAME"] = "HEADER_SEG-INVNUM";
                            dr["VALUES"] = "Blank";
                            ds_result.Tables["ERR"].Rows.Add(dr);
                        }
                        else
                        {
                            result.Append("<INVNUM>" + INVNUM + "</INVNUM>");
                        }

                        result.Append("<SUPNUM>" + SUPNUM + "</SUPNUM>"); // Fix ค่อยมาแก่ที่หลัง WAITING_DATA Supplier number. FIX "90000008" ไปก่อนค่อยมาแก้ทีหลัง
                        result.Append("<CLIENT_ID>" + CLIENT_ID + "</CLIENT_ID>"); // Fix ค่อยมาแก่ที่หลัง WAITING_DATA

                        result.Append("<RIMSTS>OPEN</RIMSTS>");
                        result.Append("<INVTYP>P</INVTYP>");
                        result.Append("<INVDTE>" + DateTime.Now.ToString("yyyy-MM-dd Thh:mm:ss") + "</INVDTE>");
                        result.Append("<ORGREF/>");
                        result.Append("<SADNUM/>");
                        result.Append("<TPL_SHIPTONO/>");

                        #region <LINE_SEG>
                        nCount_line = 0;
                        for (int j = 0; j < dtResult.Rows.Count; j++)
                        {
                            nCount_line = nCount_line + 1;
                            result.Append("<LINE_SEG>");
                            #region  LINE_SEG Data
                            result.Append("<SEGNAM>LINE_SEG</SEGNAM>");
                            if (dtResult.Rows[j]["LineNumber"].ToString().Trim() == string.Empty)
                            {
                                Count_error = true;
                                result.Append("<INVLIN/>");
                                result.Append("<INVSLN/>");

                                dr = ds_result.Tables["ERR"].NewRow();
                                dr["LINE_NO"] = nCount_line;
                                dr["PO_NO"] = searchstring;
                                dr["TAG_NAME"] = "LINE_SEG-INVLIN";
                                dr["VALUES"] = "Blank";
                                ds_result.Tables["ERR"].Rows.Add(dr);

                                dr = ds_result.Tables["ERR"].NewRow();
                                dr["LINE_NO"] = nCount_line;
                                dr["PO_NO"] = searchstring;
                                dr["TAG_NAME"] = "LINE_SEG-INVSLN";
                                dr["VALUES"] = "Blank";
                                ds_result.Tables["ERR"].Rows.Add(dr);
                            }
                            else
                            {
                                string[] words = dtResult.Rows[j]["LineNumber"].ToString().Split('.');
                                result.Append("<INVLIN>" + words[0].ToString() + ".000" + "</INVLIN>");
                                //result.Append("<INVSLN>" + words[1].ToString() + "</INVSLN>");
                                result.Append("<INVSLN>" + dtResult.Rows[j]["LineNumber"].ToString() + "</INVSLN>");
                            }

                            if (dtResult.Rows[j]["Qty"].ToString().Trim() == string.Empty)
                            {
                                result.Append("<EXPQTY>0</EXPQTY>");
                            }
                            else
                            {
                                result.Append("<EXPQTY>" + dtResult.Rows[j]["Qty"].ToString().Trim() + "</EXPQTY>");
                            }

                            if (dtResult.Rows[j]["itemNumber"].ToString().Trim() == string.Empty)
                            {
                                Count_error = true;
                                result.Append("<PRTNUM/>");

                                dr = ds_result.Tables["ERR"].NewRow();
                                dr["LINE_NO"] = nCount_line;
                                dr["PO_NO"] = searchstring;
                                dr["TAG_NAME"] = "LINE_SEG-PRTNUM";
                                dr["VALUES"] = "Blank";
                                ds_result.Tables["ERR"].Rows.Add(dr);
                            }
                            else
                            {
                                result.Append("<PRTNUM>" + dtResult.Rows[j]["itemNumber"].ToString().Trim() + "</PRTNUM>");
                            }

                            result.Append("<LOTNUM/>");
                            result.Append("<RCVSTS>1</RCVSTS>");

                            if (dtResult.Rows[j]["UOM"].ToString().Trim() == string.Empty)
                            {
                                result.Append("<INV_ATTR_STR1/>");
                            }
                            else
                            {
                                result.Append("<INV_ATTR_STR1>" + dtResult.Rows[j]["UOM"].ToString().Trim() + "</INV_ATTR_STR1>");
                            }

                            result.Append("<TPL_LOC/>");
                            result.Append("<TPL_LOTNUM/>");

                            #endregion LINE_SEG Data
                            result.Append("</LINE_SEG>");
                        }
                        #endregion <LINE_SEG>

                        result.Append("</HEADER_SEG>");
                        #endregion HEADER_SEG

                        result.Append("</CTRL_SEG>");
                        #endregion CTRL_SEG

                        result.Append("</RA_INB_IFD>");
                        #endregion RA_INB_IFD


                        if (Count_error) // Error
                        {
                            #region Create file error 
                           
                            string sSubject = "Error file Name : " + Filename;
                            classmail.Sendmail(MailTo, smtp, string.Empty, MailFrom, sSubject, ds_result);
                            System.Threading.Thread.Sleep(1000); //searchstring
                            #endregion
                        }
                        else // Not error
                        {
                            #region Create File
                            string FileName = PRE_Naming + "_" + searchstring.PadLeft(8, '0') + "_" + DateTime.Now.ToString("yyMMddhhmmss", (new System.Globalization.CultureInfo("en-US"))) + "001";
                            StreamWriter objWriter = new StreamWriter(GetPATHXML + "\\" + FileName + ".xml", false);
                            objWriter.Write(result.ToString());
                            objWriter.Close();
                            System.Threading.Thread.Sleep(1000);

                            //if (bCheck) // Convert XML success  Send mail
                            //{
                            //    string sSubject = PRE_Naming + "_" + "_" + searchstring;
                            //    classmail.Sendmail(MailTo, smtp, "Convert text file to XML success : " + FileName + ".xml", MailFrom, sSubject);
                            //}

                            //string sSubject = PRE_Naming + "_" + searchstring;
                            //classmail.Sendmail(MailTo, smtp, "Convert text file to XML success : " + FileName + ".xml", MailFrom, sSubject);


                            DataRow dr_Mail = ds_mail.Tables["Mail"].NewRow();
                            dr_Mail["LINE_NO"] = nCount;
                            dr_Mail["MESSAGE"] = "Convert text file to XML success : " + FileName + ".xml";
                            ds_mail.Tables["Mail"].Rows.Add(dr_Mail);
                            #endregion
                        }
                    }
                    #region Move File
                    string result_Move;
                    result_Move = Path.GetFileName(filepath);

                    #region send mail success
                    if (ds_mail.Tables.Count != 0)
                    {
                        string sSubject_Mail = strSubject + ": " + result_Move;
                        classmail.Sendmail(MailTo, smtp, string.Empty, MailFrom, sSubject_Mail, ds_mail);
                    }
                    #endregion

                    string sPath_bak = sPath_BAK + "/" + DateTime.Now.ToString("yyyyMMdd");
                    bool exists = System.IO.Directory.Exists(sPath_bak);
                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(sPath_bak);
                    }

                    System.IO.File.Move(sPath_Source + "/" + result_Move, sPath_bak + "/" + DateTime.Now.ToString("yyyyMMddhhmmss", (new System.Globalization.CultureInfo("en-US"))) + "_" + result_Move);
                    #endregion
                }

                string FtpSync_Outbound = System.Configuration.ConfigurationSettings.AppSettings["FtpSync_Outbound"].ToString();
                System.Diagnostics.Process pProcess_out = new System.Diagnostics.Process();
                pProcess_out.StartInfo.WorkingDirectory = WorkingDirectory;
                pProcess_out.StartInfo.FileName = FtpSync_Outbound;
                pProcess_out.StartInfo.CreateNoWindow = true; //not diplay a windows
                pProcess_out.Start();
                pProcess_out.WaitForExit();

            }
            catch (Exception ex)
            {
                classmail.Sendmail(MailTo, smtp, "Error SO :" + ex.Message.ToString(), MailFrom, strSubject);
                return;
            }
            finally
            {
                this.Dispose();
            }
        }
    }
}
