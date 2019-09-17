using System;
using System.Collections.Generic;
using System.Text;

namespace maomdlib
{
    /// <summary>
    /// a type for markdown content
    /// </summary>
    public class MarkDownContent
    {
        /// <summary>
        /// the content
        /// </summary>
        private readonly string _content;
        /// <summary>
        /// length for content
        /// </summary>
        public int Length { get => _content.Length; }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="content">content</param>
        public MarkDownContent(string content)
        {
            _content = content;
        }
        /// <summary>
        /// constructor
        /// </summary>
        public MarkDownContent()
        {
            _content = "";
        }
        /// <summary>
        /// some operation
        /// </summary>
        /// <param name="a">a</param>
        public static implicit operator string(MarkDownContent a) => a._content;
        /// <summary>
        /// some operation
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns></returns>
        public static MarkDownContent operator +(MarkDownContent a, MarkDownContent b)
        {
            return new MarkDownContent(a.ToString() + b.ToString());
        }
        /// <summary>
        /// some operation
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns></returns>
        public static MarkDownContent operator +(string a, MarkDownContent b)
        {
            return new MarkDownContent(a + b.ToString());
        }
        /// <summary>
        /// some operation
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns></returns>
        public static MarkDownContent operator +(MarkDownContent a, string b)
        {
            return new MarkDownContent(a.ToString() + b);
        }
        /// <summary>
        /// some operation to add a new line
        /// </summary>
        /// <param name="a">a</param>
        /// <returns>content</returns>
        public static MarkDownContent operator ++(MarkDownContent a)
        {
            a = a + Environment.NewLine + Environment.NewLine;
            return a;
        }
        /// <summary>
        /// some operation add two new lines and some content
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns>content</returns>
        public static MarkDownContent operator *(MarkDownContent a, string b)
        {
            if (b != "")
                a = a + Environment.NewLine + Environment.NewLine;
            return a + b;
        }
        /// <summary>
        /// some operation add one new line and some content
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns>content</returns>
        public static MarkDownContent operator /(MarkDownContent a, string b)
        {
            if (b != "")
                a = a + Environment.NewLine;
            return a + b;
        }
        /// <summary>
        /// convertor to string
        /// </summary>
        /// <returns>string</returns>
        public override string ToString() { return _content; }
    }
}
