using System;
using System.Collections.Generic;
using System.Text;

namespace Demo
{
    /// <summary>
    /// a attribute
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class Author : System.Attribute
    {
        /// <summary>
        /// the name of author
        /// </summary>
        private string name;
        /// <summary>
        /// version
        /// </summary>
        public double version;
        /// <summary>
        /// author constructor
        /// </summary>
        /// <param name="name">input name</param>
        public Author(string name)
        {
            this.name = name;
            version = 1.0;
        }
    }
}
