namespace Overworld.Ux.Simple {

  /// <summary>
  /// An element of a Simple Ux View.
  /// </summary>
  public interface IUxViewElement {

    /// <summary>
    /// Make a copy of the element and it's state.
    /// </summary>
    IUxViewElement Copy();
  }
}