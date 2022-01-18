using Meep.Tech.Data;

namespace Overworld.Data {

  public partial class Entity {
    public partial class Animation {
      public partial class Type {

        /// <summary>
        /// Animation type id
        /// </summary>
        public new class Identity : Data.Animation.Type.Identity {
          protected internal Identity(string name, string keyPrefix = null) 
            : base(name, keyPrefix, baseKeyStringOverride: "") { }
        }
      }
    }
  }
}