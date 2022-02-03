using System;
using System.Collections;
using System.Collections.Generic;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// Represents a key value set in a ui
  /// </summary>
  public class DataFieldKeyValueSet : DataField, IDictionary<string, object> {
    internal IEnumerable<Attribute> _childFieldAttributes;

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
      get => ((IDictionary<string, object>)Value)[key]; 
      set => ((IDictionary<string, object>)Value)[key] = value; 
    }

    /// <summary>
    /// Make a key value set to display in a ux.
    /// </summary>
    /// <param name="extraEntryValidation">Add validation other than the built in key validation</param>
    /// <param name="childFieldAttributes">Add attributes to each generated child input</param>
    public DataFieldKeyValueSet(
      string name,
      string tooltip = null,
      Dictionary<string, object> rows = null,
      IEnumerable<Attribute> childFieldAttributes = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enable = null,
      Func<DataField, KeyValuePair<string, object>, bool> extraEntryValidation = null
    ) : base(
      DisplayType.KeyValueFieldList,
      name,
      tooltip,
      rows, 
      dataKey,
      isReadOnly,
      enable,
      ((DataField _, object value) 
        => (_._controllerField as DataFieldKeyValueSet).ContainsKey(((KeyValuePair<string, object>)value).Key)) 
        + (extraEntryValidation is not null 
          ? (Func<DataField, object, bool>)((f,v) => extraEntryValidation(f, (KeyValuePair<string, object>)v)) 
          : ((_,_) => true))
    ) {
      _childFieldAttributes = childFieldAttributes;
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
    /// Used to update the colletction
    /// </summary>
    /// <param name="pair"></param>
    internal void _update(KeyValuePair<string, object> pair) {
      throw new NotImplementedException();
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