using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class GameOptions
    {
        public string name;
        public string plugin;
        public int hight;
        public int width;
        public int numOfPlayers;
        public int mineRate;
        public bool firstSafe;
        public bool endOnExpload;
        public bool JoinAfterStart;

        public GameOptions(string name, string plugin, int hight, int width, int numOfPlayers, int mineRate, bool firstSafe, bool endOnExpload, bool joinAfterStart)
        {
            this.name = name;
            this.plugin = plugin;
            this.hight = hight;
            this.width = width;
            this.numOfPlayers = numOfPlayers;
            this.mineRate = mineRate;
            this.firstSafe = firstSafe;
            this.endOnExpload = endOnExpload;
            JoinAfterStart = joinAfterStart;
        }
    }
}
