using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellerGen
{
    // Represents a colony within a hex.
    class Colony
    {
        Random gen = new Random();
        # region Universal World Profile Properties
        private Hex hex { get; set; }
        // Initiates all properties representing parts of the hex's universal world profile.
        public string Subsector { get; set; }
        public string Designation { get; set; }
        public string Name { get; set; }

        public string HexX { get; set; }
        public string HexY { get; set; }
        
        public int Starport { get; set; }
        public int Size { get; set; }
        public int AtmosphereType { get; set; }
        public int HydrographicPercentage { get; set; }
        public int Population { get; set; }
        public int Government { get; set; }
        public int LawLevel { get; set; }
        // ---
        public int TechLevel { get; set; }

        public List<char> Bases = new List<char>();
        public List<string> TradeCodes = new List<string>();
        public List<string[]> Factions = new List<string[]>();
        public List<string[]> Social = new List<string[]>();
        public List<string> Gear = new List<string>();

        public char TravelZone = 'G';

        // Provides a property for getting the entire world profile as a string.
        // 0103 Irkigkhan            C9C4733-9 Fl                                                CN   001 1
        string buildProfile;
        public string WorldProfile { 
            get {
                if (!Name.Equals(""))
                    buildProfile = HexX + HexY + " " + Name.PadRight(20, ' ') + " ";
                else
                    buildProfile = HexX + HexY + " " + Designation.PadRight(20, ' ') + " ";

                if (Starport <= 2) buildProfile += 'X';
                else if (Starport <= 4) buildProfile += 'E';
                else if (Starport <= 6) buildProfile += 'D';
                else if (Starport <= 8) buildProfile += 'C';
                else if (Starport <= 10) buildProfile += 'B';
                else if (Starport >= 11) buildProfile += 'A';
                buildProfile += Size.ToString("X") + AtmosphereType.ToString("X") + HydrographicPercentage.ToString("X") + Population.ToString("X") + Government.ToString("X") + LawLevel.ToString("X") + "-";
                if (TechLevel > 15)
                    TechLevel = 15;
                buildProfile += TechLevel.ToString("X") + " ";

                string remarkTemp = "";
                if (TradeCodes != null)
                {
                    for (int i = 0; i < TradeCodes.Count; i++)
                    {
                        remarkTemp += TradeCodes.ElementAt(i) + " ";
                    }
                }
                buildProfile += remarkTemp.PadRight(21);
                //buildProfile += "----------------------------";
                buildProfile += "                             ";

                string baseTemp = "";
                if (Bases!=null){
                    if (Bases.Count>2){
                        for (int i = 0; i < 2; i++)
                        {
                            baseTemp += Bases[i];
                        }
                    }
                    else
                        for (int i = 0; i < Bases.Count; i++)
                        {
                            baseTemp += Bases[i];
                        }
                }
                buildProfile += baseTemp.PadRight(2);
                if (TravelZone != 'G')
                {
                    buildProfile += " " + TravelZone + " ";
                }
                else
                    buildProfile += "   ";

                if (GasGiant)
                    buildProfile += "001 1";
                else
                    buildProfile += "000 1";
                return buildProfile;
            }
        }
        #endregion

        // Other Planetary Data
        public int Temperature { get; set; }
        public int BerthingCost { get; set; }
        public int TrueSize { get; set; }
        public int TrueTemperature { get; set; }
        public int TrueHydrographics { get; set; }
        private string TrueGravVal = null;
        public string TrueGrav { get { return String.Format("{0:0.00}", TrueGravVal); } set { TrueGravVal = value; } }
        private string pressureVal;
        public string Pressure { get { return String.Format("{0:0.00}", pressureVal); } }
        public bool GasGiant { get; set; }

        // Generates the initial Colony. A designation will be randomly generated if not entered.
        public Colony(string _Name, string _HexX, string _HexY, string _Designation, string _Subsector, Hex _hex, Random _gen)
        {
            if (!_gen.Equals(null)) gen = _gen;
            BerthingCost = -1;
            TrueSize = -1;
            hex = _hex;
            Name = _Name;
            HexX = _HexX;
            HexY = _HexY;
            Subsector = _Subsector;
            if (_Designation != null) Designation = _Designation;
            else Designation = "" + getChar(gen.Next(26)) + getChar(gen.Next(26)) + getChar(gen.Next(26)) + getChar(gen.Next(26)) + getChar(gen.Next(26)) + "-" + gen.Next(10) + gen.Next(10) + gen.Next(10) + gen.Next(10) + gen.Next(10);

            GasGiant = hex.GasGiant;
            // Size (2d6-2)
                Size = gen.Next(6) + 1 + gen.Next(6) + 1 - 2;
            // Atmosphere (2d6 - 7 + Size)
                AtmosphereType = gen.Next(6) + 1 + gen.Next(6) + 1 - 7 + Size;
                if (AtmosphereType < 0) AtmosphereType = 0;
            // Temperature (2d6 + Various Modifiers)
                int tempTemp;
                tempTemp = gen.Next(6) + 1 + gen.Next(6) + 1;
                // Apply a modifier based on atmosphere type.
                if (AtmosphereType == 2 || AtmosphereType == 3) tempTemp -= 2;
                if (AtmosphereType == 4 || AtmosphereType == 5 || AtmosphereType == 14) tempTemp -= 1;
                if (AtmosphereType == 8 || AtmosphereType == 9) tempTemp += 1;
                if (AtmosphereType == 10 || AtmosphereType == 13 || AtmosphereType == 15) tempTemp += 2;
                if (AtmosphereType == 11 || AtmosphereType == 12) tempTemp += 6;
                // Apply a modifier based on position in the habitable zone (+4, 0, or -4 randomly)
                switch (gen.Next(3))
                {
                    case 0:
                        tempTemp -= 4;
                        break;
                    case 1:
                        tempTemp += 0;
                        break;
                    case 2:
                        tempTemp += 4;
                        break;
                }
                // If the atmosphere type is 0 or 1, the temperature swings between roasting and frozen during the day and night respectively.
                if (AtmosphereType == 0 || AtmosphereType == 1) tempTemp = 0;
                Temperature = tempTemp;
            // Hydrographics (2d6 - 7 + Size + Modifiers)
                tempTemp = gen.Next(6) + 1 + gen.Next(6) + 1 - 7 + Size;
                if (AtmosphereType == 0 || AtmosphereType == 1 || AtmosphereType == 10 || AtmosphereType == 11 || AtmosphereType == 12) tempTemp -= 4;
                // if AtmosphereType is NOT 13, add modifiers for temperature too.
                if (AtmosphereType != 13) {
                    if (Temperature == 10 || Temperature == 11) tempTemp -= 2;
                    if (Temperature >= 12) tempTemp -= 6;
                }
                // Size 0 or 1 sets hydrographics to 0.
                if (Size < 2) tempTemp = 0;
                HydrographicPercentage = tempTemp;
                if (HydrographicPercentage < 0) HydrographicPercentage = 0;
                if (HydrographicPercentage > 10) HydrographicPercentage = 10;
            // Population (2d6-2) (HOMEBREW: Sizes 7, 8 and 9 roll 2d7-2 instead, making megacities possible if unlikely.)
                if (Size == 7 || Size == 8 || Size == 9) tempTemp = gen.Next(7) + 1 + gen.Next(7) + 1 - 2;
                else tempTemp = gen.Next(6) + 1 + gen.Next(6) + 1 - 2;
                Population = tempTemp;
            // Government (2d6 - 7 + Population (Will shear some off if greater than 13, and cannot go below 0.)) 
                tempTemp = gen.Next(6) + 1 + gen.Next(6) + 1 - 7 + Population;
                if (tempTemp > 13){
                    tempTemp = 13 - gen.Next(5);
                }
                if (tempTemp < 0) tempTemp = 0;
                if (Population == 0) tempTemp = 0;
                Government = tempTemp;
            // Law Level (2d6 - 7 + Government)
                LawLevel = gen.Next(6) + 1 + gen.Next(6) + 1 - 7 + Government;
                if (LawLevel > 9) LawLevel = 9;
                if (LawLevel < 0) LawLevel = 0;
                if (Population == 0) LawLevel = 0;
            // Starport
                Starport = gen.Next(6) + 1 + gen.Next(6) + 1;
            // Tech Level (1d6 + Modifiers)
                tempTemp = gen.Next(6) + 1;
                // Starport Value
                if (Starport > 10) tempTemp += 6;
                else if (Starport > 8) tempTemp += 4;
                else if (Starport > 6) tempTemp += 2;
                // Size Value
                if (Size < 2) tempTemp += 2;
                else if (Size < 5) tempTemp += 1;
                // Atmosphere Value
                if (AtmosphereType < 4 || AtmosphereType > 9) tempTemp += 1;
                // Hydro Value
                if (HydrographicPercentage == 0 || HydrographicPercentage == 9) tempTemp += 1;
                if (HydrographicPercentage == 10) tempTemp += 2;
                // Population Value
                if ((Population > 0 && Population < 6) || Population == 9) tempTemp += 1;
                if (Population == 10) tempTemp += 2;
                if (Population == 11) tempTemp += 3;
                if (Population == 12) tempTemp += 4;
                // Government Value
                if (Government == 0 || Government == 5) tempTemp += 1;
                if (Government == 7) tempTemp += 2;
                if (Government == 13 || Government == 14) tempTemp -= 2;
                if (Population == 0) tempTemp = 0;
                TechLevel = tempTemp;
                #region Facilities code is ugly. Please don't look.
                // There are 6 types of facility, N, S, T, R, I, P. Each facility rolls 1d20. Depending on the Starport quality, the individual odds of a facility appearing increase.
                    if (Starport >= 11) {
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1) >= 6)
                            Bases.Add('E');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1)>=8)
                            Bases.Add('N');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1)>=10)
                            Bases.Add('S');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1)>=4)
                            Bases.Add('T');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1) >= 8)
                            Bases.Add('V');

                    }
                    else if (Starport <= 10) {
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1) >= 12)
                            Bases.Add('C');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1) >= 8)
                            Bases.Add('E');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1)>=8)
                            Bases.Add('N');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1)>=8)
                            Bases.Add('S');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1)>=6)
                            Bases.Add('T');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1) >= 10)
                            Bases.Add('V');


                    }
                    else if (Starport <= 8) {
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1) >= 10)
                            Bases.Add('E');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1) >= 10)
                            Bases.Add('C');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1) >= 10)
                            Bases.Add('V');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1)>=8)
                            Bases.Add('S');
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1)>=10)
                            Bases.Add('T');

                    }
                    else if (Starport <= 6) {
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1) >= 12)
                            Bases.Add('C');
                        else if ((gen.Next(6) + 1 + gen.Next(6) + 1) >= 7)
                            Bases.Add('S');
                        
                    }
                    else if (Starport <= 4) {
                        if ((gen.Next(6) + 1 + gen.Next(6) + 1)>=12)
                            Bases.Add('C');
                    }

                #endregion
            // Trade Codes
                string[] codes = {"Ag", "As", "Ba", "De", "Fl", "Ga", "Hi", "Ht", "IC", "In", "Lo", "Lt", "Na", "NI", "Po", "Ri", "Va", "Wa"};
                // Ag
                if ((AtmosphereType > 3 && AtmosphereType < 10) && (HydrographicPercentage > 3 && HydrographicPercentage < 9) && (Population > 4 && Population < 8))
                    TradeCodes.Add("Ag");
                // As
                if (Size == 0 && AtmosphereType == 0 && HydrographicPercentage == 0)
                    TradeCodes.Add("As");
                // Ba
                if (Population == 0)
                    TradeCodes.Add("Ba");
                // De
                if (AtmosphereType > 1 && HydrographicPercentage == 0)
                    TradeCodes.Add("De");
                // Fl
                if (AtmosphereType > 9 && HydrographicPercentage > 0)
                    TradeCodes.Add("Fl");
                // Ga
                if (Size > 4 && AtmosphereType > 3 && AtmosphereType < 10 && HydrographicPercentage > 3 && HydrographicPercentage < 9)
                    TradeCodes.Add("Ga");
                // Hi
                if (Population > 8)
                    TradeCodes.Add("Hi");
                // Ht
                if (TechLevel > 11)
                    TradeCodes.Add("Ht");
                // IC
                if (AtmosphereType < 2 && HydrographicPercentage > 0)
                    TradeCodes.Add("IC");
                // In
                if ((AtmosphereType < 3 || AtmosphereType == 4 || AtmosphereType == 7 || AtmosphereType == 9) && Population > 8)
                    TradeCodes.Add("In");
                // Lo
                if (Population > 0 && Population < 4)
                    TradeCodes.Add("Lo");
                // Lt
                if (TechLevel < 5)
                    TradeCodes.Add("Lt");
                // Na
                if (AtmosphereType < 4 && HydrographicPercentage < 4 && Population > 5)
                    TradeCodes.Add("Na");
                // NI
                if (Population > 3 && Population < 7)
                    TradeCodes.Add("NI");
                // Po
                if (AtmosphereType > 1 && AtmosphereType < 6 && HydrographicPercentage < 4)
                    TradeCodes.Add("Po");
                // Ri
                if ((AtmosphereType == 6 || AtmosphereType == 8) && Population > 5 && Population < 9)
                    TradeCodes.Add("Ri");
                // Va
                if (AtmosphereType == 0)
                    TradeCodes.Add("Va");
                // Wa
                if (HydrographicPercentage == 10)
                    TradeCodes.Add("Wa");
                
                // Finally, determine the travel zone if any. Red codes have a very small chance of appearing on any world and overwrite other travel zones.
                if (AtmosphereType >= 10 || Government == 0 || Government == 7 || Government == 10 || LawLevel == 0 || LawLevel > 8)
                    if (gen.Next(4)==0)
                        TravelZone = 'A';
                if (gen.Next(35) == 0)
                    TravelZone = 'R';
        }

        // Version of the constructor for already provided data
        public Colony(string _name, int _hexX, int _hexY, string _starport, int _size, int _atmosphere, int _hydrographic, int _population, int _government, int _law, int _tech, char _base1, char _base2, char _zone, bool _gasGiant)
        {
            Name = _name;
            HexX = String.Format("D2", _hexX);
            HexY = String.Format("D2", _hexY);
            if (_starport.Equals("A")) Starport = 12;
            if (_starport.Equals("B")) Starport = 10;
            if (_starport.Equals("C")) Starport = 8;
            if (_starport.Equals("D")) Starport = 6;
            if (_starport.Equals("E")) Starport = 4;
            if (_starport.Equals("X")) Starport = 2;

            Size = _size;
            AtmosphereType = _atmosphere;
            HydrographicPercentage = _hydrographic;
            Population = _population;
            Government = _government;
            LawLevel = _law;
            TechLevel = _tech;
            if (_base1!=' ')Bases.Add(_base1);
            if (_base2!=' ')Bases.Add(_base2);
            TravelZone = _zone;
            if (_gasGiant == true)
                GasGiant = true;
            else
                GasGiant = false;
        }

        // Check whether the tech level of the colony is sufficient to sustain it.
        private bool checkTech(){
            if (Population == 0) return true;
            else if (AtmosphereType <= 1 && TechLevel >= 8) return true;
            else if (AtmosphereType <= 3 && TechLevel >= 5) return true;
            else if ((AtmosphereType == 4 || AtmosphereType == 7 || AtmosphereType == 9) && TechLevel >= 3) return true;
            else if (AtmosphereType == 5 || AtmosphereType == 6 || AtmosphereType == 8) return true;
            else if (AtmosphereType == 10 && TechLevel >= 8) return true;
            else if (AtmosphereType == 11 && TechLevel >= 9) return true;
            else if (AtmosphereType == 12 && TechLevel >= 10) return true;
            else if ((AtmosphereType == 13 || AtmosphereType == 14) && TechLevel >= 5) return true;
            else if (AtmosphereType == 15 && TechLevel >= 8) return true;

            return false;
        }
        private void setHydrographics()
        {
             
            if (HydrographicPercentage <= 0) TrueHydrographics = 0;
            if (HydrographicPercentage == 1) TrueHydrographics = 1 + gen.Next(15);
            if (HydrographicPercentage == 2) TrueHydrographics = 16 + gen.Next(10);
            if (HydrographicPercentage == 3) TrueHydrographics = 26 + gen.Next(10);
            if (HydrographicPercentage == 4) TrueHydrographics = 36 + gen.Next(10);
            if (HydrographicPercentage == 5) TrueHydrographics = 46 + gen.Next(10);
            if (HydrographicPercentage == 6) TrueHydrographics = 56 + gen.Next(10);
            if (HydrographicPercentage == 7) TrueHydrographics = 66 + gen.Next(10);
            if (HydrographicPercentage == 8) TrueHydrographics = 76 + gen.Next(10);
            if (HydrographicPercentage == 9) TrueHydrographics = 86 + gen.Next(10);
            if (HydrographicPercentage == 10) TrueHydrographics = 96 + gen.Next(5);
        }

        private int setSizeData()
        {
            if (TrueSize == -1)
            {
                 
                int baseSize;
                int variation;
                if (Size == 0) baseSize = 600;
                else baseSize = Size * 1400;
                if (Size == 0) variation = 400;
                else variation = 200 * Size;
                TrueSize = baseSize + gen.Next(variation);
                return TrueSize;
            }
            else return TrueSize;
        }

        private void setGravityData()
        {
                 
                if (Size == 0) TrueGravVal = "0.00";
                if (Size == 1) TrueGravVal = (0.04 + gen.Next(3) * 0.01).ToString();
                if (Size == 2) TrueGravVal = (0.12 + gen.Next(6) * 0.01).ToString();
                if (Size == 3) TrueGravVal = (0.20 + gen.Next(10) * 0.01).ToString();
                if (Size == 4) TrueGravVal = (0.30 + gen.Next(10) * 0.01).ToString();
                if (Size == 5) TrueGravVal = (0.40 + gen.Next(10) * 0.01).ToString();
                if (Size == 6) TrueGravVal = (0.6 + gen.Next(20) * 0.01).ToString();
                if (Size == 7) TrueGravVal = (0.85 + gen.Next(10) * 0.01).ToString();
                if (Size == 8) TrueGravVal = (0.95 + gen.Next(10) * 0.01).ToString();
                if (Size == 9) TrueGravVal = (1.20 + gen.Next(10) * 0.01).ToString();
                if (Size == 10) TrueGrav = (1.3 + gen.Next(20) * 0.01).ToString();
        }

        // Returns a capital letter corresponding to the input number.
        private char getChar(int number)
        {
            if (number < 26 && number >= 0)
            {
                return (char)('A' + number);
            }
            else return '#';
        }

        private string getAtmosphereData()
        {
             
            string atmos = "";
            if (AtmosphereType == 0)
            {
                atmos = "Vaccuum";
                pressureVal = "0.00";
                Gear.Add("Vacc Suit");
            }
            if (AtmosphereType == 1)
            {
                atmos = "Trace";
                pressureVal = (0.001 + (gen.Next(90)*0.001)).ToString();
                Gear.Add("Vacc Suit");
            }
            if (AtmosphereType == 2)
            {
                atmos = "Very Thin, Tainted";
                pressureVal = (0.01 + (gen.Next(32) * 0.01)).ToString();
                Gear.Add("Respirator");
                Gear.Add("Filter");
            }
            if (AtmosphereType == 3)
            {
                atmos = "Very Thin";
                pressureVal = (0.01 + (gen.Next(32) * 0.01)).ToString();
                Gear.Add("Respirator");
            }
            if (AtmosphereType == 4)
            {
                atmos = "Thin, Tainted";
                pressureVal = (0.43 + (gen.Next(27) * 0.01)).ToString();
                Gear.Add("Filter");
            }
            if (AtmosphereType == 5)
            {
                atmos = "Thin";
                pressureVal = (0.43 + (gen.Next(27) * 0.01)).ToString();
                
            }
            if (AtmosphereType == 6)
            {
                atmos = "Standard";
                pressureVal = (0.71 + (gen.Next(79) * 0.01)).ToString();
            }
            if (AtmosphereType == 7)
            {
                atmos = "Standard, Tainted";
                pressureVal = (0.71 + (gen.Next(79) * 0.01)).ToString();
                Gear.Add("Filter");
            }
            if (AtmosphereType == 8)
            {
                atmos = "Dense";
                pressureVal = (1.5 + (gen.Next(100) * 0.01)).ToString();
            }
            if (AtmosphereType == 9)
            {
                atmos = "Dense, Tainted";
                pressureVal = (1.5 + (gen.Next(100) * 0.01)).ToString();
                Gear.Add("Filter");
            }
            if (AtmosphereType == 10)
            {
                atmos = "Exotic";
                pressureVal = "<UNKNOWN>";
                Gear.Add("Air Supply");
            }
            if (AtmosphereType == 11)
            {
                atmos = "Corrosive";
                pressureVal = "<UNKNOWN>";
                Gear.Add("Vacc Suit");
            }
            if (AtmosphereType == 12)
            {
                atmos = "Insidious";
                pressureVal = "<UNKNOWN>";
                Gear.Add("Vacc Suit");
            }
            if (AtmosphereType == 13)
            {
                atmos = "Dense, High";
                pressureVal = (2.5 + (gen.Next(100) * 0.01)).ToString();
            }
            if (AtmosphereType == 14)
            {
                atmos = "Thin, Low";
                pressureVal = (0.5 - (gen.Next(50) * 0.01)).ToString();
            }
            if (AtmosphereType == 15)
            {
                atmos = "Unusual";
                pressureVal = "<UNKNOWN>";
                Gear.Add("<UNKNOWN>");
            }
            return atmos;
        }

        private string getTemperatureData()
        {
            string temp = "";
             
            if (Temperature > 12)
            {
                temp = "Roasting";
                TrueTemperature = 81 + gen.Next(70);
            }
            else if (Temperature >= 10)
            {
                temp = "Hot";
                TrueTemperature = 31 + gen.Next(50);
            }
            else if (Temperature >= 5)
            {
                temp = "Temperate";
                TrueTemperature = 0 + gen.Next(30);
            }
            else if (Temperature >= 3)
            {
                temp = "Cold";
                TrueTemperature = -51 + gen.Next(52);
            }
            else if (Temperature <= 2)
            {
                temp = "Freezing";
                TrueTemperature = -50 - gen.Next(100);
            }
            return temp;
        }

        // returns starport grade data
        private string[] getStarportData()
        {
            string[] grade = new string[]{};
            int berthFee;
             
            if (Starport <= 2)
            {
                if (BerthingCost == -1) { 
                    berthFee = 0;
                    BerthingCost = berthFee;
                }
                else berthFee = BerthingCost;
                grade = new string[] { "No Starport", "0cr", "None", "None" };
            }
            else if (Starport <= 4)
            {
                if (BerthingCost == -1)
                {
                    berthFee = 0;
                    BerthingCost = berthFee;
                }
                else berthFee = BerthingCost;
                grade = new string[] { "Frontier", "0cr", "None", "None" };
            }
            else if (Starport <= 6)
            {
                if (BerthingCost == -1)
                {
                    berthFee = 10 * (gen.Next(6) + 1);
                    BerthingCost = berthFee;
                }
                else berthFee = BerthingCost;
                grade = new string[] { "Poor", berthFee.ToString()+"cr", "Unrefined (100cr/t)", "Limited Repair" };
            }
            else if (Starport <= 8)
            {
                if (BerthingCost == -1)
                {
                    berthFee = 100 * (gen.Next(6) + 1);
                    BerthingCost = berthFee;
                }
                else berthFee = BerthingCost;
                grade = new string[] { "Routine", berthFee.ToString()+"cr", "Unrefined (100cr/t)", "Shipyard (Small Craft), Repair" };
            }
            else if (Starport <= 10)
            {
                if (BerthingCost == -1)
                {
                    berthFee = 500 * (gen.Next(6) + 1);
                    BerthingCost = berthFee;
                }
                else berthFee = BerthingCost;
                grade = new string[] { "Good", berthFee.ToString()+"cr", "Refined (500cr/t)", "Shipyard (Spacecraft), Repair" };
            }
            else if (Starport >= 11)
            {
                if (BerthingCost == -1)
                {
                    berthFee = 1000 * (gen.Next(6) + 1);
                    BerthingCost = berthFee;
                }
                else berthFee = BerthingCost;
                grade = new string[] { "Excellent", berthFee.ToString()+"cr", "Refined (500cr/t)", "Shipyard (All), Repair" };
            }

            return grade;
        }

        private UInt64 getPopulationData()
        {
             
            if (Population == 0){
                return 0;
            }
            if (Population == 1)    // 1: Single Family/Homestead
            {
                return (UInt64)(gen.Next(100) + 1);
            }
            if (Population == 2)    // 2: A village
            {
                return (UInt64)(gen.Next(900) + 100);
            }
            if (Population == 3)    
            {
                return (UInt64)(gen.Next(9000) + 1000);
            }
            if (Population == 4)    
            {
                return (UInt64)(gen.Next(9000) * 10 + 10000);
            }
            if (Population == 5)    
            {
                return (UInt64)(gen.Next(9000) * 100 + 100000);
            }
            if (Population == 6)    
            {
                return (UInt64)(gen.Next(9000) * 1000 + 1000000);
            }
            if (Population == 7)    
            {
                return (UInt64)(gen.Next(9000) * 10000 + 10000000);
            }
            if (Population == 8)   
            {
                return (UInt64)(gen.Next(9000) * 100000 + 100000000);
            }
            if (Population == 9)   
            {
                return (UInt64)(gen.Next(9000) * 1000000 + 1000000000);
            }
            if (Population == 10)    
            {
                return (UInt64)(gen.Next(9000) * 10000000 + 10000000000);
            }
            if (Population == 11)    
            {
                return (UInt64)(gen.Next(9000) * 100000000 + 100000000000);
            }
            if (Population == 12)    
            {
                return (UInt64)(gen.Next(9000) * 1000000000 + 100000000000);
            }
            return 0;
        }

        // returns a string containing faction base data
        private string getBaseData()
        {
            string baseData = "";
            // Contains N
            foreach (char i in Bases){
                if (i == 'N') baseData += "                       Naval\r\n";
                if (i == 'S') baseData += "                       Scout\r\n";
                if (i == 'T') baseData += "                       TAS\r\n";
                if (i == 'V') baseData += "                       Research\r\n";
                if (i == 'E') baseData += "                       Imperial Consulate\r\n";
                if (i == 'C') baseData += "                       Pirate\r\n";
            }
            return baseData;
        }

        private string getGovernmentData()
        {
            string govData = "";
            if (Government == 0) govData += "None";
            if (Government == 1) govData += "Company/Corporation";
            if (Government == 2) govData += "Participating Democracy";
            if (Government == 3) govData += "Self-Perpetuating Oligarchy";
            if (Government == 4) govData += "Representative Democracy";
            if (Government == 5) govData += "Feudal Technocracy";
            if (Government == 6) govData += "Captive Government";
            if (Government == 7) govData += "Balkanisation";
            if (Government == 8) govData += "Civil Service Bureaucracy";
            if (Government == 9) govData += "Impersonal Bureaucracy";
            if (Government == 10) govData += "Charismatic Dictator";
            if (Government == 11) govData += "Non-Charismatic Leader";
            if (Government == 12) govData += "Charismatic Oligarchy";
            if (Government == 13) govData += "Religious Dictatorship";
            return govData;
        }

        private string getTradeCodes()
        {
            string tC = "";
            if (TradeCodes.Contains("Ag")){
                tC += " - Ag - Agricultural\r\nAgricultural worlds are dedicated to farming and food production. Often, they are divided into vast semi-feudal estates.\r\n";
            }
            if (TradeCodes.Contains("As"))
            {
                tC += " - As - Asteroid\r\nAsteroids are usually mining colonies, but can also be orbital factories or colonies.\r\n";
            }
            if (TradeCodes.Contains("Ba"))
            {
                tC += " - Ba - Barren\r\nBarren worlds are uncolonised and empty.\r\n";
            }
            if (TradeCodes.Contains("De"))
            {
                tC += " - De - Desert\r\nDesert worlds are dry and barely habitable.\r\n";
            }
            if (TradeCodes.Contains("Fl"))
            {
                tC += " - Fl - Fluid Ocean\r\nFluid Oceans are worlds where the surface liquid is something other than water, and so are incompatible with Earth-derived life.\r\n";
            }
            if (TradeCodes.Contains("Ga"))
            {
                tC += " - Ga - Garden\r\nGarden worlds are Earth-like.\r\n";
            }
            if (TradeCodes.Contains("Hi"))
            {
                tC += " - Hi - High Population\r\nHigh Population worlds have a population in the billions.\r\n";
            }
            if (TradeCodes.Contains("Ht"))
            {
                tC += " - Ht - High Technology\r\nHigh Technology worlds are among the most technologically advanced in the Imperium.\r\n";
            }
            if (TradeCodes.Contains("IC"))
            {
                tC += " - IC - Ice-Capped\r\nIce-Capped worlds have most of their surface liquid frozen in polar ice caps, and are cold and dry.\r\n";
            }
            if (TradeCodes.Contains("In"))
            {
                tC += " - In - Industrial\r\nIndustrial worlds are dominated by factories and cities.\r\n";
            }
            if (TradeCodes.Contains("Lo"))
            {
                tC += " - Lo - Low Population\r\nLow Population worlds have a population of only a few thousand or less.\r\n";
            }
            if (TradeCodes.Contains("Lt"))
            {
                tC += " - Lt - Low Technology\r\nLow Technology worlds are preindustrial and cannot produce advanced goods.\r\n";
            }
            if (TradeCodes.Contains("Na"))
            {
                tC += " - Na - Non-Agricultural\r\nNon-Agricultural worlds are too dry or barren to support their populations using conventional food production.\r\n";
            }
            if (TradeCodes.Contains("NI"))
            {
                tC += " - NI - Non-Industrial\r\nNon-Industrial worlds are too low-population to maintain an industrial base.\r\n";
            }
            if (TradeCodes.Contains("Po"))
            {
                tC += " - Po - Poor\r\nPoor worlds lack resources, viable land or sufﬁ cient population to be anything other than marginal colonies.\r\n";
            }
            if (TradeCodes.Contains("Ri"))
            {
                tC += " - Ri - Rich\r\nRich worlds are blessed with a stable government and viable biosphere, making them economic powerhouses.\r\n";
            }
            if (TradeCodes.Contains("Va"))
            {
                tC += " - Va - Vaccuum\r\nVacuum worlds have no atmosphere.\r\n";
            }
            if (TradeCodes.Contains("Wa"))
            {
                tC += " - Wa - Water World\r\nWater Worlds are nearly entirely water-ocean. \r\n";
            }
            return tC;
        }

        private List<string[]> setFactions()
        {
             
            int factionCount = gen.Next(3) + 1;
            if (Government == 0 || Government == 7) factionCount++;
            if (Government >= 10) factionCount--;
            int factionType;
            for (int i = 0; i < factionCount; i++) {
                factionType = gen.Next(14);
                if (factionType == 0) Factions.Add(new string[] { "Anarchy", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 1) Factions.Add(new string[] { "Company/Corporation", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 2) Factions.Add(new string[] { "Participating Democracy", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 3) Factions.Add(new string[] { "Self-Perpetuating Oligarchy", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 4) Factions.Add(new string[] { "Representative Democracy", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 5) Factions.Add(new string[] { "Feudal Technocracy", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 6) Factions.Add(new string[] { "Captive Government", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 7) Factions.Add(new string[] { "Balkanisation", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 8) Factions.Add(new string[] { "Civil Service Bureaucracy", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 9) Factions.Add(new string[] { "Impersonal Bureaucracy", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 10) Factions.Add(new string[] { "Charismatic Dictator", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 11) Factions.Add(new string[] { "Non-Charismatic Leader", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 12) Factions.Add(new string[] { "Charismatic Oligarchy", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
                if (factionType == 13) Factions.Add(new string[] { "Religious Dictatorship", (gen.Next(6) + 1 + gen.Next(6) + 1).ToString() });
            }
            return Factions;
        }

        // Gets contraband lists for each law level.
        private string getLawData()
        {
            string lawData = "";
            if (LawLevel <= 0)
                lawData = "Contraband: \r\n - No Restrictions\r\n";
            if (LawLevel == 1)
                lawData = "Contraband:\r\n - Weapons: \r\n    - Poison Gas\r\n    - Explosives\r\n    - Undetectable Weapons\r\n    - WMDs \r\n - Drugs:\r\n    - Highly Addictive and Dangerous Narcotics\r\n - Information:\r\n    - Intellect Programs\r\n - Technology:\r\n    - Dangerous Technologies i.e Nanotechnology\r\n - Travellers:\r\n    - Visitors must contact planetary authorities by radio.\r\n    - Landing is permitted anywhere.\r\n - Psionics:\r\n    - Dangerous talents must be registered.\r\n";
            if (LawLevel == 2)
                lawData = "Contraband:\r\n - Weapons: \r\n    - Portable Energy Weapons (Except Ship Mounted)\r\n    - Poison Gas\r\n    - Explosives\r\n    - Undetectable Weapons\r\n    - WMDs\r\n - Drugs:\r\n    - Highly Addictive Narcotics\r\n    - Highly Addictive and Dangerous Narcotics\r\n - Information:\r\n    - Agent Programs\r\n    - Intellect Programs\r\n - Technology:\r\n    - Alien Technology\r\n    - Dangerous Technologies i.e Nanotechnology\r\n - Travellers:\r\n    - Visitors must contact planetary authorities by radio.\r\n    - Visitors must report passenger manifest.\r\n    - Landing is permitted anywhere.\r\n - Psionics:\r\n    - All psionic powers must be registered.\r\n    - Use of dangerous powers is forbidden.\r\n";
            if (LawLevel == 3)
                lawData = "Contraband:\r\n - Weapons: \r\n    - Heavy Weapons\r\n    - Portable Energy Weapons (Except Ship Mounted)\r\n    - Poison Gas\r\n    - Explosives\r\n    - Undetectable Weapons\r\n    - WMDs\r\n - Drugs:\r\n    - Combat Drugs\r\n    - Highly Addictive Narcotics\r\n - Information:\r\n    - Intrusion Programs\r\n    - Agent Programs\r\n    - Intellect Programs\r\n - Technology:\r\n    - Tech 15 Items\r\n    - Alien Technology\r\n    - Dangerous Technologies i.e Nanotechnology\r\n - Travellers:\r\n    - Visitors must contact planetary authorities by radio.\r\n    - Visitors must report passenger manifest.\r\n    - Landing is permitted only at authorized sites.\r\n - Psionics:\r\n    - All psionic powers must be registered.\r\n    - Use of dangerous powers is forbidden.\r\n    - Telepathy is restricted to government approved users.\r\n";
            if (LawLevel == 4)
                lawData = "Contraband:\r\n - Weapons: \r\n    - Light Assault Weapons\r\n    - Submachine Guns\r\n    - Heavy Weapons\r\n    - Portable Energy Weapons (Except Ship Mounted)\r\n    - Poison Gas\r\n    - Explosives\r\n    - Undetectable Weapons\r\n    - WMDs\r\n - Drugs:\r\n    - Combat Drugs\r\n    - Addictive Narcotics\r\n - Information:\r\n    - Security Programs\r\n    - Intrusion Programs\r\n    - Agent Programs\r\n    - Intellect Programs\r\n - Technology:\r\n    - Tech 13 Items\r\n    - Alien Technology\r\n    - Dangerous Technologies i.e Nanotechnology\r\n - Travellers:\r\n    - Visitors must contact planetary authorities by radio.\r\n    - Visitors must report passenger manifest.\r\n    - Landing is permitted only at the starport.\r\n - Psionics:\r\n    - All psionic powers must be registered.\r\n    - Use of dangerous powers is forbidden.\r\n    - Telepathy, Teleportation and Clairvoyance are restricted to government approved users.\r\n";
            if (LawLevel == 5)
                lawData = "Contraband:\r\n - Weapons: \r\n    - Personal Concealable Weapons\r\n    - Light Assault Weapons\r\n    - Submachine Guns\r\n    - Heavy Weapons\r\n    - Portable Energy Weapons (Except Ship Mounted)\r\n    - Poison Gas\r\n    - Explosives\r\n    - Undetectable Weapons\r\n    - WMDs\r\n - Drugs:\r\n    - Anagathics\r\n    - Combat Drugs\r\n    - Addictive Narcotics\r\n - Information:\r\n    - Expert Programs\r\n    - Security Programs\r\n    - Intrusion Programs\r\n    - Agent Programs\r\n    - Intellect Programs\r\n - Technology:\r\n    - Tech 11 Items\r\n    - Alien Technology\r\n    - Dangerous Technologies i.e Nanotechnology\r\n - Travellers:\r\n    - Visitors must contact planetary authorities by radio.\r\n    - Visitors must report passenger manifest.\r\n    - Landing is permitted only at the starport.\r\n    - Visitors must register all business.\r\n    - Citizens must register offworld travel.\r\n - Psionics:\r\n    - All psionic powers must be registered.\r\n    - Use of any psionic power is restricted to government approved users.\r\n";
            if (LawLevel == 6)
                lawData = "Contraband:\r\n - Weapons: \r\n    - All Firearms Except Shotguns and Stunners\r\n    - Carrying Weapons is Discouraged\r\n    - Poison Gas\r\n    - Explosives\r\n    - Undetectable Weapons\r\n    - WMDs\r\n - Drugs:\r\n    - Fast and Slow Drugs\r\n    - Anagathics\r\n    - Combat Drugs\r\n    - Addictive Narcotics\r\n - Information:\r\n    - Recent News From Offworld\r\n    - Expert Programs\r\n    - Security Programs\r\n    - Intrusion Programs\r\n    - Agent Programs\r\n    - Intellect Programs\r\n - Technology:\r\n    - Tech 9 Items\r\n    - Alien Technology\r\n    - Dangerous Technologies i.e Nanotechnology\r\n - Travellers:\r\n    - Visitors must contact planetary authorities by radio.\r\n    - Visitors must report passenger manifest.\r\n    - Landing is permitted only at the starport.\r\n    - Visitors must register all business.\r\n    - Citizens must register offworld travel.\r\n    - Visits are discouraged.\r\n    - Excessive contact with citizens forbidden.\r\n - Psionics:\r\n    - All psionic powers must be registered.\r\n    - Use of any psionic power is restricted to government approved users.\r\n    - Possession of psionic drugs is forbidden.\r\n";
            if (LawLevel == 7)
                lawData = "Contraband:\r\n - Weapons: \r\n    - All Firearms Including Stunners\r\n    - Carrying Weapons is Discouraged \r\n    - Poison Gas\r\n    - Explosives\r\n    - Undetectable Weapons\r\n    - WMDs\r\n - Drugs:\r\n    - Fast and Slow Drugs\r\n    - Anagathics\r\n    - Combat Drugs\r\n    - Narcotics\r\n - Information:\r\n    - Free Speech Curtailed\r\n    - Unfiltered Data About Other Worlds\r\n    - Library Programs\r\n    - Expert Programs\r\n    - Security Programs\r\n    - Intrusion Programs\r\n    - Agent Programs\r\n    - Intellect Programs\r\n - Technology:\r\n    - Tech 7 Items\r\n    - Alien Technology\r\n    - Dangerous Technologies i.e Nanotechnology\r\n - Travellers:\r\n    - Visitors must contact planetary authorities by radio.\r\n    - Visitors must report passenger manifest.\r\n    - Landing is permitted only at the starport.\r\n    - Visitors may not leave starport.\r\n    - Citizens may not leave planet.\r\n - Psionics:\r\n    - All psionic powers must be registered.\r\n    - Use of any psionic power is forbidden.\r\n    - Possession of psionic drugs is forbidden.\r\n";
            if (LawLevel == 8)
                lawData = "Contraband:\r\n - Weapons: \r\n    - All Firearms Including Stunners\r\n    - Bladed Weapons\r\n    - Poison Gas\r\n    - Explosives\r\n    - Undetectable Weapons\r\n    - WMDs\r\n - Drugs:\r\n    - Medicinal Drugs\r\n    - Fast and Slow Drugs\r\n    - Anagathics\r\n    - Combat Drugs\r\n    - Narcotics\r\n - Information:\r\n    - Information Technology\r\n    - Non-Critical Offworld Data\r\n    - Personal Media\r\n    - Library Programs\r\n    - Expert Programs\r\n    - Security Programs\r\n    - Intrusion Programs\r\n    - Agent Programs\r\n    - Intellect Programs\r\n - Technology:\r\n    - Tech 5 Items\r\n    - Alien Technology\r\n    - Dangerous Technologies i.e Nanotechnology\r\n - Travellers:\r\n    - Visitors must contact planetary authorities by radio.\r\n    - Visitors must report passenger manifest.\r\n    - Landing is permitted only to imperial agents.\r\n    - Citizens may not leave planet.\r\n - Psionics:\r\n    - All psionic powers must be registered.\r\n    - Use of any psionic power is forbidden.\r\n    - Possession of psionic drugs is forbidden.\r\n    - All psionic-related technology is forbidden.\r\n";
            if (LawLevel >= 9)
                lawData = "Contraband:\r\n - Weapons: \r\n    - Any Weapons\r\n - Drugs:\r\n    - All Drugs\r\n - Information:\r\n    - Information Technology\r\n    - All Offworld Data\r\n    - No Free Press\r\n - Technology:\r\n    - Tech 3 Items\r\n - Travellers:\r\n    - No Offworld Visitors\r\n - Psionics:\r\n    - No psionic materials\r\n    - Possession of psionic powers is a crime.\r\n";
            return lawData;
        }

        // Gets social deviations. Will usually provide one or two, but may provide 3, none or very rarely up to 5.
        private void setSocialDeviations()
        {
             
            int devCount = gen.Next(2) + gen.Next(3);
            int result = 0;
            for (int i = 0; i < devCount; i++)
            {
                result = int.Parse((gen.Next(6) + 1).ToString() + (gen.Next(6) + 1).ToString());
                if (result == 11) Social.Add(new string[] { "Sexist", "One gender is considered subservient or inferior to the other." });
                if (result == 12) Social.Add(new string[] { "Religious", "This culture is heavily influenced by a religion or belief system, possibly one unique to this world."});
                if (result == 13) Social.Add(new string[] { "Artistic", "Art and culture are highly prized. Aesthetic design is important in all artefacts produced onworld."});
                if (result == 14) Social.Add(new string[] { "Ritualized", "Social interaction and trade is highly formalised. Politeness and adherence to traditional forms is considered very important." });
                if (result == 15) Social.Add(new string[] { "Conservative", "This culture resists change and outside influences." });
                if (result == 16) Social.Add(new string[] { "Xenophobic", "This culture distrusts outsiders and alien influences. Offworlders will face considerable prejudice." });
                if (result == 21) Social.Add(new string[] { "Taboo", "A particular topic is forbidden and cannot be discussed. Persons who unwittingly mention this topic are ostracised." });
                if (result == 22) Social.Add(new string[] { "Deceptive", "Trickery and equivocation are considered acceptable. Honesty is a sign of weakness." });
                if (result == 23) Social.Add(new string[] { "Liberal", "This culture welcomes change and offworld influence. Visitors who bring new and strange ideas will be welcomed."});
                if (result == 24) Social.Add(new string[] { "Honorable", "One’s word is one’s bond in this culture. Lying is both rare and despised." });
                if (result == 25) { devCount += 1; Social.Add(new string[] { "Influenced", "This culture is heavily influenced by another, neighbouring world." }); }
                if (result == 26) { devCount += 2; Social.Add(new string[] { "Fusion", "This culture is a merger of two distinct cultures. It has considerably more distinct quirks as a result." }); }
                if (result == 31) Social.Add(new string[] { "Barbaric", "Physical strength and combat prowess are highly valued in this culture. Visitors may be challenged to a fight, or dismissed if they seem incapable of defending themselves. Sports tend towards the bloody and violent." });
                if (result == 32) Social.Add(new string[] { "Remnant", "This culture is a surviving remnant of a once-great and vibrant civilisation, clinging to its former glory. The world is filled with crumbling ruins, and every story revolves around the good old days." });
                if (result == 33) Social.Add(new string[] { "Degenerate", "This culture is falling apart and is on the brink of war or economic collapse. Violent protests are common and the social order is decaying." });
                if (result == 34) Social.Add(new string[] { "Progressive", "This culture is expanding and vibrant. Fortunes are being made in trade; science is forging bravely ahead." });
                if (result == 35) Social.Add(new string[] { "Recovering", "A recent trauma, such as a plague, war, disaster or despotic regime has left scars on this culture." });
                if (result == 36) Social.Add(new string[] { "Nexus", "Members of many different cultures and species visit here."});
                if (result == 41) Social.Add(new string[] { "Tourist Attraction", "Some aspect of this culture or the planet draws visitors from all over charted space." });
                if (result == 42) Social.Add(new string[] { "Violent", "Physical conflict is common, taking the form of duels, brawls or other contests. Trial by combat is a part of their judicial system."});
                if (result == 43) Social.Add(new string[] { "Peaceful", "Physical conflict is almost unheard-of. This culture produces few soldiers and diplomacy reigns supreme. Forceful visitors will be ostracised." });
                if (result == 44) Social.Add(new string[] { "Obsessed", "Everyone is obsessed with or addicted to a substance, personality, act or item. This monomania pervades every aspect of the culture."});
                if (result == 45) Social.Add(new string[] { "Fashion", "Fine clothing and decoration are considered vitally important in this culture. The underdressed have no standing here." });
                if (result == 46) Social.Add(new string[] { "At War", "This culture is at war, either with another planet or polity, or is troubled by terrorists or rebels." });
                if (result == 51) Social.Add(new string[] { "Unusual Custom: Offworlders", "Space travellers hold a unique position in the culture’s mythology or beliefs, and travellers will be expected to live up to these myths." });
                if (result == 52) Social.Add(new string[] { "Unusual Custom: Starport", "The planet's starport is more than a commercial centre; it might be a religious temple, or be seen as highly controversial and surrounded by protestors." });
                if (result == 53) Social.Add(new string[] { "Unusual Custom: Media", "News agencies and telecommunications channels are especially strange here. Getting accurate information may be difficult." });
                if (result == 54) Social.Add(new string[] { "Unusual Custom: Technology", "This culture interacts with technology in an unusual way. Telecommunications might be banned, robots might have civil rights, cyborgs might be property." });
                if (result == 55) Social.Add(new string[] { "Unusual Custom: Lifecycle", "There might be a mandatory age of termination, or anagathics might be widely used. Family units might be different, with children being raised by the state or banned in favour of cloning."});
                if (result == 56) Social.Add(new string[] { "Unusual Custom: Social Standings", "The culture has a distinct caste system. Persons of a low social standing who do not behave appropriately will face punishment." });
                if (result == 61) Social.Add(new string[] { "Unusual Custom: Trade", "This culture has an odd attitude towards some aspect of commerce, which may interfere with trade at the spaceport. For example, merchants might expect a gift as part of a deal, or some goods may only be handled by certain families." });
                if (result == 62) Social.Add(new string[] { "Unusual Custom: Nobility", "Those of high social standing have a strange custom associated with them; perhaps nobles are blinded, or must live in gilded cages, or only serve for a single year before being exiled." });
                if (result == 63) Social.Add(new string[] { "Unusual Custom: Sex", "This culture has an unusual attitude towards intercourse and reproduction. Perhaps cloning is used instead, or sex is used to seal commercial deals." });
                if (result == 64) Social.Add(new string[] { "Unusual Custom: Eating", "Food and drink occupies an unusual place in this culture. Perhaps eating is a private affair, or banquets and formal dinners are seen as the highest form of politeness. " });
                if (result == 65) Social.Add(new string[] { "Unusual Custom: Travel", "Travellers may be distrusted or feared, or perhaps the culture frowns on those who leave their homes" });
                if (result == 66) Social.Add(new string[] { "Unusual Custom: Conspiracy", "Something strange is going on. The government is being subverted by another group or agency." });
            }
        }
        // Gets an extended human-readable description based on randomized data. Data points begin at position 23.
        public string getDescription()
        {
            string descString = "";
            descString += "------------\r\n  PROFILE\r\n------------\r\n";
            descString += WorldProfile + "\r\n\r\n";
            // Designation and given name.
            descString += "------------\r\n   SYSTEM\r\n------------\r\n";
            if (!Name.Equals(""))
                descString += "    " + Name + "\r\n------\r\n";
            else
                descString += "    " + Designation + "\r\n------\r\n";
            // Subsector Position
            descString += "Subsector:             " + Subsector + "\r\nLocation:              " + HexX + HexY + "\r\nTravel Zone:           ";
            if (TravelZone == 'G') descString += "Green";
            if (TravelZone == 'A') descString += "Amber";
            if (TravelZone == 'R') descString += "Red";
            descString += "\r\nGas Giant Available: ";
            if (GasGiant) descString += "  Yes";
            else descString += "  No";
            descString += "\r\n------\r\n";
            // Starport Grade
            
            descString += "Starport Quality:      " + getStarportData()[0] + "\r\n";
                // Starport Subdata
            if (!getStarportData()[0].Equals("No Starport")) { 
                descString += " - Berthing Fee:       " + getStarportData()[1] + "\r\n";
                descString += " - Available Fuel:     " + getStarportData()[2] + "\r\n";
                descString += " - Facilities:         " + getStarportData()[3] + "\r\n";
            }
            // Bases
            descString += "------\r\nKnown Bases:\r\n";
            if (Bases.Count>0)
                descString += getBaseData();
            else
                descString += "                       None\r\n";

            descString += "\r\n";
            // World
            descString += "------------\r\n    WORLD\r\n------------\r\n";
            descString += "Diameter:              " + setSizeData() + "km\r\n";
            setGravityData();
            descString += "Surface Gravity:       " + TrueGrav+"g\r\n";
            descString += "------\r\n";
            descString += "Atmosphere:            " + getAtmosphereData() + "\r\n";
            descString += " - Surface Pressure:   " + Pressure + "\r\n";
            descString += " - Equipment:          \r\n";
            foreach (string i in Gear)
            {
                descString += "    - " + i + "\r\n";
            }
            if (Gear.Count == 0)
                descString += "    - None\r\n";
            descString += "------\r\n";
            descString += "Climate:               " + getTemperatureData() + "\r\n";
            descString += " - Surface Temp:       " + TrueTemperature + " Celsius\r\n";
            descString += "------\r\n";
            descString += "Hydrographics\r\n";
            setHydrographics();
            descString += " - Surface Coverage:   " + TrueHydrographics + "%\r\n\r\n";



            // Colony
            descString += "------------\r\n   COLONY\r\n------------\r\n";
            if (Population == 0)
                descString += "Status:                Uninhabited\r\n";
            else
            {
                if (checkTech()) 
                    descString += "Status:                Inhabited\r\n";
                if (!checkTech())
                    descString += "Status:                Inhabited (WARNING: Insufficent Tech)\r\n";
                UInt64 pop = getPopulationData();
                if (pop > 9999999999999) pop = 100000 + (UInt64)gen.Next(800000);
                descString += "Population:            " + string.Format("{0:n0}", pop) +"\r\n";
                descString += "Tech Level:            " + TechLevel + "\r\n";
                descString += "------\r\n";
                descString += "Trade Codes:\r\n" + getTradeCodes();
                if (!checkTech()) 
                    descString += "The inhabitants of this colony are relying on life support\r\ntechnology they cannot maintain or repair.\r\n";
                descString += "------\r\n";
                descString += "Dominant Government:   " + getGovernmentData() + "\r\n";
                descString += "------\r\n";
                setFactions();
                descString += "Rival Factions:        \r\n";
                foreach (string[] i in Factions) {
                    descString += "                       " + i[0] + "\r\n" + "                        - Influence: " + i[1] + "\r\n";
                }
                descString += "------\r\n";
                descString += "Law Level:             " + LawLevel + "\r\n";
                descString += getLawData();
                descString += "------\r\n";
                descString += "Social Deviations:\r\n";
                setSocialDeviations();
                foreach (string[] i in Social)
                {
                    descString += " - " + i[0] + "\r\n";
                    descString += i[1] + "\r\n";
                }
                if (Social.Count == 0) descString += " - No known deviations.\r\n";
            }
            descString += "------";

            return descString;
        }
    }
}
