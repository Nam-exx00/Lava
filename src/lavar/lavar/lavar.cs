//using System.Security.Cryptography;
//using System.Windows.Forms;
using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

class lavar
{
    static int mems = 65536;
    static long[] mem = new long[65536];
    static long[] slots = new long[3];
    static long foundmem = 0;
    public static long Execute(byte[] classes)
    {
        if (classes[0] == 'H' && classes[1] == 'o' && classes[2] == 't' && classes[3] == '!')
        {
            classes = classes[4..];
            bool exit = false;
            int i = 0;
            while (true)
            {
                switch (classes[i])
                {
                    //Memory & Slot Manager
                    case 0xA0:  //Set slot 0 to char
                        {
                            i++;
                            slots[0] = classes[i];
                            break;
                        }
                    case 0xA1:  //Set slot 1 to char
                        {
                            i++;
                            slots[1] = classes[i];
                            break;
                        }
                    case 0xA2:  //Set slot 2 to char
                        {
                            i++;
                            slots[2] = classes[i];
                            break;
                        }
                    case 0xA3:  //Set slot 0 to mem addr
                        {
                            i++;
                            slots[0] = mem[classes[i]];
                            break;
                        }
                    case 0xA4:  //Set slot 1 to mem addr
                        {
                            i++;
                            slots[1] = mem[classes[i]];
                            break;
                        }
                    case 0xA5:  //Set slot 2 to mem addr
                        {
                            i++;
                            slots[2] = mem[classes[i]];
                            break;
                        }
                    case 0xA6:  //Set mem addr to slot 0
                        {
                            i++;
                            mem[classes[i]] = slots[0];
                            break;
                        }
                    case 0xA7:  //Set mem addr to slot 1
                        {
                            i++;
                            mem[classes[i]] = slots[1];
                            break;
                        }
                    case 0xA8:  //Set mem addr to slot 2
                        {
                            i++;
                            mem[classes[i]] = slots[2];
                            break;
                        }
                    case 0xA9:  //Set mem addr to char
                        {
                            mem[(int)slots[0]] = slots[1];
                            break;
                        }
                    case 0xAA:  //Get found memory
                        {
                            //Early source:
                            for (long j = 0; j < mem.LongLength; j++)
                            {
                                var k = mem.LongLength - 1 - j;
                                if (mem[k] != 0)
                                {
                                    return k + 1;
                                }
                            }
                            break;
                        }
                    case 0xAB:  //Set slot 0 to code
                        {
                            i++;
                            slots[0] = Execute(new byte[] { 72, 111, 116, 33, classes[i] });
                            break;
                        }
                    case 0xAC:  //Set slot 1 to code
                        {
                            i++;
                            slots[1] = Execute(new byte[] { 72, 111, 116, 33, classes[i] });
                            break;
                        }
                    case 0xAD:  //Set slot 2 to code
                        {
                            i++;
                            slots[2] = Execute(new byte[] { 72, 111, 116, 33, classes[i] });
                            break;
                        }
                    case 0xAE:  //Set slot 3 to code
                        {
                            i++;
                            slots[3] = Execute(new byte[] { 72, 111, 116, 33, classes[i] });
                            break;
                        }
                    case 0xAF:  //Cloner
                        {
                            i++;
                            switch (classes[i])
                            {
                                case 0xA0:  //Clone slot 0 to slot 1
                                    {
                                        slots[1] = slots[0];
                                        break;
                                    }
                                case 0xA1:  //Clone slot 0 to slot 2
                                    {
                                        slots[2] = slots[0];
                                        break;
                                    }
                                case 0xA2:  //Clone slot 1 to slot 0
                                    {
                                        slots[0] = slots[1];
                                        break;
                                    }
                                case 0xA3:  //Clone slot 1 to slot 2
                                    {
                                        slots[2] = slots[1];
                                        break;
                                    }
                                case 0xA4:  //Clone slot 2 to slot 0
                                    {
                                        slots[0] = slots[2];
                                        break;
                                    }
                                case 0xA5:  //Clone slot 2 to slot 1
                                    {
                                        slots[1] = slots[2];
                                        break;
                                    }
                                case 0xA6:  //Clone slot 0 to mem addr (next byte)
                                    {
                                        i++;
                                        mem[classes[i]] = slots[0];
                                        break;
                                    }
                                case 0xA7:  //Clone slot 1 to mem addr (next byte)
                                    {
                                        i++;
                                        mem[classes[i]] = slots[1];
                                        break;
                                    }
                                case 0xA8:  //Clone slot 2 to mem addr (next byte)
                                    {
                                        i++;
                                        mem[classes[i]] = slots[2];
                                        break;
                                    }
                                case 0xA9:  //Clone mem addr to slot 0
                                    {
                                        i++;
                                        slots[0] = mem[classes[i]];
                                        break;
                                    }
                                case 0xAA:  //Clone mem addr to slot 1
                                    {
                                        i++;
                                        slots[1] = mem[classes[i]];
                                        break;
                                    }
                                case 0xAB:  //Clone mem addr to slot 2
                                    {
                                        i++;
                                        slots[2] = mem[classes[i]];
                                        break;
                                    }
                            }
                            break;
                        }
                    //IO
                    case 0xB0:  //Output Slot 0
                        {
                            Console.Write((char)slots[0]);
                            break;
                        }
                    case 0xB1:  //Output Slot 1
                        {
                            Console.Write((char)slots[1]);
                            break;
                        }
                    case 0xB2:  //Output Slot 2
                        {
                            Console.Write((char)slots[2]);
                            break;
                        }
                    case 0xB3:  //Output Mem addr
                        {
                            Console.Write((char)mem[(int)slots[0]]);
                            break;
                        }
                    case 0xB4:  //Input Slot 0
                        {
                            slots[0] = Console.ReadKey(true).KeyChar;
                            break;
                        }
                    case 0xB5:  //Input Slot 1
                        {
                            slots[1] = Console.ReadKey(true).KeyChar;
                            break;
                        }
                    case 0xB6:  //Input Slot 2
                        {
                            slots[2] = Console.ReadKey(true).KeyChar;
                            break;
                        }
                    case 0xB7:  //Input Mem addr
                        {
                            mem[(int)slots[0]] = Console.ReadKey(true).KeyChar;
                            break;
                        }
                    //Program
                    case 0xC0:  //Exit
                        {
                            exit = true;
                            //for (int j = 0; j < mem.Length; j++) Console.Write("" + mem[j]);
                            break;
                        }
                    case 0xC1:  //Jump
                        {
                            i = (int)slots[0] - 1;
                            break;
                        }
                    case 0xC2:  //Decide Jump
                        {
                            if (slots[2] != 0) i = (int)slots[0] - 1;
                            break;
                        }
                    case 0xC3:  //Back
                        {
                            return slots[0];
                        }
                    //Decide
                    case 0xD0:  //Equals
                        {
                            if (slots[0] == slots[1])
                            {
                                slots[2] = 1;
                            }
                            else slots[2] = 0;
                            break;
                        }
                    case 0xD1:  //Bigger
                        {
                            if (slots[0] > slots[1])
                            {
                                slots[2] = 1;
                            }
                            else slots[2] = 0;
                            break;
                        }
                    case 0xD2:  //Smaller
                        {
                            if (slots[0] < slots[1])
                            {
                                slots[2] = 1;
                            }
                            else slots[2] = 0;
                            break;
                        }
                    //Calculate
                    case 0xE0:  //Plus
                        {
                            slots[2] = slots[0] + slots[1];
                            break;
                        }
                    case 0xE1:  //Subtract
                        {
                            slots[2] = slots[0] - slots[1];
                            break;
                        }
                    case 0xE2:  //Mulipily
                        {
                            slots[2] = slots[0] * slots[1];
                            break;
                        }
                    case 0xE3:  //Divide
                        {
                            slots[2] = slots[0] / slots[1];
                            break;
                        }
                    //System
                    //Potrial Extension
                    case 0x90:  //Read a line
                        {
                            var memaddr = slots[0];
                            var len = slots[1];
                            var get = Console.ReadLine();
                            for (int j = 0; j < len; j++)
                            {
                                try
                                {
                                    mem[(int)memaddr + j] = get[j];
                                }
                                catch
                                {
                                    break;
                                }
                            }
                            break;
                        }
                    case 0x91:  //File read (memstart , pathlen , outindex)
                        {
                            var start = (int)slots[0];
                            var pathbuffer = string.Empty;
                            for (int j = 0; j < slots[1]; j++)
                            {
                                pathbuffer += mem[start + j];
                            }
                            var get = System.IO.File.ReadAllBytes(pathbuffer);
                            var index = (int)slots[2];
                            for (int j = 0; j < get.Length; j++)
                            {
                                mem[index + j] = get[j];
                            }
                            break;
                        }

                    //Unsupported code
                    default:
                        {
                            throw new Exception("Unsupported code.");
                        }
                }
                if (exit) return 0;
                /*else if (mem.Length > (mems - 1024))
                {
                    mems += 65536;
                    Array.Resize(ref mem, mems);
                }*/
                else i++;
            }
        }
        else throw new Exception("Can't find the header.");
    }
}