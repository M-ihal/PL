using System;
using System.IO;
using System.Text;

namespace PL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (args.Length == 0)
            {
                Console.Error.WriteLine("No file specified.\n");
                return;
            }

            string filepath = args[0];
            string extension = Path.GetExtension(filepath);
            if (extension != ".pl")
            {
                Console.Error.WriteLine("Invalid file specified (Wrong extension).\nFile specified: \"" + filepath + "\"\n");
                return;
            }

            try
            {
                string filedata = File.ReadAllText(filepath);
                Console.WriteLine("Successfully read file: \"" + filepath + "\".\n");
                Console.WriteLine(filedata);

                Lexer lexer = new Lexer(filedata);

                bool finished = false;
                while (finished == false)
                {
                    Token token = lexer.NextToken();

                    Console.WriteLine("Token: " + token.type_.ToString() + " = \"" + token.token_ + "\"");
                                        
                    switch (token.type_)
                    {
                        default:
                        {
                            break;
                        }
                        case TokenType.END_OF_FILE:
                        {
                            finished = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Exception:\n" + ex.ToString());
                return;
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}