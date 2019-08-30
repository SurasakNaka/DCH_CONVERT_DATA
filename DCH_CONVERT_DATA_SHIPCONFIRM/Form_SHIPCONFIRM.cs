using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DCH_CONVERT_DATA_SHIPCONFIRM
{
    public partial class Form_SHIPCONFIRM : Form
    {
        public Form_SHIPCONFIRM()
        {
            InitializeComponent();
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
        private void Form_SHIPCONFIRM_Load(object sender, EventArgs e)
        {
            string PATH_SOURCE = System.Configuration.ConfigurationSettings.AppSettings["PATH_SOURCE"].ToString();
            string PATH_BAK = System.Configuration.ConfigurationSettings.AppSettings["PATH_BAK"].ToString();
            string PATH_Target = System.Configuration.ConfigurationSettings.AppSettings["PATH_TARGET"].ToString();
            string sPrefix = System.Configuration.ConfigurationSettings.AppSettings["PRE_FIX"].ToString();
            string PRE_Naming_XML = System.Configuration.ConfigurationSettings.AppSettings["PRE_Naming_XML"].ToString();
            StringBuilder result = new StringBuilder();
            StringBuilder result_xml = new StringBuilder();
            string sOrder_No = string.Empty;

            string MailFrom = System.Configuration.ConfigurationSettings.AppSettings["MailFrom"].ToString();
            string MailTo = System.Configuration.ConfigurationSettings.AppSettings["MailTo"].ToString();
            string smtp = System.Configuration.ConfigurationSettings.AppSettings["SMTP"].ToString();
            string strSubject = System.Configuration.ConfigurationSettings.AppSettings["sSubjectmail"].ToString();

            string FtpSync_Inbound = System.Configuration.ConfigurationSettings.AppSettings["FtpSync_Inbound"].ToString();
            string WorkingDirectory = System.Configuration.ConfigurationSettings.AppSettings["WorkingDirectory"].ToString();

            string WHSE_ID = System.Configuration.ConfigurationSettings.AppSettings["WHSE_ID"].ToString();
            string CLIENT_ID = System.Configuration.ConfigurationSettings.AppSettings["CLIENT_ID"].ToString();
            ClassLibrarySendMail.ClassLibrarySendMail classmail = new ClassLibrarySendMail.ClassLibrarySendMail();
            try
            {
                System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                pProcess.StartInfo.WorkingDirectory = WorkingDirectory;
                pProcess.StartInfo.FileName = FtpSync_Inbound;
                pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                pProcess.Start();
                pProcess.WaitForExit();

                string Order_No = string.Empty;
                string Order_Type = string.Empty;
                string Line_Number = string.Empty;
                string PRTNUM = string.Empty;
                string sLocation = string.Empty;
                string LOTNUM = string.Empty;
                string COMQTY = string.Empty;
                string STKUOM = string.Empty;
                string Carrie_Number = string.Empty;
                string Value_ignored = string.Empty;
                string TRNDTE = string.Empty;
                string Carrie_Number2 = string.Empty;
                DataSet ds_mail = new DataSet();
                int nCount = 0;
                foreach (string filepath in Directory.GetFiles(PATH_SOURCE, sPrefix + "*.xml"))
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(filepath);

                    ds_mail = new DataSet();
                    ds_mail = AddCoulumn_reusultMail();
                    nCount = nCount + 1;
                    for (int i = 0; i < ds.Tables["SHIPMENT_LINE_SEG"].Rows.Count; i++)
                    {
                        string sTxt = string.Empty;
                        string sCompany = "00125";
                        if (ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["ORDNUM"].ToString() != string.Empty)
                        {
                            Order_No = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["ORDNUM"].ToString().Substring(7);// PAHC-SO150789-01 --> 150789-01
                            if (Order_No != string.Empty)
                            {
                                if (sOrder_No == string.Empty)
                                {
                                    sOrder_No = Order_No;
                                }
                            }
                        }
                        else
                        {

                            Order_No = string.Empty;
                        }

                        if (ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["ORDNUM"].ToString() != string.Empty)
                        {
                            Order_Type = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["ORDNUM"].ToString().Substring(5, 2); // PAHC-SO150789-01 --> SO
                        }
                        else
                        {
                            Order_Type = string.Empty;
                        }

                        if (ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["ORDLIN"].ToString() != string.Empty)
                        {
                            Line_Number = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["ORDLIN"].ToString() + "000"; // 1.001 --> 1.001000
                        }
                        else
                        {
                            Line_Number = string.Empty;
                        }
                       
                        PRTNUM = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["PRTNUM"].ToString();
                        sLocation = "";
                        LOTNUM = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["LOTNUM"].ToString();
                        COMQTY = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["COMQTY"].ToString();
                        STKUOM = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["STKUOM"].ToString();
                        Carrie_Number = "";
                        Value_ignored = "";
                        if (ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["TRNDTE"].ToString() != string.Empty)
                        {
                            TRNDTE = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["TRNDTE"].ToString(); // format mm/dd/yyyy  2019-07-26T16:37:23 ~
                            TRNDTE = TRNDTE.Substring(5, 2) + "/" + TRNDTE.Substring(8, 2) + "/" + TRNDTE.Substring(0, 4);
                        }
                        else
                        {
                            TRNDTE = string.Empty;
                        }
                       
                        Carrie_Number2 = ""; // DCHShipConfirm.txt

                        sTxt = sCompany.Trim() + "~" + Order_No.Trim() + "~" + Order_Type.Trim() + "~" + Line_Number.Trim() + "~" + PRTNUM.Trim();
                        sTxt += "~" + sLocation.Trim() + "~" + LOTNUM.Trim() + "~" + COMQTY.Trim() + "~" + STKUOM.Trim() + "~" + Carrie_Number.Trim();
                        sTxt += "~" + Value_ignored.Trim() + "~" + TRNDTE.Trim() + "~" + Carrie_Number2.Trim();
                        result.Append(sTxt);
                        result.AppendLine();
                    }

                    #region Create Text file
                    string PRE_Naming = System.Configuration.ConfigurationSettings.AppSettings["PRE_Naming"].ToString();
                    string FileName = PRE_Naming + "_" + sOrder_No + "_" + DateTime.Now.ToString("yyyyMMddhhmmss", (new System.Globalization.CultureInfo("en-US")));
                    StreamWriter objWriter = new StreamWriter(PATH_Target + @"\\" + FileName + ".txt", false);
                    objWriter.Write(result.ToString());
                    objWriter.Close();

                    DataRow dr_Mail = ds_mail.Tables["Mail"].NewRow();
                    dr_Mail["LINE_NO"] = nCount;
                    dr_Mail["MESSAGE"] = "Convert Ship confirm success : " + FileName + ".txt";
                    ds_mail.Tables["Mail"].Rows.Add(dr_Mail);

                    #endregion

                    #region Create P_601_E2A_INV .XML 
                    string CLIENT_ID_ORDER_NO = string.Empty;
                    CLIENT_ID_ORDER_NO = CLIENT_ID + sOrder_No;
                    result_xml = new StringBuilder();
                    result_xml.Append(@"<?xml version='1.0' encoding= 'UTF-8'?>");
                    #region UC_INVOICE_INB_IFD                       
                    result_xml.Append(@"<UC_INVOICE_INB_IFD xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:schemaLocation='http://jda.dch.ac2wave.com/UC_INVOICE_INB_IFD.xsd invoice_newl.xsd' xmlns='http://jda.dch.ac2wave.com/UC_INVOICE_INB_IFD.xsd'>");
                    result_xml.Append(@"<CTRL_SEG>");

                    result_xml.Append("<TRNNAM>DC_INVOICE_INB</TRNNAM>");
                    result_xml.Append("<TRNVER>2017.1</TRNVER>");
                    result_xml.Append(@"<TRNDTE>" + DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss") + "</TRNDTE>");
                    result_xml.Append("<WHSE_ID>" + WHSE_ID.Trim() + "</WHSE_ID>");

                    #region INVOICE_INB_SEG
                    result_xml.Append(@"<INVOICE_INB_SEG>");
                    result_xml.Append("<TRNTYP>C</TRNTYP>");
                    result_xml.Append("<CLIENT_ID>" + CLIENT_ID.Trim() + "</CLIENT_ID>");
                    result_xml.Append("<ORDER_NO>" + CLIENT_ID_ORDER_NO.Trim() + "</ORDER_NO>");
                    result_xml.Append("<INVOICE_NO>" + CLIENT_ID_ORDER_NO.Trim() + "</INVOICE_NO>");
                    result_xml.Append(@"</INVOICE_INB_SEG>");
                    #endregion

                    result_xml.Append(@"</CTRL_SEG>");
                    result_xml.Append("</UC_INVOICE_INB_IFD>");
                    #endregion UC_INVOICE_INB_IFD

                    FileName = PRE_Naming_XML + "_" + sOrder_No + "_" + DateTime.Now.ToString("yyMMddhhmmss", (new System.Globalization.CultureInfo("en-US")));

                    objWriter = new StreamWriter(PATH_Target + "\\" + FileName + ".xml", false);
                    objWriter.Write(result_xml.ToString());
                    objWriter.Close();



                    dr_Mail = ds_mail.Tables["Mail"].NewRow(); //PRE_Naming_XML
                    dr_Mail["LINE_NO"] = nCount;
                    dr_Mail["MESSAGE"] = "Convert Ship confirm success : " + FileName + ".xml";
                    ds_mail.Tables["Mail"].Rows.Add(dr_Mail);
                    #endregion

                    #region Move File
                    string result_Move;
                    result_Move = Path.GetFileName(filepath);
                    string sPath_bak = PATH_BAK + "/" + DateTime.Now.ToString("yyyyMMdd");
                    bool exists = System.IO.Directory.Exists(sPath_bak);
                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(sPath_bak);
                    }
                    System.IO.File.Move(PATH_SOURCE + "/" + result_Move, sPath_bak + "/" + DateTime.Now.ToString("yyyyMMddhhmmss", (new System.Globalization.CultureInfo("en-US"))) + "_" + result_Move);
                    #endregion
                    System.Threading.Thread.Sleep(1000);

                    //string sSubject = PRE_Naming + "_" + sOrder_No;
                    //classmail.Sendmail(MailTo, smtp, "Convert ShipConfirm XML to Text success : " + FileName + ".txt", MailFrom, sSubject);

                    #region send mail success
                    if (ds_mail.Tables["Mail"].Rows.Count != 0)
                    {
                        string sSubject_Mail = strSubject + ": " + result_Move;
                        classmail.Sendmail(MailTo, smtp, string.Empty, MailFrom, sSubject_Mail, ds_mail);
                    }
                    #endregion


                    //string FtpSync_Outbound = System.Configuration.ConfigurationSettings.AppSettings["FtpSync_Outbound"].ToString();
                    //System.Diagnostics.Process pProcess_out = new System.Diagnostics.Process();
                    //pProcess_out.StartInfo.WorkingDirectory = WorkingDirectory;
                    //pProcess_out.StartInfo.FileName = FtpSync_Outbound;
                    //pProcess_out.StartInfo.CreateNoWindow = true; //not diplay a windows
                    //pProcess_out.Start();
                    //pProcess_out.WaitForExit();
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
                classmail.Sendmail(MailTo, smtp, "Error Convert ShipConfirm XML to Text :" + ex.Message.ToString(), MailFrom, strSubject);
                return;
            }
            finally
            {
                this.Dispose();
            }
        }
    }
}
