using Meep.Tech.Data;
using UnityEngine;

namespace Overworld.Data {
  public partial class Entity {

    /// <summary>
    /// A still image that's used to represent an entity in some way.
    /// Can be used for non-animation sprites.
    /// </summary>
    public partial class Icon 
      : Model<Icon, Icon.Type>, IModel.IUseDefaultUniverse, IEntityDisplayableSprite {
      string IUnique.Id { get; set; }
    }
  }
}
