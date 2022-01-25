namespace Overworld.Data {
  public static class CardinalDirectionExtensions {

    /// <summary>
    /// Get the direction if you turn clockwise 90 degrees from this one.
    /// </summary>
    public static World.CardinalDirection TurnClockwise(this World.CardinalDirection direction) {
      switch(direction) {
        case World.CardinalDirection.North:
          return World.CardinalDirection.East;
        case World.CardinalDirection.East:
          return World.CardinalDirection.South;
        case World.CardinalDirection.South:
          return World.CardinalDirection.West;
        case World.CardinalDirection.West:
          return World.CardinalDirection.North;
        default:
          throw new System.NotSupportedException();
      }
    }

    /// <summary>
    /// Get the direction if you turn counter-clockwise 90 degrees from this one.
    /// </summary>
    public static World.CardinalDirection TurnCounterClockwise(this World.CardinalDirection direction) {
      switch(direction) {
        case World.CardinalDirection.North:
          return World.CardinalDirection.West;
        case World.CardinalDirection.East:
          return World.CardinalDirection.North;
        case World.CardinalDirection.South:
          return World.CardinalDirection.East;
        case World.CardinalDirection.West:
          return World.CardinalDirection.South;
        default:
          throw new System.NotSupportedException();
      }
    }

    /// <summary>
    /// Get the direction if you turn clockwise 90 degrees from this one.
    /// </summary>
    public static World.CardinalCorner TurnClockwise(this World.CardinalCorner direction) {
      switch(direction) {
        case World.CardinalCorner.NorthWest:
          return World.CardinalCorner.NorthEast;
        case World.CardinalCorner.NorthEast:
          return World.CardinalCorner.SouthEast;
        case World.CardinalCorner.SouthEast:
          return World.CardinalCorner.SouthWest;
        case World.CardinalCorner.SouthWest:
          return World.CardinalCorner.NorthWest;
        default:
          throw new System.NotSupportedException();
      }
    }

    /// <summary>
    /// Get the direction if you turn counter-clockwise 90 degrees from this one.
    /// </summary>
    public static World.CardinalCorner TurnCounterClockwise(this World.CardinalCorner direction) {
      switch(direction) {
        case World.CardinalCorner.NorthWest:
          return World.CardinalCorner.SouthWest;
        case World.CardinalCorner.SouthWest:
          return World.CardinalCorner.SouthEast;
        case World.CardinalCorner.SouthEast:
          return World.CardinalCorner.NorthEast;
        case World.CardinalCorner.NorthEast:
          return World.CardinalCorner.NorthWest;
        default:
          throw new System.NotSupportedException();
      }
    }
  }
}