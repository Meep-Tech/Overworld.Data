using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Overworld.Data.IO {

  /// <summary>
  /// used to im/export archetypes of a specific type from mods
  /// </summary>
  public abstract class ArchetypePorter<TArchetype> : IArchetypePorter
    where TArchetype : Meep.Tech.Data.Archetype, IPortableArchetype {

    /// <summary>
    /// Key for the name value in the config
    /// </summary>
    public const string NameConfigKey = "name";

    /// <summary>
    /// Key for the package name value in the config
    /// </summary>
    public const string PackageNameConfigKey = "packageName";

    /// <summary>
    /// The default package name for archetyps of this type
    /// </summary>
    public abstract string DefaultPackageName {
      get;
    }

    /// <summary>
    /// Keys that work for options for imports.
    /// </summary>
    public virtual HashSet<string> ValidImportOptionKeys
      => new() {
        IArchetypePorter.NameOverrideSetting,
        IArchetypePorter.MoveFinishedFilesToFinishedImportsFolderSetting
      };

    /// <summary>
    /// Valid Keys for the config.json
    /// </summary>
    public virtual HashSet<string> ValidConfigOptionKeys
      => new() {
        NameConfigKey,
        PackageNameConfigKey
      };

    /// <summary>
    /// The user in control of the current game, and imports.
    /// </summary>
    public User CurrentUser {
      get;
    }

    /// <summary>
    /// The cached archetypes of this kind, by resource id
    /// </summary>
    readonly Dictionary<string, TArchetype> _cachedResources
      = new();

    /// <summary>
    /// The cached archetypes of this kind, by package name then resource id.
    /// </summary>
    readonly Dictionary<string, Dictionary<string, TArchetype>> _cachedResourcesByPackage
      = new();
    /// <summary>
    /// Make a new type of archetype porter with inheritance
    /// </summary>
    protected ArchetypePorter(User currentUser) {
      CurrentUser = currentUser;
    }

    /// <summary>
    /// Used to import arhetypes of this kind from one uploaded file
    /// </summary>
    protected abstract IEnumerable<TArchetype> _importArchetypesFromExternalFile(
      string externalFileLocation,
      string resourceKey,
      string name,
      string packageKey = null,
      Dictionary<string, object> options = null
    );


    /// <summary>
    /// Used to import arhetypes of this kind from multiple uploaded files
    /// </summary>
    protected virtual IEnumerable<TArchetype> _importArchetypesFromExternalFiles(
      string[] externalFileLocations,
      string resourceKey,
      string name,
      string packageKey = null,
      Dictionary<string, object> options = null
    ) => throw new NotSupportedException();

    /// <summary>
    /// Serialize this archetype to a set of files in the mod folder.
    /// </summary>
    /// <param name="archetype">The archetype to serialize into a file or files</param>
    /// <param name="packageDirectoryPath">The root path to save files to for this archetype</param>
    /// <returns>The newly serialized file's locations</returns>
    protected abstract string[] _serializeArchetypeToModFiles(TArchetype archetype, string packageDirectoryPath);

    ///<summary><inheritdoc/></summary>
    public string[] SerializeArchetypeToModFolder(TArchetype archetype)
      => _serializeArchetypeToModFiles(
        archetype,
        GetFolderForArchetype(archetype)
      );

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="resourceKey"></param>
    /// <returns></returns>
    public TArchetype TryToGetGetCachedArchetype(string resourceKey)
      => _cachedResources.TryGetValue(resourceKey, out TArchetype found)
         ? found
         : null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="resourceKey"></param>
    /// <returns></returns>
    public TArchetype GetCachedArchetype(string resourceKey)
      => _cachedResources[resourceKey];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TArchetype TryToFindArchetypeAndLoadFromModFolder(string resourceKey, Dictionary<string, object> options = null) {
      string modFolder = GetFolderForModItem(resourceKey, out string resourceName, out string packageName);

      // escape safely early
      if(!Directory.Exists(modFolder)) {
        return null;
      }

      string[] effectedFiles = Directory.GetFiles(modFolder);
      TArchetype archetype
        = _importArchetypesFromExternalFiles(effectedFiles, resourceKey, resourceName, packageName, options)
          .First();

      if(options is not null
        && options.TryGetValue(IArchetypePorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
        && (bool)moveFiles
      ) {
        _moveFilesToFinishedImportsFolder(archetype.AsSingleItemEnumerable(), effectedFiles, packageName, options);
      }
      _cacheArchetype(archetype, packageName);


      return archetype;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TArchetype LoadArchetypeFromModFolder(string resourceKey, Dictionary<string, object> options = null) {
      string modFolder = GetFolderForModItem(resourceKey, out string name, out string packageName);

      string[] effectedFiles = Directory.GetFiles(modFolder);
      TArchetype archetype
        = _importArchetypesFromExternalFiles(effectedFiles, resourceKey, name, packageName, options)
          .First();

      if(options is not null
        && options.TryGetValue(IArchetypePorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
          ? (bool)moveFiles
          : false
      ) {
        _moveFilesToFinishedImportsFolder(archetype.AsSingleItemEnumerable(), effectedFiles, packageName, options);
      }
      _cacheArchetype(archetype, packageName);


      return archetype;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<TArchetype> ImportAndBuildNewArchetypeFromFile(string externalFileLocation, Dictionary<string, object> options = null) {
      string name = options is not null && options.TryGetValue(IArchetypePorter.NameOverrideSetting, out var nameObj)
         ? (string)nameObj
         : null;
      string packageName = options is not null && options.TryGetValue(IArchetypePorter.PagkageNameOverrideSetting, out var pkgNameObj)
         ? (string)pkgNameObj
         : null;

      string resourceKey = GetResourceKeyFromFileLocationAndSettings(externalFileLocation, ref packageName, ref name);
      if(_cachedResources.ContainsKey(resourceKey)) {
        int incrementor = 0;
        string fixedKey;
        do {
          fixedKey = resourceKey + $" ({++incrementor})";
        } while(_cachedResources.ContainsKey(fixedKey));

        resourceKey = fixedKey;
      }

      List<TArchetype> archetypes
        = _importArchetypesFromExternalFile(externalFileLocation, resourceKey, name, packageName, options)
          .ToList();

      if(options is not null
        && options.TryGetValue(IArchetypePorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
          ? (bool)moveFiles
          : false
      ) {
        _moveFilesToFinishedImportsFolder(archetypes, new string[] { externalFileLocation }, packageName, options);
      }

      foreach(TArchetype archetype in archetypes) {
        _cacheArchetype(archetype, packageName);
      }

      return archetypes;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<TArchetype> ImportAndBuildNewArchetypeFromFolder(string externalFolderLocation, Dictionary<string, object> options) {
      string name = options is not null && options.TryGetValue(IArchetypePorter.NameOverrideSetting, out var nameObj)
         ? (string)nameObj
         : null;

      if(name is null) {
        name = Path.GetDirectoryName(externalFolderLocation);
      }

      string packageName = null;
      string resourceKey = GetResourceKeyFromFileLocationAndSettings(externalFolderLocation, ref packageName, ref name);
      if(_cachedResources.ContainsKey(resourceKey)) {
        int incrementor = 0;
        string fixedKey;
        do {
          fixedKey = resourceKey + $" ({++incrementor})";
        } while(_cachedResources.ContainsKey(fixedKey));

        resourceKey = fixedKey;
      }

      string[] effectedFiles = Directory.GetFiles(externalFolderLocation);
      IEnumerable<TArchetype> archetypes = _importArchetypesFromExternalFiles(effectedFiles, resourceKey, name, packageName, options);

      if(options is not null
        && options.TryGetValue(IArchetypePorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
          ? (bool)moveFiles
          : false
      ) {
        _moveFilesToFinishedImportsFolder(archetypes, effectedFiles, packageName, options);
      }
      foreach(TArchetype archetype in archetypes) {
        _cacheArchetype(archetype, packageName);
      }


      return archetypes;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<TArchetype> ImportAndBuildNewArchetypeFromFiles(string[] externalFileLocations, Dictionary<string, object> options) {
      string name = options is not null && options.TryGetValue(IArchetypePorter.NameOverrideSetting, out var nameObj)
         ? (string)nameObj
         : null;

      string defaultNameFile = externalFileLocations.First(fileName => fileName != IArchetypePorter.ConfigFileName);
      string packageName = null;
      string resourceKey = GetResourceKeyFromFileLocationAndSettings(defaultNameFile, ref packageName, ref name);
      if(_cachedResources.ContainsKey(resourceKey)) {
        int incrementor = 0;
        string fixedKey;
        do {
          fixedKey = resourceKey + $" ({++incrementor})";
        } while(_cachedResources.ContainsKey(fixedKey));

        resourceKey = fixedKey;
      }

      IEnumerable<TArchetype> archetypes
        = _importArchetypesFromExternalFiles(externalFileLocations, resourceKey, name, packageName, options);

      if(options is not null
        && options.TryGetValue(IArchetypePorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
        && (bool)moveFiles
      ) {
        _moveFilesToFinishedImportsFolder(archetypes, externalFileLocations, packageName, options);
      }

      foreach(TArchetype archetype in archetypes) {
        _cacheArchetype(archetype);
      }

      return archetypes;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string GetFolderForArchetype(IPortableArchetype portableArchetype) {
      TArchetype archetype = (TArchetype)portableArchetype;
      return GetFolderForModItem(archetype.Id.Name, archetype.PackageName);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string GetFolderForModItem(string resourceKey, out string resourceName, out string packageName) {
      packageName = null;
      string[] keyParts = resourceKey.Split("::");
      if(keyParts.Length == 1) {
        resourceName = resourceKey;
      } else if(keyParts.Length == 2) {
        resourceName = keyParts[1];
        packageName = keyParts[0];
      } else
        throw new ArgumentException($"'::' cannot be used in backage names or resource names");
      return GetFolderForModItem(resourceName, packageName);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string GetFolderForModItem(string name, string packageName = null) {
      string modFolder = Path.Combine(Application.persistentDataPath, IArchetypePorter.ModFolderName);
      if(packageName is null) {
        modFolder = Path.Combine(modFolder, DefaultPackageName, name.Replace(".", "/"));
      } else {
        modFolder = Path.Combine(modFolder, packageName.Replace(".", "/"), DefaultPackageName, name.Replace(".", "/"));
      }

      return modFolder;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool TryToMoveRenamedArchetypeFolder(string oldName, IPortableArchetype archetype) {
      string newFolderName = GetFolderForArchetype(archetype);

      if(System.IO.Directory.Exists(newFolderName)) {
        return false;
      }

      string oldFolderName = GetFolderForModItem(oldName, archetype.PackageName);
      Directory.CreateDirectory(newFolderName);
      _copyDirectory(oldFolderName, newFolderName, true);
      return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void ForceMoveRenamedArchetypeFolder(string oldName, IPortableArchetype archetype) {
      string newFolderName = GetFolderForArchetype(archetype);

      /// empty the target folder if it already exists.
      if(System.IO.Directory.Exists(newFolderName)) {
        DirectoryInfo directoryInfo = new(newFolderName);
        directoryInfo.EnumerateFiles().ForEach(file => file.Delete());
        directoryInfo.EnumerateDirectories().ForEach(subDirectory => subDirectory.Delete(true));
      }

      string oldFolderName = GetFolderForModItem(oldName, archetype.PackageName);
      Directory.CreateDirectory(newFolderName);
      _copyDirectory(oldFolderName, newFolderName, true);
    }

    /// <summary>
    /// Used to make a new key for a new resouce made by the current user
    /// </summary>
    public virtual string GetResourceKeyFromFileLocationAndSettings(string externalFileLocation, ref string packageName, ref string name) {
      string key = "";
      string packageFolderKey = "";
      string nameFolderKey = "";
      if(packageName is null || name is null) {
        var currentFolder = new DirectoryInfo(externalFileLocation);

        if(name is null) {
          while(currentFolder.Name != DefaultPackageName) {
            nameFolderKey = currentFolder.Name + "." + nameFolderKey;
            currentFolder = currentFolder.Parent;
          }
        }

        if(packageName is null
          && currentFolder.Parent.Name != IArchetypePorter.ImportFolderName
          && currentFolder.Parent.Name != IArchetypePorter.ModFolderName
        ) {
          currentFolder = currentFolder.Parent;
          while(currentFolder.Parent.Name != IArchetypePorter.ImportFolderName
            && currentFolder.Parent.Name != IArchetypePorter.ModFolderName) {
            packageFolderKey = currentFolder.Name + "." + packageFolderKey;
            currentFolder = currentFolder.Parent;
          }
        }

        packageName ??= packageFolderKey.Trim('.');
        name ??= nameFolderKey.Trim('.');
      }

      if(!string.IsNullOrWhiteSpace(packageName)) {
        key += packageName + "::";
      }

      return key + (name = Path.GetFileNameWithoutExtension(name ?? externalFileLocation));
    }

    /// <summary>
    /// Correct package name, resource key, etc according to the config values:
    /// </summary>
    protected string CorrectBaseKeysAndNamesForConfigValues(string externalFileLocation, ref string name, ref string packageKey, JObject config) {
      name = config.TryGetValue(NameConfigKey, out JToken value)
        ? value.Value<string>()
        : name;
      packageKey = config.TryGetValue(PackageNameConfigKey, out JToken value2)
        ? value2.Value<string>()
        : packageKey;
      return GetResourceKeyFromFileLocationAndSettings(
         externalFileLocation,
         ref packageKey,
         ref name
       );
    }

    static void _copyDirectory(string sourceDir, string destinationDir, bool recursive) {
      // Get information about the source directory
      var dir = new DirectoryInfo(sourceDir);

      // Check if the source directory exists
      if(!dir.Exists)
        throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

      // Cache directories before we start copying
      DirectoryInfo[] dirs = dir.GetDirectories();

      // Create the destination directory
      Directory.CreateDirectory(destinationDir);

      // Get the files in the source directory and copy to the destination directory
      foreach(FileInfo file in dir.GetFiles()) {
        string targetFilePath = Path.Combine(destinationDir, file.Name);
        file.CopyTo(targetFilePath);
      }

      // If recursive and copying subdirectories, recursively call this method
      if(recursive) {
        foreach(DirectoryInfo subDir in dirs) {
          string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
          _copyDirectory(subDir.FullName, newDestinationDir, true);
        }
      }
    }

    void _cacheArchetype(TArchetype archetype, string packageName = null) {
      _cachedResources.Add(archetype.ResourceKey, archetype);
      if(_cachedResourcesByPackage.TryGetValue(packageName ?? "", out var existingSet)) {
        existingSet.Add(archetype.ResourceKey, archetype);
      } else if(!string.IsNullOrWhiteSpace(packageName)) {
        _cachedResourcesByPackage.Add(packageName, new() {
          {
            archetype.ResourceKey,
            archetype
          }
        });
      }
    }

    void _moveFilesToFinishedImportsFolder(IEnumerable<TArchetype> compiledArchetypes, string[] fileNames, string packageName = null, Dictionary<string, object> options = null) {
      /// Save files that are re-compiled for speed to the mod folder:
      foreach(TArchetype compiled in compiledArchetypes) {
        _serializeArchetypeToModFiles(compiled, GetFolderForArchetype(compiled));
      }

      /// Move the old files to exports
      string exportFolder
        = Path.Combine(Application.persistentDataPath, IArchetypePorter.ModFolderName, IArchetypePorter.FinishedImportsFolderName, packageName ?? compiledArchetypes.First().DefaultPackageName);
      if(packageName is not null) {
        exportFolder = Path.Combine(exportFolder, compiledArchetypes.First().DefaultPackageName);
      }

      // Move each untouched file to output:
      Directory.CreateDirectory(exportFolder);
      foreach(string fileName in fileNames) {
        System.IO.File.Move(fileName, Path.Combine(exportFolder, Path.GetFileName(fileName)));
        // TODO: these any file lookups could probably be quicker:
        if(!Directory.GetParent(fileName).GetFiles().Any()) {
          if(Directory.GetParent(fileName).Name == IArchetypePorter.ImportFolderName) {
            throw new Exception($"Folder deleting wrong");
          }
          Directory.GetParent(fileName).Delete();
        }

        if(packageName is not null) {
          if(!Directory.GetParent(fileName).GetFiles().Any() && !Directory.GetParent(fileName).GetDirectories().Any()) {
            if(Directory.GetParent(fileName).Name == IArchetypePorter.ImportFolderName) {
              throw new Exception($"Folder deleting gone wrong");
            }
            Directory.GetParent(fileName).Parent.Delete();
          }
        }
      }
    }

    #region IPorter

    IEnumerable<Archetype> IArchetypePorter.ImportAndBuildNewArchetypeFromFile(string externalFileLocation, Dictionary<string, object> options)
      => ImportAndBuildNewArchetypeFromFile(externalFileLocation, options);

    IEnumerable<Archetype> IArchetypePorter.ImportAndBuildNewArchetypeFromFolder(string externalFolderLocation, Dictionary<string, object> options)
      => ImportAndBuildNewArchetypeFromFolder(externalFolderLocation, options);

    IEnumerable<Archetype> IArchetypePorter.ImportAndBuildNewArchetypeFromFiles(string[] externalFileLocations, Dictionary<string, object> options)
      => ImportAndBuildNewArchetypeFromFiles(externalFileLocations, options);
    Archetype IArchetypePorter.GetCachedArchetype(string resourceKey)
      => GetCachedArchetype(resourceKey);

    Archetype IArchetypePorter.TryToGetGetCachedArchetype(string resourceKey)
      => TryToGetGetCachedArchetype(resourceKey);

    Archetype IArchetypePorter.LoadArchetypeFromModFolder(string resourceKey, Dictionary<string, object> options)
      => LoadArchetypeFromModFolder(resourceKey, options);

    Archetype IArchetypePorter.TryToFindArchetypeAndLoadFromModFolder(string resourceKey, Dictionary<string, object> options)
      => TryToFindArchetypeAndLoadFromModFolder(resourceKey, options);

    string[] IArchetypePorter.SerializeArchetypeToModFolder(Archetype archetype)
      => SerializeArchetypeToModFolder((TArchetype)archetype);

      /// <summary>
      /// Try to get the _config.json from the set of provided files.
      /// </summary>
      protected JObject TryToGetConfig(IEnumerable<string> externalFileLocations, out string configFileName) {
        configFileName = externalFileLocations
          .FirstOrDefault(fileName => fileName == IArchetypePorter.ConfigFileName);
        if(configFileName is null) {
          configFileName = externalFileLocations
            .FirstOrDefault(fileName => Path.GetExtension(fileName).ToLower() == ".json");
        }
        if(configFileName is not null && File.Exists(configFileName)) {
          return JObject.Parse(
            File.ReadAllText(configFileName)
          );
        } else
          return new JObject();
      }


    #endregion
  }
}
