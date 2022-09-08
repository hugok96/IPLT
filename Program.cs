using System;
using System.IO;

namespace IPLT
{
    class Program
    {
        static void Main(string[] args)
        {
            switch (args.Length > 0 ? args[0] : null)
            {
                case "compile":
                    if (args.Length != 3)
                    {
                        DisplayHelp(true);
                        return;
                    }

                    if (false == File.Exists(args[1]))
                    {
                        Console.Error.WriteLine("Error: Could not read input file.");
                        return;
                    }

                    Compiler.Compile(args[1], args[2]);
                    break;
                case "search":
                    if (args.Length != 3)
                    {
                        DisplayHelp(true);
                        return;
                    }

                    if (false == File.Exists(args[1])) 
                    {
                        Console.Error.WriteLine("Error: Could not read input file.");
                        return;
                    }

                    var ipElements = args[2].Split('.');
                    var ip = new byte[4] { 0, 0, 0, 0};
                    if (ipElements.Length != 4) 
                    {
                        Console.Error.WriteLine("Error: Invalid IP supplied, should be four 8-bit unsigned integers joined by dots.");
                        return;
                    }

                    for(int i = 0; i < 4; i++)
                    {
                        if(false == byte.TryParse(ipElements[i], out byte result))
                        {
                            Console.Error.WriteLine("Error: Invalid IP supplied, should be four 8-bit unsigned integers joined by dots.");
                            return;
                        }
                        ip[i] = result;
                    }

                    Reader.SearchIP(args[1], ip);


                    break;
                default:
                    DisplayHelp();
                    break;
            }
        }

        static void DisplayHelp(bool error = false)
        {
            var outStream = error ? Console.Error : Console.Out;
            outStream.WriteLine("Usages:");
            outStream.WriteLine("  IPLT compile input_file compiled_output_file");
            outStream.WriteLine("  IPLT search  input_file ip( X.X.X.X )");
            outStream.WriteLine("");
        }
    }
}
