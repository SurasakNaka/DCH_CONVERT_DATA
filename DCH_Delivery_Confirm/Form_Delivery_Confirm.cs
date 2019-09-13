using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace DCH_Delivery_Confirm
{
    public partial class Form_Delivery_Confirm : Form
    {
        public Form_Delivery_Confirm()
        {
            InitializeComponent();
        }

        private void Form_Delivery_Confirm_Load(object sender, EventArgs e)
        {
            string PATH_SOURCE = System.Configuration.ConfigurationSettings.AppSettings["PATH_SOURCE"].ToString();
            string PATH_BAK = System.Configuration.ConfigurationSettings.AppSettings["PATH_BAK"].ToString();
            string PATH_Target = System.Configuration.ConfigurationSettings.AppSettings["PATH_TARGET"].ToString();
            string sPrefix = System.Configuration.ConfigurationSettings.AppSettings["PRE_FIX"].ToString();
            StringBuilder result = new StringBuilder();
            string sOrder_No = string.Empty;

            string MailFrom = System.Configuration.ConfigurationSettings.AppSettings["MailFrom"].ToString();
            string MailTo = System.Configuration.ConfigurationSettings.AppSettings["MailTo"].ToString();
            string smtp = System.Configuration.ConfigurationSettings.AppSettings["SMTP"].ToString();
            string strSubject = System.Configuration.ConfigurationSettings.AppSettings["sSubjectmail"].ToString();

            string FtpSync_Inbound = System.Configuration.ConfigurationSettings.AppSettings["FtpSync_Inbound"].ToString();
            string WorkingDirectory = System.Configuration.ConfigurationSettings.AppSettings["WorkingDirectory"].ToString();

            ClassLibrarySendMail.ClassLibrarySendMail classmail = new ClassLibrarySendMail.ClassLibrarySendMail();
            try
            {
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

                System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                pProcess.StartInfo.WorkingDirectory = WorkingDirectory;
                pProcess.StartInfo.FileName = FtpSync_Inbound;
                pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                pProcess.Start();
                pProcess.WaitForExit();

                foreach (string filepath in Directory.GetFiles(PATH_SOURCE, sPrefix + "*.xml"))
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(filepath);
                    result = new StringBuilder();
                    Order_No = string.Empty;
                    sOrder_No = string.Empty;
                    for (int i = 0; i < ds.Tables["SHIPMENT_LINE_SEG"].Rows.Count; i++)
                    {
                        string sTxt = string.Empty;
                        string sCompany = System.Configuration.ConfigurationSettings.AppSettings["sCompany"].ToString();
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



                        sTxt = sCompany.Trim() + "~" + Order_No.Trim() + "~" + Order_Type.Trim() + "~" + Line_Number.Trim() + "~" + PRTNUM.Trim();
                        sTxt += "~" + sLocation.Trim() + "~" + LOTNUM.Trim() + "~" + COMQTY.Trim() + "~" + STKUOM.Trim() + "~" + Carrie_Number.Trim();
                        sTxt += "~" + Value_ignored.Trim() + "~" + TRNDTE.Trim();
                        result.Append(sTxt);
                        result.AppendLine();
                    }

                    #region Create Text file
                    string PRE_Naming = System.Configuration.ConfigurationSettings.AppSettings["PRE_Naming"].ToString();
                    string FileName = PRE_Naming + "_" + sOrder_No + "_" + DateTime.Now.ToString("yyyyMMddhhmmss", (new System.Globalization.CultureInfo("en-US")));
                    StreamWriter objWriter = new StreamWriter(PATH_Target + @"\\" + FileName + ".txt", false);
                    objWriter.Write(result.ToString());
                    objWriter.Close();

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

                    string sSubject = PRE_Naming + "_" + sOrder_No;
                    classmail.Sendmail(MailTo, smtp, "Convert DCHDeliveryConfirm XML to Text success : " + FileName + ".txt", MailFrom, sSubject);

                    string FtpSync_Outbound = System.Configuration.ConfigurationSettings.AppSettings["FtpSync_Outbound"].ToString();
                    System.Diagnostics.Process pProcess_out = new System.Diagnostics.Process();
                    pProcess_out.StartInfo.WorkingDirectory = WorkingDirectory;
                    pProcess_out.StartInfo.FileName = FtpSync_Outbound;
                    pProcess_out.StartInfo.CreateNoWindow = true; //not diplay a windows
                    pProcess_out.Start();
                    pProcess_out.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                classmail.Sendmail(MailTo, smtp, "Error Convert DCHDeliveryConfirm XML to Text :" + ex.Message.ToString(), MailFrom, strSubject);
                return;
            }
            finally
            {
                this.Dispose();
            }
        }
    }
}
