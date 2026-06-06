#if DEBUG
lavar.Execute(File.ReadAllBytes(@"./test.classes"));
#else
class Program
{
    static int Main()
    {

        byte[] raw = [(byte)'H',(byte)'o',(byte)'t',(byte)'!',160, 82, 176, 160, 117, 176, 160, 110, 176, 160, 116, 176, 160, 105, 176, 160, 109, 176, 160, 101, 176, 160, 32, 176, 160, 101, 176, 160, 114, 176, 160, 114, 176, 160, 111, 176, 160, 114, 176, 160, 44, 176, 160, 32, 176, 160, 84, 176, 160, 104, 176, 160, 101, 176, 160, 32, 176, 160, 102, 176, 160, 105, 176, 160, 108, 176, 160, 101, 176, 160, 32, 176, 160, 100, 176, 160, 97, 176, 160, 116, 176, 160, 97, 176, 160, 32, 176, 160, 105, 176, 160, 115, 176, 160, 32, 176, 160, 101, 176, 160, 109, 176, 160, 112, 176, 160, 116, 176, 160, 121, 176, 160, 32, 176, 160, 111, 176, 160, 114, 176, 160, 32, 176, 160, 99, 176, 160, 97, 176, 160, 110, 176, 160, 110, 176, 160, 111, 176, 160, 116, 176, 160, 32, 176, 160, 114, 176, 160, 101, 176, 160, 97, 176, 160, 100, 176, 160, 46, 176, 192];
        try
        {
            raw = File.ReadAllBytes(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            return (int)lavar.Execute(raw[14965039..]);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: unknown error.");
            while (true)
            {
                Console.Write('#');
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;
                else if (input.Equals("reboot"))
                {
                    Console.Clear();
                    Main();
                }
                else if (input.Equals("exit")) return 42;
                else if (input.Equals("len")) Console.WriteLine(raw.Length);
                else if (input.Equals("err")) Console.WriteLine(e.Message);
                else Console.WriteLine("Unsupported command.");
            }
        }
    }
}
#endif