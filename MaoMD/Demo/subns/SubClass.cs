using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.subns
{
    /// <summary>
    /// Class within sub namespace
    /// </summary>
    public class SubClass
    {
        /// <summary>
        /// an id private field member
        /// </summary>
        private int id;

        /// <summary>
        /// an Id property member
        /// </summary>
        public int Id { get => id; set => id = value; }
        /// <summary>
        /// a nested class
        /// </summary>
        class nestedclass
        {
            /// <summary>
            /// some name
            /// </summary>
            string name;
        }
    }
    /// <summary>
    /// a class implements an interface
    /// </summary>
    public class ImpClass : Demo.subns.subsubns.IDemo
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string comment { get; set; }
    }
}
