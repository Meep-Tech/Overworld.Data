using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overworld.Data {
  public partial class Entity {
    public partial class Icon {

      /// <summary>
      /// A type of entity icon.
      /// </summary>
      public class Type :
        Archetype<Icon, Icon.Type>,
        IEntityDisplayableSprite.IArchetype,
        IPortableArchetype,
        ITaggable {

        /// <summary>
        /// The in-game sprite this icon type represents
        /// </summary>
        public Sprite Sprite {
          get;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ResourceKey {
          get;
        }

        /// <summary>
        /// The package name that this came from.
        /// </summary>
        public virtual string PackageKey {
          get => _packageKey ?? DefaultPackageKey;
          protected set => _packageKey = value;
        } string _packageKey;

        /// <summary>
        /// The package name that this came from.
        /// </summary>
        public virtual string DefaultPackageKey {
          get;
        } = "_icons";

        /// <summary>
        /// The tile height
        /// </summary>
        public virtual string Description {
          get;
          protected set;
        }

        /// <summary>
        /// Default tags for this icon type.
        /// </summary>
        public virtual IEnumerable<Tag> DefaultTags {
          get => _DefaultTags ?? new();
          internal init => _defaultTags = value.ToHashSet();
        } /**<summary> The backing field used to initialize and override DefaultTags </summary>**/
        protected HashSet<Tag> _DefaultTags {
          get => _defaultTags; 
          set => _defaultTags = value;
        } HashSet<Tag> _defaultTags;

        /// <summary>
        /// Can be used to extend this to a new, non-templateable type.
        /// </summary>
        protected Type(
          string name,
          string resourceKey,
          string packageKey
        ) : base(new Identity(name, packageKey)) {
          ResourceKey = resourceKey;
          PackageKey = packageKey;
        }

        /// <summary>
        /// Used to make new tiles via import.
        /// </summary>
        protected internal Type(
          string name,
          string resourceKey,
          string packageKey,
          JObject config,
          Dictionary<string, object> importOptionsAndObjects
        ) : this(name, resourceKey, packageKey) {
          Description = config.TryGetValue<string>(Porter.DescriptionConfigKey);
          _defaultTags = config.TryGetValue(Porter.TagsConfigOptionKey, @default: Enumerable.Empty<Tag>()).ToHashSet();
          Sprite = importOptionsAndObjects.TryGetValue(nameof(Sprite), out var found)
            ? found as Sprite
            : null;
        }

        ///<summary><inheritdoc/></summary>
        public JObject GenerateConfig() {
          JObject config = new() {
            {
              Porter.NameConfigKey,
              JToken.FromObject(Id.Name)
            },{
              Porter.PackageNameConfigKey,
              JToken.FromObject(PackageKey)
            }
          };

          // config image data
          if (Sprite?.texture != null) {
            config.Add(Porter.PixelsPerTileConfigKey, JToken.FromObject(Sprite.pixelsPerUnit));
          }
            
          config.Add(Porter.TagsConfigOptionKey, JToken.FromObject(DefaultTags));

          return config;
        }

        IEntityDisplayableSprite IEntityDisplayableSprite.IArchetype.Make()
          => Make();

        void IPortableArchetype.Unload() {
          throw new System.NotImplementedException();
        }
      }
    }
  }
}
