using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Represents a key value set in a ui
  /// </summary>
  public class DataFieldKeyValueSet : DataField, IDictionary<string, object>, IIndexedItemsDataField {
    internal IEnumerable<Attribute> _childFieldAttributes;
    IEnumerable<Func<DataField, KeyValuePair<string, object>, (bool success, string message)>> _entryValidations;

    /// <summary>
    /// The values
    /// </summary>
    public new Dictionary<string, object> Value
      => base.Value as Dictionary<string, object>;

    ///<summary><inheritdoc/></summary>
    public ICollection<string> Keys 
      => ((IDictionary<string, object>)Value).Keys;

    ///<summary><inheritdoc/></summary>
    public ICollection<object> Values 
      => ((IDictionary<string, object>)Value).Values;

    ///<summary><inheritdoc/></summary>
    public int Count
      => ((ICollection<KeyValuePair<string, object>>)Value).Count;

    ///<summary><inheritdoc/></summary>
    public object this[string key] { 
      get => Value[key]; 
      set => Value[key] = value; 
    }

    /// <summary>
    /// Make a key value set to display in a ux.
    /// </summary>
    /// <param name="entryValidations">Add validation other than the built in validation</param>
    /// <param name="childFieldAttributes">Add attributes to each generated child input</param>
    public DataFieldKeyValueSet(
      string name,
      string tooltip = null,
      Dictionary<string, object> rows = null,
      IEnumerable<Attribute> childFieldAttributes = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enable = null,
      IEnumerable<Func<DataField, KeyValuePair<string, object>, (bool success, string message)>> entryValidations = null,
      IEnumerable<Func<DataField, Dictionary<string, object>, (bool success, string message)>> fullValidations = null
    ) : base(
      DisplayType.KeyValueFieldList,
      name,
      tooltip,
      rows, 
      dataKey,
      isReadOnly,
      enable,
      fullValidations
        ?.Select(func => func.CastMiddleType<Dictionary<string, object>, object>())
    ) {
      _childFieldAttributes = childFieldAttributes;
      _entryValidations = entryValidations;
    }

    ///<summary><inheritdoc/></summary>
    public bool TryToUpdateValueAtIndex(object key, object newValue, out string resultMessage) {
      resultMessage = "";

      if(_entryValidations is not null) {
        //Default func
        foreach((bool success, string message) in _entryValidations.Select(validator => validator(this, new KeyValuePair<string, object>(key as string, newValue)))) {
          if(!success) {
            resultMessage = string.IsNullOrWhiteSpace(message)
              ? "Value did not pass custom entry validation functions."
              : message;

            return false;
          } else
            resultMessage = message;
        }
      }

      try {
        this[key as string] = newValue;
      } catch (Exception e) {
        resultMessage = e.Message;
        return false;
      }

      return true;
    }

    ///<summary><inheritdoc/></summary>
    public override DataField Copy(View toNewView = null, bool withCurrentValuesAsNewDefaults = false) {
      var value = base.Copy(toNewView, withCurrentValuesAsNewDefaults);
      value.Value = new Dictionary<string, object>(Value);
      value.DefaultValue = withCurrentValuesAsNewDefaults ? new Dictionary<string, object>(Value) : new Dictionary<string, object>((IDictionary<string, object>)DefaultValue);
      (value as DataFieldSet)._childFieldAttributes = _childFieldAttributes;

      return value;
    }

    /// <summary>
    /// remove the last item from the collection
    /// </summary>
    internal void _removeLast() {
      throw new NotImplementedException();
    }

    /// <summary>
    /// remove the item at the collection index
    /// </summary>
    internal void _remove(string key) {
      throw new NotImplementedException();
    }

    ///<summary><inheritdoc/></summary>
    public void Add(string key, object value) {
      ((IDictionary<string, object>)Value).Add(key, value);
    }

    ///<summary><inheritdoc/></summary>
    public bool ContainsKey(string key) {
      return ((IDictionary<string, object>)Value).ContainsKey(key);
    }

    ///<summary><inheritdoc/></summary>
    public bool Remove(string key) {
      return ((IDictionary<string, object>)Value).Remove(key);
    }

    ///<summary><inheritdoc/></summary>
    public bool TryGetValue(string key, out object value) {
      return ((IDictionary<string, object>)Value).TryGetValue(key, out value);
    }

    ///<summary><inheritdoc/></summary>
    public void Add(KeyValuePair<string, object> item) {
      ((ICollection<KeyValuePair<string, object>>)Value).Add(item);
    }

    ///<summary><inheritdoc/></summary>
    public void Clear() {
      ((ICollection<KeyValuePair<string, object>>)Value).Clear();
    }

    ///<summary><inheritdoc/></summary>
    public bool Contains(KeyValuePair<string, object> item) {
      return ((ICollection<KeyValuePair<string, object>>)Value).Contains(item);
    }

    ///<summary><inheritdoc/></summary>
    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
      ((ICollection<KeyValuePair<string, object>>)Value).CopyTo(array, arrayIndex);
    }

    ///<summary><inheritdoc/></summary>
    public bool Remove(KeyValuePair<string, object> item) {
      return ((ICollection<KeyValuePair<string, object>>)Value).Remove(item);
    }
    
    ///<summary><inheritdoc/></summary>
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
      return ((IEnumerable<KeyValuePair<string, object>>)Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable)Value).GetEnumerator();
    }
  }
}