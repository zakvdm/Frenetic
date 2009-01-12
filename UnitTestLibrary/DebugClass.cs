using System;

namespace UnitTestLibrary
{
    // Written by blokeley
    /*class NUnitConsoleRunner
    {
        [STAThread]
        static void Main(string[] args)
        {
            NUnit.ConsoleRunner.Runner.Main(args);
        }
    }*/

    /*
     There is an easy way to debug NUnit tests from Visual C# Express, which has the advantages of:
* not invoking the NUnit GUI; and 
* not popping up an external console window; and
* piping output to Visual Studio's output window; and
* not requiring hacking the .csproj file...

Simply:

0. Add a reference to nunit-console-runner in your test assembly.

1. In your test assembly, make a class with the following one liner:

using System;

namespace MotorExampleTests
{

// Written by blokeley
class NUnitConsoleRunner
{
[STAThread]
static void Main(string[] args)
{
NUnit.ConsoleRunner.Runner.Main(args);
}
}
}

2. Open your test assembly's properties. For example, right-click on the assembly and select Properties.

3. On the Application tab, select Output Type: Windows Application; and Startup Object: NUNitConolseRunner (the file above).

4. On the Debug tab, enter the .csproj file name in Command Line Arguments; and browse to the folder of the .csproj file in Working Directory.

5. Save everything, set a breakpoint and run using F5 or the green arrow button.
     */
}
