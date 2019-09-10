using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.subns
{
    /// <summary>
    /// test class
    /// </summary>
    class TestClass
    {
        /// <summary>
        /// a private field
        /// </summary>
        private readonly string _name;
        /// <summary>
        /// a property
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// test method in test class
        /// </summary>
        /// <returns>an interger value</returns>
        int TestMethod()
        {
            return 1;
        }
    }
    /// <summary>
    /// a test value type
    /// </summary>
    struct TestStruct
    {
        /// <summary>
        /// member x
        /// </summary>
        int x;
        /// <summary>
        /// member y
        /// </summary>
        int y;
    }
}
