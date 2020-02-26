
using Runner;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
//using System.Windows.Forms;

namespace Runner
{
    class toLogo
    {
    }
    class Uretim
    {
        public static string uretimFisiOlusturEkle(DateTime secilenTarih, string malzemeKoduITEM_CODE, int Adet, string isIstasyonu,string LineExp)
        {

            UnityKeeper.Unitylogin();
            UnityObjects.IUnityApplication UnityAppTutucu = UnityKeeper.UnityApp;

            OpC cop = new OpC();


            string birimSeti;
            string unitRef;
            string fisNumarasi;
            string FICHENO = Kontrol.fisNoBelirle(secilenTarih, "URT", isIstasyonu);
            string yeniFis = "";
            string yeniFisSatir = "";
            unitRef = Kontrol.degerAra("UNITSETREF", "ITEMS", "CODE", "='" + malzemeKoduITEM_CODE + "'", OpC.firma);

            if (unitRef == null) return "Girmiş oldugunuz malzeme bulunamadı.";
            else
            {
                birimSeti = Kontrol.degerAra("CODE", "UNITSETL", "UNITSETREF", "='" + unitRef + "' AND MAINUNIT = 1", OpC.firma);
                if (birimSeti == null) return malzemeKoduITEM_CODE + " Malzemesine ait birim seti bulunamadı.";
                else
                {
                   fisNumarasi = Kontrol.degerAra("LOGICALREF", "STFICHE", "FICHENO", "='" + FICHENO + "'", OpC.firmaDonem);

                   if (fisNumarasi == null)
                       yeniFisSatir = yeniUretimFisiOlusturVeSatırEkle(UnityAppTutucu, secilenTarih, malzemeKoduITEM_CODE, Adet, FICHENO, birimSeti, isIstasyonu,LineExp);

                   else
                        yeniFis = uretimFisiSatirOlustur(UnityAppTutucu, malzemeKoduITEM_CODE, Adet, birimSeti, Convert.ToInt16(fisNumarasi),LineExp);
                }
            }
            if (yeniFis == "ok" || yeniFisSatir == "ok")
                return "ok";
            else
                return yeniFis + yeniFisSatir;
        }

        private static string yeniUretimFisiOlusturVeSatırEkle(UnityObjects.IUnityApplication UnityApp, DateTime secilenTarih,
            string malzemeKoduITEM_CODE, int Adet, string FICHENO, string birimSeti, string isIstasyonu, string LineExp)
        {


            object saat = 0;
            int dk = Convert.ToInt16(secilenTarih.ToString("mm"));
            int saati = Convert.ToInt16(secilenTarih.ToString("HH"));
            int saniye = Convert.ToInt16(secilenTarih.ToString("ss"));
            UnityApp.PackTime(saati, dk, saniye, ref saat);
            object saatsuan = 0;
            int dksuan = Convert.ToInt16(DateTime.Now.ToString("mm"));
            int saatisuan = Convert.ToInt16(DateTime.Now.ToString("HH"));
            int saniyesuan = Convert.ToInt16(DateTime.Now.ToString("ss"));
            UnityApp.PackTime(saatisuan, dksuan, saniyesuan, ref saatsuan);

            UnityObjects.Data items = UnityApp.NewDataObject(UnityObjects.DataObjectType.doMaterialSlip);
            items.New();
            items.DataFields.FieldByName("GROUP").Value = 3;
            items.DataFields.FieldByName("TYPE").Value = 13;
            items.DataFields.FieldByName("NUMBER").Value = FICHENO;
            items.DataFields.FieldByName("AUXIL_CODE").Value = "OPERP";
            items.DataFields.FieldByName("DATE").Value = secilenTarih.ToString("dd.MM.yyyy");
            items.DataFields.FieldByName("TIME").Value = saat;
            items.DataFields.FieldByName("SOURCE_WSCODE").Value = isIstasyonu;
            items.DataFields.FieldByName("CREATED_BY").Value = 1;
            items.DataFields.FieldByName("DATE_CREATED").Value = secilenTarih.ToString("dd.MM.yyyy");
            items.DataFields.FieldByName("HOUR_CREATED").Value = DateTime.Now.ToString("HH"); ;
            items.DataFields.FieldByName("MIN_CREATED").Value = DateTime.Now.ToString("mm"); ;
            items.DataFields.FieldByName("SEC_CREATED").Value = DateTime.Now.ToString("ss"); ;


            //Stline Insert
            UnityObjects.Lines transactions_lines = items.DataFields.FieldByName("TRANSACTIONS").Lines;
            transactions_lines.AppendLine();
            transactions_lines[transactions_lines.Count - 1].FieldByName("ITEM_CODE").Value = malzemeKoduITEM_CODE;
            transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_TYPE").Value = 0;
            transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_NUMBER").Value = "~";
            transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = Adet;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DESCRIPTION").Value = LineExp;
            transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = birimSeti;
            items.DataFields.FieldByName("DOC_DATE").Value = DateTime.Now.ToString("dd.MM.yyyy"); ;
            items.DataFields.FieldByName("DOC_TIME").Value = saatsuan;

            return UnityKeeper.UnitySendPost(items);
        }

        private static string uretimFisiSatirOlustur(UnityObjects.IUnityApplication UnityApp, string malzemeKoduITEM_CODE,
            int Adet, string birimSeti, int fichelogi,string LineExp)
        {
            if (Kontrol.satirVarmiKontrolEtVarsaEkle(fichelogi, malzemeKoduITEM_CODE, Adet, LineExp))
            {
                UnityObjects.Data uretimFisi = UnityApp.NewDataObject(UnityObjects.DataObjectType.doMaterialSlip);
                uretimFisi.Read(fichelogi);
                UnityObjects.Lines transactions_lines = null;

                transactions_lines = uretimFisi.DataFields.FieldByName("TRANSACTIONS").Lines;
                transactions_lines.AppendLine();

                transactions_lines[transactions_lines.Count - 1].FieldByName("ITEM_CODE").Value = malzemeKoduITEM_CODE;
                transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_TYPE").Value = 0;
                transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_NUMBER").Value = "~";
                transactions_lines[transactions_lines.Count - 1].FieldByName("DESCRIPTION").Value = LineExp;
                transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = Adet;
                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = birimSeti;
                return UnityKeeper.UnitySendPost(uretimFisi);
            }
            return "ok";
        }
    }
    class Kontrol
    {
        //public static UnityObjects.IUnityApplication unitylogin()
        //{
        //    //UnityObjects.IUnityApplication UnityApp = new UnityObjects.UnityApplication();
        //    //string ladi = Properties.Settings.Default.LOGOADI;
        //    //string lsifre = Properties.Settings.Default.LOGOSIFRE;
        //    //string lfirma = Properties.Settings.Default.firmaKod;
        //    //string ldonem = Properties.Settings.Default.donemKodu;
        //    //if (UnityApp.Login(ladi, lsifre, Convert.ToInt32(lfirma), Convert.ToInt32(ldonem))) { }
        //    //else { MessageBox.Show("Logoya baglanılamadı."); }

        //    //return UnityApp;
        //}

        public static string degerAra(string hedefColumn, string aranacakTablo, string aranacakColumn, string aranacakDeger, string firma)
        {
            OpC cop = new OpC();
            string cmdString = "select  " + hedefColumn + " from " + firma + aranacakTablo + " where " + aranacakColumn + " " + aranacakDeger + "";
            DataTable DT = cop.adapter(cmdString);
            try
            {
                string nullTry = DT.Rows[0][0].ToString();
                return nullTry;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string fisNoBelirle(DateTime secilenTarih, string fisTuru, string isIstasyonu)
        {
            OpC cop = new OpC();
            cop.shiftControl(secilenTarih);
            string Vardiye = OpC.Vardiye;
            string Zaman = secilenTarih.ToString("dd/MM");

            string FICHENO = isIstasyonu + "-" + fisTuru + "-" + Vardiye + "-" + Zaman;//"UGRS" + LOGI;
            return FICHENO;
        }

        public static bool satirVarmiKontrolEtVarsaEkle(int Ficheno, string malzemeKod, int adet,string LineExp)
        {
            OpC cop = new OpC();
            string malzemeLogi = degerAra("LOGICALREF", "ITEMS", "CODE", "='" + malzemeKod + "'", OpC.firma);
            string stlineLogi = degerAra("LOGICALREF", "STLINE", "STFICHEREF", "=" + Ficheno + " and STOCKREF =" + malzemeLogi, OpC.firmaDonem+" and LINEEXP='"+ LineExp + "'");
            if (stlineLogi == null)
                return true;
            else
                cop.command("update " + OpC.firmaDonem + "STLINE set AMOUNT = AMOUNT +" + adet + " where LOGICALREF = " + stlineLogi + "");
            return false;
        }
    }
    class Sarf
    {
        public static string sarfFisiOlusturEkle(DateTime secilenTarih,string malzemeKoduITEM_CODE, int Adet, string isIstasyonu,string LineExp="")
        {
            UnityKeeper.Unitylogin();
            UnityObjects.IUnityApplication UnityAppTutucu = UnityKeeper.UnityApp;

            OpC cop = new OpC();


            string birimSeti;
            string unitRef;
            string fisNumarasi;
            string FICHENO = Kontrol.fisNoBelirle(secilenTarih, "SRF", isIstasyonu);
            string yeniFis = "";
            string yeniFisSatir = "";
            unitRef = Kontrol.degerAra("UNITSETREF", "ITEMS", "CODE", "='" + malzemeKoduITEM_CODE + "'", OpC.firma);



            if (unitRef == null) return "Girmiş oldugunuz malzeme bulunamadı.";
            else
            {

                birimSeti = Kontrol.degerAra("CODE", "UNITSETL", "UNITSETREF", "='" + unitRef + "' AND MAINUNIT = 1", OpC.firma);

                if (birimSeti == null) return malzemeKoduITEM_CODE + " Malzemesine ait birim seti bulunamadı.";
                else
                {
                    fisNumarasi = Kontrol.degerAra("LOGICALREF", "STFICHE", "FICHENO", "='" + FICHENO + "'", OpC.firmaDonem);
                    if (fisNumarasi == null)

                        yeniFis = yeniSarfFisiOlusturveSatirekle(UnityAppTutucu,secilenTarih, malzemeKoduITEM_CODE, Adet, FICHENO, birimSeti, isIstasyonu);
                    else
                        yeniFisSatir = sarfFisiSatirOlustur(UnityAppTutucu,malzemeKoduITEM_CODE, Adet, birimSeti, Convert.ToInt16(fisNumarasi),LineExp);


                }

            }
            if (yeniFis == "ok" || yeniFisSatir == "ok")
                return "ok";
            else
                return yeniFis + yeniFisSatir;
        }

        private static string yeniSarfFisiOlusturveSatirekle(UnityObjects.IUnityApplication UnityApp,DateTime secilenTarih,
                string malzemeKoduITEM_CODE, int Adet, string FICHENO, string birimSeti, string isIstasyonu)
        {
            object saat = 0;
            int dk = Convert.ToInt16(secilenTarih.ToString("mm"));
            int saati = Convert.ToInt16(secilenTarih.ToString("HH"));
            int saniye = Convert.ToInt16(secilenTarih.ToString("ss"));
            UnityKeeper.UnityApp.PackTime(saati, dk, saniye, ref saat);

            object saatsuan = 0;
            int dksuan = Convert.ToInt16(DateTime.Now.ToString("mm"));
            int saatisuan = Convert.ToInt16(DateTime.Now.ToString("HH"));
            int saniyesuan = Convert.ToInt16(DateTime.Now.ToString("ss"));
            UnityKeeper.UnityApp.PackTime(saatisuan, dksuan, saniyesuan, ref saatsuan);

            UnityObjects.Data items = UnityKeeper.UnityApp.NewDataObject(UnityObjects.DataObjectType.doMaterialSlip);
            items.New();
            items.DataFields.FieldByName("GROUP").Value = 3;
            items.DataFields.FieldByName("TYPE").Value = 12;
            items.DataFields.FieldByName("NUMBER").Value = FICHENO;
            items.DataFields.FieldByName("DATE").Value = secilenTarih.ToString("dd.MM.yyyy");
            items.DataFields.FieldByName("TIME").Value = saat;
            items.DataFields.FieldByName("AUXIL_CODE").Value = "OPERP";
            items.DataFields.FieldByName("CREATED_BY").Value = 1;
            items.DataFields.FieldByName("SOURCE_WSCODE").Value = isIstasyonu;
            items.DataFields.FieldByName("DATE_CREATED").Value = secilenTarih.ToString("dd.MM.yyyy");
            items.DataFields.FieldByName("HOUR_CREATED").Value = DateTime.Now.ToString("HH"); ;
            items.DataFields.FieldByName("MIN_CREATED").Value = DateTime.Now.ToString("mm"); ;
            items.DataFields.FieldByName("SEC_CREATED").Value = DateTime.Now.ToString("ss"); ;

            UnityObjects.Lines transactions_lines = items.DataFields.FieldByName("TRANSACTIONS").Lines;
            transactions_lines.AppendLine();
            transactions_lines[transactions_lines.Count - 1].FieldByName("ITEM_CODE").Value = malzemeKoduITEM_CODE;
            transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_TYPE").Value = 0;
            transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_NUMBER").Value = "~";
            transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = Adet;
            transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = birimSeti;
            items.DataFields.FieldByName("DOC_DATE").Value = DateTime.Now.ToString("dd.MM.yyyy"); ;
            items.DataFields.FieldByName("DOC_TIME").Value = saatsuan;
            return UnityKeeper.UnitySendPost(items);

        }

        private static string sarfFisiSatirOlustur(UnityObjects.IUnityApplication UnityApp,string malzemeKoduITEM_CODE,
            int Adet, string birimSeti, int fichelogi,string LineExp)
        {
            if (Kontrol.satirVarmiKontrolEtVarsaEkle(fichelogi, malzemeKoduITEM_CODE, Adet,LineExp))
            {
                UnityObjects.Data uretimFisi = UnityKeeper.UnityApp.NewDataObject(UnityObjects.DataObjectType.doMaterialSlip);

                uretimFisi.Read(fichelogi);
                UnityObjects.Lines transactions_lines = null;


                transactions_lines = uretimFisi.DataFields.FieldByName("TRANSACTIONS").Lines;
                transactions_lines.AppendLine();

                transactions_lines[transactions_lines.Count - 1].FieldByName("ITEM_CODE").Value = malzemeKoduITEM_CODE;
                transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_TYPE").Value = 0;
                transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_NUMBER").Value = "~";
                transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = Adet;
                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = birimSeti;
                return UnityKeeper.UnitySendPost(uretimFisi);
            }
            return "ok";
        }
    }
    class SiparisIrsaliyesi
    {
        public static string SiparisIrsaliyesiOlustur(DataTable dtSevkView, string siparisNo, string cariKodu)
        {
            string clientRef = dtSevkView.Rows[0]["CLIENTREF"].ToString();

            string ficheNo = siparisNoBelirle(siparisNo);


            return yeniSiparisEkle(ficheNo, cariKodu, dtSevkView);


        }

        private static string yeniSiparisEkle(string ficheNo, string cariKodu, DataTable dtSevkView)
        {
            UnityKeeper.Unitylogin();
            UnityObjects.IUnityApplication UnityAppTutucu = UnityKeeper.UnityApp;



            object saatsuan = 0;
            int dksuan = Convert.ToInt16(DateTime.Now.ToString("mm"));
            int saatisuan = Convert.ToInt16(DateTime.Now.ToString("HH"));
            int saniyesuan = Convert.ToInt16(DateTime.Now.ToString("ss"));
            UnityAppTutucu.PackTime(saatisuan, dksuan, saniyesuan, ref saatsuan);

            //Baslıklar
            UnityObjects.Data salesdis = UnityAppTutucu.NewDataObject(UnityObjects.DataObjectType.doSalesDispatch);
            salesdis.New();
            salesdis.DataFields.FieldByName("TYPE").Value = 8;
            salesdis.DataFields.FieldByName("NUMBER").Value = ficheNo;
            salesdis.DataFields.FieldByName("DATE").Value = DateTime.Now.ToString("dd.MM.yyyy");
            salesdis.DataFields.FieldByName("TIME").Value = saatisuan;
            salesdis.DataFields.FieldByName("ARP_CODE").Value = cariKodu;

            salesdis.DataFields.FieldByName("CREATED_BY").Value = 1;
            salesdis.DataFields.FieldByName("DATE_CREATED").Value = DateTime.Now.ToString("dd.MM.yyyy");
            salesdis.DataFields.FieldByName("HOUR_CREATED").Value = saatisuan;
            salesdis.DataFields.FieldByName("MIN_CREATED").Value = dksuan;
            salesdis.DataFields.FieldByName("SEC_CREATED").Value = saniyesuan;

            foreach (DataRow dtSatir in dtSevkView.Rows)
            {
                //Satirlar
                string unitRef = Kontrol.degerAra("UNITSETREF", "ITEMS", "LOGICALREF", "=" + dtSatir["STOCKREF"], OpC.firma);

                string birimSeti = Kontrol.degerAra("CODE", "UNITSETL", "UNITSETREF", "='" + unitRef + "' AND MAINUNIT = 1", OpC.firma);

                UnityObjects.Lines transactions_lines = salesdis.DataFields.FieldByName("TRANSACTIONS").Lines;
                transactions_lines.AppendLine();
                transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value = dtSatir["CODE"];
                transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_DEF").Value = dtSatir["CODE"];
                transactions_lines[transactions_lines.Count - 1].FieldByName("ORDER_REFERENCE").Value = dtSatir["LOGICALREF"];
                transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = dtSatir["guncelCikilan"];
                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = birimSeti;
                transactions_lines[transactions_lines.Count - 1].FieldByName("MONTH").Value = DateTime.Now.ToString("MM");
                transactions_lines[transactions_lines.Count - 1].FieldByName("YEAR").Value = DateTime.Now.ToString("yyyy");

                transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 0;

            }//FOREACH BİTİŞ

            return UnityKeeper.UnitySendPost(salesdis);

        }
        private static string siparisNoBelirle(string siparisNo)
        {
            string ficheNo;
            string fisnoSayisi = Kontrol.degerAra("COUNT(LOGICALREF)", "STFICHE", "FICHENO", " like '" + siparisNo + "%'", OpC.firmaDonem);
            //Birden fazla irsaliye oluşturulduğunda artan numara veriyor.
            if (Convert.ToInt32(fisnoSayisi) > 0)
            {
                int count = Convert.ToInt32(fisnoSayisi) - 1;
                string adet = count.ToString();
                ficheNo = siparisNo + "-" + adet;
            }
            else
            {
                ficheNo = siparisNo;
            }
            return ficheNo;
        }


    }
    public class satinalmaIrs
    {
        public static string satinalmaContent(string irsaliyetarih, string FICHENO, object saat,
          DateTime textIrsaliye, string[] listMiktar, string[] lotName, string[] lotCode, string[] stokCode, string Carikodu)
        {
            UnityKeeper.Unitylogin();
            UnityObjects.IUnityApplication UnityAppTutucu = UnityKeeper.UnityApp;

            int dk = Convert.ToInt16(DateTime.Now.ToString("mm"));
            int saati = Convert.ToInt16(DateTime.Now.ToString("HH"));
            int saniye = Convert.ToInt16(DateTime.Now.ToString("ss"));
            UnityAppTutucu.PackTime(saati, dk, saniye, ref saat);
            UnityObjects.Data purdis = UnityAppTutucu.NewDataObject(UnityObjects.DataObjectType.doPurchDispatch);
            purdis.New();
            purdis.DataFields.FieldByName("TYPE").Value = 1;
            purdis.DataFields.FieldByName("NUMBER").Value = FICHENO;                           //FİŞ NUMRASI
            purdis.DataFields.FieldByName("DOC_NUMBER").Value = "MKBL-" + FICHENO;
            purdis.DataFields.FieldByName("DATE").Value = textIrsaliye.ToString("dd.MM.yyyy"); ;
            purdis.DataFields.FieldByName("TIME").Value = saat;
            purdis.DataFields.FieldByName("AUXIL_CODE").Value = "Operp";                            //ÖZELKOD BEŞ
            purdis.DataFields.FieldByName("ARP_CODE").Value = Carikodu;                                 //CARİ HESAP KODU
            purdis.DataFields.FieldByName("CREATED_BY").Value = 1;
            purdis.DataFields.FieldByName("DATE_CREATED").Value = DateTime.Now.ToString("dd.MM.yyy");
            purdis.DataFields.FieldByName("HOUR_CREATED").Value = DateTime.Now.ToString("HH");
            purdis.DataFields.FieldByName("MIN_CREATED").Value = DateTime.Now.ToString("mm");
            purdis.DataFields.FieldByName("SEC_CREATED").Value = DateTime.Now.ToString("ss");
            purdis.DataFields.FieldByName("CURRSEL_TOTALS").Value = 1;

       

            int tut = 0;
            foreach (string item in stokCode)
            {

                string itemlogi = Kontrol.degerAra("LOGICALREF", "ITEMS", "CODE", "='" + item + "'", "LG_019_");
                string unitRef = Kontrol.degerAra("UNITSETREF", "ITEMS", "LOGICALREF", "=" + itemlogi, "LG_019_");

                string birimSeti = Kontrol.degerAra("CODE", "UNITSETL", "UNITSETREF", "='" + unitRef + "' AND MAINUNIT = 1", "LG_019_");


                UnityObjects.Lines transactions_lines = purdis.DataFields.FieldByName("TRANSACTIONS").Lines;




                transactions_lines.AppendLine();
                transactions_lines[transactions_lines.Count - 1].FieldByName("TYPE").Value = 0;
                transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_CODE").Value = item;           //İtems Code
                transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = listMiktar[tut];                  //miktar
                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = birimSeti;              //Unıt set l 
                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = listMiktar[tut];                //UINFO1
                transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = listMiktar[tut];                //UINFO2
                transactions_lines[transactions_lines.Count - 1].FieldByName("VAT_RATE").Value = 18;
                //transactions_lines[transactions_lines.Count - 1].FieldByName("DATA_REFERENCE").Value = 6;

                UnityObjects.Lines sl_details0 = transactions_lines[transactions_lines.Count - 1].FieldByName("SL_DETAILS").Lines;
                sl_details0.AppendLine();
                sl_details0[sl_details0.Count - 1].FieldByName("SOURCE_MT_REFERENCE").Value = 0;
                sl_details0[sl_details0.Count - 1].FieldByName("SOURCE_SLT_REFERENCE").Value = 0;
                sl_details0[sl_details0.Count - 1].FieldByName("SOURCE_QUANTITY").Value = 0;
                sl_details0[sl_details0.Count - 1].FieldByName("IOCODE").Value = 1;
                sl_details0[sl_details0.Count - 1].FieldByName("SL_TYPE").Value = 1;
                sl_details0[sl_details0.Count - 1].FieldByName("SL_CODE").Value = lotCode[tut];                       //SERİLOT N REFERANSI
                sl_details0[sl_details0.Count - 1].FieldByName("SL_NAME").Value = lotName[tut];                       //SERİLOTN ADI
                sl_details0[sl_details0.Count - 1].FieldByName("MU_QUANTITY").Value = listMiktar[tut];              //LOT MİKTAR
                sl_details0[sl_details0.Count - 1].FieldByName("UNIT_CODE").Value = birimSeti;                          //UNİTsET L
                sl_details0[sl_details0.Count - 1].FieldByName("QUANTITY").Value = listMiktar[tut];              //AMOUNT
                sl_details0[sl_details0.Count - 1].FieldByName("REM_QUANTITY").Value = listMiktar[tut];              //AMOUNT
                sl_details0[sl_details0.Count - 1].FieldByName("LU_REM_QUANTITY").Value = listMiktar[tut];
                sl_details0[sl_details0.Count - 1].FieldByName("UNIT_CONV1").Value = listMiktar[tut];
                sl_details0[sl_details0.Count - 1].FieldByName("UNIT_CONV2").Value = listMiktar[tut];
                sl_details0[sl_details0.Count - 1].FieldByName("DATE_EXPIRED").Value = DateTime.Now.ToString("dd.MM.yyy");
                //sl_details0[sl_details0.Count - 1].FieldByName("DATA_REFERENCE").Value = 3;
                sl_details0[sl_details0.Count - 1].FieldByName("ORGLINKREF").Value = 0;
                //sl_details0[sl_details0.Count - 1].FieldByName("GUID").Value = "9246B348 - 93B9 - 4BA1 - AC77 - 5C2CE46CDE21";



                transactions_lines[transactions_lines.Count - 1].FieldByName("DIST_ORD_REFERENCE").Value = 0;
                transactions_lines[transactions_lines.Count - 1].FieldByName("MULTI_ADD_TAX").Value = 0;
                transactions_lines[transactions_lines.Count - 1].FieldByName("EDT_CURR").Value = 1;
                transactions_lines[transactions_lines.Count - 1].FieldByName("MONTH").Value = textIrsaliye.ToString("MM");
                transactions_lines[transactions_lines.Count - 1].FieldByName("YEAR").Value = textIrsaliye.ToString("yyyy");
                //transactions_lines[transactions_lines.Count - 1].FieldByName("GUID").Value = "A300883A - C082 - 4DDB - AA30 - 692C535F08FC";
                //transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_DEF").Value = "OnurPlastik Hammmadde";
                //transactions_lines[transactions_lines.Count - 1].FieldByName("MASTER_DEF2").Value = "OnurPlastik Hammmadde Deneme";
                transactions_lines[transactions_lines.Count - 1].FieldByName("FOREIGN_TRADE_TYPE").Value = 0;
                transactions_lines[transactions_lines.Count - 1].FieldByName("DISTRIBUTION_TYPE_WHS").Value = 0;
                transactions_lines[transactions_lines.Count - 1].FieldByName("DISTRIBUTION_TYPE_FNO").Value = 0;

                tut++;



            }
            purdis.DataFields.FieldByName("DEDUCTIONPART1").Value = 2;
            purdis.DataFields.FieldByName("DEDUCTIONPART2").Value = 3;
            purdis.DataFields.FieldByName("AFFECT_RISK").Value = 0;
            purdis.DataFields.FieldByName("DISP_STATUS").Value = 1;
            //purdis.DataFields.FieldByName("GUID").Value = "8A1F193E - 5117 - 4B15 - A9DD - 0850234DA140";
            purdis.DataFields.FieldByName("DOC_DATE").Value = DateTime.Now.ToString("dd.MM.yyy");
            purdis.DataFields.FieldByName("DOC_TIME").Value = 170927457;
            purdis.DataFields.FieldByName("EDESPATCH_STATUS").Value = 12;

            return UnityKeeper.UnitySendPost(purdis);
        
        }

    }
    public class Ambarfisi
    {
        public static string AmbarTransferFisi(string FICHENO, List<string> SOURCE_MT_REFERENCE,
            List<string> SOURCE_SLT_REFERENCE, List<string> DATE, List<string> lottanCikilacakMiktar, string malzemeKodu, string souceWh, string desWh,
            string iadeMiktar, string labelIadeLotno)
        {
            object saat = 0;

            UnityKeeper.Unitylogin();
            UnityObjects.IUnityApplication UnityAppTutucu = UnityKeeper.UnityApp;



            int dk = Convert.ToInt16(DateTime.Now.ToString("mm"));
            int saati = Convert.ToInt16(DateTime.Now.ToString("HH"));
            int saniye = Convert.ToInt16(DateTime.Now.ToString("ss"));
            UnityAppTutucu.PackTime(saati, dk, saniye, ref saat);


            string itemlogi = Kontrol.degerAra("LOGICALREF", "ITEMS", "CODE", "='" + malzemeKodu + "'", OpC.firma);
            string unitRef = Kontrol.degerAra("UNITSETREF", "ITEMS", "LOGICALREF", "=" + itemlogi, OpC.firma);

            string birimSeti = Kontrol.degerAra("CODE", "UNITSETL", "UNITSETREF", "='" + unitRef + "' AND MAINUNIT = 1", OpC.firma);



            UnityObjects.Data items = UnityAppTutucu.NewDataObject(UnityObjects.DataObjectType.doMaterialSlip);
            items.New();
            items.DataFields.FieldByName("GROUP").Value = 3;
            items.DataFields.FieldByName("TYPE").Value = 25;
            items.DataFields.FieldByName("NUMBER").Value = FICHENO;
            items.DataFields.FieldByName("DATE").Value = DateTime.Now.ToString("dd.MM.yyyy");
            items.DataFields.FieldByName("TIME").Value = saat;
            items.DataFields.FieldByName("SOURCE_WH").Value = souceWh;
            items.DataFields.FieldByName("DEST_WH").Value = desWh;
            items.DataFields.FieldByName("DEST_COST_GRP").Value = 1;
            items.DataFields.FieldByName("RC_RATE").Value = 5.5916;
            items.DataFields.FieldByName("CREATED_BY").Value = 2;
            items.DataFields.FieldByName("DATE_CREATED").Value = DateTime.Now.ToString("dd.MM.yyyy");
            items.DataFields.FieldByName("HOUR_CREATED").Value = 17;
            items.DataFields.FieldByName("MIN_CREATED").Value = 11;
            items.DataFields.FieldByName("SEC_CREATED").Value = 56;
            items.DataFields.FieldByName("DATA_REFERENCE").Value = 15817;

            UnityObjects.Lines transactions_lines = items.DataFields.FieldByName("TRANSACTIONS").Lines;
            transactions_lines.AppendLine();
            transactions_lines[transactions_lines.Count - 1].FieldByName("ITEM_CODE").Value = malzemeKodu;
            transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_TYPE").Value = 0;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DESTINDEX").Value = 1;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DESTCOSTGRP").Value = 1;
            transactions_lines[transactions_lines.Count - 1].FieldByName("LINE_NUMBER").Value = 1;
            transactions_lines[transactions_lines.Count - 1].FieldByName("QUANTITY").Value = iadeMiktar;
            transactions_lines[transactions_lines.Count - 1].FieldByName("RC_XRATE").Value = 5.5916;
            transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CODE").Value = birimSeti;
            transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV1").Value = iadeMiktar;
            transactions_lines[transactions_lines.Count - 1].FieldByName("UNIT_CONV2").Value = iadeMiktar;
            transactions_lines[transactions_lines.Count - 1].FieldByName("DATA_REFERENCE").Value = 31445;

            int index = 0;
            string date = DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy");

            UnityObjects.Lines sl_details0 = transactions_lines[transactions_lines.Count - 1].FieldByName("SL_DETAILS").Lines;
            foreach (var miktar in lottanCikilacakMiktar)
            {
                sl_details0.AppendLine();
                sl_details0[sl_details0.Count - 1].FieldByName("SOURCE_MT_REFERENCE").Value = SOURCE_MT_REFERENCE[index];
                sl_details0[sl_details0.Count - 1].FieldByName("SOURCE_SLT_REFERENCE").Value = SOURCE_SLT_REFERENCE[index];
                sl_details0[sl_details0.Count - 1].FieldByName("SOURCE_QUANTITY").Value = miktar;
                sl_details0[sl_details0.Count - 1].FieldByName("IOCODE").Value = 3;
                sl_details0[sl_details0.Count - 1].FieldByName("SOURCE_WH").Value = souceWh;
                sl_details0[sl_details0.Count - 1].FieldByName("SL_TYPE").Value = 1;
                sl_details0[sl_details0.Count - 1].FieldByName("SL_CODE").Value = labelIadeLotno;
                sl_details0[sl_details0.Count - 1].FieldByName("MU_QUANTITY").Value = miktar;
                sl_details0[sl_details0.Count - 1].FieldByName("UNIT_CODE").Value = birimSeti;
                sl_details0[sl_details0.Count - 1].FieldByName("QUANTITY").Value = miktar;
                sl_details0[sl_details0.Count - 1].FieldByName("UNIT_CONV1").Value = miktar;
                sl_details0[sl_details0.Count - 1].FieldByName("UNIT_CONV2").Value = miktar;
                sl_details0[sl_details0.Count - 1].FieldByName("DATE_EXPIRED").Value = DATE[index];// DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy");//"18.07.2019"; //date;//DATE[index].ToString("dd.MM.yyyy");

                index++;

            }
            return UnityKeeper.UnitySendPost(items);
        }

    }


}
