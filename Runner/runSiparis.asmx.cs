using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;

namespace Runner
{
    /// <summary>
    /// Summary description for runSiparis
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class runSiparis : System.Web.Services.WebService
    {

        [WebMethod]
        public string siparis(DataTable dtSevkView, string siparisNo, string cariKodu)
        {
            return SiparisIrsaliyesi.SiparisIrsaliyesiOlustur(dtSevkView, siparisNo, cariKodu);
        }
    }
}
