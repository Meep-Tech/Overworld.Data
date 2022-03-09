using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using Newtonsoft.Json.Linq;
using Overworld.Data.Entities.Components;
using Overworld.Data.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Overworld.Data {
  public partial class Entity {

    /// <summary>
    /// ised to inport and expoert entities to and from mod folders
    /// </summary>
    public class Porter : ArchetypePorter<Entity.Type> {

      /// <summary>
      /// Key for intial model components added to an entity from a config.
      /// </summary>
      public const string EntityExtraInitialModelComponentsConfigOptionKey
        = "initialModelComponents";

      /// <summary>
      /// Key for intial archetype components added to an entity from a config.
      /// TODO: implement
      /// </summary>
      public const string EntityExtraInitialArchetypeComponentsConfigOptionKey
        = "initialArchetypeComponents";

      /// <summary>
      /// An archetype to use instead of the default.
      /// </summary>
      public const string EntityBaseArchetypeConfigOptionKey
        = "archetype";

      /// <summary>
      /// The default sprite for an entity
      /// </summary>
      public const string DefaultSpriteIconConfigOptionKey
        = "defaultSpriteImageFile";

      /// <summary>
      /// The default sprite for an entity
      /// TOOD: implement
      /// </summary>
      public const string SpriteIconsConfigOptionKey
        = "spriteIcons";

      /// <summary>
      /// The default sprite for an entity
      /// TOOD: implement
      /// </summary>
      public const string ExtraSpriteDisplayOptionsConfigOptionKey
        = "otherSpriteDisplayOptions";

      /// <summary>
      /// The default sprite for an entity
      /// TOOD: implement
      /// </summary>
      public const string SpriteAnimationsConfigOptionKey
        = "spriteAnimations";

      /// <summary>
      /// Can be used to set the spawn location
      /// </summary>
      public const string SpawnLocationImportOptionKey
        = "SpawnLocation";

      /// <summary>
      /// Can be used to add basic physics
      /// </summary>
      public const string AddBasicPhysicsImportOptionKey
        = "AddBasicPhysics";

      ///<summary><inheritdoc/></summary>
      public override string SubFolderName
        => "_entities";

      ///<summary><inheritdoc/></summary>
      public Porter(User currentUser)
        : base(() => currentUser.UniqueName) { }

      ///<summary><inheritdoc/></summary>
      public override HashSet<string> ValidConfigOptionKeys
        => base.ValidConfigOptionKeys
          .Append(EntityBaseArchetypeConfigOptionKey)
          .Append(DefaultSpriteIconConfigOptionKey)
          .Append(SpriteAnimationsConfigOptionKey)
          .Append(SpriteIconsConfigOptionKey)
          .Append(ExtraSpriteDisplayOptionsConfigOptionKey)
          .Append(EntityExtraInitialModelComponentsConfigOptionKey)
          .Append(EntityExtraInitialArchetypeComponentsConfigOptionKey);

      ///<summary><inheritdoc/></summary>
      public override HashSet<string> ValidImportOptionKeys 
        => base.ValidImportOptionKeys
          .Append(SpawnLocationImportOptionKey)
          .Append(AddBasicPhysicsImportOptionKey);

      ///<summary><inheritdoc/></summary>
      protected override IEnumerable<Type> BuildLooselyFromConfig(
        JObject config,
        IEnumerable<string> assetFiles,
        Dictionary<string, object> options,
        out IEnumerable<string> processedFiles
      ) {
        List<string> allProcessedFiles = new();

        /// Try to get the default icon from the config.
        string defaultIconFileLocatorString
          = config.TryGetValue<string>(DefaultSpriteIconConfigOptionKey) ??
            assetFiles.FirstOrDefault(f => PorterExtensions.ValidImageExtensions.Contains(Path.GetExtension(f)));

        (string resourceName, string packageName, string resourceKey)
          = ConstructArchetypeKeys(defaultIconFileLocatorString ?? assetFiles.First(), options, config);

        if (defaultIconFileLocatorString != null) {
          Icon.Type defaultIconType = Universe.GetModData().GetPorterFor<Icon.Type>().ImportAndBuildNewArchetypesFromLooseFilesAndFolders(
             defaultIconFileLocatorString.AsSingleItemEnumerable(),
             options
                .Append(PackageNameConfigKey, packageName),
             out HashSet<string> processedDefaultIconFiles
           ).First();

          options.Add(nameof(SpriteDisplayOptions.DefaultIconType), defaultIconType);
          allProcessedFiles.AddRange(processedDefaultIconFiles);
        }

        // check if this is built using a base.
        if (config.TryGetValue(EntityBaseArchetypeConfigOptionKey, StringComparison.OrdinalIgnoreCase, out JToken foundArchetypeName)) {
          if (Entity.Types.TryToGet(foundArchetypeName.Value<string>(), out Type foundBaseArchetype)) {
            options.Add(EntityBaseArchetypeConfigOptionKey, foundBaseArchetype);
          } else throw new ArgumentException($"Cannot find an Archetype of type Entity.Type with key: {foundArchetypeName}.");
        }

        processedFiles = allProcessedFiles;
        return BuildArchetypeFromCompiledData(resourceName, packageName, resourceKey, new JObject(), options, Universe);
      }

      ///<summary><inheritdoc/></summary>
      protected override IEnumerable<Type> BuildLooselyFromAssets(
        IEnumerable<string> assetFiles,
        Dictionary<string, object> options,
        out IEnumerable<string> processedFiles
      ) {
        List<string> allProcessedFiles = new();

        /// Just the icon for loose files.
        string defaultIconFileLocatorString 
          = assetFiles.FirstOrDefault(f => PorterExtensions.ValidImageExtensions.Contains(Path.GetExtension(f)));

        if (defaultIconFileLocatorString != null) {
          (string resourceName, string packageName, string resourceKey)
            = ConstructArchetypeKeys(defaultIconFileLocatorString ?? assetFiles.First(), options, null);

          Icon.Type defaultIconType = Universe.GetModData().GetPorterFor<Icon.Type>().ImportAndBuildNewArchetypesFromLooseFilesAndFolders(
             defaultIconFileLocatorString.AsSingleItemEnumerable(),
             options,
             out HashSet<string> processedDefaultIconFiles
           ).First();

          options.Add(nameof(SpriteDisplayOptions.DefaultIconType), defaultIconType);
          allProcessedFiles.AddRange(processedDefaultIconFiles);

          processedFiles = allProcessedFiles;
          return BuildArchetypeFromCompiledData(resourceName, packageName, resourceKey, new JObject(), options, Universe);
        } else throw new ArgumentException($"Without a config; Entities can only import a single default icon image.");
      }

      ///<summary><inheritdoc/></summary>
      protected override IEnumerable<Type> BuildArchetypeFromCompiledData(
        string resourceName,
        string packageName,
        string resourceKey,
        JObject config,
        Dictionary<string, object> importOptionsAndAssets,
        Universe universe
      ) {
        if (importOptionsAndAssets.TryGetValue(EntityBaseArchetypeConfigOptionKey, out var found) && found is Entity.Type entityBaseArchetype) {
          // if there's an archetype base, use it.
          var ctor = GetPorterConstructorForArchetypeType(entityBaseArchetype.Type);
          try {
            return
              ((Entity.Type)ctor.Invoke(new object[] {
                resourceName, packageName, resourceKey, config, importOptionsAndAssets, universe
              })).AsSingleItemEnumerable();
          }
          catch (Exception e) {
            throw new ArgumentException(
              $"Cannot use Archetype of Entity.Type: {entityBaseArchetype} as a template for a new of Entity.Type, it does not have a protected constructor that matches the IPortableArchetype pattern.",
              e
            );
          }
        } // if there's no base, build normally
        else return base.BuildArchetypeFromCompiledData(resourceName, packageName, resourceKey, config, importOptionsAndAssets, universe);
      }

      ///<summary><inheritdoc/></summary>
      protected override string[] SerializeArchetypeToModFiles(Type archetype, string packageDirectoryPath) {
        List<string> createdFiles = new();

        // Save the config file:
        string configFileName = Path.Combine(packageDirectoryPath, DefaultConfigFileName);
        JObject config = archetype.GenerateConfig();

        File.WriteAllText(configFileName, config.ToString());
        createdFiles.Add(configFileName);

        return createdFiles.ToArray();
      }
    }
  }
}
