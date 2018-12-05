# TweeFly
A setup tool for interactive stories writte in Twee.

### Implemented:
- Display stats in sidebar vs display stats link in sidebar
- Display inventory in sidebar vs display inventory link in sidebar
- Display cloth in sidebar vs display cloth link in sidebar
- Bug: Add Items to Shop Combobox
- Add "description" to item and cloth
- Add "description" to jobs
- Added opening hours to shops
- Added "iswornatbeginning"
- Replace label edit fields from tabs and add to localization
- Replace skill1,2,3 edit fields from taby by localization
- Added "say" macro to characters

### TODO:
- GUI
	- Remove "mayOwnMultiple" for cloth
	- Add number, string, bool to skill1-3
	- Add configuration tab
	
- Free2Pro:
	- Add main file
	- Add new captions to inventory
				captions.Add(new CaptionPair("INVENTORY_COL_SHOP_CATEGORY_CAP", "shop category"));
                captions.Add(new CaptionPair("INVENTORY_COL_CAN_BUY_CAP", "can be bought"));
                captions.Add(new CaptionPair("INVENTORY_COL_BUY_PRICE_CAP", "buy price"));
                captions.Add(new CaptionPair("INVENTORY_COL_SELL_PRICE_CAP", "sell price"));
                captions.Add(new CaptionPair("INVENTORY_COL_CAN_OWN_MULTIPLE_CAP", "sell price"));

- Example project
	- Add "_" to all variables used by the scripts
	- Add macros to each