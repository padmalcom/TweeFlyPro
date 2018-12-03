using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweeFly
{
    class Templates
    {
        public static string INV_INIT = "::Inv[script]";

        public static string[] INV_CODE = {
            "window.getInv = captionName() { return state.active.variables.inventory; }",

            "macros.initInv = { handler: captionName(place, macroName, params, parser) {" +
                "state.active.variables.inventory = []; } };",

            "macros.addToInv = {" +
                "handler: captionName (place, macroName, params, parser) {" +
		        "if (params.length != 2) {"+
                    "throwError(place, \"<<\" + macroName + \">>: expects two parameters, item id and amount.\");"+
                    "return;"+
                "}"+

		        "if ((!Number.isInteger(params[0])) || (!Number.isInteger(params[1]))) {"+
                    "throwError(place, \"<<\" + macroName + \">>: expects two integer parameters.\");"+
			        "return;"+
		        "}"+

		        "if (typeof items[params[0]] === 'undefined') {"+
			        "throwError(place, \"<<\" + macroName + \">>: An item with id \" + params[0] + \" does not exist.\");"+
			        "return;"+
		        "}"+

                "var item_in_catalog = items.filter(obj => { return obj.ID === params[0]});"+
		        "if (item_in_catalog.length != 1) {"+
			        "throwError(place, \"<<\" + macroName + \">>: There must be exactly one item of id \" + params[0] + \" in the item catalog but there are \" + item_in_catalog.length);"+
			        "return;"+		
		        "}"+

		        "var item = JSON.parse(JSON.stringify(item_in_catalog[0]));"+
                
                "item.amount = params[1];"+

		        "var existing_items_with_id = state.active.variables.inventory.filter(obj => {"+
                    "return obj.ID === item.ID"+
                "});"+

		        "if (existing_items_with_id.length == 0) {"+
			        "state.active.variables.inventory.push(item);"+
		        "} else if (existing_items_with_id.length == 1) {"+
			        "for (var i in state.active.variables.inventory) {"+
				        "if (state.active.variables.inventory[i].ID == item.ID) {"+
					       "state.active.variables.inventory[i].amount = state.active.variables.inventory[i].amount + item.amount;"+
					        "break;"+
				        "}"+
			        "}"+
		        "} else {"+
			        "throwError(place, \"<<\" + macroName + \">>: There are several items with the same id \" + item.ID);"+
			        "return;"+
		        "}"+
	        "}"+
        "};",

        "macros.removeFromInv = {"+
            "handler: captionName (place, macroName, params, parser) {"+
		        "if ((params.length == 0) || (params.length > 2)) {"+
                    "throwError(place, \"<<\" + macroName + \">>: expecting one or two parameters.\");"+
			        "return;"+
		        "}"+

            "var existing_items_with_id = state.active.variables.inventory.filter(obj => {"+
            "return obj.ID === params[0]"+

                    "});"+
		        "if (existing_items_with_id.length == 0)"+
			        "return;"+

		        "if (params.length == 1) {"+
			        "for (var i in state.active.variables.inventory) {"+
				        "if (state.active.variables.inventory[i].ID == params[0]) {"+
					        "state.active.variables.inventory.splice(i, 1);"+
					        "break;"+
				        "}"+
			        "}"+
		        "} else if (params.length == 2) {"+
			        "for (var i in state.active.variables.inventory) {"+
				        "if (state.active.variables.inventory[i].ID == params[0]) {"+
					        "state.active.variables.inventory[i].amount = state.active.variables.inventory[i].amount - params[1];"+

					        "if (state.active.variables.inventory[i].amount <= 0) {"+
						        "state.active.variables.inventory.splice(i, 1);"+
					        "}"+
					        "break;"+
                        "}"+
                    "}"+
                "}"+
            "}"+
        "};",

        "macros.inv = {"+
            "handler: captionName (place, macroName, params, parser) {"+
                "if (state.active.variables.inventory.length == 0) {"+
                    "new Wikifier(place, 'nothing');"+
                "} else {"+

                    "var inv_str = \"<table border=\\\"1\\\"><tr><td>ID</td><td>name</td><td>category</td><td>owned</td>\";"+
                    "if (invSkill1Label.length > 0) inv_str +=\"<td>\" + invSkill1Label + \"</td>\";"+
                    "if (invSkill2Label.length > 0) inv_str +=\"<td>\" + invSkill2Label + \"</td>\";"+
                    "if (invSkill3Label.length > 0) inv_str +=\"<td>\" + invSkill3Label + \"</td>\";"+
			        "inv_str +=\"<td>image</td></tr>\";"+

			        "for (var i=0; i<state.active.variables.inventory.length; i++) {"+
				        "inv_str += \"<tr><td>\" + state.active.variables.inventory[i].ID + \"</td>\"+"+
				        "\"<td>\" + state.active.variables.inventory[i].name + \"</td>\"+"+
				        "\"<td>\" + state.active.variables.inventory[i].category + \"</td>\"+"+
				        "\"<td>\" + state.active.variables.inventory[i].owned + \"</td>\";"+

                        "if (invSkill1Label.length > 0) inv_str +=\"<td>\" + state.active.variables.inventory[i].skill1 + \"</td>\";"+
                        "if (invSkill2Label.length > 0) inv_str +=\"<td>\" + state.active.variables.inventory[i].skill2 + \"</td>\";"+
                        "if (invSkill3Label.length > 0) inv_str +=\"<td>\" + state.active.variables.inventory[i].skill3 + \"</td>\";"+
                        "inv_str += \"<td><img src=\\\"\" + state.active.variables.inventory[i].image + \"\\\"></td></tr>\";"+
			        "}"+
                    "inv_str +=\"</table>\";"+
			        "new Wikifier(place, inv_str);"+
		        "}"+
	        "}"+
        "};",


            "macros.emptyInv = { handler: captionName(place, macroName, params, parser){" +
                "state.active.variables.inventory = []}};"
        };

        public static string CLOTH_INIT = "::Cloth[script]";

        public static string[] CLOTH_CODE = { "" };

        public static string STATS_INIT = "::Stats[script]";

        public static string[] STATS_CODE = {
            "window.getStats = captionName () { return state.active.variables.stats; }"+

            "macros.initStats = {"+
                "handler: captionName(place, macroName, params, parser){"+
                    "state.active.variables.stats = [];"+
                    "for (var i = 0; i < stats.length; i++) {"+
                        "state.active.variables.stats.push(stats[i]);"+
                    "}"+
                "}"+
            "};",

            "macros.setStats = {"+
                "handler: captionName(place, macroName, params, parser) {"+
                    "if (params.length != 2) {"+
                        "throwError(place, \"<<\" + macroName + \">>: expects two parameters, stat id and value.\");"+
                        "return;"+
                    "}"+

                    "for (var i in state.active.variables.stats) {"+
                        "if (state.active.variables.stats[i].ID == params[0]) {"+
                            "state.active.variables.stats[i].value = params[1];"+
                            "break;"+
                        "}"+
                     "}"+
                "}"+
            "}",

            "macros.getStats = {"+
                "handler: captionName(place, macroName, params, parser) {"+
                "if (params.length != 1) {"+
                    "throwError(place, \"<<\" + macroName + \">>: expects stat id.\");"+
                    "return;"+
                "}"+

                "for (var i in state.active.variables.stats) {"+
                    "if (state.active.variables.stats[i].ID == params[0]) {"+
                        "return state.active.variables.stats[i].value;"+
                    "}"+
                "}"+
            "}"+
        "}",

        "macros.addStats = {"+
            "handler: captionName(place, macroName, params, parser){"+
                "if (params.length != 2) {"+
                    "throwError(place, \"<<\" + macroName + \">>: expects two parameters, stat id and value.\");"+
                    "return;"+
                "}"+

                "for (var i in state.active.variables.stats) {"+
                    "if (state.active.variables.stats[i].ID == params[0]) {"+
                        "state.active.variables.stats[i].value += params[1];"+
                        "break;"+
                    "}"+
                "}"+
            "}"+
        "}",

        "macros.stats = {"+
            "handler: captionName(place, macroName, params, parser) {"+
                "if (state.active.variables.stats.length == 0) {"+
                    "new Wikifier(place, 'No stats');"+
                "}"+
                "else {"+
                    "var stats_str = \"<table border=\\\"1\\\"><tr><td>ID</td><td>name</td><td>value</td></tr>\";"+

                    "for (var i = 0; i < state.active.variables.stats.length; i++) {"+
                        "stats_str += \"<tr>"+
                        "<td><img src=\" + state.active.variables.stats[i].image + \"></td>\" +"+
                        "\"<td>\" + state.active.variables.stats[i].ID + \"</td>\" +"+
                        "\"<td>\" + state.active.variables.stats[i].name + \"</td>\" +"+
                        "\"<td>\" + state.active.variables.stats[i].value + \"</td></tr>\";"+
                    "}"+
                    "stats_str += \"</table>\";"+
                    "new Wikifier(place, stats_str);"+
                "}"+
            "}"+
        "};"
        };

        public static string DAYTIME_INIT = "::Daytime[script]";

        public static string[] DAYTIME_CODE = { "" };

        public static string SHOP_INIT = "::Shop[script]";

        public static string[] SHOP_CODE = { "" };

        public static string MONEY_INIT = "::Money[script]";

        public static string[] MONEY_CODE = { "" };

        public static string JOBS_INIT = "::Jobs[script]";

        public static string[] JOBS_CODE = { "" };

        public static string CHARACTER_INIT = "::Character[script]";

        public static string[] CHARACTER_CODE = { "" };

        public static string CSS_INIT = "::";

        public static string[] CSS_CODE =
        {
            "::CSSStyle[scss stylesheet]",

            "/* classes for table layout */",
            "table.cloth {border: 1px solid black; table-layout:fixed; width: 100%; }",
            "td.cloth {overflow: hidden; text-align: center; vertical-align: middle;}",
            "th.cloth {}",

            "table.cloth_sidebar {border: 1px solid black; table-layout:fixed; width: 100%;}",
            "td.cloth_sidebar {overflow: hidden; text-align: center; vertical-align: middle;}",
            "th.cloth_sidebar {}",

            "table.inventory {border: 1px solid black; table-layout:fixed; width: 100%;}",
            "td.inventory {overflow: hidden; text-align: center; vertical-align: middle;}",
            "th.inventory {}",

            "table.inventory_sidebar {border: 1px solid black; table-layout:fixed; width: 100%;}",
            "td.inventory_sidebar {overflow: hidden; text-align: center; vertical-align: middle;}",
            "th.inventory_sidebar {}",

            "table.wardrobe {border: 1px solid black; table-layout:fixed; width: 100%;}",
            "td.wardrobe {overflow: hidden; text-align: center; vertical-align: middle;}",
            "th.wardrobe {}",

            "table.stats {border: 1px solid black; table-layout:fixed; width: 100%;}",
            "td.stats {overflow: hidden; text-align: center; vertical-align: middle;}",
            "th.stats {}",

            "table.stats_sidebar {border: 1px solid black; table-layout:fixed; width: 100%;}",
            "td.stats_sidebar {overflow: hidden; text-align: center; vertical-align: middle;}",
            "th.stats_sidebar {}",

            "table.shop {border: 1px solid black; table-layout:fixed; width: 100%;}",
            "td.shop {overflow: hidden; text-align: center; vertical-align: middle;}",
            "th.shop {}",

            "table.jobs {border: 1px solid black; table-layout:fixed; width: 100%;}",
            "td.jobs {overflow: hidden; text-align: center; vertical-align: middle;}",
            "th.jobs {}",
            "img.jobs {width:100%; max-width:100px;}",

            "table.character {border: 1px solid black; table-layout:fixed; width: 100%;}",
            "td.character {overflow: hidden;text-align: center; vertical-align: middle;}",
            "th.character {}",

            "table.character_sidebar {border: 1px solid black; table-layout:fixed; width: 100%;}",
            "td.character_sidebar {overflow: hidden;text-align: center; vertical-align: middle;}",
            "th.character_sidebar {}"
        };

    }
}
