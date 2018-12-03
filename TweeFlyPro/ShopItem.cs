using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweeFly
{
    [Serializable]
    public class ShopItem
    {
        public string type { get; set; } = "Item";
        public int id { get; set; } = 0;
        public int quantityStart { get; set; } = 0;
        public int quantityMax { get; set; } = 0;
        public int refillDelay { get; set; } = 0;

        public ShopItem(string _type, int _id, int _quantityStart, int _quantityMax, int _refillDelay)
        {
            type = _type;
            id = _id;
            quantityStart = _quantityStart;
            quantityMax = _quantityMax;
            refillDelay = _refillDelay;
        }

        public ShopItem(){}
    }
}
