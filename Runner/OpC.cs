using System;
using System.Data.SqlClient;
using System.Data;
//using OPerp.Formlar;
using System.Globalization;
using System.Diagnostics;
using System.Configuration;


//using System.Configuration.ConfigurationSettings;
namespace Runner
{
    class OpC
    {
        //public static terminalGiris terminal;
        //public static string kullanici;
        //public static string yetki;
        //public static Giris mainFrom;
        //public static string PrintLotno;
        //public static Giris personLoginForm;
        //public static Malkabul malKabulFormu;


        string configvalue1 = ConfigurationManager.AppSettings["countoffiles"];
        string configvalue2 = ConfigurationManager.AppSettings["logfilelocation"];


        //public static UnityObjects.IUnityApplication UnityApp;

        //public static string isIstasyonu = Properties.Settings.Default.isistasyonu;

        public static int BarkodKayitID = 0;

        public static string suan = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        //string deneme = System.Configuration.ConfigurationSettings.AppSettings.Keys[1];// AppSettings.Item(“HostName“);

        public static string firmaDonem = "LG_" + ConfigurationManager.AppSettings["logoFirm"] + "_" + ConfigurationManager.AppSettings["logoDonem"] + "_";
        public static string firma = "LG_" + ConfigurationManager.AppSettings["logoFirm"] + "_";

        public static string ataFirma = "Ata_" + ConfigurationManager.AppSettings["logoFirm"] + "_" + ConfigurationManager.AppSettings["logoDonem"] + "_";
        public static string ataFirmaDonemsiz = "Ata_" + ConfigurationManager.AppSettings["logoFirm"] + "_";

        //public static SqlConnection conn = new SqlConnection("Server=" + Properties.Settings.Default.serverAdress +
        //    ";Database=" + Properties.Settings.Default.serverName + ";User Id=" + Properties.Settings.Default.serverID +
        //    ";Password=" + Properties.Settings.Default.serverPassword + ";");
        public static SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString);


        //public static SqlConnection connLog = new SqlConnection("Server=" + Properties.Settings.Default.serverAdress +
        //";Database=operpLogdb;User Id=" + Properties.Settings.Default.serverID +
        //";Password=" + Properties.Settings.Default.serverPassword + ";");


        //public static void objekill()
        //{
        //    try
        //    {
        //        OpC.UnityApp = null;
        //    var kapatilacakIslem = "LOBJECTS";
        //    Process[] processler = Process.GetProcessesByName(kapatilacakIslem.ToString());

        //    foreach (Process process in processler)
        //    {
        //        //Debug.WriteLine("İşlem sonlandırıldı: " + process.ProcessName);
        //        process.Kill();
        //    }

        //    }
        //    catch (Exception)
        //    {

        //    }
        //}
        public int bukim()
        {
            try
            {
                string cmd = "select id from " + OpC.ataFirma + "Person where personName = '" + Sifrele("0"/*Properties.Settings.Default.Hat*/) + "'";
                DataTable dt = new DataTable();
                dt = adapter(cmd);
                int deger = Convert.ToInt32(dt.Rows[0][0].ToString());

                return deger;

            }
            catch (Exception)
            {
                return 0;
            }
        }


        public bool command(string cmd)
        {
            bool deger = true;
            try
            {
                SqlCommand cemde = new SqlCommand(cmd, conn);
                if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
                cemde.ExecuteNonQuery();
                if (conn.State != System.Data.ConnectionState.Closed) conn.Close();
            }
            catch (Exception )
            {
                deger = false;
                //System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            return deger;
        }
        public bool dBaseVarmi(string cmd)
        {
            bool deger = true;

            DataTable dt = new DataTable();

            dt = adapter(cmd);

            try
            {
                if (dt.Rows.Count != 0) deger = true;
                else deger = false;
            }
            catch (Exception) { return false; }

            return deger;
        }
        /*public static SqlCommand compar/*= */
        //new SqlCommand()*/;
        public bool commandparam(string cmd, SqlCommand compar)
        {
            bool deger = true;
            //try
            //{
            //     compar = new SqlCommand(cmd, conn);
            if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
            compar.ExecuteNonQuery();
            if (conn.State != System.Data.ConnectionState.Closed) conn.Close();
            //}
            //catch (Exception ex )
            //{
            //    deger = false;
            //    System.Windows.Forms.MessageBox.Show(ex.Message);
            //}
            return deger;
        }
        public DataTable adapter(string cmd)
        {
            DataTable dt = new DataTable();
            //try
            //{
            SqlDataAdapter sda = new SqlDataAdapter(cmd, conn);
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
                sda.Fill(dt);
                if (conn.State != System.Data.ConnectionState.Closed) conn.Close();

            }
            catch (Exception )
            {
                //System.Windows.Forms.MessageBox.Show("Logoya baglanılamadı.\r\n"+ex);
            }
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show(ex.Message);
            //}
            return dt;
        }

        public string guiduret()
        {
            string GuidKey = Guid.NewGuid().ToString();
            return GuidKey;

        }


        public string Sifrele(string data)
        {
            byte[] tempDizi = System.Text.ASCIIEncoding.ASCII.GetBytes(data);// şifrelenecek veri byte dizisine çevrilir
            string finalData = System.Convert.ToBase64String(tempDizi);//Base64 ile şifrelenir
            return finalData;
        }

        public string SifreCoz(string cozulecekdata)
        {
            byte[] tempDizi = Convert.FromBase64String(cozulecekdata);
            string CozulecekVeri = System.Text.ASCIIEncoding.ASCII.GetString(tempDizi);
            return CozulecekVeri;
        }


        public static bool isNumeric(string value)
        {
            try
            {
                Convert.ToInt32(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string Vardiye;
        public void shiftControl(DateTime tarih)
        {
            DateTime date = tarih;
            //8.05.2019 13:53:22

            DateTime baslangicA = DateTime.ParseExact("23:59:59", "HH:mm:ss", CultureInfo.CurrentCulture);
            DateTime bitisA = DateTime.ParseExact("08:00:00", "HH:mm:ss", CultureInfo.CurrentCulture);

            DateTime baslangicB = DateTime.ParseExact("08:00:00", "HH:mm:ss", CultureInfo.CurrentCulture);
            DateTime bitisB = DateTime.ParseExact("16:00:00", "HH:mm:ss", CultureInfo.CurrentCulture);

            DateTime baslangicC = DateTime.ParseExact("16:00:00", "HH:mm:ss", CultureInfo.CurrentCulture);
            DateTime bitisC = DateTime.ParseExact("23:59:59", "HH:mm:ss", CultureInfo.CurrentCulture);

            if (date > baslangicC && date < bitisC)
            {
                Vardiye = "C";
            }
            else if (date > baslangicB && date < bitisB)
            {
                Vardiye = "B";
            }
            else
            {
                Vardiye = "A";
            }

        }


        public static void unityStfiche(string FICHENO, string clientRef, string amount, DateTime Date_, string unit_CODE, string items_CODE)
        {




        }

    }

    public class ComboboxItem
    {

        public string Text { get; set; }
        public object Value { get; set; }
        public object Logi { get; set; }

        public override string ToString()
        {
            return Text;
        }

    }
}



