namespace Overworld.Data.Entities.Components {

  /// <summary>
  /// When attached to an entity in world, it means this entity will spawn at this location when the world starts.
  /// </summary>
  public class SimpleSpawnLocation : IDefaultEntityComponent<SimpleSpawnLocation> {

    /// <summary>
    /// E/W location in world to spawn at
    /// </summary>
    public int X {
      get;
      set;
    }

    /// <summary>
    /// N/S location in world to spawn at
    /// </summary>
    public int Y {
      get;
      set;
    }

    /// <summary>
    /// Height in world to spawn at
    /// </summary>
    public int Height {
      get;
      set;
    }

    /// <summary>
    /// If this entity should resapwn when destroyed/when it falls off the world.
    /// </summary>
    public bool RespawnWhenDestroyed {
      get;
      set;
    }

    bool IToggleableComponent.IsEnabled {
      get;
      set;
    }
  }
}
