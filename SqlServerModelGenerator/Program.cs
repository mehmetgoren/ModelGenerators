namespace SqlServerModelGenerator
{
    using System;
    using System.IO;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            if (null == args || args.Length < 2)
                return;

            string connStr = args[0];
            string output = args[1];


            TextWriter tw = new StringWriter();

            PocoCore core = new PocoCore(connStr);

            PocoCore.Tables tables = core.LoadTables();
            if (null != tables)
            {
                void WritePocoClassAttributes(PocoCore.Table table)
                {
                    string tableName = "	[Table(\"" + table.ClassName + "\"";
                    if (!String.Equals("dbo", table.Schema, StringComparison.OrdinalIgnoreCase))
                        tableName += ", Schema=\"" + table.Schema + "\"";
                    tableName += ")]";
                    tw.WriteLine(tableName);
                }


                string WritePocoColumn(PocoCore.Column c)
                {
                    //return	"[Key]" + "\n		" +  c.Entity;
                    //return c.Entity;

                    string s = "[DbSchema(IsNullable = " + c.IsNullable.ToString().ToLower();

                    if (c.IsPrimaryKey)
                        s += ", IsKey=true";
                    if (c.IsIdentity)
                        s += ", DatabaseGeneratedOption = ionix.Data.StoreGeneratedPattern.Identity";
                    if (c.MaxLength > 0)
                        s += ", MaxLength = " + c.MaxLength;
                    if (!String.IsNullOrEmpty(c.Default))
                    {
                        if (!c.Default.Contains("'"))
                            s += ", DefaultValue = \"" + c.Default + "\"";
                        else
                            s += ", DefaultValue = " + c.Default.Replace("N", "");
                    }

                    s += ")]";

                    if (c.PropertyType == "DateTime")
                    {
                        if (null == c.Table)
                            c.Table = "";

                        switch (c.Table)
                        {
                            default:
                                s += "\n" + "        [JsonConverter(typeof(SimpleDateTimeConverter))]";
                                break;
                        }
                    }

                    //if (!c.IsNullable)
                    //{
                    //	 s += "\n" +  "        [Required]";
                    //}
                    //if (c.MaxLength > 0)
                    //{
                    //     s += "\n" +  "        [StringLength(" + c.MaxLength + ")]";
                    //}

                    return s + "\n		" + c.Entity + "\n";
                };


                tw.WriteLine("namespace Models");
                tw.WriteLine("{");
                tw.WriteLine("    using System;");
                tw.WriteLine("    using System.ComponentModel.DataAnnotations.Schema;");
                tw.WriteLine("    using ionix.Data;");
                tw.WriteLine("    using Newtonsoft.Json;");
                tw.WriteLine();

                foreach (PocoCore.Table table in tables)
                {

                    WritePocoClassAttributes(table);
                    tw.WriteLine($"    public partial class {table.NameHumanCase}");
                    tw.WriteLine("    {");

                    foreach (PocoCore.Column col in table.Columns.OrderBy(x => x.Ordinal))
                    {
                        tw.Write("        ");
                        tw.WriteLine(WritePocoColumn(col));
                    }

                    tw.WriteLine("    }");
                    tw.WriteLine();
                }

                tw.WriteLine("}");
            }


            using (Stream fs = new FileStream(output, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(tw.ToString());
                }
            }
        }
    }
}
