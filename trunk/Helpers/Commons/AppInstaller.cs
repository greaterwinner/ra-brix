/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.IO;
using System.Web;
using ICSharpCode.SharpZipLib.Zip;
using LanguageRecords;
using Ra;
using System;

namespace HelperGlobals
{
    public class AppInstaller
    {
        public static void InstallApplication(string zipFileName, byte[] zipFileContent)
        {
            string folderName = zipFileName.Replace(".zip", "").Replace(".ZIP", "");
            string binFolder = HttpContext.Current.Server.MapPath("~/bin");
            if (Directory.Exists(binFolder + "/" + folderName))
                Directory.SetCreationTime(binFolder + "/" + folderName, DateTime.Now);
            Directory.CreateDirectory(binFolder + "/" + folderName);
            using (MemoryStream memStream = new MemoryStream(zipFileContent))
            {
                memStream.Position = 0;
                using (ZipInputStream zipInput = new ZipInputStream(memStream))
                {
                    ZipEntry current = zipInput.GetNextEntry();
                    while (current != null)
                    {
                        using (FileStream output = new FileStream(
                            binFolder + "/" + folderName + "/" + current.Name,
                            FileMode.Create,
                            FileAccess.Write))
                        {
                            byte[] buffer = new byte[current.Size];
                            zipInput.Read(buffer, 0, buffer.Length);
                            output.Write(buffer, 0, buffer.Length);
                        }
                        current = zipInput.GetNextEntry();
                    }
                }
            }
            Language.Instance.SetDefaultValue("ApplicationWasInstalledRedirecting", @"
A new application was installed, and hence we had to refresh the browser 
and you might need to login again...");
            AjaxManager.Instance.Redirect("~/?message=ApplicationWasInstalledRedirecting");
        }
    }
}




