﻿using Paganism.Lexer.Enums;
using Paganism.Lexer.Tokenizers;
using System.Collections.Generic;

namespace Paganism.Lexer
{
    public class Lexer
    {
        public Lexer(string[] text, string filepath)
        {
            var result = new string[text.Length + 1];

            for (int i = 0; i < text.Length; i++)
            {
                result[i] = text[i] += "\n";
            }

            result[result.Length - 1] = string.Empty;

            Text = result;
            Filepath = filepath;
        }

        public string[] Text { get; }

        public char Current => Text[Line][Position];

        public string Filepath { get; }

        public int Position { get; private set; }

        public int Line { get; private set; }

        public Token[] Run()
        {
            List<Token> tokens = new();

            var savedLine = string.Empty;

            while (Line < Text.Length)
            {
                while (Position < Text[Line].Length)
                {
                    if (Current == '/' && Text[Line].Length > 1 && Text[Line][Position + 1] == '/')
                    {
                        Line++;
                        Position = 0;
                        continue;
                    }

                    if (Current == '\t')
                    {
                        Position++;
                        continue;
                    }

                    if (Tokens.KeywordsType.TryGetValue(savedLine, out TokenType tokenType))
                    {
                        //It is check need because variable name can like 'format' and we check next symbol, if it is space or operator we add keyword to tokens
                        if (Current == '\0' || Current == ' ' || Current == '\n' || Tokens.OperatorsType.ContainsKey(Current.ToString()))
                        {
                            tokens.Add(new Token(savedLine, Position, Line, tokenType));
                            savedLine = string.Empty;
                            continue;
                        }

                        savedLine += Current;
                        Position++;
                        continue;
                    }
                    else if (Tokens.OperatorsType.ContainsKey(Current.ToString()))
                    {
                        var tokenizer = new OperatorTokenizer(Current.ToString(), tokens, savedLine, this);
                        var token = tokenizer.Tokenize();
                        Position = tokenizer.Position;
                        Line = tokenizer.Line;

                        savedLine = string.Empty;
                        tokens.Add(token);

                        Position++;
                        continue;
                    }
                    else if (Current == '\"')
                    {
                        var tokenizer = new StringTokenizer(Text, this);
                        var token = tokenizer.Tokenize();
                        Position = tokenizer.Position;
                        Line = tokenizer.Line;

                        savedLine = string.Empty;
                        tokens.Add(token);

                        continue;
                    }
                    else if (Current == '\'')
                    {
                        var tokenizer = new CharTokenizer(Text, this);
                        var token = tokenizer.Tokenize();
                        Position = tokenizer.Position;
                        Line = tokenizer.Line;

                        savedLine = string.Empty;
                        tokens.Add(token);

                        continue;
                    }

                    else if (char.IsDigit(Current) && (string.IsNullOrEmpty(savedLine) || string.IsNullOrWhiteSpace(savedLine)))
                    {
                        var tokenizer = new NumberTokenizer(Text, this);
                        var token = tokenizer.Tokenize();
                        Position = tokenizer.Position;
                        Line = tokenizer.Line;

                        savedLine = string.Empty;
                        tokens.Add(token);

                        Position++;
                        continue;
                    }
                    else if (Current == ' ')
                    {
                        if (string.IsNullOrEmpty(savedLine) || string.IsNullOrWhiteSpace(savedLine))
                        {
                            Position++;

                            continue;
                        }

                        var replacedLine = savedLine.Replace(" ", string.Empty);

                        if (Tokens.KeywordsType.ContainsKey(replacedLine))
                        {
                            savedLine = replacedLine;
                            continue;
                        }

                        tokens.Add(new Token(replacedLine, Position, Line, TokenType.Word));
                        savedLine = string.Empty;
                    }

                    savedLine += Current;
                    Position++;
                }

                if (!string.IsNullOrEmpty(savedLine) && !string.IsNullOrWhiteSpace(savedLine))
                {
                    tokens.Add(new Token(savedLine.Replace("\n", string.Empty).Replace(" ", string.Empty), Position, Line, TokenType.Word));
                }

                savedLine = string.Empty;
                Position = 0;
                Line++;
            }

            return tokens.ToArray();
        }
    }
}
