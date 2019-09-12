using System;
using System.Collections.Generic;
using System.Text;

namespace maomdlib
{
    public class MarkDownContent
    {
        private readonly string _content;
        public int Length { get => _content.Length; }

        public MarkDownContent(string content)
        {
            _content = content;
        }

        public MarkDownContent()
        {
            _content = "";
        }

        public static implicit operator string(MarkDownContent a) => a._content;

        public static MarkDownContent operator +(MarkDownContent a, MarkDownContent b)
        {
            return new MarkDownContent(a.ToString() + b.ToString());
        }

        public static MarkDownContent operator +(string a, MarkDownContent b)
        {
            return new MarkDownContent(a + b.ToString());
        }

        public static MarkDownContent operator +(MarkDownContent a, string b)
        {
            return new MarkDownContent(a.ToString() + b);
        }

        public static MarkDownContent operator ++(MarkDownContent a)
        {
            a = a + Environment.NewLine + Environment.NewLine;
            return a;
        }

        public static MarkDownContent operator *(MarkDownContent a, string b)
        {
            if (b != "")
                a = a + Environment.NewLine + Environment.NewLine;
            return a + b;
        }

        public static MarkDownContent operator /(MarkDownContent a, string b)
        {
            if (b != "")
                a = a + Environment.NewLine;
            return a + b;
        }

        public override string ToString() { return _content; }
    }
}
