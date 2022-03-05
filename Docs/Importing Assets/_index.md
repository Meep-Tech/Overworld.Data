All imported Assets in [[../Overworld]] are controlled by [[Archetype]]s imported along with them. [[Porter]]s are the entities used in Code and In-Game to import, process, package, and export [[Archetype]]s, Assets, and all other data files for [[Mod Package]]s and [[Plugin]]s.

By default. Archetypes that are imported are packaged into the [[Mods Folder]] under a custom [[Mod Package]] named in the style: "\[USERNAME]'s Custom Assets". Pre-Processed versions of all imported files are also placed under the `__processed_imports` file within the [[Mods Folder]]. Make sure to clean out this file when you are done!

# Importing Mods
Archetypes can be imported In-Game or via the [[Overworld Chat Launcher]].
## In-Game
Importing in a Game [[World]] can be done in a few ways using [[World Editor]] mode:
- All platforms support selecting one or multiple files via a file explorer promt.
- Some platforms support dragging and dropping groups of files or individual files.
All platforms also allow you to change the Archetype Upload settings in the editor to Batch Mode or Single Mode.
### Single Mode
Single mode assumes all files being uploaded belong to just one Archetype.
### Batch Mode
Batch mode assumes any files or folders being uploaded could contain multiple Archetypes. In the case of a set of multiple files, the program will try to match each [[config.json]] file to a set of assets, and then will go through the remaining assets in order as if they were each an Archetype. This last step may vary depending on the type of Archetype being uploaded though. When folders are uploaded via a batch, the batch assumes each folder is an individual Archetype first, and then may use un-processed files for other Archetypes later that request them. This means 'loose' Assets with no associated config within these nested sub-folders will be ignored by default.
See [[#Mod Files Folder Structure]] below for details on how mods are uploaded in a from a folder structure.
## Import via Launcher
Importing via the Launcher is similar to [[#In-Game]], but is done in a special [[Archetype Examiner]] menu. Batch or Single upload are both available here as well. Only some platforms support drag and drop. 
## Import via Mod Folders
Importing via the [[Mods Folder]]'s `__imports` sub-folder is probably the quickest way to upload multiple Archetypes/Assets at once. This is done by placing 'loose' Archetypes inside of the appropriate Archetype sub-folder (Ex: `_entities`), and Archetypes you want packaged together specifically as folders with the items to import within them. 

Folders that start with `.` and Non-Special or Non-Archetype Sub-Folders that begin with `_` will have their contents ignored.
### Mod Files Folder Structure
Importing via the [[Mods Folder]] supports both 'loose' Archetypes, and sub-folders of Archetypes and Packages.

All Sub-Folders are contained within the `mods` folder. This is located at the Unity Persistent Data Path (on windows currently: `C:\Users\[USERNAME]\AppData\LocalLow\DefaultCompany\Overworld\mods`).

The `mods` folder itself can contain several special sub-folders and files:
- `__imports/`: This is the folder where new Archetypes that need to be imported and re-packaged into [[Mod Package]]s need to go. This folder's sub structre are the same as the parent `mods` folder, aside from there being no `__processed_imports` sub-folder here. Loose Assets that are placed within multiple levels of Sub-Folders will automatically be packaged into a Mod Package with the name corelating to the oath of the folders, aside from the folder containing the assets itself. The imports folder ca also contain it's own Archetype Sub Folders similar to a Mod Package. These folders have the naming sheme:  `_[LOOSE-ARCHETYPE-SUB-FOLDER-NAME]/` . Assets within these loose folders will be added to your default Mod Package when imported.
- `__processed_imports/`: This is where the pre-processed files from `__imports` are placed after they are re-packaged into Mod Packages and placed in the root `mods` folder. 
- `[PACKAGE-NAME]/` : These are [[Mod Package]]s that have not been zipped into .modpak files. These are loaded as they are and are not re-packaged by the importer. If you place packages here without putting them though the `__imports` folder first, it may slow down start-up time for certain [[World]]s that use your mod.
- `[PACKAGE-NAME].modpak`: These are the same as the above [[Mod Package]], but they have been zipped up and compressed.
#### Mod Package Folder Structure
Mod packages are seperated into [[Archetype Sub-Folder]]s with the naming scheem:
`_[ARCHETYPE-SUB-FOLDER-NAME]/`. Any assets not within these Sub-Folders in a [[Mod Package]] will be ignored. 
Each Archetype Sub-Folder can contain Loose Assets and [[config.json]] files, or more nested [[Single Archetype Folder|Single Archetype Sub-Folders]].
Assets with no associated Config.Json within these sub-folders will be ignored by default, and will not be processed as 'loose' Archetypes, like they would be if placed in the root of the Archetype Sub Folder.
# Default Naming of Imported Archetypes
Archetypes can be provided a specific "name" via their [[config.json]]. If no name is provided, the name will be taken from the name of the dominant asset used to make this Archetype (usually other than the config.json), or from the name of the current folder if the asset is in a non-special Sub-Folder of either the [[Mods Folder]] or a [[Mod Package]].
## Package Names
Archetypes can be provided a specific package to be added to via their [[config.json]].
Package names are taken, by default; from the parent-most folder that imported assets are placed in. Loose assets that go though the import and packaging proccess are  placed into a [[Mod Package]] folder under the [[Mods Folder]] root with the naming scheme: "\[USERNAME]'s Custom Assets"
# Importing Plugins
[[Plugin]]s are C# Classes and Code where one can directly define logic for [[Component]]s and [[Archetype]]s. Plugins must be imported though the [[Overworld Chat Launcher]] menu, and can be enabled in [[World]]s individually. [[World]]s can also allow remote plugins, and will have options to allow users to request their plugin be added to a [[World]] by an [[Chat Moderator]].