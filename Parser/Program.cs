using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Common;

namespace Parser
{
    internal static class Program
    {
        private const string ErrorPath = "errors.log";
        private const string DataPath = "Data\\videos.json";

        private static void Main()
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var parser = new Parser(new JsonVideoRepository(DataPath));
                Console.WriteLine("WORK STARTED...");
                var sw = Stopwatch.StartNew();
                parser.RunAsync().Wait();
                Console.WriteLine($"DONE! Time: [{sw.Elapsed.TotalSeconds} s]");
                sw.Stop();
                if (parser.Errors.Count > 0)
                {
                    try
                    {
                        File.WriteAllText(ErrorPath, string.Join(Environment.NewLine, parser.Errors.ToArray()));
                        Console.WriteLine($"See errors: {Path.GetFullPath(ErrorPath)}");
                    }
                    catch (System.Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error saving error: {e.Message}");
                        Console.ResetColor();
                    }
                }
            }
            catch (System.Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
            }
            Console.ReadKey(true);
        }
    }
}
