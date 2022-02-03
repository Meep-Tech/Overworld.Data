using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Overworld.Utility {

  public static class StringExtensions {

    /// <summary>
    /// Make a string from "CamelCase" to "Display Case"
    /// </summary>
    public static string ToDisplayCase(this string @string)
      => Regex.Replace(@string, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1").Trim('_').Replace("_", " ");
  }

 /* public static partial class DictionaryExtensions {

    /// <summary>
    /// Add an item to a ICollection within a dictionary at the given key
    /// </summary>
    public static void AddToValueCollection<TKey, TValue>(this IDictionary<TKey, ICollection<TValue>> dictionary, TKey key, TValue value) {
      if(dictionary.TryGetValue(key, out ICollection<TValue> valueCollection)) {
        valueCollection.Add(value);
      } else
        dictionary.Add(key, new List<TValue> { value });
    }

    /// <summary>
    /// Add an item to a ICollection within a dictionary at the given key
    /// </summary>
    public static bool ValueCollectionContains<TKey, TValue>(this IDictionary<TKey, ICollection<TValue>> dictionary, TKey key, TValue value) {
      if(dictionary.TryGetValue(key, out ICollection<TValue> valueCollection)) {
        return valueCollection.Contains(value);
      }

      return false;
    }

    /// <summary>
    /// Add an item to a ICollection within a dictionary at the given key
    /// </summary>
    public static void AddToHashSet<TKey, TValue>(this IDictionary<TKey, HashSet<TValue>> dictionary, TKey key, TValue value) {
      if(dictionary.TryGetValue(key, out HashSet<TValue> valueCollection)) {
        valueCollection.Add(value);
      } else
        dictionary.Add(key, new HashSet<TValue> { value });
    }

    /// <summary>
    /// Remove an item from an ICollection within a dictionary at the given key
    /// </summary>
    public static bool RemoveFromValueCollection<TKey, TValue>(this IDictionary<TKey, ICollection<TValue>> dictionary, TKey key, TValue value) {
      if(dictionary.TryGetValue(key, out ICollection<TValue> valueCollection)) {
        if(valueCollection.Remove(value)) {
          if(!valueCollection.Any()) {
            dictionary.Remove(key);
          }
          return true;
        }
      }

      return false;
    }
  }*/
}
