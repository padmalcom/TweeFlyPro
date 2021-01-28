using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweeFly
{
    [Serializable]
    public class Job
    {
        public int ID { get; set; } = 0;
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public bool available { get; set; } = false;
        public int cooldown { get; set; } = 0;
        public string category { get; set; } = "";
        public string image { get; set; } = "";
        public int rewardMoney { get; set; } = 0;
        public int duration { get; set; } = 0;
        public List<RewardItem> rewardItems { get; set; } = new List<RewardItem>();

        public string passage { get; set; } = "";

        public Job(int iD, string name, string description, bool available, int cooldown, string category, string image, int rewardMoney, int duration, string passage)
        {
            ID = iD;
            this.name = name;
            this.description = description;
            this.available = available;
            this.cooldown = cooldown;
            this.category = category;
            this.image = image;
            this.rewardMoney = rewardMoney;
            this.duration = duration;
            this.rewardItems = new List<RewardItem>();
            this.passage = passage;
        }

        public Job() { }
    }
}
