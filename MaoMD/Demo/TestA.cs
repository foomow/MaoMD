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
        /// a gclass
        /// </summary>
        public GClass<string,int,int> a;
       /// <summary>
       /// aaa class
       /// </summary>
        public List<NewClass> aaa;

        /// <summary>
        /// const fiele
        /// </summary>
        /// <remarks>haha</remarks>
        public const int fooc=3;
        /// <summary>
        /// readonly fiele
        /// </summary>
        /// <remarks>
        /// haha
        /// </remarks>
        public readonly NewClass foob;
        /// <summary>
        /// a foo value
        /// </summary>
        private int foo;
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
        public int Foo { get => foo; set => foo = value; }
        /// <summary>
        /// a implementation of tick
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
        /// <summary>
        /// test set method
        /// </summary>
        private void set_Foo(WeekDay day= WeekDay.Mon) => foo = -1;
        /// <summary>
        /// static method
        /// </summary>
        /// <param name="sclass">p0</param>
        /// <param name="a">p1</param>
        /// <param name="b">p2</param>
        /// <returns>a test calss</returns>
        internal static TestClass DoJob(SubClass sclass, int a, string b="bstr") => new TestClass();
        /// <summary>
        /// override method
        /// </summary>
        /// <returns>a string</returns>
        internal override string Afun()
        {
            return base.Afun();
        }
        /// <summary>
        /// protected method
        /// </summary>
        /// <returns>an int</returns>
        protected int pmethod()
        {
            return 1;
        }
        /// <summary>
        /// method with type parameters
        /// </summary>
        /// <typeparam name="T">the typeparameter</typeparam>
        /// <typeparam name="T1">another typeparameter</typeparam>
        /// <param name="fullname">test parameter</param>
        /// <returns>string return value</returns>
        public string GetNameOfType<T, T1>(bool fullname)
        {
            return "OK";
        }
    }
}
