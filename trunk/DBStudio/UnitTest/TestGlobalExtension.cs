using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace SSCEViewer_UnitTest
{
    public static class TestGlobalExtension
    {
        public static void RecordTestMethod(this TestContext context)
        {
            Debug.WriteLine(context.TestName);
        }

        public static void RecordTestMethod()
        { 
            //If code enter here , in most scenario mean successful
            Debug.WriteLine("Test method mayby OK");
            Assert.Inconclusive("A method that does not return a value cannot be verified.You can check the output for more failure info");
        }

    }
}
