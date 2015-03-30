// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Text.Formatting
{
    public static partial class Format
    {
        public const byte NoPrecision = Byte.MaxValue;

        // once we have a non allocating conversion from string to ReadOnlySpan<char>, we can remove this overload
        public static Format.Parsed Parse(string format)
        {
            if (format == null || format.Length == 0)
            {
                return default(Format.Parsed);
            }

            uint precision = NoPrecision;
            if (format.Length > 1)
            {
                if (!InvariantParser.TryParse(format, 1, format.Length - 1, out precision))
                {
                    throw new NotImplementedException("Unable to parse precision specification");
                }

                if (precision > Parsed.MaxPrecision)
                {
                    // TODO: this is a contract violation
                    throw new Exception(Strings.PrecisionValueOutOfRange);
                }
            }

            var specifier = format[0];
            return new Parsed(specifier, (byte)precision);
        }

        public static Format.Parsed Parse(ReadOnlySpan<char> format)
        {
            if (format.Length == 0)
            {
                return default(Format.Parsed);
            }

            uint precision = NoPrecision;
            if (format.Length > 1)
            {
                var span = format.Slice(1, format.Length - 1);

                if (!InvariantParser.TryParse(span, out precision))
                {
                    throw new NotImplementedException(Strings.UnableToParsePrecision);
                }

                if (precision > Parsed.MaxPrecision)
                {
                    // TODO: this is a contract violation
                    throw new Exception(Strings.PrecisionValueOutOfRange);
                }
            }

            // TODO: this is duplicated from above. It needs to be refactored
            var specifier = format[0];
            return new Parsed(specifier, (byte)precision);
        }

        public static Format.Parsed Parse(char format)
        {
            return new Parsed(format, NoPrecision);
        }

        //private static char CreateCharFromSymbol(Symbol symbol)
        //{
        //    switch (symbol)
        //    {
        //        case Symbol.B: return 'B';
        //        case Symbol.D: return 'D';
        //        case Symbol.E: return 'E';
        //        case Symbol.F: return 'F';
        //        case Symbol.G: return 'G';
        //        case Symbol.g: return 'g';
        //        case Symbol.N: return 'n';
        //        case Symbol.O: return 'O';
        //        case Symbol.P: return 'P';
        //        case Symbol.R: return 'R';
        //        case Symbol.t: return 't';
        //        case Symbol.X: return 'X';
        //        case Symbol.x: return 'x';
        //        default:
        //            throw new NotImplementedException();
        //    }
        //} 
        public struct Parsed
        {
            internal const byte MaxPrecision = 99;

            public char Symbol;
            public byte Precision;

            public Parsed(char symbol, byte precision = NoPrecision)
            {
                Symbol = symbol;
                Precision = precision;
            }
            public bool IsHexadecimal
            {
                get { return Symbol == 'X' || Symbol == 'x'; }
            }
            public bool HasPrecision
            {
                get { return Precision != NoPrecision; }
            }

            public bool IsDefault {
                get { return Symbol == (char)0; }
            }

            public static Format.Parsed HexUppercase = new Format.Parsed('X', Format.NoPrecision);
            public static Format.Parsed HexLowercase = new Format.Parsed('x', Format.NoPrecision);

            public static implicit operator Parsed(char symbol)
            {
                return new Parsed() { Symbol = symbol };
            }
        }
    }
}
