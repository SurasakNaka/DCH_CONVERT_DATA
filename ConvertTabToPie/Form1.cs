using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ConvertTabToPie
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public DataTable ConvertToDataTable(string fileName)
        {
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
          
                var lines = File.ReadAllLines(fileName, System.Text.Encoding.UTF8);
                // reading rest of the data
                for (int i = 0; i < lines.Count(); i++)
                {
                    DataRow dr = dt.NewRow();
                    string[] values = lines[i].Split(new char[] { '\t' });

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
                }

                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable ConvertToDataTable(string fileName, out DataTable dt_no)
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
                var lines = File.ReadAllLines(fileName, System.Text.Encoding.UTF8);
                // reading rest of the data
                for (int i = 0; i < lines.Count(); i++)
                {
                    DataRow dr = dt.NewRow();
                    string[] values = lines[i].Split(new char[] { '\t' });

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
        int k, j = 0;
        StringBuilder result = new StringBuilder();

        private void Button1_Click(object sender, EventArgs e)
        {
            string PATH_SOURCE = System.Configuration.ConfigurationSettings.AppSettings["PATH_SOURCE"].ToString();
            string sPrefix = System.Configuration.ConfigurationSettings.AppSettings["PRE_FIX"].ToString();
            string Temp = "";
            if (radioButton1.Checked)
            {
                Temp = "*.txt";
            }
            else
            {
                Temp = "*.xml";
            }

            try
            {
                foreach (string filepath in Directory.GetFiles(PATH_SOURCE, Temp))
                {
                    DataTable dt = new DataTable();
                    DataTable dt_no = new DataTable();
                    if (radioButton1.Checked) // Tab To |
                    {
                        dt = ConvertToDataTable(filepath);

                        if (dt.Rows.Count != 0)
                        {
                            #region Detail
                            foreach (DataRow row in dt.Rows)
                            {
                                object[] array = row.ItemArray;

                                for (k = 0; k < array.Length - 1; k++)
                                {
                                    result.Append(array[k].ToString().Trim() + "|");
                                }
                                result.AppendLine(array[k].ToString().Trim());
                                //result.AppendLine();
                            }
                            #endregion

                            #region Create Text file
                            
                            string FileName = sPrefix + "_" + DateTime.Now.ToString("yyyyMMddhhmmss", (new System.Globalization.CultureInfo("en-US")));
                            StreamWriter objWriter = new StreamWriter(PATH_SOURCE + @"\\" + FileName + ".txt", false);
                            objWriter.Write(result.ToString());
                            objWriter.Close();
                            System.Threading.Thread.Sleep(1000);
                            #endregion
                        }
                    }
                    else // XML To Text
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(filepath);

                        for (int i = 0; i < ds.Tables["SHIPMENT_LINE_SEG"].Rows.Count; i++)
                        {
                            string sCompany = "00125";
                            string Order_No = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["ORDNUM"].ToString();// PAHC-SO150789-01 --> 150789-01
                            string Order_Type = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["ORDNUM"].ToString(); // PAHC-SO150789-01 --> SO
                            string Line_Number = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["ORDLIN"].ToString(); // 1.001 --> 1.001000
                            string PRTNUM = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["PRTNUM"].ToString();
                            string sLocation = "";
                            string LOTNUM = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["LOTNUM"].ToString();
                            string COMQTY = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["COMQTY"].ToString();
                            string STKUOM = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["STKUOM"].ToString();
                            string Carrie_Number = "";
                            string Value_ignored = "";
                            string TRNDTE = ds.Tables["SHIPMENT_LINE_SEG"].Rows[i]["TRNDTE"].ToString(); // format mm/dd/yyyy
                            string Carrie_Number2 = "";
                        }
                    }
                }

                MessageBox.Show("OK");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
