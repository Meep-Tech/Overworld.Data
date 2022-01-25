﻿using Meep.Tech.Data;
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
        _moveFileToFinishedImportsFolder(archetype, effectedFiles, packageName, options);
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
        _moveFileToFinishedImportsFolder(archetype, effectedFiles, packageName, options);
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

      string packageName = null;
      if(!((options?.ContainsKey(IArchetypePorter.NoPackageName) ?? false) && (bool)options[IArchetypePorter.NoPackageName])) {
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
        && options.TryGetValue(IArchetypePorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
          ? (bool)moveFiles
          : false
      ) {
        foreach(TArchetype archetype in archetypes) {
          _cacheArchetype(archetype, packageName);
          _moveFileToFinishedImportsFolder(archetype, new string[] { externalFileLocation }, packageName, options);
        }
      } else {
        foreach(TArchetype archetype in archetypes) {
          _cacheArchetype(archetype, packageName);
        }
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
        && options.TryGetValue(IArchetypePorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
          ? (bool)moveFiles
          : false
      ) {
        foreach(TArchetype archetype in archetypes) {
          _cacheArchetype(archetype, packageName);
          _moveFileToFinishedImportsFolder(archetype, effectedFiles, packageName, options);
        }
      } else {
        foreach(TArchetype archetype in archetypes) {
          _cacheArchetype(archetype, packageName);
        }
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
        && options.TryGetValue(IArchetypePorter.MoveFinishedFilesToFinishedImportsFolderSetting, out var moveFiles)
        && (bool)moveFiles
      ) {
        foreach(TArchetype archetype in archetypes) {
          _cacheArchetype(archetype);
          _moveFileToFinishedImportsFolder(archetype, externalFileLocations, null, options);
        }
      } else {
        foreach(TArchetype archetype in archetypes) {
          _cacheArchetype(archetype);
        }
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
        modFolder = Path.Combine(modFolder, DefaultPackageName, name);
      } else {
        modFolder = Path.Combine(modFolder, packageName, DefaultPackageName, name);
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
      string exportFolder = Path.Combine(Application.persistentDataPath, IArchetypePorter.ModFolderName, IArchetypePorter.FinishedImportsFolderName, packageName ?? compiled.DefaultPackageName);
      if(packageName is not null) {
        exportFolder = Path.Combine(exportFolder, compiled.DefaultPackageName);
      }

      // save files that are re-compiled for speed to the mod folder:
      _serializeArchetypeToModFiles(compiled, GetFolderForArchetype(compiled));

      // Move each untouched file to output:
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
          if(!Directory.GetParent(fileName).Parent.GetFiles().Any()) {
            if(Directory.GetParent(fileName).Name == IArchetypePorter.ImportFolderName) {
              throw new Exception($"Folder deleting wrong");
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

    #endregion
  }
}