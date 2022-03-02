namespace Overworld.Data {
  public partial class Entity {
    public partial class Type {
      /// <summary>
      /// Ids for entity types
      /// </summary>
      public new class Identity : Meep.Tech.Data.Archetype<Entity, Type>.Identity {
        protected internal Identity(string name, string keyPrefix = null)
          : base(name, keyPrefix, baseKeyStringOverride: "") { }
      }
    }
  }
}