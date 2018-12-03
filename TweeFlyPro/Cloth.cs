﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweeFly
{
    [Serializable]
    public class Cloth
    {
        public int ID { get; set; } = 0;
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public bool canBeBought { get; set; } = false;
        public string shopCategory { get; set; } = "";
        public string category { get; set; } = "";
        public string bodyPart { get; set; } = "";
        public string image { get; set; } = "";
        public int buyPrice { get; set; } = 0;
        public int sellPrice { get; set; } = 0;
        public bool canOwnMultiple { get; set; } = false;
        public int owned { get; set; } = 0;
        public string skill1 { get; set; } = "";
        public string skill2 { get; set; } = "";
        public string skill3 { get; set; } = "";
        public bool isWornAtBeginning { get; set; } = false;

        public Cloth(int iD, string name, string description, bool canBeBought, string shopCategory, string category, string bodyPart, string image, int buyPrice, int sellPrice, bool canOwnMultiple, int owned, string skill1, string skill2, string skill3, bool isWornAtBeginning)
        {
            ID = iD;
            this.name = name;
            this.description = description;
            this.canBeBought = canBeBought;
            this.shopCategory = shopCategory;
            this.category = category;
            this.bodyPart = bodyPart;
            this.image = image;
            this.buyPrice = buyPrice;
            this.sellPrice = sellPrice;
            this.canOwnMultiple = canOwnMultiple;
            this.owned = owned;
            this.skill1 = skill1;
            this.skill2 = skill2;
            this.skill3 = skill3;
            this.isWornAtBeginning = isWornAtBeginning;
        }

        public Cloth() { }
    }
}
