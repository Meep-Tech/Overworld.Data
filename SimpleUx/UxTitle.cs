namespace Overworld.Ux.Simple {
  /// <summary>
  /// A title that takes up it's own row, or can be added to a row or column to prefix it.
  /// </summary>
  public class UxTitle : IUxViewElement {

    /// <summary>
    /// Title Size
    /// </summary>
    public enum FontSize {
      Small,
      Medium,
      Large
    }

    /// <summary>
    /// The tile text
    /// </summary>
    public string Text;

    /// <summary>
    /// The tile tooltip
    /// </summary>
    public string Tooltip;

    /// <summary>
    /// The title size
    /// </summary>
    public FontSize Size {
      get;
    }

    /// <summary>
    /// Make a title for a UX.
    /// </summary>
    UxTitle(string label, string labelTooltip = null, FontSize size = FontSize.Medium) {
      Size = size;
      Tooltip = labelTooltip;
      Text = label;
    }

    ///<summary><inheritdoc/></summary>
    public UxTitle Copy()
      => new(Text, Tooltip, Size);

    IUxViewElement IUxViewElement.Copy()
      => Copy();
  }
}
