using System;
using System.Web.Services.Description;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Security.Permissions;
using System.Text;
using System.IO;

public class WsdlManager
{

    public static void Generate(string wsdlPath, string outputFilePath)
    {
        if (File.Exists(wsdlPath) == false)
        {
            return;
        }


        ServiceDescription wsdlDescription = ServiceDescription.Read(wsdlPath);
        ServiceDescriptionImporter wsdlImporter = new ServiceDescriptionImporter();


        wsdlImporter.ProtocolName = "Soap12";
        wsdlImporter.AddServiceDescription(wsdlDescription, null, null);
        wsdlImporter.Style = ServiceDescriptionImportStyle.Server;


        wsdlImporter.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties;


        CodeNamespace codeNamespace = new CodeNamespace();
        CodeCompileUnit codeUnit = new CodeCompileUnit();
        codeUnit.Namespaces.Add(codeNamespace);


        ServiceDescriptionImportWarnings importWarning = wsdlImporter.Import(codeNamespace, codeUnit);


        if (importWarning == 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);


            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");
            codeProvider.GenerateCodeFromCompileUnit(codeUnit, stringWriter, new CodeGeneratorOptions());


            stringWriter.Close();


            File.WriteAllText(outputFilePath, stringBuilder.ToString(), Encoding.UTF8);
        }
        else
        {
            Console.WriteLine(importWarning);
        }
    }


    [PermissionSetAttribute(SecurityAction.Demand, Name = "Full Trust")]
    public static void Run(string WsdlName)
    {
        // Get a WSDL file describing a service.
        ServiceDescription description = ServiceDescription.Read(WsdlName);

        // Initialize a service description importer.
        ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
        importer.ProtocolName = "Soap12";  // Use SOAP 1.2.
        importer.AddServiceDescription(description, null, null);

        // Report on the service descriptions.
        Console.WriteLine("Importing {0} service descriptions with {1} associated schemas.",
                          importer.ServiceDescriptions.Count, importer.Schemas.Count);

        // Generate a proxy client.
        importer.Style = ServiceDescriptionImportStyle.Client;

        // Generate properties to represent primitive values.
        importer.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties;

        // Initialize a Code-DOM tree into which we will import the service.
        CodeNamespace nmspace = new CodeNamespace();
        CodeCompileUnit unit = new CodeCompileUnit();
        unit.Namespaces.Add(nmspace);

        // Import the service into the Code-DOM tree. This creates proxy code 
        // that uses the service.
        ServiceDescriptionImportWarnings warning = importer.Import(nmspace, unit);

        if (warning == 0)
        {
            // Generate and print the proxy code in C#.
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            provider.GenerateCodeFromCompileUnit(unit, Console.Out, new CodeGeneratorOptions());
        }
        else
        {
            // Print an error message.
            Console.WriteLine(warning);
        }
    }


}
