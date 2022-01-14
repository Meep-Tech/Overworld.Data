using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Overworld.Data.IO {
  public abstract class ArchetypePorter<TArchetype> : IPorter
    where TArchetype : Meep.Tech.Data.Archetype, IPortable {

    /// <summary>
    /// The default package name for archetyps of this type
    /// </summary>
    public abstract string DefaultPackageName {
      get;
    }

    /// <summary>
    /// The cached archetypes of this kind, by resource id
    /// </summary>
    readonly Dictionary<string, TArchetype> _cachedResources
      = new Dictionary<string, TArchetype>();

    /// <summary>
    /// The cached archetypes of this kind, by package name then resource id.
    /// </summary>
    readonly Dictionary<string, Dictionary<string, TArchetype>> _cachedResourcesByPackage
      = new();

    /// <summary>
    /// The user in control of the current game, and imports.
    /// </summary>
    public User CurrentUser {
      get;
    }

    protected ArchetypePorter(User currentUser) {
      CurrentUser = currentUser;
    }

    protected abstract IEnumerable<TArchetype> _importArchetypesFromExternalFile(
      string externalFileLocation,
      string resourceKey,
      string name,
      string packageKey = null,
      Dictionary<string, object> options = null
    );

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

    /// <summary>
    /// Try to get an existing archetype
    /// </summary>
    public TArchetype TryToGetGetCachedArchetype(string resourceKey)
      => _cachedResources.TryGetValue(resourceKey, out TArchetype found)
         ? found
         : null;
      
    /// <summary>
    /// get an existing archetype
    /// </summary>
    public TArchetype GetCachedArchetype(string resourceKey)
      => _cachedResources[resourceKey];

    /*public TArchetype RequestArchetypeFromUser(string resourceKey, Dictionary<string, object> options = null)
      => throw new System.NotImplementedException("TODO: for network requests of data");*/

    /// <summary>
    /// Try to get an existing archetype from file
    /// </summary>
    /*public TArchetype TryToLoadExistingArchetypeFromModFolder(string resourceKey, Dictionary<string, object> options = null)
      => ImportAndBuildNewArchetypeFromFolder*/

    /// <summary>
    /// Try to get an existing archetype from the compiled mod folder files.
    /// This doesn't throw if it finds no files, but may throw if the found files are invalid, or the archetype already exists.
    /// Returns null on failure to find.
    /// </summary>
    public TArchetype TryToFindArchetypeAndLoadFromModFolder(string resourceKey, Dictionary<string, object> options = null) {
      string name;
      string packageName = null;
      string[] keyParts = resourceKey.Split("::");
      if(keyParts.Length == 1) {
        name = resourceKey;
      }
      else if(keyParts.Length == 2) {
        name = keyParts[1];
        packageName = keyParts[0];
      } else
        throw new ArgumentException($"'::' cannot be used in backage names or resource names");

      string modFolder = Path.Combine(Application.persistentDataPath, IPorter.ModFolderName);
      if(packageName is null) {
        modFolder = Path.Combine(modFolder, DefaultPackageName, name);
      }
      else {
        modFolder = Path.Combine(modFolder, packageName, DefaultPackageName, name);
      }

      // escape safely early
      if(!Directory.Exists(modFolder)) {
        return null;
      }

      string[] effectedFiles = Directory.GetFiles(modFolder);
      TArchetype archetype
        = _importArchetypesFromExternalFiles(effectedFiles, resourceKey, name, packageName, options)
          .First();

      if(options is not null
        && options.TryGetValue(IPorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
        && (bool)moveFiles
      ) {
        _moveFileToFinishedImportsFolder(archetype, effectedFiles, packageName, options);
      }        
      _cacheArchetype(archetype, packageName);


      return archetype;
    }

    /// <summary>
    /// get an existing archetype from the compiled mod folder files
    /// </summary>
    public TArchetype LoadArchetypeFromModFolder(string resourceKey, Dictionary<string, object> options = null) {
      string name;
      string packageName = null;
      string[] keyParts = resourceKey.Split("::");
      if(keyParts.Length == 1) {
        name = resourceKey;
      }
      else if(keyParts.Length == 2) {
        name = keyParts[1];
        packageName = keyParts[0];
      } else
        throw new ArgumentException($"'::' cannot be used in backage names or resource names");

      string modFolder = Path.Combine(Application.persistentDataPath, IPorter.ModFolderName);
      if(packageName is null) {
        modFolder = Path.Combine(modFolder, DefaultPackageName, name);
      }
      else {
        modFolder = Path.Combine(modFolder, packageName, DefaultPackageName, name);
      }

      string[] effectedFiles = Directory.GetFiles(modFolder);
      TArchetype archetype
        = _importArchetypesFromExternalFiles(effectedFiles, resourceKey, name, packageName, options)
          .First();

      if(options is not null
        && options.TryGetValue(IPorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
          ? (bool)moveFiles
          : false
      ) {
        _moveFileToFinishedImportsFolder(archetype, effectedFiles, packageName, options);
      }        
      _cacheArchetype(archetype, packageName);


      return archetype;
    }

    /// <summary>
    /// Import a new archetype from the external file location.
    /// </summary>
    public IEnumerable<TArchetype> ImportAndBuildNewArchetypeFromFile(string externalFileLocation, Dictionary<string, object> options = null) {
      string name = options is not null && options.TryGetValue(IPorter.NameOverrideSetting, out var nameObj)
         ? (string)nameObj
         : null;

      string packageName = null;
      if(!((options?.ContainsKey(IPorter.NoPackageName) ?? false) && (bool)options[IPorter.NoPackageName])) {
        string directoryName = new DirectoryInfo(Path.GetDirectoryName(externalFileLocation)).Name;
        if(!directoryName.StartsWith("-")) {
          packageName = directoryName;
        }
      }

      string resourceKey = _getNewResourceKeyFromFileNameAndSettings(externalFileLocation, packageName, ref name);
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
        && options.TryGetValue(IPorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
          ? (bool)moveFiles
          : false
      ) {
        foreach(TArchetype archetype in archetypes) {
          _cacheArchetype(archetype, packageName);
          _moveFileToFinishedImportsFolder(archetype, new string[] { externalFileLocation }, packageName, options);
        }
      }
      else {
        foreach(TArchetype archetype in archetypes) {
          _cacheArchetype(archetype, packageName);
        }
      }

      return archetypes;
    }

    /// <summary>
    /// Import a new archetype from the external folder location.
    /// </summary>
    public IEnumerable<TArchetype> ImportAndBuildNewArchetypeFromFolder(string externalFolderLocation, Dictionary<string, object> options) {
      string name = options is not null && options.TryGetValue(IPorter.NameOverrideSetting, out var nameObj)
         ? (string)nameObj
         : null;

      if(name is null) {
        name = Path.GetDirectoryName(externalFolderLocation);
      }

      string packageName = null;
      string directoryName = System.IO.Directory.GetParent(externalFolderLocation).Name;
      if(!directoryName.StartsWith("-")) {
        packageName = directoryName;
      }

      string resourceKey = _getNewResourceKeyFromFileNameAndSettings(externalFolderLocation, packageName, ref name);
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
        && options.TryGetValue(IPorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
          ? (bool)moveFiles
          : false
      ) {
        foreach(TArchetype archetype in archetypes) {
          _cacheArchetype(archetype, packageName);
          _moveFileToFinishedImportsFolder(archetype, effectedFiles, packageName, options);
        }
      }
      else {
        foreach(TArchetype archetype in archetypes) {
          _cacheArchetype(archetype, packageName);
        }
      }

      return archetypes;
    }

    /// <summary>
    /// Import a new archetype from the external folder location.
    /// </summary>
    public IEnumerable<TArchetype> ImportAndBuildNewArchetypeFromFiles(string[] externalFileLocations, Dictionary<string, object> options) {
      string name = options is not null && options.TryGetValue(IPorter.NameOverrideSetting, out var nameObj)
         ? (string)nameObj
         : null;

      string defaultNameFile = externalFileLocations.First(fileName => fileName != IPorter.ConfigFileName);
      string resourceKey = _getNewResourceKeyFromFileNameAndSettings(defaultNameFile, null, ref name);
      if(_cachedResources.ContainsKey(resourceKey)) {
        int incrementor = 0;
        string fixedKey;
        do {
          fixedKey = resourceKey + $" ({++incrementor})";
        } while(_cachedResources.ContainsKey(fixedKey));

        resourceKey = fixedKey;
      }

      IEnumerable<TArchetype> archetypes
        = _importArchetypesFromExternalFiles(externalFileLocations, resourceKey, name, null, options);

      if(options is not null
        && options.TryGetValue(IPorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
        && (bool)moveFiles
      ) {
        foreach(TArchetype archetype in archetypes) {
          _cacheArchetype(archetype);
          _moveFileToFinishedImportsFolder(archetype, externalFileLocations, null, options);
        }
      }
      else {
        foreach(TArchetype archetype in archetypes) {
          _cacheArchetype(archetype);
        }
      }

      return archetypes;
    }

    void _cacheArchetype(TArchetype archetype, string packageName = null) {
      _cachedResources.Add(archetype.ResourceKey, archetype);
      if(_cachedResourcesByPackage.TryGetValue(packageName ?? "", out var existingSet)) {
        existingSet.Add(archetype.ResourceKey, archetype);
      } else if (!string.IsNullOrWhiteSpace(packageName)) {
        _cachedResourcesByPackage.Add(packageName, new() {
          {
            archetype.ResourceKey,
            archetype
          }
        });
      }
    }

    /// <summary>
    /// Used to make a new key for a new resouce made by the current user
    /// </summary>
    protected virtual string _getNewResourceKeyFromFileNameAndSettings(string externalFileLocation, string packageName, ref string name) {
      string key = "";
      if(packageName is not null) {
        key += packageName + "::";
      }
      return key + (name ??= Path.GetFileNameWithoutExtension(externalFileLocation));
    }

    void _moveFileToFinishedImportsFolder(TArchetype compiled, string[] fileNames, string packageName = null, Dictionary<string, object> options = null) {
      string destinationPackage = Path.Combine(Application.persistentDataPath, IPorter.ModFolderName, IPorter.FinishedImportsFolderName, packageName ?? compiled.DefaultPackageName);
      if(packageName is not null) {
        destinationPackage = Path.Combine(destinationPackage, compiled.DefaultPackageName);
      }

      // save files that are re-compiled for speed to the mod folder:
      _serializeArchetypeToModFiles(compiled, destinationPackage);

      // Move each untouched file to output:
      foreach(string fileName in fileNames) {
        System.IO.File.Move(fileName, Path.Combine(destinationPackage, Path.GetFileName(fileName)));
        // TODO: these any file lookups could probably be quicker:
        if(!Directory.GetParent(fileName).GetFiles().Any()) {
          if(Directory.GetParent(fileName).Name == IPorter.ImportFolderName) {
            throw new Exception($"Folder deleting wrong");
          }
          Directory.GetParent(fileName).Delete();
        }

        if(packageName is not null) {
          if(!Directory.GetParent(fileName).Parent.GetFiles().Any()) {
            if(Directory.GetParent(fileName).Name == IPorter.ImportFolderName) {
              throw new Exception($"Folder deleting wrong");
            }
            Directory.GetParent(fileName).Parent.Delete();
          }
        }
      }
    }

    IEnumerable<Archetype> IPorter.ImportAndBuildNewArchetypeFromFile(string externalFileLocation, Dictionary<string, object> options)
      => ImportAndBuildNewArchetypeFromFile(externalFileLocation, options);

    IEnumerable<Archetype> IPorter.ImportAndBuildNewArchetypeFromFolder(string externalFolderLocation, Dictionary<string, object> options)
      => ImportAndBuildNewArchetypeFromFolder(externalFolderLocation, options);

    IEnumerable<Archetype> IPorter.ImportAndBuildNewArchetypeFromFiles(string[] externalFileLocations, Dictionary<string, object> options)
      => ImportAndBuildNewArchetypeFromFiles(externalFileLocations, options);
    Archetype IPorter.GetCachedArchetype(string resourceKey)
      => GetCachedArchetype(resourceKey);

    Archetype IPorter.TryToGetGetCachedArchetype(string resourceKey)
      => TryToGetGetCachedArchetype(resourceKey);

    Archetype IPorter.LoadArchetypeFromModFolder(string resourceKey, Dictionary<string, object> options)
      => LoadArchetypeFromModFolder(resourceKey, options);

    Archetype IPorter.TryToFindArchetypeAndLoadFromModFolder(string resourceKey, Dictionary<string, object> options)
      => TryToFindArchetypeAndLoadFromModFolder(resourceKey, options);
    
  }
}
