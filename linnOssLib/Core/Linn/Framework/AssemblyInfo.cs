using System;
using System.Reflection;
using System.Text;

namespace Linn
{
	internal class AssemblyInfo
	{
		internal static AssemblyInfoModel GetAssemblyInfo() {
			
			string description = "";
		    string version = "";
		    string company = "";
		    string copyright = "";
		    string title = "";
		    string product = "";
            string informationalVersion = "";
		    
		    System.Reflection.Assembly entryAssembly = Assembly.GetEntryAssembly();
		    
            object[] attributes = entryAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (attributes.Length != 0)
            {
            	description = ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
                     
            attributes = entryAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (attributes.Length != 0)
            {
            	company = ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
                    
            attributes = entryAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length != 0)
            {
                copyright = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
            
           	attributes = entryAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
         	if (attributes.Length != 0)
            {
                product = ((AssemblyProductAttribute)attributes[0]).Product;
            }

            title = System.IO.Path.GetFileNameWithoutExtension(entryAssembly.CodeBase);
         	attributes = entryAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
         	if (attributes.Length != 0)
            {
                if (((AssemblyTitleAttribute)attributes[0]).Title != "") 
                {
                    title = ((AssemblyTitleAttribute)attributes[0]).Title;
                }
            }
        
            version = entryAssembly.GetName().Version.ToString().Replace(".0", "");

            attributes = entryAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            if (attributes.Length != 0)
            {
                informationalVersion = ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
            }
            
            AssemblyInfoModel properties = new AssemblyInfoModel(description
                                                                                         ,version
                                                                                         ,company
                                                                                         ,copyright
                                                                                         , title
                                                                                         , product
                                                                                         , informationalVersion);
            
            return properties;			
		}
		
	}
}
