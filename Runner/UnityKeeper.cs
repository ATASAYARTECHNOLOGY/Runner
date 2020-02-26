using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Runner
{
    public class UnityKeeper
    {
        public static UnityObjects.UnityApplication UnityApp;

        public static void Unitylogin()
        {
            if (UnityKeeper.UnityApp == null)
            {
                UnityKeeper.UnityApp = new UnityObjects.UnityApplication();
                string ladi = ConfigurationManager.AppSettings["logoUsr"];
                string lsifre = ConfigurationManager.AppSettings["logoPw"];
                string lfirma = ConfigurationManager.AppSettings["logoFirm"];
                string ldonem = ConfigurationManager.AppSettings["logoDonem"];
                UnityKeeper.UnityApp.Login(ladi, lsifre, Convert.ToInt32(lfirma), Convert.ToInt32(ldonem));
            }
        }

        public static string UnitySendPost(UnityObjects.Data items)
        {
            string msg;
            if (items.Post() == true)
            {
                msg = ("ok");
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