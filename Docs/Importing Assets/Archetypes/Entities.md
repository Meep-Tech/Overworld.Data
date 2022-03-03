# [[Entity]].Type
**Base Archetype Class:** `Overworld.Data.Entity.Type`
**Porter Class:** `Overworld.Data.Entity.Porter`
**Archetype Sub-Folder:** `_entities`
## Valid File Configurations
### Single File:
- **Defaut Icon Image Only with Name:**
	- `[NAME].png`
- **Config Only (Invisible Entity):**
	- `*.json`
### Multi-File/Folder:
- **Name Implied from Default Icon Image:**
	- `[NAME].png`
	- `[NAME].json`
- **All Data In Config with Default Icon Image Provided:**
	- `*.png`
	- `*.json`
- **All Data In Config with Default Icon Image, other Icons, and other Animations Provided**: #Not_Yet_Implemented ; This method uses the default config (named `_config.json`) to try to import the Entity. For any missing Animations or Icons, it will check if one of the other provided configs handles it, and import that into the proper Archetype Sub-Folder of the current [[Mod Package]] as well.
	- `_config.json`: Main Config
	- `*.png`: Default Icon Image (optional)
	- `*.json` Animation Config
	- `*.png` Animation Frame 1
	- `*.png` Animation Frame 2
	- `*.png` Animation Frame 3
	- `*.json` Icon 1 Config
	- `*.png` Icon Image
	- `*.json` Icon 2 Config
	- `*.png` Icon Image 2
## Config Json
- `string: name`(optional): The name of the Archetype. Must be unique per-[[Mod Package]] and [[Base Archetype]].
- `string: packageName`(optional): The package this Archetype is from.
- `string: description`(optional): A decription of this Entity Type.
- `string[]: tags`(optional): Tags used to find and describe this Entity.Type
- `string: archetype`(optional): A base Entity.Type Archetype to use as a Template instead of the class 'Entity.Type'.
- `string: defaultSpriteImageFile`(optional): An [[Image Lookup String]] for finding, potentially up/downloading, and setting the Default [[Entity Icon]].
- `object<object<*>[]> | string[]: initialModelComponents`(optional): [[Entity Component|Components]] to add to each of the Entity models created from the resulting Archetype. This field can either be an array of Component Keys, or an Object with each Property Key being the Component Key, and each Property Value being an Object representing the initial data of the Component as JSON, or null/""(empty string) to represent that it should use default settings to construct the Component.
- `object<object<*>[]> | string[]: initialArchetypeComponents`(optional): [[Entity Component|Components]] to add to the resulting Archetype. This field can either be an array of Component Keys, or an Object with each Property Key being the Component Key, and each Property Value being an Object representing the initial data of the Component as JSON, or null/""(empty string) to represent that it should use default settings to construct the Component.
- `object<string[]>: spriteIcons`(optional): [[Entity Icon|Icons]] with acompanying tags to add to this Entity.
- `object<string[]>: spriteAnimations`(optional): [[Entity Animations|Animations]] with acompanying tags to add to this Entity.
- `object<string[]>: otherSpriteDisplayOptions`(optional): other non Animation or Icon [[Entity Sprite Display Option]]s to add to this Entity.