using Meep.Tech.Data;

namespace Overworld.Data {
  public partial class Entity {

    /// <summary>
    /// A type of entity
    /// </summary>
    public abstract partial class Type : Archetype<Entity, Entity.Type>, IPortableArchetype {

      /// <summary>
      /// <inheritdoc/>
      /// </summary>
      public string ResourceKey {
        get;
      }

      /// <summary>
      /// Example, how to add a entity component that will be carried to the child.
      /// TODO: Remove test
      /// </summary>
      /*public override HashSet<Func<IBuilder, IModel.IComponent>> InitialUnlinkedModelComponentCtors
        => base.InitialUnlinkedModelComponentCtors.Append(builder => {
          return new Entites.Components.DoEveryFrameHook();
        });*/

      /// <summary>
      /// The package name that this came from.
      /// </summary>
      public virtual string PackageName {
        get => _packageName ?? DefaultPackageName;
        protected set => _packageName = value;
      } string _packageName;

      /// <summary>
      /// The package name that this came from.
      /// </summary>
      public virtual string DefaultPackageName {
        get;
      } = "-Entities";

      /// <summary>
      /// Used to make a new type of entity
      /// </summary>
      protected internal Type(string resourceKey, Identity id)
        : base(id ?? new Identity(resourceKey)) {
      }

      Type()
        : base(new Identity("Basic Entity")) {}
    }
  }
}