using Meep.Tech.Data;
using System;
using System.Collections.Generic;

namespace Overworld.Data.Entities.Components {

  /// <summary>
  /// Some basic physical stats for all characters
  /// </summary>
  public class BasicPhysicalStats : Entity.Component<BasicPhysicalStats> {

    /// <summary>
    /// Height, in "Tiles" (1.75 is average)
    /// </summary>
    public float Height {
      get => _height;
      set {
        float toSet = value;
        foreach((_, Func<float, float, float> action) in _onHeightChangeFuncs) {
          toSet += action(_height, value);
        }
        _height = toSet;
      }
    } float _height
      = 1.75f;

    /// <summary>
    /// Width, in "Tiles" (0.75 is average)
    /// </summary>
    public float Width {
      get => _width;
      set {
        float toSet = value;
        foreach((_, Func<float, float, float> action) in _onWidthChangeFuncs) {
          toSet += action(_width, value);
        }
        _width = toSet;
      }
    } float _width
      = 0.75f;

    /// <summary>
    /// Weight in "Units", used for physics and some other things.
    /// 175u is average.
    /// </summary>
    public float Weight {
      get => _weight;
      set {
        float toSet = value;
        foreach((_, Func<float, float, float> action) in _onWeightChangeFuncs) {
          toSet += action(_weight, value);
        }
        _weight = toSet;
      }
    } float _weight
      = 175;
    
    Dictionary<string, Func<float, float, float>>
      _onWidthChangeFuncs
        = new();

    Dictionary<string, Func<float, float, float>> 
      _onWeightChangeFuncs
        = new();

    Dictionary<string, Func<float, float, float>>
      _onHeightChangeFuncs 
        = new();

    /// <summary>
    /// Add an Func to be executed when the height is changed.
    /// </summary>
    /// <param name="key">The key of the Func, so it can be removed potentially</param>
    /// <param name="onHeightChange">The Func to execute.
    /// Params:
    /// old height,
    /// new height.
    /// Returns:
    /// extra height to add.
    /// </param>
    public void AddHeightChangeFunc(string key, Func<float, float, float> onHeightChange) {
      _onHeightChangeFuncs.Add(key, onHeightChange);
    }

    /// <summary>
    /// Add an Func to be executed when the weight is changed.
    /// </summary>
    /// <param name="key">The key of the Func, so it can be removed potentially</param>
    /// <param name="onWeightChange">The Func to execute.
    /// Params:
    /// old weight,
    /// new weight.
    /// Returns:
    /// extra weight to add.
    /// </param>
    public void AddWeightChangeFunc(string key, Func<float, float, float> onWeightChange) {
      _onWeightChangeFuncs.Add(key, onWeightChange);
    }

    /// <summary>
    /// Add an Func to be executed when the width is changed.
    /// </summary>
    /// <param name="key">The key of the Func, so it can be removed potentially</param>
    /// <param name="onWidthChange">The Func to execute.
    /// Params:
    /// old width,
    /// new width.
    /// Returns:
    /// extra width to add.
    /// </param>
    public void AddWidthChangeFunc(string key, Func<float, float, float> onWidthChange) {
      _onWidthChangeFuncs.Add(key, onWidthChange);
    }

    /// <summary>
    /// Remove a width change Func
    /// </summary>
    public bool RemoveWidthChangeFunc(string key, out Func<float, float, float> onWidthChange) {
      if(_onWidthChangeFuncs.TryGetValue(key, out onWidthChange)) {
        _onWidthChangeFuncs.Remove(key);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Remove a height change Func
    /// </summary>
    public bool RemoveHeightChangeFunc(string key, out Func<float, float, float> onHeightChange) {
      if(_onHeightChangeFuncs.TryGetValue(key, out onHeightChange)) {
        _onHeightChangeFuncs.Remove(key);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Remove a weight change Func
    /// </summary>
    public bool RemoveWeightChangeFunc(string key, out Func<float, float, float> onWeightChange) {
      if(_onWeightChangeFuncs.TryGetValue(key, out onWeightChange)) {
        _onWeightChangeFuncs.Remove(key);
        return true;
      }

      return false;
    }
  }
}
