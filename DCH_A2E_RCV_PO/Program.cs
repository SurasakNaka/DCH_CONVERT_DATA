using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DCH_A2E_RCV_PO
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form_A2E_RCV_PO());
        }
    }
}
