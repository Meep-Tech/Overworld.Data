using System.Collections.Generic;

/// <summary>
/// A tagged collection that can only be read, not modified.
/// </summary>
public interface IReadOnlyTagedCollection<TTag, TValue> : IEnumerable<KeyValuePair<IEnumerable<TTag>, TValue>> {

  /// <summary>
  /// Fetch a set of values by tag.
  /// </summary>
  IEnumerable<TValue> this[TTag tag] { get; }

  /// <summary>
  /// Fetch all the tags for a given value
  /// </summary>
  IEnumerable<TTag> this[TValue value] { get; }

  /// <summary>
  /// All distinct tags
  /// </summary>
  IEnumerable<TTag> Tags { get; }

  /// <summary>
  /// All distinct values
  /// </summary>
  IEnumerable<TValue> Values { get; }

  /// <summary>
  /// Find the values that match the most tags in order
  /// </summary>
  IEnumerable<TValue> FindBestMatches(IEnumerable<TTag> orderedTags);

  /// <summary>
  /// Find the values that match the most tags in order
  /// </summary>
  IEnumerable<TValue> FindBestMatches(params TTag[] tags);

  /// <summary>
  /// Find matches given tags with specified weights
  /// The higher the weight, the more wanted the tag
  /// </summary>
  IEnumerable<TValue> FindWeightedMatches(IEnumerable<(TTag tag, int weight)> @params);

  /// <summary>
  /// Find the best matches, taking into account tag order
  /// </summary>
  IEnumerable<TValue> FindWeightedMatches(IEnumerable<TTag> orderedTags, int weightMultiplier = 2);

  /// <summary>
  /// Find the best matches, taking into account tag order
  /// </summary>
  IEnumerable<TValue> FindWeightedMatches(int weightMultiplier, params TTag[] orderedTags);

  /// <summary>
  /// Find matches given tags with specified weights
  /// The higher the weight, the more wanted the tag
  /// </summary>
  IEnumerable<TValue> FindWeightedMatches(params (TTag tag, int weight)[] @params);

  /// <summary>
  /// Find the best matches, taking into account tag order
  /// </summary>
  IEnumerable<TValue> FindWeightedMatches(params TTag[] orderedTags);

  /// <summary>
  /// Find the first value with the tags, or a default one with the best match
  /// </summary>
  TValue FirstWithTagsOrDefault(IEnumerable<TTag> tags);

  /// <summary>
  /// Find the first value with the tags, or a default one with the best match
  /// </summary>
  TValue FirstWithTagsOrDefault(params TTag[] tags);

  /// <summary>
  /// Find the values that match the most tags
  /// </summary>
  IEnumerable<TValue> GetAllSortedByBestMatch(IEnumerable<TTag> orderedTags);

  /// <summary>
  /// Find the values that match the most tags
  /// </summary>
  IEnumerable<TValue> GetAllSortedByBestMatch(params TTag[] tags);

  /// <summary>
  /// Find matches given tags with specified weights
  /// The higher the weight, the more wanted the tag
  /// </summary>
  IEnumerable<TValue> GetAllSortedByWeight(IEnumerable<(TTag tag, int weight)> @params);

  /// <summary>
  /// Find the best matches, taking into account tag order
  /// </summary>
  IEnumerable<TValue> GetAllSortedByWeight(IEnumerable<TTag> orderedTags);

  /// <summary>
  /// Find the best matches, taking into account tag order
  /// </summary>
  IEnumerable<TValue> GetAllSortedByWeight(IList<TTag> orderedTags, int weightMultiplier = 2);

  /// <summary>
  /// Find the best matches, taking into account tag order
  /// </summary>
  IEnumerable<TValue> GetAllSortedByWeight(int weightMultiplier, params TTag[] orderedTags);

  /// <summary>
  /// Find matches given tags with specified weights
  /// The higher the weight, the more wanted the tag
  /// </summary>
  IEnumerable<TValue> GetAllSortedByWeight(params (TTag tag, int weight)[] @params);

  /// <summary>
  /// Find the best matches, taking into account tag order
  /// </summary>
  IEnumerable<TValue> GetAllSortedByWeight(params TTag[] orderedTags);

  /// <summary>
  /// Select the values that match the most tags in order
  /// Slower than Find due to the cast
  /// </summary>
  TagedCollection<TTag, TValue> SelectBestMatches(IEnumerable<TTag> orderedTags);

  /// <summary>
  /// Select the values that match the most tags in order
  /// Slower than Find due to the cast
  /// </summary>
  TagedCollection<TTag, TValue> SelectBestMatches(params TTag[] tags);

  /// <summary>
  /// Find the values that match any of the tags, unordered
  /// </summary>
  TagedCollection<TTag, TValue> SelectMatches(IEnumerable<TTag> tags);

  /// <summary>
  /// Find the values that match any of the tags, unordered
  /// </summary>
  TagedCollection<TTag, TValue> SelectMatches(params TTag[] tags);

  /// <summary>
  /// Select matches given tags with specified weights
  /// The higher the weight, the more wanted the tag
  /// Slower than Find due to the cast
  /// </summary>
  TagedCollection<TTag, TValue> SelectWeightedMatches(IEnumerable<(TTag tag, int weight)> @params);

  /// <summary>
  /// Select the best matches, taking into account tag order
  /// Slower than Find due to the cast
  /// </summary>
  TagedCollection<TTag, TValue> SelectWeightedMatches(IEnumerable<TTag> orderedTags, int weightMultiplier = 2);

  /// <summary>
  /// Select the best matches, taking into account tag order
  /// Slower than Find due to the cast
  /// </summary>
  TagedCollection<TTag, TValue> SelectWeightedMatches(int weightMultiplier, params TTag[] orderedTags);

  /// <summary>
  /// Select matches given tags with specified weights
  /// The higher the weight, the more wanted the tag
  /// Slower than Find due to the cast
  /// </summary>
  TagedCollection<TTag, TValue> SelectWeightedMatches(params (TTag tag, int weight)[] @params);

  /// <summary>
  /// Select the best matches, taking into account tag order
  /// Slower than Find due to the cast
  /// </summary>
  TagedCollection<TTag, TValue> SelectWeightedMatches(params TTag[] orderedTags);
}