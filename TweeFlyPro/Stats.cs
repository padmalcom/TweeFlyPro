using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweeFly
{
    [Serializable]
    public class Stats
    {
        public int ID { get; set; } = 0;
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public string value { get; set; } = "";
        public string unit { get; set; } = "";
        public string image { get; set; } = "";

        public bool visible { get; set; } = true;

        public string isSkill { get; set; } = "None";

        public Stats(int iD, string name, string description, string value, string unit, string image, bool visible, string isSkill)
        {
            ID = iD;
            this.name = name;
            this.description = description;
            this.value = value;
            this.unit = unit;
            this.image = image;
            this.visible = visible;
            this.isSkill = isSkill;
        }

        public Stats() { }
    }
}
