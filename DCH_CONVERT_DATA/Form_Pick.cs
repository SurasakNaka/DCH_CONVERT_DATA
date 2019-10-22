using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Principal;
using System.Net;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace DCH_CONVERT_DATA
{
    public partial class Form_Pick : Form
    {

        public Form_Pick()
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
        private bool CopyFile()
        {
            return true;
    //        try
    //        {
    //            string PATH_TARGET = System.Configuration.ConfigurationSettings.AppSettings["PATH_TARGET"].ToString();
    //            string PATH_SOURCE = System.Configuration.ConfigurationSettings.AppSettings["PATH_SOURCE"].ToString();
    //            string USER_NAME = System.Configuration.ConfigurationSettings.AppSettings["USER_NAME"].ToString();
    //            string PASSWORD = System.Configuration.ConfigurationSettings.AppSettings["PASSWORD"].ToString();
    //            string sPrefix = System.Configuration.ConfigurationSettings.AppSettings["PRE_FIX"].ToString();

    //            //string username = "alexander.junior";
    //            //string password = "lordsusan126*";
    //            //string updir = PATH_SOURCE;// @"\\Z:\Alex\SIB_TEST\abcd.txt";


    //            //AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
    //            //WindowsIdentity identity = new WindowsIdentity(USER_NAME, PASSWORD);
    //            //WindowsImpersonationContext context = identity.Impersonate();
    //            //File.Move(PATH_SOURCE, PATH_TARGET);

    //            //foreach (string filepath in Directory.GetFiles(updir, sPrefix + "*.txt"))
    //            //{
    //            //    File.Move(filepath, PATH_TARGET);
    //            //}

    //            //var networkPath = @"//server/share";

    //            var oNetworkCredential =
    //new System.Net.NetworkCredential()
    //{
    //    Domain = "lfasia",
    //    UserName = "lfasia" + "\\" + "SurasakNaka",
    //    Password = "Aug@2019"
    //};

    //            var networkPath = PATH_SOURCE;
    //            var credentials = new NetworkCredential(USER_NAME, PASSWORD);
    //            string[] fileList;
    //            using (new NetworkConnection(networkPath, oNetworkCredential))
    //            {
    //                fileList = Directory.GetFiles(networkPath);
    //            }

    //            //foreach (var file in fileList)
    //            //{
    //            //    Console.WriteLine("{0}", Path.GetFileName(file));
    //            //}

    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            return false;
    //        }
        }
        #endregion
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
        private DataSet AddCoulumn()
        {
            DataSet ds_output = new DataSet();
            DataTable dt = new DataTable("ERR");
            try
            {
                
                dt.Columns.Add("LINE_NO", typeof(String));
                dt.Columns.Add("SALES_ORDER_NO", typeof(String));
                dt.Columns.Add("TAG_NAME", typeof(String));
                dt.Columns.Add("VALUES", typeof(String));
                ds_output.Tables.Add(dt);

                //ds_output.Tables.Add(new DataTable());
                //ds_output.Tables["ERR"].Columns.Add("LINE_NO", typeof(string));
                //ds_output.Tables["ERR"].Columns.Add("SALES_ORDER_NO", typeof(string));
                //ds_output.Tables["ERR"].Columns.Add("TAG_NAME", typeof(string));
                //ds_output.Tables["ERR"].Columns.Add("VALUES", typeof(string));
                return ds_output;
            }
            catch (Exception ex)
            {
                return ds_output;
            }

        }
        private void Form_Pick_Load(object sender, EventArgs e)
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
                //Process.Start(@"D:\Other\FTP_Sync\ScpSync.exe Session_GetFileInbound");

                System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                pProcess.StartInfo.WorkingDirectory = WorkingDirectory;
                pProcess.StartInfo.FileName = FtpSync_Inbound;
                //pProcess.StartInfo.Arguments = "";// "Session_GetFileInbound_Send"; //argument
                //pProcess.StartInfo.UseShellExecute = false;
                //pProcess.StartInfo.RedirectStandardOutput = true;
                //pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                pProcess.Start();
                pProcess.WaitForExit();

                string sPath_Source = GetPATH_SOURCE();
                string sPath_BAK = GetPATH_BAK();
                string GetPATHXML = GetPATH_XML();
                string[] sFiles = null;
                sFiles = GetPRE_FIX(sPath_Source);
                DataTable dt = new DataTable();
                DataTable dt_no = new DataTable();
                DataTable dtResult = new DataTable();
                DataSet ds_mail = new DataSet();

                StringBuilder result = new StringBuilder();
                StringBuilder result_error = new StringBuilder();
                string sPrefix = System.Configuration.ConfigurationSettings.AppSettings["PRE_FIX"].ToString();
                string WHSE_ID = System.Configuration.ConfigurationSettings.AppSettings["WHSE_ID"].ToString();
                string CLIENT_ID = System.Configuration.ConfigurationSettings.AppSettings["CLIENT_ID"].ToString();
                string PRT_CLIENT_ID = System.Configuration.ConfigurationSettings.AppSettings["PRT_CLIENT_ID"].ToString();
                string SHIP_TO_JDE_ADDRESS = "";
                string SALES_ORDER_NO = "";
                string SHIP_TO = "";
                bool bCheck = false;
                bool Count_error = false;
                int nCount = 0;
                int nCount_line = 0;
                string Filename = string.Empty;
                foreach (string filepath in Directory.GetFiles(sPath_Source, sPrefix + "*.txt"))
                {
                    Filename = string.Empty;
                    Filename = Path.GetFileName(filepath);
                    bCheck = true;
                    dt = new DataTable();
                    dt_no = new DataTable();
                    dt = ConvertToDataTable(filepath, out dt_no);
                    Count_error = false;
                    string searchstring = string.Empty;
                    nCount = 0;
                    DataSet ds_result = new DataSet();
                    DataRow dr;

                    ds_mail = new DataSet();
                    ds_mail = AddCoulumn_reusultMail();

                    for (int i = 0; i < dt_no.Rows.Count; i++)
                    {
                        nCount = nCount + 1;
                        result_error = new StringBuilder();
                        if (searchstring != dt_no.Rows[i]["SALES_ORDER_NO"].ToString().Trim())
                        {
                            ds_result = AddCoulumn();
                        }
                        searchstring = dt_no.Rows[i]["SALES_ORDER_NO"].ToString();

                        dtResult = new DataTable();
                        dtResult = dt.Select("SALES_ORDER_NO LIKE '%" + searchstring + "%'").CopyToDataTable();

                        SHIP_TO_JDE_ADDRESS = dtResult.Rows[0]["SHIP_TO_JDE_ADDRESS"].ToString();
                        SALES_ORDER_NO = dtResult.Rows[0]["SALES_ORDER_NO"].ToString();
                        SHIP_TO = SHIP_TO_JDE_ADDRESS.PadLeft(8, '0');
                        result = new StringBuilder();
                        result.Append(@"<?xml version='1.0' encoding= 'UTF-8'?>");
                        //result.AppendLine();
                        #region ORDER_INB_IFD                       
                        result.Append(@"<ORDER_INB_IFD xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>");
                        //result.AppendLine();
                        #region <CTRL_SEG>
                        result.Append("<CTRL_SEG>");
                        //result.AppendLine();
                        result.Append("<TRNNAM>ORDER_TRAN</TRNNAM>");
                        //result.AppendLine();
                        result.Append("<TRNVER>9.1</TRNVER>");
                        //result.AppendLine();
                        result.Append(@"<TRNDTE>" + DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss") + "</TRNDTE>");
                        //result.AppendLine();
                        result.Append("<WHSE_ID>" + WHSE_ID + "</WHSE_ID>");
                        //result.AppendLine();

                        #region ST_CUST_SEG
                        result.Append("<ST_CUST_SEG>");
                        //result.AppendLine();

                        result.Append("<CLIENT_ID>" + CLIENT_ID + "</CLIENT_ID>"); // Client ID Waitng DMT Asssign
                                                                       //result.AppendLine();

                        string HOST_EXT_ID = "PAHC-" + dtResult.Rows[0]["SALES_ORDER_TYPE"].ToString() + dtResult.Rows[0]["SALES_ORDER_NO"].ToString() + SHIP_TO;
                        result.Append("<HOST_EXT_ID>" + HOST_EXT_ID + "</HOST_EXT_ID>");
                        //result.AppendLine();

                        if (dtResult.Rows[0]["SHIP_TO_NAME"].ToString() == string.Empty)
                        {
                            Count_error = true;
                            result.Append("<ADRNAM/>"); //result_error

                            dr = ds_result.Tables["ERR"].NewRow();
                            dr["LINE_NO"] = nCount;
                            dr["SALES_ORDER_NO"] = searchstring;
                            dr["TAG_NAME"] = "ST_CUST_SEG-ADRNAM";
                            dr["VALUES"] = "Blank";
                            ds_result.Tables["ERR"].Rows.Add(dr);

                            //result_error.AppendLine("SALES_ORDER_NO :" + " " + searchstring + " " + " Schema ST_CUST_SEG-ADRNAM is Blank, ");
                        }
                        else
                        {
                            result.Append("<ADRNAM>" + dtResult.Rows[0]["SHIP_TO_NAME"].ToString() + "</ADRNAM>");
                        }
                        //result.AppendLine();

                        result.Append("<ADRTYP>CST</ADRTYP>");
                        //result.AppendLine();

                        if (dtResult.Rows[0]["SHIP_TO_ADDRESS_LINE1"].ToString() == string.Empty)
                        {
                            result.Append("<ADRLN1/>");

                            Count_error = true;
                            //result_error.AppendLine("SALES_ORDER_NO :" + " " + searchstring + " " + " Schema ST_CUST_SEG-ADRLN1 is Blank, ");

                            dr = ds_result.Tables["ERR"].NewRow();
                            dr["LINE_NO"] = nCount;
                            dr["SALES_ORDER_NO"] = searchstring;
                            dr["TAG_NAME"] = "ST_CUST_SEG-ADRLN1";
                            dr["VALUES"] = "Blank";
                            ds_result.Tables["ERR"].Rows.Add(dr);
                        }
                        else
                        {
                            result.Append("<ADRLN1>" + dtResult.Rows[0]["SHIP_TO_ADDRESS_LINE1"].ToString() + "</ADRLN1>");

                        }
                        //result.AppendLine();

                        if (dtResult.Rows[0]["SHIP_TO_ADDRESS_LINE2"].ToString() == string.Empty)
                        {
                            result.Append("<ADRLN2/>");
                        }
                        else
                        {
                            result.Append("<ADRLN2>" + dtResult.Rows[0]["SHIP_TO_ADDRESS_LINE2"].ToString() + "</ADRLN2>");

                        }
                        //result.AppendLine();

                        if (dtResult.Rows[0]["SHIP_TO_ADDRESS_LINE3"].ToString() == string.Empty)
                        {
                            result.Append("<ADRLN3/>");
                        }
                        else
                        {
                            result.Append("<ADRLN3>" + dtResult.Rows[0]["SHIP_TO_ADDRESS_LINE3"].ToString() + "</ADRLN3>");

                        }
                        //result.AppendLine();

                        if (dtResult.Rows[0]["SHIP_TO_CITY"].ToString() == string.Empty)
                        {
                            result.Append("<ADRCTY/>");
                        }
                        else
                        {
                            result.Append("<ADRCTY>" + dtResult.Rows[0]["SHIP_TO_CITY"].ToString() + "</ADRCTY>");
                        }
                        //result.AppendLine();

                        if (dtResult.Rows[0]["SHIP_TO_STATE_CODE"].ToString() == string.Empty)
                        {
                            result.Append("<ADRSTC/>");
                        }
                        else
                        {
                            result.Append("<ADRSTC>" + dtResult.Rows[0]["SHIP_TO_STATE_CODE"].ToString() + "</ADRSTC>");
                        }
                        //result.AppendLine();


                        if (dtResult.Rows[0]["SHIP_TO_ZIP_CODE"].ToString() == string.Empty)
                        {
                            result.Append("<ADRPSZ/>");
                        }
                        else
                        {
                            result.Append("<ADRPSZ>" + dtResult.Rows[0]["SHIP_TO_ZIP_CODE"].ToString() + "</ADRPSZ>");
                        }
                        //result.AppendLine();

                        result.Append("<CTRY_NAME>TH</CTRY_NAME>");
                        //result.AppendLine();

                        if (dtResult.Rows[0]["SHIP_TO_PHONE_NO"].ToString() == string.Empty)
                        {
                            result.Append("<PHNNUM/>");
                        }
                        else
                        {
                            result.Append("<PHNNUM>" + dtResult.Rows[0]["SHIP_TO_PHONE_NO"].ToString() + "</PHNNUM>");
                        }
                        //result.AppendLine();

                        result.Append(@"<FAXNUM/>");
                        result.Append(@"<LAST_NAME/>");
                        result.Append(@"<FIRST_NAME/>");
                        result.Append(@"<WEB_ADR/>");
                        result.Append(@"<EMAIL_ADR/>");
                        result.Append(@"<ATTN_NAME/>");
                        result.Append(@"<ATTN_TEL/>");


                        if (dtResult.Rows[0]["SHIP_TO_NAME"].ToString() == string.Empty)
                        {
                            result.Append("<CONT_NAME/>");
                        }
                        else
                        {
                            result.Append("<CONT_NAME>" + dtResult.Rows[0]["SHIP_TO_NAME"].ToString() + "</CONT_NAME>");
                        }
                        //result.AppendLine();

                        if (dtResult.Rows[0]["SHIP_TO_PHONE_NO"].ToString() == string.Empty)
                        {
                            result.Append("<CONT_TEL/>");
                        }
                        else
                        {
                            result.Append("<CONT_TEL>" + dtResult.Rows[0]["SHIP_TO_PHONE_NO"].ToString() + "</CONT_TEL>");
                        }
                        //result.AppendLine();

                        result.Append(@"<CONT_TITLE/>");
                        //result.AppendLine();
                        result.Append(@"</ST_CUST_SEG>");
                        //result.AppendLine();
                        #endregion ST_CUST_SEG

                        #region <ORDER_SEG>
                        result.Append("<ORDER_SEG>");
                        //result.AppendLine();

                        result.Append("<SEGNAM>ORDER</SEGNAM>");
                        //result.AppendLine();

                        result.Append("<TRNTYP>A</TRNTYP>"); // รอ Confirm
                                                             //result.AppendLine();

                        result.Append("<CLIENT_ID>" + CLIENT_ID + "</CLIENT_ID>");// WAITING_DATA
                        //result.AppendLine();

                        if (dtResult.Rows[0]["SALES_ORDER_TYPE"].ToString().Trim() + dtResult.Rows[0]["SALES_ORDER_NO"].ToString().Trim() == string.Empty)
                        {
                            result.Append(@"<ORDNUM/>");

                            Count_error = true;
                            //result_error.AppendLine("SALES_ORDER_NO :" + " " + searchstring + " " + " Schema ORDER_SEG-ORDNUM is Blank, ");

                            dr = ds_result.Tables["ERR"].NewRow();
                            dr["LINE_NO"] = nCount;
                            dr["SALES_ORDER_NO"] = searchstring;
                            dr["TAG_NAME"] = "ORDER_SEG-ORDNUM";
                            dr["VALUES"] = "Blank";
                            ds_result.Tables["ERR"].Rows.Add(dr);
                        }
                        else
                        {
                            string ORDNUM = "PAHC-" + dtResult.Rows[0]["SALES_ORDER_TYPE"].ToString() + dtResult.Rows[0]["SALES_ORDER_NO"].ToString();
                            result.Append("<ORDNUM>" + ORDNUM + "</ORDNUM>");
                        }

                        //result.AppendLine();

                        result.Append("<ORDTYP>C</ORDTYP>");
                        //result.AppendLine();
                        string sTime = "T" + DateTime.Now.ToString("hh:mm:ss");
                        if (dtResult.Rows[0]["REQUESTED_DATE"].ToString() == string.Empty)
                        {
                            result.Append("<ENTDTE/>");
                        }
                        else
                        {
                            result.Append("<ENTDTE>" + dtResult.Rows[0]["REQUESTED_DATE"].ToString().Substring(4, 4) + "-" + dtResult.Rows[0]["REQUESTED_DATE"].ToString().Substring(0, 2) + "-" + dtResult.Rows[0]["REQUESTED_DATE"].ToString().Substring(2, 2) + sTime + "</ENTDTE>");

                        }
                        //result.AppendLine();


                        if (dtResult.Rows[0]["SHIP_TO_JDE_ADDRESS"].ToString() == string.Empty)
                        {
                            result.Append("<STCUST/>");
                        }
                        else
                        {
                            result.Append("<STCUST>" + dtResult.Rows[0]["SHIP_TO_JDE_ADDRESS"].ToString() + "</STCUST>");

                        }
                        //result.AppendLine();

                        result.Append("<BTCUST/>"); // รอ Confirm 
                                                    //result.AppendLine();

                        if (dtResult.Rows[0]["CUSTOMER_PURCHASE_ORDER_NO"].ToString() == string.Empty)
                        {
                            result.Append("<CPONUM/>");
                        }
                        else
                        {
                            result.Append("<CPONUM>" + dtResult.Rows[0]["CUSTOMER_PURCHASE_ORDER_NO"].ToString() + "</CPONUM>");

                        }
                        //result.AppendLine();

                        if (dtResult.Rows[0]["ORDER_DATE"].ToString() == string.Empty)
                        {
                            result.Append("<CPODTE/>");
                        }
                        else
                        {
                            result.Append("<CPODTE>" + dtResult.Rows[0]["ORDER_DATE"].ToString().Substring(4, 4) + "-" + dtResult.Rows[0]["ORDER_DATE"].ToString().Substring(0, 2) + "-" + dtResult.Rows[0]["ORDER_DATE"].ToString().Substring(2, 2) + sTime + "</CPODTE>");
                            //result.Append("<CPODTE>" + dtResult.Rows[0]["ORDER_DATE"].ToString().Substring(4, 4) +  dtResult.Rows[0]["ORDER_DATE"].ToString().Substring(0, 2) + dtResult.Rows[0]["ORDER_DATE"].ToString().Substring(2, 2) +  "</CPODTE>");

                        }
                        //result.AppendLine();

                        if (dtResult.Rows[0]["TERMS_OF_DELIVERY1"].ToString() == string.Empty)
                        {
                            result.Append("<PAYTRM/>");
                        }
                        else
                        {
                            result.Append("<PAYTRM>" + dtResult.Rows[0]["TERMS_OF_DELIVERY1"].ToString().Trim() + "</PAYTRM>");

                        }
                        //result.AppendLine();

                        result.Append("<RUSH_FLG>0</RUSH_FLG>");
                        //result.AppendLine();

                        result.Append("<DEPTNO/>");
                        //result.AppendLine();

                        if (dtResult.Rows[0]["SHIP_TO_ZIP_CODE"].ToString() == string.Empty)
                        {
                            result.Append("<DUTY_PAYMENT_ACCT/>");
                        }
                        else
                        {
                            result.Append("<DUTY_PAYMENT_ACCT>" + dtResult.Rows[0]["SHIP_TO_ZIP_CODE"].ToString() + "</DUTY_PAYMENT_ACCT>");

                        }
                        //result.AppendLine();

                        #region  ORDER_NOTE_SEG

                        result.Append("<ORDER_NOTE_SEG>");
                        //result.AppendLine();

                        result.Append("<SEGNAM>ORDER_NOTE</SEGNAM>");
                        //result.AppendLine();

                        string ORDNUM1 = "PAHC-" + dtResult.Rows[0]["SALES_ORDER_TYPE"].ToString() + dtResult.Rows[0]["SALES_ORDER_NO"].ToString();//"PAHC-" + dtResult.Rows[0]["SALES_ORDER_NO"].ToString();
                        result.Append("<ORDNUM>" + ORDNUM1 + "</ORDNUM>");
                        //result.AppendLine();

                        result.Append("<NOTLIN>1</NOTLIN>");
                        //result.AppendLine();

                        string DELIVERY_INSTRUCTIONS_LINE = string.Empty;
                        DELIVERY_INSTRUCTIONS_LINE = dtResult.Rows[0]["DELIVERY_INSTRUCTIONS_LINE1"].ToString() + dtResult.Rows[0]["DELIVERY_INSTRUCTIONS_LINE2"].ToString();
                        //if (DELIVERY_INSTRUCTIONS_LINE.Trim() == string.Empty)
                        //{
                        //    result.Append("<NOTTXT/>");

                        //    Count_error = true;
                        //    //result_error.AppendLine("SALES_ORDER_NO :" + " " + searchstring + " " + " Schema ORDER_NOTE_SEG-NOTTXT is Blank, ");

                        //    dr = ds_result.Tables["ERR"].NewRow();
                        //    dr["LINE_NO"] = nCount;
                        //    dr["SALES_ORDER_NO"] = searchstring;
                        //    dr["TAG_NAME"] = "ORDER_NOTE_SEG-NOTTXT";
                        //    dr["VALUES"] = "Blank";
                        //    ds_result.Tables["ERR"].Rows.Add(dr);
                        //}
                        //else
                        //{
                        //    result.Append("<NOTTXT>" + DELIVERY_INSTRUCTIONS_LINE + "</NOTTXT>");
                        //}
                        result.Append("<NOTTXT>" + DELIVERY_INSTRUCTIONS_LINE + "</NOTTXT>");
                        //result.AppendLine();

                        result.Append("<NOTTYP>GENERIC</NOTTYP>");
                        //result.AppendLine();

                        result.Append("</ORDER_NOTE_SEG>");
                        //result.AppendLine();
                        #endregion ORDER_NOTE_SEG

                        #region ORDER_LINE_SEG
                        nCount_line = 0;
                        for (int j = 0; j < dtResult.Rows.Count; j++)
                        {
                            nCount_line = nCount_line + 1;
                            result.Append("<ORDER_LINE_SEG>");
                            //result.AppendLine();

                            result.Append("<SEGNAM>ORDER_LINE</SEGNAM>");
                            //result.AppendLine();

                            string ORDNUM_LINE_SEG = string.Empty;
                            ORDNUM_LINE_SEG = "PAHC-" + dtResult.Rows[j]["SALES_ORDER_TYPE"].ToString() + dtResult.Rows[j]["SALES_ORDER_NO"].ToString();
                            if (ORDNUM_LINE_SEG.Trim() == string.Empty)
                            {

                                result.Append("<ORDNUM/>");

                                Count_error = true;
                                //result_error.AppendLine("SALES_ORDER_NO :" + " " + searchstring + " LINE No." + " " + nCount_line + " Schema ORDER_LINE_SEG-ORDNUM is Blank, ");
                                dr = ds_result.Tables["ERR"].NewRow();
                                dr["LINE_NO"] = nCount_line;
                                dr["SALES_ORDER_NO"] = searchstring;
                                dr["TAG_NAME"] = "ORDER_LINE_SEG-ORDNUM";
                                dr["VALUES"] = "Blank";
                                ds_result.Tables["ERR"].Rows.Add(dr);
                            }
                            else
                            {
                                result.Append("<ORDNUM>" + ORDNUM_LINE_SEG + "</ORDNUM>");
                            }
                            //result.AppendLine();

                            if (dtResult.Rows[j]["LINE_NO"].ToString().Trim() == string.Empty)
                            {
                                result.Append("<ORDLIN/>");
                                result.Append("<ORDSLN/>");

                                Count_error = true;
                                //result_error.AppendLine("SALES_ORDER_NO :" + " " + searchstring + "LINE No." + " " + nCount_line + " Schema ORDER_LINE_SEG-ORDLIN is Blank, ");
                                //result_error.AppendLine("SALES_ORDER_NO :" + " " + searchstring + "LINE No." + " " + nCount_line + " Schema ORDER_LINE_SEG-ORDSLN is Blank, ");

                                dr = ds_result.Tables["ERR"].NewRow();
                                dr["LINE_NO"] = nCount_line;
                                dr["SALES_ORDER_NO"] = searchstring;
                                dr["TAG_NAME"] = "ORDER_LINE_SEG-ORDNUM";
                                dr["VALUES"] = "Blank";
                                ds_result.Tables["ERR"].Rows.Add(dr);

                                dr = ds_result.Tables["ERR"].NewRow();
                                dr["LINE_NO"] = nCount_line;
                                dr["SALES_ORDER_NO"] = searchstring;
                                dr["TAG_NAME"] = "ORDER_LINE_SEG-ORDSLN";
                                dr["VALUES"] = "Blank";
                                ds_result.Tables["ERR"].Rows.Add(dr);
                            }
                            else
                            {

                                string[] words = dtResult.Rows[j]["LINE_NO"].ToString().Split('.');

                                result.Append("<ORDLIN>" + words[0].ToString() + "</ORDLIN>");
                                //result.AppendLine();
                                result.Append("<ORDSLN>" + words[1].ToString() + "</ORDSLN>");
                                //result.AppendLine();


                            }


                            if (dtResult.Rows[j]["PAHC_ITEM_NO"].ToString().Trim() == string.Empty)// PAHC_ITEM_NO
                            {
                                result.Append("<PRTNUM/>");
                                Count_error = true;
                                //result_error.AppendLine("SALES_ORDER_NO :" + " " + searchstring + " LINE No." + " " + nCount_line + " Schema ORDER_LINE_SEG-PRTNUM is Blank, ");

                                dr = ds_result.Tables["ERR"].NewRow();
                                dr["LINE_NO"] = nCount_line;
                                dr["SALES_ORDER_NO"] = searchstring;
                                dr["TAG_NAME"] = "ORDER_LINE_SEG-PRTNUM";
                                dr["VALUES"] = "Blank";
                                ds_result.Tables["ERR"].Rows.Add(dr);
                            }
                            else
                            {
                                result.Append("<PRTNUM>" + dtResult.Rows[j]["PAHC_ITEM_NO"].ToString() + "</PRTNUM>");
                            }
                            //result.AppendLine();

                            result.Append("<PRT_CLIENT_ID>" + PRT_CLIENT_ID + "</PRT_CLIENT_ID>"); // WAITING_DATA
                            //result.AppendLine();

                            if (dtResult.Rows[j]["UNIT_OF_Measure"].ToString() == string.Empty)
                            {
                                result.Append("<UC_HOST_ORDUOM/>");
                            }
                            else
                            {
                                result.Append("<UC_HOST_ORDUOM>" + dtResult.Rows[j]["UNIT_OF_Measure"].ToString() + "</UC_HOST_ORDUOM>");
                            }
                            //result.AppendLine();

                            if (dtResult.Rows[j]["LOT_NO"].ToString() == string.Empty)
                            {
                                result.Append("<LOTNUM/>");
                            }
                            else
                            {
                                result.Append("<LOTNUM>" + dtResult.Rows[j]["LOT_NO"].ToString() + "</LOTNUM>");
                            }
                            //result.AppendLine();

                            if (dtResult.Rows[j]["QUANTITY"].ToString().Trim() == string.Empty)
                            {
                                result.Append("<ORDQTY/>");
                                Count_error = true;
                                //result_error.AppendLine("SALES_ORDER_NO :" + " " + searchstring + " LINE No." + " " + nCount_line + " Schema ORDER_LINE_SEG-ORDQTY is Blank, ");

                                dr = ds_result.Tables["ERR"].NewRow();
                                dr["LINE_NO"] = nCount_line;
                                dr["SALES_ORDER_NO"] = searchstring;
                                dr["TAG_NAME"] = "ORDER_LINE_SEG-ORDQTY";
                                dr["VALUES"] = "Blank";
                                ds_result.Tables["ERR"].Rows.Add(dr);
                            }
                            else
                            {
                                result.Append("<ORDQTY>" + dtResult.Rows[j]["QUANTITY"].ToString().Trim().Split('.')[0] + "</ORDQTY>");
                                //result.AppendLine();
                            }


                            result.Append("<PRJNUM/>");
                            //result.AppendLine();

                            if (dtResult.Rows[j]["PAHC_ITEM_NO"].ToString() == string.Empty)// PAHC_ITEM_NO
                            {
                                result.Append("<CSTPRT/>");
                            }
                            else
                            {
                                result.Append("<CSTPRT>" + dtResult.Rows[j]["PAHC_ITEM_NO"].ToString() + "</CSTPRT>");
                            }
                            //result.AppendLine();

                            result.Append("<RSVPRI/>");
                            //result.AppendLine();

                            result.Append("<SALES_ORDLIN>0.000</SALES_ORDLIN>");
                            //result.AppendLine();

                            //result.Append("<RSVQTY>" + dtResult.Rows[j]["QUANTITY"].ToString() + "</RSVQTY>");
                            if (dtResult.Rows[j]["QUANTITY"].ToString().Trim() == string.Empty)
                            {
                                result.Append("<RSVQTY/>");
                                Count_error = true;
                                //result_error.AppendLine("SALES_ORDER_NO :" + " " + searchstring + " LINE No." + " " + nCount_line + " Schema ORDER_LINE_SEG-ORDQTY is Blank, ");

                                dr = ds_result.Tables["ERR"].NewRow();
                                dr["LINE_NO"] = nCount_line;
                                dr["SALES_ORDER_NO"] = searchstring;
                                dr["TAG_NAME"] = "ORDER_LINE_SEG-RSVQTY";
                                dr["VALUES"] = "Blank";
                                ds_result.Tables["ERR"].Rows.Add(dr);
                            }
                            else
                            {
                                result.Append("<RSVQTY>" + dtResult.Rows[j]["QUANTITY"].ToString().Split('.')[0] + "</RSVQTY>");
                                //result.AppendLine();
                            }

                            //result.AppendLine();

                            //result.Append("<HOST_ORDQTY>" + dtResult.Rows[j]["QUANTITY"].ToString() + "</HOST_ORDQTY>");
                            if (dtResult.Rows[j]["QUANTITY"].ToString().Trim() == string.Empty)
                            {
                                result.Append("<HOST_ORDQTY/>");
                                Count_error = true;
                                //result_error.AppendLine("SALES_ORDER_NO :" + " " + searchstring + " LINE No." + " " + nCount_line + " Schema ORDER_LINE_SEG-ORDQTY is Blank, ");

                                dr = ds_result.Tables["ERR"].NewRow();
                                dr["LINE_NO"] = nCount_line;
                                dr["SALES_ORDER_NO"] = searchstring;
                                dr["TAG_NAME"] = "ORDER_LINE_SEG-HOST_ORDQTY";
                                dr["VALUES"] = "Blank";
                                ds_result.Tables["ERR"].Rows.Add(dr);
                            }
                            else
                            {
                                result.Append("<HOST_ORDQTY>" + dtResult.Rows[j]["QUANTITY"].ToString().Split('.')[0] + "</HOST_ORDQTY>");
                                //result.AppendLine();
                            }
                            //result.AppendLine();

                            result.Append("<HSTSTS>1</HSTSTS>");
                            //result.AppendLine();

                            result.Append("<MIN_SHELF_HRS>1</MIN_SHELF_HRS>");
                            //result.AppendLine();

                            result.Append("</ORDER_LINE_SEG>");
                            //result.AppendLine();
                        }
                        #endregion ORDER_LINE_SEG

                        ////result.AppendLine();
                        result.Append("</ORDER_SEG>");
                        #endregion </ORDER_SEG>
                        //result.AppendLine();
                        result.Append("</CTRL_SEG>");
                        #endregion
                        //result.AppendLine();
                        result.Append("</ORDER_INB_IFD>");
                        #endregion

                        if (Count_error) // Error
                        {
                            #region Create file error 
                            //string SHIP_TO_X = SHIP_TO_JDE_ADDRESS.PadLeft(8, '0');
                            string sSubject = "Error file Name : " + Filename;
                            classmail.Sendmail(MailTo, smtp, string.Empty, MailFrom, sSubject, ds_result);

                            System.Threading.Thread.Sleep(1000); //searchstring
                            #endregion
                        }
                        else // Not error
                        {
                            #region Create File
                            string SHIP_TO_X = SHIP_TO_JDE_ADDRESS.PadLeft(8, '0');
                            string FileName = PRE_Naming + "_" + searchstring.PadLeft(8, '0') + "_" + DateTime.Now.ToString("yyMMddhhmmss", (new System.Globalization.CultureInfo("en-US"))) + "001";

                            StreamWriter objWriter = new StreamWriter(GetPATHXML + "\\" + FileName + ".xml", false);
                            objWriter.Write(result.ToString());
                            objWriter.Close();

                            System.Threading.Thread.Sleep(1000);

                            //if (bCheck) // Convert XML success  Send mail
                            //{
                            //    string sSubject = PRE_Naming + "_" + SHIP_TO_X + "_" + searchstring;
                            //    classmail.Sendmail(MailTo, smtp, "Convert text file to XML success : " + FileName + ".xml", MailFrom, sSubject);
                            //}

                            //string sSubject = PRE_Naming + "_" + SHIP_TO_X + "_" + searchstring;
                            //classmail.Sendmail(MailTo, smtp, "Convert text file to XML success : " + FileName + ".xml", MailFrom, sSubject);

                            DataRow dr_Mail = ds_mail.Tables["Mail"].NewRow();
                            dr_Mail["LINE_NO"] = nCount;
                            dr_Mail["MESSAGE"] = "Convert Text to XML success : " + FileName + ".xml";
                            ds_mail.Tables["Mail"].Rows.Add(dr_Mail);
                            #endregion
                        }


                    }
                    #region Move File
                    string result_Move;
                    result_Move = Path.GetFileName(filepath);
                    string sPath_bak = sPath_BAK + "/" + DateTime.Now.ToString("yyyyMMdd");
                    bool exists = System.IO.Directory.Exists(sPath_bak);
                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(sPath_bak);
                    }

                    #region send mail success
                    if (ds_mail.Tables["Mail"].Rows.Count != 0)
                    {
                        string sSubject_Mail = strSubject + ": " + result_Move;
                        classmail.Sendmail(MailTo, smtp, string.Empty, MailFrom, sSubject_Mail, ds_mail);
                    }
                    #endregion

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

        public DataTable ConvertToDataTable(string fileName,out DataTable dt_no)
        {
            try
            {
                DataTable dt = new DataTable("Inbound");
                dt_no = new DataTable();
                #region add column
                dt.Columns.Add("SHIPMENT_NUMBER");
                dt.Columns.Add("COMPANY_NUMBER");
                dt.Columns.Add("SHIPPING_BRANCH");
                dt.Columns.Add("SALES_ORDER_NO");
                dt.Columns.Add("SALES_ORDER_TYPE");
                dt.Columns.Add("LINE_NO");
                dt.Columns.Add("PICK_SLIP_NO");
                dt.Columns.Add("CUSTOMER_PURCHASE_ORDER_NO");
                dt.Columns.Add("SHIP_TO_NAME");
                dt.Columns.Add("SHIP_TO_JDE_ADDRESS");
                dt.Columns.Add("SHIP_TO_ADDRESS_LINE1");
                dt.Columns.Add("SHIP_TO_ADDRESS_LINE2");
                dt.Columns.Add("SHIP_TO_ADDRESS_LINE3");
                dt.Columns.Add("SHIP_TO_CITY");
                dt.Columns.Add("SHIP_TO_STATE_CODE");
                dt.Columns.Add("SHIP_TO_ZIP_CODE");
                dt.Columns.Add("SHIP_TO_PHONE_NO");
                dt.Columns.Add("SOLD_TO_NAME");
                dt.Columns.Add("SOLD_TO_JDE_ADDRESS_NO");
                dt.Columns.Add("SOLD_TO_ADDRESS_LINE1");
                dt.Columns.Add("SOLD_TO_ADDRESS_LINE2");
                dt.Columns.Add("SOLD_TO_ADDRESS_LINE3");
                dt.Columns.Add("SOLD_TO_CITY");
                dt.Columns.Add("SOLD_TO_STATE_CODE");
                dt.Columns.Add("SOLD_TO_ZIP_CODE");
                dt.Columns.Add("PAHC_ITEM_NO");
                dt.Columns.Add("PAHC_ITEM_DESCRIPTION1");
                dt.Columns.Add("PAHC_ITEM_DESCRIPTION2");
                dt.Columns.Add("QUANTITY");
                dt.Columns.Add("UNIT_OF_Measure");
                dt.Columns.Add("LOT_NO");
                dt.Columns.Add("LOT_EXPIRATION_DATE");
                dt.Columns.Add("LOCATION");
                dt.Columns.Add("UNIT_WEIGHT");
                dt.Columns.Add("WEIGHT_UNIT_OF_MEASURE");
                dt.Columns.Add("UNIT_VOLUME");
                dt.Columns.Add("VOLUME_UNIT_OF_MEASURE");
                dt.Columns.Add("FREIGHT_CLASSIFICATION");
                dt.Columns.Add("FREIGHT_HANDLING_CODE");
                dt.Columns.Add("CARRIE_NUMBER");
                dt.Columns.Add("NUMBER_OF_PALLET");
                dt.Columns.Add("ORDER_DATE");
                dt.Columns.Add("SCHEDULED_PICK_DATE");
                dt.Columns.Add("REQUESTED_DATE");
                dt.Columns.Add("DELIVERY_INSTRUCTIONS_LINE1");
                dt.Columns.Add("DELIVERY_INSTRUCTIONS_LINE2");
                dt.Columns.Add("ORDER_CREATED");
                dt.Columns.Add("RELATED_ORDER_NUMBER");
                dt.Columns.Add("RELATED_ORDER_TYPE");
                dt.Columns.Add("TERMS_OF_DELIVERY1");
                dt.Columns.Add("TERMS_OF_DELIVERY2");
                dt.Columns.Add("UOM");
                dt.Columns.Add("DUMMY1");
                dt.Columns.Add("DUMMY2");

                #endregion
                #region Sale order
                dt_no.Columns.Add("SALES_ORDER_NO");
                #endregion
                var lines = File.ReadAllLines(fileName, System.Text.Encoding.UTF8).Where(s => s.Trim() != string.Empty).ToArray();
                // reading rest of the data
                for (int i = 0; i < lines.Count(); i++)
                {
                    DataRow dr = dt.NewRow();
                    string[] values = lines[i].Split(new char[] { '|' });

                    dr["SHIPMENT_NUMBER"] = values[0];
                    dr["COMPANY_NUMBER"] = values[1];
                    dr["SHIPPING_BRANCH"] = values[2];
                    dr["SALES_ORDER_NO"] = values[3];
                    dr["SALES_ORDER_TYPE"] = values[4];
                    dr["LINE_NO"] = values[5];
                    dr["PICK_SLIP_NO"] = values[6];
                    dr["CUSTOMER_PURCHASE_ORDER_NO"] = values[7];
                    dr["SHIP_TO_NAME"] = values[8];
                    dr["SHIP_TO_JDE_ADDRESS"] = values[9];
                    dr["SHIP_TO_ADDRESS_LINE1"] = values[10];
                    dr["SHIP_TO_ADDRESS_LINE2"] = values[11];
                    dr["SHIP_TO_ADDRESS_LINE3"] = values[12];
                    dr["SHIP_TO_CITY"] = values[13];
                    dr["SHIP_TO_STATE_CODE"] = values[14];
                    dr["SHIP_TO_ZIP_CODE"] = values[15];
                    dr["SHIP_TO_PHONE_NO"] = values[16];
                    dr["SOLD_TO_NAME"] = values[17];
                    dr["SOLD_TO_JDE_ADDRESS_NO"] = values[18];
                    dr["SOLD_TO_ADDRESS_LINE1"] = values[19];
                    dr["SOLD_TO_ADDRESS_LINE2"] = values[20];
                    dr["SOLD_TO_ADDRESS_LINE3"] = values[21];
                    dr["SOLD_TO_CITY"] = values[22];
                    dr["SOLD_TO_STATE_CODE"] = values[23];
                    dr["SOLD_TO_ZIP_CODE"] = values[24];
                    dr["PAHC_ITEM_NO"] = values[25];
                    dr["PAHC_ITEM_DESCRIPTION1"] = values[26];
                    dr["PAHC_ITEM_DESCRIPTION2"] = values[27];
                    dr["QUANTITY"] = values[28];
                    dr["UNIT_OF_Measure"] = values[29];
                    dr["LOT_NO"] = values[30];
                    dr["LOT_EXPIRATION_DATE"] = values[31];
                    dr["LOCATION"] = values[32];
                    dr["UNIT_WEIGHT"] = values[33];
                    dr["WEIGHT_UNIT_OF_MEASURE"] = values[34];
                    dr["UNIT_VOLUME"] = values[35];
                    dr["VOLUME_UNIT_OF_MEASURE"] = values[36];
                    dr["FREIGHT_CLASSIFICATION"] = values[37];
                    dr["FREIGHT_HANDLING_CODE"] = values[38];
                    dr["CARRIE_NUMBER"] = values[39];
                    dr["NUMBER_OF_PALLET"] = values[40];
                    dr["ORDER_DATE"] = values[41];
                    dr["SCHEDULED_PICK_DATE"] = values[42];
                    dr["REQUESTED_DATE"] = values[43];
                    dr["DELIVERY_INSTRUCTIONS_LINE1"] = values[44];
                    dr["DELIVERY_INSTRUCTIONS_LINE2"] = values[45];
                    dr["ORDER_CREATED"] = values[46];
                    dr["RELATED_ORDER_NUMBER"] = values[47];
                    dr["RELATED_ORDER_TYPE"] = values[48];
                    dr["TERMS_OF_DELIVERY1"] = values[49];
                    dr["TERMS_OF_DELIVERY2"] = values[50];
                    dr["UOM"] = values[51];
                    dr["DUMMY1"] = values[52];
                    dr["DUMMY2"] = values[53];
                    dt.Rows.Add(dr);


                    DataRow dr_no = dt_no.NewRow();
                    dr_no["SALES_ORDER_NO"] = values[3];
                    dt_no.Rows.Add(dr_no);
                }
                dt_no = dt_no.DefaultView.ToTable(true, "SALES_ORDER_NO"); // removeDuplicatesRows
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    public class NetworkConnection : IDisposable
    {
        readonly string _networkName;

        public NetworkConnection(string networkName, NetworkCredential credentials)
        {
            _networkName = networkName;

            var netResource = new NetResource
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            };

            var userName = string.IsNullOrEmpty(credentials.Domain)
                ? credentials.UserName
                : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

            var result = WNetAddConnection2(
                netResource,
                credentials.Password,
                userName,
                0);

            if (result != 0)
            {
                throw new Win32Exception(result, "Error connecting to remote share");
            }
        }

        ~NetworkConnection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_networkName, 0, true);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource,
            string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags,
            bool force);

        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ResourceScope Scope;
            public ResourceType ResourceType;
            public ResourceDisplaytype DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        public enum ResourceScope : int
        {
            Connected = 1,
            GlobalNetwork,
            Remembered,
            Recent,
            Context
        };

        public enum ResourceType : int
        {
            Any = 0,
            Disk = 1,
            Print = 2,
            Reserved = 8,
        }

        public enum ResourceDisplaytype : int
        {
            Generic = 0x0,
            Domain = 0x01,
            Server = 0x02,
            Share = 0x03,
            File = 0x04,
            Group = 0x05,
            Network = 0x06,
            Root = 0x07,
            Shareadmin = 0x08,
            Directory = 0x09,
            Tree = 0x0a,
            Ndscontainer = 0x0b
        }
    }
}
