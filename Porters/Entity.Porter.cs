using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Newtonsoft.Json.Linq;
using Overworld.Data.Entities.Components;
using Overworld.Data.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Overworld.Data {
  public partial class Entity {

    public class Porter : ArchetypePorter<Entity.Type> {
      
      /// <summary>
      /// Valid image extensions
      /// </summary>
      public readonly static HashSet<string> ValidDefaultImageExtensions
        = new() {
          {".png"}
        };

      /// <summary>
      /// Key for intial components added to an entity from a config.
      /// </summary>
      public const string EntityExtraInitialModelComponentsConfigOptionKey
        = "initialModelComponents";

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
      /// Can be used to set the spawn location
      /// </summary>
      public const string SpawnLocationImportOptionKey
        = "SpawnLocation";

      /// <summary>
      /// Can be used to add basic physics
      /// </summary>
      public const string AddBasicPhysicsImportOptionKey
        = "AddBasicPhysics";

      static Dictionary<string, System.Type>
        _entityComponentDefaultBuilderCache = new();

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
          .Append(EntityExtraInitialModelComponentsConfigOptionKey);

      ///<summary><inheritdoc/></summary>
      public override HashSet<string> ValidImportOptionKeys 
        => base.ValidImportOptionKeys
          .Append(SpawnLocationImportOptionKey)
          .Append(AddBasicPhysicsImportOptionKey);

      protected override IEnumerable<Type> _importArchetypesFromExternalFiles(string[] externalFileLocations, string resourceKey, string name, string packageKey = null, Dictionary<string, object> options = null) {
        return base._importArchetypesFromExternalFiles(externalFileLocations, resourceKey, name, packageKey, options);
      }

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
        Entity.Icon.Type defaultEntityIcon = null;
        /// import just from a config json file
        if ((extension = Path.GetExtension(externalFileLocation).ToLower()) == ".json") {
          JObject config = TryToGetConfig(externalFileLocation.AsSingleItemEnumerable(), out _);

          resourceKey = CorrectBaseKeysAndNamesForConfigValues(
            externalFileLocation,
            ref name,
            ref packageKey,
            config
          );

          IEnumerable<Func<IBuilder, IModel.IComponent>> initialModelComponents
            = _tryToGetInitialModelComponentTypeConstructors(config);

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
              defaultEntityIcon = iconImporter.ImportAndBuildNewArchetypeFromFile(
                iconInfoString,
                options
                  .Append(NameConfigKey, name)
                  .Append(PackageNameConfigKey, packageKey ?? iconImporter.DefaultPackageName)
                  .ToDictionary(e => e.Key, e => e.Value)
              ).FirstOrDefault();
            } // if it's not a valid URI for an image import, check if it's an known archetype:
            else {
              defaultEntityIcon = Entity.Icon.Types.TryToGet(iconInfoString);
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
                  ((Entity.Type)ctor.Invoke(new object[] {
                    resourceKey,
                    new Type.Identity(name, packageKey)
                  })).AppendModelComponentConstructors(initialModelComponents.ToHashSet());
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
                packageKey
              ).AppendModelComponentConstructors(initialModelComponents.ToHashSet());
          }
        }
        /// check if it's a valid image type and import that way instead
        else if (ValidDefaultImageExtensions.Contains(extension)) {
          @return =
            new Entity.Type(
              name,
              resourceKey,
              packageKey
            );

          Icon.Porter iconImporter = (ArchetypePorter<Entity.Icon.Type>.DefaultInstance as Entity.Icon.Porter);
          if (Icon.Porter._isValidImageImportString(externalFileLocation, out _)) {
            defaultEntityIcon = iconImporter.ImportAndBuildNewArchetypeFromFile(
              externalFileLocation,
              options
                .Append(NameConfigKey, name)
                .Append(PackageNameConfigKey, packageKey ?? iconImporter.DefaultPackageName)
                .ToDictionary(e => e.Key, e => e.Value)
            ).FirstOrDefault();
          }
        }

        /// apply the found image.
        if (defaultEntityIcon is not null) {
          @return.SetDefaultIconType(defaultEntityIcon);
        }

        /// options
        if (@return is not null) {
          // add spawn location if provided by the options.
          if (options.TryGetValue(SpawnLocationImportOptionKey, out object value) && value is (int x, int y, int height)) {
            @return.AppendModelComponentConstructors(
              new Func<IBuilder, IModel.IComponent>(_ => new SimpleSpawnLocation {
                X = x,
                Y = y,
                Height = height
              }).AsSingleItemEnumerable().ToHashSet()
            );
          }

          // add basic physics if requested by the options.
          if (options.TryGetValue(AddBasicPhysicsImportOptionKey, out object ifShould) && ifShould is true) {
            @return.AppendModelComponentConstructors(
              new Func<IBuilder, IModel.IComponent>(
                builder => (IModel.IComponent)
                  Components<SimplePhysics>.BuilderFactory.Make(builder)
              ).AsSingleItemEnumerable().ToHashSet()
            );
          }
        }

        return new[] {
          @return
        };
      }

      protected override string[] _serializeArchetypeToModFiles(Type archetype, string packageDirectoryPath) {
        throw new NotImplementedException();
      }

      IEnumerable<Func<IBuilder, IModel.IComponent>> _tryToGetInitialModelComponentTypeConstructors(JObject config) {
        JToken initialModelComponentsList = config.TryGetValue<JToken>(EntityExtraInitialModelComponentsConfigOptionKey);
        List<Func<IBuilder, IModel.IComponent>> componentConstructors = new();
        if (initialModelComponentsList is JObject objectBasedList) {
          foreach (KeyValuePair<string, JToken> child in objectBasedList) {
            // if the value is a json object, try to deserialize it as the given component type.
            if (child.Value is JObject componentJson) {
              componentConstructors.Add(builder => {
                var component = (IModel.IComponent)
                  IComponent.FromJson(componentJson, _getComponentType(child.Key), withConfigurationParameters: builder);
                return component;
              });
            } // if it's not an json object, then we just use the key to make a default
            else {
              componentConstructors.Add(
                _ => _getDefaultComponentBuilderFor(child.Key).Make()
              );
            }
          }
        } else if (initialModelComponentsList is JArray simpleArrayOfTypes) {
          foreach (string componentTypeName in simpleArrayOfTypes.Values<string>()) {
            componentConstructors.Add(
              _ => _getDefaultComponentBuilderFor(componentTypeName).Make()
            );
          }
        } else throw new System.ArgumentException(EntityExtraInitialModelComponentsConfigOptionKey);

        return componentConstructors;
      }

      static IComponent.IBuilderFactory _getDefaultComponentBuilderFor(string componentTypeName)
        => Meep.Tech.Data.Components.GetBuilderFactoryFor(
          _getComponentType(componentTypeName)
        );

      static System.Type _getComponentType(string componentTypeName) 
        => _entityComponentDefaultBuilderCache.TryGetValue(componentTypeName, out System.Type found)
          ? found
          : _entityComponentDefaultBuilderCache[componentTypeName] =
            TypeExtensions.GetTypeByFullName(componentTypeName);
    }
  }
}
