using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

internal class Parser
{
    private Token[] tokens_;
    private int position_;

    public Parser(List<Token> tokens)
    {
        tokens_ = tokens.ToArray();
        position_ = 0;
    }

    private Token? PeekToken(int n = 0)
    {
        int peek = position_ + n;
        if (peek > tokens_.Length)
        {
            return null;
        }
        return tokens_[peek];
    }

    private Token? PeekPrevToken()
    {
        if (position_ == 0)
        {
            return null;
        }
        return tokens_[position_ - 1];
    }

    private Token? EatToken()
    {
        Token token = tokens_[position_];
        if (position_ < tokens_.Length)
        {
            position_++;
        }
        return token;
    }

    private Token EatTokenExpect(TokenType expected)
    {
        Token? token = EatToken();
        if(token == null)
        {
            throw new Exception("Got null token when not expecting one @todo");
        }
        if (token.Value.type_ != expected)
        {
            throw new Exception("Unexpected token " + token.Value.type_.ToString() + " received while expecting " + expected.ToString());
        }
        return token.Value;
    }

    private AstProcedure ParseProcedure()
    {
        AstProcedure ast_proc;

        Token? ident_token = EatToken();
        if (ident_token.HasValue)
        {
            ast_proc = new AstProcedure(ident_token.Value.token_);

            // Eat Double Colons and Open parenthesis
            EatToken();
            EatToken();

            while (true)
            {
                Token? token = EatToken();
                if (!token.HasValue)
                {
                    throw new Exception("Received null token while parsing procedure argument list.");
                }

                if (token.Value.type_ == TokenType.CLOSE_PARENTHESIS)
                {
                    break;
                }
                else if (token.Value.type_ == TokenType.IDENTIFIER)
                {
                    EatTokenExpect(TokenType.COLON);

                    Token? type = EatToken();
                    if (!type.HasValue)
                    {
                        throw new Exception("Syntax error in procedure's argument list.");
                    }

                    ast_proc.AddArgument(token.Value.token_, type.Value.type_);
                }
            }

            Token? past_proc = EatToken();
            if (!past_proc.HasValue || (past_proc.HasValue && past_proc.Value.type_ != TokenType.OPEN_BRACE))
            {
                throw new Exception("Expected { after procedure signature.");
            }

            return ast_proc;
        }
        else
        {
            throw new Exception("Invalid call to ParseProcedure.");
        }
    }

    public AstNode ParseToAST()
    {
        AstProgramRoot root = new AstProgramRoot();

        while (true)
        {
            Token? _token = PeekToken();
            if (_token == null)
            {
                throw new Exception("Peeked null token before reaching EOF.");
            }

            Token token = _token.Value;

            // Check if EOF
            if (token.type_ == TokenType.END_OF_FILE)
            {
                break;
            }
            else if (token.type_ == TokenType.IDENTIFIER)
            {
                // Check if procedure signature
                {
                    Token? token_n1;
                    Token? token_n2;

                    if ((token_n1 = PeekToken(1)) != null && token_n1.Value.type_ == TokenType.DOUBLE_COLON && (token_n2 = PeekToken(2)) != null && token_n2.Value.type_ == TokenType.OPEN_PARENTHESIS)
                    {
                        // It is procedure signature
                        AstProcedure ast_proc = ParseProcedure();
                        root.AddSubNode(ast_proc);
                    }
                }
            }
            else
            {
                EatToken();
            }
        }

        return root;
    }
}