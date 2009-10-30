using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.UI.MobileControls;
using System.IO;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Service : System.Web.Services.WebService
{
    public Service () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public CandyEntity[] GetAllModules() 
    {
        List<CandyEntity> retVal = new List<CandyEntity>();
        foreach (string idx in Directory.GetFiles(Server.MapPath("~/Candies/"), "*.zip"))
        {
            string candyFileName = idx.Substring(idx.LastIndexOf('\\') + 1);
            CandyEntity entity = new CandyEntity();
            entity.CandyName = candyFileName;
            entity.CandyImageUrl =
                this.Context.Request.Url.ToString().Replace("CandyStore.asmx", "") + 
                "Candies/" +
                candyFileName.Replace(".zip", ".png");
            retVal.Add(entity);
        }
        return retVal.ToArray();
    }

    [WebMethod]
    public byte[] GetSpecificModule(string fileName)
    {
        using (FileStream stream = File.OpenRead(Server.MapPath("~/Candies/" + fileName)))
        {
            byte[] retVal = new byte[stream.Length];
            stream.Read(retVal, 0, retVal.Length);
            return retVal;
        }
    }
}
