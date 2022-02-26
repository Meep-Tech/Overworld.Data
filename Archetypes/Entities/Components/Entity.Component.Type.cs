/*using Meep.Tech.Data;

namespace Overworld.Data {
  public partial class Entity {
    public abstract partial class Component<TEntityComponentBaseType> {

      /// <summary>
      ///  on init, set the builder factory for each subtype:
      /// </summary>
      static Component() {
        Components<TEntityComponentBaseType>.BuilderFactory
          = new Type() {
            ModelBaseType = typeof(TEntityComponentBaseType)
          };
      }

      /// <summary>
      /// There can only be one component per type attached to an entity.
      /// </summary>
      [Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
      public class Type : IComponent<TEntityComponentBaseType>.BuilderFactory, IType {

        internal Type()
          : base(new Identity(typeof(TEntityComponentBaseType).FullName)) 
        {}
      }
    }
  }
}*/