# TweeFly
A setup tool for interactive stories writte in Twee2 and Twine.

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
- Added "renameCharacter" macro to characters

### Release notes:
	- 0.13
		- Bugfixes
			- missing TR tag in clothing sideview - fixed
			- Two currencly signs appeared in sidebar - fixed
		- Beta features
			- First draft of cheat sheet

	- 0.14
		- New features:
			- setNextDay: Sets date to the next day. Parameters are hours and minutes.
			- addToInventory: Added optional 3rd parameter. If True: A short message that an item was received is shown. Added new caption to configuration: INVENTORY_RECEIVED_ITEM_CAP
			- removeFromInventory: Added optional 3rd parameter. If True: A short message that an item was removed is shown. Added new caption to configuration: INVENTORY_REMOVED_ITEM_CAP
			- addToWardrobe: Added optional 3rd parameter. If True: A short message that clothing was received is shown. Added new caption to configuration: WARDROBE_RECEIVED_CLOTHING_CAP
			- (new function) removeFromWardrobe: Added optional 3rd parameter. If True: A short message that clothing was removed is shown. Added new caption to configuration: WARDROBE_REMOVED_CLOTHING_CAP
			- Tables in sidebar are now collapsable/expandable
			- Jobs can forward to a passage
		- Improvements:
			- Item, clothing, character ... images are shown as first item in each table now
			- Path subtract has been replaced with path prefix. Path prefix is a relative path to the story where images are stored.
### TODO:
- GUI
	- Postponed: Remove "mayOwnMultiple" for cloth
	- Postponed: Add number, string, bool to skill1-3
	- Postponed: make lists sortable
	
- Macros:
	- Display boolean values as images.
	- Activate/Deactivate menus and links in sidebar (e.g. to inventory)
	- Add change relation to player for characters
	- Inventory Mouse Over - Show only description
	- Remove distance between dialogs in tweefly	
	- Add macro to display images. Pro feature: convert image to base64
	- Replace subtraction path by prefix
	
- Example project
	- Add "_" to all variables used by the scripts
	- Add macros to each
	
- Bugs:
	- Tooltips for cloth
	- cloth skill 1 not show in shops
	- "back" does not reset variables -> tweefly sidebar replace return with back
