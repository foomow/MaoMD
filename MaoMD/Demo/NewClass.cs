using System;
using System.Collections.Generic;
using System.Text;

namespace Demo
{
    /// <summary>
    /// a new class,
    /// we use it to test
    /// how to make this 
    /// </summary>
    /// <remarks>
    /// this is remarks content
    /// </remarks>
    /// <seealso cref="Demo.DemoClass"/>
    /// <seealso cref="Demo.subns.ImpClass"/>
    [Author("foomow", version = 1.1)]
    [Serializable]
    class NewClass
    {
        /// <summary>
        /// a field
        /// </summary>
        private int _id;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id">input id</param>
        public NewClass(int id)
        {
            _id = id;            
        }
        /// <summary>
        /// a property
        /// </summary>
        public int Id { get => _id; set => _id = value; }
        /// <summary>
        /// this is a function
        /// </summary>
        /// <returns>return a string</returns>
        /// <remarks>some remarks</remarks>
        internal virtual string Afun() {
            return "";
        }
    }
    /// <summary>
    /// a struct
    /// </summary>    
    struct Date
    {
        /// <summary>
        /// year
        /// </summary>
        ushort year;
        /// <summary>
        /// month
        /// </summary>
        ushort month;
        /// <summary>
        /// day
        /// </summary>
        ushort day;
    }
}
