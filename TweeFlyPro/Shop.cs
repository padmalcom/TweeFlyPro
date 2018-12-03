using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweeFly
{
    [Serializable]
    public class Shop
    {
        public int ID { get; set; } = 0;
        public string name { get; set; } = "";
        public DateTime opening { get; set; } = new DateTime(2018, 01, 01, 0, 0, 0);
        public DateTime closing { get; set; } = new DateTime(2018, 01, 01, 23, 59, 59);
        public List<ShopItem> items { get; set; } = new List<ShopItem>();

        public Shop(int iD, string name, DateTime opening, DateTime closing)
        {
            ID = iD;
            this.name = name;
            this.opening = opening;
            this.closing = closing;
            this.items = new List<ShopItem>();
        }

        public Shop(){}
    }
}
