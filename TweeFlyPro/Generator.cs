using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TweeFly
{
    class Generator
    {
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

        private static void generateMain(Configuration _conf, string _path)
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
                if (_conf.clothActive) twMain.WriteLine("_js_cloth.tw2");
                if (_conf.statsActive) twMain.WriteLine("_js_stats.tw2");
                if (_conf.moneyActive) twMain.WriteLine("_js_money.tw2");
                twMain.WriteLine("_js_navigation.tw2");
                if (_conf.daytimeActive) twMain.WriteLine("_js_daytime.tw2");
                if (_conf.shopActive) twMain.WriteLine("_js_shops.tw2");
                if (_conf.jobsActive) twMain.WriteLine("_js_jobs.tw2");
                if (_conf.charactersActive) twMain.WriteLine("_js_characters.tw2");
                twMain.WriteLine("_tw_sidebar.tw2");
                twMain.WriteLine("_css_style.tw2");
                if (!string.IsNullOrEmpty(_conf.mainFile))
                {
                    string storyFileAbsolute = Path.IsPathRooted(_conf.mainFile) ? _conf.mainFile : Path.Combine(_path, _conf.mainFile);

                    if (!File.Exists(storyFileAbsolute))
                    {
                        if (Path.GetExtension(storyFileAbsolute).Equals(""))
                        {
                            storyFileAbsolute += ".tw2";
                        }
                        MessageBox.Show("Story file '" + storyFileAbsolute + "' does not exist. Creating dummy story as '" + storyFileAbsolute + "'.");
                        generateEmptyStory(storyFileAbsolute);
                    }
                    twMain.WriteLine(Path.GetFileName(storyFileAbsolute));
                }
                twMain.WriteLine("");

                // Story init
                twMain.WriteLine("::StoryInit");
                if (_conf.daytimeActive) twMain.WriteLine("<<initDaytime>>");
                if (_conf.clothActive)
                {
                    twMain.WriteLine("<<initAllCloth>>");
                    twMain.WriteLine("<<initCloth>>");
                    twMain.WriteLine("<<initWardrobe>>");
                }
                if (_conf.inventoryActive)
                {
                    twMain.WriteLine("<<initItems>>");
                    twMain.WriteLine("<<initInventory>>");
                }
                if (_conf.shopActive) twMain.WriteLine("<<initShops>>");
                if (_conf.statsActive) twMain.WriteLine("<<initStats>>");
                if (_conf.moneyActive) twMain.WriteLine("<<initMoney>>");
                if (_conf.jobsActive) twMain.WriteLine("<<initJobs>>");
                if (_conf.charactersActive) twMain.WriteLine("<<initCharacters>>");
                twMain.WriteLine("");

                // Start
                twMain.WriteLine("::Start");
                twMain.WriteLine("Start writing your story here...");
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

        private static void generateEmptyStory(string _path)
        {
            TextWriter twStory = null;
            try
            {
                twStory = new StreamWriter(_path, false, new UTF8Encoding(false));
                twStory.WriteLine("::Start");
                twStory.WriteLine("Start writing your story here...");
            }
            finally
            {
                if (twStory != null)
                {
                    twStory.Flush();
                    twStory.Close();
                }
            }
        }

        private static void generateMenu(Configuration _conf, string _path)
        {
            string menuPath = Path.Combine(_path, "_tw_sidebar.tw2");
            TextWriter twMenu = null;
            try
            {
                twMenu = new StreamWriter(menuPath, false, new UTF8Encoding(false));
                twMenu.WriteLine("::StoryCaption");

                // Links
                if (_conf.inventoryActive && _conf.inventoryLinkInSidebar)
                    twMenu.WriteLine("[[" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_LINK_CAP")).caption + "->InventoryMenu]]");
                
                if (_conf.clothActive && _conf.wardrobeLinkInSidebar)
                    twMenu.WriteLine("[[" + _conf.captions.Single(s => s.captionName.Equals("WARDROBE_LINK_CAP")).caption + "->WardrobeMenu]]");

                if (_conf.clothActive && _conf.clothLinkInSidebar)
                    twMenu.WriteLine("[[" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_LINK_CAP")).caption + "->ClothMenu]]");

                if (_conf.statsActive && _conf.statsLinkInSidebar)
                    twMenu.WriteLine("[[" + _conf.captions.Single(s => s.captionName.Equals("STATS_LINK_CAP")).caption + "->StatsMenu]]");

                if (_conf.charactersActive && _conf.charactersLinkInSidebar)
                    twMenu.WriteLine("[[" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_LINK_CAP")).caption + "->CharactersMenu]]");
                twMenu.WriteLine("");

                // Variables
                if (_conf.moneyActive && _conf.moneyInSidebar)
                    twMenu.WriteLine(_conf.captions.Single(s => s.captionName.Equals("MONEY_SIDEBAR_TITLE_CAP")).caption + " <<printMoney>>" +
                        _conf.captions.Single(s => s.captionName.Equals("MONEY_UNIT_CAP")).caption);

                if (_conf.daytimeActive && _conf.daytimeInSidebar)
                {
                    switch(_conf.daytimeFormat)
                    {
                        case 0: twMenu.WriteLine("<<getTime>>"); break;
                        case 1: twMenu.WriteLine("<<getDate>>"); break;
                        case 2: twMenu.WriteLine("<<getDateTime>>"); break;
                        case 3: twMenu.WriteLine("<<getTimeOfDay>>"); break;
                    }
                }
                twMenu.WriteLine("");

                // Sidebar menus
                if (_conf.inventoryActive && _conf.inventoryInSidebar) twMenu.WriteLine("<<inventorySidebar>>");
                if (_conf.clothActive && _conf.clothInSidebar) twMenu.WriteLine("<<clothSidebar>>");
                if (_conf.statsActive && _conf.statsInSidebar) twMenu.WriteLine("<<statsSidebar>>");
                if (_conf.charactersActive && _conf.charactersInSidebar) twMenu.WriteLine("<<charactersSidebar>>");
                twMenu.WriteLine("");

                // Menu paragraphs
                if (_conf.inventoryActive)
                {
                    twMenu.WriteLine("::InventoryMenu[noreturn]");
                    twMenu.WriteLine("<<if $inventory.length == 0>>" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_EMPTY_CAP")).caption +
                        "<<else>>" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_TITLE_CAP")).caption);
                    twMenu.WriteLine("<<inventory>>");
                    twMenu.WriteLine("<<endif>>");
                    twMenu.WriteLine("[["+ _conf.captions.Single(s => s.captionName.Equals("BACK_CAP")).caption +"|$return]]");
                    twMenu.WriteLine("");
                }

                if (_conf.clothActive)
                {
                    twMenu.WriteLine("::ClothMenu[noreturn]");
                    twMenu.WriteLine("<h1>"+_conf.captions.Single(s => s.captionName.Equals("CLOTH_TITLE_CAP")).caption+"</h1>");
                    twMenu.WriteLine("<<cloth>>");
                    twMenu.WriteLine("[[" + _conf.captions.Single(s => s.captionName.Equals("BACK_CAP")).caption + "|$return]]");
                    twMenu.WriteLine("");

                    twMenu.WriteLine("::WardrobeMenu[noreturn]");
                    twMenu.WriteLine("<h1>" + _conf.captions.Single(s => s.captionName.Equals("WARDROBE_TITLE_CAP")).caption + "</h1>");
                    twMenu.WriteLine("<<wardrobe>>");
                    twMenu.WriteLine("[[" + _conf.captions.Single(s => s.captionName.Equals("BACK_CAP")).caption + "|$return]]");
                    twMenu.WriteLine("");
                }

                if (_conf.statsActive)
                {
                    twMenu.WriteLine("::StatsMenu[noreturn]");
                    twMenu.WriteLine("<<stats>>");
                    twMenu.WriteLine("[[" + _conf.captions.Single(s => s.captionName.Equals("BACK_CAP")).caption + "|$return]]");
                    twMenu.WriteLine("");
                }

                if (_conf.charactersActive)
                {
                    twMenu.WriteLine("::CharactersMenu[noreturn]");
                    twMenu.WriteLine("<<characters>>");
                    twMenu.WriteLine("[[" + _conf.captions.Single(s => s.captionName.Equals("BACK_CAP")).caption + "|$return]]");
                    twMenu.WriteLine("");
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

        private static void generateInventory(Configuration _conf, string _path)
        {
            string inventoryPath = Path.Combine(_path, "_js_inventory.tw2");
            TextWriter twInventory = null;
            try
            {
                twInventory = new StreamWriter(inventoryPath, false, new UTF8Encoding(false));
                twInventory.WriteLine("::Inventory[script]");
                twInventory.WriteLine("");

                // Init items
                twInventory.WriteLine("macros.initItems = {");
                twInventory.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twInventory.WriteLine("\t\tif (state.active.variables.items === undefined) {");
                twInventory.WriteLine("\t\t\tstate.active.variables.items = [];");

                for(int i=0; i<_conf.items.Count; i++)
                {
                    twInventory.Write("\t\t\tstate.active.variables.items.push({");
                    twInventory.Write("\"ID\":" + _conf.items[i].ID + ",");
                    twInventory.Write("\"name\":\"" + _conf.items[i].name + "\",");
                    twInventory.Write("\"description\":\"" + _conf.items[i].description + "\",");
                    twInventory.Write("\"category\":\"" + _conf.items[i].category + "\",");
                    twInventory.Write("\"shopCategory\":\"" + _conf.items[i].shopCategory + "\",");
                    twInventory.Write("\"image\":\"" + pathSubtract(_conf.items[i].image, _conf.pathSubtract) + "\",");
                    twInventory.Write("\"canBeBought\":" + _conf.items[i].canBeBought.ToString().ToLower() + ",");
                    twInventory.Write("\"buyPrice\":" + _conf.items[i].buyPrice+ ",");
                    twInventory.Write("\"sellPrice\":" + _conf.items[i].sellPrice + ",");
                    twInventory.Write("\"canOwnMultiple\":" + _conf.items[i].canOwnMultiple.ToString().ToLower() + ",");
                    twInventory.Write("\"owned\":" + _conf.items[i].owned);
                    if (_conf.inventoryUseSkill1)
                    {
                        string skill1val = (isBool(_conf.items[i].skill1) || isNumber(_conf.items[i].skill1)) ? _conf.items[i].skill1 : "\"" + _conf.items[i].skill1 + "\"";
                        twInventory.Write(",\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL1_CAP")).caption + "\":" + skill1val);
                    }
                    if (_conf.inventoryUseSkill2)
                    {
                        string skill2val = (isBool(_conf.items[i].skill2) || isNumber(_conf.items[i].skill2)) ? _conf.items[i].skill2 : "\"" + _conf.items[i].skill2 + "\"";
                        twInventory.Write(",\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL2_CAP")).caption + "\":" + skill2val);
                    }
                    if (_conf.inventoryUseSkill3)
                    {
                        string skill3val = (isBool(_conf.items[i].skill3) || isNumber(_conf.items[i].skill3)) ? _conf.items[i].skill3 : "\"" + _conf.items[i].skill3 + "\"";
                        twInventory.Write(",\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL3_CAP")).caption + "\":" + skill3val);
                    }
                    twInventory.WriteLine("});");
                }
                twInventory.WriteLine("\t\t}");
                twInventory.WriteLine("\t}");
                twInventory.WriteLine("};");
                twInventory.WriteLine("");

                // getInventory
                twInventory.WriteLine("window.getInventory = function() {");
                twInventory.WriteLine("\treturn state.active.variables.inventory;");
                twInventory.WriteLine("}");
                twInventory.WriteLine("");

                // initInventory
                twInventory.WriteLine("macros.initInventory = {");
                twInventory.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twInventory.WriteLine("\t\tstate.active.variables.inventory = [];");

                // Add items to inventory
                for(int i=0; i<_conf.items.Count; i++)
                {
                    if (_conf.items[i].owned > 0)
                    {
                        twInventory.Write("\t\tstate.active.variables.inventory.push({");
                        twInventory.Write("\"ID\":" + _conf.items[i].ID + ",");
                        twInventory.Write("\"name\":\"" + _conf.items[i].name + "\",");
                        twInventory.Write("\"description\":\"" + _conf.items[i].description + "\",");
                        twInventory.Write("\"category\":\"" + _conf.items[i].category + "\",");
                        twInventory.Write("\"shopCategory\":\"" + _conf.items[i].shopCategory + "\",");
                        twInventory.Write("\"image\":\"" + pathSubtract(_conf.items[i].image, _conf.pathSubtract) + "\",");
                        twInventory.Write("\"canBeBought\":" + _conf.items[i].canBeBought.ToString().ToLower() + ",");
                        twInventory.Write("\"buyPrice\":" + _conf.items[i].buyPrice + ",");
                        twInventory.Write("\"sellPrice\":" + _conf.items[i].sellPrice + ",");
                        twInventory.Write("\"canOwnMultiple\":" + _conf.items[i].canOwnMultiple.ToString().ToLower() + ",");
                        twInventory.Write("\"owned\":" + _conf.items[i].owned);
                        if (_conf.inventoryUseSkill1)
                        {
                            string skill1val = (isBool(_conf.items[i].skill1) || isNumber(_conf.items[i].skill1)) ? _conf.items[i].skill1 : "\"" + _conf.items[i].skill1 + "\"";
                            twInventory.Write(",\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL1_CAP")).caption + "\":" + skill1val);
                        }
                        if (_conf.inventoryUseSkill2)
                        {
                            string skill2val = (isBool(_conf.items[i].skill2) || isNumber(_conf.items[i].skill2)) ? _conf.items[i].skill2 : "\"" + _conf.items[i].skill2 + "\"";
                            twInventory.Write(",\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL2_CAP")).caption + "\":" + skill2val);
                        }
                        if (_conf.inventoryUseSkill3)
                        {
                            string skill3val = (isBool(_conf.items[i].skill3) || isNumber(_conf.items[i].skill3)) ? _conf.items[i].skill3 : "\"" + _conf.items[i].skill3 + "\"";
                            twInventory.Write(",\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL3_CAP")).caption + "\":" + skill3val);
                        }
                        twInventory.WriteLine("});");
                    }
                }
                twInventory.WriteLine("\t}");
                twInventory.WriteLine("};");

                // addToInventory
                twInventory.WriteLine("macros.addToInventory = {");
                twInventory.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twInventory.WriteLine("\t\tif (params.length != 2) {");
                twInventory.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two parameters, item id and amount.\");");
                twInventory.WriteLine("\t\t\treturn;");
                twInventory.WriteLine("\t\t}");
                twInventory.WriteLine("\t\tif ((!Number.isInteger(params[0])) || (!Number.isInteger(params[1]))) {");
                twInventory.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two integer parameters.\");");
                twInventory.WriteLine("\t\t\treturn;");
                twInventory.WriteLine("\t\t}");
                twInventory.WriteLine("\t\tif (typeof state.active.variables.items[params[0]] === 'undefined') {");
                twInventory.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: An item with id \" + params[0] + \" does not exist.\");");
                twInventory.WriteLine("\t\t\treturn;");
                twInventory.WriteLine("\t\t}");
                twInventory.WriteLine("\t\tvar item_in_catalog = state.active.variables.items.filter(obj => { return obj.ID === params[0]});");
                twInventory.WriteLine("\t\tif (item_in_catalog.length != 1) {");
                twInventory.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: There must be exactly one item of id \" + params[0] + \" in the item catalog but there are \" + item_in_catalog.length);");
                twInventory.WriteLine("\t\t\treturn;");
                twInventory.WriteLine("\t\t}");
                twInventory.WriteLine("\t\tvar item = JSON.parse(JSON.stringify(item_in_catalog[0]));");
                twInventory.WriteLine("\t\titem.owned = params[1];");
                twInventory.WriteLine("\t\tvar existing_items_with_id = state.active.variables.inventory.filter(obj => {return obj.ID === item.ID});");
                twInventory.WriteLine("\t\tif (existing_items_with_id.length == 0) {");
                twInventory.WriteLine("\t\t\tstate.active.variables.inventory.push(item);");
                twInventory.WriteLine("\t\t} else if (existing_items_with_id.length > 0) {");
                twInventory.WriteLine("\t\t\tfor (var i in state.active.variables.inventory) {");
                twInventory.WriteLine("\t\t\t\tif (state.active.variables.inventory[i].ID == item.ID) {");
                twInventory.WriteLine("\t\t\t\t\tstate.active.variables.inventory[i].owned += item.owned;");
                twInventory.WriteLine("\t\t\t\t\tbreak;");
                twInventory.WriteLine("\t\t\t\t}");
                twInventory.WriteLine("\t\t\t}");
                twInventory.WriteLine("\t\t} else {");
                twInventory.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: There are several items with the same id \" + item.ID);");
                twInventory.WriteLine("\t\t\treturn;");
                twInventory.WriteLine("\t\t}");
                twInventory.WriteLine("\t}");
                twInventory.WriteLine("};");
                twInventory.WriteLine("");

                // removeFromInventory
                twInventory.WriteLine("macros.removeFromInventory = {");
                twInventory.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twInventory.WriteLine("\t\tif ((params.length == 0) || (params.length > 2)) {");
                twInventory.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: expecting one or two parameters.\");");
                twInventory.WriteLine("\t\t\treturn;");
                twInventory.WriteLine("\t\t}");
                twInventory.WriteLine("\t\tvar existing_items_with_id = state.active.variables.inventory.filter(obj => {return obj.ID === params[0]});");
                twInventory.WriteLine("\t\tif (existing_items_with_id.length == 0) return;");
                twInventory.WriteLine("\t\tif (params.length == 1) {");
                twInventory.WriteLine("\t\t\tfor (var i in state.active.variables.inventory) {");
                twInventory.WriteLine("\t\t\t\tif (state.active.variables.inventory[i].ID == params[0]) {");
                twInventory.WriteLine("\t\t\t\t\tstate.active.variables.inventory.splice(i, 1);");
                twInventory.WriteLine("\t\t\t\t\tbreak;");
                twInventory.WriteLine("\t\t\t\t}");
                twInventory.WriteLine("\t\t\t}");
                twInventory.WriteLine("\t\t} else if (params.length == 2) {");
                twInventory.WriteLine("\t\t\tfor (var i in state.active.variables.inventory) {");
                twInventory.WriteLine("\t\t\t\tif (state.active.variables.inventory[i].ID == params[0]) {");
                twInventory.WriteLine("\t\t\t\t\tstate.active.variables.inventory[i].owned -= params[1];");
                twInventory.WriteLine("\t\t\t\t\tif (state.active.variables.inventory[i].owned <= 0) {");
                twInventory.WriteLine("\t\t\t\t\t\tstate.active.variables.inventory.splice(i, 1);");
                twInventory.WriteLine("\t\t\t\t\t}");
                twInventory.WriteLine("\t\t\t\t\tbreak;");
                twInventory.WriteLine("\t\t\t\t}");
                twInventory.WriteLine("\t\t\t}");
                twInventory.WriteLine("\t\t}");
                twInventory.WriteLine("\t}");
                twInventory.WriteLine("};");
                twInventory.WriteLine("");

                // Inventory
                twInventory.WriteLine("macros.inventory = {");
                twInventory.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twInventory.WriteLine("\t\tif (state.active.variables.inventory.length == 0) {");
                twInventory.WriteLine("\t\t\tnew Wikifier(place, 'nothing');");
                twInventory.WriteLine("\t\t} else {");
                twInventory.WriteLine("\t\t\tvar inv_str = \"<table class=\\\"inventory\\\"><tr>\";");
                if (_conf.displayInInventory.Contains("ID")) twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_ID_CAP")).caption + "</th>\";");
                if (_conf.displayInInventory.Contains("Name")) twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_NAME_CAP")).caption + "</th>\";");
                if (_conf.displayInInventory.Contains("Description")) twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_DESCRIPTION_CAP")).caption + "</th>\";");
                if (_conf.displayInInventory.Contains("Category")) twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_CATEGORY_CAP")).caption + "</th>\";");
                if (_conf.displayInInventory.Contains("Shop category")) twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SHOP_CATEGORY_CAP")).caption + "</th>\";");
                if (_conf.displayInInventory.Contains("Owned")) twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_OWNED_CAP")).caption + "</th>\";");
                if (_conf.displayInInventory.Contains("Can buy")) twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_CAN_BUY_CAP")).caption + "</th>\";");
                if (_conf.displayInInventory.Contains("Buy price")) twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_BUY_PRICE_CAP")).caption + "</th>\";");
                if (_conf.displayInInventory.Contains("Sell price")) twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SELL_PRICE_CAP")).caption + "</th>\";");
                if (_conf.displayInInventory.Contains("Can own multiple")) twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_CAN_OWN_MULTIPLE_CAP")).caption + "</th>\";");

                if (_conf.displayInInventory.Contains("Skill1") && _conf.inventoryUseSkill1)
                    twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\" >" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL1_CAP")).caption + "</th>\";");

                if (_conf.displayInInventory.Contains("Skill2") && _conf.inventoryUseSkill2)
                    twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL2_CAP")).caption + "</th>\";");

                if (_conf.displayInInventory.Contains("Skill3") && _conf.inventoryUseSkill3)
                    twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_SKILL3_CAP")).caption + "</th>\";");

                if (_conf.displayInInventory.Contains("Image")) twInventory.WriteLine("\t\t\tinv_str += \"<th class=\\\"inventory\\\">" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_IMAGE_CAP")).caption + "</th>\";");
                twInventory.WriteLine("\t\t\tinv_str += \"</tr>\";");

                twInventory.WriteLine("\t\t\tfor (var i = 0; i < state.active.variables.inventory.length; i++)");
                twInventory.WriteLine("\t\t\t{");
                twInventory.WriteLine("\t\t\t\tinv_str += \"<tr>\";");
                if (_conf.displayInInventory.Contains("ID")) twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\">\" + state.active.variables.inventory[i].ID + \"</td>\";");
                if (_conf.displayInInventory.Contains("Name")) twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].name + \"</td>\";");
                if (_conf.displayInInventory.Contains("Description")) twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].description + \"</td>\";");
                if (_conf.displayInInventory.Contains("Category")) twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].category + \"</td>\";");
                if (_conf.displayInInventory.Contains("Shop category")) twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].shopCategory + \"</td>\";");
                if (_conf.displayInInventory.Contains("Owned")) twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].owned + \"</td>\";");
                if (_conf.displayInInventory.Contains("Can buy")) twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].canBeBought + \"</td>\";");
                if (_conf.displayInInventory.Contains("Buy price")) twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].buyPrice + \"</td>\";");
                if (_conf.displayInInventory.Contains("Sell price")) twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].sellPrice + \"</td>\";");
                if (_conf.displayInInventory.Contains("Can own multiple")) twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].canOwnMultiple + \"</td>\";");

                if (_conf.displayInInventory.Contains("Skill1") && _conf.inventoryUseSkill1)
                    twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].skill1 + \"</td>\";");

                if (_conf.displayInInventory.Contains("Skill2") && _conf.inventoryUseSkill2)
                    twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].skill2 + \"</td>\";");

                if (_conf.displayInInventory.Contains("Skill3") && _conf.inventoryUseSkill3)
                    twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"> \" + state.active.variables.inventory[i].skill3 + \"</td>\";");

                if (_conf.displayInInventory.Contains("Image")) twInventory.WriteLine("\t\t\t\tinv_str += \"<td class=\\\"inventory\\\"><img class=\\\"paragraph\\\" src=\\\"\" + state.active.variables.inventory[i].image + \"\\\" ></td>\";");
                twInventory.WriteLine("\t\t\t\tinv_str += \"</tr>\";");
                twInventory.WriteLine("\t\t\t}");
                twInventory.WriteLine("\t\t\tinv_str += \"</table>\";");
                twInventory.WriteLine("\t\t\tnew Wikifier(place, inv_str);");
                twInventory.WriteLine("\t\t}");
                twInventory.WriteLine("\t}");
                twInventory.WriteLine("};");
                twInventory.WriteLine("");

                // inventorySidebar
                twInventory.WriteLine("macros.inventorySidebar = {");
                twInventory.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twInventory.WriteLine("");
                twInventory.WriteLine("\t\tvar wstr = \"<table class=\\\"inventory_sidebar\\\">\";");
                twInventory.WriteLine("\t\twstr +=\"<tr><td colspan=2>Inventory</td></tr>\";");
                twInventory.WriteLine("\t\tfor (var w = 0; w<state.active.variables.inventory.length; w +=2) {");
                twInventory.WriteLine("\t\t\twstr +=\"<tr>\";");
                twInventory.WriteLine("");
                twInventory.WriteLine("\t\t\tvar item_info_1 = \"\";");
                if (_conf.displayInInventory.Contains("ID")) twInventory.WriteLine("\t\t\titem_info_1 += \"ID: \" + state.active.variables.inventory[w].ID + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Name")) twInventory.WriteLine("\t\t\titem_info_1 += \"name:\" + state.active.variables.inventory[w].name + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Description")) twInventory.WriteLine("\t\t\titem_info_1 += \"description:\" + state.active.variables.inventory[w].description + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Category")) twInventory.WriteLine("\t\t\titem_info_1 += \"category:\" + state.active.variables.inventory[w].category + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Shop category")) twInventory.WriteLine("\t\t\titem_info_1 += \"shop category:\" + state.active.variables.inventory[w].shopCategory + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Owned")) twInventory.WriteLine("\t\t\titem_info_1 += \"owned:\" + state.active.variables.inventory[w].owned + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Can buy")) twInventory.WriteLine("\t\t\titem_info_1 += \"can buy:\" + state.active.variables.inventory[w].canBeBought + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Buy price")) twInventory.WriteLine("\t\t\titem_info_1 += \"buy price:\" + state.active.variables.inventory[w].buyPrice + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Sell price")) twInventory.WriteLine("\t\t\titem_info_1 += \"sell price:\" + state.active.variables.inventory[w].sellPrice + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Can own multiple")) twInventory.WriteLine("\t\t\titem_info_1 += \"can own multiple:\" + state.active.variables.inventory[w].canOwnMultiple + \"&#10;\";");

                if (_conf.displayInInventory.Contains("Skill1") && _conf.inventoryUseSkill1)
                    twInventory.WriteLine("\t\t\titem_info_1 +=\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SKILL1_CAP")).caption + ": \" + state.active.variables.inventory[w].skill1 + \"&#10;\";");

                if (_conf.displayInInventory.Contains("Skill2") && _conf.inventoryUseSkill2)
                    twInventory.WriteLine("\t\t\titem_info_1 +=\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SKILL2_CAP")).caption + ": \" + state.active.variables.inventory[w].skill2 + \"&#10;\";");

                if (_conf.displayInInventory.Contains("Skill3") && _conf.inventoryUseSkill3)
                    twInventory.WriteLine("\t\t\titem_info_1 +=\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SKILL3_CAP")).caption + ": \" + state.active.variables.inventory[w].skill3 + \"&#10;\";");

                if (_conf.inventorySidebarTooltip)
                {
                    twInventory.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.inventory[w].image + \"\\\" title=\\\"\" + item_info_1 + \"\\\"></td>\";");
                } else
                {
                    twInventory.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.inventory[w].image + \"\\\"></td>\";");
                }
                twInventory.WriteLine("");
                twInventory.WriteLine("\t\t\tif (w+1 < state.active.variables.inventory.length) {");
                twInventory.WriteLine("");
                twInventory.WriteLine("\t\t\t\tvar item_info_2 = \"\";");
                if (_conf.displayInInventory.Contains("ID")) twInventory.WriteLine("\t\t\t\titem_info_2 += \"ID: \" + state.active.variables.inventory[w+1].ID + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Name")) twInventory.WriteLine("\t\t\t\titem_info_2 += \"name:\" + state.active.variables.inventory[w+1].name + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Description")) twInventory.WriteLine("\t\t\t\titem_info_2 += \"description:\" + state.active.variables.inventory[w+1].description + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Category")) twInventory.WriteLine("\t\t\t\titem_info_2 += \"category:\" + state.active.variables.inventory[w+1].category + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Shop category")) twInventory.WriteLine("\t\t\t\titem_info_2 += \"shop category:\" + state.active.variables.inventory[w+1].shopCategory + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Owned")) twInventory.WriteLine("\t\t\t\titem_info_2 += \"owned:\" + state.active.variables.inventory[w+1].owned + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Can buy")) twInventory.WriteLine("\t\t\t\titem_info_2 += \"can buy:\" + state.active.variables.inventory[w+1].canBeBought + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Buy price")) twInventory.WriteLine("\t\t\t\titem_info_2 += \"buy price:\" + state.active.variables.inventory[w+1].buyPrice + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Sell price")) twInventory.WriteLine("\t\t\t\titem_info_2 += \"sell price:\" + state.active.variables.inventory[w+1].sellPrice + \"&#10;\";");
                if (_conf.displayInInventory.Contains("Can own multiple")) twInventory.WriteLine("\t\t\t\titem_info_2 += \"can own multiple:\" + state.active.variables.inventory[w+1].canOwnMultiple + \"&#10;\";");

                if (_conf.displayInInventory.Contains("Skill1") && _conf.inventoryUseSkill1)
                    twInventory.WriteLine("\t\t\t\titem_info_2 +=\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SKILL1_CAP")).caption + ": \" + state.active.variables.inventory[w+1].skill1 + \"&#10;\";");

                if (_conf.displayInInventory.Contains("Skill2") && _conf.inventoryUseSkill2)
                    twInventory.WriteLine("\t\t\t\titem_info_2 +=\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SKILL2_CAP")).caption + ": \" + state.active.variables.inventory[w+1].skill2 + \"&#10;\";");

                if (_conf.displayInInventory.Contains("Skill3") && _conf.inventoryUseSkill3)
                    twInventory.WriteLine("\t\t\t\titem_info_2 +=\"" + _conf.captions.Single(s => s.captionName.Equals("INVENTORY_COL_SKILL3_CAP")).caption + ": \" + state.active.variables.inventory[w+1].skill3 + \"&#10;\";");

                twInventory.WriteLine("");

                if (_conf.inventorySidebarTooltip)
                {
                    twInventory.WriteLine("\t\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.inventory[w+1].image + \"\\\" title=\\\"\" + item_info_2 + \"\\\"></td>\";");
                } else
                {
                    twInventory.WriteLine("\t\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.inventory[w+1].image + \"\\\"></td>\";");
                }
                twInventory.WriteLine("\t\t\t} else {");
                twInventory.WriteLine("\t\t\t\twstr +=\"<td></td>\";");
                twInventory.WriteLine("\t\t\t}");
                twInventory.WriteLine("\t\t\twstr +=\"</tr>\";");
                twInventory.WriteLine("\t\t}");
                twInventory.WriteLine("\t\twstr +=\"</table>\";");
                twInventory.WriteLine("");
                twInventory.WriteLine("\t\tnew Wikifier(place,wstr);");
                twInventory.WriteLine("\t}");
                twInventory.WriteLine("};");
                twInventory.WriteLine("");

                // clearInventory
                twInventory.WriteLine("macros.clearInventory = {");
                twInventory.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twInventory.WriteLine("\t\tstate.active.variables.inventory = [];");
                twInventory.WriteLine("\t}");
                twInventory.WriteLine("};");
                twInventory.WriteLine("");
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

        private static void generateCloth(Configuration _conf, string _path)
        {
            string clothPath = Path.Combine(_path, "_js_cloth.tw2");
            TextWriter twCloth = null;
            try
            {
                twCloth = new StreamWriter(clothPath, false, new UTF8Encoding(false));
                twCloth.WriteLine("::Captions[script]");
                twCloth.WriteLine("");

                // Constants
                twCloth.WriteLine("var HEAD_NAME = \""+ _conf.captions.Single(s => s.captionName.Equals("HEAD_CAP")).caption + "\";");
                twCloth.WriteLine("var HAIR_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("HAIR_CAP")).caption + "\";");
                twCloth.WriteLine("var NECK_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("NECK_CAP")).caption + "\";");
                twCloth.WriteLine("var UPPER_BODY_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("UPPER_BODY_CAP")).caption + "\";");
                twCloth.WriteLine("var LOWER_BODY_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("LOWER_BODY_CAP")).caption + "\";");
                twCloth.WriteLine("var BELT_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("BELT_CAP")).caption + "\";");
                twCloth.WriteLine("var SOCKS_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("SOCKS_CAP")).caption + "\";");
                twCloth.WriteLine("var SHOES_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("SHOES_CAP")).caption + "\";");
                twCloth.WriteLine("var UNDERWEAR_TOP_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("UNDERWEAR_TOP_CAP")).caption + "\";");
                twCloth.WriteLine("var UNDERWEAR_BOTTOM_NAME = \"" + _conf.captions.Single(s => s.captionName.Equals("UNDERWEAR_BOTTOM_CAP")).caption + "\";");
                twCloth.WriteLine("");

                // InitAllCloth
                twCloth.WriteLine("macros.initAllCloth = {");
                twCloth.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twCloth.WriteLine("\t\tif (state.active.variables.all_cloth === undefined)");
                twCloth.WriteLine("\t\t{");
                twCloth.WriteLine("\t\t\tstate.active.variables.all_cloth = [];");
                for(int i=0; i<_conf.cloth.Count; i++)
                {
                    twCloth.Write("\t\t\tstate.active.variables.all_cloth.push({");
                    twCloth.Write("\"ID\":" + _conf.cloth[i].ID + ",");
                    twCloth.Write("\"name\":\"" + _conf.cloth[i].name + "\",");
                    twCloth.Write("\"description\":\"" + _conf.cloth[i].description + "\",");
                    twCloth.Write("\"canBuy\":" + _conf.cloth[i].canBeBought.ToString().ToLower() + ",");
                    twCloth.Write("\"shopCategory\":\"" + _conf.cloth[i].shopCategory + "\",");
                    twCloth.Write("\"category\":\"" + _conf.cloth[i].category + "\",");
                    twCloth.Write("\"bodyPart\":\"" + _conf.cloth[i].bodyPart + "\",");
                    twCloth.Write("\"image\":\"" + pathSubtract(_conf.cloth[i].image, _conf.pathSubtract) + "\",");
                    twCloth.Write("\"buyPrice\":" + _conf.cloth[i].buyPrice + ",");
                    twCloth.Write("\"sellPrice\":" + _conf.cloth[i].sellPrice + ",");
                    twCloth.Write("\"isWorn\":" + _conf.cloth[i].isWornAtBeginning.ToString().ToLower() + ",");
                    twCloth.Write("\"canOwnMultiple\":" + _conf.cloth[i].canOwnMultiple.ToString().ToLower() + ",");
                    twCloth.Write("\"owned\":" + _conf.cloth[i].owned);
                    if (_conf.clothUseSkill1)
                    {
                        string skill1val = (isBool(_conf.cloth[i].skill1) || isNumber(_conf.cloth[i].skill1)) ? _conf.cloth[i].skill1 : "\"" + _conf.cloth[i].skill1 + "\"";
                        twCloth.Write(",\"" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_SKILL1_CAP")).caption + "\":" + skill1val);
                    }
                    if (_conf.clothUseSkill2)
                    {
                        string skill2val = (isBool(_conf.cloth[i].skill2) || isNumber(_conf.cloth[i].skill2)) ? _conf.cloth[i].skill2 : "\"" + _conf.cloth[i].skill2 + "\"";
                        twCloth.Write(",\"" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_SKILL2_CAP")).caption + "\":" + skill2val);
                    }
                    if (_conf.clothUseSkill3)
                    {
                        string skill3val = (isBool(_conf.cloth[i].skill3) || isNumber(_conf.cloth[i].skill3)) ? _conf.cloth[i].skill3 : "\"" + _conf.cloth[i].skill3 + "\"";
                        twCloth.Write(",\"" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_SKILL3_CAP")).caption + "\":" + skill3val);
                    }
                    twCloth.WriteLine(",\"isWornAtBeginning\":" + _conf.cloth[i].isWornAtBeginning.ToString().ToLower() + "});");
                }
                twCloth.WriteLine("\t\t}");
                twCloth.WriteLine("\t}");
                twCloth.WriteLine("};");
                twCloth.WriteLine("");

                // isWorn
                twCloth.WriteLine("window.is_worn = function(_id) {");
                twCloth.WriteLine("\tfor (var i = 0; i < state.active.variables.wearing.length; i++) {");
                twCloth.WriteLine("\t\tif (state.active.variables.wearing[i].ID == _id) return true;");
                twCloth.WriteLine("\t}");
                twCloth.WriteLine("\treturn false;");
                twCloth.WriteLine("};");
                twCloth.WriteLine("");

                // initCloth
                twCloth.WriteLine("macros.initCloth = {");
                twCloth.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twCloth.WriteLine("\t\tif (state.active.variables.wearing === undefined)");
                twCloth.WriteLine("\t\t{");
                twCloth.WriteLine("\t\t\tstate.active.variables.wearing = {};");
                for(int i=0; i<_conf.cloth.Count; i++)
                {
                    if ((_conf.cloth[i].isWornAtBeginning) && (_conf.cloth[i].owned > 0))
                    {
                        twCloth.WriteLine("\t\t\tstate.active.variables.wearing[" + _conf.cloth[i].bodyPart + "] = state.active.variables.all_cloth[" + i + "];");
                    }
                }
                twCloth.WriteLine("\t\t}");
                twCloth.WriteLine("\t}");
                twCloth.WriteLine("};");
                twCloth.WriteLine("");

                // initWardrobe
                twCloth.WriteLine("macros.initWardrobe = {");
                twCloth.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twCloth.WriteLine("\t\tif (state.active.variables.wardrobe === undefined)");
                twCloth.WriteLine("\t\t{");
                twCloth.WriteLine("\t\t\tstate.active.variables.wardrobe = [];");
                for (int i = 0; i < _conf.cloth.Count; i++)
                {
                    if ((_conf.cloth[i].isWornAtBeginning) && (_conf.cloth[i].owned > 0))
                    {
                        twCloth.WriteLine("\t\t\tstate.active.variables.wardrobe.push(state.active.variables.all_cloth[" + i + "]);");
                    }
                }
                twCloth.WriteLine("\t\t}");
                twCloth.WriteLine("\t}");
                twCloth.WriteLine("};");
                twCloth.WriteLine("");

                // cloth
                twCloth.WriteLine("macros.cloth = {");
                twCloth.WriteLine("\thandler: function(place, macroName, params, parser) {");

                // header
                twCloth.WriteLine("\t\tvar s = \"<table class=\\\"cloth\\\">\";");
                twCloth.WriteLine("\t\ts +=\"<tr>\";");
                if (_conf.displayInClothView.Contains("ID"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_ID_CAP")).caption + "</td>\";");
                if (_conf.displayInClothView.Contains("Name"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_NAME_CAP")).caption + "</td>\";");
                if (_conf.displayInClothView.Contains("Description"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_DESCRIPTION_CAP")).caption + "</td>\";");
                if (_conf.displayInClothView.Contains("Category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_CATEGORY_CAP")).caption + "</td>\";");
                if (_conf.displayInClothView.Contains("Shop category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_SHOP_CATEGORY_CAP")).caption + "</td>\";");
                if (_conf.displayInClothView.Contains("Body part"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_BODY_PART_CAP")).caption + "</td>\";");
                if (_conf.displayInClothView.Contains("Owned"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_OWNED_CAP")).caption + "</td>\";");
                if (_conf.displayInClothView.Contains("Is worn"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_IS_WORN_CAP")).caption + "</td>\";");
                if (_conf.displayInClothView.Contains("Can buy"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_CAN_BUY_CAP")).caption + "</td>\";");
                if (_conf.displayInClothView.Contains("Buy price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_BUY_PRICE_CAP")).caption + "</td>\";");
                if (_conf.displayInClothView.Contains("Sell price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_SELL_PRICE_CAP")).caption + "</td>\";");
                if (_conf.displayInClothView.Contains("Can own multiple"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_CAN_OWN_MULTIPLE_CAP")).caption + "</td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInClothView.Contains("Skill1"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_SKILL1_CAP")).caption + "</b></td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInClothView.Contains("Skill2"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_SKILL2_CAP")).caption + "</b></td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInClothView.Contains("Skill3"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_SKILL3_CAP")).caption + "</b></td>\";");
                twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_IMAGE_CAP")).caption + "</td>\";");
                twCloth.WriteLine("\t\ts +=\"</tr>\";");

                // head
                twCloth.WriteLine("\t\ts +=\"<tr>\";");
                if (_conf.displayInClothView.Contains("ID"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].ID + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Name"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].name + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Description"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].description + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].category + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Shop category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].shopCategory + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Body part"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].bodyPart + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Owned"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].owned + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Is worn"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].isWorn + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can buy"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].canBuy + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Buy price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].buyPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Sell price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].sellPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can own multiple"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].canOwnMultiple + \"</b></td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInClothView.Contains("Skill1"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].skill1 + \"</b></td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInClothView.Contains("Skill2"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].skill2 + \"</b></td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInClothView.Contains("Skill3"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HEAD_NAME].skill3 + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Image"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[HEAD_NAME].image+\"></td>\";");
                twCloth.WriteLine("\t\ts +=\"</tr>\";");

                // hair
                twCloth.WriteLine("\t\ts +=\"<tr>\";");
                if (_conf.displayInClothView.Contains("ID"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].ID + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Name"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].name + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Description"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].description + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].category + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Shop category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].shopCategory + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Body part"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].bodyPart + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Owned"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].owned + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Is worn"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].isWorn + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can buy"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].canBuy + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Buy price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].buyPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Sell price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].sellPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can own multiple"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].canOwnMultiple + \"</b></td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInClothView.Contains("Skill1"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].skill1 + \"</b></td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInClothView.Contains("Skill2"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].skill2 + \"</b></td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInClothView.Contains("Skill3"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[HAIR_NAME].skill3 + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Image"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[HAIR_NAME].image+\"></td>\";");
                twCloth.WriteLine("\t\ts +=\"</tr>\";");

                // neck
                twCloth.WriteLine("\t\ts +=\"<tr>\";");
                if (_conf.displayInClothView.Contains("ID"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].ID + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Name"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].name + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Description"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].description + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].category + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Shop category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].shopCategory + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Body part"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].bodyPart + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Owned"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].owned + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Is worn"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].isWorn + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can buy"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].canBuy + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Buy price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].buyPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Sell price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].sellPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can own multiple"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].canOwnMultiple + \"</b></td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInClothView.Contains("Skill1"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].skill1 + \"</b></td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInClothView.Contains("Skill2"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].skill2 + \"</b></td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInClothView.Contains("Skill3"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[NECK_NAME].skill3 + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Image"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[NECK_NAME].image+\"></td>\";");
                twCloth.WriteLine("\t\ts +=\"</tr>\";");

                // upper body
                twCloth.WriteLine("\t\ts +=\"<tr>\";");
                if (_conf.displayInClothView.Contains("ID"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].ID + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Name"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].name + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Description"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].description + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].category + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Shop category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].shopCategory + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Body part"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].bodyPart + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Owned"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].owned + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Is worn"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].isWorn + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can buy"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].canBuy + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Buy price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].buyPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Sell price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].sellPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can own multiple"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].canOwnMultiple + \"</b></td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInClothView.Contains("Skill1"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].skill1 + \"</b></td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInClothView.Contains("Skill2"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].skill2 + \"</b></td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInClothView.Contains("Skill3"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UPPER_BODY_NAME].skill3 + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Image"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[UPPER_BODY_NAME].image+\"></td>\";");
                twCloth.WriteLine("\t\ts +=\"</tr>\";");

                // lower body
                twCloth.WriteLine("\t\ts +=\"<tr>\";");
                if (_conf.displayInClothView.Contains("ID"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].ID + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Name"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].name + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Description"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].description + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].category + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Shop category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].shopCategory + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Body part"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].bodyPart + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Owned"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].owned + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Is worn"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].isWorn + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can buy"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].canBuy + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Buy price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].buyPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Sell price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].sellPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can own multiple"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].canOwnMultiple + \"</b></td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInClothView.Contains("Skill1"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].skill1 + \"</b></td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInClothView.Contains("Skill2"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].skill2 + \"</b></td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInClothView.Contains("Skill3"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[LOWER_BODY_NAME].skill3 + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Image"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[LOWER_BODY_NAME].image+\"></td>\";");
                twCloth.WriteLine("\t\ts +=\"</tr>\";");

                // belt
                twCloth.WriteLine("\t\ts +=\"<tr>\";");
                if (_conf.displayInClothView.Contains("ID"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].ID + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Name"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].name + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Description"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].description + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].category + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Shop category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].shopCategory + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Body part"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].bodyPart + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Owned"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].owned + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Is worn"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].isWorn + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can buy"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].canBuy + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Buy price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].buyPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Sell price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].sellPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can own multiple"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].canOwnMultiple + \"</b></td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInClothView.Contains("Skill1"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].skill1 + \"</b></td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInClothView.Contains("Skill2"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].skill2 + \"</b></td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInClothView.Contains("Skill3"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[BELT_NAME].skill3 + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Image"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[BELT_NAME].image+\"></td>\";");
                twCloth.WriteLine("\t\ts +=\"</tr>\";");

                // socks
                twCloth.WriteLine("\t\ts +=\"<tr>\";");
                if (_conf.displayInClothView.Contains("ID"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].ID + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Name"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].name + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Description"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].description + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].category + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Shop category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].shopCategory + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Body part"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].bodyPart + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Owned"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].owned + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Is worn"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].isWorn + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can buy"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].canBuy + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Buy price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].buyPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Sell price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].sellPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can own multiple"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].canOwnMultiple + \"</b></td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInClothView.Contains("Skill1"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].skill1 + \"</b></td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInClothView.Contains("Skill2"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].skill2 + \"</b></td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInClothView.Contains("Skill3"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SOCKS_NAME].skill3 + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Image"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[SOCKS_NAME].image+\"></td>\";");
                twCloth.WriteLine("\t\ts +=\"</tr>\";");

                // shoes
                twCloth.WriteLine("\t\ts +=\"<tr>\";");
                if (_conf.displayInClothView.Contains("ID"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].ID + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Name"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].name + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Description"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].description + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].category + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Shop category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].shopCategory + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Body part"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].bodyPart + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Owned"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].owned + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Is worn"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].isWorn + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can buy"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].canBuy + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Buy price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].buyPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Sell price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].sellPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can own multiple"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].canOwnMultiple + \"</b></td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInClothView.Contains("Skill1"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].skill1 + \"</b></td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInClothView.Contains("Skill2"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].skill2 + \"</b></td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInClothView.Contains("Skill3"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[SHOES_NAME].skill3 + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Image"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[SHOES_NAME].image+\"></td>\";");
                twCloth.WriteLine("\t\ts +=\"</tr>\";");

                // underwear bottom
                twCloth.WriteLine("\t\ts +=\"<tr>\";");
                if (_conf.displayInClothView.Contains("ID"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].ID + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Name"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].name + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Description"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].description + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].category + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Shop category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].shopCategory + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Body part"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].bodyPart + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Owned"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].owned + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Is worn"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].isWorn + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can buy"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].canBuy + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Buy price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].buyPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Sell price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].sellPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can own multiple"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].canOwnMultiple + \"</b></td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInClothView.Contains("Skill1"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].skill1 + \"</b></td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInClothView.Contains("Skill2"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].skill2 + \"</b></td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInClothView.Contains("Skill3"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].skill3 + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Image"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].image+\"></td>\";");
                twCloth.WriteLine("\t\ts +=\"</tr>\";");

                // underwear top
                twCloth.WriteLine("\t\ts +=\"<tr>\";");
                if (_conf.displayInClothView.Contains("ID"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].ID + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Name"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].name + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Description"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].description + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].category + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Shop category"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].shopCategory + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Body part"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].bodyPart + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Owned"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].owned + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Is worn"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].isWorn + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can buy"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].canBuy + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Buy price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].buyPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Sell price"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].sellPrice + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Can own multiple"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].canOwnMultiple + \"</b></td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInClothView.Contains("Skill1"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].skill1 + \"</b></td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInClothView.Contains("Skill2"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].skill2 + \"</b></td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInClothView.Contains("Skill3"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><b>\" + state.active.variables.wearing[UNDERWEAR_TOP_NAME].skill3 + \"</b></td>\";");
                if (_conf.displayInClothView.Contains("Image"))
                    twCloth.WriteLine("\t\ts +=\"<td class=\\\"cloth\\\"><img class=\\\"paragraph\\\" src=\"+state.active.variables.wearing[UNDERWEAR_TOP_NAME].image+\"></td>\";");
                twCloth.WriteLine("\t\ts +=\"</tr>\";");

                twCloth.WriteLine("\t\ts +=\"</table>\";");
                twCloth.WriteLine("\t\tnew Wikifier(place, s);");
                twCloth.WriteLine("\t}");
                twCloth.WriteLine("};");
                twCloth.WriteLine("");

                // clothSidebar
                twCloth.WriteLine("macros.clothSidebar = {");
                twCloth.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twCloth.WriteLine("");
                twCloth.WriteLine("\tnew Wikifier(place,");
                twCloth.WriteLine("\t\t\"<table class=\\\"cloth_sidebar\\\">\"+");
                twCloth.WriteLine("\t\t\"<tr><td colspan=2>Cloth</td></tr>\"+");
                twCloth.WriteLine("\t\t\"<tr><td class=\\\"cloth_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[HEAD_NAME].image+\"></td>\" +");
                twCloth.WriteLine("\t\t\"<td class=\\\"cloth_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[HAIR_NAME].image+\"></td></tr>\" +");
                twCloth.WriteLine("\t\t\"<tr><td class=\\\"cloth_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[NECK_NAME].image+\"></td>\" +");
                twCloth.WriteLine("\t\t\"<td class=\\\"cloth_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[UPPER_BODY_NAME].image+\"></td></tr>\" +");
                twCloth.WriteLine("\t\t\"<tr><td class=\\\"cloth_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[LOWER_BODY_NAME].image+\"></td>\" +");
                twCloth.WriteLine("\t\t\"<td class=\\\"cloth_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[BELT_NAME].image+\"></td></tr>\" +");
                twCloth.WriteLine("\t\t\"<tr><td class=\\\"cloth_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[SOCKS_NAME].image+\"></td>\"+");
                twCloth.WriteLine("\t\t\"<td class=\\\"cloth_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[SHOES_NAME].image+\"></td></tr>\"+");
                twCloth.WriteLine("\t\t\"<tr><td class=\\\"cloth_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[UNDERWEAR_BOTTOM_NAME].image+\"></td>\"+");
                twCloth.WriteLine("\t\t\"<td class=\\\"cloth_sidebar\\\"><img class=\\\"sidebar\\\" src=\"+state.active.variables.wearing[UNDERWEAR_TOP_NAME].image+\"></td></tr>\"+");
                twCloth.WriteLine("\t\t\"</table>\");");
                twCloth.WriteLine("\t}");
                twCloth.WriteLine("};");
                twCloth.WriteLine("");

                // wear
                twCloth.WriteLine("window.wear = function(_cloth) {");
                twCloth.WriteLine("\tvar cloth_obj = JSON.parse(unescape(_cloth));");
                twCloth.WriteLine("\tstate.active.variables.wearing[cloth_obj.bodyPart] = cloth_obj;");
                twCloth.WriteLine("\tstate.display(state.active.title, null, \"back\");");
                twCloth.WriteLine("};");
                twCloth.WriteLine("");

                // wardrobe
                twCloth.WriteLine("macros.wardrobe = {");
                twCloth.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twCloth.WriteLine("\t\tvar wstr = \"<table class=\\\"wardrobe\\\">\";");
                twCloth.WriteLine("\t\twstr +=\"<tr>\";");
                if (_conf.displayInWardrobe.Contains("ID")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_ID_CAP")).caption + "</b></td>\";");
                if (_conf.displayInWardrobe.Contains("Name")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_NAME_CAP")).caption + "</b></td>\";");
                if (_conf.displayInWardrobe.Contains("Description")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_DESCRIPTION_CAP")).caption + "</b></td>\";");
                if (_conf.displayInWardrobe.Contains("Category")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_CATEGORY_CAP")).caption + "</b></td>\";");
                if (_conf.displayInWardrobe.Contains("Shop category")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_SHOP_CATEGORY_CAP")).caption + "</b></td>\";");
                if (_conf.displayInWardrobe.Contains("Is worn")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_IS_WORN_CAP")).caption + "</b></td>\";");
                if (_conf.displayInWardrobe.Contains("Can buy")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_CAN_BUY_CAP")).caption + "</b></td>\";");
                if (_conf.displayInWardrobe.Contains("Buy price")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_BUY_PRICE_CAP")).caption + "</b></td>\";");
                if (_conf.displayInWardrobe.Contains("Sell price")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_SELL_PRICE_CAP")).caption + "</b></td>\";");
                if (_conf.displayInWardrobe.Contains("Can own multiple")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_CAN_OWN_MULTIPLE_CAP")).caption + "</b></td>\";");
                if (_conf.displayInWardrobe.Contains("Body part")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_BODY_PART_CAP")).caption +"</b></td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInWardrobe.Contains("Skill1")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_SKILL1_CAP")).caption + "</b></td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInWardrobe.Contains("Skill2")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_SKILL2_CAP")).caption + "</b></td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInWardrobe.Contains("Skill3")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_SKILL3_CAP")).caption + "</b></td>\";");
                if (_conf.displayInWardrobe.Contains("Owned")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_OWNED_CAP")).caption + "</b></td>\";");
                if (_conf.displayInWardrobe.Contains("Image")) twCloth.WriteLine("\t\twstr +=\"<td class=\\\"wardrobe\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_COL_IMAGE_CAP")).caption + "</b></td>\";");
                twCloth.WriteLine("\t\twstr +=\"<td></td>\";"); // wear / isworn
                twCloth.WriteLine("\t\twstr +=\"</tr>\";");
                twCloth.WriteLine("");
                twCloth.WriteLine("\t\tfor (var w = 0; w<state.active.variables.wardrobe.length; w++) {");
                twCloth.WriteLine("\t\t\twstr +=\"<tr>\";");
                if (_conf.displayInWardrobe.Contains("ID")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].ID + \"</td>\";");
                if (_conf.displayInWardrobe.Contains("Name")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].name + \"</td>\";");
                if (_conf.displayInWardrobe.Contains("Description")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].description + \"</td>\";");
                if (_conf.displayInWardrobe.Contains("Category")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].category + \"</td>\";");
                if (_conf.displayInWardrobe.Contains("Shop category")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].shopCategory + \"</td>\";");
                if (_conf.displayInWardrobe.Contains("Is worn")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].isWorn + \"</td>\";");
                if (_conf.displayInWardrobe.Contains("Can buy")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].canBuy + \"</td>\";");
                if (_conf.displayInWardrobe.Contains("Buy price")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].buyPrice + \"</td>\";");
                if (_conf.displayInWardrobe.Contains("Sell price")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].sellPrice + \"</td>\";");
                if (_conf.displayInWardrobe.Contains("Can own multiple")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].canOwnMultiple + \"</td>\";");
                if (_conf.displayInWardrobe.Contains("Body part")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].bodyPart + \"</td>\";");
                if (_conf.clothUseSkill1 && _conf.displayInWardrobe.Contains("Skill1")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].skill1 + \"</td>\";");
                if (_conf.clothUseSkill2 && _conf.displayInWardrobe.Contains("Skill2")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].skill2 + \"</td>\";");
                if (_conf.clothUseSkill3 && _conf.displayInWardrobe.Contains("Skill3")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].skill3 + \"</td>\";");
                if (_conf.displayInWardrobe.Contains("Owned")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\">\" + state.active.variables.wardrobe[w].owned + \"</td>\";");
                if (_conf.displayInWardrobe.Contains("Image")) twCloth.WriteLine("\t\t\twstr +=\"<td class=\\\"wardrobe\\\"><img class=\\\"paragraph\\\" src=\\\"\" + state.active.variables.wardrobe[w].image + \"\\\"></td>\";");
                twCloth.WriteLine("");
                twCloth.WriteLine("\t\t\tif (state.active.variables.wearing[state.active.variables.wardrobe[w].bodyPart].ID != state.active.variables.wardrobe[w].ID) {");
                twCloth.WriteLine("\t\t\t\twstr +=\"<td class=\\\"wardrobe\\\"><a onClick=\\\"wear('\"+escape(JSON.stringify(state.active.variables.wardrobe[w]))+\"');\\\" href=\\\"javascript:void(0);\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_WEAR_CAP")).caption +"</a></td></tr>\";");
                twCloth.WriteLine("\t\t\t} else {");
                twCloth.WriteLine("\t\t\t\twstr +=\"<td class=\\\"wardrobe\\\">" + _conf.captions.Single(s => s.captionName.Equals("CLOTH_IS_WORN_CAP")).caption +"</td></tr>\";");
                twCloth.WriteLine("\t\t\t}");
                twCloth.WriteLine("\t\t}");
                twCloth.WriteLine("\twstr +=\"</table>\";");
                twCloth.WriteLine("");
                twCloth.WriteLine("\tnew Wikifier(place,wstr);");
                twCloth.WriteLine("\t}");
                twCloth.WriteLine("};");
                twCloth.WriteLine("");

                // addToWardrobe
                twCloth.WriteLine("macros.addToWardrobe = {");
                twCloth.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twCloth.WriteLine("\t\tif (params.length != 2) {");
                twCloth.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two parameters, cloth ID and amount.\");");
                twCloth.WriteLine("\t\t\treturn;");
                twCloth.WriteLine("\t\t}");
                twCloth.WriteLine("");
                twCloth.WriteLine("\t\tif ((!Number.isInteger(params[0])) || (!Number.isInteger(params[1]))) {");
                twCloth.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two integer parameters.\");");
                twCloth.WriteLine("\t\t\treturn;");
                twCloth.WriteLine("\t\t}");
                twCloth.WriteLine("");
                twCloth.WriteLine("\t\tvar cloth_in_catalog = state.active.variables.all_cloth.filter(obj => {return obj.ID === params[0]});");
                twCloth.WriteLine("");
                twCloth.WriteLine("\t\tif (cloth_in_catalog.length == 0) {");
                twCloth.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Cloth with id \" + params[0] + \" does not exist.\");");
                twCloth.WriteLine("\t\t\treturn;");
                twCloth.WriteLine("\t\t}");
                twCloth.WriteLine("");
                twCloth.WriteLine("\t\t// Clone cloth");
                twCloth.WriteLine("\t\tvar new_cloth = JSON.parse(JSON.stringify(cloth_in_catalog[0]));");
                twCloth.WriteLine("\t\tnew_cloth.owned = params[1];");
                twCloth.WriteLine("");
                twCloth.WriteLine("\t\t// cloth not yet existent?");
                twCloth.WriteLine("\t\tvar existing_cloth_in_wardrobe_with_id = state.active.variables.wardrobe.filter(obj => {return obj.ID === params[0]});");
                twCloth.WriteLine("");
                twCloth.WriteLine("\t\tif (existing_cloth_in_wardrobe_with_id.length == 0) {");
                twCloth.WriteLine("\t\t\t// add new cloth");
                twCloth.WriteLine("\t\t\tstate.active.variables.wardrobe.push(new_cloth);");
                twCloth.WriteLine("\t\t} else if (existing_cloth_in_wardrobe_with_id.length > 0) {");
                twCloth.WriteLine("\t\t\t// change owned");
                twCloth.WriteLine("\t\t\tfor (var i in state.active.variables.wardrobe) {");
                twCloth.WriteLine("\t\t\t\tif (state.active.variables.wardrobe[i].ID == new_cloth.ID) {");
                twCloth.WriteLine("\t\t\t\t\tstate.active.variables.wardrobe[i].owned += new_cloth.owned;");
                twCloth.WriteLine("\t\t\t\t\tbreak;");
                twCloth.WriteLine("\t\t\t\t}");
                twCloth.WriteLine("\t\t\t}");
                twCloth.WriteLine("\t\t} else {");
                twCloth.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: There are several cloth with the same id \" + new_cloth.ID);");
                twCloth.WriteLine("\t\t\treturn;");
                twCloth.WriteLine("\t\t}");
                twCloth.WriteLine("\t}");
                twCloth.WriteLine("};");
            }
            finally
            {
                if (twCloth != null)
                {
                    twCloth.Flush();
                    twCloth.Close();
                }
            }
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

                // initStats
                twStats.WriteLine("macros.initStats = {");
                twStats.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twStats.WriteLine("\t\tstate.active.variables.stats = [];");

                for(int i=0; i<_conf.stats.Count; i++)
                {
                    twStats.Write("\t\tstate.active.variables.stats.push({");
                    twStats.Write("\"ID\":" + _conf.stats[i].ID + ",");
                    twStats.Write("\"name\":\"" + _conf.stats[i].name + "\",");
                    twStats.Write("\"value\":" + _conf.stats[i].value + ",");
                    twStats.Write("\"description\":\"" + _conf.stats[i].description + "\",");
                    twStats.WriteLine("\"image\":\"" + pathSubtract(_conf.stats[i].img, _conf.pathSubtract) + "\"});");
                }
                twStats.WriteLine("\t}");
                twStats.WriteLine("};");
                twStats.WriteLine("");

                // setStats
                twStats.WriteLine("macros.setStats = {");
                twStats.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twStats.WriteLine("\t\tif (params.length != 2) {");
                twStats.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two parameters, stat id and value.\");");
                twStats.WriteLine("\t\t\treturn;");
                twStats.WriteLine("\t\t}");
                twStats.WriteLine("\t\tfor (var i in state.active.variables.stats)");
                twStats.WriteLine("\t\t{");
                twStats.WriteLine("\t\t\tif (state.active.variables.stats[i].ID == params[0]) {");
                twStats.WriteLine("\t\t\t\tstate.active.variables.stats[i].value = params[1];");
                twStats.WriteLine("\t\t\t\tbreak;");
                twStats.WriteLine("\t\t\t}");
                twStats.WriteLine("\t\t}");
                twStats.WriteLine("\t}");
                twStats.WriteLine("};");
                twStats.WriteLine("");

                // getStats
                twStats.WriteLine("macros.getStats = {");
                twStats.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twStats.WriteLine("\t\tif (params.length != 1) {");
                twStats.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \" >>: expects stat id.\");");
                twStats.WriteLine("\t\t\treturn;");
                twStats.WriteLine("\t\t}");
                twStats.WriteLine("\t\tfor (var i in state.active.variables.stats)");
                twStats.WriteLine("\t\t{");
                twStats.WriteLine("\t\t\tif (state.active.variables.stats[i].ID == params[0]) {");
                twStats.WriteLine("\t\t\t\treturn state.active.variables.stats[i].value;");
                twStats.WriteLine("\t\t\t}");
                twStats.WriteLine("\t\t}");
                twStats.WriteLine("\t}");
                twStats.WriteLine("};");
                twStats.WriteLine("");

                // addStats
                twStats.WriteLine("macros.addStats = {");
                twStats.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twStats.WriteLine("\t\tif (params.length != 2) {");
                twStats.WriteLine("\t\t\tthrowError(place, \" << \" + macroName + \" >>: expects two parameters, stat id and value.\");");
                twStats.WriteLine("\t\t\treturn;");
                twStats.WriteLine("\t\t}");
                twStats.WriteLine("\t\tfor (var i in state.active.variables.stats)");
                twStats.WriteLine("\t\t{");
                twStats.WriteLine("\t\t\tif (state.active.variables.stats[i].ID == params[0]) {");
                twStats.WriteLine("\t\t\t\tstate.active.variables.stats[i].value += params[1];");            
                twStats.WriteLine("\t\t\t\tbreak;");
                twStats.WriteLine("\t\t\t}");
                twStats.WriteLine("\t\t}");
                twStats.WriteLine("\t}");
                twStats.WriteLine("};");
                twStats.WriteLine("");

                // stats
                twStats.WriteLine("macros.stats = {");
                twStats.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twStats.WriteLine("\t\tif (state.active.variables.stats.length == 0)");
                twStats.WriteLine("\t\t{");
                twStats.WriteLine("\t\t\tnew Wikifier(place, 'No stats');");
                twStats.WriteLine("\t\t}");
                twStats.WriteLine("\t\telse");
                twStats.WriteLine("\t\t{");
                twStats.WriteLine("\t\t\tvar stats_str = \"<table class=\\\"stats\\\"><tr>\";");
                twStats.WriteLine("\t\t\tstats_str += \"<td class=\\\"stats\\\"><b>ID</b></td>\";");
                twStats.WriteLine("\t\t\tstats_str += \"<td class=\\\"stats\\\"><b>name</b></td>\";");
                twStats.WriteLine("\t\t\tstats_str += \"<td class=\\\"stats\\\"><b>value</b></td>\";");
                twStats.WriteLine("\t\t\tstats_str += \"<td class=\\\"stats\\\"><b>description</b></td>\";");
                twStats.WriteLine("\t\t\tstats_str += \"<td class=\\\"stats\\\"><b>image</b></td>\";");
                twStats.WriteLine("\t\t\tstats_str += \"</tr>\";");
                twStats.WriteLine("\t\t\tfor (var i = 0; i < state.active.variables.stats.length; i++)");
                twStats.WriteLine("\t\t\t{");
                twStats.WriteLine("\t\t\t\tstats_str += \"<tr>\";");
                twStats.WriteLine("\t\t\t\tstats_str += \"<td class=\\\"stats\\\">\" + state.active.variables.stats[i].ID + \"</td>\";");
                twStats.WriteLine("\t\t\t\tstats_str += \"<td class=\\\"stats\\\">\" + state.active.variables.stats[i].name + \"</td>\";");
                twStats.WriteLine("\t\t\t\tstats_str += \"<td class=\\\"stats\\\">\" + state.active.variables.stats[i].value + \"</td>\";");
                twStats.WriteLine("\t\t\t\tstats_str += \"<td class=\\\"stats\\\">\" + state.active.variables.stats[i].description + \"</td>\";");
                twStats.WriteLine("\t\t\t\tstats_str += \"<td class=\\\"stats\\\"><img class=\\\"paragraph\\\" src=\\\"\" + state.active.variables.stats[i].image + \"\\\"></td>\";");
                twStats.WriteLine("\t\t\t\tstats_str += \"</tr>\";");
                twStats.WriteLine("\t\t\t}");
                twStats.WriteLine("\t\t\tstats_str += \"</table>\";");
                twStats.WriteLine("\t\t\tnew Wikifier(place, stats_str);");
                twStats.WriteLine("\t\t}");
                twStats.WriteLine("\t}");
                twStats.WriteLine("};");
                twStats.WriteLine("");

                // statsSidebar
                twStats.WriteLine("macros.statsSidebar = {");
                twStats.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twStats.WriteLine("\t\tif (state.active.variables.stats.length == 0)");
                twStats.WriteLine("\t\t{");
                twStats.WriteLine("\t\t\tnew Wikifier(place, 'No stats');");
                twStats.WriteLine("\t\t} else {");
                twStats.WriteLine("\t\t\tvar stats_str = \"<table class=\\\"stats_sidebar\\\">\";");
                twStats.WriteLine("\t\t\tfor (var i = 0; i < state.active.variables.stats.length; i++)");
                twStats.WriteLine("\t\t\t{");
                twStats.WriteLine("\t\t\t\tstats_str += \"<tr><td class=\\\"stats_sidebar\\\">\" + state.active.variables.stats[i].name + \"</td>\" +");
                twStats.WriteLine("\t\t\t\t\"<td class=\\\"stats\\\">\" + state.active.variables.stats[i].value + \"</td></tr>\";");
                twStats.WriteLine("\t\t\t}");
                twStats.WriteLine("\t\t\tstats_str += \"</table>\";");
                twStats.WriteLine("\t\t\tnew Wikifier(place, stats_str);");
                twStats.WriteLine("\t\t}");
                twStats.WriteLine("\t}");
                twStats.WriteLine("};");
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

        private static void generateNavigation(Configuration _conf, string _path)
        {
            string navigationPath = Path.Combine(_path, "_js_navigation.tw2");
            TextWriter twNavigation = null;
            try
            {
                twNavigation = new StreamWriter(navigationPath, false, new UTF8Encoding(false));
                twNavigation.WriteLine("::Navigation[script]");
                twNavigation.WriteLine("");
                if (!_conf.navigationArrows) twNavigation.WriteLine("Config.history.controls = false;");
                if (_conf.debugMode) twNavigation.WriteLine("Config.debug = true;");
                twNavigation.WriteLine("Config.saves.slots = " + _conf.saveSlots);
                twNavigation.WriteLine("");

                twNavigation.WriteLine("predisplay[\"Menu Return\"] = function (taskName) {");
                twNavigation.WriteLine("\tif (! tags().contains(\"noreturn\")) {");
                twNavigation.WriteLine("\t\tState.variables.return = passage();");
                twNavigation.WriteLine("\t}");
                twNavigation.WriteLine("};");
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

        private static void generateDaytime(Configuration _conf, string _path)
        {
            string daytimePath = Path.Combine(_path, "_js_daytime.tw2");
            TextWriter twDaytime = null;
            try
            {
                twDaytime = new StreamWriter(daytimePath, false, new UTF8Encoding(false));
                twDaytime.WriteLine("::Daytime[script]");
                twDaytime.WriteLine("");

                // formatDate
                twDaytime.WriteLine("function formatDate(date) {");
                twDaytime.WriteLine("\tvar monthNames = [");
                twDaytime.WriteLine("\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_JANUARY_CAP")).caption + "\", ");
                twDaytime.WriteLine("\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_FEBRUARY_CAP")).caption + "\", ");
                twDaytime.WriteLine("\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_MARCH_CAP")).caption + "\",");
                twDaytime.WriteLine("\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_APRIL_CAP")).caption + "\", ");
                twDaytime.WriteLine("\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_MAY_CAP")).caption + "\", ");
                twDaytime.WriteLine("\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_JUNE_CAP")).caption + "\", ");
                twDaytime.WriteLine("\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_JULY_CAP")).caption + "\",");
                twDaytime.WriteLine("\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_AUGUST_CAP")).caption + "\", ");
                twDaytime.WriteLine("\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_SEPTEMBER_CAP")).caption + "\", ");
                twDaytime.WriteLine("\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_OCTOBER_CAP")).caption + "\",");
                twDaytime.WriteLine("\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_NOVEMBER_CAP")).caption + "\", ");
                twDaytime.WriteLine("\t\t\"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_DECEMBER_CAP")).caption + "\"");
                twDaytime.WriteLine("\t];");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\tvar day = date.getDate();");
                twDaytime.WriteLine("\tvar monthIndex = date.getMonth();");
                twDaytime.WriteLine("\tvar year = date.getFullYear();");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\treturn day + ' ' + monthNames[monthIndex] + ' ' + year;");
                twDaytime.WriteLine("}");
                twDaytime.WriteLine("");

                // initDaytime
                twDaytime.WriteLine("macros.initDaytime = {");
                twDaytime.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twDaytime.WriteLine("\t\tif (state.active.variables.time === undefined) {");
                twDaytime.WriteLine("\t\t\tstate.active.variables.time = new Date(" + _conf.startDate.Year +"," + _conf.startDate.Month + "," +
                    _conf.startDate.Day + "," + _conf.startDate.Hour + "," + _conf.startDate.Minute + "," + _conf.startDate.Second +");");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("\t}");
                twDaytime.WriteLine("};");
                twDaytime.WriteLine("");

                // getTime
                twDaytime.WriteLine("macros.getTime = {");
                twDaytime.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tif (state.active.variables.time === undefined) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tnew Wikifier(place, (\"0\" + state.active.variables.time.getHours()).slice(-2) + \":\" + (\"0\" + state.active.variables.time.getMinutes()).slice(-2) + \":\" + (\"0\" + state.active.variables.time.getSeconds()).slice(-2));");
                twDaytime.WriteLine("\t}");
                twDaytime.WriteLine("};");
                twDaytime.WriteLine("");

                // getDate
                twDaytime.WriteLine("macros.getDate = {");
                twDaytime.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twDaytime.WriteLine("\t\tif (state.active.variables.time === undefined) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("\t\tnew Wikifier(place, formatDate(state.active.variables.time));");
                twDaytime.WriteLine("\t}");
                twDaytime.WriteLine("};");
                twDaytime.WriteLine("");

                // getDateTime
                twDaytime.WriteLine("macros.getDateTime = {");
                twDaytime.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twDaytime.WriteLine("\t\tif (state.active.variables.time === undefined) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tnew Wikifier(place, formatDate(state.active.variables.time) + \" \" + (\"0\" + state.active.variables.time.getHours()).slice(-2) +");
                twDaytime.WriteLine("\t\t\t\":\" + (\"0\" + state.active.variables.time.getMinutes()).slice(-2) + \":\" + (\"0\" + state.active.variables.time.getSeconds()).slice(-2));");
                twDaytime.WriteLine("\t}");
                twDaytime.WriteLine("};");
                twDaytime.WriteLine("");

                // getTimeOfDay
                twDaytime.WriteLine("macros.getTimeOfDay = {");
                twDaytime.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twDaytime.WriteLine("\t\tif (state.active.variables.time === undefined) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tif((state.active.variables.time.getHours() >= 1) && (state.active.variables.time < 4)) {");
                twDaytime.WriteLine("\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_EARLY_MORNING_CAP")).caption + "\");");
                twDaytime.WriteLine("\t\t} else if ((state.active.variables.time.getHours() >= 4) && (state.active.variables.time.getHours() < 6)) {");
                twDaytime.WriteLine("\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_DAWN_CAP")).caption + "\");");
                twDaytime.WriteLine("\t\t} else if ((state.active.variables.time.getHours() >= 6) && (state.active.variables.time.getHours() < 11)) {");
                twDaytime.WriteLine("\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_MORNING_CAP")).caption + "\");");
                twDaytime.WriteLine("\t\t} else if ((state.active.variables.time.getHours() >= 11) && (state.active.variables.time.getHours() < 13)) {");
                twDaytime.WriteLine("\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_NOON_CAP")).caption + "\");");
                twDaytime.WriteLine("\t\t} else if ((state.active.variables.time.getHours() >= 13) && (state.active.variables.time.getHours() < 16)) {");
                twDaytime.WriteLine("\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_AFTERNOON_CAP")).caption + "\");");
                twDaytime.WriteLine("\t\t} else if ((state.active.variables.time.getHours() >= 16) && (state.active.variables.time.getHours() < 21)) {");
                twDaytime.WriteLine("\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_EVENING_CAP")).caption + "\");");
                twDaytime.WriteLine("\t\t} else if ((state.active.variables.time.getHours() >= 21) && (state.active.variables.time.getHours() < 24)) {");
                twDaytime.WriteLine("\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_NIGHT_CAP")).caption + "\");");
                twDaytime.WriteLine("\t\t} else if ((state.active.variables.time.getHours() >= 0) && (state.active.variables.time.getHours() < 1)) {");
                twDaytime.WriteLine("\t\t\tnew Wikifier(place, \"" + _conf.captions.Single(s => s.captionName.Equals("DAYTIME_MID_NIGHT_CAP")).caption + "\");");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("\t}");
                twDaytime.WriteLine("};");
                twDaytime.WriteLine("");

                // setTime
                twDaytime.WriteLine("macros.setTime = {");
                twDaytime.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tif (state.active.variables.time === undefined) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tif (params.length != 3) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Expecting three parameters, hours, minutes and seconds.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tvar hours = params[0];");
                twDaytime.WriteLine("\t\tvar minutes = params[1];");
                twDaytime.WriteLine("\t\tvar seconds = params[2];");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setHours(hours);");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setMinutes(minutes);");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setSeconds(seconds);");
                twDaytime.WriteLine("\t}");
                twDaytime.WriteLine("};");
                twDaytime.WriteLine("");

                // setDate
                twDaytime.WriteLine("macros.setDate = {");
                twDaytime.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tif (state.active.variables.time === undefined) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tif (params.length != 3) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Expecting three parameters: year, month and days.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tvar year = params[0];");
                twDaytime.WriteLine("\t\tvar month = params[1];");
                twDaytime.WriteLine("\t\tvar day = params[2];");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setYear(year);");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setMonth(month);");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setDay(day);");
                twDaytime.WriteLine("\t}");
                twDaytime.WriteLine("};");
                twDaytime.WriteLine("");

                // setDateTime
                twDaytime.WriteLine("macros.setDateTime = {");
                twDaytime.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tif (state.active.variables.time === undefined) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tif (params.length != 6) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Expecting six parameters: year, month, days, hours, minutes and seconds.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tvar year = params[0];");
                twDaytime.WriteLine("\t\tvar month = params[1];");
                twDaytime.WriteLine("\t\tvar day = params[2];");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tvar hours = params[3];");
                twDaytime.WriteLine("\t\tvar minutes = params[4];");
                twDaytime.WriteLine("\t\tvar seconds = params[5];");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setHours(hours);");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setMinutes(minutes);");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setSeconds(seconds);");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setHours(hours);");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setMinutes(minutes);");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setSeconds(seconds);");
                twDaytime.WriteLine("\t}");
                twDaytime.WriteLine("};");
                twDaytime.WriteLine("");

                // addTimeInMinutes
                twDaytime.WriteLine("macros.addTimeInMinutes = {");
                twDaytime.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tif (state.active.variables.time === undefined) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tif (params.length != 1) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Expecting minutes as first parameter.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tvar minutes = params[0];");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setMinutes(state.active.variables.time.getMinutes() + minutes);");
                twDaytime.WriteLine("\t}");
                twDaytime.WriteLine("};");
                twDaytime.WriteLine("");

                // addTimeInDays
                twDaytime.WriteLine("macros.addTimeInDays = {");
                twDaytime.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tif (state.active.variables.time === undefined) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Please call initDaytime first.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tif (params.length != 1) {");
                twDaytime.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: Expecting days as first parameter.\");");
                twDaytime.WriteLine("\t\t\treturn;");
                twDaytime.WriteLine("\t\t}");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tvar days = params[0];");
                twDaytime.WriteLine("");
                twDaytime.WriteLine("\t\tstate.active.variables.time.setDays(state.active.variables.time.setDays() + 30);");
                twDaytime.WriteLine("\t}");
                twDaytime.WriteLine("};");
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


        private static void generateShops(Configuration _conf, string _path)
        {
            string shopsPath = Path.Combine(_path, "_js_shops.tw2");
            TextWriter twShops = null;
            try
            {
                twShops = new StreamWriter(shopsPath, false, new UTF8Encoding(false));
                twShops.WriteLine("::Shops[script]");
                twShops.WriteLine("");

                // initShops
                twShops.WriteLine("macros.initShops = {");
                twShops.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twShops.WriteLine("");
                twShops.WriteLine("\t\tif (state.active.variables.shops === undefined) {");
                twShops.WriteLine("\t\t\tstate.active.variables.shops = [];");

                for(int i=0; i<_conf.shops.Count; i++)
                {
                    twShops.Write("\t\tstate.active.variables.shops.push({");
                    twShops.Write("\"ID\":" + _conf.shops[i].ID + ",");
                    twShops.Write("\"name\":\"" + _conf.shops[i].name + "\",");
                    twShops.Write("\"open\":\"" + _conf.shops[i].opening.Hour + ":" + _conf.shops[i].opening.Minute + ":" + _conf.shops[i].opening.Second + "\",");
                    twShops.Write("\"close\":\"" + _conf.shops[i].closing.Hour + ":" + _conf.shops[i].closing.Minute + ":" + _conf.shops[i].closing.Second + "\",");
                    twShops.WriteLine("\"items\":[");

                    for(int j=0; j<_conf.shops[i].items.Count; j++)
                    {
                        twShops.Write("\t\t\t{\"type\":\"" + _conf.shops[i].items[j].type + "\",");
                        twShops.Write("\"ID\":" + _conf.shops[i].items[j].id + ",");
                        twShops.Write("\"quantity\":" + _conf.shops[i].items[j].quantityStart + ",");
                        twShops.Write("\"quantityMax\":" + _conf.shops[i].items[j].quantityMax + ",");
                        twShops.Write("\"refillDelay\":" + _conf.shops[i].items[j].refillDelay + ",");
                        
                        if (j<_conf.shops[i].items.Count-1)
                        {
                            twShops.WriteLine("\"lastRefill\":new Date(-8640000000000000)},");
                        } else
                        {
                            twShops.WriteLine("\"lastRefill\":new Date(-8640000000000000)}");
                        }
                    }

                    twShops.WriteLine("\t\t]});");
                }

                twShops.WriteLine("\t\t}");
                twShops.WriteLine("\t}");
                twShops.WriteLine("};");
                twShops.WriteLine("");

                // buy
                twShops.WriteLine("\twindow.buy = function (_item, _type, _shopId, _itemIndex) {");
                twShops.WriteLine("\tvar item_obj = JSON.parse(unescape(_item));");
                twShops.WriteLine("");
                twShops.WriteLine("\tif ((state.active.variables.money >= item_obj.buyPrice) && (state.active.variables.shops[_shopId].items[_itemIndex].quantity > 0)) {");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t/*");
                twShops.WriteLine("\t\t* Buy an item");
                twShops.WriteLine("\t\t*/");
                twShops.WriteLine("\t\tif (_type.toUpperCase() === \"ITEM\") {");
                twShops.WriteLine("\t\t\t// item not yet existent?");
                twShops.WriteLine("\t\t\tvar existing_items_with_id = state.active.variables.inventory.filter(obj => {");
                twShops.WriteLine("\t\t\t\treturn obj.ID === item_obj.ID");
                twShops.WriteLine("\t\t\t});");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\tif (existing_items_with_id.length == 0) {");
                twShops.WriteLine("\t\t\t\t// add new item");
                twShops.WriteLine("\t\t\t\tstate.active.variables.inventory.push(item_obj);");
                twShops.WriteLine("\t\t\t\tstate.active.variables.money -=item_obj.buyPrice;");
                twShops.WriteLine("\t\t\t\tstate.active.variables.shops[_shopId].items[_itemIndex].quantity -=1;");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t} else if (existing_items_with_id.length > 0) {");
                twShops.WriteLine("\t\t\t\t// change amount");
                twShops.WriteLine("\t\t\t\tfor (var i in state.active.variables.inventory) {");
                twShops.WriteLine("\t\t\t\t\tif (state.active.variables.inventory[i].ID == item_obj.ID) {");
                twShops.WriteLine("\t\t\t\t\t\tstate.active.variables.inventory[i].owned +=1;");
                twShops.WriteLine("\t\t\t\t\t\tstate.active.variables.money -=item_obj.buyPrice;");
                twShops.WriteLine("\t\t\t\t\t\tstate.active.variables.shops[_shopId].items[_itemIndex].quantity -=1;");
                twShops.WriteLine("\t\t\t\t\t\tbreak;");
                twShops.WriteLine("\t\t\t\t\t}");
                twShops.WriteLine("\t\t\t\t}");
                twShops.WriteLine("\t\t\t} else {");
                twShops.WriteLine("\t\t\t\tthrowError(null, \"<<\" + macroName + \">>: There are several items with the same id \" + item_obj.ID);");
                twShops.WriteLine("\t\t\t\treturn;");
                twShops.WriteLine("\t\t\t}");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t/*");
                twShops.WriteLine("\t\t* Buy cloth");
                twShops.WriteLine("\t\t*/");
                twShops.WriteLine("\t\t} else if (_type.toUpperCase() === \"CLOTH\") {");
                twShops.WriteLine("\t\t\t// cloth not yet existent?");
                twShops.WriteLine("\t\t\tvar existing_cloths_with_id = state.active.variables.wardrobe.filter(obj => {");
                twShops.WriteLine("\t\t\t\treturn obj.ID === item_obj.ID");
                twShops.WriteLine("\t\t\t});");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\tif (existing_cloths_with_id.length == 0) {");
                twShops.WriteLine("\t\t\t\t// add new cloth");
                twShops.WriteLine("\t\t\t\tstate.active.variables.wardrobe.push(item_obj);");
                twShops.WriteLine("\t\t\t\tstate.active.variables.money -=item_obj.buyPrice;");
                twShops.WriteLine("\t\t\t} else if (existing_cloths_with_id.length > 0) {");
                twShops.WriteLine("\t\t\t\t// change amount");
                twShops.WriteLine("\t\t\t\tfor (var i in state.active.variables.wardrobe) {");
                twShops.WriteLine("\t\t\t\t\tif (state.active.variables.wardrobe[i].ID == item_obj.ID) {");
                twShops.WriteLine("\t\t\t\t\t\tstate.active.variables.wardrobe[i].owned +=1;");
                twShops.WriteLine("\t\t\t\t\t\tstate.active.variables.money -=item_obj.buyPrice;");
                twShops.WriteLine("\t\t\t\t\t\tstate.active.variables.shops[_shopId].items[_itemIndex].quantity -=1;");
                twShops.WriteLine("\t\t\t\t\t\tbreak;");
                twShops.WriteLine("\t\t\t\t\t}");
                twShops.WriteLine("\t\t\t\t}");
                twShops.WriteLine("\t\t\t} else {");
                twShops.WriteLine("\t\t\t\tthrowError(null, \"<<\" + macroName + \">>: There are several cloth with the same id \" + item_obj.ID);");
                twShops.WriteLine("\t\t\t\treturn;");
                twShops.WriteLine("\t\t\t}");
                twShops.WriteLine("\t\t} else {");
                twShops.WriteLine("\t\t\tthrowError(null, \"buy: Unknown item type \" + item_obj.type);");
                twShops.WriteLine("\t\t\treturn;");
                twShops.WriteLine("\t\t}");
                twShops.WriteLine("");
                twShops.WriteLine("\t}");
                twShops.WriteLine("");
                twShops.WriteLine("\t// Refresh the page.");
                twShops.WriteLine("\tstate.display(state.active.title, null, \"back\");");
                twShops.WriteLine("};");
                twShops.WriteLine("");
                
                // sell
                twShops.WriteLine("window.sell = function (_item, _type, _shopId, _itemIndex) {");
                twShops.WriteLine("\tvar item_obj = JSON.parse(unescape(_item));");
                twShops.WriteLine("");
                twShops.WriteLine("\t/*");
                twShops.WriteLine("\t* Sell item");
                twShops.WriteLine("\t*/");
                twShops.WriteLine("\tif (_type.toUpperCase() === \"ITEM\") {");
                twShops.WriteLine("");
                twShops.WriteLine("\t\tvar existing_items_with_id = state.active.variables.inventory.filter(obj => {");
                twShops.WriteLine("\t\t\treturn obj.ID === item_obj.ID");
                twShops.WriteLine("\t\t});");
                twShops.WriteLine("\t\tif (existing_items_with_id.length == 0)");
                twShops.WriteLine("\t\t\treturn;");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t// Delete a special amount from inventory");
                twShops.WriteLine("\t\t\tfor (var i in state.active.variables.inventory) {");
                twShops.WriteLine("\t\t\t\tif (state.active.variables.inventory[i].ID == item_obj.ID) {");
                twShops.WriteLine("\t\t\t\t\tstate.active.variables.inventory[i].owned -= 1;");
                twShops.WriteLine("\t\t\t\t\tstate.active.variables.money +=item_obj.sellPrice;");
                twShops.WriteLine("\t\t\t\t\tstate.active.variables.shops[_shopId].items[_itemIndex].quantity +=1;");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\t\t// Delete item completely if owned <= 0");
                twShops.WriteLine("\t\t\t\t\tif (state.active.variables.inventory[i].owned <= 0) {");
                twShops.WriteLine("\t\t\t\t\t\tstate.active.variables.inventory.splice(i, 1);");
                twShops.WriteLine("\t\t\t\t\t}");
                twShops.WriteLine("\t\t\t\t\tbreak;");
                twShops.WriteLine("\t\t\t\t}");
                twShops.WriteLine("\t\t\t}");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t/*");
                twShops.WriteLine("\t\t\t* Sell cloth");
                twShops.WriteLine("\t\t\t*/");
                twShops.WriteLine("\t\t\t} else if (_type.toUpperCase() === \"CLOTH\") {");
                twShops.WriteLine("\t\t\t\tvar existing_items_with_id = state.active.variables.wardrobe.filter(obj => {");
                twShops.WriteLine("\t\t\t\t\treturn obj.ID === item_obj.ID");
                twShops.WriteLine("\t\t\t});");
                twShops.WriteLine("\t\t\tif (existing_items_with_id.length == 0)");
                twShops.WriteLine("\t\t\t\treturn;");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\t// TODO Check if wearing");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\t// Delete a special amount from cloth");
                twShops.WriteLine("\t\t\t\tfor (var i in state.active.variables.wardrobe) {");
                twShops.WriteLine("\t\t\t\tif (state.active.variables.wardrobe[i].ID == item_obj.ID) {");
                twShops.WriteLine("\t\t\t\t\tstate.active.variables.wardrobe[i].owned -= 1;");
                twShops.WriteLine("\t\t\t\t\tstate.active.variables.money +=item_obj.sellPrice;");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\t\t// Delete item completely if owned <= 0");
                twShops.WriteLine("\t\t\t\t\tif (state.active.variables.wardrobe[i].owned <= 0) {");
                twShops.WriteLine("\t\t\t\t\t\tstate.active.variables.wardrobe.splice(i, 1);");
                twShops.WriteLine("\t\t\t\t\t}");
                twShops.WriteLine("\t\t\t\t\tbreak;");
                twShops.WriteLine("\t\t\t\t}");
                twShops.WriteLine("\t\t\t}");
                twShops.WriteLine("\t\t}");
                twShops.WriteLine("");
                twShops.WriteLine("\t// Refresh page");
                twShops.WriteLine("\tstate.display(state.active.title, null, \"back\");");
                twShops.WriteLine("};");
                
                // shop
                twShops.WriteLine("macros.shop = {");
                twShops.WriteLine("\thandler: function (place, macroName, params, parser) {");
                twShops.WriteLine("");
                twShops.WriteLine("\t\tif (params.length != 1) {");
                twShops.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects shop id as parameter\");");
                twShops.WriteLine("\t\t\treturn;");
                twShops.WriteLine("\t\t}");
                twShops.WriteLine("");
                twShops.WriteLine("\t\tif (state.active.variables.shops[params[0]] === undefined) {");
                twShops.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: There is no shop with the id \" + params[0]);");
                twShops.WriteLine("\t\t\treturn;");
                twShops.WriteLine("\t\t}");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t// Is open?");
                twShops.WriteLine("\t\tif ((state.active.variables.time !== undefined) && (state.active.variables.time <= state.active.variables.shops[params[0]].open) && (state.active.variables.time >= state.active.variables.shops[params[0]].close)) {");
                twShops.WriteLine("\t\t\tnew Wikifier(place, \"Shop is closed.\");");
                twShops.WriteLine("\t\t\treturn;");
                twShops.WriteLine("\t\t}");
                twShops.WriteLine("");
                twShops.WriteLine("\t\tif (state.active.variables.shops[params[0]].items.length == 0) {");
                twShops.WriteLine("\t\t\tnew Wikifier(place, '" + _conf.captions.Single(s => s.captionName.Equals("SHOP_NO_ITEMS_CAP")).caption + "');");
                twShops.WriteLine("\t\t} else {");
                twShops.WriteLine("\t\t\tvar shop_str = \"<table class=\\\"shop\\\"><tr>\";");
                if (_conf.itemPropertiesInShops.Contains("ID")) twShops.WriteLine("\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_ID_CAP")).caption + "</b></td>\";");
                if (_conf.itemPropertiesInShops.Contains("Name")) twShops.WriteLine("\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_NAME_CAP")).caption + "</b></td>\";");
                if (_conf.itemPropertiesInShops.Contains("Quantity")) twShops.WriteLine("\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_QUANTITY_CAP")).caption + "</b></td>\";");
                if (_conf.itemPropertiesInShops.Contains("Buy")) twShops.WriteLine("\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_BUY_CAP")).caption + "</b></td>\";");
                if (_conf.itemPropertiesInShops.Contains("Sell")) twShops.WriteLine("\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_SELL_CAP")).caption + "</b></td>\";");
                if (_conf.itemPropertiesInShops.Contains("Skill1") && _conf.shopUseSkill1) twShops.WriteLine("\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_SKILL1_CAP")).caption + "</b></td>\";");
                if (_conf.itemPropertiesInShops.Contains("Skill2") && _conf.shopUseSkill2) twShops.WriteLine("\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_SKILL2_CAP")).caption + "</b></td>\";");
                if (_conf.itemPropertiesInShops.Contains("Skill3") && _conf.shopUseSkill3) twShops.WriteLine("\t\t\tshop_str += \"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_SKILL3_CAP")).caption + "</b></td>\";");
                if (_conf.itemPropertiesInShops.Contains("Image")) twShops.WriteLine("\t\t\tshop_str +=\"<td class=\\\"shop\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("SHOP_COL_IMAGE_CAP")).caption + "</b></td>\";");
                twShops.WriteLine("\t\t\tshop_str +=\"</tr>\";");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\tfor (var i=0; i<state.active.variables.shops[params[0]].items.length; i++) {");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\tif (state.active.variables.shops[params[0]].items[i].quantity < state.active.variables.shops[params[0]].items[i].maxQuantity) {");
                twShops.WriteLine("\t\t\t\t\tvar minutes_diff = Math.floor(((Math.abs(state.active.variables.time - state.active.variables.shops[params[0]].items[i].lastRefill))/1000)/60);				");
                twShops.WriteLine("\t\t\t\t\tif (minutes_diff > 0) {");
                twShops.WriteLine("\t\t\t\t\t\tvar items_after_refill = state.active.variables.shops[params[0]].items[i].quantity + Math.floor(minutes_diff / state.active.variables.shops[params[0]].items[i].refillDelay);");
                twShops.WriteLine("\t\t\t\t\t\tstate.active.variables.shops[params[0]].items[i].quantity = max(state.active.variables.shops[params[0]].items[i].maxQuantity, items_after_refill);");
                twShops.WriteLine("\t\t\t\t\t\tstate.active.variables.shops[params[0]].items[i].lastRefill = state.active.variables.time;");
                twShops.WriteLine("\t\t\t\t\t}");
                twShops.WriteLine("\t\t\t\t}");
                twShops.WriteLine("\t\t\t\tvar existing_items_with_id = [];");
                twShops.WriteLine("\t\t\t\tvar object_owned = [];");
                twShops.WriteLine("\t\t\t\tvar is_cloth_worn = false;");
                twShops.WriteLine("\t\t\t\tif (state.active.variables.shops[params[0]].items[i].type.toUpperCase() === \"ITEM\") {");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\t\t// Get the item in the list of all items");
                twShops.WriteLine("\t\t\t\t\texisting_items_with_id = state.active.variables.items.filter(obj => {return obj.ID === state.active.variables.shops[params[0]].items[i].ID});");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\t\t// Get the items owned by the player");
                twShops.WriteLine("\t\t\t\t\tif (existing_items_with_id.length > 0) {");
                twShops.WriteLine("\t\t\t\t\t\tobject_owned = state.active.variables.inventory.filter(obj => {return obj.ID === existing_items_with_id[0].ID});");
                twShops.WriteLine("\t\t\t\t\t}");
                twShops.WriteLine("\t\t\t\t} else if (state.active.variables.shops[params[0]].items[i].type.toUpperCase() === \"CLOTH\") {");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\t\t// Get the cloth in the list of all cloth");
                twShops.WriteLine("\t\t\t\t\texisting_items_with_id = state.active.variables.all_cloth.filter(obj => {return obj.ID === state.active.variables.shops[params[0]].items[i].ID});");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\t\t// Get the cloth owned by the player");
                twShops.WriteLine("\t\t\t\t\tif (existing_items_with_id.length > 0) {");
                twShops.WriteLine("\t\t\t\t\t\tobject_owned = state.active.variables.wardrobe.filter(obj => {return obj.ID === existing_items_with_id[0].ID});");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\t\t\t// Check if cloth is worn");
                twShops.WriteLine("\t\t\t\t\t\tis_cloth_worn = is_worn(existing_items_with_id[0].ID);");
                twShops.WriteLine("\t\t\t\t\t}");
                twShops.WriteLine("\t\t\t\t}");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\tif (existing_items_with_id.length == 0) {");
                twShops.WriteLine("\t\t\t\t\tthrowError(place, \"<<\" + macroName + \">>: There is no item from type \" + state.active.variables.shops[params[0]].items[i].type + \" with ID \" + state.active.variables.shops[params[0]].items[i].ID);");
                twShops.WriteLine("\t\t\t\t\treturn;");
                twShops.WriteLine("\t\t\t\t}");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\tshop_str += \"<tr>\";");
                if (_conf.itemPropertiesInShops.Contains("ID"))
                    twShops.WriteLine("\t\t\t\tshop_str += \"<td class=\\\"shop\\\">\" + state.active.variables.shops[params[0]].items[i].ID + \"</td>\";");
                if (_conf.itemPropertiesInShops.Contains("Name"))
                    twShops.WriteLine("\t\t\t\tshop_str += \"<td class=\\\"shop\\\">\" + existing_items_with_id[0].name + \"</td>\";");
                if (_conf.itemPropertiesInShops.Contains("Quantity"))
                    twShops.WriteLine("\t\t\t\tshop_str += \"<td class=\\\"shop\\\">\" + state.active.variables.shops[params[0]].items[i].quantity + \"</td>\";");

                if (_conf.itemPropertiesInShops.Contains("Skill1") && _conf.shopUseSkill1)
                    twShops.WriteLine("\t\t\t\tshop_str +=\"<td class=\\\"shop\\\">\" + existing_items_with_id[0].skill1 + \"</td>\";");
                if (_conf.itemPropertiesInShops.Contains("Skill2") && _conf.shopUseSkill2)
                    twShops.WriteLine("\t\t\t\tshop_str +=\"<td class=\\\"shop\\\">\" + existing_items_with_id[0].skill2 + \"</td>\";");
                if (_conf.itemPropertiesInShops.Contains("Skill3") && _conf.shopUseSkill3)
                    twShops.WriteLine("\t\t\t\tshop_str +=\"<td class=\\\"shop\\\">\" + existing_items_with_id[0].skill3 + \"</td>\";	");
                twShops.WriteLine("");
                twShops.WriteLine("\t\t\t\t// buy");

                if (_conf.itemPropertiesInShops.Contains("Buy"))
                {
                    twShops.WriteLine("\t\t\t\tif ((state.active.variables.money >= existing_items_with_id[0].buyPrice) && (state.active.variables.shops[params[0]].items[i].quantity > 0)) {");
                    twShops.WriteLine("\t\t\t\tshop_str += \"<td class=\\\"shop\\\"><a onClick=\\\"buy('\"+escape(JSON.stringify(existing_items_with_id[0]))+");
                    twShops.WriteLine("\t\t\t\t\t\"','\"+state.active.variables.shops[params[0]].items[i].type+\"',\" +params[0]+ \",\" +i+\");\\\" href=\\\"javascript:void(0);\\\">buy for \" + existing_items_with_id[0].buyPrice + \"\" + currency +\"</a></td>\";");
                    twShops.WriteLine("\t\t\t\t} else {");
                    twShops.WriteLine("\t\t\t\t\tshop_str += \"<td class=\\\"shop\\\">buy for \" + existing_items_with_id[0].buyPrice + \"\" + currency +\"</td>\";");
                    twShops.WriteLine("\t\t\t\t}");
                    twShops.WriteLine("");
                }

                if (_conf.itemPropertiesInShops.Contains("Sell"))
                {
                    twShops.WriteLine("\t\t\t\t// sell");
                    twShops.WriteLine("\t\t\t\tvar min_owned = (is_cloth_worn) ? 1 : 0;");
                    twShops.WriteLine("\t\t\t\tif ((object_owned.length > 0) && (object_owned[0].owned > min_owned)) {");
                    twShops.WriteLine("\t\t\t\t\tshop_str += \"<td class=\\\"shop\\\"><a onClick=\\\"sell('\"+escape(JSON.stringify(existing_items_with_id[0]))+");
                    twShops.WriteLine("\t\t\t\t\t\t\"','\"+state.active.variables.shops[params[0]].items[i].type+\"',\" +params[0]+ \",\" +i+\");\\\" href=\\\"javascript:void(0);\\\">sell for \" + existing_items_with_id[0].sellPrice + \"\" + currency + \"</a></td>\";");
                    twShops.WriteLine("\t\t\t\t} else {");
                    twShops.WriteLine("");
                    twShops.WriteLine("\t\t\t\t\t// Is player wearing that cloth?");
                    twShops.WriteLine("\t\t\t\t\tif ((object_owned.length > 0) && (object_owned[0].owned == 1) && (is_cloth_worn)) {");
                    twShops.WriteLine("\t\t\t\t\t\tshop_str += \"<td class=\\\"shop\\\">sell for \" + existing_items_with_id[0].sellPrice + \"\" + currency + \"<br>(You are wearing that)</td>\";");
                    twShops.WriteLine("\t\t\t\t\t} else {");
                    twShops.WriteLine("\t\t\t\t\t\tshop_str += \"<td class=\\\"shop\\\">sell for \" + existing_items_with_id[0].sellPrice + \"\" + currency + \"</td>\";");
                    twShops.WriteLine("\t\t\t\t\t}");
                    twShops.WriteLine("\t\t\t\t}");
                    twShops.WriteLine("");
                }

                if (_conf.itemPropertiesInShops.Contains("Image"))
                    twShops.WriteLine("\t\t\t\tshop_str += \"<td class=\\\"shop\\\"><img class=\\\"paragraph\\\" src=\\\"\" + existing_items_with_id[0].image + \"\\\"></td></tr>\";");
                twShops.WriteLine("\t\t\t}");
                twShops.WriteLine("\t\t\tshop_str +=\"</table>\";");
                twShops.WriteLine("\t\t\tnew Wikifier(place, shop_str);");
                twShops.WriteLine("\t\t}");
                twShops.WriteLine("\t}");
                twShops.WriteLine("};");
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

        private static void generateMoney(Configuration _conf, string _path)
        {
            string moneyPath = Path.Combine(_path, "_js_money.tw2");
            TextWriter twMoney = null;
            try
            {
                twMoney = new StreamWriter(moneyPath, false, new UTF8Encoding(false));
                twMoney.WriteLine("::Money[script]");
                twMoney.WriteLine("");

                // currency
                twMoney.WriteLine("var currency = \"" + _conf.captions.Single(s => s.captionName.Equals("MONEY_UNIT_CAP")).caption + "\";");
                twMoney.WriteLine("");

                // initMoney
                twMoney.WriteLine("macros.initMoney = {");
                twMoney.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twMoney.WriteLine("\t\tif (state.active.variables.money === undefined)");
                twMoney.WriteLine("\t\t{");
                twMoney.WriteLine("\t\t\tstate.active.variables.money = " + _conf.startMoney + ";");
                twMoney.WriteLine("\t\t\tstate.active.variables.moneyPerDay = " + _conf.moneyPerDay + ";");
                twMoney.WriteLine("\t\t\tstate.active.variables.lastMoneyUpdate = new Date(-8640000000000000);");
                twMoney.WriteLine("\t\t}");
                twMoney.WriteLine("\t}");
                twMoney.WriteLine("};");
                twMoney.WriteLine("");

                // printMoney
                twMoney.WriteLine("macros.printMoney = {");
                twMoney.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twMoney.WriteLine("\t\tif (state.active.variables.money != undefined)");
                twMoney.WriteLine("\t\t{");
                twMoney.WriteLine("\t\t\t// Update money if \"money per day\" has to be added.");
                twMoney.WriteLine("\t\t\tvar dayDiff = Math.floor((state.active.variables.time - state.active.variables.lastMoneyUpdate) / 86400000);");
                twMoney.WriteLine("\t\t\tif (dayDiff > 0)");
                twMoney.WriteLine("\t\t\t{");
                twMoney.WriteLine("\t\t\t\tstate.active.variables.money += state.active.variables.moneyPerDay;");
                twMoney.WriteLine("\t\t\t\tstate.active.variables.lastMoneyUpdate = state.active.variables.time;");
                twMoney.WriteLine("\t\t\t}");
                twMoney.WriteLine("\t\t\tnew Wikifier(place, \"\"+state.active.variables.money);");
                twMoney.WriteLine("\t\t} else {");
                twMoney.WriteLine("\t\t\tthrowError(place, \" << \" + macroName + \">>: please call initMoney first.\");");
                twMoney.WriteLine("\t\t\treturn;");
                twMoney.WriteLine("\t\t}");
                twMoney.WriteLine("\t}");
                twMoney.WriteLine("};");
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

        private static void generateJobs(Configuration _conf, string _path)
        {
            string jobsPath = Path.Combine(_path, "_js_jobs.tw2");
            TextWriter twJobs = null;
            try
            {
                twJobs = new StreamWriter(jobsPath, false, new UTF8Encoding(false));
                twJobs.WriteLine("::Jobs[script]");
                twJobs.WriteLine("");

                // initJobs
                twJobs.WriteLine("macros.initJobs = {");
                twJobs.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\tif (state.active.variables.jobs === undefined) {");
                twJobs.WriteLine("\t\t\tstate.active.variables.jobs = [];");
                
                for(int i=0; i<_conf.jobs.Count; i++)
                {
                    twJobs.Write("\t\t\tstate.active.variables.jobs.push({");
                    twJobs.Write("\"ID\":" + _conf.jobs[i].ID + ",");
                    twJobs.Write("\"name\":\"" + _conf.jobs[i].name + "\",");
                    twJobs.Write("\"description\":\"" + _conf.jobs[i].description + "\",");
                    twJobs.Write("\"category\":\"" + _conf.jobs[i].category + "\",");
                    twJobs.Write("\"available\":" + _conf.jobs[i].available.ToString().ToLower() + ",");
                    twJobs.Write("\"rewardMoney\":" + _conf.jobs[i].rewardMoney + ",");
                    twJobs.Write("\"cooldown\":" + _conf.jobs[i].cooldown + ",");
                    twJobs.Write("\"lastStart\":new Date(0, 0, 0, 0, 0, 0),");
                    twJobs.Write("\"duration\":" + _conf.jobs[i].duration + ",");
                    twJobs.Write("\"image\":\"" + pathSubtract(_conf.jobs[i].image, _conf.pathSubtract) + "\",");
                    twJobs.WriteLine("\"rewardItems\":[");
                    for(int j=0; j<_conf.jobs[i].rewardItems.Count; j++)
                    {
                        if (j < _conf.jobs[i].rewardItems.Count-1)
                        {
                            twJobs.WriteLine("\t\t\t\t{\"type\":\"" + _conf.jobs[i].rewardItems[j].type + "\", \"ID\":" + _conf.jobs[i].rewardItems[j].ID + ",\"amount\":" + _conf.jobs[i].rewardItems[j].amount + "},");
                        }
                        else
                        {
                            twJobs.WriteLine("\t\t\t\t{\"type\":\"" + _conf.jobs[i].rewardItems[j].type + "\", \"ID\":" + _conf.jobs[i].rewardItems[j].ID + ",\"amount\":" + _conf.jobs[i].rewardItems[j].amount + "}");
                        }
                    }
                    twJobs.WriteLine("\t\t\t]});");
                }

                twJobs.WriteLine("\t\t}");
                twJobs.WriteLine("\t}");
                twJobs.WriteLine("};");
                twJobs.WriteLine("");

                // doJob
                twJobs.WriteLine("window.doJob = function (_job) {");
                twJobs.WriteLine("");
                twJobs.WriteLine("\tvar job_obj = JSON.parse(unescape(_job));");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t// Add time");
                twJobs.WriteLine("\tif (state.active.variables.time !== undefined) {");
                twJobs.WriteLine("\t\tstate.active.variables.time.setMinutes(state.active.variables.time.getMinutes() + job_obj.duration);");
                twJobs.WriteLine("\t} else {");
                twJobs.WriteLine("\t\tthrowError(null, \"Time system has not been initialized and the cooldown can not be applied to job system.\");");
                twJobs.WriteLine("\t}");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t// Give reward");
                twJobs.WriteLine("\tif (state.active.variables.time !== undefined) {");
                twJobs.WriteLine("\t\tstate.active.variables.money +=job_obj.rewardMoney;");
                twJobs.WriteLine("\t} else {");
                twJobs.WriteLine("\t\tthrowError(null, \"Money system has not been initialized so no reward money can be given for doing job.\");");
                twJobs.WriteLine("\t}");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t// Add reward items");
                twJobs.WriteLine("\tif (state.active.variables.inventory !== undefined) {");
                twJobs.WriteLine("\t\tfor (var i=0; i<job_obj.rewardItems.length; i++) {");
                twJobs.WriteLine("\t\t\tif (job_obj.rewardItems[i].type === \"ITEM\") {");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t\t\tvar item_in_catalog = state.active.variables.items.filter(obj => {return obj.ID === job_obj.rewardItems[i].ID});");
                twJobs.WriteLine("\t\t\t\tif (item_in_catalog.length != 1) {");
                twJobs.WriteLine("\t\t\t\t\tthrowError(place, \"<<\" + macroName + \">>: There must be exactly one item of id \" + params[0] + \" in the item catalog but there are \" + item_in_catalog.length);");
                twJobs.WriteLine("\t\t\t\t\treturn;");
                twJobs.WriteLine("\t\t\t\t}");
                twJobs.WriteLine("\t\t\t\tvar item = JSON.parse(JSON.stringify(item_in_catalog[0]));");
                twJobs.WriteLine("\t\t\t\titem.owned = job_obj.rewardItems[i].amount;");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t\t\t// item not yet existent?");
                twJobs.WriteLine("\t\t\t\tvar existing_items_with_id = state.active.variables.inventory.filter(obj => {");
                twJobs.WriteLine("\t\t\t\t\treturn obj.ID === item.ID");
                twJobs.WriteLine("\t\t\t\t});");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t\t\tif (existing_items_with_id.length == 0) {");
                twJobs.WriteLine("\t\t\t\t\t// add new item");
                twJobs.WriteLine("\t\t\t\t\tstate.active.variables.inventory.push(item);");
                twJobs.WriteLine("\t\t\t\t} else if (existing_items_with_id.length > 0) {");
                twJobs.WriteLine("\t\t\t\t\t// change owned");
                twJobs.WriteLine("\t\t\t\t\tfor (var i in state.active.variables.inventory) {");
                twJobs.WriteLine("\t\t\t\t\t\tif (state.active.variables.inventory[i].ID == item.ID) {");
                twJobs.WriteLine("\t\t\t\t\t\t\tstate.active.variables.inventory[i].owned += item.owned;");
                twJobs.WriteLine("\t\t\t\t\t\t\tbreak;");
                twJobs.WriteLine("\t\t\t\t\t\t}");
                twJobs.WriteLine("\t\t\t\t\t}");
                twJobs.WriteLine("\t\t\t\t} else {");
                twJobs.WriteLine("\t\t\t\t\tthrowError(place, \"<<\" + macroName + \">>: There are several items with the same id \" + item.ID);");
                twJobs.WriteLine("\t\t\t\t\treturn;");
                twJobs.WriteLine("\t\t\t\t}");
                twJobs.WriteLine("\t\t\t} else if (job_obj.rewardItems[i].type === \"CLOTH\") {");
                twJobs.WriteLine("\t\t\t\tvar cloth_in_catalog = state.active.variables.all_cloth.filter(obj => {return obj.ID === job_obj.rewardItems[i].ID});");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t\t\tif (cloth_in_catalog.length == 0) {");
                twJobs.WriteLine("\t\t\t\t\tthrowError(place, \"<<\" + macroName + \">>: Cloth with id \" + job_obj.rewardItems[i].ID + \" does not exist.\");");
                twJobs.WriteLine("\t\t\t\t\treturn;");
                twJobs.WriteLine("\t\t\t\t}");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t\t\t// Clone cloth");
                twJobs.WriteLine("\t\t\t\tvar new_cloth = JSON.parse(JSON.stringify(cloth_in_catalog[0]));");
                twJobs.WriteLine("\t\t\t\tnew_cloth.owned = job_obj.rewardItems[i].amount;");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t\t\t// cloth not yet existent?");
                twJobs.WriteLine("\t\t\t\tvar existing_cloth_in_wardrobe_with_id = state.active.variables.wardrobe.filter(obj => {return obj.ID === job_obj.rewardItems[i].ID});");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t\t\tif (existing_cloth_in_wardrobe_with_id.length == 0) {");
                twJobs.WriteLine("\t\t\t\t\t// add new cloth");
                twJobs.WriteLine("\t\t\t\t\tstate.active.variables.wardrobe.push(new_cloth);");
                twJobs.WriteLine("\t\t\t\t} else if (existing_cloth_in_wardrobe_with_id.length > 0) {");
                twJobs.WriteLine("\t\t\t\t\t// change owned");
                twJobs.WriteLine("\t\t\t\t\tfor (var i in state.active.variables.wardrobe) {");
                twJobs.WriteLine("\t\t\t\t\t\tif (state.active.variables.wardrobe[i].ID == new_cloth.ID) {");
                twJobs.WriteLine("\t\t\t\t\t\t\tstate.active.variables.wardrobe[i].owned += new_cloth.owned;");
                twJobs.WriteLine("\t\t\t\t\t\t\tbreak;");
                twJobs.WriteLine("\t\t\t\t\t\t}");
                twJobs.WriteLine("\t\t\t\t\t}");
                twJobs.WriteLine("\t\t\t\t} else {");
                twJobs.WriteLine("\t\t\t\t\tthrowError(place, \"<<\" + macroName + \">>: There are several cloth with the same id \" + new_cloth.ID);");
                twJobs.WriteLine("\t\t\t\t\treturn;");
                twJobs.WriteLine("\t\t\t\t}");
                twJobs.WriteLine("\t\t\t}");
                twJobs.WriteLine("\t\t}");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t// Set cooldown");
                twJobs.WriteLine("\t\tvar job_by_id = state.active.variables.jobs.filter(obj => {return obj.ID == job_obj.ID});");
                twJobs.WriteLine("\t\tif (job_by_id.length == 1) {");
                twJobs.WriteLine("\t\t\tjob_by_id[0].lastStart = state.active.variables.time;");
                twJobs.WriteLine("\t\t}");
                twJobs.WriteLine("\t} else {");
                twJobs.WriteLine("\t\tthrowError(null, \"Inventory system has not been initialized so no reward items can be given for doing job.\");");
                twJobs.WriteLine("\t}");
                twJobs.WriteLine("\tstate.display(state.active.title, null, \"back\");");
                twJobs.WriteLine("};");
                twJobs.WriteLine("");

                // showJobs
                twJobs.WriteLine("macros.showJobs = {");
                twJobs.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\tif (params.length < 1) {");
                twJobs.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: showJobs expects an array of job IDs as first parameter.\");");
                twJobs.WriteLine("\t\t\treturn;");
                twJobs.WriteLine("\t\t}");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\tif (state.active.variables.jobs === undefined) {");
                twJobs.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: please call initJobs first.\");");
                twJobs.WriteLine("\t\t\treturn;");
                twJobs.WriteLine("\t\t}");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\tvar jobs_str = \"<table class=\\\"jobs\\\"><tr>\";");
                if (_conf.displayInJobsView.Contains("ID")) twJobs.WriteLine("\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_ID_CAP")).caption + "</th>\";");
                if (_conf.displayInJobsView.Contains("Name")) twJobs.WriteLine("\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_NAME_CAP")).caption + "</th>\";");
                if (_conf.displayInJobsView.Contains("Description")) twJobs.WriteLine("\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_DESCRIPTION_CAP")).caption + "</th>\";");
                if (_conf.displayInJobsView.Contains("Category")) twJobs.WriteLine("\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_CATEGORY_CAP")).caption + "</th>\";");
                if (_conf.displayInJobsView.Contains("Available")) twJobs.WriteLine("\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_AVAILABLE_CAP")).caption + "</th>\";");
                if (_conf.displayInJobsView.Contains("RewardMoney")) twJobs.WriteLine("\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_REWARD_MONEY_CAP")).caption + "</th>\";");
                if (_conf.displayInJobsView.Contains("Cooldown")) twJobs.WriteLine("\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_COOLDOWN_CAP")).caption + "</th>\";");
                if (_conf.displayInJobsView.Contains("LastStart")) twJobs.WriteLine("\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_LAST_START_CAP")).caption + "</th>\";");
                if (_conf.displayInJobsView.Contains("Duration")) twJobs.WriteLine("\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_DURATION_CAP")).caption + "</th>\";");
                if (_conf.displayInJobsView.Contains("Image")) twJobs.WriteLine("\t\tjobs_str +=\"<th class=\\\"jobs\\\">" + _conf.captions.Single(s => s.captionName.Equals("JOBS_COL_IMAGE_CAP")).caption + "</th>\";");
                twJobs.WriteLine("\t\tjobs_str +=\"<th class=\\\"jobs\\\">start</th>\";");
                twJobs.WriteLine("\t\tjobs_str +=\"</tr>\";");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\tfor(var i=0; i<params.length; i++) {");
                twJobs.WriteLine("\t\t\tvar job_by_id = state.active.variables.jobs.filter(obj => {return obj.ID == params[i]});");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t\tif (job_by_id.length == 1) {");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t\t\t// Check if job cooldown is passed");
                twJobs.WriteLine("\t\t\t\tvar minutes_diff = Math.floor(((Math.abs(state.active.variables.time - job_by_id[0].lastStart))/1000)/60);");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t\t\tjobs_str +=\"<tr>\";");
                if (_conf.displayInJobsView.Contains("ID")) twJobs.WriteLine("\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].ID + \"</td>\";");
                if (_conf.displayInJobsView.Contains("Name")) twJobs.WriteLine("\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].name + \"</td>\";");
                if (_conf.displayInJobsView.Contains("Description")) twJobs.WriteLine("\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].description + \"</td>\";");
                if (_conf.displayInJobsView.Contains("Category")) twJobs.WriteLine("\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].category + \"</td>\";");
                if (_conf.displayInJobsView.Contains("Available")) twJobs.WriteLine("\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].avaiable + \"</td>\";");
                if (_conf.displayInJobsView.Contains("RewardMoney")) twJobs.WriteLine("\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].rewardMoney + \"</td>\";");
                if (_conf.displayInJobsView.Contains("Cooldown")) twJobs.WriteLine("\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].cooldown + \"</td>\";");
                if (_conf.displayInJobsView.Contains("LastStart")) twJobs.WriteLine("\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].lastStart + \"</td>\";");
                if (_conf.displayInJobsView.Contains("Duration")) twJobs.WriteLine("\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">\" + job_by_id[0].duration + \"</td>\";");
                if (_conf.displayInJobsView.Contains("Image")) twJobs.WriteLine("\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\"><img class=\\\"paragraph\\\" src=\\\"\" + job_by_id[0].image + \"\\\"></td>\";");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t\t\tif ((minutes_diff >= job_by_id[0].cooldown) && (job_by_id[0].available)) {");
                twJobs.WriteLine("\t\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\"><a onClick=\\\"doJob('\"+escape(JSON.stringify(job_by_id[0]))+\"');\\\" href=\\\"javascript:void(0);\\\">Start</a></td>\";");
                twJobs.WriteLine("\t\t\t\t} else {");
                twJobs.WriteLine("\t\t\t\t\tjobs_str +=\"<td class=\\\"jobs\\\">Not ready</td>\";");
                twJobs.WriteLine("\t\t\t\t}");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\t\t\tjobs_str +=\"</tr>\";");
                twJobs.WriteLine("\t\t\t}");
                twJobs.WriteLine("\t\t}");
                twJobs.WriteLine("");
                twJobs.WriteLine("\t\tjobs_str +=\"</table>\";");
                twJobs.WriteLine("\t\tnew Wikifier(place, jobs_str);");
                twJobs.WriteLine("\t}");
                twJobs.WriteLine("};");
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

        private static void generateCharacters(Configuration _conf, string _path)
        {
            string characterPath = Path.Combine(_path, "_js_characters.tw2");
            TextWriter twCharacters = null;
            try
            {
                twCharacters = new StreamWriter(characterPath, false, new UTF8Encoding(false));
                twCharacters.WriteLine("::Characters[script]");
                twCharacters.WriteLine("");
                
                // initCharacters
                twCharacters.WriteLine("macros.initCharacters = {");
                twCharacters.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twCharacters.WriteLine("");
                twCharacters.WriteLine("\t\tif (state.active.variables.characters === undefined) {");
                twCharacters.WriteLine("\t\t\tstate.active.variables.characters = [];");

                for(int i=0; i<_conf.characters.Count; i++)
                {
                    twCharacters.Write("\t\t\tstate.active.variables.characters.push({");
                    twCharacters.Write("\"ID\":" + _conf.characters[i].ID + ",");
                    twCharacters.Write("\"name\":\"" + _conf.characters[i].name + "\",");
                    twCharacters.Write("\"category\":\"" + _conf.characters[i].category + "\",");
                    twCharacters.Write("\"description\":\"" + _conf.characters[i].description + "\",");
                    twCharacters.Write("\"age\":" + _conf.characters[i].age + ",");
                    twCharacters.Write("\"gender\":\"" + _conf.characters[i].gender + "\",");
                    twCharacters.Write("\"job\":\"" + _conf.characters[i].job + "\",");
                    twCharacters.Write("\"relation\":" + _conf.characters[i].relation + ",");
                    twCharacters.Write("\"known\":" + _conf.characters[i].known.ToString().ToLower() + ",");
                    twCharacters.Write("\"color\":\"" + _conf.characters[i].color + "\",");
                    twCharacters.Write("\"image\":\"" + pathSubtract(_conf.characters[i].image, _conf.pathSubtract) + "\"");
                    if (_conf.characterUseSkill1)
                    {
                        string skill1val = (isBool(_conf.characters[i].skill1) || isNumber(_conf.characters[i].skill1)) ? _conf.characters[i].skill1 : "\"" + _conf.characters[i].skill1 + "\"";
                        twCharacters.Write(",\"" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_SKILL1_CAP")).caption + "\":" + skill1val);
                    }
                    if (_conf.characterUseSkill2)
                    {
                        string skill2val = (isBool(_conf.characters[i].skill2) || isNumber(_conf.characters[i].skill2)) ? _conf.characters[i].skill2 : "\"" + _conf.characters[i].skill2 + "\"";
                        twCharacters.Write(",\"" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_SKILL2_CAP")).caption + "\":" + skill2val);
                    }
                    if (_conf.characterUseSkill3)
                    {
                        string skill3val = (isBool(_conf.characters[i].skill3) || isNumber(_conf.characters[i].skill3)) ? _conf.characters[i].skill3 : "\"" + _conf.characters[i].skill3 + "\"";
                        twCharacters.Write(",\"" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_SKILL3_CAP")).caption + "\":" + skill3val);
                    }
                    twCharacters.WriteLine("});");
                }


                twCharacters.WriteLine("\t\t}");
                twCharacters.WriteLine("\t}");
                twCharacters.WriteLine("};");
                twCharacters.WriteLine("");

                // characters
                twCharacters.WriteLine("macros.characters = {");
                twCharacters.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twCharacters.WriteLine("\t\tvar wstr = \"<table class=\\\"character\\\">\";");
                twCharacters.WriteLine("\t\twstr +=\"<tr>\";");
                if (_conf.displayInCharactersView.Contains("ID")) twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_ID_CAP")).caption + "</b></td>\";");
                if (_conf.displayInCharactersView.Contains("Name")) twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_NAME_CAP")).caption + "</b></td>\";");
                if (_conf.displayInCharactersView.Contains("Category")) twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_CATEGORY_CAP")).caption + "</b></td>\";");
                if (_conf.displayInCharactersView.Contains("Description")) twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_DESCRIPTION_CAP")).caption + "</b></td>\";");
                if (_conf.displayInCharactersView.Contains("Age")) twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_AGE_CAP")).caption + "</b></td>\";");
                if (_conf.displayInCharactersView.Contains("Gender")) twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_GENDER_CAP")).caption + "</b></td>\";");
                if (_conf.displayInCharactersView.Contains("Job")) twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_JOB_CAP")).caption + "</b></td>\";");
                if (_conf.displayInCharactersView.Contains("Relation")) twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_RELATION_CAP")).caption + "</b></td>\";");
                if (_conf.displayInCharactersView.Contains("Known")) twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_KNOWN_CAP")).caption + "</b></td>\";");
                if (_conf.displayInCharactersView.Contains("Color")) twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_COLOR_CAP")).caption + "</b></td>\";");
                if (_conf.displayInCharactersView.Contains("Image")) twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_IMAGE_CAP")).caption + "</b></td>\";");
                twCharacters.WriteLine("");

                if (_conf.characterUseSkill1 && _conf.displayInCharactersView.Contains("Skill1"))
                    twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_SKILL1_CAP")).caption + "</b></td>\";");

                if (_conf.characterUseSkill2 && _conf.displayInCharactersView.Contains("Skill2"))
                    twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_SKILL2_CAP")).caption + " </b></td>\";");

                if (_conf.characterUseSkill2 && _conf.displayInCharactersView.Contains("Skill3"))
                    twCharacters.WriteLine("\t\twstr +=\"<td class=\\\"character\\\"><b>" + _conf.captions.Single(s => s.captionName.Equals("CHARACTER_COL_SKILL3_CAP")).caption + "</b></td>\";");
                twCharacters.WriteLine("\t\twstr +=\"</tr>\";");
                twCharacters.WriteLine("");
                twCharacters.WriteLine("\t\tfor (var w = 0; w<state.active.variables.characters.length; w++) {");
                twCharacters.WriteLine("\t\t\twstr +=\"<tr>\";");
                if (_conf.displayInCharactersView.Contains("ID")) twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].ID + \"</td>\";");
                if (_conf.displayInCharactersView.Contains("Name")) twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].name + \"</td>\";");
                if (_conf.displayInCharactersView.Contains("Category")) twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].category + \"</td>\";");
                if (_conf.displayInCharactersView.Contains("Description")) twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].description + \"</td>\";");
                if (_conf.displayInCharactersView.Contains("Age")) twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].age + \"</td>\";");
                if (_conf.displayInCharactersView.Contains("Gender")) twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].gender + \"</td>\";");
                if (_conf.displayInCharactersView.Contains("Job")) twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].job + \"</td>\";");
                if (_conf.displayInCharactersView.Contains("Relation")) twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].relation + \"</td>\";");
                if (_conf.displayInCharactersView.Contains("Known")) twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].known + \"</td>\";");
                if (_conf.displayInCharactersView.Contains("Color")) twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].color + \"</td>\";");
                if (_conf.displayInCharactersView.Contains("Image")) twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"paragraph\\\" src=\\\"\" + state.active.variables.characters[w].image + \"\\\"></td>\";");
                twCharacters.WriteLine("");

                if (_conf.characterUseSkill1 && _conf.displayInCharactersView.Contains("Skill1"))
                    twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].skill1 + \"</td>\";");

                if (_conf.characterUseSkill2 && _conf.displayInCharactersView.Contains("Skill2"))
                    twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].skill2 + \"</td>\";");

                if (_conf.characterUseSkill3 && _conf.displayInCharactersView.Contains("Skill3"))
                    twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\">\" + state.active.variables.characters[w].skill3 + \"</td>\";");
                twCharacters.WriteLine("");
                twCharacters.WriteLine("\t\t\twstr +=\"</tr>\";");
                twCharacters.WriteLine("\t\t}");
                twCharacters.WriteLine("\t\twstr +=\"</table>\";");
                twCharacters.WriteLine("");
                twCharacters.WriteLine("\t\tnew Wikifier(place,wstr);");
                twCharacters.WriteLine("\t}");
                twCharacters.WriteLine("};");
                twCharacters.WriteLine("");

                // say
                twCharacters.WriteLine("macros.say = {");
                twCharacters.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twCharacters.WriteLine("");
                twCharacters.WriteLine("\t\tif (params.length != 2) {");
                twCharacters.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: expects two parameters, character ID and text but there were \" + params.length + \" parameters.\");");
                twCharacters.WriteLine("\t\t\treturn;");
                twCharacters.WriteLine("\t\t}");
                twCharacters.WriteLine("");
                twCharacters.WriteLine("\t\tvar character = state.active.variables.characters.filter(c => {return c.ID === params[0]});");
                twCharacters.WriteLine("\t\tif (character.length != 1) {");
                twCharacters.WriteLine("\t\t\tthrowError(place, \"<<\" + macroName + \">>: There must be exactly one character of id \" + params[0] + \" in the character list but there are \" + item_in_catalog.length);");
                twCharacters.WriteLine("\t\t\treturn;");
                twCharacters.WriteLine("\t\t}");
                twCharacters.WriteLine("");
                twCharacters.WriteLine("\t\tvar wstr = \"<table class=\\\"say\\\"><tr><td rowspan=\\\"2\\\"><img class=\\\"dialog\\\" src=\\\"\" + character[0].image + \"\\\"></td><td><font color=\\\"\"+character[0].color+\"\\\">\" + character[0].name + \"</font></td></tr><tr><td>\"+params[1]+\"</td></tr></table>\";");
                twCharacters.WriteLine("");
                twCharacters.WriteLine("\t\tnew Wikifier(place, wstr);");
                twCharacters.WriteLine("\t}");
                twCharacters.WriteLine("};");
                twCharacters.WriteLine("");

                // characterSidebar
                twCharacters.WriteLine("macros.charactersSidebar = {");
                twCharacters.WriteLine("\thandler: function(place, macroName, params, parser) {");
                twCharacters.WriteLine("");
                twCharacters.WriteLine("\t\tvar wstr = \"<table class=\\\"character\\\">\";	");
                twCharacters.WriteLine("\t\twstr +=\"<tr><td colspan=2>Characters</td></tr>\";	");
                twCharacters.WriteLine("\t\tfor (var w = 0; w<state.active.variables.characters.length; w +=2) {");
                twCharacters.WriteLine("\t\t\twstr +=\"<tr>\";");
                twCharacters.WriteLine("");
                twCharacters.WriteLine("\t\t\tvar char_info_1 = \"\";");
                if (_conf.displayInCharactersView.Contains("ID")) twCharacters.WriteLine("\t\t\tchar_info_1 += \"ID: \" + state.active.variables.characters[w].ID + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Name")) twCharacters.WriteLine("\t\t\tchar_info_1 += \"name:\" + state.active.variables.characters[w].name + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Category")) twCharacters.WriteLine("\t\t\tchar_info_1 += \"category:\" + state.active.variables.characters[w].category + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Description")) twCharacters.WriteLine("\t\t\tchar_info_1 += \"description:\" + state.active.variables.characters[w].description + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Age")) twCharacters.WriteLine("\t\t\tchar_info_1 += \"age:\" + state.active.variables.characters[w].age + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Gender")) twCharacters.WriteLine("\t\t\tchar_info_1 += \"gender:\" + state.active.variables.characters[w].gender + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Job")) twCharacters.WriteLine("\t\t\tchar_info_1 += \"job:\" + state.active.variables.characters[w].job + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Relation")) twCharacters.WriteLine("\t\t\tchar_info_1 += \"relation:\" + state.active.variables.characters[w].relation + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Known")) twCharacters.WriteLine("\t\t\tchar_info_1 += \"known:\" + state.active.variables.characters[w].known + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Color")) twCharacters.WriteLine("\t\t\tchar_info_1 += \"color:\" + state.active.variables.characters[w].color + \"&#10;\";");
                if (_conf.characterUseSkill1 && _conf.displayInCharactersView.Contains("Skill1"))
                    twCharacters.WriteLine("\t\t\tchar_info_1 +=\"skill1: \" + state.active.variables.characters[w].skill1 + \"&#10;\";");
                if (_conf.characterUseSkill2 && _conf.displayInCharactersView.Contains("Skill2"))
                    twCharacters.WriteLine("\t\t\tchar_info_1 +=\"snill2: \" + state.active.variables.characters[w].skill2 + \"&#10;\";");
                if (_conf.characterUseSkill3 && _conf.displayInCharactersView.Contains("Skill3"))
                    twCharacters.WriteLine("\t\t\tchar_info_1 +=\"skill3: \" + state.active.variables.characters[w].skill3 + \"&#10;\";");
                twCharacters.WriteLine("");

                if (_conf.charactersSidebarTooltip)
                {
                    twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.characters[w].image + \"\\\" title=\\\"\" + char_info_1 + \"\\\"></td>\";");
                } else
                {
                    twCharacters.WriteLine("\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.characters[w].image + \"\\\"></td>\";");
                }
                twCharacters.WriteLine("\t\t\tif (w+1 < state.active.variables.characters.length) {");
                twCharacters.WriteLine("");
                twCharacters.WriteLine("\t\t\t\tvar char_info_2 = \"\";");
                if (_conf.displayInCharactersView.Contains("ID")) twCharacters.WriteLine("\t\t\t\tchar_info_2 += \"ID: \" + state.active.variables.characters[w+1].ID + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Name")) twCharacters.WriteLine("\t\t\t\tchar_info_2 += \"name:\" + state.active.variables.characters[w+1].name + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Category")) twCharacters.WriteLine("\t\t\t\tchar_info_2 += \"category:\" + state.active.variables.characters[w+1].category + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Description")) twCharacters.WriteLine("\t\t\t\tchar_info_2 += \"description:\" + state.active.variables.characters[w+1].description + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Age")) twCharacters.WriteLine("\t\t\t\tchar_info_2 += \"age:\" + state.active.variables.characters[w+1].age + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Gender")) twCharacters.WriteLine("\t\t\t\tchar_info_2 += \"gender:\" + state.active.variables.characters[w+1].gender + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Job")) twCharacters.WriteLine("\t\t\t\tchar_info_2 += \"job:\" + state.active.variables.characters[w+1].job + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Relation")) twCharacters.WriteLine("\t\t\t\tchar_info_2 += \"relation:\" + state.active.variables.characters[w+1].relation + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Known")) twCharacters.WriteLine("\t\t\t\tchar_info_2 += \"known:\" + state.active.variables.characters[w+1].known + \"&#10;\";");
                if (_conf.displayInCharactersView.Contains("Color")) twCharacters.WriteLine("\t\t\t\tchar_info_2 += \"color:\" + state.active.variables.characters[w+1].color + \"&#10;\";");

                if (_conf.characterUseSkill1 && _conf.displayInCharactersView.Contains("Skill1"))
                    twCharacters.WriteLine("\t\t\t\tchar_info_2 +=\"skill1: \" + state.active.variables.characters[w+1].skill1 + \"&#10;\";");

                if (_conf.characterUseSkill2 && _conf.displayInCharactersView.Contains("Skill2"))
                    twCharacters.WriteLine("\t\t\t\tchar_info_2 +=\"snill2: \" + state.active.variables.characters[w+1].skill2 + \"&#10;\";");

                if (_conf.characterUseSkill3 && _conf.displayInCharactersView.Contains("Skill3"))
                    twCharacters.WriteLine("\t\t\t\tchar_info_2 +=\"skill3: \" + state.active.variables.characters[w+1].skill3 + \"&#10;\";");
                twCharacters.WriteLine("");

                if (_conf.charactersSidebarTooltip)
                {
                    twCharacters.WriteLine("\t\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.characters[w+1].image + \"\\\" title=\\\"\" + char_info_2 + \"\\\"></td>\";");
                } else
                {
                    twCharacters.WriteLine("\t\t\t\twstr +=\"<td class=\\\"character\\\"><img class=\\\"sidebar\\\" src=\\\"\" + state.active.variables.characters[w+1].image + \"\\\"></td>\";");
                }
                twCharacters.WriteLine("\t\t\t} else {");
                twCharacters.WriteLine("\t\t\t\twstr +=\"<td></td>\";");
                twCharacters.WriteLine("\t\t\t}");
                twCharacters.WriteLine("\t\t\twstr +=\"</tr>\";");
                twCharacters.WriteLine("\t\t}");
                twCharacters.WriteLine("\t\twstr +=\"</table>\";");
                twCharacters.WriteLine("");
                twCharacters.WriteLine("\t\tnew Wikifier(place,wstr);");
                twCharacters.WriteLine("\t}");
                twCharacters.WriteLine("};");
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

                twBat.WriteLine("twee2 build " + storyFile + " " + htmlFile);
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

        private static void generateCss(Configuration _conf, string _path)
        {
            string cssPath = Path.Combine(_path, "_css_style.tw2");
            TextWriter twCss = null;
            try
            {
                twCss = new StreamWriter(cssPath, false, new UTF8Encoding(false));
                twCss.WriteLine("::CSSStyle[scss stylesheet]");
                twCss.WriteLine("");
                twCss.WriteLine("table.cloth {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%; }");
                twCss.WriteLine("td.cloth {overflow: hidden; text-align: center; vertical-align: middle;}");
                twCss.WriteLine("th.cloth {}");
                twCss.WriteLine("");
                twCss.WriteLine("table.cloth_sidebar {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}");
                twCss.WriteLine("td.cloth_sidebar {overflow: hidden; text-align: center; vertical-align: middle;}");
                twCss.WriteLine("th.cloth_sidebar {}");
                twCss.WriteLine("");
                twCss.WriteLine("table.inventory {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}");
                twCss.WriteLine("td.inventory {overflow: hidden; text-align: center; vertical-align: middle;}");
                twCss.WriteLine("th.inventory {}");
                twCss.WriteLine("");
                twCss.WriteLine("table.inventory_sidebar {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}");
                twCss.WriteLine("td.inventory_sidebar {overflow: hidden; text-align: center; vertical-align: middle;}");
                twCss.WriteLine("th.inventory_sidebar {}");
                twCss.WriteLine("");
                twCss.WriteLine("table.wardrobe {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}");
                twCss.WriteLine("td.wardrobe {overflow: hidden; text-align: center; vertical-align: middle;}");
                twCss.WriteLine("th.wardrobe {}");
                twCss.WriteLine("");
                twCss.WriteLine("table.stats {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}");
                twCss.WriteLine("td.stats {overflow: hidden; text-align: center; vertical-align: middle;}");
                twCss.WriteLine("th.stats {}");
                twCss.WriteLine("");
                twCss.WriteLine("table.stats_sidebar {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}");
                twCss.WriteLine("td.stats_sidebar {overflow: hidden; text-align: center; vertical-align: middle;}");
                twCss.WriteLine("th.stats_sidebar {}");
                twCss.WriteLine("");
                twCss.WriteLine("table.shop {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}");
                twCss.WriteLine("td.shop {overflow: hidden; text-align: center; vertical-align: middle;}");
                twCss.WriteLine("th.shop {}");
                twCss.WriteLine("");
                twCss.WriteLine("table.jobs {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}");
                twCss.WriteLine("td.jobs {overflow: hidden; text-align: center; vertical-align: middle;}");
                twCss.WriteLine("th.jobs {}");
                twCss.WriteLine("img.jobs {width:100%; max-width:100px;}");
                twCss.WriteLine("");
                twCss.WriteLine("table.character {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}");
                twCss.WriteLine("td.character {overflow: hidden;text-align: center; vertical-align: middle;}");
                twCss.WriteLine("th.character {}");
                twCss.WriteLine("");
                twCss.WriteLine("table.character_sidebar {border: 1px solid white; text-align: center; table-layout:fixed; width: 100%;}");
                twCss.WriteLine("td.character_sidebar {overflow: hidden;text-align: center; vertical-align: middle;}");
                twCss.WriteLine("th.character_sidebar {}");
                twCss.WriteLine("");
                twCss.WriteLine("table.say {border: 1px solid white; table-layout:fixed; width: 100%;}");
                twCss.WriteLine("td.say {overflow: hidden;text-align: center; vertical-align: middle;}");
                twCss.WriteLine("th.say {}");
                twCss.WriteLine("");
                twCss.WriteLine("#passages {max-width: " + _conf.paragraphWidth + "px;}");

                if (_conf.resizeImagesInSidebar)
                {
                    twCss.WriteLine("img.sidebar {height: auto; width: auto; max-width: " + _conf.imageWidthInSidebar + "px; max-height: " + _conf.imageHeightInSidebar + "px; }");
                }
                else
                {
                    twCss.WriteLine("img.sidebar {}");
                }

                if (_conf.resizeImagesInParagraph)
                {
                    twCss.WriteLine("img.paragraph {height: auto; width: auto; max-width: " + _conf.imageWidthInParagraph + "px; max-height: " + _conf.imageHeightInParagraph + "px; }");
                }
                else
                {
                    twCss.WriteLine("img.paragraph {}");
                }

                if (_conf.resizeImagesInDialogs)
                {
                    twCss.WriteLine("img.dialogs {height: auto; width: auto; max-width: " + _conf.imageWidthInDialogs + "px; max-height: " + _conf.imageHeightInDialogs + "px; }");
                }
                else
                {
                    twCss.WriteLine("img.dialogs {}");
                }
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


        public static string generate(Configuration _conf)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            DialogResult res = fbd.ShowDialog();
                if ((res == DialogResult.OK) && (!string.IsNullOrEmpty(fbd.SelectedPath)))
                {
                    generateMain(_conf, fbd.SelectedPath);
                    generateMenu(_conf, fbd.SelectedPath);
                    //generateCaptions(_conf, fbd.SelectedPath);
                    generateNavigation(_conf, fbd.SelectedPath);
                    generateCss(_conf, fbd.SelectedPath);
                    if (_conf.inventoryActive) generateInventory(_conf, fbd.SelectedPath);
                    if (_conf.clothActive) generateCloth(_conf, fbd.SelectedPath);
                    if (_conf.statsActive) generateStats(_conf, fbd.SelectedPath);
                    if (_conf.daytimeActive) generateDaytime(_conf, fbd.SelectedPath);
                    if (_conf.shopActive) generateShops(_conf, fbd.SelectedPath);
                    if (_conf.moneyActive) generateMoney(_conf, fbd.SelectedPath);
                    if (_conf.jobsActive) generateJobs(_conf, fbd.SelectedPath);
                    if (_conf.charactersActive) generateCharacters(_conf, fbd.SelectedPath);
                    generateBat(_conf, fbd.SelectedPath);
                }

            return fbd.SelectedPath;
        }

        private static string pathSubtract(string _s, string _subtract)
        {
            int index = _s.IndexOf(_subtract);
            return (index < 0) ? _s : _s.Remove(index, _subtract.Length);
        }
    }
}
