using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
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
      /// Valid image extensions
      /// </summary>
      public readonly static HashSet<string> ValidDefaultImageExtensions
        = new() {
          {".png"}
        };

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
      public const string DefaultSpriteConfigOptionKey
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
      public override string DefaultPackageName
        => "_entities";

      ///<summary><inheritdoc/></summary>
      public Porter(User currentUser)
        : base(currentUser) { }

      ///<summary><inheritdoc/></summary>
      public override HashSet<string> ValidConfigOptionKeys
        => base.ValidConfigOptionKeys
          .Append(EntityBaseArchetypeConfigOptionKey)
          .Append(DefaultSpriteConfigOptionKey)
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

      /// <summary>
      /// If it's a single file and it's a config, it's added as an invisible entity(ghost icon)
      /// If it's a single file and it's an image file, it's added as a new entity with basic physics.
      /// </summary>
      /// <param name="options"></param>
      protected override IEnumerable<Type> _importArchetypesFromExternalFile(
        string externalFileLocation,
        string resourceKey,
        string name,
        string packageKey = null,
        Dictionary<string, object> options = null
      ) {
        Type @return = null;
        string extension;
        /// import just from a config json file
        if ((extension = Path.GetExtension(externalFileLocation).ToLower()) == ".json") {
          JObject config = TryToGetConfig(externalFileLocation.AsSingleItemEnumerable(), out _);

          resourceKey = CorrectBaseKeysAndNamesForConfigValues(
            externalFileLocation,
            ref name,
            ref packageKey,
            config
          );

          options ??= new();
          /// try to get an default icon, or set it to default(ghost) and make it so it doesn't display by default either.
          string iconInfoString;
          // check fist if we have a string to fetch the icon.
          if (((iconInfoString = config.TryGetValue<string>(DefaultSpriteConfigOptionKey)?.ToLower()) != null)
          // if that fails, try to get a file with the desired name from the same folder as the config.
          || (iconInfoString = new DirectoryInfo(externalFileLocation).GetFiles($"{name}.*")
            .Where(f => ValidDefaultImageExtensions.Contains(f.Extension)).FirstOrDefault()?.FullName) != null
          ) {
            Icon.Porter iconImporter = (ArchetypePorter<Entity.Icon.Type>.DefaultInstance as Entity.Icon.Porter);
            if (Icon.Porter._isValidImageImportString(iconInfoString, out _)) {
              options.Add(nameof(SpriteDisplayOptions.DefaultIconType), iconImporter.ImportAndBuildNewArchetypeFromFile(
                iconInfoString,
                options
                  .Append(NameConfigKey, name)
                  .Append(PackageNameConfigKey, packageKey ?? iconImporter.DefaultPackageName)
                  .ToDictionary(e => e.Key, e => e.Value)
              ).FirstOrDefault());
            } // if it's not a valid URI for an image import, check if it's an known archetype:
            else {
              options.Add(nameof(SpriteDisplayOptions.DefaultIconType), Entity.Icon.Types.TryToGet(iconInfoString));
            }
          }

          // if there's an archetype base to use as a template, try to use it with the default ctor, and add any components:
          if (config.TryGetValue(EntityBaseArchetypeConfigOptionKey, out JToken foundArchetypeName)) {
            if (Entity.Types.TryToGet(foundArchetypeName.Value<string>(), out var foundBaseArchetype)) {
              var ctor = foundBaseArchetype.Type.GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new System.Type[] {
                typeof(string),
                typeof(Entity.Type.Identity)
              }, null);
              if (ctor is not null && ctor.IsFamily) {
                @return =
                  (Entity.Type)ctor.Invoke(new object[] {
                    resourceKey,
                    new Type.Identity(name, packageKey)
                  });
              } else throw new ArgumentException(
                  $"Cannot use Archetype of Entity.Type: {foundArchetypeName} as a template for a new of Entity.Type, it does not have a protected constructor that takes a resource key and an Entity.Type.Identity."
                );
            } else throw new ArgumentException(
              $"Cannot find an Archetype of type Entity.Type with key: {foundArchetypeName}."
            );
          } // no template, make a basic type.
          else {
            @return =
              new Entity.Type(
                name,
                resourceKey,
                packageKey,
                config,
                options
              );
          }
        }
        /// check if it's a valid image type and import that way instead
        else if (ValidDefaultImageExtensions.Contains(extension)) {
          // import it as the default icon if it was found:
          Icon.Porter iconImporter = (ArchetypePorter<Entity.Icon.Type>.DefaultInstance as Entity.Icon.Porter);
          if (Icon.Porter._isValidImageImportString(externalFileLocation, out _)) {
            options.Add(nameof(SpriteDisplayOptions.DefaultIconType), iconImporter.ImportAndBuildNewArchetypeFromFile(
              externalFileLocation,
              options
                .Append(NameConfigKey, name)
                .Append(PackageNameConfigKey, packageKey ?? iconImporter.DefaultPackageName)
                .ToDictionary(e => e.Key, e => e.Value)
            ).FirstOrDefault());

            @return =
              new Entity.Type(
                name,
                resourceKey,
                packageKey,
                new JObject(),
                options
              );
          }
        }

        return @return
          .AsSingleItemEnumerable();
      }

      ///<summary><inheritdoc/></summary>
      protected override IEnumerable<Type> _importArchetypesFromExternalFiles(
        string[] externalFileLocations,
        string resourceKey,
        string name,
        string packageKey = null,
        Dictionary<string, object> options = null
      ) {
        Type @return = null;
        JObject config = TryToGetConfig(externalFileLocations, out string configFileLocation);

        resourceKey = CorrectBaseKeysAndNamesForConfigValues(
          configFileLocation,
          ref name,
          ref packageKey,
          config
        );

        options ??= new();
        /// try to get an default icon, or set it to default(ghost) and make it so it doesn't display by default either.
        string iconInfoString;
        // check fist if we have a string to fetch the icon.
        if (((iconInfoString = config.TryGetValue<string>(DefaultSpriteConfigOptionKey)?.ToLower()) != null)
          // if that fails, try to get a file with the desired name from the same folder as the config.
          || ((iconInfoString = externalFileLocations.Where(file => Path.GetFileName(file).StartsWith($"{name}."))
            .Where(f => ValidDefaultImageExtensions.Contains(Path.GetExtension(f))).FirstOrDefault()) != null)
          // without the same name, get the first matching image
          || ((iconInfoString = externalFileLocations.Where(f => ValidDefaultImageExtensions.Contains(Path.GetExtension(f))).FirstOrDefault()) != null)
        ) {
          Icon.Porter iconImporter = (ArchetypePorter<Entity.Icon.Type>.DefaultInstance as Entity.Icon.Porter);
          if (Icon.Porter._isValidImageImportString(iconInfoString, out _)) {
            options.Add(nameof(SpriteDisplayOptions.DefaultIconType), iconImporter.ImportAndBuildNewArchetypeFromFile(
              iconInfoString,
              options
                .Append(NameConfigKey, name)
                .Append(PackageNameConfigKey, packageKey ?? iconImporter.DefaultPackageName)
                .ToDictionary(e => e.Key, e => e.Value)
            ).FirstOrDefault());
          } // if it's not a valid URI for an image import, check if it's an known archetype:
          else {
            options.Add(nameof(SpriteDisplayOptions.DefaultIconType), Entity.Icon.Types.TryToGet(iconInfoString));
          }
        }

        // if there's an archetype base to use as a template, try to use it with the default ctor, and add any components:
        if (config.TryGetValue(EntityBaseArchetypeConfigOptionKey, out JToken foundArchetypeName)) {
          if (Entity.Types.TryToGet(foundArchetypeName.Value<string>(), out var foundBaseArchetype)) {
            var ctor = foundBaseArchetype.Type.GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new System.Type[] {
                typeof(string),
                typeof(Entity.Type.Identity)
              }, null);
            if (ctor is not null && ctor.IsFamily) {
              @return =
                (Entity.Type)ctor.Invoke(new object[] {
                    resourceKey,
                    new Type.Identity(name, packageKey)
                });
            } else throw new ArgumentException(
                $"Cannot use Archetype of Entity.Type: {foundArchetypeName} as a template for a new of Entity.Type, it does not have a protected constructor that takes a resource key and an Entity.Type.Identity."
              );
          } else throw new ArgumentException(
            $"Cannot find an Archetype of type Entity.Type with key: {foundArchetypeName}."
          );
        } // no template, make a basic type.
        else {
          @return =
            new Entity.Type(
              name,
              resourceKey,
              packageKey,
              config,
              options
            );
        }

        return @return.AsSingleItemEnumerable();
      }

      ///<summary><inheritdoc/></summary>
      protected override string[] _serializeArchetypeToModFiles(Type archetype, string packageDirectoryPath) {
        List<string> createdFiles = new();

        // Save the config file:
        string configFileName = Path.Combine(packageDirectoryPath, IArchetypePorter.ConfigFileName);
        JObject config = archetype.GenerateConfig();

        File.WriteAllText(configFileName, config.ToString());
        createdFiles.Add(configFileName);

        return createdFiles.ToArray();
      }
    }
  }
}
