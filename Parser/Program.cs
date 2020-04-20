using System;
using System.Text;
using Common;

namespace Parser
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var parser = new Parser(1, new JsonVideoRepository());
                parser.Parse().Wait();
                Console.ReadKey(true);
            }
            catch (System.Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
            }
        }
    }
}
