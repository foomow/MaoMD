using System;
using System.Collections.Generic;
using System.Text;

namespace Demo
{
    interface ITest
    {
        int tick { get; set; }
        string DoWork();

        bool IsTest(subns.TestStruct value);
      
    }
}
