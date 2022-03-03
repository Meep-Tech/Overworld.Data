# Tile.Type
**Archetype Sub-Folder:** `_tiles`
## Valid File Configurations
### Single File:
- **Image Only with Name:**
	- `[NAME].png`
- **Config Only:**
	- `*.json`
### Multi-File/Folder:
- **Name Implied:**
	- `[NAME].png`
	- `[NAME].json`
- **All Data In Config:**
	- `*.png`
	- `*.json`
## Config Json
- `string: name`(optional): The name of the Archetype. Must be unique per-[[Mod Package]] and [[Base Archetype]].
- `string: packageName`(optional): The package this Archetype is from.
- `string: description`(optional): A decription of this Tile Type.
- `string[]: tags`(optional): Tags used to find and describe this Tile.Type
- `int: pixelsPerTileDiameter`(optional): The diameter of a single Tile in Pixels. This is for automatically splitting a larger Tile Sheet Image Asset into multiple Tile Types. See `sizeInTiles` for an alternative. By default, this uses the currently set [[World]] tile size.
- `Vector2Int: sizeInTiles`(optional): Alternative to `pixelsPerTileDiameter` for specifying the dimensions of a Tile Sheet Image Asset using the dimensions of the tiles it needs to be cropped into instead of the size of each tile. `x` is width, and `y` is height.
- `enum: mode`(optional): The import mode can be specified here. The valid options are:
	- `"Individual"`: This config represents one(or no) Image Asset containing just one Tile.Type 
	- `"Sheet"`: This config represents a Tile Sheet Image Asset that needs to be split into multiple Tile.Types depending on an `pixelsPerTileDiameter` or `sizeInTiles` value
	- `"Animation"`: #Not_Yet_Implemented; This config represents a Tile Sheet Image Asset or multiple individual Image Assets that need to be combined into an Animated Tile.Type.
	
**Note:** Including any of the following 'Special Values' will make this Tile import produce two sperate Tile.Type [[Archetype]]s. One will be just the Tile's background image for appearence painting purposes; The 'Basic' or 'Background' Archetype, and the other will be the full Tile Archetype; The 'Special' Archetype, with the applied 'Special Values' as default settings; such as Hooks and Height value, that will override any current 'Special Values' set to the In-Game Tile.
- `float: height`(optional): A default Height Map value to give to all Tile.Types created.
- `bool: useDefaultBackgroundAsInWorldTileImage`(optional): Allows you to make invisible Tile.Types that still have an icon representation in the World Editor when set to false. This setting makes it so that when this Tile.Type is placed, the image representing the Tile.Type is not placed in the [[World]]. Defaults to `true`.
- `string: specialDescription`(optional): #Not_Yet_Implemented; A custom description just for the "Special" Archetype
- `string[]: specialTags`(optional): #Not_Yet_Implemented; Additional [[Tag]]s just for the "Special" Archetype.
- `bool: dontUseBackgroundAsInWorldTileImage`(optional): #Not_Yet_Implemented; Set this to true if you don't want the provided associated Image file to be used as an in-world background for the Tile Archetype being created. Instead, the image will just be used for the Archetype's Icon in the World Editor.
- `bool: dontCreateSeperateBackgroundArchetype`(optional): #Not_Yet_Implemented; Set this to true if you want to only create the Special Archetype, and not the Background Archetype as described in the **Note** above.