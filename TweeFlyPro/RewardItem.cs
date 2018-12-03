using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweeFly
{
    [Serializable]
    public class RewardItem
    {
        public string type { get; set; } = "ITEM";
        public int ID { get; set; } = 0;
        public int amount { get; set; } = 0;

        public RewardItem(string type, int iD, int amount)
        {
            this.type = type;
            ID = iD;
            this.amount = amount;
        }

        public RewardItem() { }
    }
}
