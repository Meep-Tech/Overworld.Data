using Meep.Tech.Data;

namespace Overworld.Data {
  public partial struct Tile {

    public partial class Type {
      public new class Identity : Archetype<Tile, Tile.Type>.Identity {
        protected internal Identity(string name, string keyPrefix = null)
          : base(name, keyPrefix, baseKeyStringOverride: "") { }
      }
    }
  }
}
