using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

internal class Program
{
    private static void PrintASTree(AstNode? node, int level)
    {
        if(node == null)
        {
            return;
        }

        // @TODO 
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        if (args.Length == 0)
        {
            Console.Error.WriteLine("No file specified.\n");
            return;
        }

        try
        {
            string filepath = args[0];
            string extension = Path.GetExtension(filepath);
            if (extension != ".pl")
            {
                throw new Exception("Invalid file specified (Wrong extension).\nFile specified: \"" + filepath + "\"\n");
            }

            string filedata = File.ReadAllText(filepath);
            Console.WriteLine("Successfully read file: \"" + filepath + "\".\n");
            Console.WriteLine(filedata);
            Console.WriteLine();

            Lexer lexer = new Lexer(filedata);
            List<Token> tokens = lexer.Tokenize();

            // Write out tokens
            {
                Console.WriteLine("Tokens:");
                foreach(Token token in tokens)
                {
                    Console.WriteLine(token.type_.ToString() + " -> \"" + token.token_ + "\"");
                }
            }

            Parser parser = new Parser(tokens);
            AstNode ast_root = parser.ParseToAST();

            if(ast_root != null)
            {
                PrintASTree(ast_root, 0);
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