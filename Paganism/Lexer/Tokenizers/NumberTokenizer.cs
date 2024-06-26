﻿using Paganism.Exceptions;
using Paganism.Lexer.Enums;
using System.Linq;

namespace Paganism.Lexer.Tokenizers
{
    public class NumberTokenizer : Tokenizer
    {
        public NumberTokenizer(string[] text, Lexer lexer) : base(lexer)
        {
            Text = text;
        }

        public string[] Text { get; }

        public char Current => Text[Line][Position];

        public override Token Tokenize()
        {
            int startPosition = Position;
            int startLine = Line;

            string savedLine = string.Empty;

            while (Line < Text.Length)
            {
                while (Position < Text[Line].Length)
                {
                    if (!char.IsDigit(Current) && Current != '.')
                    {
                        if (Current == '.' && savedLine.Contains('.'))
                        {
                            throw new LexerException($"Two decimal markers in number.", startLine, startPosition, Filepath);
                        }

                        Position--; //Cuz in next iterate in main lexer loop, lexer skip this non number token
                        return new Token(savedLine, startPosition, startLine, TokenType.Number);
                    }

                    savedLine += Current;
                    Position++;
                }

                Line++;
            }

            throw new LexerException($"Number is infinity", startLine, startPosition, Filepath);
        }
    }
}
