using Meep.Tech.Data;
using System.Collections.Generic;

namespace Overworld.Data {

  /// <summary>
  /// used to im/export archetypes from mods
  /// </summary>
  public interface IArchetypePorter {

    /// <summary>
    /// The base mod folder name
    /// </summary>
    public const string ModFolderName
      = "mods";

    /// <summary>
    /// The imports folder name
    /// </summary>
    public const string ImportFolderName
      = "__imports";

    /// <summary>
    /// The finished imports folder name.
    /// </summary>
    public const string FinishedImportsFolderName
      = "__exports";

    /// <summary>
    /// Option parameter to override the object name
    /// </summary>
    public const string NameOverrideSetting
      = "Name";

    /// <summary>
    /// Option parameter to Move the imported files to the finished imports folder.
    /// Accepts a bool
    /// </summary>
    public const string MoveFinishedFilesToFinishedImportsFolderSetting
      = "MoveImportedFilesToFinished";

    /// <summary>
    /// Option parameter to indicate there's no package name
    /// Accepts a bool
    /// </summary>
    public const string NoPackageName
      = "NoPackageName";

    /// <summary>
    /// The name of the config json file.
    /// </summary>
    public const string ConfigFileName = "_config.json";

    /// <summary>
    /// Get an already loaded archetype
    /// </summary>
    Archetype GetCachedArchetype(string resourceKey);

    /// <summary>
    /// Try to get an already loaded archetype
    /// </summary>
    Archetype TryToGetGetCachedArchetype(string resourceKey);

    /// <summary>
    /// get an archetype from the mods folder files
    /// </summary>
    Archetype LoadArchetypeFromModFolder(string resourceKey, Dictionary<string, object> options = null);

    /// <summary>
    /// Try to get an existing archetype from the compiled mod folder files.
    /// This doesn't throw if it finds no files, but may throw if the found files are invalid, or the archetype already exists.
    /// Returns null on failure to find.
    /// </summary>
    Archetype TryToFindArchetypeAndLoadFromModFolder(string resourceKey, Dictionary<string, object> options = null);

    /// <summary>
    /// Import a new archetype or archetypes from the external file location.
    /// </summary>
    IEnumerable<Archetype> ImportAndBuildNewArchetypeFromFile(string externalFileLocation, Dictionary<string, object> options = null);

    /// <summary>
    /// Import a new archetype or archetypes from the external collection of files.
    /// </summary>
    IEnumerable<Archetype> ImportAndBuildNewArchetypeFromFiles(string[] externalFileLocations, Dictionary<string, object> options = null);


    /// <summary>
    /// Import a new archetype from the external folder, full of files.
    /// </summary>
    IEnumerable<Archetype> ImportAndBuildNewArchetypeFromFolder(string externalFolderLocation, Dictionary<string, object> options = null);

    /// <summary>
    /// Get the sub folder under the mod folder on the device used for this specfic archetype,
    /// also splits up the key into it's parts
    /// </summary>
    string GetFolderForModItem(string resouceKey, out string resourceName, out string packageName);    

    /// <summary>
    /// Get the sub folder unther the mod folder on the device used for this specfic archetype
    /// </summary>
    string GetFolderForModItem(string resourceName, string packageName);

    /// <summary>
    /// Get the sub folder under the mod folder on the device used for this specfic archetype
    /// </summary>
    string GetFolderForArchetype(IPortableArchetype archetype);

    /// <summary>
    /// Serialize this archetype to a set of files in the mod folder.
    /// </summary>
    /// <param name="archetype">The archetype to serialize into a file or files</param>
    /// <returns>The newly serialized file's locations</returns>
    public string[] SerializeArchetypeToModFolder(Archetype archetype);

    /// <summary>
    /// Move an archetype from it's old name to a new folder with it's new name (within the same package)
    /// WARNING This overwrites any existing archetypes with the same name. Use try if you don't want to do this.
    /// </summary>
    void ForceMoveRenamedArchetypeFolder(string oldName, IPortableArchetype archetype);

    /// <summary>
    /// Move an archetype from it's old name to a new folder with it's new name (within the same package)
    /// This returns false if the file exists already, meaning there's already an archetype with the given key.
    /// </summary>
    bool TryToMoveRenamedArchetypeFolder(string oldName, IPortableArchetype archetype);
  }
}