﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DCH_CONVERT_DATA_SHIPCONFIRM
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
            Application.Run(new Form_SHIPCONFIRM());
        }
    }
}
