using Meep.Tech.Data;
using Simple.Ux.XBam.Components;

namespace Overworld.Data.Entites.Components {

  /// <summary>
  /// A component to place an execution hook on an entity interaction.
  /// </summary>
  public abstract class EntityInteractionHook : EntityHook<EntityInteractionHook> {

    ///<summary><inheritdoc/></summary>
    public override string SimpleUxMenuTitle
      => "Right Click Menu Items";
  }
}
