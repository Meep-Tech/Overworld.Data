namespace Overworld.Ux.Simple {

  /// <summary>
  /// A Data field that is an indexed collection of other fields.
  /// </summary>
  public interface IIndexedItemsDataField : IUxViewElement {

    /// <summary>
    /// Called after an individual field has validated itself.
    /// Used to update the internal Value at the key with the updated child field value
    /// </summary>
    public bool TryToUpdateValueAtIndex(object key, object newValue, out string resultMessage);
  }
}