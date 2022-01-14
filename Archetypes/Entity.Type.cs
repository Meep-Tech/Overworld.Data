using Meep.Tech.Data;

namespace Overworld.Data {
  public partial class Entity {

    /// <summary>
    /// A type of entity
    /// </summary>
    public abstract partial class Type : Archetype<Entity, Entity.Type>, IPortable {

      public string ResourceKey {
        get;
      }

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

      protected internal Type(string resourceKey, Identity id)
        : base(id ?? new Identity(resourceKey)) {
      }

      Type()
        : base(new Identity("Basic Entity")) {}
    }
  }
}