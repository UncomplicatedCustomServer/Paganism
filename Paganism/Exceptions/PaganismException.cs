﻿using System;

namespace Paganism.Exceptions
{
    public abstract class PaganismException : Exception
    {
        public abstract string Name { get; }

        public PaganismException()
        {
        }
        public PaganismException(string message, string name)
            : base($"{name} error: {message} ")
        {
        }


        public PaganismException(string message, int line, int position, string name)
            : base($"{name} error: {message} Line: {line + 1}, position: {position + 1}")
        {
        }
    }
}
