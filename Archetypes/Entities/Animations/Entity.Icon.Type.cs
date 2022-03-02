using Meep.Tech.Data;
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
        IPortableArchetype
      {

        /// <summary>
        /// The in-game sprite this icon type represents
        /// </summary>
        public Sprite Sprite {
          get;
          internal set;
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
          protected internal set => _packageKey = value;
        } string _packageKey;

        /// <summary>
        /// The package name that this came from.
        /// </summary>
        public virtual string DefaultPackageKey {
          get;
        } = "_icons";

        internal Type(string name, string resourceKey, string packageKey = null) 
          : base(new Identity(name, packageKey)) {
          ResourceKey = resourceKey;
          _packageKey = packageKey;
        }

        IEntityDisplayableSprite IEntityDisplayableSprite.IArchetype.Make()
          => Make();
      }
    }
  }
}
