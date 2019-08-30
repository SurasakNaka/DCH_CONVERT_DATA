using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace DCH_A2E_RCV_PO
{
    public partial class Form_A2E_RCV_PO : Form
    {
        public Form_A2E_RCV_PO()
        {
            InitializeComponent();
        }

        private void Form_A2E_RCV_PO_Load(object sender, EventArgs e)
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
                System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                pProcess.StartInfo.WorkingDirectory = WorkingDirectory;
                pProcess.StartInfo.FileName = FtpSync_Inbound;
                pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                pProcess.Start();
                pProcess.WaitForExit();

                string INVNUM = string.Empty;
                string PO_Type = string.Empty;
                string INVLIN = string.Empty;
                string PRTNUM = string.Empty;
                string RCVQTY = string.Empty;
                string STKUOM = string.Empty;

                string LOTNUM = string.Empty;
                string TRNDTE = string.Empty;
                string EXPIRE_DTE = string.Empty;
                string SHORTITEM = string.Empty;
                foreach (string filepath in Directory.GetFiles(PATH_SOURCE, sPrefix + "*.xml"))
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(filepath);

                    for (int i = 0; i < ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows.Count; i++)
                    {
                        string sTxt = string.Empty;
                        string sCompany = "00125";
                        if (ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["INVNUM"].ToString() != string.Empty)
                        {
                            INVNUM = ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["INVNUM"].ToString().Substring(7);// PO306768
                        }
                        else
                        {
                            INVNUM = string.Empty;
                        }

                        PO_Type = "OP";

                        if (ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["INVLIN"].ToString() != string.Empty)
                        {
                            INVLIN = ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["INVLIN"].ToString();
                        }
                        else
                        {
                            INVLIN = string.Empty;
                        }

                        if (ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["PRTNUM"].ToString() != string.Empty)
                        {
                            PRTNUM = ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["PRTNUM"].ToString();
                        }
                        else
                        {
                            PRTNUM = string.Empty;
                        }

                        RCVQTY = ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["RCVQTY"].ToString();
                        STKUOM = ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["STKUOM"].ToString();

                        if (ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["LOTNUM"].ToString() != string.Empty)
                        {
                            LOTNUM = ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["LOTNUM"].ToString();
                        }
                        else
                        {
                            LOTNUM = string.Empty;
                        }

                        
                        if (ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["TRNDTE"].ToString() != string.Empty)
                        {
                            TRNDTE = ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["TRNDTE"].ToString(); // format mm/dd/yyyy  2019-07-26T16:37:23 |
                            TRNDTE = TRNDTE.Substring(5, 2) + "/" + TRNDTE.Substring(8, 2) + "/" + TRNDTE.Substring(0, 4);
                        }
                        else
                        {
                            TRNDTE = string.Empty;
                        }

                        if (ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["EXPIRE_DTE"].ToString() != string.Empty)
                        {
                            EXPIRE_DTE = ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["EXPIRE_DTE"].ToString(); // format mm/dd/yyyy  2019-07-26T16:37:23 |
                            EXPIRE_DTE = EXPIRE_DTE.Substring(5, 2) + "/" + EXPIRE_DTE.Substring(8, 2) + "/" + EXPIRE_DTE.Substring(0, 4);
                        }
                        else
                        {
                            EXPIRE_DTE = string.Empty;
                        }

                        //if (ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["SHORTITEM"].ToString() != string.Empty)
                        //{
                        //    SHORTITEM = ds.Tables["INVENTORY_RECEIPT_IFD_SEG"].Rows[i]["SHORTITEM"].ToString();
                        //}
                        //else
                        //{
                        //    SHORTITEM = string.Empty;
                        //}

                        SHORTITEM = string.Empty;

                        sTxt = sCompany.Trim() + "|" + INVNUM.Trim() + "|" + PO_Type.Trim() + "|" + INVLIN.Trim() + "|" + PRTNUM.Trim();
                        sTxt += "|" + RCVQTY.Trim() + "|" + STKUOM.Trim() + "|" + LOTNUM.Trim() + "|" + TRNDTE.Trim() + "|" + EXPIRE_DTE.Trim();
                        sTxt += "|" + SHORTITEM.Trim();
                        result.Append(sTxt);
                        result.AppendLine();
                    }

                    #region Create Text file
                    string PRE_Naming = System.Configuration.ConfigurationSettings.AppSettings["PRE_Naming"].ToString();
                    string FileName = PRE_Naming + "_" + INVNUM + "_" + DateTime.Now.ToString("yyyyMMddhhmmss", (new System.Globalization.CultureInfo("en-US")));
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

                    string sSubject = PRE_Naming + "_" + INVNUM;
                    classmail.Sendmail(MailTo, smtp, "Convert A2E_RCV XML to Text success : " + FileName + ".txt", MailFrom, sSubject);

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
                classmail.Sendmail(MailTo, smtp, "Error Convert A2E_RCV XML to Text :" + ex.Message.ToString(), MailFrom, strSubject);
                return;
            }
            finally
            {
                this.Dispose();
            }
        }
    }
}
