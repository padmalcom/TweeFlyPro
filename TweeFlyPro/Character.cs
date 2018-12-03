using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweeFly
{
    [Serializable]
    public class Character
    {
        public int ID { get; set; } = 0;
        public string name { get; set; } = "";
        public int age { get; set; } = 0;
        public string description { get; set; } = "";
        public bool known { get; set; } = false;
        public string category { get; set; } = "";
        public string image { get; set; } = "";
        public string gender { get; set; } = "";
        public string job { get; set; } = "";
        public int relation { get; set; } = 0;
        public string color { get; set; } = "";
        public string skill1 { get; set; } = "";
        public string skill2 { get; set; } = "";
        public string skill3 { get; set; } = "";

        public Character(int iD, string name, int age, string description, bool known, string category, string image, string gender, string job, int relation, string color, string skill1, string skill2, string skill3)
        {
            ID = iD;
            this.name = name;
            this.age = age;
            this.description = description;
            this.known = known;
            this.category = category;
            this.image = image;
            this.gender = gender;
            this.job = job;
            this.relation = relation;
            this.color = color;
            this.skill1 = skill1;
            this.skill2 = skill2;
            this.skill3 = skill3;
        }

        public Character() { }
    }
}
