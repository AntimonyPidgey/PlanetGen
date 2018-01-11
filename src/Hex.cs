using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellerGen
{
    // Represents a hex within a subsector.
    class Hex
    {
        public bool HasWorld { get; private set; }
        public Colony World { get; private set; }
        public bool GasGiant { get; set; }
        public char Starport { get; private set; }
        public string Designation { get; private set; }
        public string Name { get; private set; }
        public char TravelZone { get; private set; }
        public List<char> Bases { get; private set; }
        public string HexX { get; private set; }
        public string HexY { get; private set; }

        /// <summary>
        /// Creates a hex inhabited by a world and its accompanying system.
        /// </summary>
        /// <param name="_Name">Local name of the world. Null leaves the name out.</param>
        /// <param name="_Designation">Designation of the world. Randomly generates a designation if null.</param>
        /// <param name="_HexX">Horizontal position</param>
        /// <param name="_HexY">Vertical position</param>
        public Hex(string _Name, string _Designation, string _HexX, string _HexY, string _Subsector, Random _gen){
                Random gen = new Random();
                if (!_gen.Equals(null)) gen = _gen;
                HasWorld = true;

                // Does the system have a gas giant?
                if ((gen.Next(6) + gen.Next(6) + 2) < 10)
                    GasGiant = true;
                else GasGiant = false;

                // Create the colony.
                World = new Colony(_Name, _HexX, _HexY, _Designation, _Subsector, this, gen);

                // Take values from the colony.
                if (World.Starport <= 2) Starport = 'X';
                else if (World.Starport <= 4) Starport = 'E';
                else if (World.Starport <= 6) Starport = 'D';
                else if (World.Starport <= 8) Starport = 'C';
                else if (World.Starport <= 10) Starport = 'B';
                else if (World.Starport >= 11) Starport = 'A';

                Designation = World.Designation;
                Name = World.Name;
                Bases = World.Bases;
                HexX = World.HexX;
                HexY = World.HexY;
                TravelZone = World.TravelZone;
        }

        /// <summary>
        /// Creates an uninhabited Hex.
        /// </summary>
        /// <param name="_HexX">Horizontal Position</param>
        /// <param name="_HexY">Vertical Position</param>
        public Hex(string _HexX, string _HexY)
        {
            HasWorld = false;
            HexX = _HexX;
            HexY = _HexY;
        }
    }
}
