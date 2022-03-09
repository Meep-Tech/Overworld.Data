namespace Overworld.Data.Entities.Components {

  /// <summary>
  /// Keeps track of an entity location on the map.
  /// Modifying a value here will move an entity in the world to the new location on the next frame update.
  /// </summary>
  public class Location : IDefaultEntityComponent<Location> {
    private int _x;
    private int _y;
    private int _height;

    public int X {
      get => _x;
      set => _x = value;
    }

    public int Y {
      get => _y;
      set => _y = value;
    }

    public int Height {
      get => _height;
      set => _height = value;
    }
  }
}
