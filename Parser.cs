using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// @TODO : Hardcoding variables to int64 for now

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

    private AstBlock ParseBlock()
    {
        EatTokenExpect(TokenType.OPEN_BRACE);

        AstBlock ast_block = new AstBlock();

        bool finished_block = false;
        while (finished_block == false)
        {
            Token? _token = EatToken();
            if(_token == null)
            {
                throw new Exception("Not closed block.");
            }

            Token token = _token.Value;
            switch(token.type_)
            {
                default:
                {
                    throw new Exception("Invalid token in procedure block.");
                }

                case TokenType.SEMI_COLON: continue; // Ignore multiple semi colons

                case TokenType.CLOSE_BRACE:
                {
                    finished_block = true;
                    continue;
                }

                case TokenType.KEYWORD_RETURN:
                {
                    Token? next_ = EatToken();

                    if (next_.HasValue)
                    {
                        Token next = next_.Value;

                        switch(next.type_)
                        {
                            default:
                            {
                                throw new Exception("Unexpected token after return keyword.");
                            }

                            case TokenType.NUMBER:
                            {
                                ast_block.AddStatement(new AstReturn(new AstLiteralInt64(next.value_.value_int64)));
                                EatTokenExpect(TokenType.SEMI_COLON);
                            } break;

                            case TokenType.IDENTIFIER:
                            {
                                ast_block.AddStatement(new AstReturn(new AstVariableInt64(next.token_)));
                                EatTokenExpect(TokenType.SEMI_COLON);
                            } break;
                        }
                    }

                } break;

                case TokenType.IDENTIFIER:
                {
                    Token? next_ = EatToken();

                    if(next_.HasValue)
                    {
                        Token next = next_.Value;

                        switch(next.type_)
                        {
                            default:
                            {
                                throw new Exception("Unexpected token after identifier.");
                            }

                            case TokenType.COLON:
                            {
                                Token token_int64 = EatTokenExpect(TokenType.KEYWORD_INT64);
                                EatTokenExpect(TokenType.SEMI_COLON);
                                AstDeclarationInt64 ast_decl = new AstDeclarationInt64(token.token_);
                                ast_block.AddStatement(ast_decl);
                            } break;

                            case TokenType.EQUALS:
                            {
                                Token token_int64 = EatTokenExpect(TokenType.NUMBER);
                                EatTokenExpect(TokenType.SEMI_COLON);
                                AstAssignment ast_assign = new AstAssignment(new AstVariableInt64(token.token_), new AstLiteralInt64(token_int64.value_.value_int64));
                                ast_block.AddStatement(ast_assign);
                            } break;
                        }
                    } 
                    else
                    {
                        throw new Exception("No token after identifier inside a block.");
                    }
                } break;
            }
        }

        return ast_block;
    }

    private AstProcedure ParseProcedure()
    {
        Token? ident_token = EatToken();
        if (ident_token.HasValue)
        {
            // Eat Double Colons and Open parenthesis
            EatToken();
            EatToken();

            AstProcedure ast_proc = new AstProcedure(ident_token.Value.token_);

            while (true)
            {
                Token? token = EatToken();
                if (!token.HasValue)
                {
                    throw new Exception("Received null token while parsing procedure argument list.");
                }

                if (token.Value.type_ == TokenType.COMMA)
                {
                    continue;
                }
                else if (token.Value.type_ == TokenType.CLOSE_PARENTHESIS)
                {
                    break;
                }
                else if (token.Value.type_ == TokenType.IDENTIFIER)
                {
                    EatTokenExpect(TokenType.COLON);

                    Token? type = EatToken();
                    if (!type.HasValue)
                    {
                        throw new Exception("Syntax error in procedure's parameter list.");
                    }

                    switch (type.Value.type_)
                    {
                        default:
                        {
                            throw new Exception("Undefined parameter type in procedure signature");
                        }

                        case TokenType.KEYWORD_INT64:
                        {
                            AstParameterInt64 param_int64 = new AstParameterInt64(token.Value.token_);
                            ast_proc.AddParameter(param_int64);
                        }
                        break;
                    }
                }
                else
                {
                    throw new Exception("Syntax error in procedure's parameter list.");
                }
            }

            Token? past_proc_ = PeekToken();
            if (!past_proc_.HasValue)
            {
                throw new Exception("Expected { or -> after procedure declaration.");
            }

            Token past_proc = past_proc_.Value;
            switch(past_proc.type_)
            {
                default:
                {
                    throw new Exception("Expected { or -> after procedure declaration.");
                }

                case TokenType.ARROW:
                {
                    EatToken();
                    Token return_type = EatTokenExpect(TokenType.KEYWORD_INT64);
                    ast_proc.SetReturnType("Int64");
                } break;

                case TokenType.OPEN_BRACE: { } break;
            }

            AstBlock ast_block = ParseBlock();
            ast_proc.SetBlock(ast_block);

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

                        continue;
                    }
                }

                // @TODO : Check if is variable declaration
                {
                    EatToken();
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