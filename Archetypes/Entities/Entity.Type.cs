using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
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
      } = "_entities";

      /// <summary>
      /// Can be used to add default components via override
      /// </summary>
      protected virtual HashSet<Func<IBuilder, IModel.IComponent>> DefaultModelComponentCtors 
        => new() {
          _ => new Location(),
          _ => new BasicPhysicalStats()
        };

      ///<summary><inheritdoc/></summary>
      protected override HashSet<IComponent> InitialComponents
        => base.InitialComponents
          .Append(new SpriteDisplayOptions());

      /// <summary>
      /// Inital components
      /// </summary>
      protected override HashSet<Func<IBuilder, IModel.IComponent>> InitialUnlinkedModelComponentCtors 
        => __initialUnlinkedModelComponentCtors ?? DefaultModelComponentCtors;
      HashSet<Func<IBuilder, IModel.IComponent>> __initialUnlinkedModelComponentCtors 
        = null;

      /// <summary>
      /// Used to make a new type of entity.
      /// Make a copy of this constructor that is protected to allow modification of your archetype via mods,
      /// a fully private constructor will prevent people from modding your archetype.
      /// </summary>
      protected internal Type(string name, string resourceKey, string packageKey = null)
        : base(new Identity(name, packageKey)) {
        ResourceKey = resourceKey;
        _packageKey = packageKey;
      }

      Type()
        : base(new Identity("Basic Entity")) {}

      /// <summary>
      /// Can be used to initialize extra model component ctors from a json.config.
      /// </summary>
      internal protected virtual Entity.Type AppendModelComponentConstructors(HashSet<Func<IBuilder, IModel.IComponent>> newComponents) {
        if (__initialUnlinkedModelComponentCtors is not null) {
        newComponents.ForEach(newItem => __initialUnlinkedModelComponentCtors.Add(newItem)); 
        } else
          __initialUnlinkedModelComponentCtors = DefaultModelComponentCtors
            .Concat(newComponents).ToHashSet();

        return this;
      } 

      /// <summary>
      /// Can be used to initialize the default icon of the entity.
      /// </summary>
      internal protected virtual void SetDefaultIconType(Icon.Type defaultEntityIconType)
        => GetComponent<SpriteDisplayOptions>().DefaultIconType = defaultEntityIconType;
    }
  }
}