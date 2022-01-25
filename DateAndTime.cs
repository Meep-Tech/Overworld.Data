using System;

namespace Overworld.Data {
  /// <summary>
  /// A date and time in the game world.
  /// </summary>
  public struct DateAndTime {
    /// <summary>
    /// The numerical date in the game world.
    /// </summary>
    public readonly int Date;

    /// <summary>
    /// The numeric time in the game world.
    /// </summary>
    public readonly float Time;

    /// <summary>
    /// Make a date with an optional time
    /// </summary>
    public DateAndTime(int date, float time = 0) : this() {
      Time = time;
      Date = date;
    }

    /// <summary>
    /// The local date and time of the current world
    /// </summary>
    public static DateAndTime Local
      // TODO: uses game world data, this class needs to be implimented in unity data libraries.
      => throw new NotImplementedException();
  }
}
