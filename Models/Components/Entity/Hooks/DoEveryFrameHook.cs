using Meep.Tech.Data;
using Simple.Ux.XBam.Components;

namespace Overworld.Data.Entites.Components {

  /// <summary>
  /// A Hook to execute something every frame
  /// </summary>
  public abstract class DoEveryFrameHook : EntityHook<DoEveryFrameHook> {

    ///<summary><inheritdoc/></summary>
    public override string SimpleUxMenuTitle
      => "Hook: Do Every Frame";
  }
}