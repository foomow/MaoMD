using System;
using System.Collections.Generic;
using System.Text;
using Demo.subns;

namespace Demo
{
    /// <summary>
    /// TestA
    /// </summary>
    internal class TestA : NewClass, ITest
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id">input value</param>
        public TestA(int id) : base(id)
        {
        }
        /// <summary>
        /// inherited value
        /// </summary>
        public int tick { get; set; }
        /// <summary>
        /// a work
        /// </summary>
        /// <returns>back message</returns>
        public string DoWork()
        {
            return "OK";
        }
        /// <summary>
        /// test message
        /// </summary>
        /// <param name="value">a struct</param>
        /// <returns>is it ok</returns>
        public bool IsTest(TestStruct value)
        {
            return false;
        }
    }
}
