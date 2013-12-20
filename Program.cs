using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Loyalty.DataAccess;

namespace Loyalty
{
    static class Program
    {
        public delegate void CardDetected_Handler(string cardnum);
        public delegate void CardLost_Handler();

        public static event CardDetected_Handler CardDetected;
        public static event CardLost_Handler CardLost;

        public static string ActiveCard = "";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
                //Application.Run(new frmOperator());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error on start - " + ex.Message);
            }
        }
    }
}
