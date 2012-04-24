using System;
using System.Collections.Generic;
using System.Text;
using MSBuildVersioning.Test;

namespace MSBuildVersioning.Debug
{
    /*
     * To debug the MSBuildVersioning code, you need to either:
     * (a) attach the Visual Studio debugger to NUnit (requires Visual Studio Professional); or
     * (b) call the desired code from here, and then run this project in the Visual Studio debugger
     * 
     * This is a template file. Copy this file, rename it to Program.cs, and then modify it as
     * desired.
     */

    public class Program
    {
        static void Main(string[] args)
        {
            new HgInfoProviderTest().GetRevisionIdTest();
        }
    }
}
