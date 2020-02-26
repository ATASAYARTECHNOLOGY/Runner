using System;
using System.Collections.Generic;
//using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Runner
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class run : System.Web.Services.WebService
    {

        [WebMethod]
        public string satinalmaIrs_(string irsaliyetarih, string FICHENO, object saat,
          DateTime textIrsaliye, string[] listMiktar, string[] lotName, string[] lotCode, string[] stokCode, string Carikodu)
        {
          return  satinalmaIrs.satinalmaContent(irsaliyetarih, FICHENO, saat, textIrsaliye, listMiktar, lotName, lotCode, stokCode, Carikodu);
        }

        [WebMethod]
        public string Sarffisi(DateTime Tarih, string kasnak, int Adet, string isIstasyonu)
        {
            return Sarf.sarfFisiOlusturEkle(Tarih, kasnak, Adet, isIstasyonu);
        }
        [WebMethod]
        public string UretimFisi(DateTime Tarih, string kasnak, int Adet, string isIstasyonu,string LineExp)
        {
            return Uretim.uretimFisiOlusturEkle(Tarih, kasnak, Adet, isIstasyonu,LineExp);
        }

        [WebMethod]
        public string AmbarAktarim(string FICHENO, List<string> SOURCE_MT_REFERENCE,
        List<string> SOURCE_SLT_REFERENCE, List<string> DATE, List<string> lottanCikilacakMiktar, string malzemeKodu, string souceWh, string desWh,
        string iadeMiktar, string labelIadeLotno)
        {
            return Ambarfisi.AmbarTransferFisi(FICHENO, SOURCE_MT_REFERENCE,SOURCE_SLT_REFERENCE, DATE, lottanCikilacakMiktar, malzemeKodu, souceWh, desWh,
         iadeMiktar, labelIadeLotno);
        }

        [WebMethod]
        public string hi()
        {
            return "hi";
        }

        [WebMethod]
        public string Try(string fNo)
        {
            UnityKeeper.Unitylogin();
                UnityObjects.IUnityApplication UnityAppTutucu =UnityKeeper.UnityApp;


            UnityObjects.Data items = UnityAppTutucu.NewDataObject(UnityObjects.DataObjectType.doMaterialSlip);
            items.New();
            items.DataFields.FieldByName("GROUP").Value = 3;
            items.DataFields.FieldByName("TYPE").Value = 13;
            items.DataFields.FieldByName("NUMBER").Value = fNo;
            items.DataFields.FieldByName("DATE").Value = "25.12.2019";
            items.DataFields.FieldByName("TIME").Value = 271718400;
            items.DataFields.FieldByName("AUXIL_CODE").Value = "OPERP";
            //items.DataFields.FieldByName("SOURCE_WSCODE").Value = 001;
            items.DataFields.FieldByName("CREATED_BY").Value = 1;
            items.DataFields.FieldByName("DATE_CREATED").Value = "24.12.2019";
            items.DataFields.FieldByName("HOUR_CREATED").Value = 16;
            items.DataFields.FieldByName("MIN_CREATED").Value = 50;
            items.DataFields.FieldByName("SEC_CREATED").Value = 40;
            items.DataFields.FieldByName("MODIFIED_BY").Value = 1;
            items.DataFields.FieldByName("DATE_MODIFIED").Value = "25.12.2019";
            items.DataFields.FieldByName("HOUR_MODIFIED").Value = 17;
            items.DataFields.FieldByName("MIN_MODIFIED").Value = 49;
            items.DataFields.FieldByName("SEC_MODIFIED").Value = 24;
            items.DataFields.FieldByName("DATA_REFERENCE").Value = 50;

            UnityObjects.Lines transactions_lines = items.DataFields.FieldByName("TRANSACTIONS").Lines;
            transactions_lines.AppendLine();
            transactions_lines[transactions_lines.Count - 1].FieldByName("ITEM_CODE").Value = "PT0950603000PSYG460654";
            transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_TYPE").Value = 0;
            transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_NUMBER").Value = 1;
            transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = 3;
            transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = "ADET";
            transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = 1;
            transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = 1;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DATA_REFERENCE").Value = 76;
            transactions_lines[transactions_lines.Count - 1].FieldByName("EU_VAT_STATUS").Value = 4;
            transactions_lines[transactions_lines.Count - 1].FieldByName("EDT_CURR").Value = 1;
            items.DataFields.FieldByName("DOC_DATE").Value = "24.12.2019";
            items.DataFields.FieldByName("DOC_TIME").Value = 271722496;
            string msg;

            if (items.Post() == true)
            {
                msg = ("POST OK !");
            }
            else
            {
                if (items.ErrorCode != 0)
                {
                    msg = ("DBError(" + items.ErrorCode.ToString() + ")-" + items.ErrorDesc + items.DBErrorDesc);
                }
                else //if (items.ValidateErrors.Count > 0)
                {
                    string result = "XML ErrorList:";
                    for (int i = 0; i < items.ValidateErrors.Count; i++)
                    {
                        result += "(" + items.ValidateErrors[i].ID.ToString() + ") - " + items.ValidateErrors[i].Error;
                    }
                    msg = (result);
                }
            }
            return msg;
        }
    }
}
