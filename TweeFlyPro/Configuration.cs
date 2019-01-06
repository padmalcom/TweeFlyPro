using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TweeFly
{
    [Serializable]
    public class Configuration
    {
        public bool inventoryActive { get; set; } = false;
        public bool clothingActive { get; set; } = false;
        public bool statsActive { get; set; } = false;
        public bool daytimeActive { get; set; } = false;
        public bool shopActive { get; set; } = false;
        public bool moneyActive { get; set; } = false;
        public bool jobsActive { get; set; } = false;
        public bool charactersActive { get; set; } = false;

        // Build
        public string pathSubtract { get; set; } = "";
        public string storyName { get; set; } = "story";
        public string mainFile { get; set; } = "my_story.tw2";
        public bool runAfterGenerate { get; set; } = false;

        // Story options
        public bool navigationArrows { get; set; } = false;
        public bool debugMode { get; set; } = false;
        public int saveSlots { get; set; } = 6;
        public int paragraphWidth { get; set; } = 1000;

        public bool resizeImagesInSidebar { get; set; } = false;
        public int imageWidthInSidebar { get; set; } = 150;
        public int imageHeightInSidebar { get; set; } = 150;

        public bool resizeImagesInParagraph { get; set; } = false;
        public int imageWidthInParagraph { get; set; } = 150;
        public int imageHeightInParagraph { get; set; } = 150;

        public bool resizeImagesInDialogs { get; set; } = false;
        public int imageWidthInDialogs { get; set; } = 150;
        public int imageHeightInDialogs { get; set; } = 150;

        // Captions
        public List<CaptionPair> captions = new List<CaptionPair>();

        // Inventory
        public bool inventoryInSidebar { get; set; } = false;
        public bool inventoryLinkInSidebar { get; set; } = false;
        public bool inventorySidebarTooltip { get; set; } = false;
        public bool inventoryUseSkill1 { get; set; } = false;
        public bool inventoryUseSkill2 { get; set; } = false;
        public bool inventoryUseSkill3 { get; set; } = false;
        public List<Item> items { get; set; } = new List<Item>();
        public List<string> displayInInventory { get; set; } = new List<string>();

        // Clothing
        public bool clothingInSidebar { get; set; } = false;
        public bool clothingLinkInSidebar { get; set; } = false;
        public bool wardrobeLinkInSidebar { get; set; } = false;
        public bool clothingUseSkill1 { get; set; } = false;
        public bool clothingUseSkill2 { get; set; } = false;
        public bool clothingUseSkill3 { get; set; } = false;
        public List<Clothing> clothing { get; set; } = new List<Clothing>();
        public List<string> displayInWardrobe { get; set; } = new List<string>();
        public List<string> displayInClothingView { get; set; } = new List<string>();

        // Stats
        public bool statsInSidebar { get; set; } = false;
        public bool statsLinkInSidebar { get; set; } = false;
        public List<Stats> stats { get; set; } = new List<Stats>();

        // Daytime
        public DateTime startDate { get; set; } = DateTime.Now;
        public bool daytimeInSidebar { get; set; } = false;
        public int daytimeFormat { get; set; } = 0;

        // Shops
        public bool shopUseSkill1 { get; set; } = false;
        public bool shopUseSkill2 { get; set; } = false;
        public bool shopUseSkill3 { get; set; } = false;
        public List<Shop> shops { get; set; } = new List<Shop>();
        public List<string> itemPropertiesInShops { get; set; } = new List<string>();

        // Money
        public bool moneyInSidebar { get; set; } = false;
        public int startMoney { get; set; } = 0;
        public int moneyPerDay { get; set; } = 0;

        // Jobs
        public List<Job> jobs { get; set; } = new List<Job>();
        public List<string> displayInJobsView { get; set; } = new List<string>();

        // Characters
        public bool charactersInSidebar { get; set; } = false;
        public bool charactersLinkInSidebar { get; set; } = false;
        public bool charactersSidebarTooltip { get; set; } = false;
        public bool characterUseSkill1 { get; set; } = false;
        public bool characterUseSkill2 { get; set; } = false;
        public bool characterUseSkill3 { get; set; } = false;
        public List<Character> characters { get; set; } = new List<Character>();
        public List<string> displayInCharactersView { get; set; } = new List<string>();

        // Constructor
        public Configuration() { }

        public Configuration(bool _withCaptions = false)
        {
            if (_withCaptions)
            {
                {
                    // Navigation
                    captions.Add(new CaptionPair("BACK_CAP", "back"));
                    captions.Add(new CaptionPair("RETURN_CAP", "return"));
                    captions.Add(new CaptionPair("CONTINUE_CAP", "continue"));
                }

                {
                    // Inventory
                    captions.Add(new CaptionPair("INVENTORY_TITLE_CAP", "Inventory"));
                    captions.Add(new CaptionPair("INVENTORY_LINK_CAP", "Inventory"));
                    captions.Add(new CaptionPair("INVENTORY_SIDEBAR_TITLE_CAP", "Inventory"));
                    captions.Add(new CaptionPair("INVENTORY_EMPTY_CAP", "No items in inventory."));
                    captions.Add(new CaptionPair("INVENTORY_SKILL1_CAP", "skill1"));
                    captions.Add(new CaptionPair("INVENTORY_SKILL2_CAP", "skill2"));
                    captions.Add(new CaptionPair("INVENTORY_SKILL3_CAP", "skill3"));
                    captions.Add(new CaptionPair("INVENTORY_COL_ID_CAP", "ID"));
                    captions.Add(new CaptionPair("INVENTORY_COL_NAME_CAP", "name"));
                    captions.Add(new CaptionPair("INVENTORY_COL_DESCRIPTION_CAP", "description"));
                    captions.Add(new CaptionPair("INVENTORY_COL_CATEGORY_CAP", "category"));
                    captions.Add(new CaptionPair("INVENTORY_COL_SHOP_CATEGORY_CAP", "shop category"));
                    captions.Add(new CaptionPair("INVENTORY_COL_CAN_BUY_CAP", "can be bought"));
                    captions.Add(new CaptionPair("INVENTORY_COL_BUY_PRICE_CAP", "buy price"));
                    captions.Add(new CaptionPair("INVENTORY_COL_SELL_PRICE_CAP", "sell price"));
                    captions.Add(new CaptionPair("INVENTORY_COL_CAN_OWN_MULTIPLE_CAP", "Can own multiple"));
                    captions.Add(new CaptionPair("INVENTORY_COL_OWNED_CAP", "owned"));
                    captions.Add(new CaptionPair("INVENTORY_COL_IMAGE_CAP", "image"));
                    captions.Add(new CaptionPair("INVENTORY_COL_SKILL1_CAP", "skill1"));
                    captions.Add(new CaptionPair("INVENTORY_COL_SKILL2_CAP", "skill2"));
                    captions.Add(new CaptionPair("INVENTORY_COL_SKILL3_CAP", "skill3"));
                }

                {
                    // Wardrobe
                    captions.Add(new CaptionPair("WARDROBE_TITLE_CAP", "Wardrobe"));
                    captions.Add(new CaptionPair("WARDROBE_LINK_CAP", "Wardrobe"));
                    captions.Add(new CaptionPair("WARDROBE_SIDEBAR_TITLE_CAP", "Wardrobe"));
                    captions.Add(new CaptionPair("WARDROBE_EMPTY_CAP", "No clothing in wardrobe."));
                    captions.Add(new CaptionPair("WARDROBE_COL_ID_CAP", "ID"));
                    captions.Add(new CaptionPair("WARDROBE_COL_NAME_CAP", "name"));
                    captions.Add(new CaptionPair("WARDROBE_COL_DESCRIPTION_CAP", "description"));
                    captions.Add(new CaptionPair("WARDROBE_COL_CATEGORY_CAP", "category"));
                    captions.Add(new CaptionPair("WARDROBE_COL_OWNED_CAP", "owned"));
                    captions.Add(new CaptionPair("WARDROBE_COL_SKILL1_CAP", "skill1"));
                    captions.Add(new CaptionPair("WARDROBE_COL_SKILL2_CAP", "skill2"));
                    captions.Add(new CaptionPair("WARDROBE_COL_SKILL3_CAP", "skill3"));
                    captions.Add(new CaptionPair("WARDROBE_COL_IMAGE_CAP", "image"));

                    // Clothing
                    captions.Add(new CaptionPair("CLOTHING_TITLE_CAP", "Clothing"));
                    captions.Add(new CaptionPair("CLOTHING_LINK_CAP", "Clothing"));
                    captions.Add(new CaptionPair("CLOTHING_SKILL1_CAP", "skill1"));
                    captions.Add(new CaptionPair("CLOTHING_SKILL2_CAP", "skill2"));
                    captions.Add(new CaptionPair("CLOTHING_SKILL3_CAP", "skill3"));
                    captions.Add(new CaptionPair("CLOTHING_SIDEBAR_TITLE_CAP", "Clothing"));
                    captions.Add(new CaptionPair("CLOTHING_COL_ID_CAP", "ID"));
                    captions.Add(new CaptionPair("CLOTHING_COL_NAME_CAP", "name"));
                    captions.Add(new CaptionPair("CLOTHING_COL_DESCRIPTION_CAP", "description"));
                    captions.Add(new CaptionPair("CLOTHING_COL_CATEGORY_CAP", "category"));
                    captions.Add(new CaptionPair("CLOTHING_COL_SHOP_CATEGORY_CAP", "shop category"));
                    captions.Add(new CaptionPair("CLOTHING_COL_OWNED_CAP", "owned"));
                    captions.Add(new CaptionPair("CLOTHING_COL_IS_WORN_CAP", "is worn"));
                    captions.Add(new CaptionPair("CLOTHING_COL_CAN_BUY_CAP", "can buy"));
                    captions.Add(new CaptionPair("CLOTHING_COL_BUY_PRICE_CAP", "buy price"));
                    captions.Add(new CaptionPair("CLOTHING_COL_SELL_PRICE_CAP", "sell price"));
                    captions.Add(new CaptionPair("CLOTHING_COL_CAN_OWN_MULTIPLE_CAP", "can own multiple"));
                    captions.Add(new CaptionPair("CLOTHING_COL_BODY_PART_CAP", "body part"));
                    captions.Add(new CaptionPair("CLOTHING_COL_SKILL1_CAP", "skill1"));
                    captions.Add(new CaptionPair("CLOTHING_COL_SKILL2_CAP", "skill2"));
                    captions.Add(new CaptionPair("CLOTHING_COL_SKILL3_CAP", "skill3"));
                    captions.Add(new CaptionPair("CLOTHING_COL_IMAGE_CAP", "image"));
                    captions.Add(new CaptionPair("CLOTHING_WEAR_CAP", "wear"));
                    captions.Add(new CaptionPair("CLOTHING_IS_WORN_CAP", "This is worn."));

                    // Bodyparts
                    captions.Add(new CaptionPair("NOTHING_CAP", "-"));
                    captions.Add(new CaptionPair("BODY_PART_CAP", "body part"));
                    captions.Add(new CaptionPair("HEAD_CAP", "head"));
                    captions.Add(new CaptionPair("HAIR_CAP", "hair"));
                    captions.Add(new CaptionPair("NECK_CAP", "neck"));
                    captions.Add(new CaptionPair("UPPER_BODY_CAP", "upper body"));
                    captions.Add(new CaptionPair("LOWER_BODY_CAP", "lower body"));
                    captions.Add(new CaptionPair("BELT_CAP", "belt"));
                    captions.Add(new CaptionPair("SOCKS_CAP", "socks"));
                    captions.Add(new CaptionPair("SHOES_CAP", "shoes"));
                    captions.Add(new CaptionPair("UNDERWEAR_TOP_CAP", "underwear (top)"));
                    captions.Add(new CaptionPair("UNDERWEAR_BOTTOM_CAP", "underwear (bottom)"));
                }

                {
                    // Stats
                    captions.Add(new CaptionPair("STATS_TITLE_CAP", "Stats"));
                    captions.Add(new CaptionPair("STATS_LINK_CAP", "Stats"));
                    captions.Add(new CaptionPair("STATS_SIDEBAR_TITLE_CAP", "Stats"));
                    captions.Add(new CaptionPair("STATS_COL_ID_CAP", "ID"));
                    captions.Add(new CaptionPair("STATS_COL_NAME_CAP", "name"));
                    captions.Add(new CaptionPair("STATS_COL_DESCRIPTION_CAP", "description"));
                    captions.Add(new CaptionPair("STATS_COL_VALUE_CAP", "value"));
                    captions.Add(new CaptionPair("STATS_COL_IMAGE_CAP", "image"));
                }

                {
                    // Daytime
                    captions.Add(new CaptionPair("DAYTIME_SIDEBAR_TITLE_CAP", "Daytime"));
                    captions.Add(new CaptionPair("DAYTIME_EARLY_MORNING_CAP", "Early morning"));
                    captions.Add(new CaptionPair("DAYTIME_DAWN_CAP", "Dawn"));
                    captions.Add(new CaptionPair("DAYTIME_MORNING_CAP", "Morning"));
                    captions.Add(new CaptionPair("DAYTIME_NOON_CAP", "Noon"));
                    captions.Add(new CaptionPair("DAYTIME_AFTERNOON_CAP", "Afternoon"));
                    captions.Add(new CaptionPair("DAYTIME_EVENING_CAP", "Evening"));
                    captions.Add(new CaptionPair("DAYTIME_NIGHT_CAP", "Night"));
                    captions.Add(new CaptionPair("DAYTIME_MID_NIGHT_CAP", "Mid-Night"));

                    captions.Add(new CaptionPair("DAYTIME_JANUARY_CAP", "January"));
                    captions.Add(new CaptionPair("DAYTIME_FEBRUARY_CAP", "February"));
                    captions.Add(new CaptionPair("DAYTIME_MARCH_CAP", "March"));
                    captions.Add(new CaptionPair("DAYTIME_APRIL_CAP", "April"));
                    captions.Add(new CaptionPair("DAYTIME_MAY_CAP", "May"));
                    captions.Add(new CaptionPair("DAYTIME_JUNE_CAP", "June"));
                    captions.Add(new CaptionPair("DAYTIME_JULY_CAP", "July"));
                    captions.Add(new CaptionPair("DAYTIME_AUGUST_CAP", "August"));
                    captions.Add(new CaptionPair("DAYTIME_SEPTEMBER_CAP", "September"));
                    captions.Add(new CaptionPair("DAYTIME_OCTOBER_CAP", "October"));
                    captions.Add(new CaptionPair("DAYTIME_NOVEMBER_CAP", "November"));
                    captions.Add(new CaptionPair("DAYTIME_DECEMBER_CAP", "December"));
                }

                {
                    // Shops
                    captions.Add(new CaptionPair("SHOP_COL_ID_CAP", "id"));
                    captions.Add(new CaptionPair("SHOP_COL_NAME_CAP", "name"));
                    captions.Add(new CaptionPair("SHOP_COL_QUANTITY_CAP", "quantity"));
                    captions.Add(new CaptionPair("SHOP_COL_PRICE_CAP", "price"));
                    captions.Add(new CaptionPair("SHOP_COL_SKILL1_CAP", "skill1"));
                    captions.Add(new CaptionPair("SHOP_COL_SKILL2_CAP", "skill2"));
                    captions.Add(new CaptionPair("SHOP_COL_SKILL3_CAP", "skill3"));
                    captions.Add(new CaptionPair("SHOP_COL_PRICE_CAP", "price"));
                    captions.Add(new CaptionPair("SHOP_COL_DESCRIPTION_CAP", "description"));
                    captions.Add(new CaptionPair("SHOP_COL_IMAGE_CAP", "image"));
                    captions.Add(new CaptionPair("SHOP_BUY_CAP", "buy"));
                    captions.Add(new CaptionPair("SHOP_SELL_CAP", "sell"));
                    captions.Add(new CaptionPair("SHOP_NO_ITEMS_CAP", "no items to sell."));
                    captions.Add(new CaptionPair("SHOP_CLOSED_CAP", "shop is closed."));
                }

                {
                    // Money
                    captions.Add(new CaptionPair("MONEY_SIDEBAR_TITLE_CAP", "Money: "));
                    captions.Add(new CaptionPair("MONEY_UNIT_CAP", "$"));
                }

                {
                    // Jobs
                    captions.Add(new CaptionPair("JOBS_COL_ID_CAP", "ID"));
                    captions.Add(new CaptionPair("JOBS_COL_NAME_CAP", "job"));
                    captions.Add(new CaptionPair("JOBS_COL_DESCRIPTION_CAP", "description"));
                    captions.Add(new CaptionPair("JOBS_COL_CATEGORY_CAP", "category"));
                    captions.Add(new CaptionPair("JOBS_COL_AVAILABLE_CAP", "available"));
                    captions.Add(new CaptionPair("JOBS_COL_REWARD_MONEY_CAP", "reward money"));
                    captions.Add(new CaptionPair("JOBS_COL_COOLDOWN_CAP", "cooldown"));
                    captions.Add(new CaptionPair("JOBS_COL_DURATION_CAP", "duration"));
                    captions.Add(new CaptionPair("JOBS_COL_IMAGE_CAP", "image"));
                    captions.Add(new CaptionPair("JOBS_COL_REWARD_ITEMS_CAP", "reward items"));
                    captions.Add(new CaptionPair("JOBS_COL_DO_JOB_CAP", "start"));
                    captions.Add(new CaptionPair("JOBS_COL_JOB_NOT_READY_CAP", "not ready"));
                }

                {
                    // Characters
                    captions.Add(new CaptionPair("CHARACTER_TITLE_CAP", "Characters"));
                    captions.Add(new CaptionPair("CHARACTER_LINK_CAP", "Characters"));
                    captions.Add(new CaptionPair("CHARACTER_SIDEBAR_TITLE_CAP", "Characters"));
                    captions.Add(new CaptionPair("CHARACTER_SKILL1_CAP", "skill1"));
                    captions.Add(new CaptionPair("CHARACTER_SKILL2_CAP", "skill2"));
                    captions.Add(new CaptionPair("CHARACTER_SKILL3_CAP", "skill3"));
                    captions.Add(new CaptionPair("CHARACTER_COL_ID_CAP", "ID"));
                    captions.Add(new CaptionPair("CHARACTER_COL_NAME_CAP", "name"));
                    captions.Add(new CaptionPair("CHARACTER_COL_CATEGORY_CAP", "category"));
                    captions.Add(new CaptionPair("CHARACTER_COL_DESCRIPTION_CAP", "description"));
                    captions.Add(new CaptionPair("CHARACTER_COL_AGE_CAP", "age"));
                    captions.Add(new CaptionPair("CHARACTER_COL_GENDER_CAP", "gender"));
                    captions.Add(new CaptionPair("CHARACTER_COL_JOB_CAP", "job"));
                    captions.Add(new CaptionPair("CHARACTER_COL_RELATION_CAP", "relation"));
                    captions.Add(new CaptionPair("CHARACTER_COL_KNOWN_CAP", "known"));
                    captions.Add(new CaptionPair("CHARACTER_COL_COLOR_CAP", "color"));
                    captions.Add(new CaptionPair("CHARACTER_COL_SKILL1_CAP", "skill1"));
                    captions.Add(new CaptionPair("CHARACTER_COL_SKILL2_CAP", "skill2"));
                    captions.Add(new CaptionPair("CHARACTER_COL_SKILL3_CAP", "skill3"));
                    captions.Add(new CaptionPair("CHARACTER_COL_IMAGE_CAP", "image"));
                }
            }
        }
    }
}
