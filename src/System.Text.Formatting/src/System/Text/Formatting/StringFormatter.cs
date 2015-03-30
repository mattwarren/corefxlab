﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Text.Formatting;

namespace System.Text.Formatting
{
    public class StringFormatter : IFormatter
    {
        byte[] _buffer;
        int _count;
        FormattingData _culture = FormattingData.InvariantUtf16;

        public StringFormatter() : this(64)
        {
            Clear();
        }

        public StringFormatter(int capacity)
        {
            _buffer = BufferPool.Shared.RentBuffer(capacity * 2);
        }

        public void Append(char character) {
            _buffer[_count++] = (byte)character;
            _buffer[_count++] = (byte)(character >> 8);
        }

        //TODO: this should use ByteSpan
        public void Append(string text)
        {
            foreach (char character in text)
            {
                Append(character);
            }
        }

        //TODO: this should use ByteSpan
        public void Append(ReadOnlySpan<char> substring)
        {
            for (int i = 0; i < substring.Length; i++)
            {
                Append(substring[i]);
            }
        }

        public void Clear()
        {
            _count = 0;
        }
        public override string ToString()
        {
            var text = Encoding.Unicode.GetString(_buffer, 0, _count);
            return text;
        }

        Span<byte> IFormatter.FreeBuffer
        {
            get { return new Span<byte>(_buffer, _count); }
        }

        public FormattingData FormattingData
        {
            get
            {
                return _culture;
            }
            set
            {
                _culture = value;
            }
        }

        void IFormatter.ResizeBuffer()
        {
            BufferPool.Shared.Enlarge(ref _buffer, _buffer.Length * 2);
        }
        void IFormatter.CommitBytes(int bytes)
        {
            _count += bytes;
        }
    }
}
