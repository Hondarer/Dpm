using System;
using System.Linq;
using System.Reflection;

// TODO: 必要最低限しか作業していない

namespace Hondarersoft.Dpm.Environment
{
    public class AssemblyInfo
    {
        public string Name { get; protected set; }
        public string Title { get; protected set; }
        public Version Version { get; protected set; }

        public string Description { get; protected set; }

        public string Company { get; protected set; }

        public AssemblyInfo()
        {
            Assembly asm = Assembly.GetEntryAssembly();

            AssemblyName asmName = asm.GetName();
            Name = asmName.Name;
            Version = asmName.Version;
            string fullname = "AssemblyName.FullName : " + asmName.FullName;
            string processor = "AssemblyName.ProcessorArchitecture : " + asmName.ProcessorArchitecture;
            string runtime = "Assembly.ImageRuntimeVersion : " + asm.ImageRuntimeVersion;

            object titleObject = asm.GetCustomAttributes(typeof(AssemblyTitleAttribute), false).FirstOrDefault();
            if (titleObject != null)
            {
                Title = ((AssemblyTitleAttribute)titleObject).Title;
            }

            object descriptionObject = asm.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).FirstOrDefault();
            if (descriptionObject != null)
            {
                Description = ((AssemblyDescriptionAttribute)descriptionObject).Description;
            }

            object companyObject = asm.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false).FirstOrDefault();
            if (companyObject != null)
            {
                Company = ((AssemblyCompanyAttribute)companyObject).Company;
            }

            object[] productarray = asm.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            string product = "AssemblyProductAttribute : " + ((AssemblyProductAttribute)productarray[0]).Product;

            object[] copyrightarray = asm.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            string copyright = "AssemblyCopyrightAttribute : " + ((AssemblyCopyrightAttribute)copyrightarray[0]).Copyright;

            object[] trademarkarray = asm.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);
            string trademark = "AssemblyTrademarkAttribute : " + ((AssemblyTrademarkAttribute)trademarkarray[0]).Trademark;

            object[] fileversionarray = asm.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            string fileversion = "AssemblyFileVersionAttribute : " + ((AssemblyFileVersionAttribute)fileversionarray[0]).Version;
        }
    }
}
