using Meep.Tech.Data;

namespace Overworld.Data.Entites.Components {

  /// <summary>
  /// A Hook to execute something every frame
  /// </summary>
  public abstract class DoEveryFrameHook : EntityHook<DoEveryFrameHook> {
    static DoEveryFrameHook() {
      Archetypes<Type>._.DisplayName = "Hook: Do Every Frame";
    }
  }
}