
using Meep.Tech.Data;

namespace Overworld.Data {

  public partial class Executeable {
    public abstract partial class Type {
      /// <summary>
      /// An Id for an executable type
      /// </summary>
      public new class Identity : Archetype<Executeable, Executeable.Type>.Identity {

        /// <summary>
        /// Make a new Executable Type Id
        /// </summary>
        public Identity(string name, string keyPrefixEndingAdditions = null) 
          : base(name, keyPrefixEndingAdditions) {}
      }
    }
  }
}