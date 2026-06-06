using System.Runtime.CompilerServices;

class lavac
{
    static List<byte> Output = [];
    static List<string> varname = [];
    static List<int> varaddr = [];
    static List<string> vardata = [];
    static List<string> vartype = [];
    static List<string> funcname = [];
    static List<string> funcargs = [];
    static List<string> funcdata = [];
    static List<bool> funcback = [];
    static string path = string.Empty;
    static string package = "main";
    static int foundmem = 0;
    public static byte[] Compile(string code)
    {
        Output.Add((byte)'H');
        Output.Add((byte)'o');
        Output.Add((byte)'t');
        Output.Add((byte)'!');
        Run(code);
        Output.Add(0xC0);
        return Output.ToArray();
    }
    static string Run(string code)
    {
        var lines = code.Split('\n');

        for (long i = 0; i < lines.LongLength; i++)
        {
            var backupp = path;
            var line = System.Text.RegularExpressions.Regex.Unescape(lines[i].Trim());
            bool safe = true;
            bool consted = false;
            bool publiced = false;
            if (string.IsNullOrEmpty(line) || line.StartsWith("//")) continue;
            if (line.EndsWith(';')) line = line[..(line.Length - 1)].Trim();
            if (line.StartsWith("public"))
            {
                publiced = true;
                path = string.Empty;
                line = line[6..].Trim();
            }
            if (line.StartsWith("unsafe"))
            {
                safe = false;
                line = line[6..].Trim();
            }
            if (line.StartsWith("const"))
            {
                consted = true;
                line = line[5..].Trim();
            }
            {
                string name;
                string arg = string.Empty;
                bool hasarg = false;
                try
                {
                    name = line[..(line.IndexOf('('))].Trim();
                    hasarg = true;
                }
                catch
                {
                    name = line.Trim();
                }
                try
                {
                    arg = line[(line.IndexOf('(') + 1)..(line.IndexOf(')'))];
                }
                catch
                {
                    hasarg = false;
                }
                bool brk = false;
                for (int j = 0; j < funcname.Count; j++)
                {
                    var k = funcname.Count - 1 - j;
                    if (name == funcname[k])
                    {
                        if (hasarg)
                        {
                            if (funcback[k]) return Run(funcargs[k] + '=' + arg + '\n' + funcdata[k]);
                            else Run(funcargs[k] + '=' + arg + '\n' + funcdata[k]);
                        }
                        else
                        {
                            if (funcback[k]) return Run(funcdata[k]);
                            else Run(funcdata[k]);
                        }
                        brk = true;
                        break;
                    }
                    else if (brk) break;
                }
                if (brk) break;
                for (int j = 0; j < varname.Count; j++)
                {
                    var k = varname.Count - 1 - j;
                    if (line == varname[k])
                    {
                        //Console.WriteLine("[DEBUG] line eq:"+(line == varname[j]));
                        if (varaddr[k] == -1)
                        {
                            return vartype[k] + vardata[k];
                        }
                        else
                        {
                            return "(Address)" + vartype[k] + varaddr[k];
                        }
                    }
                }
            }
            if (line.StartsWith('(') && line.EndsWith(')')) return Run(line[1..(line.Length - 1)]);
            else if (line.StartsWith('"') && line.EndsWith('"')) return "(String)" + line[1..(line.Length - 1)];
            else if (line.StartsWith('\'') && line.EndsWith('\''))
            {
                if (line.Length > 3)
                {
                    return Run('"' + line[1..(line.Length - 1)] + '"');
                }
                else
                {
                    return "(Char)" + line[1];
                }
            }
            else if (line.StartsWith("library")) Run(File.ReadAllText(@"./library/" + line[7..].Trim().Replace('.', '\\')));
            else if (line.StartsWith("package")) package = line[7..].Trim();
            else if (line.StartsWith("System.screen.msg.create"))
            {
                var result = Run(line[24..]);
                if (result.StartsWith("(String)"))
                {
                    result = result[8..];
                    for (int j = 0; j < result.Length; j++)
                    {
                        Output.Add(0xA0);
                        Output.Add((byte)result[j]);
                        Output.Add(0xB0);
                    }
                }
                else if (result.StartsWith("(Char)"))
                {
                    var c = result[6];
                    Output.Add(0xA0);
                    Output.Add((byte)c);
                    Output.Add(0xB0);
                }
                else if (result.StartsWith("(Byte)"))
                {
                    var c = result[6..];
                    for (int j = 0; j < c.Length; j++)
                    {
                        Output.Add(0xA0);
                        Output.Add((byte)(byte.Parse(c[j].ToString()) + 48));
                        Output.Add(0xB0);
                    }
                }
                else if (result.StartsWith("(Address)(Byte)"))
                {
                    var c = result[6..];
                    for (int j = 0; j < c.Length; j++)
                    {
                        Output.Add(0xA0);
                        Output.Add((byte)(byte.Parse(c[j].ToString()) + 48));
                        Output.Add(0xB0);
                    }
                }
                else if (result.StartsWith("(Address)(Bit["))
                {
                    int closeBracket = result.IndexOf(']');
                    int len = int.Parse(result[14..closeBracket]);
                    int addr = int.Parse(result[(closeBracket + 2)..]);

                    for (int j = 0; j < len; j++)
                    {
                        Output.Add(0xA3);
                        Output.Add((byte)(addr + j));
                        Output.Add(0xB0);
                    }
                }
                else if (result.StartsWith("(Address)(Char["))
                {
                    var strlen = Int128.Parse(result[15..(result.IndexOf(']'))]);
                    //Console.WriteLine("[DEBUG] strlen = " + strlen);
                    for (Int128 j = 0; j < strlen; j++)
                    {
                        Output.Add(0xA0);
                        //Console.WriteLine("[DEBUG] byte:"+ ;
                        Output.Add((byte)(byte.Parse(result[(result.IndexOf(']') + 2)..]) + (byte)j));
                        Output.Add(0xB3);
                    }
                }
                else throw new Exception("Unsupported type.");
            }
            else if (line.StartsWith("System.keyboard.reads"))
            {
                Output.Add(0xA0);
                Output.Add((byte)foundmem);
                Output.Add(0xA1);
                Output.Add(255);
                foundmem += 255;
                Output.Add(0x90);
                return "(Address)(Char)" + (foundmem - 255);
            }
            else if (line.StartsWith("System.keyboard.read"))
            {
                Output.Add(0xB4);
                Output.Add(0xA6);
                Output.Add((byte)foundmem);
                foundmem++;
                return "(Address)(Char)" + (foundmem - 1);
            }
            else if (line.StartsWith("goto"))
            {
                var result = Run(line[4..]);
                if (result.StartsWith("(Byte)"))
                {
                    Output.Add(0xA0);
                    Output.Add(byte.Parse(result[6..]));
                    Output.Add(0xC1);
                }
            }
            else if (line == "System.lab.line") return "(Char)\n";
            else if (line.StartsWith("class"))
            {
                Int128 depth = 0;
                line = line[5..].Trim();
                string name;
                try
                {
                    name = line[..(line.IndexOf('{'))].Trim();
                }
                catch
                {
                    name = line.Trim();
                }
                if (line.EndsWith('}')) //void main() {...}
                {
                    var d = line[(line.IndexOf('{') + 1)..(line.Length - 1)];
                    Run(d);
                }
                else
                {
                    string extract = "";
                    // 找到第一个 '{'
                    int startIndex = line.IndexOf('{');
                    if (startIndex >= 0)
                    {
                        extract = line[(startIndex + 1)..];
                    }
                    else
                    {
                        // 在当前行没找到 '{'，继续找下一行
                        while (true)
                        {
                            i++;
                            if (i >= lines.Length) break;
                            line = lines[i];
                            startIndex = line.IndexOf('{');
                            if (startIndex >= 0)
                            {
                                extract = line[(startIndex + 1)..];
                                break;
                            }
                        }
                    }

                    // 继续读取直到匹配的 '}'
                    while (true)
                    {
                        i++;
                        if (i >= lines.Length) break;

                        string currentLine = lines[i];

                        // 统计当前行的花括号
                        foreach (char c in currentLine)
                        {
                            if (c == '{') depth++;
                            else if (c == '}')
                            {
                                if (depth > 0) depth--;
                                else
                                {
                                    // 找到匹配的结束括号
                                    int closingIndex = currentLine.IndexOf('}');
                                    if (closingIndex >= 0)
                                    {
                                        extract += currentLine[..closingIndex];
                                    }
                                    goto EndExtract;
                                }
                            }
                        }

                        // 如果还没结束，添加整行
                        extract += currentLine + "\n";
                    }

                EndExtract:
                    var backup = path;
                    path += name + '.';
                    Run(extract);
                    path = backup;
                }
            }
            else if (line.StartsWith("void"))
            {
                Int128 depth = 0;
                bool main = false;
                line = line[4..].Trim();
                string name;
                try
                {
                    name = line[..(line.IndexOf('('))].Trim();
                }
                catch
                {
                    name = line.Trim();
                }
                if (path == string.Empty || publiced)
                {
                    if (name == package && publiced) main = true; else funcname.Add(name);
                }
                else
                {
                    if (name == package && publiced) main = true; else funcname.Add(path + name);
                }
                try { if (main) ; else funcargs.Add(line[(line.IndexOf('(') + 1)..(line.IndexOf(')'))]); }
                catch { funcargs.Add(string.Empty); }

                //voiddata:
                if (line.EndsWith('}')) //void main() {...}
                {
                    var d = line[(line.IndexOf('{') + 1)..(line.Length - 1)];
                    if (name == package)
                    {
                        //Console.WriteLine("[DEBUG] main function found, executing body");
                        Run(d);
                    }
                    else funcdata.Add(d);
                    //Console.WriteLine("[DEBUG]" + main.ToString());
                    //Console.WriteLine("[DEBUG] geted data:" + d);
                }
                else
                {
                    string extract = "";

                    // 找到第一个 '{'
                    int startIndex = line.IndexOf('{');
                    if (startIndex >= 0)
                    {
                        extract = line[(startIndex + 1)..];
                    }
                    else
                    {
                        // 在当前行没找到 '{'，继续找下一行
                        while (true)
                        {
                            i++;
                            if (i >= lines.Length) break;
                            line = lines[i];
                            startIndex = line.IndexOf('{');
                            if (startIndex >= 0)
                            {
                                extract = line[(startIndex + 1)..];
                                break;
                            }
                        }
                    }

                    // 继续读取直到匹配的 '}'
                    while (true)
                    {
                        i++;
                        if (i >= lines.Length) break;

                        string currentLine = lines[i];

                        // 统计当前行的花括号
                        foreach (char c in currentLine)
                        {
                            if (c == '{') depth++;
                            else if (c == '}')
                            {
                                if (depth > 0) depth--;
                                else
                                {
                                    // 找到匹配的结束括号
                                    int closingIndex = currentLine.IndexOf('}');
                                    if (closingIndex >= 0)
                                    {
                                        extract += currentLine[..closingIndex];
                                    }
                                    goto EndExtract;
                                }
                            }
                        }

                        // 如果还没结束，添加整行
                        extract += currentLine + "\n";
                    }

                EndExtract:
                    if (main) Run(extract);
                    else
                    {
                        funcdata.Add(extract);
                        funcback.Add(false);
                    }
                }
            }
            else if (line.StartsWith("return"))
            {
                Int128 depth = 0;
                bool main = false;
                line = line[6..].Trim();
                string name;
                try
                {
                    name = line[..(line.IndexOf('('))].Trim();
                }
                catch
                {
                    name = line.Trim();
                }
                if (path == string.Empty || publiced)
                {
                    if (name == package && publiced) main = true; else funcname.Add(name);
                }
                else
                {
                    if (name == package && publiced) main = true; else funcname.Add(path + name);
                }
                try { if (main) ; else funcargs.Add(line[(line.IndexOf('(') + 1)..(line.IndexOf(')'))]); }
                catch { funcargs.Add(string.Empty); }

                //voiddata:
                if (line.EndsWith('}')) //void main() {...}
                {
                    var d = line[(line.IndexOf('{') + 1)..(line.Length - 1)];
                    if (name == package)
                    {
                        //Console.WriteLine("[DEBUG] main function found, executing body");
                        Run(d);
                    }
                    else funcdata.Add(d);
                    //Console.WriteLine("[DEBUG]" + main.ToString());
                    //Console.WriteLine("[DEBUG] geted data:" + d);
                }
                else
                {
                    string extract = "";

                    // 找到第一个 '{'
                    int startIndex = line.IndexOf('{');
                    if (startIndex >= 0)
                    {
                        extract = line[(startIndex + 1)..];
                    }
                    else
                    {
                        // 在当前行没找到 '{'，继续找下一行
                        while (true)
                        {
                            i++;
                            if (i >= lines.Length) break;
                            line = lines[i];
                            startIndex = line.IndexOf('{');
                            if (startIndex >= 0)
                            {
                                extract = line[(startIndex + 1)..];
                                break;
                            }
                        }
                    }

                    // 继续读取直到匹配的 '}'
                    while (true)
                    {
                        i++;
                        if (i >= lines.Length) break;

                        string currentLine = lines[i];

                        // 统计当前行的花括号
                        foreach (char c in currentLine)
                        {
                            if (c == '{') depth++;
                            else if (c == '}')
                            {
                                if (depth > 0) depth--;
                                else
                                {
                                    // 找到匹配的结束括号
                                    int closingIndex = currentLine.IndexOf('}');
                                    if (closingIndex >= 0)
                                    {
                                        extract += currentLine[..closingIndex];
                                    }
                                    goto EndExtract;
                                }
                            }
                        }

                        // 如果还没结束，添加整行
                        extract += currentLine + "\n";
                    }

                EndExtract:
                    if (main) Run(extract);
                    else
                    {
                        funcdata.Add(extract);
                        funcback.Add(true);
                    }
                }
            }
            else if (line.StartsWith("if"))
            {
                Int128 depth = 0;
                line = line[2..];
                string result;
                try
                {
                    result = Run(line[..(line.IndexOf('{'))]);
                }
                catch { result = Run(line); }
                string extract = string.Empty;

                // 找到循环体
                if (line.Contains('{') && line.EndsWith('}')) // 单行: while(...) {...}
                {
                    extract = line[(line.IndexOf('{') + 1)..(line.Length - 1)];
                }
                else
                {
                    // 找 '{'
                    int bracePos = line.IndexOf('{');
                    if (bracePos >= 0)
                    {
                        extract = line[(bracePos + 1)..];
                    }
                    else
                    {
                        // 当前行没找到 '{'，继续找下一行
                        while (true)
                        {
                            i++;
                            if (i >= lines.Length) break;
                            line = lines[i];
                            bracePos = line.IndexOf('{');
                            if (bracePos >= 0)
                            {
                                extract = line[(bracePos + 1)..];
                                break;
                            }
                        }
                    }

                    // 继续读取直到匹配的 '}'
                    while (true)
                    {
                        i++;
                        if (i >= lines.Length) break;

                        string currentLine = lines[i];

                        // 逐字符统计花括号
                        for (int j = 0; j < currentLine.Length; j++)
                        {
                            if (currentLine[j] == '{')
                            {
                                depth++;
                            }
                            else if (currentLine[j] == '}')
                            {
                                if (depth > 0)
                                {
                                    depth--;
                                }
                                else
                                {
                                    // 找到匹配的结束括号
                                    extract += currentLine[..j];
                                    goto EndWhile;
                                }
                            }
                        }

                        // 还没结束，添加整行
                        extract += currentLine + "\n";
                    }
                }

            EndWhile:
                if (result == "(Bool)1")
                {
                    Run(extract);
                }
                else if (result.StartsWith("(Address)(Bool)"))
                {
                    int addr = int.Parse(result[15..]);

                    // 加载布尔值到 slot2
                    Output.Add(0xA5);
                    Output.Add((byte)addr);

                    // 取反：比较 test == 0
                    Output.Add(0xA0);
                    Output.Add(0x00);
                    Output.Add(0xD0);  // slot2 = (test == 0)

                    // 记录跳转指令的位置
                    int jumpPos = Output.Count;
                    Output.Add(0xA0);
                    Output.Add(0x00);  // 占位符
                    Output.Add(0xC2);

                    // 编译 if 体
                    Run(extract);

                    // 计算 if 体结束后的地址，回填跳转目标
                    int afterIfBody = Output.Count;
                    Output[jumpPos + 1] = (byte)afterIfBody;
                }
                //Run(extract);
                // 保存循环体
            }
            else if (line.StartsWith("while"))
            {
                string whileLine = line[5..].Trim();
                string condition;
                int bracePos = whileLine.IndexOf('{');

                if (bracePos >= 0)
                {
                    condition = whileLine[..bracePos].Trim();
                }
                else
                {
                    condition = whileLine;
                }

                // 记录循环开始的位置
                int loopStart = Output.Count;

                string result = Run(condition);

                string extract = "";
                int balance = 0;

                // 提取循环体（与 if 相同的逻辑）
                if (bracePos >= 0 && whileLine.Contains('{'))
                {
                    string remaining = whileLine[(bracePos + 1)..];
                    if (remaining.Contains('}') && !remaining[..remaining.IndexOf('}')].Contains('{'))
                    {
                        extract = remaining[..remaining.IndexOf('}')].Trim();
                    }
                    else
                    {
                        extract = remaining;
                        balance = 1;
                        for (int j = 0; j < extract.Length; j++)
                        {
                            if (extract[j] == '{') balance++;
                            else if (extract[j] == '}') balance--;
                        }
                        while (balance > 0 && i + 1 < lines.Length)
                        {
                            i++;
                            string nextLine = lines[i];
                            for (int j = 0; j < nextLine.Length; j++)
                            {
                                if (nextLine[j] == '{') balance++;
                                else if (nextLine[j] == '}')
                                {
                                    balance--;
                                    if (balance == 0)
                                    {
                                        extract += "\n" + nextLine[..j];
                                        goto EndWhile;
                                    }
                                }
                            }
                            extract += "\n" + nextLine;
                        }
                    }
                }
                else
                {
                    while (i + 1 < lines.Length)
                    {
                        i++;
                        string nextLine = lines[i];
                        if (nextLine.TrimStart().StartsWith('{'))
                        {
                            string content = nextLine.TrimStart()[1..];
                            balance = 1;
                            for (int j = 0; j < content.Length; j++)
                            {
                                if (content[j] == '{') balance++;
                                else if (content[j] == '}')
                                {
                                    balance--;
                                    if (balance == 0)
                                    {
                                        extract = content[..j];
                                        goto EndWhile;
                                    }
                                }
                            }
                            extract = content;
                            while (balance > 0 && i + 1 < lines.Length)
                            {
                                i++;
                                nextLine = lines[i];
                                for (int j = 0; j < nextLine.Length; j++)
                                {
                                    if (nextLine[j] == '{') balance++;
                                    else if (nextLine[j] == '}')
                                    {
                                        balance--;
                                        if (balance == 0)
                                        {
                                            extract += "\n" + nextLine[..j];
                                            goto EndWhile;
                                        }
                                    }
                                }
                                extract += "\n" + nextLine;
                            }
                            break;
                        }
                        else
                        {
                            extract = nextLine;
                            goto EndWhile;
                        }
                    }
                }

            EndWhile:
                extract = extract.Trim();

                if (result == "(Bool)1")
                {
                    // 条件恒为真，生成无限循环
                    int loopBodyStart = Output.Count;
                    Run(extract);
                    // 无条件跳回循环开始
                    Output.Add(0xA0);
                    Output.Add((byte)loopStart);
                    Output.Add(0xC1);
                }
                else if (result.StartsWith("(Address)(Bool)"))
                {
                    int addr = int.Parse(result[15..]);

                    // 加载布尔值
                    Output.Add(0xA5);
                    Output.Add((byte)addr);
                    // 取反：比较 test == 0
                    Output.Add(0xA0);
                    Output.Add(0x00);
                    Output.Add(0xD0);

                    // 如果条件为假，跳出循环
                    int jumpPos = Output.Count;
                    Output.Add(0xA0);
                    Output.Add(0x00);
                    Output.Add(0xC2);

                    // 编译循环体
                    Run(extract);

                    // 跳回循环开始
                    Output.Add(0xA0);
                    Output.Add((byte)loopStart);
                    Output.Add(0xC1);

                    // 回填跳出循环的地址
                    Output[jumpPos + 1] = (byte)Output.Count;
                }
            }

            else if (line.StartsWith("var")) Run(line[3..]);
            else if (line.StartsWith("Char") || line.StartsWith("Byte")) Run(line[4..]);
            else if (line.StartsWith("Int32")) Run(line[5..]);
            else if (line.StartsWith("Int64")) Run(line[5..]);
            else if (line.StartsWith("String") || line.StartsWith("Int128")) Run(line[6..]);
            else if (line == "true")
            {
                return "(Bool)1";
            }
            else if (line == "false")
            {
                return "(Bool)0";
            }
            else if (line == "Lab.line") return "(Char)\n";

            /*else if (line.Contains("equals"))
            {
                try
                {
                    var parts = line.Split("equals",2);
                    var left = Run(parts[0]);
                    var right = Run(parts[1]);
                    if (left.StartsWith("(Char)") && right.StartsWith("(Char)"))
                    {
                        
                    }
                }
                catch
                {
                    
                }
            }*/
            else if (line.Contains('='))
            {
                try
                {
                    var parts = line.Split('=', 2);
                    string name;
                    if (publiced) name = parts[0].Trim();
                    else name = path + parts[0].Trim();
                    var data = Run(parts[1]);
                    varname.Add(name);
                    if (consted)
                    {
                        varaddr.Add(-1);
                        if (data.StartsWith("(Char)"))
                        {
                            vartype.Add("(Char)");
                            vardata.Add(data[6].ToString());
                        }
                    }
                    else
                    {
                        vardata.Add(string.Empty);
                        if (data.StartsWith("(Char)"))
                        {
                                Output.Add(0xA0);
                                Output.Add((byte)foundmem);
                                Output.Add(0xA1);
                                Output.Add((byte)data[6]);
                                vartype.Add("(Char)");
                                Output.Add(0xA9);
                                varaddr.Add(foundmem);
                                foundmem++;
                        }
                        else if (data.StartsWith("(String)"))
                        {
                            var str = data[8..];
                            varaddr.Add(foundmem);
                            vartype.Add("(Char[" + str.Length + "])");
                            for (int j = 0; j < str.Length; j++)
                            {
                                Output.Add(0xA0);
                                Output.Add((byte)foundmem);
                                Output.Add(0xA1);
                                Output.Add((byte)str[j]);
                                Output.Add(0xA9);
                                foundmem++;
                            }

                        }
                        else if (data.StartsWith("(Byte)"))
                        {
                            var str = data[6..];  // str = "12"
                            varaddr.Add(foundmem);
                            vartype.Add("(Bit[" + str.Length + "])");  // 应该是 "(Bit[2])"
                            for (int j = 0; j < str.Length; j++)
                            {
                                Output.Add(0xA0);
                                Output.Add((byte)foundmem);
                                Output.Add(0xA1);
                                Output.Add((byte)str[j]);  // 存储 '1', '2' 的 ASCII 码
                                Output.Add(0xA9);
                                foundmem++;
                            }
                        }
                        else if (data.StartsWith("(Bool)"))
                        {
                            Output.Add(0xA0);
                            Output.Add((byte)foundmem);
                            Output.Add(0xA1);
                            if (data[6] == '1') Output.Add(0x01);
                            else Output.Add(0x00);
                            vartype.Add("(Bool)");
                            Output.Add(0xA9);
                            varaddr.Add(foundmem);
                            foundmem++;
                        }
                        else if (data.StartsWith("(Address)(Char)"))
                        {
                            vartype.Add("(Char)");
                            varaddr.Add(int.Parse(data[15..]));
                        }
                    }
                }
                catch { }
            }
            else
            {
                try
                {
                    byte.Parse(line);
                    return "(Byte)" + line;
                }
                catch
                {
                    try
                    {
                        int.Parse(line);
                        return "(Int32)" + line;
                    }
                    catch
                    {
                        try
                        {
                            long.Parse(line);
                            return "(Int64)" + line;
                        }
                        catch
                        {
                            try
                            {
                                Int128.Parse(line);
                                return "(Int128)" + line;
                            }
                            catch
                            {
                                if (safe) throw new Exception("Unsupported syntax.");
                                else return line;
                            }
                        }
                    }
                }
            }
        }

        return string.Empty;
    }
}