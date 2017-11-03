namespace TsModelGenerator
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class Program
    {
        public static void Main(string[] args)
        {
            if (null != args && args.Length > 1)
            {
                //const string sourcePath = @"X:\MiaTek\Projects\BioID\BioID.Server\0BioID.Models\bin\Debug\Northwind.Models.dll";
                //const string destPath = @"X:\MiaTek\Projects\BioID\BioID.Client\src\BioID.Client\app\models\entities.ts";

                string sourcePath = args[0];// @"X:\ionix.Demo\Northwind\Northwind.Models\bin\Debug\Northwind.Models.dll";
                string destPath = args[1];// @"X:\ionix.Demo\Northwind\Northwind.Client\app\models\entities.ts";

                if (!String.IsNullOrEmpty(sourcePath) && !String.IsNullOrEmpty(destPath))
                {
                    //while (true)
                    //{
                    try
                    {
                        Assembly asm = Assembly.UnsafeLoadFrom(sourcePath);

                        StringBuilder text = new StringBuilder();
                        ModuleWriter mw = new ModuleWriter();
                        mw.Write(text, asm);

                        string code = text.ToString();

                        using (Stream fs = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                        {
                            using (TextWriter sw = new StreamWriter(fs))
                            {
                                sw.WriteLine(code);
                            }
                        }
                        // Console.WriteLine("İşlem Balarılı");
                        // Console.WriteLine("İşlemi Sonlandırmak için 'n' tuşuna basınız");
                        // char input = Console.ReadKey().KeyChar;
                        // if (input == 'n')
                        // break;

                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (Exception exSub in ex.LoaderExceptions)
                        {
                            sb.AppendLine(exSub.Message);
                            if (exSub is FileNotFoundException exFileNotFound)
                            {
                                if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                                {
                                    sb.AppendLine("Assembly Yüklenemedi:");
                                    sb.AppendLine(exFileNotFound.FusionLog);
                                }
                            }
                            sb.AppendLine();
                        }
                        string errorMessage = sb.ToString();
                        Console.WriteLine(errorMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Assembly Okunamadı. " + ex.Message);
                    }
                    // }   
                }
            }
        }
    }
}
