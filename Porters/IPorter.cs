using Meep.Tech.Data;
using System.Collections.Generic;

namespace Overworld.Data {
  public interface IPorter {

    /// <summary>
    /// The base mod folder name
    /// </summary>
    public const string ModFolderName
      = "_mods";

    /// <summary>
    /// The imports folder name
    /// </summary>
    public const string ImportFolderName
      = "_imports";

    /// <summary>
    /// The finished imports folder name.
    /// </summary>
    public const string FinishedImportsFolderName
      = "_imports_finished";

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

    Archetype GetCachedArchetype(string resourceKey);
    Archetype TryToGetGetCachedArchetype(string resourceKey);
    Archetype LoadArchetypeFromModFolder(string resourceKey, Dictionary<string, object> options = null);
    Archetype TryToFindArchetypeAndLoadFromModFolder(string resourceKey, Dictionary<string, object> options = null);
    IEnumerable<Archetype> ImportAndBuildNewArchetypeFromFile(string externalFileLocation, Dictionary<string, object> options = null);
    IEnumerable<Archetype> ImportAndBuildNewArchetypeFromFiles(string[] externalFileLocations, Dictionary<string, object> options = null);
    IEnumerable<Archetype> ImportAndBuildNewArchetypeFromFolder(string externalFolderLocation, Dictionary<string, object> options = null);
  }
}