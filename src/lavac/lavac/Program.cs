using System.Diagnostics;

class Program
{
    static void Main(String[] args)
    {
#if DEBUG
        args = ["build","exe","test.lava","test.exe"];
#endif
        try
        {
            if (args[0] == "build")
            {
                if (args[1] == "classes")
                {
                    var data = args[2];
                    byte[] result = { 0xC0 };
                    try
                    {
                        data = File.ReadAllText(data);
                    }
                    catch
                    {

                    }
                    try
                    {
                        result = lavac.Compile(data);
                        File.WriteAllBytes(args[3], result);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Compile error:" + ex.Message);
                    }
                }
                else if (args[1] == "exe")
                {
                    var data = args[2];
                    byte[] result = { 0xC0 };
                    try
                    {
                        data = File.ReadAllText(data);
                    }
                    catch
                    {

                    }
                    try
                    {
                        var classes = String.Concat(args[3].TrimEnd(".exe"), ".classes");
                        result = lavac.Compile(data);
                        File.WriteAllBytes(classes, result);
                        Process.Start("cmd.exe", "/c copy /b runtime.exe + " + classes + ' ' + args[3] + " > nul").WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Compile error:" + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Error: Unsupported option.");
                }
            }
            else if (args[0] == "version") Console.WriteLine("Lava Compiler Snapshot-20260531-00\nCopyright (C) 2026 Nam_exx00");
            else Console.WriteLine("Error:Unsupported option.");
        }
        catch
        {
            Console.WriteLine("Lava Compiler Helper:\n\tlavac build classes <source code> <output> -> Compile to classes file.\n\tlavac build exe <source code> <output> -> Compile to Windows execute file.\n\t lavac version -> Show the compiler version.");
        }
#if DEBUG
        Process.Start("test.exe").WaitForExit();
#endif
    }
}