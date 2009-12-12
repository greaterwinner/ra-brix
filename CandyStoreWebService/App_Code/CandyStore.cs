using System.Collections.Generic;
using System.Web.Services;
using System.IO;
using CandyStoreEntities;

[WebService(Namespace = "http://ra-brix.org/CandyStoreWebService")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Service : WebService
{
    [WebMethod]
    public CandyEntity[] GetAllModules() 
    {
        List<CandyEntity> retVal = new List<CandyEntity>();
        foreach (string idx in Directory.GetFiles(Server.MapPath("~/Candies/"), "*.zip"))
        {
            string candyFileName = idx.Substring(idx.LastIndexOf('\\') + 1);
            CandyEntity entity = new CandyEntity();
            entity.Updated = File.GetLastWriteTime(idx);
            entity.CandyName = candyFileName;
            entity.CandyImageUrl = candyFileName.Replace(".zip", ".png");
            string descriptionFileName = Server.MapPath("~/Candies/") + 
                candyFileName.Replace(".zip", ".txt");
            if (File.Exists(descriptionFileName))
            {
                using (TextReader stream = File.OpenText(descriptionFileName))
                {
                    entity.Description = stream.ReadToEnd();
                }
            }
            else
            {
                entity.Description = "This candy doesn't have any available description";
            }
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
