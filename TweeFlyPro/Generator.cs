using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using HtmlAgilityPack;
using System.Web;
using System.Text.RegularExpressions;

namespace TweeFly
{
    class Generator
    {
        private static string TWEEFLY_CSS_COMMENT_START = "/* TWEEFLY_START */";
        private static string TWEEFLY_CSS_COMMENT_END = "/* TWEEFLY_END */";

        private static string TWEEFLY_COMMENT_START_ESC = System.Security.SecurityElement.Escape("<!-- TWEEFLY_START -->");
        private static string TWEEFLY_COMMENT_END_ESC = System.Security.SecurityElement.Escape("<!-- TWEEFLY_END -->");

        private static bool isBool(string s)
        {
            bool r;
            return Boolean.TryParse(s, out r);
        }

        private static bool isNumber(string s)
        {
            int ri;
            double rd;
            bool res = int.TryParse(s, out ri);
            if (!res)
            {
                res = double.TryParse(s, out rd);
            }
            return res;
        }

        private static void generateMain(Configuration _conf, string _path, string _mainFile)
        {
            string name = string.IsNullOrEmpty(_conf.storyName) ? "story" : _conf.storyName;
            string fileName = name + ".tw2";
            string mainPath = Path.Combine(_path, fileName);
            TextWriter twMain = null;
            try
            {
                twMain = new StreamWriter(mainPath, false, new UTF8Encoding(false));
                twMain.WriteLine("::StoryTitle");
                twMain.WriteLine(name);
                twMain.WriteLine("");

                twMain.WriteLine("::Configuration[twee2]");
                string ifid = System.Guid.NewGuid().ToString("D");
                twMain.WriteLine("Twee2::build_config.story_ifid = '"+ ifid + "';");
                twMain.WriteLine("Twee2::build_config.story_format = 'SugarCube2';");
                twMain.WriteLine("");

                // Story includes
                twMain.WriteLine("::StoryIncludes");
                if (_conf.inventoryActive) twMain.WriteLine("_js_inventory.tw2");
                if (_conf.clothingActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) twMain.WriteLine("_js_clothing.tw2");
                if (_conf.statsActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) twMain.WriteLine("_js_stats.tw2");
                if (_conf.moneyActive) twMain.WriteLine("_js_money.tw2");
                twMain.WriteLine("_js_navigation.tw2");
                if (_conf.daytimeActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) twMain.WriteLine("_js_daytime.tw2");
                if (_conf.shopActive) twMain.WriteLine("_js_shops.tw2");
                if (_conf.jobsActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) twMain.WriteLine("_js_jobs.tw2");
                if (_conf.charactersActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) twMain.WriteLine("_js_characters.tw2");
                twMain.WriteLine("_tw_sidebar.tw2");
                twMain.WriteLine("_css_style.tw2");
                if (!string.IsNullOrEmpty(_mainFile))
                {
                    twMain.WriteLine(Path.GetFileName(_mainFile));
                }
                twMain.WriteLine("");

                // Story init
                twMain.WriteLine("::StoryInit");
                if (_conf.daytimeActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) twMain.WriteLine("<<initDaytime>>");
                if (_conf.clothingActive && TweeFlyPro.Properties.Settings.Default.IsProEdition)
                {
                    twMain.WriteLine("<<initAllClothing>>");
                    twMain.WriteLine("<<initClothing>>");
                    twMain.WriteLine("<<initWardrobe>>");
                }
                if (_conf.inventoryActive)
                {
                    twMain.WriteLine("<<initItems>>");
                    twMain.WriteLine("<<initInventory>>");
                }
                if (_conf.shopActive) twMain.WriteLine("<<initShops>>");
                if (_conf.statsActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) twMain.WriteLine("<<initStats>>");
                if (_conf.moneyActive) twMain.WriteLine("<<initMoney>>");
                if (_conf.jobsActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) twMain.WriteLine("<<initJobs>>");
                if (_conf.charactersActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) twMain.WriteLine("<<initCharacters>>");
                twMain.WriteLine("");
            }
            finally
            {
                if (twMain != null)
                {
                    twMain.Flush();
                    twMain.Close();
                }
            }
        }

        private static string generateMenuString(Configuration _conf)
        {
            string menu = "";

            if (_conf.inventoryActive && _conf.inventoryLinkInSidebar)
                menu += "[[" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_LINK_CAP")).caption + "->InventoryMenu]]\n";

            if (_conf.clothingActive && _conf.wardrobeLinkInSidebar && TweeFlyPro.Properties.Settings.Default.IsProEdition)
                menu += "[[" + _conf.captions.Single(s => s.captionName.Equals("WARDROBE_LINK_CAP")).caption + "->WardrobeMenu]]\n";

            if (_conf.clothingActive && _conf.clothingLinkInSidebar && TweeFlyPro.Properties.Settings.Default.IsProEdition)
                menu += "[[" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_LINK_CAP")).caption + "->ClothingMenu]]\n";

            if (_conf.statsActive && _conf.statsLinkInSidebar && TweeFlyPro.Properties.Settings.Default.IsProEdition)
                menu += "[[" + _conf.captions.Single(s => s.captionName.Equals("STATS_LINK_CAP")).caption + "->StatsMenu]]\n";

            if (_conf.charactersActive && _conf.charactersLinkInSidebar && TweeFlyPro.Properties.Settings.Default.IsProEdition)
                menu += "[[" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_LINK_CAP")).caption + "->CharactersMenu]]\n";
            menu += "\n";

            // Variables
            if (_conf.moneyActive && _conf.moneyInSidebar)
                menu += _conf.captions.Single(s => s.captionName.Equals("MONEY_SIDEBAR_TITLE_CAP")).caption + " <<printMoney>>" +
                    _conf.captions.Single(s => s.captionName.Equals("MONEY_UNIT_CAP")).caption + "\n";

            if (_conf.daytimeActive && _conf.daytimeInSidebar && TweeFlyPro.Properties.Settings.Default.IsProEdition)
            {
                switch (_conf.daytimeFormat)
                {
                    case 0: menu += "<<getTime>>\n"; break;
                    case 1: menu += "<<getDate>>\n"; break;
                    case 2: menu += "<<getDateTime>>\n"; break;
                    case 3: menu += "<<getTimeOfDay>>\n"; break;
                }
            }
            menu += "\n";

            // Sidebar menus
            if (_conf.inventoryActive && _conf.inventoryInSidebar) menu += "<<inventorySidebar>>\n";
            if (_conf.clothingActive && _conf.clothingInSidebar && TweeFlyPro.Properties.Settings.Default.IsProEdition) menu += "<<clothingSidebar>>\n";
            if (_conf.statsActive && _conf.statsInSidebar && TweeFlyPro.Properties.Settings.Default.IsProEdition) menu += "<<statsSidebar>>\n";
            if (_conf.charactersActive && _conf.charactersInSidebar && TweeFlyPro.Properties.Settings.Default.IsProEdition) menu += "<<charactersSidebar>>\n";
            menu += "\n";
            return menu;
        }

        private static string generateInventoryPassageString(Configuration _conf) {
            string menu = "";
            // Menu paragraphs
            if (_conf.inventoryActive)
            {
                menu += "<<if $inventory.length == 0>>" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_EMPTY_CAP")).caption +
                    "<<else>>" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_TITLE_CAP")).caption;
                menu += "<<inventory>>\n";
                menu += "<<endif>>\n";
                menu += "[[" + _conf.captions.Single(s => s.captionName.Equals("BACK_CAP")).caption + "|$return]]\n";
                menu += "\n";
            }
            return menu;
        }

        private static string generateClothPassageString(Configuration _conf) {

            string menu = "";
            if (_conf.clothingActive && TweeFlyPro.Properties.Settings.Default.IsProEdition)
            {
                menu += "<h1>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_TITLE_CAP")).caption + "</h1>\n";
                menu += "<<clothing>>\n";
                menu += "[[" + _conf.captions.Single(s => s.captionName.Equals("BACK_CAP")).caption + "|$return]]\n";
                menu += "\n";
            }
            return menu;
        }

        private static string generateWardrobePassageString(Configuration _conf) {

            string menu = "";
            if (_conf.clothingActive && TweeFlyPro.Properties.Settings.Default.IsProEdition)
            {
                menu += "<h1>" + _conf.captions.Single(s => s.captionName.Equals("WARDROBE_TITLE_CAP")).caption + "</h1>\n";
                menu += "<<wardrobe>>\n";
                menu += "[[" + _conf.captions.Single(s => s.captionName.Equals("BACK_CAP")).caption + "|$return]]\n";
                menu += "\n";
            }
            return menu;
        }

        private static string generateStatsPassageString(Configuration _conf)
        {
            string menu = "";
            if (_conf.statsActive && TweeFlyPro.Properties.Settings.Default.IsProEdition)
            {
                menu += "<<stats>>\n";
                menu += "[[" + _conf.captions.Single(s => s.captionName.Equals("BACK_CAP")).caption + "|$return]]\n";
                menu += "\n";
            }
            return menu;
        }

        private static string generateCharactersPassageString(Configuration _conf) {
            string menu = "";
            if (_conf.charactersActive && TweeFlyPro.Properties.Settings.Default.IsProEdition)
            {
                menu += "<<characters>>\n";
                menu += "[[" + _conf.captions.Single(s => s.captionName.Equals("BACK_CAP")).caption + "|$return]]\n";
                menu += "\n";
            }
            return menu;
        }

        private static void generateMenu(Configuration _conf, string _path)
        {
            string menuPath = Path.Combine(_path, "_tw_sidebar.tw2");
            TextWriter twMenu = null;
            try
            {
                twMenu = new StreamWriter(menuPath, false, new UTF8Encoding(false));
                twMenu.WriteLine("::StoryCaption");
                twMenu.WriteLine(generateMenuString(_conf));
                if (_conf.inventoryActive)
                {
                    twMenu.WriteLine("::InventoryMenu[noreturn]\n");
                    twMenu.WriteLine(generateInventoryPassageString(_conf));
                }
                if (_conf.clothingActive && TweeFlyPro.Properties.Settings.Default.IsProEdition)
                {
                    twMenu.WriteLine("::ClothingMenu[noreturn]\n");
                    twMenu.WriteLine(generateClothPassageString(_conf));
                }
                if (_conf.clothingActive && TweeFlyPro.Properties.Settings.Default.IsProEdition)
                {
                    twMenu.WriteLine("::WardrobeMenu[noreturn]\n");
                    twMenu.WriteLine(generateWardrobePassageString(_conf));
                }
                if (_conf.statsActive && TweeFlyPro.Properties.Settings.Default.IsProEdition)
                {
                    twMenu.WriteLine("::StatsMenu[noreturn]\n");
                    twMenu.WriteLine(generateStatsPassageString(_conf));
                }
                if (_conf.charactersActive && TweeFlyPro.Properties.Settings.Default.IsProEdition)
                {
                    twMenu.WriteLine("::CharactersMenu[noreturn]\n");
                    twMenu.WriteLine(generateCharactersPassageString(_conf));
                }
            }
            finally
            {
                if (twMenu != null)
                {
                    twMenu.Flush();
                    twMenu.Close();
                }
            }
        }

        private static string generateInventoryScripts(Configuration _conf)
        {
            string inv = "";
            // Init items
            inv +="macros.initItems = {\n";
            inv +="\thandler: function(place, macroName, params, parser) {\n";
            inv +="\t\tif (state.active.variables.items === undefined) {\n";
            inv +="\t\t\tstate.active.variables.items = [];\n";

            for (int i = 0; i < _conf.items.Count; i++)
            {
                inv += "\t\t\tstate.active.variables.items.push({";
                inv += "\"ID\":" + _conf.items[i].ID + ",";
                inv += "\"name\":\"" + _conf.items[i].name + "\",";
                inv += "\"description\":\"" + _conf.items[i].description + "\",";
                inv += "\"category\":\"" + _conf.items[i].category + "\",";
                inv += "\"shopCategory\":\"" + _conf.items[i].shopCategory + "\",";
                inv += "\"image\":\"" + pathSubtract(_conf.items[i].image, _conf.pathSubtract) + "\",";
                inv += "\"canBeBought\":" + _conf.items[i].canBeBought.ToString().ToLower() + ",";
                inv += "\"buyPrice\":" + _conf.items[i].buyPrice + ",";
                inv += "\"sellPrice\":" + _conf.items[i].sellPrice + ",";
                inv += "\"canOwnMultiple\":" + _conf.items[i].canOwnMultiple.ToString().ToLower() + ",";
                inv += "\"owned\":" + _conf.items[i].owned;
                if (_conf.inventoryUseSkill1)
                {
                    string skill1val = (isBool(_conf.items[i].skill1) || isNumber(_conf.items[i].skill1)) ? _conf.items[i].skill1 : "\"" + _conf.items[i].skill1 + "\"";
                    inv += ",\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL1_CAP")).caption + "\":" + skill1val;
                }
                if (_conf.inventoryUseSkill2)
                {
                    string skill2val = (isBool(_conf.items[i].skill2) || isNumber(_conf.items[i].skill2)) ? _conf.items[i].skill2 : "\"" + _conf.items[i].skill2 + "\"";
                    inv +=",\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL2_CAP")).caption + "\":" + skill2val;
                }
                if (_conf.inventoryUseSkill3)
                {
                    string skill3val = (isBool(_conf.items[i].skill3) || isNumber(_conf.items[i].skill3)) ? _conf.items[i].skill3 : "\"" + _conf.items[i].skill3 + "\"";
                    inv += ",\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL3_CAP")).caption + "\":" + skill3val;
                }
                inv +="});\n";
            }
            inv +="\t\t}\n";
            inv +="\t}\n";
            inv +="};\n";
            inv +="\n";

            // getInventory
            inv +="window.getInventory = function() {\n";
            inv +="\treturn state.active.variables.inventory;\n";
            inv +="}\n";
            inv +="\n";

            // initInventory
            inv +="macros.initInventory = {\n";
            inv +="\thandler: function(place, macroName, params, parser) {\n";
            inv +="\t\tstate.active.variables.inventory = [];\n";

            // Add items to inventory
            for (int i = 0; i < _conf.items.Count; i++)
            {
                if (_conf.items[i].owned > 0)
                {
                    inv += "\t\tstate.active.variables.inventory.push({";
                    inv += "\"ID\":" + _conf.items[i].ID + ",";
                    inv += "\"name\":\"" + _conf.items[i].name + "\",";
                    inv += "\"description\":\"" + _conf.items[i].description + "\",";
                    inv += "\"category\":\"" + _conf.items[i].category + "\",";
                    inv += "\"shopCategory\":\"" + _conf.items[i].shopCategory + "\",";
                    inv += "\"image\":\"" + pathSubtract(_conf.items[i].image, _conf.pathSubtract) + "\",";
                    inv += "\"canBeBought\":" + _conf.items[i].canBeBought.ToString().ToLower() + ",";
                    inv += "\"buyPrice\":" + _conf.items[i].buyPrice + ",";
                    inv += "\"sellPrice\":" + _conf.items[i].sellPrice + ",";
                    inv += "\"canOwnMultiple\":" + _conf.items[i].canOwnMultiple.ToString().ToLower() + ",";
                    inv += "\"owned\":" + _conf.items[i].owned;
                    if (_conf.inventoryUseSkill1)
                    {
                        string skill1val = (isBool(_conf.items[i].skill1) || isNumber(_conf.items[i].skill1)) ? _conf.items[i].skill1 : "\"" + _conf.items[i].skill1 + "\"";
                        inv += ",\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL1_CAP")).caption + "\":" + skill1val;
                    }
                    if (_conf.inventoryUseSkill2)
                    {
                        string skill2val = (isBool(_conf.items[i].skill2) || isNumber(_conf.items[i].skill2)) ? _conf.items[i].skill2 : "\"" + _conf.items[i].skill2 + "\"";
                        inv += ",\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL2_CAP")).caption + "\":" + skill2val;
                    }
                    if (_conf.inventoryUseSkill3)
                    {
                        string skill3val = (isBool(_conf.items[i].skill3) || isNumber(_conf.items[i].skill3)) ? _conf.items[i].skill3 : "\"" + _conf.items[i].skill3 + "\"";
                        inv += ",\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL3_CAP")).caption + "\":" + skill3val;
                    }
                    inv +="});\n";
                }
            }
            inv +="\t}\n";
            inv +="};\n";

            // addToInventory
            inv +="macros.addToInventory = {\n";
            inv +="\thandler: function(place, macroName, params, parser) {\n";
            inv +="\t\tif (params.length != 2) {\n";
            inv += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two parameters, item id and amount.\");\n";
            inv +="\t\t\treturn;\n";
            inv +="\t\t}\n";
            inv +="\t\tif ((!Number.isInteger(params[0])) || (!Number.isInteger(params[1]))) {\n";
            inv += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two integer parameters.\");\n";
            inv +="\t\t\treturn;\n";
            inv +="\t\t}\n";
            inv +="\t\tif (typeof state.active.variables.items[params[0]] === 'undefined') {\n";
            inv += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: An item with id \" + params[0] + \" does not exist.\");\n";
            inv +="\t\t\treturn;\n";
            inv +="\t\t}\n";
            inv +="\t\tvar item_in_catalog = state.active.variables.items.filter(obj => { return obj.ID === params[0]});\n";
            inv +="\t\tif (item_in_catalog.length != 1) {\n";
            inv +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: There must be exactly one item of id \" + params[0] + \" in the item catalog but there are \" + item_in_catalog.length);\n";
            inv +="\t\t\treturn;\n";
            inv +="\t\t}\n";
            inv +="\t\tvar item = JSON.parse(JSON.stringify(item_in_catalog[0]));\n";
            inv +="\t\titem.owned = params[1];\n";
            inv +="\t\tvar existing_items_with_id = state.active.variables.inventory.filter(obj => {return obj.ID === item.ID});\n";
            inv +="\t\tif (existing_items_with_id.length == 0) {\n";
            inv +="\t\t\tstate.active.variables.inventory.push(item);\n";
            inv +="\t\t} else if (existing_items_with_id.length > 0) {\n";
            inv +="\t\t\tfor (var i in state.active.variables.inventory) {\n";
            inv +="\t\t\t\tif (state.active.variables.inventory[i].ID == item.ID) {\n";
            inv +="\t\t\t\t\tstate.active.variables.inventory[i].owned += item.owned;\n";
            inv +="\t\t\t\t\tbreak;\n";
            inv +="\t\t\t\t}\n";
            inv +="\t\t\t}\n";
            inv +="\t\t} else {\n";
            inv +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: There are several items with the same id \" + item.ID);\n";
            inv +="\t\t\treturn;\n";
            inv +="\t\t}\n";
            inv +="\t}\n";
            inv +="};\n";
            inv +="\n";

            // removeFromInventory
            inv +="macros.removeFromInventory = {\n";
            inv +="\thandler: function(place, macroName, params, parser) {\n";
            inv +="\t\tif ((params.length == 0) || (params.length > 2)) {\n";
            inv += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: expecting one or two parameters.\");\n";
            inv +="\t\t\treturn;\n";
            inv +="\t\t}\n";
            inv +="\t\tvar existing_items_with_id = state.active.variables.inventory.filter(obj => {return obj.ID === params[0]});\n";
            inv +="\t\tif (existing_items_with_id.length == 0) return;\n";
            inv +="\t\tif (params.length == 1) {\n";
            inv +="\t\t\tfor (var i in state.active.variables.inventory) {\n";
            inv +="\t\t\t\tif (state.active.variables.inventory[i].ID == params[0]) {\n";
            inv +="\t\t\t\t\tstate.active.variables.inventory.splice(i, 1);\n";
            inv +="\t\t\t\t\tbreak;\n";
            inv +="\t\t\t\t}\n";
            inv +="\t\t\t}\n";
            inv +="\t\t} else if (params.length == 2) {\n";
            inv +="\t\t\tfor (var i in state.active.variables.inventory) {\n";
            inv +="\t\t\t\tif (state.active.variables.inventory[i].ID == params[0]) {\n";
            inv +="\t\t\t\t\tstate.active.variables.inventory[i].owned -= params[1];\n";
            inv +="\t\t\t\t\tif (state.active.variables.inventory[i].owned <= 0) {\n";
            inv +="\t\t\t\t\t\tstate.active.variables.inventory.splice(i, 1);\n";
            inv +="\t\t\t\t\t}\n";
            inv +="\t\t\t\t\tbreak;\n";
            inv +="\t\t\t\t}\n";
            inv +="\t\t\t}\n";
            inv +="\t\t}\n";
            inv +="\t}\n";
            inv +="};\n";
            inv +="\n";

            // hasItem
            inv +="window.has_item = function(_id) {\n";
            inv +="\tvar existing_items_with_id = state.active.variables.inventory.filter(obj => { return obj.ID == _id});\n";
            inv +="\tif (existing_items_with_id.length > 0) return 1;\n";
            inv +="\treturn 0;\n";
            inv +="}\n";

            // Inventory
            inv +="macros.inventory = {\n";
            inv +="\thandler: function(place, macroName, params, parser) {\n";
            inv +="\t\tif (state.active.variables.inventory.length == 0) {\n";
            inv +="\t\t\tnew Wikifier(place, 'nothing');\n";
            inv +="\t\t} else {\n";
            inv +="\t\t\tvar inv_str = \"<table class=\\\"inventory\\\"><tr>\";\n";
            if (_conf.displayInInventory.Contains("ID")) inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_ID_CAP")).caption + "</th>\";\n";
            if (_conf.displayInInventory.Contains("Name")) inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_NAME_CAP")).caption + "</th>\";\n";
            if (_conf.displayInInventory.Contains("Description")) inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_DESCRIPTION_CAP")).caption + "</th>\";\n";
            if (_conf.displayInInventory.Contains("Category")) inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_CATEGORY_CAP")).caption + "</th>\";\n";
            if (_conf.displayInInventory.Contains("Shop category")) inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SHOP_CATEGORY_CAP")).caption + "</th>\";\n";
            if (_conf.displayInInventory.Contains("Owned")) inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_OWNED_CAP")).caption + "</th>\";\n";
            if (_conf.displayInInventory.Contains("Can buy")) inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_CAN_BUY_CAP")).caption + "</th>\";\n";
            if (_conf.displayInInventory.Contains("Buy price")) inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_BUY_PRICE_CAP")).caption + "</th>\";\n";
            if (_conf.displayInInventory.Contains("Sell price")) inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SELL_PRICE_CAP")).caption + "</th>\";\n";
            if (_conf.displayInInventory.Contains("Can own multiple")) inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_CAN_OWN_MULTIPLE_CAP")).caption + "</th>\";\n";

            if (_conf.displayInInventory.Contains("Skill1") && _conf.inventoryUseSkill1)
                inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\" >" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL1_CAP")).caption + "</th>\";\n";

            if (_conf.displayInInventory.Contains("Skill2") && _conf.inventoryUseSkill2)
                inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL2_CAP")).caption + "</th>\";\n";

            if (_conf.displayInInventory.Contains("Skill3") && _conf.inventoryUseSkill3)
                inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL3_CAP")).caption + "</th>\";\n";

            if (_conf.displayInInventory.Contains("Image")) inv +="\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_IMAGE_CAP")).caption + "</th>\";\n";
            inv +="\t\t\tinv_str += \"</tr>\";\n";

            inv +="\t\t\tfor (var i = 0; i < state.active.variables.inventory.length; i++)\n";
            inv +="\t\t\t{\n";
            inv +="\t\t\t\tinv_str += \"<tr>\";\n";
            if (_conf.displayInInventory.Contains("ID")) inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\">\" + state.active.variables.inventory[i].ID + \"</td>\";\n";
            if (_conf.displayInInventory.Contains("Name")) inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].name + \"</td>\";\n";
            if (_conf.displayInInventory.Contains("Description")) inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].description + \"</td>\";\n";
            if (_conf.displayInInventory.Contains("Category")) inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].category + \"</td>\";\n";
            if (_conf.displayInInventory.Contains("Shop category")) inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].shopCategory + \"</td>\";\n";
            if (_conf.displayInInventory.Contains("Owned")) inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].owned + \"</td>\";\n";
            if (_conf.displayInInventory.Contains("Can buy")) inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].canBeBought + \"</td>\";\n";
            if (_conf.displayInInventory.Contains("Buy price")) inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].buyPrice + \"</td>\";\n";
            if (_conf.displayInInventory.Contains("Sell price")) inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].sellPrice + \"</td>\";\n";
            if (_conf.displayInInventory.Contains("Can own multiple")) inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].canOwnMultiple + \"</td>\";\n";

            if (_conf.displayInInventory.Contains("Skill1") && _conf.inventoryUseSkill1)
                inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].skill1 + \"</td>\";\n";

            if (_conf.displayInInventory.Contains("Skill2") && _conf.inventoryUseSkill2)
                inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].skill2 + \"</td>\";\n";

            if (_conf.displayInInventory.Contains("Skill3") && _conf.inventoryUseSkill3)
                inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].skill3 + \"</td>\";\n";

            if (_conf.displayInInventory.Contains("Image")) inv +="\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"><img class=\\\"paragraph\\\" src=\\\"\" + state.active.variables.inventory[i].image + \"\\\" ></td>\";\n";
            inv +="\t\t\t\tinv_str += \"</tr>\";\n";
            inv +="\t\t\t}\n";
            inv +="\t\t\tinv_str += \"</table>\";\n";
            inv += "\t\t\tnew Wikifier(place, inv_str);\n";
            inv +="\t\t}\n";
            inv +="\t}\n";
            inv +="};\n";
            inv +="\n";

            // inventorySidebar
            inv +="macros.inventorySidebar = {\n";
            inv +="\thandler: function(place, macroName, params, parser) {\n";
            inv +="\n";
            inv +="\t\tvar wstr = \"<table class=\\\"inventory_sidebar\\\">\";\n";
            inv +="\t\twstr +=\"<tr><td colspan=2>Inventory</td></tr>\";\n";
            inv +="\t\tfor (var w = 0; w<state.active.variables.inventory.length; w +=2) {\n";
            inv +="\t\t\twstr +=\"<tr>\";\n";
            inv +="\n";
            inv +="\t\t\tvar item_info_1 = \"\";\n";
            if (_conf.displayInInventory.Contains("ID")) inv +="\t\t\titem_info_1 += \"ID: \" + state.active.variables.inventory[w].ID + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Name")) inv +="\t\t\titem_info_1 += \"name:\" + state.active.variables.inventory[w].name + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Description")) inv +="\t\t\titem_info_1 += \"description:\" + state.active.variables.inventory[w].description + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Category")) inv +="\t\t\titem_info_1 += \"category:\" + state.active.variables.inventory[w].category + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Shop category")) inv +="\t\t\titem_info_1 += \"shop category:\" + state.active.variables.inventory[w].shopCategory + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Owned")) inv +="\t\t\titem_info_1 += \"owned:\" + state.active.variables.inventory[w].owned + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Can buy")) inv +="\t\t\titem_info_1 += \"can buy:\" + state.active.variables.inventory[w].canBeBought + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Buy price")) inv +="\t\t\titem_info_1 += \"buy price:\" + state.active.variables.inventory[w].buyPrice + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Sell price")) inv +="\t\t\titem_info_1 += \"sell price:\" + state.active.variables.inventory[w].sellPrice + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Can own multiple")) inv +="\t\t\titem_info_1 += \"can own multiple:\" + state.active.variables.inventory[w].canOwnMultiple + \"&#10;\";\n";

            if (_conf.displayInInventory.Contains("Skill1") && _conf.inventoryUseSkill1)
                inv +="\t\t\titem_info_1 +=\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SKILL1_CAP")).caption + ": \" + state.active.variables.inventory[w].skill1 + \"&#10;\";\n";

            if (_conf.displayInInventory.Contains("Skill2") && _conf.inventoryUseSkill2)
                inv +="\t\t\titem_info_1 +=\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SKILL2_CAP")).caption + ": \" + state.active.variables.inventory[w].skill2 + \"&#10;\";\n";

            if (_conf.displayInInventory.Contains("Skill3") && _conf.inventoryUseSkill3)
                inv +="\t\t\titem_info_1 +=\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SKILL3_CAP")).caption + ": \" + state.active.variables.inventory[w].skill3 + \"&#10;\";\n";

            if (_conf.inventorySidebarTooltip)
            {
                inv +="\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.inventory[w].image + \"\\\" title=\\\"\" + item_info_1 + \"\\\"></td>\";\n";
            }
            else
            {
                inv +="\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.inventory[w].image + \"\\\"></td>\";\n";
            }
            inv +="\n";
            inv +="\t\t\tif (w+1 < state.active.variables.inventory.length) {\n";
            inv +="\n";
            inv +="\t\t\t\tvar item_info_2 = \"\";\n";
            if (_conf.displayInInventory.Contains("ID")) inv +="\t\t\t\titem_info_2 += \"ID: \" + state.active.variables.inventory[w+1].ID + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Name")) inv +="\t\t\t\titem_info_2 += \"name:\" + state.active.variables.inventory[w+1].name + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Description")) inv +="\t\t\t\titem_info_2 += \"description:\" + state.active.variables.inventory[w+1].description + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Category")) inv +="\t\t\t\titem_info_2 += \"category:\" + state.active.variables.inventory[w+1].category + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Shop category")) inv +="\t\t\t\titem_info_2 += \"shop category:\" + state.active.variables.inventory[w+1].shopCategory + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Owned")) inv +="\t\t\t\titem_info_2 += \"owned:\" + state.active.variables.inventory[w+1].owned + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Can buy")) inv +="\t\t\t\titem_info_2 += \"can buy:\" + state.active.variables.inventory[w+1].canBeBought + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Buy price")) inv +="\t\t\t\titem_info_2 += \"buy price:\" + state.active.variables.inventory[w+1].buyPrice + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Sell price")) inv +="\t\t\t\titem_info_2 += \"sell price:\" + state.active.variables.inventory[w+1].sellPrice + \"&#10;\";\n";
            if (_conf.displayInInventory.Contains("Can own multiple")) inv +="\t\t\t\titem_info_2 += \"can own multiple:\" + state.active.variables.inventory[w+1].canOwnMultiple + \"&#10;\";\n";

            if (_conf.displayInInventory.Contains("Skill1") && _conf.inventoryUseSkill1)
                inv +="\t\t\t\titem_info_2 +=\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SKILL1_CAP")).caption + ": \" + state.active.variables.inventory[w+1].skill1 + \"&#10;\";\n";

            if (_conf.displayInInventory.Contains("Skill2") && _conf.inventoryUseSkill2)
                inv +="\t\t\t\titem_info_2 +=\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SKILL2_CAP")).caption + ": \" + state.active.variables.inventory[w+1].skill2 + \"&#10;\";\n";

            if (_conf.displayInInventory.Contains("Skill3") && _conf.inventoryUseSkill3)
                inv +="\t\t\t\titem_info_2 +=\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SKILL3_CAP")).caption + ": \" + state.active.variables.inventory[w+1].skill3 + \"&#10;\";\n";

            inv +="\n";

            if (_conf.inventorySidebarTooltip)
            {
                inv +="\t\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.inventory[w+1].image + \"\\\" title=\\\"\" + item_info_2 + \"\\\"></td>\";\n";
            }
            else
            {
                inv +="\t\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.inventory[w+1].image + \"\\\"></td>\";\n";
            }
            inv +="\t\t\t} else {\n";
            inv +="\t\t\t\twstr +=\"<td></td>\";\n";
            inv +="\t\t\t}\n";
            inv +="\t\t\twstr +=\"</tr>\";\n";
            inv +="\t\t}\n";
            inv +="\t\twstr +=\"</table>\";\n";
            inv +="\n";
            inv += "\t\tif (state.active.variables.inventorySidebarVisible == 1) new Wikifier(place,wstr);\n"; // TODO: Check
            inv +="\t}\n";
            inv +="};\n";
            inv +="\n";

            // clearInventory
            inv +="macros.clearInventory = {\n";
            inv +="\thandler: function(place, macroName, params, parser) {\n";
            inv +="\t\tstate.active.variables.inventory = [];\n";
            inv +="\t}\n";
            inv +="};\n";
            inv +="\n";

            // showInventoryLink
            inv += "macros.showInventoryLink = {\n";
            inv += "\thandler: function(place, macroName, params, parser) {\n";
            inv += "\t\tstate.active.variables.inventoryLinkVisible = 1;\n";
            inv += "\t}\n";
            inv += "};\n";
            inv += "\n";

            // showInventorySidebar
            inv += "macros.showInventorySidebar = {\n";
            inv += "\thandler: function(place, macroName, params, parser) {\n";
            inv += "\t\tstate.active.variables.inventorySidebarVisible = 1;\n";
            inv += "\t}\n";
            inv += "};\n";
            inv += "\n";

            // showInventoryLink
            inv += "macros.hideInventoryLink = {\n";
            inv += "\thandler: function(place, macroName, params, parser) {\n";
            inv += "\t\tstate.active.variables.inventoryLinkVisible = 0;\n";
            inv += "\t}\n";
            inv += "};\n";
            inv += "\n";

            // hideInventorySidebar
            inv += "macros.hideInventorySidebar = {\n";
            inv += "\thandler: function(place, macroName, params, parser) {\n";
            inv += "\t\tstate.active.variables.inventorySidebarVisible = 0;\n";
            inv += "\t}\n";
            inv += "};\n";
            inv += "\n";

            return inv;
        }

        private static void generateInventory(Configuration _conf, string _path)
        {
            string inventoryPath = Path.Combine(_path, "_js_inventory.tw2");
            TextWriter twInventory = null;
            try
            {
                twInventory = new StreamWriter(inventoryPath, false, new UTF8Encoding(false));
                twInventory.WriteLine("::Inventory[script]");
                twInventory.WriteLine("");
                twInventory.WriteLine(generateInventoryScripts(_conf));
            }
            finally
            {
                if (twInventory != null)
                {
                    twInventory.Flush();
                    twInventory.Close();
                }
            }
        }

        private static string generateClothingScripts(Configuration _conf)
        {
            string cloth = "";

            cloth += "var HEAD_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("HEAD_CAP")).caption + "\";\n";
            cloth +="var HAIR_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("HAIR_CAP")).caption + "\";\n";
            cloth +="var NECK_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("NECK_CAP")).caption + "\";\n";
            cloth +="var UPPER_BODY_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("UPPER_BODY_CAP")).caption + "\";\n";
            cloth +="var LOWER_BODY_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("LOWER_BODY_CAP")).caption + "\";\n";
            cloth +="var BELT_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("BELT_CAP")).caption + "\";\n";
            cloth +="var SOCKS_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("SOCKS_CAP")).caption + "\";\n";
            cloth +="var SHOES_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("SHOES_CAP")).caption + "\";\n";
            cloth +="var UNDERWEAR_TOP_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("UNDERWEAR_TOP_CAP")).caption + "\";\n";
            cloth +="var UNDERWEAR_BOTTOM_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("UNDERWEAR_BOTTOM_CAP")).caption + "\";\n";
            cloth +="\n";

            // InitAllClothing
            cloth +="macros.initAllClothing = {\n";
            cloth +="\thandler: function(place, macroName, params, parser) {\n";
            cloth +="\t\tif (state.active.variables.allClothing === undefined)\n";
            cloth +="\t\t{\n";
            cloth +="\t\t\tstate.active.variables.allClothing = [];\n";
            for (int i = 0; i < _conf.clothing.Count; i++)
            {
                cloth += "\t\t\tstate.active.variables.allClothing.push({";
                cloth += "\"ID\":" + _conf.clothing[i].ID + ",";
                cloth += "\"name\":\"" + _conf.clothing[i].name + "\",";
                cloth += "\"description\":\"" + _conf.clothing[i].description + "\",";
                cloth += "\"canBuy\":" + _conf.clothing[i].canBeBought.ToString().ToLower() + ",";
                cloth += "\"shopCategory\":\"" + _conf.clothing[i].shopCategory + "\",";
                cloth += "\"category\":\"" + _conf.clothing[i].category + "\",";
                cloth += "\"bodyPart\":" + _conf.clothing[i].bodyPart + ",";
                cloth += "\"image\":\"" + pathSubtract(_conf.clothing[i].image, _conf.pathSubtract) + "\",";
                cloth += "\"buyPrice\":" + _conf.clothing[i].buyPrice + ",";
                cloth += "\"sellPrice\":" + _conf.clothing[i].sellPrice + ",";
                cloth += "\"isWorn\":" + _conf.clothing[i].isWornAtBeginning.ToString().ToLower() + ",";
                cloth += "\"canOwnMultiple\":" + _conf.clothing[i].canOwnMultiple.ToString().ToLower() + ",";
                cloth += "\"owned\":" + _conf.clothing[i].owned;
                if (_conf.clothingUseSkill1)
                {
                    string skill1val = (isBool(_conf.clothing[i].skill1) || isNumber(_conf.clothing[i].skill1)) ? _conf.clothing[i].skill1 : "\"" + _conf.clothing[i].skill1 + "\"";
                    cloth += ",\"" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_SKILL1_CAP")).caption + "\":" + skill1val;
                }
                if (_conf.clothingUseSkill2)
                {
                    string skill2val = (isBool(_conf.clothing[i].skill2) || isNumber(_conf.clothing[i].skill2)) ? _conf.clothing[i].skill2 : "\"" + _conf.clothing[i].skill2 + "\"";
                    cloth += ",\"" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_SKILL2_CAP")).caption + "\":" + skill2val;
                }
                if (_conf.clothingUseSkill3)
                {
                    string skill3val = (isBool(_conf.clothing[i].skill3) || isNumber(_conf.clothing[i].skill3)) ? _conf.clothing[i].skill3 : "\"" + _conf.clothing[i].skill3 + "\"";
                    cloth += ",\"" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_SKILL3_CAP")).caption + "\":" + skill3val;
                }
                cloth +=",\"isWornAtBeginning\":" + _conf.clothing[i].isWornAtBeginning.ToString().ToLower() + "});\n";
            }
            cloth +="\t\t}\n";
            cloth +="\t}\n";
            cloth +="};\n";
            cloth +="\n";

            // isWorn
            cloth +="window.is_worn = function(_id) {\n";

            cloth +="\tfor (var w in state.active.variables.wearing) {\n";

            cloth +="\t\tif (state.active.variables.wearing[w].ID == _id) return true;\n";
            cloth +="\t}\n";
            cloth +="\treturn false;\n";
            cloth +="};\n";
            cloth +="\n";

            // initCloth
            cloth +="macros.initClothing = {\n";
            cloth +="\thandler: function(place, macroName, params, parser) {\n";
            cloth +="\t\tif (state.active.variables.wearing === undefined)\n";
            cloth +="\t\t{\n";
            cloth +="\t\t\tstate.active.variables.wearing = {};\n";
            for (int i = 0; i < _conf.clothing.Count; i++)
            {
                if ((_conf.clothing[i].isWornAtBeginning) && (_conf.clothing[i].owned > 0))
                {
                    cloth +="\t\t\tstate.active.variables.wearing[" + _conf.clothing[i].bodyPart + "] = state.active.variables.allClothing[" + i + "];\n";
                }
            }
            cloth +="\t\t}\n";
            cloth +="\t}\n";
            cloth +="};\n";
            cloth +="\n";

            // initWardrobe
            cloth +="macros.initWardrobe = {\n";
            cloth +="\thandler: function(place, macroName, params, parser) {\n";
            cloth +="\t\tif (state.active.variables.wardrobe === undefined)\n";
            cloth +="\t\t{\n";
            cloth +="\t\t\tstate.active.variables.wardrobe = [];\n";
            for (int i = 0; i < _conf.clothing.Count; i++)
            {
                if ((_conf.clothing[i].isWornAtBeginning) && (_conf.clothing[i].owned > 0))
                {
                    cloth +="\t\t\tstate.active.variables.wardrobe.push(state.active.variables.allClothing[" + i + "]);\n";
                }
            }
            cloth +="\t\t}\n";
            cloth +="\t}\n";
            cloth +="};\n";
            cloth +="\n";

            // cloth
            cloth +="macros.clothing = {\n";
            cloth +="\thandler: function(place, macroName, params, parser) {\n";

            // header
            cloth += "\t\tvar s = \"<table class=\\\"clothing\\\">\";\n";
            cloth +="\t\ts +=\"<tr>\";\n";
            if (_conf.displayInClothingView.Contains("ID"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_ID_CAP")).caption + "</td>\";\n";
            if (_conf.displayInClothingView.Contains("Name"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_NAME_CAP")).caption + "</td>\";\n";
            if (_conf.displayInClothingView.Contains("Description"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_DESCRIPTION_CAP")).caption + "</td>\";\n";
            if (_conf.displayInClothingView.Contains("Category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_CATEGORY_CAP")).caption + "</td>\";\n";
            if (_conf.displayInClothingView.Contains("Shop category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_SHOP_CATEGORY_CAP")).caption + "</td>\";\n";
            if (_conf.displayInClothingView.Contains("Body part"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_BODY_PART_CAP")).caption + "</td>\";\n";
            if (_conf.displayInClothingView.Contains("Owned"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_OWNED_CAP")).caption + "</td>\";\n";
            if (_conf.displayInClothingView.Contains("Is worn"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_IS_WORN_CAP")).caption + "</td>\";\n";
            if (_conf.displayInClothingView.Contains("Can buy"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_CAN_BUY_CAP")).caption + "</td>\";\n";
            if (_conf.displayInClothingView.Contains("Buy price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_BUY_PRICE_CAP")).caption + "</td>\";\n";
            if (_conf.displayInClothingView.Contains("Sell price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_SELL_PRICE_CAP")).caption + "</td>\";\n";
            if (_conf.displayInClothingView.Contains("Can own multiple"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_CAN_OWN_MULTIPLE_CAP")).caption + "</td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInClothingView.Contains("Skill1"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_SKILL1_CAP")).caption + "</b></td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInClothingView.Contains("Skill2"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_SKILL2_CAP")).caption + "</b></td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInClothingView.Contains("Skill3"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_SKILL3_CAP")).caption + "</b></td>\";\n";
            cloth +="\t\ts +=\"<td class=\\\"clothing\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_IMAGE_CAP")).caption + "</td>\";\n";
            cloth +="\t\ts +=\"</tr>\";\n";

            // head
            cloth +="\t\ts +=\"<tr>\";\n";
            if (_conf.displayInClothingView.Contains("ID"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].ID + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Name"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].name + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Description"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].description + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].category + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Shop category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].shopCategory + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Body part"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].bodyPart + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Owned"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].owned + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Is worn"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].isWorn + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can buy"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].canBuy + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Buy price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].buyPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Sell price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].sellPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can own multiple"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].canOwnMultiple + \"</b></td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInClothingView.Contains("Skill1"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].skill1 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInClothingView.Contains("Skill2"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].skill2 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInClothingView.Contains("Skill3"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].skill3 + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Image"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[HEAD_NAME].image+\"></td>\";\n";
            cloth +="\t\ts +=\"</tr>\";\n";

            // hair
            cloth +="\t\ts +=\"<tr>\";\n";
            if (_conf.displayInClothingView.Contains("ID"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].ID + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Name"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].name + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Description"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].description + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].category + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Shop category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].shopCategory + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Body part"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].bodyPart + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Owned"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].owned + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Is worn"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].isWorn + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can buy"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].canBuy + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Buy price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].buyPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Sell price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].sellPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can own multiple"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].canOwnMultiple + \"</b></td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInClothingView.Contains("Skill1"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].skill1 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInClothingView.Contains("Skill2"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].skill2 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInClothingView.Contains("Skill3"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].skill3 + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Image"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[HAIR_NAME].image+\"></td>\";\n";
            cloth +="\t\ts +=\"</tr>\";\n";

            // neck
            cloth +="\t\ts +=\"<tr>\";\n";
            if (_conf.displayInClothingView.Contains("ID"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].ID + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Name"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].name + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Description"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].description + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].category + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Shop category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].shopCategory + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Body part"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].bodyPart + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Owned"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].owned + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Is worn"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].isWorn + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can buy"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].canBuy + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Buy price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].buyPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Sell price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].sellPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can own multiple"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].canOwnMultiple + \"</b></td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInClothingView.Contains("Skill1"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].skill1 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInClothingView.Contains("Skill2"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].skill2 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInClothingView.Contains("Skill3"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[NECK_NAME].skill3 + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Image"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[NECK_NAME].image+\"></td>\";\n";
            cloth +="\t\ts +=\"</tr>\";\n";

            // upper body
            cloth +="\t\ts +=\"<tr>\";\n";
            if (_conf.displayInClothingView.Contains("ID"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].ID + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Name"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].name + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Description"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].description + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].category + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Shop category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].shopCategory + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Body part"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].bodyPart + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Owned"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].owned + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Is worn"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].isWorn + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can buy"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].canBuy + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Buy price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].buyPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Sell price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].sellPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can own multiple"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].canOwnMultiple + \"</b></td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInClothingView.Contains("Skill1"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].skill1 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInClothingView.Contains("Skill2"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].skill2 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInClothingView.Contains("Skill3"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].skill3 + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Image"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[UPPER_BODY_NAME].image+\"></td>\";\n";
            cloth +="\t\ts +=\"</tr>\";\n";

            // lower body
            cloth +="\t\ts +=\"<tr>\";\n";
            if (_conf.displayInClothingView.Contains("ID"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].ID + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Name"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].name + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Description"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].description + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].category + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Shop category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].shopCategory + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Body part"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].bodyPart + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Owned"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].owned + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Is worn"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].isWorn + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can buy"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].canBuy + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Buy price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].buyPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Sell price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].sellPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can own multiple"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].canOwnMultiple + \"</b></td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInClothingView.Contains("Skill1"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].skill1 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInClothingView.Contains("Skill2"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].skill2 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInClothingView.Contains("Skill3"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].skill3 + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Image"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[LOWER_BODY_NAME].image+\"></td>\";\n";
            cloth +="\t\ts +=\"</tr>\";\n";

            // belt
            cloth +="\t\ts +=\"<tr>\";\n";
            if (_conf.displayInClothingView.Contains("ID"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].ID + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Name"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].name + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Description"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].description + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].category + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Shop category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].shopCategory + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Body part"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].bodyPart + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Owned"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].owned + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Is worn"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].isWorn + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can buy"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].canBuy + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Buy price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].buyPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Sell price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].sellPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can own multiple"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].canOwnMultiple + \"</b></td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInClothingView.Contains("Skill1"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].skill1 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInClothingView.Contains("Skill2"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].skill2 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInClothingView.Contains("Skill3"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[BELT_NAME].skill3 + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Image"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[BELT_NAME].image+\"></td>\";\n";
            cloth +="\t\ts +=\"</tr>\";\n";

            // socks
            cloth +="\t\ts +=\"<tr>\";\n";
            if (_conf.displayInClothingView.Contains("ID"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].ID + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Name"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].name + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Description"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].description + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].category + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Shop category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].shopCategory + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Body part"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].bodyPart + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Owned"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].owned + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Is worn"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].isWorn + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can buy"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].canBuy + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Buy price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].buyPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Sell price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].sellPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can own multiple"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].canOwnMultiple + \"</b></td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInClothingView.Contains("Skill1"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].skill1 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInClothingView.Contains("Skill2"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].skill2 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInClothingView.Contains("Skill3"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].skill3 + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Image"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[SOCKS_NAME].image+\"></td>\";\n";
            cloth +="\t\ts +=\"</tr>\";\n";

            // shoes
            cloth +="\t\ts +=\"<tr>\";\n";
            if (_conf.displayInClothingView.Contains("ID"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].ID + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Name"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].name + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Description"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].description + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].category + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Shop category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].shopCategory + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Body part"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].bodyPart + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Owned"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].owned + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Is worn"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].isWorn + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can buy"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].canBuy + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Buy price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].buyPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Sell price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].sellPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can own multiple"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].canOwnMultiple + \"</b></td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInClothingView.Contains("Skill1"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].skill1 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInClothingView.Contains("Skill2"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].skill2 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInClothingView.Contains("Skill3"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].skill3 + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Image"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[SHOES_NAME].image+\"></td>\";\n";
            cloth +="\t\ts +=\"</tr>\";\n";

            // underwear bottom
            cloth +="\t\ts +=\"<tr>\";\n";
            if (_conf.displayInClothingView.Contains("ID"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].ID + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Name"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].name + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Description"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].description + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].category + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Shop category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].shopCategory + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Body part"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].bodyPart + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Owned"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].owned + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Is worn"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].isWorn + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can buy"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].canBuy + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Buy price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].buyPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Sell price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].sellPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can own multiple"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].canOwnMultiple + \"</b></td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInClothingView.Contains("Skill1"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].skill1 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInClothingView.Contains("Skill2"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].skill2 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInClothingView.Contains("Skill3"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].skill3 + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Image"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].image+\"></td>\";\n";
            cloth +="\t\ts +=\"</tr>\";\n";

            // underwear top
            cloth +="\t\ts +=\"<tr>\";\n";
            if (_conf.displayInClothingView.Contains("ID"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].ID + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Name"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].name + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Description"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].description + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].category + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Shop category"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].shopCategory + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Body part"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].bodyPart + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Owned"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].owned + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Is worn"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].isWorn + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can buy"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].canBuy + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Buy price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].buyPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Sell price"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].sellPrice + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Can own multiple"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].canOwnMultiple + \"</b></td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInClothingView.Contains("Skill1"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].skill1 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInClothingView.Contains("Skill2"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].skill2 + \"</b></td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInClothingView.Contains("Skill3"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].skill3 + \"</b></td>\";\n";
            if (_conf.displayInClothingView.Contains("Image"))
                cloth +="\t\ts +=\"<td class=\\\"clothing\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[UNDERWEAR_TOP_NAME].image+\"></td>\";\n";
            cloth +="\t\ts +=\"</tr>\";\n";

            cloth +="\t\ts +=\"</table>\";\n";
            cloth +="\t\tnew Wikifier(place, s);\n";
            cloth +="\t}\n";
            cloth +="};\n";
            cloth +="\n";

            // clothSidebar
            cloth +="macros.clothingSidebar = {\n";
            cloth +="\thandler: function(place, macroName, params, parser) {\n";
            cloth +="\n";
            cloth +="\tnew Wikifier(place,\n";
            cloth +="\t\t\"<table class=\\\"clothing_sidebar\\\">\"+\n";
            cloth +="\t\t\"<tr><td colspan=2>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_SIDEBAR_TITLE_CAP")).caption + "</td></tr>\"+\n";
            cloth +="\t\t\"<tr><td class=\\\"clothing_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[HEAD_NAME].image+\"></td>\" +\n";
            cloth +="\t\t\"<td class=\\\"clothing_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[HAIR_NAME].image+\"></td></tr>\" +\n";
            cloth +="\t\t\"<tr><td class=\\\"clothing_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[NECK_NAME].image+\"></td>\" +\n";
            cloth +="\t\t\"<td class=\\\"clothing_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[UPPER_BODY_NAME].image+\"></td></tr>\" +\n";
            cloth +="\t\t\"<tr><td class=\\\"clothing_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[LOWER_BODY_NAME].image+\"></td>\" +\n";
            cloth +="\t\t\"<td class=\\\"clothing_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[BELT_NAME].image+\"></td></tr>\" +\n";
            cloth +="\t\t\"<tr><td class=\\\"clothing_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[SOCKS_NAME].image+\"></td>\"+\n";
            cloth +="\t\t\"<td class=\\\"clothing_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[SHOES_NAME].image+\"></td></tr>\"+\n";
            cloth +="\t\t\"<tr><td class=\\\"clothing_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].image+\"></td>\"+\n";
            cloth +="\t\t\"<td class=\\\"clothing_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[UNDERWEAR_TOP_NAME].image+\"></td></tr>\"+\n";
            cloth += "\t\t\"</table>\");\n";
            cloth +="\t}\n";
            cloth +="};\n";
            cloth +="\n";

            // wear
            cloth +="window.wear = function(_clothing) {\n";
            cloth +="\tvar cloth_obj = JSON.parse(unescape(_clothing));\n";
            cloth +="\tstate.active.variables.wearing[cloth_obj.bodyPart] = cloth_obj;\n";
            cloth += "\tstate.display(state.active.title, null, \"back\");\n";
            cloth +="};\n";
            cloth +="\n";

            // wardrobe
            cloth +="macros.wardrobe = {\n";
            cloth +="\thandler: function(place, macroName, params, parser) {\n";
            cloth +="\t\tvar wstr = \"<table class=\\\"wardrobe\\\">\";\n";
            cloth +="\t\twstr +=\"<tr>\";\n";
            if (_conf.displayInWardrobe.Contains("ID")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_ID_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInWardrobe.Contains("Name")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_NAME_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInWardrobe.Contains("Description")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_DESCRIPTION_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInWardrobe.Contains("Category")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_CATEGORY_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInWardrobe.Contains("Shop category")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_SHOP_CATEGORY_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInWardrobe.Contains("Is worn")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_IS_WORN_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInWardrobe.Contains("Can buy")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_CAN_BUY_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInWardrobe.Contains("Buy price")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_BUY_PRICE_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInWardrobe.Contains("Sell price")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_SELL_PRICE_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInWardrobe.Contains("Can own multiple")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_CAN_OWN_MULTIPLE_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInWardrobe.Contains("Body part")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_BODY_PART_CAP")).caption + "</b></td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInWardrobe.Contains("Skill1")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_SKILL1_CAP")).caption + "</b></td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInWardrobe.Contains("Skill2")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_SKILL2_CAP")).caption + "</b></td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInWardrobe.Contains("Skill3")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_SKILL3_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInWardrobe.Contains("Owned")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_OWNED_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInWardrobe.Contains("Image")) cloth +="\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_COL_IMAGE_CAP")).caption + "</b></td>\";\n";
            cloth +="\t\twstr +=\"<td></td>\";\n"; // wear / isworn
            cloth +="\t\twstr +=\"</tr>\";\n";
            cloth +="\n";
            cloth +="\t\tfor (var w = 0; w<state.active.variables.wardrobe.length; w++) {\n";
            cloth +="\t\t\twstr +=\"<tr>\";\n";
            if (_conf.displayInWardrobe.Contains("ID")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].ID + \"</td>\";\n";
            if (_conf.displayInWardrobe.Contains("Name")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].name + \"</td>\";\n";
            if (_conf.displayInWardrobe.Contains("Description")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].description + \"</td>\";\n";
            if (_conf.displayInWardrobe.Contains("Category")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].category + \"</td>\";\n";
            if (_conf.displayInWardrobe.Contains("Shop category")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].shopCategory + \"</td>\";\n";
            if (_conf.displayInWardrobe.Contains("Is worn")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].isWorn + \"</td>\";\n";
            if (_conf.displayInWardrobe.Contains("Can buy")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].canBuy + \"</td>\";\n";
            if (_conf.displayInWardrobe.Contains("Buy price")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].buyPrice + \"</td>\";\n";
            if (_conf.displayInWardrobe.Contains("Sell price")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].sellPrice + \"</td>\";\n";
            if (_conf.displayInWardrobe.Contains("Can own multiple")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].canOwnMultiple + \"</td>\";\n";
            if (_conf.displayInWardrobe.Contains("Body part")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].bodyPart + \"</td>\";\n";
            if (_conf.clothingUseSkill1 && _conf.displayInWardrobe.Contains("Skill1")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].skill1 + \"</td>\";\n";
            if (_conf.clothingUseSkill2 && _conf.displayInWardrobe.Contains("Skill2")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].skill2 + \"</td>\";\n";
            if (_conf.clothingUseSkill3 && _conf.displayInWardrobe.Contains("Skill3")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].skill3 + \"</td>\";\n";
            if (_conf.displayInWardrobe.Contains("Owned")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].owned + \"</td>\";\n";
            if (_conf.displayInWardrobe.Contains("Image")) cloth +="\t\t\twstr +=\"<td class=\\\"wardrobe\\\"><img class=\\\"paragraph\\\" src=\\\"\" + state.active.variables.wardrobe[w].image + \"\\\"></td>\";\n";
            cloth +="\n";
            cloth +="\t\t\tif (((typeof state.active.variables.wearing[state.active.variables.wardrobe[w].bodyPart] !== \"undefined\") && (state.active.variables.wearing[state.active.variables.wardrobe[w].bodyPart].ID != state.active.variables.wardrobe[w].ID)) || (typeof state.active.variables.wearing[state.active.variables.wardrobe[w].bodyPart] === \"undefined\")) {\n";
            cloth +="\t\t\t\twstr +=\"<td class=\\\"wardrobe\\\"><a onClick=\\\"wear('\"+escape(JSON.stringify(state.active.variables.wardrobe[w]))+\"');\\\" href=\\\"javascript:void(0);\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_WEAR_CAP")).caption + "</a></td></tr>\";\n";
            cloth +="\t\t\t} else {\n";
            cloth +="\t\t\t\twstr +=\"<td class=\\\"wardrobe\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTHING_IS_WORN_CAP")).caption + "</td></tr>\";\n";
            cloth +="\t\t\t}\n";
            cloth +="\t\t}\n";
            cloth +="\twstr +=\"</table>\";\n";
            cloth +="\n";
            cloth +="\tnew Wikifier(place,wstr);\n";
            cloth +="\t}\n";
            cloth +="};\n";
            cloth +="\n";

            // addToWardrobe
            cloth +="macros.addToWardrobe = {\n";
            cloth +="\thandler: function (place, macroName, params, parser) {\n";
            cloth +="\t\tif (params.length != 2) {\n";
            cloth += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two parameters, clothing ID and amount.\");\n";
            cloth +="\t\t\treturn;\n";
            cloth +="\t\t}\n";
            cloth +="\n";
            cloth +="\t\tif ((!Number.isInteger(params[0])) || (!Number.isInteger(params[1]))) {\n";
            cloth += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two integer parameters.\");\n";
            cloth +="\t\t\treturn;\n";
            cloth +="\t\t}\n";
            cloth +="\n";
            cloth +="\t\tvar clothing_in_catalog = state.active.variables.allClothing.filter(obj => {return obj.ID === params[0]});\n";
            cloth +="\n";
            cloth +="\t\tif (clothing_in_catalog.length == 0) {\n";
            cloth += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Clothing with id \" + params[0] + \" does not exist.\");\n";
            cloth +="\t\t\treturn;\n";
            cloth +="\t\t}\n";
            cloth +="\n";
            cloth +="\t\t// Clone clothing\n";
            cloth +="\t\tvar new_clothing = JSON.parse(JSON.stringify(clothing_in_catalog[0]));\n";
            cloth +="\t\tnew_clothing.owned = params[1];\n";
            cloth +="\n";
            cloth +="\t\t// clothing not yet existent?\n";
            cloth +="\t\tvar existing_clothing_in_wardrobe_with_id = state.active.variables.wardrobe.filter(obj => {return obj.ID === params[0]});\n";
            cloth +="\n";
            cloth +="\t\tif (existing_clothing_in_wardrobe_with_id.length == 0) {\n";
            cloth +="\t\t\t// add new clothing\n";
            cloth +="\t\t\tstate.active.variables.wardrobe.push(new_clothing);\n";
            cloth +="\t\t} else if (existing_clothing_in_wardrobe_with_id.length > 0) {\n";
            cloth +="\t\t\t// change owned\n";
            cloth +="\t\t\tfor (var i in state.active.variables.wardrobe) {\n";
            cloth +="\t\t\t\tif (state.active.variables.wardrobe[i].ID == new_clothing.ID) {\n";
            cloth +="\t\t\t\t\tstate.active.variables.wardrobe[i].owned += new_clothing.owned;\n";
            cloth +="\t\t\t\t\tbreak;\n";
            cloth +="\t\t\t\t}\n";
            cloth +="\t\t\t}\n";
            cloth +="\t\t} else {\n";
            cloth +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: There are several clothing with the same id \" + new_clothing.ID);\n";
            cloth +="\t\t\treturn;\n";
            cloth +="\t\t}\n";
            cloth +="\t}\n";
            cloth +="};\n";
            return cloth;
        }

        private static void generateClothing(Configuration _conf, string _path)
        {
            string clothingPath = Path.Combine(_path, "_js_clothing.tw2");
            TextWriter twClothing = null;
            try
            {
                twClothing = new StreamWriter(clothingPath, false, new UTF8Encoding(false));
                twClothing.WriteLine("::Captions[script]");
                twClothing.WriteLine("");
                twClothing.WriteLine(generateClothingScripts(_conf));
            }
            finally
            {
                if (twClothing != null)
                {
                    twClothing.Flush();
                    twClothing.Close();
                }
            }
        }

        private static string generateStatsScripts(Configuration _conf)
        {
            string stats = "";
            stats += "macros.initStats = {\n";
            stats += "\thandler: function(place, macroName, params, parser) {\n";
            stats += "\t\tstate.active.variables.stats = [];\n";

            for (int i = 0; i < _conf.stats.Count; i++)
            {
                stats += "\t\tstate.active.variables.stats.push({";
                stats += "\"ID\":" + _conf.stats[i].ID + ",";
                stats += "\"name\":\"" + _conf.stats[i].name + "\",";
                stats += "\"value\":" + _conf.stats[i].value + ",";
                stats += "\"description\":\"" + _conf.stats[i].description + "\",";
                stats += "\"image\":\"" + pathSubtract(_conf.stats[i].image, _conf.pathSubtract) + "\"});";
            }
            stats += "\t}\n";
            stats += "};\n";
            stats += "\n";

            // setStats
            stats += "macros.setStats = {\n";
            stats += "\thandler: function(place, macroName, params, parser) {\n";
            stats += "\t\tif (params.length != 2) {\n";
            stats += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two parameters, stat id and value.\");\n";
            stats += "\t\t\treturn;\n";
            stats += "\t\t}\n";
            stats += "\t\tfor (var i in state.active.variables.stats)\n";
            stats += "\t\t{\n";
            stats += "\t\t\tif (state.active.variables.stats[i].ID == params[0]) {\n";
            stats += "\t\t\t\tstate.active.variables.stats[i].value = params[1];\n";
            stats += "\t\t\t\tbreak;\n";
            stats += "\t\t\t}\n";
            stats += "\t\t}\n";
            stats += "\t}\n";
            stats += "};\n";
            stats += "\n";

            // getStats
            stats += "macros.getStats = {\n";
            stats += "\thandler: function(place, macroName, params, parser) {\n";
            stats += "\t\tif (params.length != 1) {\n";
            stats += "\t\t\tthrowError(place, \"<<\" + macroName + \" >>: expects stat id.\");\n";
            stats += "\t\t\treturn;\n";
            stats += "\t\t}\n";
            stats += "\t\tfor (var i in state.active.variables.stats)\n";
            stats += "\t\t{\n";
            stats += "\t\t\tif (state.active.variables.stats[i].ID == params[0]) {\n";
            stats += "\t\t\t\treturn state.active.variables.stats[i].value;\n";
            stats += "\t\t\t}\n";
            stats += "\t\t}\n";
            stats += "\t}\n";
            stats += "};\n";
            stats += "\n";

            // addStats
            stats += "macros.addStats = {\n";
            stats += "\thandler: function(place, macroName, params, parser) {\n";
            stats += "\t\tif (params.length != 2) {\n";
            stats += "\t\t\tthrowError(place, \" << \" + macroName + \" >>: expects two parameters, stat id and value.\");\n";
            stats += "\t\t\treturn;\n";
            stats += "\t\t}\n";
            stats += "\t\tfor (var i in state.active.variables.stats)\n";
            stats += "\t\t{\n";
            stats += "\t\t\tif (state.active.variables.stats[i].ID == params[0]) {\n";
            stats += "\t\t\t\tstate.active.variables.stats[i].value += params[1];\n";
            stats += "\t\t\t\tbreak;\n";
            stats += "\t\t\t}\n";
            stats += "\t\t}\n";
            stats += "\t}\n";
            stats += "};\n";
            stats += "\n";

            // stats
            stats += "macros.stats = {\n";
            stats += "\thandler: function(place, macroName, params, parser) {\n";
            stats += "\t\tif (state.active.variables.stats.length == 0)\n";
            stats += "\t\t{\n";
            stats += "\t\t\tnew Wikifier(place, 'No stats');\n";
            stats += "\t\t}\n";
            stats += "\t\telse\n";
            stats += "\t\t{\n";
            stats += "\t\t\tvar stats_str = \"<table class=\\\"stats\\\"><tr>\";\n";
            stats += "\t\t\tstats_str += \"<td class=\\\"stats\\\"><b>ID</b></td>\";\n";
            stats += "\t\t\tstats_str += \"<td class=\\\"stats\\\"><b>name</b></td>\";\n";
            stats += "\t\t\tstats_str += \"<td class=\\\"stats\\\"><b>value</b></td>\";\n";
            stats += "\t\t\tstats_str += \"<td class=\\\"stats\\\"><b>description</b></td>\";\n";
            stats += "\t\t\tstats_str += \"<td class=\\\"stats\\\"><b>image</b></td>\";\n";
            stats += "\t\t\tstats_str += \"</tr>\";\n";
            stats += "\t\t\tfor (var i = 0; i < state.active.variables.stats.length; i++)\n";
            stats += "\t\t\t{\n";
            stats += "\t\t\t\tstats_str += \"<tr>\";\n";
            stats += "\t\t\t\tstats_str += \"<td class=\\\"stats\\\">\" + state.active.variables.stats[i].ID + \"</td>\";\n";
            stats += "\t\t\t\tstats_str += \"<td class=\\\"stats\\\">\" + state.active.variables.stats[i].name + \"</td>\";\n";
            stats += "\t\t\t\tstats_str += \"<td class=\\\"stats\\\">\" + state.active.variables.stats[i].value + \"</td>\";\n";
            stats += "\t\t\t\tstats_str += \"<td class=\\\"stats\\\">\" + state.active.variables.stats[i].description + \"</td>\";\n";
            stats += "\t\t\t\tstats_str += \"<td class=\\\"stats\\\"><img class=\\\"paragraph\\\" src=\\\"\" + state.active.variables.stats[i].image + \"\\\"></td>\";\n";
            stats += "\t\t\t\tstats_str += \"</tr>\";\n";
            stats += "\t\t\t}\n";
            stats += "\t\t\tstats_str += \"</table>\";\n";
            stats += "\t\t\tnew Wikifier(place, stats_str);\n";
            stats += "\t\t}\n";
            stats += "\t}\n";
            stats += "};\n";
            stats += "\n";

            // statsSidebar
            stats += "macros.statsSidebar = {\n";
            stats += "\thandler: function(place, macroName, params, parser) {\n";
            stats += "\t\tif (state.active.variables.stats.length == 0)\n";
            stats += "\t\t{\n";
            stats += "\t\t\tnew Wikifier(place, 'No stats');\n";
            stats += "\t\t} else {\n";
            stats += "\t\t\tvar stats_str = \"<table class=\\\"stats_sidebar\\\">\";\n";
            stats += "\t\t\tfor (var i = 0; i < state.active.variables.stats.length; i++)\n";
            stats += "\t\t\t{\n";
            stats += "\t\t\t\tstats_str += \"<tr><td class=\\\"stats_sidebar\\\">\" + state.active.variables.stats[i].name + \"</td>\" +\n";
            stats += "\t\t\t\t\"<td class=\\\"stats\\\">\" + state.active.variables.stats[i].value + \"</td></tr>\";\n";
            stats += "\t\t\t}\n";
            stats += "\t\t\tstats_str += \"</table>\";\n";
            stats += "\t\t\tnew Wikifier(place, stats_str);\n";
            stats += "\t\t}\n";
            stats += "\t}\n";
            stats += "};\n";
            return stats;
        }

        private static void generateStats(Configuration _conf, string _path)
        {
            string statsPath = Path.Combine(_path, "_js_stats.tw2");
            TextWriter twStats = null;
            try
            {
                twStats = new StreamWriter(statsPath, false, new UTF8Encoding(false));
                twStats.WriteLine("::Stats[script]");
                twStats.WriteLine("");
                twStats.WriteLine(generateStatsScripts(_conf));
            }
            finally
            {
                if (twStats != null)
                {
                    twStats.Flush();
                    twStats.Close();
                }
            }
        }

        private static string generateNavigationScripts(Configuration _conf)
        {
            string nav = "";
            if (!_conf.navigationArrows) nav +="Config.history.controls = false;\n";
            if (_conf.debugMode) nav += "Config.debug = true;\n";
            nav += "Config.saves.slots = " + _conf.saveSlots + ";\n";
            nav += "\n";

            nav += "predisplay[\"Menu Return\"] = function (taskName) {\n";
            nav += "\tif (! tags().contains(\"noreturn\")) {\n";
            nav += "\t\tState.variables.return = passage();\n";
            nav += "\t}\n";
            nav += "};\n";
            return nav;
        }

        private static void generateNavigation(Configuration _conf, string _path)
        {
            string navigationPath = Path.Combine(_path, "_js_navigation.tw2");
            TextWriter twNavigation = null;
            try
            {
                twNavigation = new StreamWriter(navigationPath, false, new UTF8Encoding(false));
                twNavigation.WriteLine("::Navigation[script]");
                twNavigation.WriteLine("");
                twNavigation.WriteLine(generateNavigationScripts(_conf));
            }
            finally
            {
                if (twNavigation != null)
                {
                    twNavigation.Flush();
                    twNavigation.Close();
                }
            }
        }

        private static string generateDaytimeScripts(Configuration _conf)
        {
            string daytime = "";
            daytime +="function formatDate(date) {\n";
            daytime +="\tvar monthNames = [\n";
            daytime +="\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_JANUARY_CAP")).caption + "\", \n";
            daytime +="\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_FEBRUARY_CAP")).caption + "\", \n";
            daytime +="\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_MARCH_CAP")).caption + "\",\n";
            daytime +="\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_APRIL_CAP")).caption + "\", \n";
            daytime +="\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_MAY_CAP")).caption + "\", \n";
            daytime +="\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_JUNE_CAP")).caption + "\", \n";
            daytime +="\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_JULY_CAP")).caption + "\",\n";
            daytime +="\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_AUGUST_CAP")).caption + "\", \n";
            daytime +="\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_SEPTEMBER_CAP")).caption + "\", \n";
            daytime +="\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_OCTOBER_CAP")).caption + "\",\n";
            daytime +="\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_NOVEMBER_CAP")).caption + "\", \n";
            daytime +="\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_DECEMBER_CAP")).caption + "\"\n";
            daytime +="\t];\n";
            daytime +="\n";
            daytime +="\tvar day = date.getDate();\n";
            daytime +="\tvar monthIndex = date.getMonth();\n";
            daytime +="\tvar year = date.getFullYear();\n";
            daytime +="\n";
            daytime +="\treturn day + ' ' + monthNames[monthIndex] + ' ' + year;\n";
            daytime +="}\n";
            daytime +="\n";

            // initDaytime
            daytime +="macros.initDaytime = {\n";
            daytime +="\thandler: function (place, macroName, params, parser) {\n";
            daytime +="\t\tif (state.active.variables.time === undefined) {\n";
            daytime +="\t\t\tstate.active.variables.time = new Date(" + _conf.startDate.Year + "," + _conf.startDate.Month + "," +
                _conf.startDate.Day + "," + _conf.startDate.Hour + "," + _conf.startDate.Minute + "," + _conf.startDate.Second + ");\n";
            daytime +="\t\t}\n";
            daytime +="\t}\n";
            daytime +="};\n";
            daytime +="\n";

            // getTime
            daytime +="macros.getTime = {\n";
            daytime +="\thandler: function (place, macroName, params, parser) {\n";
            daytime +="\n";
            daytime +="\t\tif (state.active.variables.time === undefined) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tnew Wikifier(place, (\"0\" + state.active.variables.time.getHours()).slice(-2) + \":\" + (\"0\" + state.active.variables.time.getMinutes()).slice(-2) + \":\" + (\"0\" + state.active.variables.time.getSeconds()).slice(-2));\n";
            daytime +="\t}\n";
            daytime +="};\n";
            daytime +="\n";

            // getDate
            daytime +="macros.getDate = {\n";
            daytime +="\thandler: function (place, macroName, params, parser) {\n";
            daytime +="\t\tif (state.active.variables.time === undefined) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\t\tnew Wikifier(place, formatDate(state.active.variables.time));\n";
            daytime +="\t}\n";
            daytime +="};\n";
            daytime +="\n";

            // getDateTime
            daytime +="macros.getDateTime = {\n";
            daytime +="\thandler: function (place, macroName, params, parser) {\n";
            daytime +="\t\tif (state.active.variables.time === undefined) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tnew Wikifier(place, formatDate(state.active.variables.time) + \" \" + (\"0\" + state.active.variables.time.getHours()).slice(-2) +\n";
            daytime +="\t\t\t\":\" + (\"0\" + state.active.variables.time.getMinutes()).slice(-2) + \":\" + (\"0\" + state.active.variables.time.getSeconds()).slice(-2));\n";
            daytime +="\t}\n";
            daytime +="};\n";
            daytime +="\n";

            // getTimeOfDay
            daytime +="macros.getTimeOfDay = {\n";
            daytime +="\thandler: function (place, macroName, params, parser) {\n";
            daytime +="\t\tif (state.active.variables.time === undefined) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tif((state.active.variables.time.getHours() >= 1) && (state.active.variables.time < 4)) {\n";
            daytime +="\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_EARLY_MORNING_CAP")).caption + "\");\n";
            daytime +="\t\t} else if ((state.active.variables.time.getHours() >= 4) && (state.active.variables.time.getHours() < 6)) {\n";
            daytime +="\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_DAWN_CAP")).caption + "\");\n";
            daytime += "\t\t} else if ((state.active.variables.time.getHours() >= 6) && (state.active.variables.time.getHours() < 11)) {\n";
            daytime +="\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_MORNING_CAP")).caption + "\");\n";
            daytime += "\t\t} else if ((state.active.variables.time.getHours() >= 11) && (state.active.variables.time.getHours() < 13)) {\n";
            daytime +="\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_NOON_CAP")).caption + "\");\n";
            daytime += "\t\t} else if ((state.active.variables.time.getHours() >= 13) && (state.active.variables.time.getHours() < 16)) {\n";
            daytime +="\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_AFTERNOON_CAP")).caption + "\");\n";
            daytime += "\t\t} else if ((state.active.variables.time.getHours() >= 16) && (state.active.variables.time.getHours() < 21)) {\n";
            daytime +="\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_EVENING_CAP")).caption + "\");\n";
            daytime += "\t\t} else if ((state.active.variables.time.getHours() >= 21) && (state.active.variables.time.getHours() < 24)) {\n";
            daytime +="\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_NIGHT_CAP")).caption + "\");\n";
            daytime += "\t\t} else if ((state.active.variables.time.getHours() >= 0) && (state.active.variables.time.getHours() < 1)) {\n";
            daytime +="\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_MID_NIGHT_CAP")).caption + "\");\n";
            daytime +="\t\t}\n";
            daytime +="\t}\n";
            daytime +="};\n";
            daytime +="\n";

            // setTime
            daytime +="macros.setTime = {\n";
            daytime +="\thandler: function (place, macroName, params, parser) {\n";
            daytime +="\n";
            daytime +="\t\tif (state.active.variables.time === undefined) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tif (params.length != 3) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Expecting three parameters, hours, minutes and seconds.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tvar hours = params[0];\n";
            daytime +="\t\tvar minutes = params[1];\n";
            daytime +="\t\tvar seconds = params[2];\n";
            daytime +="\n";
            daytime +="\t\tstate.active.variables.time.setHours(hours);\n";
            daytime +="\t\tstate.active.variables.time.setMinutes(minutes);\n";
            daytime +="\t\tstate.active.variables.time.setSeconds(seconds);\n";
            daytime +="\t}\n";
            daytime +="};\n";
            daytime +="\n";

            // setDate
            daytime +="macros.setDate = {\n";
            daytime +="\thandler: function (place, macroName, params, parser) {\n";
            daytime +="\n";
            daytime +="\t\tif (state.active.variables.time === undefined) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tif (params.length != 3) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Expecting three parameters: year, month and days.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tvar year = params[0];\n";
            daytime +="\t\tvar month = params[1];\n";
            daytime +="\t\tvar day = params[2];\n";
            daytime +="\n";
            daytime +="\t\tstate.active.variables.time.setYear(year);\n";
            daytime +="\t\tstate.active.variables.time.setMonth(month);\n";
            daytime +="\t\tstate.active.variables.time.setDay(day);\n";
            daytime +="\t}\n";
            daytime +="};\n";
            daytime +="\n";

            // setDateTime
            daytime +="macros.setDateTime = {\n";
            daytime +="\thandler: function (place, macroName, params, parser) {\n";
            daytime +="\n";
            daytime +="\t\tif (state.active.variables.time === undefined) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tif (params.length != 6) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Expecting six parameters: year, month, days, hours, minutes and seconds.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tvar year = params[0];\n";
            daytime +="\t\tvar month = params[1];\n";
            daytime +="\t\tvar day = params[2];\n";
            daytime +="\n";
            daytime +="\t\tvar hours = params[3];\n";
            daytime +="\t\tvar minutes = params[4];\n";
            daytime +="\t\tvar seconds = params[5];\n";
            daytime +="\n";
            daytime +="\t\tstate.active.variables.time.setHours(hours);\n";
            daytime +="\t\tstate.active.variables.time.setMinutes(minutes);\n";
            daytime +="\t\tstate.active.variables.time.setSeconds(seconds);\n";
            daytime +="\t\tstate.active.variables.time.setHours(hours);\n";
            daytime +="\t\tstate.active.variables.time.setMinutes(minutes);\n";
            daytime +="\t\tstate.active.variables.time.setSeconds(seconds);\n";
            daytime +="\t}\n";
            daytime +="};\n";
            daytime +="\n";

            // addTimeInMinutes
            daytime +="macros.addTimeInMinutes = {\n";
            daytime +="\thandler: function (place, macroName, params, parser) {\n";
            daytime +="\n";
            daytime +="\t\tif (state.active.variables.time === undefined) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tif (params.length != 1) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Expecting minutes as first parameter.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tvar minutes = params[0];\n";
            daytime +="\n";
            daytime +="\t\tstate.active.variables.time.setMinutes(state.active.variables.time.getMinutes() + minutes);\n";
            daytime +="\t}\n";
            daytime +="};\n";
            daytime +="\n";

            // addTimeInDays
            daytime +="macros.addTimeInDays = {\n";
            daytime +="\thandler: function (place, macroName, params, parser) {\n";
            daytime +="\n";
            daytime +="\t\tif (state.active.variables.time === undefined) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tif (params.length != 1) {\n";
            daytime += "\t\t\tthrowError(place, \"<<\" + macroName + \">>: Expecting days as first parameter.\");\n";
            daytime +="\t\t\treturn;\n";
            daytime +="\t\t}\n";
            daytime +="\n";
            daytime +="\t\tvar days = params[0];\n";
            daytime +="\n";
            daytime +="\t\tstate.active.variables.time.setDays(state.active.variables.time.setDays() + 30);\n";
            daytime +="\t}\n";
            daytime +="};\n";
            return daytime;
        }

        private static void generateDaytime(Configuration _conf, string _path)
        {
            string daytimePath = Path.Combine(_path, "_js_daytime.tw2");
            TextWriter twDaytime = null;
            try
            {
                twDaytime = new StreamWriter(daytimePath, false, new UTF8Encoding(false));
                twDaytime.WriteLine("::Daytime[script]");
                twDaytime.WriteLine("");
                twDaytime.WriteLine(generateDaytimeScripts(_conf));
            }
            finally
            {
                if (twDaytime != null)
                {
                    twDaytime.Flush();
                    twDaytime.Close();
                }
            }
        }

        private static string generateShopsScripts(Configuration _conf)
        {
            string shops = "";
            // initShops
            shops +="macros.initShops = {\n";
            shops +="\thandler: function (place, macroName, params, parser) {\n";
            shops +="\n";
            shops +="\t\tif (state.active.variables.shops === undefined) {\n";
            shops +="\t\t\tstate.active.variables.shops = [];\n";

            for (int i = 0; i < _conf.shops.Count; i++)
            {
                shops += "\t\tstate.active.variables.shops.push({";
                shops += "\"ID\":" + _conf.shops[i].ID + ",";
                shops += "\"name\":\"" + _conf.shops[i].name + "\",";
                shops += "\"open\":\"" + _conf.shops[i].opening.Hour + ":" + _conf.shops[i].opening.Minute + ":" + _conf.shops[i].opening.Second + "\",";
                shops += "\"close\":\"" + _conf.shops[i].closing.Hour + ":" + _conf.shops[i].closing.Minute + ":" + _conf.shops[i].closing.Second + "\",";
                shops +="\"items\":[";

                for (int j = 0; j < _conf.shops[i].items.Count; j++)
                {
                    if (!TweeFlyPro.Properties.Settings.Default.IsProEdition && _conf.shops[i].items[j].type.Equals("CLOTHING")) continue;
                    shops += "\t\t\t{\"type\":\"" + _conf.shops[i].items[j].type + "\",";
                    shops += "\"ID\":" + _conf.shops[i].items[j].id + ",";
                    shops += "\"quantity\":" + _conf.shops[i].items[j].quantityStart + ",";
                    shops += "\"quantityMax\":" + _conf.shops[i].items[j].quantityMax + ",";
                    shops += "\"refillDelay\":" + _conf.shops[i].items[j].refillDelay + ",";

                    if (j < _conf.shops[i].items.Count - 1)
                    {
                        shops +="\"lastRefill\":new Date(-8640000000000000)},\n";
                    }
                    else
                    {
                        shops +="\"lastRefill\":new Date(-8640000000000000)}\n";
                    }
                }

                shops +="\t\t]});\n";
            }

            shops +="\t\t}\n";
            shops +="\t}\n";
            shops +="};\n";
            shops +="\n";

            // buy
            shops +="\twindow.buy = function (_item, _type, _shopId, _itemIndex) {\n";
            shops +="\tvar item_obj = JSON.parse(unescape(_item));\n";
            shops +="\n";
            shops +="\tif ((state.active.variables.money >= item_obj.buyPrice) && (state.active.variables.shops[_shopId].items[_itemIndex].quantity > 0)) {\n";
            shops +="\n";
            shops +="\t\t/*\n";
            shops +="\t\t* Buy an item\n";
            shops +="\t\t*/\n";
            shops +="\t\tif (_type.toUpperCase() === \"ITEM\") {\n";
            shops +="\t\t\t// item not yet existent?\n";
            shops +="\t\t\tvar existing_items_with_id = state.active.variables.inventory.filter(obj => {\n";
            shops +="\t\t\t\treturn obj.ID === item_obj.ID\n";
            shops +="\t\t\t});\n";
            shops +="\n";
            shops +="\t\t\tif (existing_items_with_id.length == 0) {\n";
            shops +="\t\t\t\t// add new item\n";
            shops +="\t\t\t\tstate.active.variables.inventory.push(item_obj);\n";
            shops +="\t\t\t\tstate.active.variables.money -=item_obj.buyPrice;\n";
            shops +="\t\t\t\tstate.active.variables.shops[_shopId].items[_itemIndex].quantity -=1;\n";
            shops +="\n";
            shops +="\t\t\t} else if (existing_items_with_id.length > 0) {\n";
            shops +="\t\t\t\t// change amount\n";
            shops +="\t\t\t\tfor (var i in state.active.variables.inventory) {\n";
            shops +="\t\t\t\t\tif (state.active.variables.inventory[i].ID == item_obj.ID) {\n";
            shops +="\t\t\t\t\t\tstate.active.variables.inventory[i].owned +=1;\n";
            shops +="\t\t\t\t\t\tstate.active.variables.money -=item_obj.buyPrice;\n";
            shops +="\t\t\t\t\t\tstate.active.variables.shops[_shopId].items[_itemIndex].quantity -=1;\n";
            shops +="\t\t\t\t\t\tbreak;\n";
            shops +="\t\t\t\t\t}\n";
            shops +="\t\t\t\t}\n";
            shops +="\t\t\t} else {\n";
            shops +="\t\t\t\tthrowError(null, \"<<\" + macroName + \">>: There are several items with the same id \" + item_obj.ID);\n";
            shops +="\t\t\t\treturn;\n";
            shops +="\t\t\t}\n";
            shops +="\n";
            shops +="\t\t/*\n";
            shops +="\t\t* Buy clothing\n";
            shops +="\t\t*/\n";
            shops +="\t\t} else if (_type.toUpperCase() === \"CLOTHING\") {\n";
            shops +="\t\t\t// clothing not yet existent?\n";
            shops +="\t\t\tvar existing_clothing_with_id = state.active.variables.wardrobe.filter(obj => {\n";
            shops +="\t\t\t\treturn obj.ID === item_obj.ID\n";
            shops +="\t\t\t});\n";
            shops +="\n";
            shops +="\t\t\tif (existing_clothing_with_id.length == 0) {\n";
            shops +="\t\t\t\t// add new clothing\n";
            shops +="\t\t\t\tstate.active.variables.wardrobe.push(item_obj);\n";
            shops +="\t\t\t\tstate.active.variables.money -=item_obj.buyPrice;\n";
            shops +="\t\t\t} else if (existing_clothing_with_id.length > 0) {\n";
            shops +="\t\t\t\t// change amount\n";
            shops +="\t\t\t\tfor (var i in state.active.variables.wardrobe) {\n";
            shops +="\t\t\t\t\tif (state.active.variables.wardrobe[i].ID == item_obj.ID) {\n";
            shops +="\t\t\t\t\t\tstate.active.variables.wardrobe[i].owned +=1;\n";
            shops +="\t\t\t\t\t\tstate.active.variables.money -=item_obj.buyPrice;\n";
            shops +="\t\t\t\t\t\tstate.active.variables.shops[_shopId].items[_itemIndex].quantity -=1;\n";
            shops +="\t\t\t\t\t\tbreak;\n";
            shops +="\t\t\t\t\t}\n";
            shops +="\t\t\t\t}\n";
            shops +="\t\t\t} else {\n";
            shops +="\t\t\t\tthrowError(null, \"<<\" + macroName + \">>: There are several clothing with the same id \" + item_obj.ID);\n";
            shops +="\t\t\t\treturn;\n";
            shops +="\t\t\t}\n";
            shops +="\t\t} else {\n";
            shops +="\t\t\tthrowError(null, \"buy: Unknown item type \" + item_obj.type);\n";
            shops +="\t\t\treturn;\n";
            shops +="\t\t}\n";
            shops +="\n";
            shops +="\t}\n";
            shops +="\n";
            shops +="\t// Refresh the page.\n";
            shops +="\tstate.display(state.active.title, null, \"back\");\n";
            shops +="};\n";
            shops +="\n";

            // sell
            shops +="window.sell = function (_item, _type, _shopId, _itemIndex) {\n";
            shops +="\tvar item_obj = JSON.parse(unescape(_item));\n";
            shops +="\n";
            shops +="\t/*\n";
            shops +="\t* Sell item\n";
            shops +="\t*/\n";
            shops +="\tif (_type.toUpperCase() === \"ITEM\") {\n";
            shops +="\n";
            shops +="\t\tvar existing_items_with_id = state.active.variables.inventory.filter(obj => {\n";
            shops +="\t\t\treturn obj.ID === item_obj.ID\n";
            shops +="\t\t});\n";
            shops +="\t\tif (existing_items_with_id.length == 0)\n";
            shops +="\t\t\treturn;\n";
            shops +="\n";
            shops +="\t\t\t// Delete a special amount from inventory\n";
            shops +="\t\t\tfor (var i in state.active.variables.inventory) {\n";
            shops +="\t\t\t\tif (state.active.variables.inventory[i].ID == item_obj.ID) {\n";
            shops +="\t\t\t\t\tstate.active.variables.inventory[i].owned -= 1;\n";
            shops +="\t\t\t\t\tstate.active.variables.money +=item_obj.sellPrice;\n";
            shops +="\t\t\t\t\tstate.active.variables.shops[_shopId].items[_itemIndex].quantity +=1;\n";
            shops +="\n";
            shops +="\t\t\t\t\t// Delete item completely if owned <= 0\n";
            shops +="\t\t\t\t\tif (state.active.variables.inventory[i].owned <= 0) {\n";
            shops +="\t\t\t\t\t\tstate.active.variables.inventory.splice(i, 1);\n";
            shops +="\t\t\t\t\t}\n";
            shops +="\t\t\t\t\tbreak;\n";
            shops +="\t\t\t\t}\n";
            shops +="\t\t\t}\n";
            shops +="\n";
            shops +="\t\t\t/*\n";
            shops +="\t\t\t* Sell clothing\n";
            shops +="\t\t\t*/\n";
            shops +="\t\t\t} else if (_type.toUpperCase() === \"CLOTHING\") {\n";
            shops +="\t\t\t\tvar existing_items_with_id = state.active.variables.wardrobe.filter(obj => {\n";
            shops +="\t\t\t\t\treturn obj.ID === item_obj.ID\n";
            shops +="\t\t\t});\n";
            shops +="\t\t\tif (existing_items_with_id.length == 0)\n";
            shops +="\t\t\t\treturn;\n";
            shops +="\n";
            shops +="\t\t\t\t// TODO Check if wearing\n";
            shops +="\n";
            shops +="\t\t\t\t// Delete a special amount from clothing\n";
            shops +="\t\t\t\tfor (var i in state.active.variables.wardrobe) {\n";
            shops +="\t\t\t\tif (state.active.variables.wardrobe[i].ID == item_obj.ID) {\n";
            shops +="\t\t\t\t\tstate.active.variables.wardrobe[i].owned -= 1;\n";
            shops +="\t\t\t\t\tstate.active.variables.money +=item_obj.sellPrice;\n";
            shops +="\n";
            shops +="\t\t\t\t\t// Delete item completely if owned <= 0\n";
            shops +="\t\t\t\t\tif (state.active.variables.wardrobe[i].owned <= 0) {\n";
            shops +="\t\t\t\t\t\tstate.active.variables.wardrobe.splice(i, 1);\n";
            shops +="\t\t\t\t\t}\n";
            shops +="\t\t\t\t\tbreak;\n";
            shops +="\t\t\t\t}\n";
            shops +="\t\t\t}\n";
            shops +="\t\t}\n";
            shops +="\n";
            shops +="\t// Refresh page\n";
            shops +="\tstate.display(state.active.title, null, \"back\");\n";
            shops +="};\n";

            // shop
            shops +="macros.shop = {\n";
            shops +="\thandler: function (place, macroName, params, parser) {\n";
            shops +="\n";
            shops +="\t\tif (params.length != 1) {\n";
            shops +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects shop id as parameter\");\n";
            shops +="\t\t\treturn;\n";
            shops +="\t\t}\n";
            shops +="\n";
            shops +="\t\tif (state.active.variables.shops[params[0]] === undefined) {\n";
            shops +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: There is no shop with the id \" + params[0]);\n";
            shops +="\t\t\treturn;\n";
            shops +="\t\t}\n";
            shops +="\n";
            shops +="\t\t// Is open?\n";
            shops +="\t\tif ((state.active.variables.time !== undefined) && (state.active.variables.time <= state.active.variables.shops[params[0]].open) && (state.active.variables.time >= state.active.variables.shops[params[0]].close)) {\n";
            shops +="\t\t\tnew Wikifier(place, \"Shop is closed.\");\n";
            shops +="\t\t\treturn;\n";
            shops +="\t\t}\n";
            shops +="\n";
            shops +="\t\tif (state.active.variables.shops[params[0]].items.length == 0) {\n";
            shops +="\t\t\tnew Wikifier(place, '" + _conf.captions.Single(s => s.captionName.Equals("SHOP_NO_ITEMS_CAP")).caption + "');\n";
            shops +="\t\t} else {\n";
            shops +="\t\t\tvar shop_str = \"<table class=\\\"shop\\\"><tr>\";\n";
            if (_conf.itemPropertiesInShops.Contains("ID")) shops +="\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_ID_CAP")).caption + "</b></td>\";\n";
            if (_conf.itemPropertiesInShops.Contains("Name")) shops +="\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_NAME_CAP")).caption + "</b></td>\";\n";
            if (_conf.itemPropertiesInShops.Contains("Quantity")) shops +="\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_QUANTITY_CAP")).caption + "</b></td>\";\n";
            if (_conf.itemPropertiesInShops.Contains("Buy")) shops +="\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_BUY_CAP")).caption + "</b></td>\";\n";
            if (_conf.itemPropertiesInShops.Contains("Sell")) shops +="\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_SELL_CAP")).caption + "</b></td>\";\n";
            if (_conf.itemPropertiesInShops.Contains("Skill1") && _conf.shopUseSkill1) shops +="\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_SKILL1_CAP")).caption + "</b></td>\";\n";
            if (_conf.itemPropertiesInShops.Contains("Skill2") && _conf.shopUseSkill2) shops +="\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_SKILL2_CAP")).caption + "</b></td>\";\n";
            if (_conf.itemPropertiesInShops.Contains("Skill3") && _conf.shopUseSkill3) shops +="\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_SKILL3_CAP")).caption + "</b></td>\";\n";
            if (_conf.itemPropertiesInShops.Contains("Image")) shops +="\t\t\tshop_str +=\"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_IMAGE_CAP")).caption + "</b></td>\";\n";
            shops +="\t\t\tshop_str +=\"</tr>\";\n";
            shops +="\n";
            shops +="\t\t\tfor (var i=0; i<state.active.variables.shops[params[0]].items.length; i++) {\n";
            shops +="\n";
            shops +="\t\t\t\tif (state.active.variables.shops[params[0]].items[i].quantity < state.active.variables.shops[params[0]].items[i].maxQuantity) {\n";
            shops +="\t\t\t\t\tvar minutes_diff = Math.floor(((Math.abs(state.active.variables.time - state.active.variables.shops[params[0]].items[i].lastRefill))/1000)/60);				\n";
            shops +="\t\t\t\t\tif (minutes_diff > 0) {\n";
            shops +="\t\t\t\t\t\tvar items_after_refill = state.active.variables.shops[params[0]].items[i].quantity + Math.floor(minutes_diff / state.active.variables.shops[params[0]].items[i].refillDelay);\n";
            shops +="\t\t\t\t\t\tstate.active.variables.shops[params[0]].items[i].quantity = max(state.active.variables.shops[params[0]].items[i].maxQuantity, items_after_refill);\n";
            shops +="\t\t\t\t\t\tstate.active.variables.shops[params[0]].items[i].lastRefill = state.active.variables.time;\n";
            shops +="\t\t\t\t\t}\n";
            shops +="\t\t\t\t}\n";
            shops +="\t\t\t\tvar existing_items_with_id = [];\n";
            shops +="\t\t\t\tvar object_owned = [];\n";
            shops +="\t\t\t\tvar is_clothing_worn = false;\n";
            shops +="\t\t\t\tif (state.active.variables.shops[params[0]].items[i].type.toUpperCase() === \"ITEM\") {\n";
            shops +="\n";
            shops +="\t\t\t\t\t// Get the item in the list of all items\n";
            shops +="\t\t\t\t\texisting_items_with_id = state.active.variables.items.filter(obj => {return obj.ID === state.active.variables.shops[params[0]].items[i].ID});\n";
            shops +="\n";
            shops +="\t\t\t\t\t// Get the items owned by the player\n";
            shops +="\t\t\t\t\tif (existing_items_with_id.length > 0) {\n";
            shops +="\t\t\t\t\t\tobject_owned = state.active.variables.inventory.filter(obj => {return obj.ID === existing_items_with_id[0].ID});\n";
            shops +="\t\t\t\t\t}\n";
            shops +="\t\t\t\t} else if (state.active.variables.shops[params[0]].items[i].type.toUpperCase() === \"CLOTHING\") {\n";
            shops +="\n";
            shops +="\t\t\t\t\t// Get the clothing in the list of all clothings\n";
            shops +="\t\t\t\t\texisting_items_with_id = state.active.variables.allClothing.filter(obj => {return obj.ID === state.active.variables.shops[params[0]].items[i].ID});\n";
            shops +="\n";
            shops +="\t\t\t\t\t// Get the clothing owned by the player\n";
            shops +="\t\t\t\t\tif (existing_items_with_id.length > 0) {\n";
            shops +="\t\t\t\t\t\tobject_owned = state.active.variables.wardrobe.filter(obj => {return obj.ID === existing_items_with_id[0].ID});\n";
            shops +="\n";
            shops +="\t\t\t\t\t\t// Check if cloth is worn\n";
            shops +="\t\t\t\t\t\tis_clothing_worn = is_worn(existing_items_with_id[0].ID);\n";
            shops +="\t\t\t\t\t}\n";
            shops +="\t\t\t\t}\n";
            shops +="\n";
            shops +="\t\t\t\tif (existing_items_with_id.length == 0) {\n";
            shops +="\t\t\t\t\tthrowError(place, \"<<\" + macroName + \">>: There is no item from type \" + state.active.variables.shops[params[0]].items[i].type + \" with ID \" + state.active.variables.shops[params[0]].items[i].ID);\n";
            shops +="\t\t\t\t\treturn;\n";
            shops +="\t\t\t\t}\n";
            shops +="\n";
            shops +="\t\t\t\tshop_str += \"<tr>\";\n";
            if (_conf.itemPropertiesInShops.Contains("ID"))
                shops +="\t\t\t\tshop_str += \"<td class=\\\"shop\\\">\" + state.active.variables.shops[params[0]].items[i].ID + \"</td>\";\n";
            if (_conf.itemPropertiesInShops.Contains("Name"))
                shops +="\t\t\t\tshop_str += \"<td class=\\\"shop\\\">\" + existing_items_with_id[0].name + \"</td>\";\n";
            if (_conf.itemPropertiesInShops.Contains("Quantity"))
                shops +="\t\t\t\tshop_str += \"<td class=\\\"shop\\\">\" + state.active.variables.shops[params[0]].items[i].quantity + \"</td>\";\n";

            if (_conf.itemPropertiesInShops.Contains("Skill1") && _conf.shopUseSkill1)
                shops +="\t\t\t\tshop_str +=\"<td class=\\\"shop\\\">\" + existing_items_with_id[0].skill1 + \"</td>\";\n";
            if (_conf.itemPropertiesInShops.Contains("Skill2") && _conf.shopUseSkill2)
                shops +="\t\t\t\tshop_str +=\"<td class=\\\"shop\\\">\" + existing_items_with_id[0].skill2 + \"</td>\";\n";
            if (_conf.itemPropertiesInShops.Contains("Skill3") && _conf.shopUseSkill3)
                shops +="\t\t\t\tshop_str +=\"<td class=\\\"shop\\\">\" + existing_items_with_id[0].skill3 + \"</td>\";	\n";
            shops +="\n";
            shops +="\t\t\t\t// buy\n";

            if (_conf.itemPropertiesInShops.Contains("Buy"))
            {
                shops +="\t\t\t\tif ((state.active.variables.money >= existing_items_with_id[0].buyPrice) && (state.active.variables.shops[params[0]].items[i].quantity > 0)) {\n";
                shops +="\t\t\t\tshop_str += \"<td class=\\\"shop\\\"><a onClick=\\\"buy('\"+escape(JSON.stringify(existing_items_with_id[0]))+\n";
                shops += "\t\t\t\t\t\"','\"+state.active.variables.shops[params[0]].items[i].type+\"',\" +params[0]+ \",\" +i+\");\\\" href=\\\"javascript:void(0);\\\">buy for \" + existing_items_with_id[0].buyPrice + \"\" + currency +\"</a></td>\";\n";
                shops +="\t\t\t\t} else {\n";
                shops +="\t\t\t\t\tshop_str += \"<td class=\\\"shop\\\">buy for \" + existing_items_with_id[0].buyPrice + \"\" + currency +\"</td>\";\n";
                shops +="\t\t\t\t}\n";
                shops +="\n";
            }

            if (_conf.itemPropertiesInShops.Contains("Sell"))
            {
                shops +="\t\t\t\t// sell\n";
                shops +="\t\t\t\tvar min_owned = (is_clothing_worn) ? 1 : 0;\n";
                shops +="\t\t\t\tif ((object_owned.length > 0) && (object_owned[0].owned > min_owned)) {\n";
                shops +="\t\t\t\t\tshop_str += \"<td class=\\\"shop\\\"><a onClick=\\\"sell('\"+escape(JSON.stringify(existing_items_with_id[0]))+\n";
                shops += "\t\t\t\t\t\t\"','\"+state.active.variables.shops[params[0]].items[i].type+\"',\" +params[0]+ \",\" +i+\");\\\" href=\\\"javascript:void(0);\\\">sell for \" + existing_items_with_id[0].sellPrice + \"\" + currency + \"</a></td>\";\n";
                shops +="\t\t\t\t} else {\n";
                shops +="\n";
                shops +="\t\t\t\t\t// Is player wearing that clothing?\n";
                shops +="\t\t\t\t\tif ((object_owned.length > 0) && (object_owned[0].owned == 1) && (is_clothing_worn)) {\n";
                shops +="\t\t\t\t\t\tshop_str += \"<td class=\\\"shop\\\">sell for \" + existing_items_with_id[0].sellPrice + \"\" + currency + \"<br>(You are wearing that)</td>\";\n";
                shops +="\t\t\t\t\t} else {\n";
                shops +="\t\t\t\t\t\tshop_str += \"<td class=\\\"shop\\\">sell for \" + existing_items_with_id[0].sellPrice + \"\" + currency + \"</td>\";\n";
                shops +="\t\t\t\t\t}\n";
                shops +="\t\t\t\t}\n";
                shops +="\n";
            }

            if (_conf.itemPropertiesInShops.Contains("Image"))
                shops +="\t\t\t\tshop_str += \"<td class=\\\"shop\\\"><img class=\\\"paragraph\\\" src=\\\"\" + existing_items_with_id[0].image + \"\\\"></td></tr>\";\n";
            shops +="\t\t\t}\n";
            shops +="\t\t\tshop_str +=\"</table>\";\n";
            shops +="\t\t\tnew Wikifier(place, shop_str);\n";
            shops +="\t\t}\n";
            shops +="\t}\n";
            shops +="};\n";
            return shops;
        }

        private static void generateShops(Configuration _conf, string _path)
        {
            string shopsPath = Path.Combine(_path, "_js_shops.tw2");
            TextWriter twShops = null;
            try
            {
                twShops = new StreamWriter(shopsPath, false, new UTF8Encoding(false));
                twShops.WriteLine("::Shops[script]");
                twShops.WriteLine("");
                twShops.WriteLine(generateShopsScripts(_conf));
            }
            finally
            {
                if (twShops != null)
                {
                    twShops.Flush();
                    twShops.Close();
                }
            }
        }

        private static string generateMoneyScripts(Configuration _conf)
        {
            string money = "";
            money += "var currency = \"" + _conf.captions.Single(s => s.captionName.Equals("MONEY_UNIT_CAP")).caption + "\";\n";
            money += "\n";

            // initMoney
            money += "macros.initMoney = {\n";
            money += "\thandler: function(place, macroName, params, parser) {\n";
            money += "\t\tif (state.active.variables.money === undefined)\n";
            money += "\t\t{\n";
            money += "\t\t\tstate.active.variables.money = " + _conf.startMoney + ";\n";
            money += "\t\t\tstate.active.variables.moneyPerDay = " + _conf.moneyPerDay + ";\n";
            money += "\t\t\tstate.active.variables.lastMoneyUpdate = new Date(-8640000000000000);\n";
            money += "\t\t}\n";
            money += "\t}\n";
            money += "};\n";
            money += "\n";

            // printMoney
            money += "macros.printMoney = {\n";
            money += "\thandler: function(place, macroName, params, parser) {\n";
            money += "\t\tif (state.active.variables.money != undefined)\n";
            money += "\t\t{\n";
            money += "\t\t\t// Update money if \"money per day\" has to be added.\n";
            money += "\t\t\tvar dayDiff = Math.floor((state.active.variables.time - state.active.variables.lastMoneyUpdate) / 86400000);\n";
            money += "\t\t\tif (dayDiff > 0)\n";
            money += "\t\t\t{\n";
            money += "\t\t\t\tstate.active.variables.money += state.active.variables.moneyPerDay;\n";
            money += "\t\t\t\tstate.active.variables.lastMoneyUpdate = state.active.variables.time;\n";
            money += "\t\t\t}\n";
            money += "\t\t\tnew Wikifier(place, \"\"+state.active.variables.money);\n";
            money += "\t\t} else {\n";
            money += "\t\t\tthrowError(place, \" << \" + macroName + \">>: please call initMoney first.\");\n";
            money += "\t\t\treturn;\n";
            money += "\t\t}\n";
            money += "\t}\n";
            money += "};\n";
            return money;
        }

        private static void generateMoney(Configuration _conf, string _path)
        {
            string moneyPath = Path.Combine(_path, "_js_money.tw2");
            TextWriter twMoney = null;
            try
            {
                twMoney = new StreamWriter(moneyPath, false, new UTF8Encoding(false));
                twMoney.WriteLine("::Money[script]");
                twMoney.WriteLine("");
                twMoney.WriteLine(generateMoneyScripts(_conf));
            }
            finally
            {
                if (twMoney != null)
                {
                    twMoney.Flush();
                    twMoney.Close();
                }
            }
        }

        private static string generateJobsScripts(Configuration _conf)
        {
            string jobs = "";
            jobs +="macros.initJobs = {\n";
            jobs +="\thandler: function(place, macroName, params, parser) {\n";
            jobs +="\n";
            jobs +="\t\tif (state.active.variables.jobs === undefined) {\n";
            jobs +="\t\t\tstate.active.variables.jobs = [];\n";

            for (int i = 0; i < _conf.jobs.Count; i++)
            {
                jobs +="\t\t\tstate.active.variables.jobs.push({";
                jobs +="\"ID\":" + _conf.jobs[i].ID + ",";
                jobs +="\"name\":\"" + _conf.jobs[i].name + "\",";
                jobs +="\"description\":\"" + _conf.jobs[i].description + "\",";
                jobs +="\"category\":\"" + _conf.jobs[i].category + "\",";
                jobs +="\"available\":" + _conf.jobs[i].available.ToString().ToLower() + ",";
                jobs +="\"rewardMoney\":" + _conf.jobs[i].rewardMoney + ",";
                jobs +="\"cooldown\":" + _conf.jobs[i].cooldown + ",";
                jobs +="\"lastStart\":new Date(0, 0, 0, 0, 0, 0),";
                jobs +="\"duration\":" + _conf.jobs[i].duration + ",";
                jobs +="\"image\":\"" + pathSubtract(_conf.jobs[i].image, _conf.pathSubtract) + "\",";
                jobs +="\"rewardItems\":[\n";
                for (int j = 0; j < _conf.jobs[i].rewardItems.Count; j++)
                {
                    if (j < _conf.jobs[i].rewardItems.Count - 1)
                    {
                        jobs +="\t\t\t\t{\"type\":\"" + _conf.jobs[i].rewardItems[j].type + "\", \"ID\":" + _conf.jobs[i].rewardItems[j].ID + ",\"amount\":" + _conf.jobs[i].rewardItems[j].amount + "},\n";
                    }
                    else
                    {
                        jobs +="\t\t\t\t{\"type\":\"" + _conf.jobs[i].rewardItems[j].type + "\", \"ID\":" + _conf.jobs[i].rewardItems[j].ID + ",\"amount\":" + _conf.jobs[i].rewardItems[j].amount + "}\n";
                    }
                }
                jobs +="\t\t\t]});\n";
            }

            jobs +="\t\t}\n";
            jobs +="\t}\n";
            jobs +="};\n";
            jobs +="\n";

            // doJob
            jobs +="window.doJob = function (_job) {\n";
            jobs +="\n";
            jobs +="\tvar job_obj = JSON.parse(unescape(_job));\n";
            jobs +="\n";
            jobs +="\t// Add time\n";
            jobs +="\tif (state.active.variables.time !== undefined) {\n";
            jobs +="\t\tstate.active.variables.time.setMinutes(state.active.variables.time.getMinutes() + job_obj.duration);\n";
            jobs +="\t} else {\n";
            jobs +="\t\tthrowError(null, \"Time system has not been initialized and the cooldown can not be applied to job system.\");\n";
            jobs +="\t}\n";
            jobs +="\n";
            jobs +="\t// Give reward\n";
            jobs +="\tif (state.active.variables.time !== undefined) {\n";
            jobs +="\t\tstate.active.variables.money +=job_obj.rewardMoney;\n";
            jobs +="\t} else {\n";
            jobs +="\t\tthrowError(null, \"Money system has not been initialized so no reward money can be given for doing job.\");\n";
            jobs +="\t}\n";
            jobs +="\n";
            jobs +="\t// Add reward items\n";
            jobs +="\tif (state.active.variables.inventory !== undefined) {\n";
            jobs +="\t\tfor (var i=0; i<job_obj.rewardItems.length; i++) {\n";
            jobs +="\t\t\tif (job_obj.rewardItems[i].type === \"ITEM\") {\n";
            jobs +="\n";
            jobs +="\t\t\t\tvar item_in_catalog = state.active.variables.items.filter(obj => {return obj.ID === job_obj.rewardItems[i].ID});\n";
            jobs +="\t\t\t\tif (item_in_catalog.length != 1) {\n";
            jobs +="\t\t\t\t\tthrowError(place, \"<<\" + macroName + \">>: There must be exactly one item of id \" + params[0] + \" in the item catalog but there are \" + item_in_catalog.length);\n";
            jobs +="\t\t\t\t\treturn;\n";
            jobs +="\t\t\t\t}\n";
            jobs +="\t\t\t\tvar item = JSON.parse(JSON.stringify(item_in_catalog[0]));\n";
            jobs +="\t\t\t\titem.owned = job_obj.rewardItems[i].amount;\n";
            jobs +="\n";
            jobs +="\t\t\t\t// item not yet existent?\n";
            jobs +="\t\t\t\tvar existing_items_with_id = state.active.variables.inventory.filter(obj => {\n";
            jobs +="\t\t\t\t\treturn obj.ID === item.ID\n";
            jobs +="\t\t\t\t});\n";
            jobs +="\n";
            jobs +="\t\t\t\tif (existing_items_with_id.length == 0) {\n";
            jobs +="\t\t\t\t\t// add new item\n";
            jobs +="\t\t\t\t\tstate.active.variables.inventory.push(item);\n";
            jobs +="\t\t\t\t} else if (existing_items_with_id.length > 0) {\n";
            jobs +="\t\t\t\t\t// change owned\n";
            jobs +="\t\t\t\t\tfor (var i in state.active.variables.inventory) {\n";
            jobs +="\t\t\t\t\t\tif (state.active.variables.inventory[i].ID == item.ID) {\n";
            jobs +="\t\t\t\t\t\t\tstate.active.variables.inventory[i].owned += item.owned;\n";
            jobs +="\t\t\t\t\t\t\tbreak;\n";
            jobs +="\t\t\t\t\t\t}\n";
            jobs +="\t\t\t\t\t}\n";
            jobs +="\t\t\t\t} else {\n";
            jobs +="\t\t\t\t\tthrowError(place, \"<<\" + macroName + \">>: There are several items with the same id \" + item.ID);\n";
            jobs +="\t\t\t\t\treturn;\n";
            jobs +="\t\t\t\t}\n";
            jobs +="\t\t\t} else if (job_obj.rewardItems[i].type === \"CLOTHING\") {\n";
            jobs +="\t\t\t\tvar clothing_in_catalog = state.active.variables.allClothing.filter(obj => {return obj.ID === job_obj.rewardItems[i].ID});\n";
            jobs +="\n";
            jobs +="\t\t\t\tif (clothing_in_catalog.length == 0) {\n";
            jobs +="\t\t\t\t\tthrowError(place, \"<<\" + macroName + \">>: Clothing with id \" + job_obj.rewardItems[i].ID + \" does not exist.\");\n";
            jobs +="\t\t\t\t\treturn;\n";
            jobs +="\t\t\t\t}\n";
            jobs +="\n";
            jobs +="\t\t\t\t// Clone clothing\n";
            jobs +="\t\t\t\tvar new_clothing = JSON.parse(JSON.stringify(clothing_in_catalog[0]));\n";
            jobs +="\t\t\t\tnew_clothing.owned = job_obj.rewardItems[i].amount;\n";
            jobs +="\n";
            jobs +="\t\t\t\t// clothing not yet existent?\n";
            jobs +="\t\t\t\tvar existing_clothing_in_wardrobe_with_id = state.active.variables.wardrobe.filter(obj => {return obj.ID === job_obj.rewardItems[i].ID});\n";
            jobs +="\n";
            jobs +="\t\t\t\tif (existing_clothing_in_wardrobe_with_id.length == 0) {\n";
            jobs +="\t\t\t\t\t// add new clothing\n";
            jobs +="\t\t\t\t\tstate.active.variables.wardrobe.push(new_clothing);\n";
            jobs +="\t\t\t\t} else if (existing_clothing_in_wardrobe_with_id.length > 0) {\n";
            jobs +="\t\t\t\t\t// change owned\n";
            jobs +="\t\t\t\t\tfor (var i in state.active.variables.wardrobe) {\n";
            jobs +="\t\t\t\t\t\tif (state.active.variables.wardrobe[i].ID == new_clothing.ID) {\n";
            jobs +="\t\t\t\t\t\t\tstate.active.variables.wardrobe[i].owned += new_clothing.owned;\n";
            jobs +="\t\t\t\t\t\t\tbreak;\n";
            jobs +="\t\t\t\t\t\t}\n";
            jobs +="\t\t\t\t\t}\n";
            jobs +="\t\t\t\t} else {\n";
            jobs +="\t\t\t\t\tthrowError(place, \"<<\" + macroName + \">>: There are several clothing with the same id \" + new_clothing.ID);\n";
            jobs +="\t\t\t\t\treturn;\n";
            jobs +="\t\t\t\t}\n";
            jobs +="\t\t\t}\n";
            jobs +="\t\t}\n";
            jobs +="\n";
            jobs +="\t\t// Set cooldown\n";
            jobs +="\t\tvar job_by_id = state.active.variables.jobs.filter(obj => {return obj.ID == job_obj.ID});\n";
            jobs +="\t\tif (job_by_id.length == 1) {\n";
            jobs +="\t\t\tjob_by_id[0].lastStart = state.active.variables.time;\n";
            jobs +="\t\t}\n";
            jobs +="\t} else {\n";
            jobs +="\t\tthrowError(null, \"Inventory system has not been initialized so no reward items can be given for doing job.\");\n";
            jobs +="\t}\n";
            jobs +="\tstate.display(state.active.title, null, \"back\");\n";
            jobs +="};\n";
            jobs +="\n";

            // showJobs
            jobs +="macros.showJobs = {\n";
            jobs +="\thandler: function(place, macroName, params, parser) {\n";
            jobs +="\n";
            jobs +="\t\tif (params.length < 1) {\n";
            jobs +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: showJobs expects an array of job IDs as first parameter.\");\n";
            jobs +="\t\t\treturn;\n";
            jobs +="\t\t}\n";
            jobs +="\n";
            jobs +="\t\tif (state.active.variables.jobs === undefined) {\n";
            jobs +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: please call initJobs first.\");\n";
            jobs +="\t\t\treturn;\n";
            jobs +="\t\t}\n";
            jobs +="\n";
            jobs +="\t\tvar jobs_str = \"<table class=\\\"jobs\\\"><tr>\";\n";
            if (_conf.displayInJobsView.Contains("ID")) jobs +="\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_ID_CAP")).caption + "</th>\";\n";
            if (_conf.displayInJobsView.Contains("Name")) jobs +="\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_NAME_CAP")).caption + "</th>\";\n";
            if (_conf.displayInJobsView.Contains("Description")) jobs +="\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_DESCRIPTION_CAP")).caption + "</th>\";\n";
            if (_conf.displayInJobsView.Contains("Category")) jobs +="\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_CATEGORY_CAP")).caption + "</th>\";\n";
            if (_conf.displayInJobsView.Contains("Available")) jobs +="\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_AVAILABLE_CAP")).caption + "</th>\";\n";
            if (_conf.displayInJobsView.Contains("RewardMoney")) jobs +="\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_REWARD_MONEY_CAP")).caption + "</th>\";\n";
            if (_conf.displayInJobsView.Contains("Cooldown")) jobs +="\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_COOLDOWN_CAP")).caption + "</th>\";\n";
            if (_conf.displayInJobsView.Contains("LastStart")) jobs +="\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_LAST_START_CAP")).caption + "</th>\";\n";
            if (_conf.displayInJobsView.Contains("Duration")) jobs +="\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_DURATION_CAP")).caption + "</th>\";\n";
            if (_conf.displayInJobsView.Contains("Image")) jobs +="\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_IMAGE_CAP")).caption + "</th>\";\n";
            jobs +="\t\tjobs_str +=\"<th class=\\\"jobs\\\">start</th>\";\n";
            jobs +="\t\tjobs_str +=\"</tr>\";\n";
            jobs +="\n";
            jobs +="\t\tfor(var i=0; i<params.length; i++) {\n";
            jobs +="\t\t\tvar job_by_id = state.active.variables.jobs.filter(obj => {return obj.ID == params[i]});\n";
            jobs +="\n";
            jobs +="\t\t\tif (job_by_id.length == 1) {\n";
            jobs +="\n";
            jobs +="\t\t\t\t// Check if job cooldown is passed\n";
            jobs +="\t\t\t\tvar minutes_diff = Math.floor(((Math.abs(state.active.variables.time - job_by_id[0].lastStart))/1000)/60);\n";
            jobs +="\n";
            jobs +="\t\t\t\tjobs_str +=\"<tr>\";\n";
            if (_conf.displayInJobsView.Contains("ID")) jobs +="\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].ID + \"</td>\";\n";
            if (_conf.displayInJobsView.Contains("Name")) jobs +="\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].name + \"</td>\";\n";
            if (_conf.displayInJobsView.Contains("Description")) jobs +="\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].description + \"</td>\";\n";
            if (_conf.displayInJobsView.Contains("Category")) jobs +="\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].category + \"</td>\";\n";
            if (_conf.displayInJobsView.Contains("Available")) jobs +="\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].avaiable + \"</td>\";\n";
            if (_conf.displayInJobsView.Contains("RewardMoney")) jobs +="\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].rewardMoney + \"</td>\";\n";
            if (_conf.displayInJobsView.Contains("Cooldown")) jobs +="\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].cooldown + \"</td>\";\n";
            if (_conf.displayInJobsView.Contains("LastStart")) jobs +="\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].lastStart + \"</td>\";\n";
            if (_conf.displayInJobsView.Contains("Duration")) jobs +="\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].duration + \"</td>\";\n";
            if (_conf.displayInJobsView.Contains("Image")) jobs +="\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\"><img class=\\\"paragraph\\\" src=\\\"\" + job_by_id[0].image + \"\\\"></td>\";\n";
            jobs +="\n";
            jobs +="\t\t\t\tif ((minutes_diff >= job_by_id[0].cooldown) && (job_by_id[0].available)) {\n";
            jobs +="\t\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\"><a onClick=\\\"doJob('\"+escape(JSON.stringify(job_by_id[0]))+\"');\\\" href=\\\"javascript:void(0);\\\">Start</a></td>\";\n";
            jobs +="\t\t\t\t} else {\n";
            jobs +="\t\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">Not ready</td>\";\n";
            jobs +="\t\t\t\t}\n";
            jobs +="\n";
            jobs +="\t\t\t\tjobs_str +=\"</tr>\";\n";
            jobs +="\t\t\t}\n";
            jobs +="\t\t}\n";
            jobs +="\n";
            jobs +="\t\tjobs_str +=\"</table>\";\n";
            jobs +="\t\tnew Wikifier(place, jobs_str);\n";
            jobs +="\t}\n";
            jobs +="};\n";
            return jobs;
        }

        private static void generateJobs(Configuration _conf, string _path)
        {
            string jobsPath = Path.Combine(_path, "_js_jobs.tw2");
            TextWriter twJobs = null;
            try
            {
                twJobs = new StreamWriter(jobsPath, false, new UTF8Encoding(false));
                twJobs.WriteLine("::Jobs[script]");
                twJobs.WriteLine("");
                twJobs.WriteLine(generateJobsScripts(_conf));
            }
            finally
            {
                if (twJobs != null)
                {
                    twJobs.Flush();
                    twJobs.Close();
                }
            }
        }

        private static string generateCharactersScripts(Configuration _conf)
        {
            string characters = "";
            characters +="macros.initCharacters = {\n";
            characters +="\thandler: function(place, macroName, params, parser) {\n";
            characters +="\n";
            characters +="\t\tif (state.active.variables.characters === undefined) {\n";
            characters +="\t\t\tstate.active.variables.characters = [];\n";

            for (int i = 0; i < _conf.characters.Count; i++)
            {
                characters +="\t\t\tstate.active.variables.characters.push({";
                characters +="\"ID\":" + _conf.characters[i].ID + ",";
                characters +="\"name\":\"" + _conf.characters[i].name + "\",";
                characters +="\"category\":\"" + _conf.characters[i].category + "\",";
                characters +="\"description\":\"" + _conf.characters[i].description + "\",";
                characters +="\"age\":" + _conf.characters[i].age + ",";
                characters +="\"gender\":\"" + _conf.characters[i].gender + "\",";
                characters +="\"job\":\"" + _conf.characters[i].job + "\",";
                characters +="\"relation\":" + _conf.characters[i].relation + ",";
                characters +="\"known\":" + _conf.characters[i].known.ToString().ToLower() + ",";
                characters +="\"color\":\"" + _conf.characters[i].color + "\",";
                characters +="\"image\":\"" + pathSubtract(_conf.characters[i].image, _conf.pathSubtract) + "\"";
                if (_conf.characterUseSkill1)
                {
                    string skill1val = (isBool(_conf.characters[i].skill1) || isNumber(_conf.characters[i].skill1)) ? _conf.characters[i].skill1 : "\"" + _conf.characters[i].skill1 + "\"";
                    characters +=",\"" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_SKILL1_CAP")).caption + "\":" + skill1val;
                }
                if (_conf.characterUseSkill2)
                {
                    string skill2val = (isBool(_conf.characters[i].skill2) || isNumber(_conf.characters[i].skill2)) ? _conf.characters[i].skill2 : "\"" + _conf.characters[i].skill2 + "\"";
                    characters +=",\"" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_SKILL2_CAP")).caption + "\":" + skill2val;
                }
                if (_conf.characterUseSkill3)
                {
                    string skill3val = (isBool(_conf.characters[i].skill3) || isNumber(_conf.characters[i].skill3)) ? _conf.characters[i].skill3 : "\"" + _conf.characters[i].skill3 + "\"";
                    characters +=",\"" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_SKILL3_CAP")).caption + "\":" + skill3val;
                }
                characters +="});\n";
            }


            characters +="\t\t}\n";
            characters +="\t}\n";
            characters +="};\n";
            characters +="\n";

            // characters
            characters +="macros.characters = {\n";
            characters +="\thandler: function(place, macroName, params, parser) {\n";
            characters +="\t\tvar wstr = \"<table class=\\\"character\\\">\";\n";
            characters +="\t\twstr +=\"<tr>\";\n";
            if (_conf.displayInCharactersView.Contains("ID")) characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_ID_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInCharactersView.Contains("Name")) characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_NAME_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInCharactersView.Contains("Category")) characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_CATEGORY_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInCharactersView.Contains("Description")) characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_DESCRIPTION_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInCharactersView.Contains("Age")) characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_AGE_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInCharactersView.Contains("Gender")) characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_GENDER_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInCharactersView.Contains("Job")) characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_JOB_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInCharactersView.Contains("Relation")) characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_RELATION_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInCharactersView.Contains("Known")) characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_KNOWN_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInCharactersView.Contains("Color")) characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_COLOR_CAP")).caption + "</b></td>\";\n";
            if (_conf.displayInCharactersView.Contains("Image")) characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_IMAGE_CAP")).caption + "</b></td>\";\n";
            characters +="\n";

            if (_conf.characterUseSkill1 && _conf.displayInCharactersView.Contains("Skill1"))
                characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_SKILL1_CAP")).caption + "</b></td>\";\n";

            if (_conf.characterUseSkill2 && _conf.displayInCharactersView.Contains("Skill2"))
                characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_SKILL2_CAP")).caption + " </b></td>\";\n";

            if (_conf.characterUseSkill3 && _conf.displayInCharactersView.Contains("Skill3"))
                characters +="\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_SKILL3_CAP")).caption + "</b></td>\";\n";
            characters +="\t\twstr +=\"</tr>\";\n";
            characters +="\n";
            characters +="\t\tfor (var w = 0; w<state.active.variables.characters.length; w++) {\n";
            characters +="\t\t\tif (state.active.variables.characters[w].known == false) continue;\n";
            characters +="\t\t\twstr +=\"<tr>\";\n";
            if (_conf.displayInCharactersView.Contains("ID")) characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].ID + \"</td>\";\n";
            if (_conf.displayInCharactersView.Contains("Name")) characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].name + \"</td>\";\n";
            if (_conf.displayInCharactersView.Contains("Category")) characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].category + \"</td>\";\n";
            if (_conf.displayInCharactersView.Contains("Description")) characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].description + \"</td>\";\n";
            if (_conf.displayInCharactersView.Contains("Age")) characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].age + \"</td>\";\n";
            if (_conf.displayInCharactersView.Contains("Gender")) characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].gender + \"</td>\";\n";
            if (_conf.displayInCharactersView.Contains("Job")) characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].job + \"</td>\";\n";
            if (_conf.displayInCharactersView.Contains("Relation")) characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].relation + \"</td>\";\n";
            if (_conf.displayInCharactersView.Contains("Known")) characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].known + \"</td>\";\n";
            if (_conf.displayInCharactersView.Contains("Color")) characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].color + \"</td>\";\n";
            if (_conf.displayInCharactersView.Contains("Image")) characters +="\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"paragraph\\\" src=\\\"\" + state.active.variables.characters[w].image + \"\\\"></td>\";\n";
            characters +="\n";

            if (_conf.characterUseSkill1 && _conf.displayInCharactersView.Contains("Skill1"))
                characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].skill1 + \"</td>\";\n";

            if (_conf.characterUseSkill2 && _conf.displayInCharactersView.Contains("Skill2"))
                characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].skill2 + \"</td>\";\n";

            if (_conf.characterUseSkill3 && _conf.displayInCharactersView.Contains("Skill3"))
                characters +="\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].skill3 + \"</td>\";\n";
            characters +="\n";
            characters +="\t\t\twstr +=\"</tr>\";\n";
            characters +="\t\t}\n";
            characters +="\t\twstr +=\"</table>\";\n";
            characters +="\n";
            characters +="\t\tnew Wikifier(place,wstr);\n";
            characters +="\t}\n";
            characters +="};\n";
            characters +="\n";

            // say
            characters +="macros.say = {\n";
            characters +="\thandler: function(place, macroName, params, parser) {\n";
            characters +="\n";
            characters +="\t\tif (params.length != 2) {\n";
            characters +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two parameters, character ID and text but there were \" + params.length + \" parameters.\");\n";
            characters +="\t\t\treturn;\n";
            characters +="\t\t}\n";
            characters +="\n";
            characters +="\t\tvar character = state.active.variables.characters.filter(c => {return c.ID === params[0]});\n";
            characters +="\t\tif (character.length != 1) {\n";
            characters +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: There must be exactly one character of id \" + params[0] + \" in the character list but there are \" + item_in_catalog.length);\n";
            characters +="\t\t\treturn;\n";
            characters +="\t\t}\n";
            characters +="\n";
            characters +="\t\tvar wstr = \"<table class=\\\"say\\\"><tr><td style='width: 20%;'><img class=\\\"dialog\\\" src=\\\"\" + character[0].image + \"\\\"></td><td style='vertical-align: top; text-align: left;'><font color=\\\"\"+character[0].color+\"\\\">\" + character[0].name + \"</font>: \"+params[1]+\"</td></tr></table>\";\n";
            characters +="\n";
            characters +="\t\tnew Wikifier(place, wstr);\n";
            characters +="\t}\n";
            characters +="};\n";
            characters +="\n";

            // setKnown
            characters +="macros.setKnown = {\n";
            characters +="\thandler: function(place, macroName, params, parser) {\n";
            characters +="\n";
            characters +="\t\tif (params.length != 1) {\n";
            characters +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects one parameter: character ID.\");\n";
            characters +="\t\t\treturn;\n";
            characters +="\t\t}\n";
            characters +="\n";
            characters +="\t\tvar character = state.active.variables.characters.filter(c => {return c.ID === params[0]});\n";
            characters +="\t\tif (character.length != 1) {\n";
            characters +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: There must be exactly one character of id \" + params[0] + \" in the character list but there are \" + item_in_catalog.length);\n";
            characters +="\t\t\treturn;\n";
            characters +="\t\t}\n";
            characters +="\n";
            characters +="\t\tcharacter[0].known = true;\n";
            characters +="\t}\n";
            characters +="};\n";
            characters +="\n";

            // rename
            characters +="macros.renameCharacter = {\n";
            characters +="\thandler: function(place, macroName, params, parser) {\n";
            characters +="\n";
            characters +="\t\tif (params.length != 2) {\n";
            characters +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two parameters: character ID and new name.\");\n";
            characters +="\t\t\treturn;\n";
            characters +="\t\t}\n";
            characters +="\n";
            characters +="\t\tvar character = state.active.variables.characters.filter(c => {return c.ID === params[0]});\n";
            characters +="\t\tif (character.length != 1) {\n";
            characters +="\t\t\tthrowError(place, \"<<\" + macroName + \">>: There must be exactly one character of id \" + params[0] + \" in the character list but there are \" + item_in_catalog.length);\n";
            characters +="\t\t\treturn;\n";
            characters +="\t\t}\n";
            characters +="\n";
            characters +="\t\tcharacter[0].name = params[1];\n";
            characters +="\t}\n";
            characters +="};\n";
            characters +="\n";

            // characterSidebar
            characters +="macros.charactersSidebar = {\n";
            characters +="\thandler: function(place, macroName, params, parser) {\n";
            characters +="\n";
            characters +="\t\tvar wstr = \"<table class=\\\"character\\\">\";	\n";
            characters +="\t\twstr +=\"<tr><td colspan=2>Characters</td></tr>\";	\n";

            // get known characters
            characters +="\t\tvar knownCharacters = state.active.variables.characters.filter(c => {return c.known == true});\n";

            characters +="\t\tfor (var w = 0; w<knownCharacters.length; w +=2) {\n";
            characters +="\t\t\twstr +=\"<tr>\";\n";
            characters +="\n";
            characters +="\t\t\tvar char_info_1 = \"\";\n";
            if (_conf.displayInCharactersView.Contains("ID")) characters +="\t\t\tchar_info_1 += \"ID: \" + knownCharacters[w].ID + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Name")) characters +="\t\t\tchar_info_1 += \"name:\" + knownCharacters[w].name + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Category")) characters +="\t\t\tchar_info_1 += \"category:\" + knownCharacters[w].category + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Description")) characters +="\t\t\tchar_info_1 += \"description:\" + knownCharacters[w].description + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Age")) characters +="\t\t\tchar_info_1 += \"age:\" + knownCharacters[w].age + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Gender")) characters +="\t\t\tchar_info_1 += \"gender:\" + knownCharacters[w].gender + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Job")) characters +="\t\t\tchar_info_1 += \"job:\" + knownCharacters[w].job + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Relation")) characters +="\t\t\tchar_info_1 += \"relation:\" + knownCharacters[w].relation + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Known")) characters +="\t\t\tchar_info_1 += \"known:\" + knownCharacters[w].known + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Color")) characters +="\t\t\tchar_info_1 += \"color:\" + knownCharacters[w].color + \"&#10;\";\n";
            if (_conf.characterUseSkill1 && _conf.displayInCharactersView.Contains("Skill1"))
                characters +="\t\t\tchar_info_1 +=\"skill1: \" + knownCharacters[w].skill1 + \"&#10;\";\n";
            if (_conf.characterUseSkill2 && _conf.displayInCharactersView.Contains("Skill2"))
                characters +="\t\t\tchar_info_1 +=\"snill2: \" + knownCharacters[w].skill2 + \"&#10;\";\n";
            if (_conf.characterUseSkill3 && _conf.displayInCharactersView.Contains("Skill3"))
                characters +="\t\t\tchar_info_1 +=\"skill3: \" + sknownCharacters[w].skill3 + \"&#10;\";\n";
            characters +="\n";

            if (_conf.charactersSidebarTooltip)
            {
                characters +="\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + knownCharacters[w].image + \"\\\" title=\\\"\" + char_info_1 + \"\\\"></td>\";\n";
            }
            else
            {
                characters +="\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + knownCharacters[w].image + \"\\\"></td>\";\n";
            }
            characters +="\t\t\tif (w+1 < knownCharacters.length) {\n";
            characters +="\n";
            characters +="\t\t\t\tvar char_info_2 = \"\";\n";
            if (_conf.displayInCharactersView.Contains("ID")) characters +="\t\t\t\tchar_info_2 += \"ID: \" + knownCharacters[w+1].ID + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Name")) characters +="\t\t\t\tchar_info_2 += \"name:\" + knownCharacters[w+1].name + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Category")) characters +="\t\t\t\tchar_info_2 += \"category:\" + knownCharacters[w+1].category + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Description")) characters +="\t\t\t\tchar_info_2 += \"description:\" + knownCharacters[w+1].description + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Age")) characters +="\t\t\t\tchar_info_2 += \"age:\" + knownCharacters[w+1].age + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Gender")) characters +="\t\t\t\tchar_info_2 += \"gender:\" + knownCharacters[w+1].gender + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Job")) characters +="\t\t\t\tchar_info_2 += \"job:\" + knownCharacters[w+1].job + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Relation")) characters +="\t\t\t\tchar_info_2 += \"relation:\" + knownCharacters[w+1].relation + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Known")) characters +="\t\t\t\tchar_info_2 += \"known:\" + knownCharacters[w+1].known + \"&#10;\";\n";
            if (_conf.displayInCharactersView.Contains("Color")) characters +="\t\t\t\tchar_info_2 += \"color:\" + knownCharacters[w+1].color + \"&#10;\";\n";

            if (_conf.characterUseSkill1 && _conf.displayInCharactersView.Contains("Skill1"))
                characters +="\t\t\t\tchar_info_2 +=\"skill1: \" + knownCharacters[w+1].skill1 + \"&#10;\";\n";

            if (_conf.characterUseSkill2 && _conf.displayInCharactersView.Contains("Skill2"))
                characters +="\t\t\t\tchar_info_2 +=\"snill2: \" + knownCharacters[w+1].skill2 + \"&#10;\";\n";

            if (_conf.characterUseSkill3 && _conf.displayInCharactersView.Contains("Skill3"))
                characters +="\t\t\t\tchar_info_2 +=\"skill3: \" + knownCharacters[w+1].skill3 + \"&#10;\";\n";
            characters +="\n";

            if (_conf.charactersSidebarTooltip)
            {
                characters +="\t\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + knownCharacters[w+1].image + \"\\\" title=\\\"\" + char_info_2 + \"\\\"></td>\";\n";
            }
            else
            {
                characters +="\t\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.characters[w+1].image + \"\\\"></td>\";\n";
            }
            characters +="\t\t\t} else {\n";
            characters +="\t\t\t\twstr +=\"<td></td>\";\n";
            characters +="\t\t\t}\n";
            characters +="\t\t\twstr +=\"</tr>\";\n";
            characters +="\t\t}\n";
            characters +="\t\twstr +=\"</table>\";\n";
            characters +="\n";
            characters +="\t\tnew Wikifier(place,wstr);\n";
            characters +="\t}\n";
            characters +="};\n";
            return characters;
        }

        private static void generateCharacters(Configuration _conf, string _path)
        {
            string characterPath = Path.Combine(_path, "_js_characters.tw2");
            TextWriter twCharacters = null;
            try
            {
                twCharacters = new StreamWriter(characterPath, false, new UTF8Encoding(false));
                twCharacters.WriteLine("::Characters[script]");
                twCharacters.WriteLine("");
                twCharacters.WriteLine(generateCharactersScripts(_conf));
            }
            finally
            {
                if (twCharacters != null)
                {
                    twCharacters.Flush();
                    twCharacters.Close();
                }
            }
        }

        private static void generateBat(Configuration _conf, string _path)
        {
            string batPath = Path.Combine(_path, "build.bat");
            TextWriter twBat = null;
            try
            {
                twBat = new StreamWriter(batPath, false, new UTF8Encoding(false));

                string storyFile = string.IsNullOrEmpty(_conf.storyName) ? "story.tw2" : _conf.storyName;
                storyFile = storyFile.EndsWith(".tw2") ? storyFile : storyFile + ".tw2";
                string htmlFile = storyFile.Remove(storyFile.Length - 4, 4) + ".html";

                twBat.WriteLine("twee2 build \"" + storyFile + "\" \"" + htmlFile + "\"");
            }
            finally
            {
                if (twBat != null)
                {
                    twBat.Flush();
                    twBat.Close();
                }
            }
        }

        private static string generateCssContent(Configuration _conf)
        {
            string css = "";
            css += "table.clothing {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%; }\n";
            css += "td.clothing {overflow: hidden; text-align: center; vertical-align: middle;}\n";
            css += "th.clothing {}\n";
            css += "\n";
            css += "table.clothing_sidebar {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}\n";
            css += "td.clothing_sidebar {overflow: hidden; text-align: center; vertical-align: middle;}\n";
            css += "th.clothing_sidebar {}\n";
            css += "\n";
            css += "table.inventory {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}\n";
            css += "td.inventory {overflow: hidden; text-align: center; vertical-align: middle;}\n";
            css += "th.inventory {}\n";
            css += "\n";
            css += "table.inventory_sidebar {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}\n";
            css += "td.inventory_sidebar {overflow: hidden; text-align: center; vertical-align: middle;}\n";
            css += "th.inventory_sidebar {}\n";
            css += "\n";
            css += "table.wardrobe {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}\n";
            css += "td.wardrobe {overflow: hidden; text-align: center; vertical-align: middle;}\n";
            css += "th.wardrobe {}\n";
            css += "\n";
            css += "table.stats {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}\n";
            css += "td.stats {overflow: hidden; text-align: center; vertical-align: middle;}\n";
            css += "th.stats {}\n";
            css += "\n";
            css += "table.stats_sidebar {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}\n";
            css += "td.stats_sidebar {overflow: hidden; text-align: center; vertical-align: middle;}\n";
            css += "th.stats_sidebar {}\n";
            css += "\n";
            css += "table.shop {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}\n";
            css += "td.shop {overflow: hidden; text-align: center; vertical-align: middle;}\n";
            css += "th.shop {}\n";
            css += "\n";
            css += "table.jobs {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}\n";
            css += "td.jobs {overflow: hidden; text-align: center; vertical-align: middle;}\n";
            css += "th.jobs {}\n";
            css += "img.jobs {width:100%; max-width:100px;}\n";
            css += "\n";
            css += "table.character {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}\n";
            css += "td.character {overflow: hidden;text-align: center; vertical-align: middle;}\n";
            css += "th.character {}\n";
            css += "\n";
            css += "table.character_sidebar {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}\n";
            css += "td.character_sidebar {overflow: hidden;text-align: center; vertical-align: middle;}\n";
            css += "th.character_sidebar {}\n";
            css += "\n";
            css += "table.say {border: 1px solid white; table-layout:fixed; width: 100%;}\n";
            css += "td.say {overflow: hidden;text-align: center; vertical-align: middle;}\n";
            css += "th.say {}\n";
            css += "\n";
            css += "#passages {max-width: " + _conf.paragraphWidth + "px;}\n";

            if (_conf.resizeImagesInSidebar)
            {
                css += "img.sidebar {height: auto; width: auto; max-width: " + _conf.imageWidthInSidebar + "px; max-height: " + _conf.imageHeightInSidebar + "px; }\n";
            }
            else
            {
                css += "img.sidebar {}\n";
            }

            if (_conf.resizeImagesInParagraph)
            {
                css += "img.paragraph {height: auto; width: auto; max-width: " + _conf.imageWidthInParagraph + "px; max-height: " + _conf.imageHeightInParagraph + "px; }\n";
            }
            else
            {
                css += "img.paragraph {}\n";
            }

            if (_conf.resizeImagesInDialogs)
            {
                css += "img.dialog {height: auto; width: auto; max-width: " + _conf.imageWidthInDialogs + "px; max-height: " + _conf.imageHeightInDialogs + "px; }\n";
            }
            else
            {
                css += "img.dialog {}\n";
            }
            return css;
        }

        private static void generateCss(Configuration _conf, string _path)
        {
            string cssPath = Path.Combine(_path, "_css_style.tw2");
            TextWriter twCss = null;
            try
            {
                twCss = new StreamWriter(cssPath, false, new UTF8Encoding(false));
                twCss.WriteLine("::CSSStyle[scss stylesheet]");
                twCss.WriteLine("");
                twCss.WriteLine(generateCssContent(_conf));
            }
            finally
            {
                if (twCss != null)
                {
                    twCss.Flush();
                    twCss.Close();
                }
            }
        }


        public static string generateTwee2(Configuration _conf)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "|*.tw2|*.*|";
            openFileDialog1.Title = "Open Twee2 File";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                string path = Path.GetDirectoryName(openFileDialog1.FileName);
                generateMain(_conf, path, openFileDialog1.FileName);
                generateMenu(_conf, path);
                generateNavigation(_conf, path);
                generateCss(_conf, path);
                if (_conf.inventoryActive) generateInventory(_conf, path);
                if (_conf.clothingActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) generateClothing(_conf, path);
                if (_conf.statsActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) generateStats(_conf, path);
                if (_conf.daytimeActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) generateDaytime(_conf, path);
                if (_conf.shopActive) generateShops(_conf, path);
                if (_conf.moneyActive) generateMoney(_conf, path);
                if (_conf.jobsActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) generateJobs(_conf, path);
                if (_conf.charactersActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) generateCharacters(_conf, path);
                generateBat(_conf, path);
                return openFileDialog1.FileName;
            }

            return "";
        }

        private static HtmlNode GetTwStoryData(HtmlAgilityPack.HtmlDocument doc)
        {
            List<HtmlNode> storyNodes = doc.DocumentNode.Descendants("tw-storydata").ToList();
            if (storyNodes.Count == 1)
            {
                return storyNodes[0];
            } else
            {
                throw new Exception("Could not find 'tw-storydata' node in twine file.");
            }
        }

        private static int getNextFreeNode(HtmlAgilityPack.HtmlDocument doc)
        {
            for (int i = 0; i < 1000000; i++)
            {
                HtmlNode passageNode = GetTwStoryData(doc).SelectSingleNode("tw-passagedata[@pid='" + i + "']");
                if (passageNode == null)
                {
                    return i;
                }
            }
            return -1;
        }

        private static HtmlAgilityPack.HtmlNode getParagraph(HtmlAgilityPack.HtmlDocument doc, string name)
        {
            return GetTwStoryData(doc).SelectSingleNode("tw-passagedata[@name='" + name + "']");
        }

        public static string generateTwine(Configuration _conf)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "|*.html||*.*";
            openFileDialog1.Title = "Open Twine File";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(openFileDialog1.FileName);

                HtmlNode root = GetTwStoryData(doc);

                if (root == null)
                {
                    MessageBox.Show("Twine story is invalid. Must be version 2.3.2.");
                    return null;
                }

                // Check if SugarCube2
                string format = GetTwStoryData(doc).Attributes["format"].Value;
                if (string.IsNullOrEmpty(format) || !format.Equals("SugarCube"))
                {
                    MessageBox.Show("Story format must be 'SugarCube'.");
                    return null;
                }

                // Create start node
                {
                    int freeNode = getNextFreeNode(doc);
                    if (freeNode == -1)
                    {
                        MessageBox.Show("There is no free passage to add the start node.");
                        return null;
                    }

                    string startNode = "";
                    if (_conf.daytimeActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) startNode += "<<initDaytime>>";
                    if (_conf.clothingActive && TweeFlyPro.Properties.Settings.Default.IsProEdition)
                    {
                        startNode += "<<initAllClothing>>";
                        startNode += "<<initClothing>>";
                        startNode += "<<initWardrobe>>";
                    }
                    if (_conf.inventoryActive)
                    {
                        startNode += "<<initItems>>";
                        startNode += "<<initInventory>>";
                    }
                    if (_conf.shopActive) startNode += "<<initShops>>";
                    if (_conf.statsActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) startNode += "<<initStats>>";
                    if (_conf.moneyActive) startNode += "<<initMoney>>";
                    if (_conf.jobsActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) startNode += "<<initJobs>>";
                    if (_conf.charactersActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) startNode += "<<initCharacters>>";
                    startNode = System.Security.SecurityElement.Escape(startNode);

                    HtmlNode storyInitNode = getParagraph(doc, "StoryInit");
                    if (storyInitNode != null)
                    {
                        string innerHtml = storyInitNode.InnerHtml;
                        if (innerHtml.Contains(Generator.TWEEFLY_COMMENT_START_ESC))
                        {
                            int start = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_START_ESC);
                            int end = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_END_ESC);
                            string oldCode = innerHtml.Substring(start, end - start + Generator.TWEEFLY_COMMENT_END_ESC.Length);
                            storyInitNode.InnerHtml = innerHtml.Replace(oldCode, TWEEFLY_COMMENT_START_ESC + startNode + TWEEFLY_COMMENT_END_ESC);
                        }
                        else
                        {
                            innerHtml += TWEEFLY_COMMENT_START_ESC + startNode + TWEEFLY_COMMENT_END_ESC;
                            storyInitNode.InnerHtml = innerHtml;
                        }
                    }
                    else
                    {
                        root.AppendChild(HtmlNode.CreateNode("<tw-passagedata pid=\"" + freeNode + "\" name=\"StoryInit\" tags=\"\" position=\"10,10\" size=\"100, 100\">" +
                            TWEEFLY_COMMENT_START_ESC + startNode + TWEEFLY_COMMENT_END_ESC + "</tw-passagedata>"));
                    }
                }

                // Create menu /story caption
                {
                    int freeNode = getNextFreeNode(doc);
                    if (freeNode == -1)
                    {
                        MessageBox.Show("There is no free passage to add the menu node.");
                        return null;
                    }

                    string storyCaptionNodeString = System.Security.SecurityElement.Escape(generateMenuString(_conf));

                    HtmlNode storyCaptionNode = getParagraph(doc, "StoryCaption");
                    if (storyCaptionNode != null)
                    {
                        string innerHtml = storyCaptionNode.InnerHtml;
                        if (innerHtml.Contains(Generator.TWEEFLY_COMMENT_START_ESC))
                        {
                            int start = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_START_ESC);
                            int end = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_END_ESC);
                            string oldCode = innerHtml.Substring(start, end - start + Generator.TWEEFLY_COMMENT_END_ESC.Length);
                            storyCaptionNode.InnerHtml = innerHtml.Replace(oldCode, TWEEFLY_COMMENT_START_ESC + storyCaptionNodeString + TWEEFLY_COMMENT_END_ESC);
                        }
                        else
                        {
                            innerHtml += TWEEFLY_COMMENT_START_ESC + storyCaptionNodeString + TWEEFLY_COMMENT_END_ESC;
                            storyCaptionNode.InnerHtml = innerHtml;
                        }
                    }
                    else
                    {
                        root.AppendChild(HtmlNode.CreateNode("<tw-passagedata pid=\"" + freeNode + "\" name=\"StoryCaption\" tags=\"\" position=\"120,10\" size=\"100, 100\">" +
                           TWEEFLY_COMMENT_START_ESC + storyCaptionNodeString + TWEEFLY_COMMENT_END_ESC + "</tw-passagedata>"));
                    }
                }

                // Create inventory passage
                {
                    int freeNode = getNextFreeNode(doc);
                    if (freeNode == -1)
                    {
                        MessageBox.Show("There is no free passage to add the menu node.");
                        return null;
                    }

                    string inventoryNodeString = System.Security.SecurityElement.Escape(generateInventoryPassageString(_conf));

                    HtmlNode inventoryNode = getParagraph(doc, "InventoryMenu");
                    if (inventoryNode != null)
                    {
                        if ((inventoryNode.Attributes["tags"] == null) || (!inventoryNode.Attributes["tags"].Value.Equals("noreturn"))) {
                            DialogResult dialogResult = MessageBox.Show("InventoryMenu paragraph requires attribute 'tags' with value 'noreturn'. Do you want to add it? Otherwise the process will fail.",
                                "noreturn attribute", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                inventoryNode.SetAttributeValue("tags", "noreturn");
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                return null;
                            }
                        }

                        string innerHtml = inventoryNode.InnerHtml;
                        if (innerHtml.Contains(Generator.TWEEFLY_COMMENT_START_ESC))
                        {
                            int start = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_START_ESC);
                            int end = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_END_ESC);
                            string oldCode = innerHtml.Substring(start, end - start + Generator.TWEEFLY_COMMENT_END_ESC.Length);
                            inventoryNode.InnerHtml = innerHtml.Replace(oldCode, TWEEFLY_COMMENT_START_ESC + inventoryNodeString + TWEEFLY_COMMENT_END_ESC);
                        }
                        else
                        {
                            innerHtml += TWEEFLY_COMMENT_START_ESC + inventoryNodeString + TWEEFLY_COMMENT_END_ESC;
                            inventoryNode.InnerHtml = innerHtml;
                        }
                    }
                    else
                    {
                        root.AppendChild(HtmlNode.CreateNode("<tw-passagedata pid=\"" + freeNode + "\" name=\"InventoryMenu\" tags=\"noreturn\" position=\"240,10\" size=\"100, 100\">" +
                            TWEEFLY_COMMENT_START_ESC + inventoryNodeString + TWEEFLY_COMMENT_END_ESC + "</tw-passagedata>"));
                    }
                }

                // Create cloth passage
                {
                    int freeNode = getNextFreeNode(doc);
                    if (freeNode == -1)
                    {
                        MessageBox.Show("There is no free passage to add the menu node.");
                        return null;
                    }

                    string clothNodeString = System.Security.SecurityElement.Escape(generateClothPassageString(_conf));

                    HtmlNode clothNode = getParagraph(doc, "ClothingMenu");
                    if (clothNode != null)
                    {
                        if ((clothNode.Attributes["tags"] == null) || (!clothNode.Attributes["tags"].Value.Equals("noreturn"))) {
                            DialogResult dialogResult = MessageBox.Show("ClothingMenu paragraph requires attribute 'tags' with value 'noreturn'. Do you want to add it? Otherwise the process will fail.",
                                "noreturn attribute", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                clothNode.SetAttributeValue("tags", "noreturn");
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                return null;
                            }
                        }

                        string innerHtml = clothNode.InnerHtml;
                        if (innerHtml.Contains(Generator.TWEEFLY_COMMENT_START_ESC))
                        {
                            int start = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_START_ESC);
                            int end = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_END_ESC);
                            string oldCode = innerHtml.Substring(start, end - start + Generator.TWEEFLY_COMMENT_END_ESC.Length);
                            clothNode.InnerHtml = innerHtml.Replace(oldCode, TWEEFLY_COMMENT_START_ESC + clothNodeString + TWEEFLY_COMMENT_END_ESC);
                        }
                        else
                        {
                            innerHtml += TWEEFLY_COMMENT_START_ESC + clothNodeString + TWEEFLY_COMMENT_END_ESC;
                            clothNode.InnerHtml = innerHtml;
                        }
                    }
                    else
                    {
                        root.AppendChild(HtmlNode.CreateNode("<tw-passagedata pid=\"" + freeNode + "\" name=\"ClothingMenu\" tags=\"noreturn\" position=\"360,10\" size=\"100, 100\">" +
                           TWEEFLY_COMMENT_START_ESC + clothNodeString + TWEEFLY_COMMENT_END_ESC + "</tw-passagedata>"));
                    }
                }

                // Create wardrobe passage
                {
                    int freeNode = getNextFreeNode(doc);
                    if (freeNode == -1)
                    {
                        MessageBox.Show("There is no free passage to add the menu node.");
                        return null;
                    }

                    string wardrobeNodeString = System.Security.SecurityElement.Escape(generateWardrobePassageString(_conf));

                    HtmlNode wardrobeNode = getParagraph(doc, "WardrobeMenu");
                    if (wardrobeNode != null)
                    {
                        if ((wardrobeNode.Attributes["tags"] == null) || (!wardrobeNode.Attributes["tags"].Value.Equals("noreturn"))) {
                            DialogResult dialogResult = MessageBox.Show("WardrobeMenu paragraph requires attribute 'tags' with value 'noreturn'. Do you want to add it? Otherwise the process will fail.",
                                "noreturn attribute", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                wardrobeNode.SetAttributeValue("tags", "noreturn");
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                return null;
                            }
                        }

                        string innerHtml = wardrobeNode.InnerHtml;
                        if (innerHtml.Contains(Generator.TWEEFLY_COMMENT_START_ESC))
                        {
                            int start = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_START_ESC);
                            int end = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_END_ESC);
                            string oldCode = innerHtml.Substring(start, end - start + Generator.TWEEFLY_COMMENT_END_ESC.Length);
                            wardrobeNode.InnerHtml = innerHtml.Replace(oldCode, TWEEFLY_COMMENT_START_ESC + wardrobeNodeString + TWEEFLY_COMMENT_END_ESC);
                        }
                        else
                        {
                            innerHtml += TWEEFLY_COMMENT_START_ESC + wardrobeNodeString + TWEEFLY_COMMENT_END_ESC;
                            wardrobeNode.InnerHtml = innerHtml;
                        }
                    }
                    else
                    {
                        root.AppendChild(HtmlNode.CreateNode("<tw-passagedata pid=\"" + freeNode + "\" name=\"WardrobeMenu\" tags=\"noreturn\" position=\"480,10\" size=\"100, 100\">" +
                           TWEEFLY_COMMENT_START_ESC + wardrobeNodeString + TWEEFLY_COMMENT_END_ESC + "</tw-passagedata>"));
                    }
                }

                // Create stats passage
                {
                    int freeNode = getNextFreeNode(doc);
                    if (freeNode == -1)
                    {
                        MessageBox.Show("There is no free passage to add the menu node.");
                        return null;
                    }

                    string statsNodeString = System.Security.SecurityElement.Escape(generateStatsPassageString(_conf));

                    HtmlNode statsNode = getParagraph(doc, "StatsMenu");
                    if (statsNode != null)
                    {
                        if ((statsNode.Attributes["tags"] == null) || (!statsNode.Attributes["tags"].Value.Equals("noreturn"))) {
                            DialogResult dialogResult = MessageBox.Show("StatsMenu paragraph requires attribute 'tags' with value 'noreturn'. Do you want to add it? Otherwise the process will fail.",
                                "noreturn attribute", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                statsNode.SetAttributeValue("tags", "noreturn");
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                return null;
                            }
                        }

                        string innerHtml = statsNode.InnerHtml;
                        if (innerHtml.Contains(Generator.TWEEFLY_COMMENT_START_ESC))
                        {
                            int start = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_START_ESC);
                            int end = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_END_ESC);
                            string oldCode = innerHtml.Substring(start, end - start + Generator.TWEEFLY_COMMENT_END_ESC.Length);
                            statsNode.InnerHtml = innerHtml.Replace(oldCode, TWEEFLY_COMMENT_START_ESC + statsNodeString + TWEEFLY_COMMENT_END_ESC);
                        }
                        else
                        {
                            innerHtml += TWEEFLY_COMMENT_START_ESC + statsNodeString + TWEEFLY_COMMENT_END_ESC;
                            statsNode.InnerHtml = innerHtml;
                        }
                    }
                    else
                    {
                        root.AppendChild(HtmlNode.CreateNode("<tw-passagedata pid=\"" + freeNode + "\" name=\"StatsMenu\" tags=\"noreturn\" position=\"600,10\" size=\"100, 100\">" +
                            TWEEFLY_COMMENT_START_ESC + statsNodeString + TWEEFLY_COMMENT_END_ESC + "</tw-passagedata>"));
                    }
                }

                // Create characters passage
                {
                    int freeNode = getNextFreeNode(doc);
                    if (freeNode == -1)
                    {
                        MessageBox.Show("There is no free passage to add the menu node.");
                        return null;
                    }

                    string charactersNodeString = System.Security.SecurityElement.Escape(generateCharactersPassageString(_conf));

                    HtmlNode charactersNode = getParagraph(doc, "CharactersMenu");
                    if (charactersNode != null)
                    {
                        if ((charactersNode.Attributes["tags"] == null) || (!charactersNode.Attributes["tags"].Value.Equals("noreturn"))) {
                            DialogResult dialogResult = MessageBox.Show("CharactersMenu paragraph requires attribute 'tags' with value 'noreturn'. Do you want to add it? Otherwise the process will fail.",
                                "noreturn attribute", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                charactersNode.SetAttributeValue("tags", "noreturn");
                                
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                return null;
                            }                           
                        }

                        string innerHtml = charactersNode.InnerHtml;
                        if (innerHtml.Contains(Generator.TWEEFLY_COMMENT_START_ESC))
                        {
                            int start = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_START_ESC);
                            int end = innerHtml.IndexOf(Generator.TWEEFLY_COMMENT_END_ESC);
                            string oldCode = innerHtml.Substring(start, end - start + Generator.TWEEFLY_COMMENT_END_ESC.Length);
                            charactersNode.InnerHtml = innerHtml.Replace(oldCode, TWEEFLY_COMMENT_START_ESC + charactersNodeString + TWEEFLY_COMMENT_END_ESC);
                        }
                        else
                        {
                            innerHtml += TWEEFLY_COMMENT_START_ESC + charactersNodeString + TWEEFLY_COMMENT_END_ESC;
                            charactersNode.InnerHtml = innerHtml;
                        }
                    }
                    else
                    {
                        root.AppendChild(HtmlNode.CreateNode("<tw-passagedata pid=\"" + freeNode + "\" name=\"CharactersMenu\" tags=\"noreturn\" position=\"720,10\" size=\"100, 100\">" +
                            TWEEFLY_COMMENT_START_ESC + charactersNodeString + TWEEFLY_COMMENT_END_ESC + "</tw-passagedata>"));
                    }
                }


                // Create CSS
                HtmlNode cssNode = GetTwStoryData(doc).SelectSingleNode("style[@role='stylesheet' and @id='twine-user-stylesheet']");
                string cssCode = generateCssContent(_conf);
                if (cssNode != null)
                {
                    string innerHtml = cssNode.InnerHtml;
                    if (innerHtml.Contains(Generator.TWEEFLY_CSS_COMMENT_START))
                    {
                        int start = innerHtml.IndexOf(Generator.TWEEFLY_CSS_COMMENT_START);
                        int end = innerHtml.IndexOf(Generator.TWEEFLY_CSS_COMMENT_END);
                        string oldCode = innerHtml.Substring(start, end - start + Generator.TWEEFLY_CSS_COMMENT_END.Length);
                        cssNode.InnerHtml = innerHtml.Replace(oldCode, TWEEFLY_CSS_COMMENT_START + cssCode + TWEEFLY_CSS_COMMENT_END);
                    }
                    else
                    {
                        innerHtml += TWEEFLY_CSS_COMMENT_START + cssCode + TWEEFLY_CSS_COMMENT_END;
                        cssNode.InnerHtml = innerHtml;
                    }
                } else
                {
                    root.AppendChild(HtmlNode.CreateNode("<style role=\"stylesheet\" id=\"twine-user-stylesheet\" type=\"text/twine-css\">" + TWEEFLY_CSS_COMMENT_START + cssCode + TWEEFLY_CSS_COMMENT_END + "</style>"));
                }



                // Create JavaScript
                HtmlNode scriptNode = GetTwStoryData(doc).SelectSingleNode("script[@role='script' and @id='twine-user-script']");
                string newInnerText = "";
                newInnerText += generateNavigationScripts(_conf); // Navigation
                if (_conf.inventoryActive) newInnerText += generateInventoryScripts(_conf); // Inventory
                if (_conf.clothingActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) newInnerText += generateClothingScripts(_conf); // Clothing
                if (_conf.daytimeActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) newInnerText += generateDaytimeScripts(_conf); // Daytime
                if (_conf.statsActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) newInnerText += generateStatsScripts(_conf); // Stats
                if (_conf.shopActive) newInnerText += generateShopsScripts(_conf); // Shops
                if (_conf.moneyActive) newInnerText += generateMoneyScripts(_conf); // Money
                if (_conf.jobsActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) newInnerText += generateJobsScripts(_conf); // Jobs
                if (_conf.charactersActive && TweeFlyPro.Properties.Settings.Default.IsProEdition) newInnerText += generateCharactersScripts(_conf); // Characters

                if (scriptNode != null)
                {
                    string innerText = scriptNode.InnerText;
                    if (innerText.Contains(Generator.TWEEFLY_CSS_COMMENT_START))
                    {
                        int start = innerText.IndexOf(Generator.TWEEFLY_CSS_COMMENT_START);
                        int end = innerText.IndexOf(Generator.TWEEFLY_CSS_COMMENT_END);
                        string oldCode = innerText.Substring(start, end - start + Generator.TWEEFLY_CSS_COMMENT_END.Length);
                        innerText = innerText.Replace(oldCode, TWEEFLY_CSS_COMMENT_START + newInnerText + TWEEFLY_CSS_COMMENT_END);
                    } else
                    {
                        innerText += TWEEFLY_CSS_COMMENT_START + newInnerText + TWEEFLY_CSS_COMMENT_END;
                    }

                    scriptNode.ParentNode.RemoveChild(scriptNode);
                    root.AppendChild(HtmlTextNode.CreateNode("<script role=\"script\" id=\"twine-user-script\" type=\"text/twine-javascript\">" + innerText + "</script>"));
                }
                else
                {
                    root.AppendChild(HtmlNode.CreateNode("<style role=\"script\" id=\"twine-user-stylesheet\" type=\"text/twine-javascript\">" + TWEEFLY_CSS_COMMENT_START + cssCode + TWEEFLY_CSS_COMMENT_END + "</style>"));
                }

                doc.Save(openFileDialog1.FileName);
                return openFileDialog1.FileName;
            }
            return null;
        }

        private static string pathSubtract(string _s, string _subtract)
        {
            int index = _s.IndexOf(_subtract);
            return (index < 0) ? _s : _s.Remove(index, _subtract.Length);
        }
    }
}
