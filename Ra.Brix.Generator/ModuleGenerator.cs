using System;
using System.Collections.Generic;
using System.Text;
using Ra.Brix.Generator.Properties;
using System.IO;

namespace Ra.Brix.Generator
{
    public class ModuleGenerator
    {
        #region [-- CTOR's --]

        /// <summary>
        /// Creates a new ModuleGenerator
        /// </summary>
        /// <param name="moduleName">The fully qualified name of the module</param>
        public ModuleGenerator(string moduleName)
        {
            ModuleName = moduleName.Substring(moduleName.LastIndexOf(".") + 1);
            ModuleNamespace = moduleName.Replace("." + ModuleName, string.Empty);
        }

        #endregion

        #region [-- Private Properties --]

        private string ModuleName { get; set; }
        private string ModuleNamespace { get; set; }

        private string ModuleFullName
        {
            get { return string.Format("{0}.{1}", ModuleNamespace, ModuleName); }
        }

        private string ModuleAssemblyInfoText
        {
            get { return string.Format(Resources.AssemblyInfo_cs, ModuleNamespace, DateTime.Now.ToString("yyyy")); }
        }

        private string ModuleAscxText
        {
            get { return string.Format(Resources.Brix_ascx, ModuleFullName); }
        }

        private string ModuleAscxCsText
        {
            get { return string.Format(Resources.Brix_ascx_cs, ModuleNamespace, ModuleName); }
        }

        private string ModuleProjectText
        {
            get { return string.Format(Resources.BrixModules_csproj, ModuleNamespace, ModuleName); }
        }

        private string ControllerCsText
        {
            get { return string.Format(Resources.BrixController_cs, ModuleName, ModuleFullName); }
        }

        private string ControllerAssemblyInfoText
        {
            get { return string.Format(Resources.AssemblyInfo_cs, ModuleName + "Controller", DateTime.Now.ToString("yyyy")); }
        }

        private string ControllerProjectText
        {
            get { return string.Format(Resources.BrixController_csproj, ModuleName); }
        }

        #endregion

        public string CreateFiles(string folderName)
        {
            try
            {
                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                string moduleFolder = Path.Combine(folderName, "Module");
                string controllerFolder = Path.Combine(folderName, "Controller");

                Directory.CreateDirectory(Path.Combine(moduleFolder, "Properties"));
                Directory.CreateDirectory(Path.Combine(controllerFolder, "Properties"));

                GenerateModuleFiles(moduleFolder);
                GenerateControllerFiles(controllerFolder);

                return "Files created successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void GenerateModuleFiles(string folderName)
        {
            File.WriteAllText(Path.Combine(folderName, ModuleName + ".ascx"), ModuleAscxText, Encoding.UTF8);
            File.WriteAllText(Path.Combine(folderName, ModuleName + ".ascx.cs"), ModuleAscxCsText, Encoding.UTF8);
            File.WriteAllText(Path.Combine(folderName, "Properties/AssemblyInfo.cs"), ModuleAssemblyInfoText, Encoding.UTF8);
            File.WriteAllText(Path.Combine(folderName, ModuleNamespace + ".csproj"), ModuleProjectText, Encoding.UTF8);
        }

        private void GenerateControllerFiles(string folderName)
        {
            File.WriteAllText(Path.Combine(folderName, ModuleName + "Controller.cs"), ControllerCsText, Encoding.UTF8);
            File.WriteAllText(Path.Combine(folderName, "Properties/AssemblyInfo.cs"), ControllerAssemblyInfoText, Encoding.UTF8);
            File.WriteAllText(Path.Combine(folderName, ModuleName + "Controller.csproj"), ControllerProjectText, Encoding.UTF8);
        }
    }
}
