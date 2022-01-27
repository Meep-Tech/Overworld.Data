namespace Overworld.Ux.Simple {

  /// <summary>
  /// An element of a Simple Ux View.
  /// </summary>
  public interface IUxViewElement {

    /// <summary>
    /// The view this element is in.
    /// </summary>
    UxView View {
      get;
    }

    /// <summary>
    /// Make a copy of the element and it's state.
    /// Provide the new view if there is one, if not, it will be set on setting in the builder.
    /// </summary>
    IUxViewElement Copy(UxView toNewView = null);
  }
}