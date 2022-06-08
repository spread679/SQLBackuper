using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBackuper
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Welcome on SQLBackuper, a simple console app to generate the necessaries files to backup your database.");
                Console.WriteLine("              _");
                Console.WriteLine("             | |");
                Console.WriteLine("             | |===( )   //////");
                Console.WriteLine("             |_|   |||  | o o|");
                Console.WriteLine("                    ||| ( c  )                  ____");
                Console.WriteLine("                     ||| \\= /                  ||   \\_");
                Console.WriteLine("                      ||||||                   ||     |");
                Console.WriteLine("                      ||||||                ...||__/|-\"");
                Console.WriteLine("                      ||||||             __|________|__");
                Console.WriteLine("                        |||             |______________|");
                Console.WriteLine("                        |||             || ||      || ||");
                Console.WriteLine("                        |||             || ||      || ||");
                Console.WriteLine("------------------------|||-------------||-||------||-||-------");
                Console.WriteLine("                        |__>            || ||      || ||");
                Console.WriteLine("");
                Console.WriteLine("\r\n");

                bool validPath;
                string fullPath;
                string fullPathScriptSQL;
                do
                {
                    Console.Write("[+] Enter the full path to create the files: ");
                    fullPath = Console.ReadLine();

                    validPath = System.IO.Directory.Exists(fullPath);
                    if (!validPath)
                        Console.WriteLine("[-] Invalid path, enter a valid one.");

                } while (!validPath);

                Console.WriteLine("[+] The path exists.\r\n");

                while (true)
                {
                    try
                    {
                        Console.WriteLine("[+] START SQL SCRIPT PROCESS.");
                        Console.Write("[+] Enter SQL script name (script.sql): ");
                        string scriptSQL = Console.ReadLine();
                        const string SQL_EXTENSION = ".sql";

                        if (string.IsNullOrEmpty(scriptSQL))
                            scriptSQL = $"script{SQL_EXTENSION}";

                        if (!scriptSQL.EndsWith(SQL_EXTENSION))
                            scriptSQL += SQL_EXTENSION;

                        Console.Write("[+] Enter the database name: ");
                        string database = Console.ReadLine();

                        if (string.IsNullOrEmpty(database))
                            throw new ArgumentException("Invalid database name, is empty.");

                        Console.Write("[+] Enter a title: ");
                        string title = Console.ReadLine();

                        StringBuilder sqlBuilder = new StringBuilder();

                        sqlBuilder.Append("USE [master]\r\n");
                        sqlBuilder.Append("GO\r\n\r\n");
                        sqlBuilder.Append($"BACKUP DATABASE [{database}] TO  DISK = N'{System.IO.Path.Combine(fullPath, string.Concat(database, ".bak"))}' WITH NOFORMAT,\r\n");
                        sqlBuilder.Append($"INIT, NAME = N'{title}', SKIP, NOREWIND, NOUNLOAD\r\nGO\r\n");

                        fullPathScriptSQL = System.IO.Path.Combine(fullPath, scriptSQL);
                        System.IO.File.WriteAllText(fullPathScriptSQL, sqlBuilder.ToString());
                        Console.WriteLine("[+] Created a SQL script to generate backup with the following information: ");
                        Console.WriteLine($"[+] Full path: {fullPathScriptSQL}");
                        Console.WriteLine($"[+] Database: {database}");
                        Console.WriteLine($"[+] Title: {title}\r\n");
                        break;
                    }
                    catch (Exception ex) { Console.WriteLine($"[-] {ex.Message}"); }
                }

                while (true)
                {
                    try
                    {
                        Console.WriteLine("[+] START BATCH PROCESS.");
                        Console.Write("[+] Enter batch script name (script.bat): ");
                        string batchScript = Console.ReadLine();
                        const string BATCH_EXTENSION = ".bat";

                        if (string.IsNullOrEmpty(batchScript))
                            batchScript = $"script{BATCH_EXTENSION}";

                        if (!batchScript.EndsWith(BATCH_EXTENSION))
                            batchScript += BATCH_EXTENSION;

                        string batchFullName = System.IO.Path.Combine(fullPath, batchScript);
                        Console.Write("[+] Enter SQL Server instance name: ");
                        string sqlServerInstance = Console.ReadLine();

                        StringBuilder batchBuilder = new StringBuilder();

                        batchBuilder.Append("@echo off\r\n");
                        batchBuilder.Append($"sqlcmd -E -S {sqlServerInstance} -i \"{fullPathScriptSQL}\" -o \"{batchFullName.TrimEnd(new char[] { '.', 'b', 'a', 't' })}.log\"\r\n");

                        System.IO.File.WriteAllText(batchFullName, batchBuilder.ToString());
                        Console.WriteLine("[+] Created a Batch script to generate backup with the following information: ");
                        Console.WriteLine($"[+] Full path: {batchFullName}");
                        Console.WriteLine($"[+] SQL Server Instance: {sqlServerInstance}\r\n");
                        break;
                    }
                    catch (Exception ex) { Console.WriteLine($"[-] {ex.Message}"); }
                }

                Console.Write($"[?] Open the location {fullPath}? (yes/no) ");
                string answer = Console.ReadLine();

                if (answer.ToLower() == "yes")
                    System.Diagnostics.Process.Start(fullPath);

                Console.WriteLine("\r\n[+] SQL Backuper ended, press enter to close the application.");
                Console.ReadLine();
            }
            catch (Exception ex) { Console.WriteLine($"[-] {ex.Message}"); }
        }
           
    }
}
