using Meep.Tech.Data;

namespace Overworld.Data {

  public partial class Entity {
    public partial class Animation {
      public partial class Type {

        public new class Identity : Archetype<Entity.Animation, Entity.Animation.Type>.Identity {
          protected internal Identity(string name, string keyPrefix = null) 
            : base(name, keyPrefix, baseKeyStringOverride: "") { }
        }
      }
    }
  }
}