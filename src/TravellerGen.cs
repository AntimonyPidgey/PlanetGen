using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellerGen
{
    class TravellerGen
    {
        static void Main(string[] args)
        {
            string input = "";
            string input2 = "";
            StreamWriter file;

            Random gen = new Random();
            Boolean exit = false;
            while (exit == false)
            {
                try { Console.Clear(); }
                catch (Exception e) { Console.WriteLine(e.StackTrace); }
                Console.WriteLine("0) Exit");
                Console.WriteLine("1) Generate Sector");
                Console.WriteLine("2) Verbose Generate World");
                Console.WriteLine("3) Generate Verbose from WP");
                Console.WriteLine("Mode: ");
                input = Console.ReadLine();
                if (input.Equals("0")){
                    exit = true;
                }
                if (input.Equals("1"))
                {
                    Console.WriteLine("Sector Name: ");
                    string secName = Console.ReadLine();
                    try { Console.Clear(); }
                    catch (Exception e) { Console.WriteLine(e.StackTrace); }
                    Console.WriteLine("Density (1-100, Default 50):");
                    input = Console.ReadLine();
                    file = new System.IO.StreamWriter(secName + ".txt");
                    // 32 across, 40 down
                    file.WriteLine("Hex  Name                 UWP       Remarks               {Ix}   (Ex)    [Cx]   N     B  Z PBG W  A    Stellar        ");
                    file.WriteLine("---- -------------------- --------- --------------------- ------ ------- ------ ----- -- - --- -- ---- ---------------");
                    for (int i = 1; i <= 32; i++)
                    {
                        for (int j = 1; j <= 40; j++)
                        {
                            if (gen.Next(100) + 1 <= Convert.ToInt32(input))
                                file.WriteLine(new Hex("", null, i.ToString("D2"), j.ToString("D2"), "", gen).World.WorldProfile);
                        }
                    }
                    Console.WriteLine("Written to " + secName + ".txt. Press any key to return to menu...");
                    file.Flush();
                    file.Dispose();
                    Console.ReadLine();
                }
                if (input.Equals("2"))
                {
                    Console.WriteLine("Name The Sector: ");
                    input = Console.ReadLine();
                    Console.WriteLine("Name The System: ");
                    input2 = Console.ReadLine();
                    string backinput;
                    bool exitMode2 = false;
                    while (!exitMode2)
                    {
                        try { Console.Clear(); }
                        catch (Exception e) { Console.WriteLine(e.StackTrace); }
                        Hex hex = new Hex(input2, null, "01", "02", input, gen);
                        Console.WriteLine(hex.World.getDescription());
                        Console.WriteLine("\r\n------\r\nPress Enter to generate a new world. Enter 'save' to save this world to a file. Enter 'exit' to go back to the menu.");
                        backinput = Console.ReadLine();
                        if (backinput == "save")
                        {
                            file = new StreamWriter(hex.World.Name + " " + hex.World.Designation + ".txt");
                            file.Write(hex.World.getDescription());
                            file.Flush();
                            file.Dispose();
                            Console.WriteLine("Saved to " + hex.World.Name + " " + hex.World.Designation + ".txt. Press enter to generate another world, or enter 'exit' to go back to menu.");
                            backinput = Console.ReadLine();
                        }
                        if (backinput == "exit")
                        {
                            exitMode2 = true;
                        }
                    }
                }
                if (input.Equals("3"))
                {
                    Console.WriteLine("Paste the desired line from the sector list:");
                    input = Console.ReadLine();
                    try
                    {
                        int hexX = Convert.ToInt32("" + input[0] + input[1]);
                        int hexY = Convert.ToInt32("" + input[2] + input[3]);
                        string Name = "";
                        for (int i = 5; i < 25; i++) Name += input[i];
                        // WP 26-34
                        string Starport = "" + input[26];
                        int Size = Int32.Parse("" + input[27], System.Globalization.NumberStyles.HexNumber);
                        int Atmosphere = Int32.Parse("" + input[28], System.Globalization.NumberStyles.HexNumber);
                        int Hydrographic = Int32.Parse("" + input[29], System.Globalization.NumberStyles.HexNumber);
                        int Population = Int32.Parse("" + input[30], System.Globalization.NumberStyles.HexNumber);
                        int Government = Int32.Parse("" + input[31], System.Globalization.NumberStyles.HexNumber);
                        int Law = Int32.Parse("" + input[32], System.Globalization.NumberStyles.HexNumber);
                        int Tech = Int32.Parse("" + input[34], System.Globalization.NumberStyles.HexNumber);
                        char base1 = input[86];
                        char base2 = input[87];
                        char zone = input[89];
                        Boolean gasGiant = false;
                        if (input[93] == '1')
                            gasGiant = true;

                        Colony colony = new Colony(Name, hexX, hexY, Starport, Size, Atmosphere, Hydrographic, Population, Government, Law, Tech, base1, base2, zone, gasGiant);
                        Console.WriteLine(colony.getDescription());
                        Console.ReadLine();

                        Console.WriteLine("Enter 'save' to save this world to a file. Press Enter to go back to the menu.");
                        string backinput = Console.ReadLine();
                        if (backinput == "save")
                        {
                            file = new StreamWriter(colony.Name + " " + colony.Designation + ".txt");
                            file.Write(colony.getDescription());
                            file.Flush();
                            file.Dispose();
                            Console.WriteLine("Saved to " + colony.Name + " " + colony.Designation + ".txt. Press Enter to return to menu.");
                            backinput = Console.ReadLine();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Invalid String. Are you using the string from this generator?");
                        Console.ReadLine();
                    }
                }
            }
        }

        // Create Data
        private void generate()
        {

        }
    }
}
