using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using Newtonsoft.Json.Linq;
using Overworld.Data.Entities.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Data {

  public partial class Entity {

    /// <summary>
    /// A type of entity
    /// </summary>
    [Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
    public partial class Type 
      : Archetype<Entity, Entity.Type>.WithDefaultParamBasedModelBuilders,
        IPortableArchetype
    {

      /// <summary>
      /// Built in model components for all entities
      /// </summary>
      public static Dictionary<string, Func<IBuilder, IModel.IComponent>> BuildInModelComponentCtors { get; } = new() {
        { Components<Location>.Key, _ => new Location() },
        { Components<BasicPhysicalStats>.Key, _ => new BasicPhysicalStats() }
      };

      /// <summary>
      /// <inheritdoc/>
      /// </summary>
      public string ResourceKey {
        get;
      }

      /// <summary>
      /// The tile height
      /// </summary>
      public virtual string Description {
        get;
        protected set;
      }

      /// <summary>
      /// The package name that this came from.
      /// </summary>
      public virtual string PackageKey {
        get => _packageKey ?? DefaultPackageKey;
        protected internal set => _packageKey = value;
      } string _packageKey;

      /// <summary>
      /// The package name that this came from.
      /// </summary>
      public virtual string DefaultPackageKey {
        get;
      } = "_entities";

      /// <summary>
      /// Default tags for this entity type.
      /// </summary>
      public virtual IEnumerable<Tag> DefaultTags {
        get => _DefaultTags ?? new();
        internal init => _defaultTags = value.ToHashSet();
      } /**<summary> The backing field used to initialize and override DefaultTags </summary>**/
      protected HashSet<Tag> _DefaultTags {
        get => _defaultTags;
        set => _defaultTags = value;
      }  HashSet<Tag> _defaultTags;

      /// <summary>
      /// Can be used to add default components via override
      /// </summary>
      protected virtual Dictionary<string, Func<IBuilder, IModel.IComponent>> DefaultModelComponentCtors {
        get => _DefaultModelComponentCtors ?? BuildInModelComponentCtors;
      } /** <summary> The backing field used to initialize and override the initail value of DefaultModelComponentCtors. You can this syntax to override or add to the base initial value: '=> _DefaultModelComponentCtors ??= base.DefaultModelComponentCtors.Append(...' </summary> **/
      protected Dictionary<string, Func<IBuilder, IModel.IComponent>> _DefaultModelComponentCtors { 
        get => __DefaultModelComponentCtors; 
        set => __DefaultModelComponentCtors = value;
      } Dictionary<string, Func<IBuilder, IModel.IComponent>> __DefaultModelComponentCtors;

      ///<summary><inheritdoc/></summary>
      protected override HashSet<IComponent> InitialComponents
        => _InitialComponents ??= base.InitialComponents
          .Append(new SpriteDisplayOptions());

      /// <summary>
      /// Inital components.
      /// Overrite DefaultModelComponentCtors instead.
      /// </summary>
      protected sealed override HashSet<Func<IBuilder, IModel.IComponent>> InitialUnlinkedModelComponentCtors 
        => (_initialUnlinkedModelComponentCtors ??= DefaultModelComponentCtors).Values.ToHashSet();
      Dictionary<string, Func<IBuilder, IModel.IComponent>> _initialUnlinkedModelComponentCtors 
        = null;

      /// <summary>
      /// Can be used to extend this to a new, non-templateable type.
      /// </summary>
      protected Type(
        string name,
        string packageKey,
        string resourceKey,
        Universe universe = null
      ) : base(new Identity(name, packageKey), universe) {
         ResourceKey = resourceKey;
        _packageKey = packageKey;
      }

      /// <summary>
      /// Used to make a new type of entity.
      /// Make a copy of this constructor that is protected to allow modification of your archetype via mods,
      /// a fully private constructor will prevent people from modding your archetype.
      /// </summary>
      protected internal Type(
        string name,
        string packageKey,
        string resourceKey,
        JObject config,
        Dictionary<string, object> importOptionsAndObjects,
        Universe universe
      ) : this(name, resourceKey, packageKey, universe) {
        /// tags
        _defaultTags = config.TryGetValue(Porter.TagsConfigOptionKey, @default: Enumerable.Empty<Tag>()).ToHashSet();

        /// model components
        AddInitialModelComponentTypeConstructorsFromConfig(config);

        /// sprites
        if (importOptionsAndObjects.TryGetValue(nameof(SpriteDisplayOptions.DefaultIconType), out object defaultIconType)) {
          SetDefaultIconType((Entity.Icon.Type)defaultIconType);
        }
        AddSpriteDisplayOptionsFromConfig(config);

        /// general options
        // add spawn location if provided by the options.
        if (importOptionsAndObjects.TryGetValue(Porter.SpawnLocationImportOptionKey, out object value) && value is (int x, int y, int height)) {
          AppendModelComponentConstructors(new() {
            {
              Components<SimpleSpawnLocation>.Key,
              new Func<IBuilder, IModel.IComponent>(_ => new SimpleSpawnLocation {
                X = x,
                Y = y,
                Height = height
              })
            }
          });
        }

        // add basic physics if requested by the options.
        if (importOptionsAndObjects.TryGetValue(Porter.AddBasicPhysicsImportOptionKey, out object ifShould) && ifShould is true) {
          AppendModelComponentConstructors(new() {
            {
              Components<SimpleSpawnLocation>.Key,
              new Func<IBuilder, IModel.IComponent>(
                builder => Components<SimplePhysics>.BuilderFactory.Make(builder)
              )
            }
          });
        }
      }

      Type()
        : base(new Identity("Basic Entity")) {}

      /// <summary>
      /// Can be used to initialize extra model component ctors from a json.config.
      /// </summary>
      internal protected virtual Entity.Type AppendModelComponentConstructors(Dictionary<string, Func<IBuilder, IModel.IComponent>> newComponents) {
        if (_initialUnlinkedModelComponentCtors is not null) {
          newComponents.ForEach(newItem => _initialUnlinkedModelComponentCtors[newItem.Key] = newItem.Value);
        } else
          _initialUnlinkedModelComponentCtors = DefaultModelComponentCtors
            .Merge(newComponents);

        return this;
      } 

      /// <summary>
      /// Can be used to initialize the default icon of the entity.
      /// </summary>
      protected virtual void SetDefaultIconType(Icon.Type defaultEntityIconType)
        => GetComponent<SpriteDisplayOptions>().DefaultIconType = defaultEntityIconType;

      ///<summary><inheritdoc/></summary>
      public JObject GenerateConfig() {
        JObject config = new() {
          {
            Porter.NameConfigKey,
            JToken.FromObject(Id.Name)
          },
          {
            Porter.PackageNameConfigKey,
            JToken.FromObject(PackageKey)
          },
          {
            Porter.DescriptionConfigKey,
            JToken.FromObject(Description)
          }
        };

        /// if this is from an template archetype:
        if (GetType() != typeof(Entity.Type)) {
          config.Add(Porter.EntityBaseArchetypeConfigOptionKey, JToken.FromObject(Id.ExternalId));
        }

        /// components
        if (DefaultModelComponentCtors?.Any() ?? false) {
          JObject components = JObject.FromObject(DefaultModelComponentCtors
            .Where(e => e.Key != Components<SpriteManager>.Key).ToDictionary(
              e => e.Key,
              e => e.Value.Invoke(MakeDefaultBuilder())
            ));

          config.Add(
            Porter.EntityExtraInitialModelComponentsConfigOptionKey,
            components
          );
        }

        if (Components?.Any() ?? false) {
          JObject components = JObject.FromObject(DefaultModelComponentCtors
            .Where(e => e.Key != Components<SpriteDisplayOptions>.Key).ToDictionary(
              e => e.Key,
              e => e.Value.Invoke(MakeDefaultBuilder())
            ));

          config.Add(
            Porter.EntityExtraInitialArchetypeComponentsConfigOptionKey,
            components
          );
        }

        /// sprites and animations from the manager component:
        // default sprite icon:
        SpriteDisplayOptions spriteData = GetComponent<SpriteDisplayOptions>();
        if (spriteData.DefaultIconType is not null) {
          config.Add(Porter.DefaultSpriteIconConfigOptionKey, spriteData.DefaultIconType.Id.Key);
        }

        HashSet<IEntityDisplayableSprite.IArchetype> serializedDisplayTypes
          = new();

        // other icons
        if (spriteData.IconTypes.Any()) {
          Dictionary<string, IEnumerable<Tag>> tagsPerIcon = new();
          spriteData.IconTypes.ForEach(e => {
            tagsPerIcon.Add(e.Value.Id.Key, e.Key);
            serializedDisplayTypes.Add(e.Value);
          });
          config.Add(Porter.SpriteIconsConfigOptionKey, JToken.FromObject(tagsPerIcon));
        }

        // animations
        if (spriteData.AnimationTypes.Any()) {
          Dictionary<string, IEnumerable<Tag>> tagsPerAnimation = new();
          spriteData.AnimationTypes.ForEach(e => {
            tagsPerAnimation.Add(e.Value.Id.Key, e.Key);
            serializedDisplayTypes.Add(e.Value);
          });
          config.Add(Porter.SpriteAnimationsConfigOptionKey, JToken.FromObject(tagsPerAnimation));
        }

        // other display options
        IEnumerable<IEntityDisplayableSprite.IArchetype> unserializedDisplayTypes
          = spriteData.AllDisplayOptionTypes.Values.Except(serializedDisplayTypes);
        if (unserializedDisplayTypes.Any()) {
          Dictionary<string, IEnumerable<Tag>> otherDisplayOptions = new();
          unserializedDisplayTypes.ForEach(e => {
            otherDisplayOptions.Add(e.Id.Key, spriteData.AllDisplayOptionTypes[e]);
          });
          config.Add(Porter.ExtraSpriteDisplayOptionsConfigOptionKey, JToken.FromObject(otherDisplayOptions));
        }

        return config;
      }

      /// <summary>
      /// Get sprite display options for an entity from a config.
      /// </summary>
      protected void AddSpriteDisplayOptionsFromConfig(JObject config) {
        Dictionary<IEntityDisplayableSprite.IArchetype, IEnumerable<Tag>> allDisplayTypesToAdd
          = null;

        Dictionary<string, IEnumerable<Tag>> icons = new();
        Dictionary<string, IEnumerable<Tag>> animations = new();
        Dictionary<string, IEnumerable<Tag>> other = new();

        JObject iconsWithTags = config.TryGetValue<JObject>(Porter.SpriteIconsConfigOptionKey);
        if (iconsWithTags is not null) {
          foreach (KeyValuePair<string, JToken> property in iconsWithTags) {
            icons.Add(property.Key, property.Value.Value<IEnumerable<Tag>>());
          }
        }

        JObject animationsWithTags = config.TryGetValue<JObject>(Porter.SpriteAnimationsConfigOptionKey);
        if (animationsWithTags is not null) {
          foreach (KeyValuePair<string, JToken> property in animationsWithTags) {
            animations.Add(property.Key, property.Value.Value<IEnumerable<Tag>>());
          }
        }

        JObject otherWithTags = config.TryGetValue<JObject>(Porter.ExtraSpriteDisplayOptionsConfigOptionKey);
        if (animationsWithTags is not null) {
          foreach (KeyValuePair<string, JToken> property in otherWithTags) {
            other.Add(property.Key, property.Value.Value<IEnumerable<Tag>>());
          }
        }

        allDisplayTypesToAdd
          = icons.Concat(animations).Concat(other)
            .ToDictionary(
              tagsByDisplayTypeKey => {
                try {
                  return (IEntityDisplayableSprite.IArchetype)Archetypes.All.Get(tagsByDisplayTypeKey.Key);
                } catch (Exception ex) {
                  throw new ArgumentException($"Could not get display type archetype of type Icon, Animation, etc with key:{tagsByDisplayTypeKey.Key ?? "NULLKEYERROR"}", ex);
                }
              },
              e => e.Value
            );


        /// add any found sprite display options
        if (allDisplayTypesToAdd is not null) {
          SpriteDisplayOptions spriteData = GetComponent<SpriteDisplayOptions>();
          allDisplayTypesToAdd.ForEach(e => spriteData.Add(e.Key, e.Value));
        }
      }

      /// <summary>
      /// Add initial model component ctors from the config.
      /// </summary>
      protected void AddInitialModelComponentTypeConstructorsFromConfig(JObject config) {
        JToken initialModelComponentsList = config.TryGetValue<JToken>(Porter.EntityExtraInitialModelComponentsConfigOptionKey);
        OrderedDictionary<string, Func<IBuilder, IModel.IComponent>> componentConstructors = new();
        if (initialModelComponentsList is JObject objectBasedList) {
          foreach (KeyValuePair<string, JToken> child in objectBasedList) {
            // if the value is a json object, try to deserialize it as the given component type.
            if (child.Value is JObject componentJson) {
              componentConstructors.Add(
                child.Key,
                builder => {
                  var component = (IModel.IComponent)
                    IComponent.FromJson(componentJson, _getComponentType(child.Key), withConfigurationParameters: builder);
                  return component;
                }
              );
            } // if it's not an json object, then we just use the key to make a default
            else {
              componentConstructors.Add(
                child.Key,
                builder => _getDefaultComponentBuilderFor(child.Key).Make(builder)
              );
            }
          }
        } // if it's justan array of strings, we use the strings to build default components
        else if (initialModelComponentsList is JArray simpleArrayOfTypes) {
          foreach (string componentTypeKey in simpleArrayOfTypes.Values<string>()) {
            componentConstructors.Add(
              componentTypeKey,
              builder => _getDefaultComponentBuilderFor(componentTypeKey).Make(builder)
            );
          }
        } else throw new System.ArgumentException(Porter.EntityExtraInitialModelComponentsConfigOptionKey);

        AppendModelComponentConstructors(componentConstructors);
      }

      static Dictionary<string, System.Type>
        _entityComponentDefaultBuilderCache = new();

      static IComponent.IBuilderFactory _getDefaultComponentBuilderFor(string componentTypeName)
        => Meep.Tech.Data.Components.GetBuilderFactoryFor(
          _getComponentType(componentTypeName)
        );

      static System.Type _getComponentType(string componentTypeName)
        => _entityComponentDefaultBuilderCache.TryGetValue(componentTypeName, out System.Type found)
          ? found
          : _entityComponentDefaultBuilderCache[componentTypeName] =
            TypeExtensions.GetTypeByFullName(componentTypeName);

      void IPortableArchetype.Unload()
        => TryToUnload();
    }
  }
}