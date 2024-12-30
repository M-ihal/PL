using System;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

internal class Program
{
    private static void PrintSpaces(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Console.Write(" ");
        }
    }

    private static void PrintASTree(AstNode? node, int level)
    {
        if(node == null)
        {
            return;
        }

        PrintSpaces(level);

        switch (node)
        {
            default:
            {
                Console.WriteLine("Unknown/Invalid AST Node...");
                return;
            }

            case AstProgramRoot ast_root:
            {
                Console.WriteLine(ast_root.ToString());
                foreach(AstNode sub_node in ast_root.SubNodes)
                {
                    PrintASTree(sub_node, level + 1);
                }
            } break;

            case AstProcedure ast_proc:
            {
                Console.WriteLine(ast_proc.ToString() + " -> " + ast_proc.signature_ + ", return: " + ast_proc.return_type_);
                foreach (AstNode arg_node in ast_proc.arguments_)
                {
                    PrintASTree(arg_node, level + 1);
                }
                PrintASTree(ast_proc.block_, level + 1);
            } break;

            case AstParameterInt64 ast_param_int64:
            {
                Console.WriteLine(ast_param_int64.ToString());
            } break;

            case AstDeclarationInt64 ast_decl_int64:
            {
                Console.WriteLine(ast_decl_int64.ToString());
            } break;

            case AstBlock ast_block:
            {
                Console.WriteLine(ast_block.ToString());
                foreach(AstNode statement in ast_block.statements_)
                {
                    PrintASTree(statement, level + 1);
                }
            } break;

            case AstVariableInt64 ast_var_int64:
            {
                Console.WriteLine(ast_var_int64.ToString());
            } break;

            case AstReturn ast_return:
            {
                Console.WriteLine(ast_return.ToString());
                PrintASTree(ast_return.return_value_, level + 1);
            } break;

            case AstLiteralInt64 ast_literal_int64:
            {
                Console.WriteLine(ast_literal_int64.ToString() + " -> " + ast_literal_int64.value_.ToString());
            } break;

            case AstAssignment ast_assign:
            {
                Console.WriteLine(ast_assign.ToString());
                PrintASTree(ast_assign.variable_, level + 1);
                PrintASTree(ast_assign.assign_value_, level + 1);
            } break;
        }
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

            // Write out ast nodes
            Console.WriteLine();
            PrintASTree(ast_root, 0);
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