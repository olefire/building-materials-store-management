using System;
using Npgsql;
using System.Configuration;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Views;

namespace BuildingMaterialsStoreManagement
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var conn = new NpgsqlConnection(connString);
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HomeForm(conn));
        }
    }
} 