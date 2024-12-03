﻿using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace PL
{
    public enum TokenType
    {
        UNKNOWN = 0,
        IDENTIFIER,
        NUMBER,
        STRING,
        COLON,
        DOUBLE_COLON,
        SEMI_COLON,
        ARROW,
        OPEN_BRACE,
        CLOSE_BRACE,
        OPEN_PARENTHESIS,
        CLOSE_PARENTHESIS,
        KEYWORD_INT64,
        KEYWORD_RETURN,
        END_OF_FILE,
    }

    public static class Keywords
    {
        private static Dictionary<string, TokenType> keywords_ = new Dictionary<string, TokenType> 
        {
            { "liczba", TokenType.KEYWORD_INT64 },
            { "cofnij", TokenType.KEYWORD_RETURN },
        };

        public static TokenType IsTokenKeyword(string token)
        {
            bool is_keyword = keywords_.TryGetValue(token, out TokenType token_keyword);
            if(is_keyword == false)
            {
                return TokenType.UNKNOWN;
            }
            else
            {
                return token_keyword;
            }
        }
    }
    

    public enum KeywordType
    {
        KEYWORD_RETURN,

    }

    // "Union" for variable values
    [StructLayout(LayoutKind.Explicit)]
    public struct TokenValue 
    {
        [FieldOffset(0)] public Int64  value_integer;
        [FieldOffset(0)] public double value_float;
    }

    public struct Token
    {
        public TokenType  type_;
        public TokenValue value_;
        public string     token_;

        public Token()
        {
            type_ = TokenType.UNKNOWN;
            token_ = "";
            value_ = new TokenValue(); 
        }
    }

    public class Lexer
    {
        private string input_;
        private int position_;
        
        public Lexer(string input)
        {
            input_ = input;
            position_ = 0;
        }

        private char PeekChar()
        {
            if (position_ >= input_.Length)
            {
                return '\0';
            }
            return input_[position_];
        }

        private char EatChar()
        {
            char _char = PeekChar();
            if(_char != '\0')
            {
                position_ += 1;
            }
            return _char;
        }

        private void EatWhitespaces()
        {
            while (Char.IsWhiteSpace(PeekChar())) {
                EatChar();
            }
        }

        private string ReadIdentifier()
        {
            string identifier = "";

            while(true)
            {
                char _char = PeekChar();

                if(Char.IsLetter(_char) || Char.IsDigit(_char))
                {
                    identifier += _char;
                    EatChar();
                }
                else
                {
                    break;
                }
            }

            return identifier;
        }

        private string ReadNumber()
        {
            string number = "";

            while (true)
            {
                char _char = PeekChar();

                if (Char.IsDigit(_char))
                {
                    number += _char;
                    EatChar();
                }
                else
                {
                    break;
                }
            }

            return number;
        }

        public Token NextToken()
        {
            EatWhitespaces();

            Token token = new Token();

            char _char = PeekChar();
            if(_char == '\0')
            {
                token.type_ = TokenType.END_OF_FILE;
            } 
            else if(Char.IsLetter(_char))
            {
                string identifier = ReadIdentifier();
                
                TokenType keyword_token = Keywords.IsTokenKeyword(identifier);
                if(keyword_token != TokenType.UNKNOWN)
                {
                    token.type_ = keyword_token;
                }
                else
                {
                    token.type_ = TokenType.IDENTIFIER;
                }

                token.token_ = identifier;
            }
            else if(Char.IsNumber(_char))
            {
                string number = ReadNumber();
                token.type_ = TokenType.NUMBER;
                token.value_.value_integer = Int64.Parse(number);
                token.token_ = number;
            }
            else if(_char == '{')
            {
                EatChar();
                token.type_ = TokenType.OPEN_BRACE;
                token.token_ = "{";
            }
            else if (_char == '}')
            {
                EatChar();
                token.type_ = TokenType.CLOSE_BRACE;
                token.token_ = "}";
            }
            else if (_char == '(')
            {
                EatChar();
                token.type_ = TokenType.OPEN_PARENTHESIS;
                token.token_ = "(";
            }
            else if (_char == ')')
            {
                EatChar();
                token.type_ = TokenType.CLOSE_PARENTHESIS;
                token.token_ = ")";
            }
            else if (_char == ':')
            {
                EatChar();

                char next = PeekChar();
                if (next == ':')
                {
                    EatChar();
                    token.type_ = TokenType.DOUBLE_COLON;
                    token.token_ = "::";
                }
                else
                {
                    token.type_ = TokenType.COLON;
                    token.token_ = ":";
                }
            }
            else if(_char == ';')
            {
                EatChar();
                token.type_ = TokenType.SEMI_COLON;
                token.token_ = ";";
            }
            else if (_char == '-')
            {
                EatChar();

                char next = PeekChar();
                if(next == '>')
                {
                    EatChar();
                    token.type_ = TokenType.ARROW;
                    token.token_ = "->";
                } 
                else if(Char.IsNumber(next))
                {
                    string number = "-" + ReadNumber();
                    token.type_ = TokenType.NUMBER;
                    token.value_.value_integer = Int64.Parse(number);
                    token.token_ = number;
                }
                else
                {
                    // Unknown token type
                    token.type_ = TokenType.UNKNOWN;
                }
            }
            else
            {
                EatChar();
                token.type_ = TokenType.UNKNOWN;
            }
            return token;
        }
    }
}
