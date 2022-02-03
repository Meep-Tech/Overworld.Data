using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Ux.Simple {

  /// <summary>
  /// A data field for input or display in a simple ux pannel/view
  /// </summary>
  public class DataField : IUxViewElement {

    /// <summary>
    /// The view this field is in.
    /// </summary>
    public View View {
      get;
      internal set;
    }

    /// <summary>
    /// The type of displays available for simple ux data types.
    /// </summary>
    public enum DisplayType {
      Text,
      Toggle,
      RangeSlider,
      Dropdown,
      FieldList,
      KeyValueFieldList,
      Executeable,
      ColorPicker,
      Image,
      Button
    }

    /// <summary>
    /// The type of display this field should use.
    /// </summary>
    public DisplayType Type {
      get;
    }

    /// <summary>
    /// Functions that take the current field, and updated object data, and validate it.
    /// </summary>
    public IReadOnlyCollection<Func<DataField, object, (bool success, string message)>> Validations
      => _validations; List<Func<DataField, object, (bool success, string message)>> _validations
        = new();

    /// <summary>
    /// The current value of the field.
    /// </summary>
    public object Value {
      get;
      internal set;
    }

    /// <summary>
    /// The default initial value.
    /// </summary>
    public object DefaultValue {
      get;
      internal set;
    }

    /// <summary>
    /// If this field is readonly
    /// </summary>
    public virtual bool IsReadOnly {
      get;
      internal set;
    } = false;

    /// <summary>
    /// The name of the field.
    /// Used as a default data key
    /// </summary>
    public virtual string Name {
      get;
    }

    /// <summary>
    /// Data key for the field.
    /// Used to access it from the editor component display data.
    /// </summary>
    public virtual string DataKey {
      get;
    }

    /// <summary>
    /// Info tooltip for the field
    /// </summary>
    public virtual string Tooltip {
      get;
    } = null;

    /// <summary>
    /// Used to determine if the field should be enabled.
    /// </summary>
    public Func<DataField, View, bool> Enable {
      get;
    }

    internal DataField _controllerField;

    /// <summary>
    /// Make a new data field for a Simple Ux.
    /// </summary>
    /// <param name="type">the DisplayType to use for this field</param>
    /// <param name="name">the field name. should be unique unless you change the data key</param>
    /// <param name="tooltip">a breif description of the field, will appear on mouse hover in the ui</param>
    /// <param name="value">default/current value of the field</param>
    /// <param name="dataKey">Used to get the value of this field from the view</param>
    /// <param name="isReadOnly">Some read only fields may be formatted differently (like Text). try passing '() => false' to enable if you want a blured out input field instead.</param>
    /// <param name="enable">A function to determine if this field should be enabled currently or not. Parameters are this field, and the parent pannel.</param>
    /// <param name="validations">functions used to validate changes to the field.</param>
    public DataField(
      DisplayType type,
      string name,
      string tooltip = null,
      object value = null,
      string dataKey = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enable = null,
      params Func<DataField, object, (bool success, string message)>[] validations
    ) : this(type, name, tooltip, value, dataKey, isReadOnly, enable, validations.AsEnumerable()) {}

    /// <summary>
    /// Make a new data field for a Simple Ux.
    /// </summary>
    /// <param name="type">the DisplayType to use for this field</param>
    /// <param name="name">the field name. should be unique unless you change the data key</param>
    /// <param name="tooltip">a breif description of the field, will appear on mouse hover in the ui</param>
    /// <param name="value">default/current value of the field</param>
    /// <param name="dataKey">Used to get the value of this field from the view</param>
    /// <param name="isReadOnly">Some read only fields may be formatted differently (like Text). try passing '() => false' to enable if you want a blured out input field instead.</param>
    /// <param name="enable">A function to determine if this field should be enabled currently or not. Parameters are this field, and the parent pannel.</param>
    /// <param name="validations">functions used to validate changes to the field.</param>
    protected DataField(
        DisplayType type,
        string name,
        string tooltip = null,
        object value = null,
        string dataKey = null,
        bool isReadOnly = false,
        Func<DataField, View, bool> enable = null,
        IEnumerable<Func<DataField, object, (bool success, string message)>> validations = null
    ) {
      Type = type;
      Name = name;
      Tooltip = tooltip;
      DefaultValue = Value = value;
      IsReadOnly = isReadOnly;
      DataKey = string.IsNullOrWhiteSpace(dataKey)
        ? name
        : dataKey;

      if(!isReadOnly && DataKey is null) {
        throw new ArgumentException($"Non-read-only fields require a data key. Provide a title, name, or datakey to the field constructor or Make function");
      }

      Enable = enable ?? ((_, _) => true);
      _validations = validations?.ToList();
    }

    /// <summary>
    /// Try to update the field value to a new one.
    /// Checks validations and returns an error message if there is one.
    /// </summary>
    public virtual bool TryToSetValue(object value, out string resultMessage) {
      resultMessage = "";

      if(Validations is not null) {
        //Default func
        foreach((bool success, string message) in Validations.Select(validator => validator(this, value))) {
          if(!success) {
            resultMessage = string.IsNullOrWhiteSpace(message)
              ? "Value did not pass custom validation functions."
              : message;

            return false;
          } else
            resultMessage = message;
        }
      }

      /// for controller fields, that need to be validated by their parent.
      if(_controllerField is not null) {
        (object key, object value)? pair = null;
        if(value is KeyValuePair<string, object> stringKeyedPair) {
          pair = (stringKeyedPair.Key, stringKeyedPair.Value);
        } else if (value is KeyValuePair<int, object> intKeyedPair) {
          pair = (intKeyedPair.Key, intKeyedPair.Value);
        }
        if(pair.HasValue) {
          if (!((_controllerField as IIndexedItemsDataField)?.TryToUpdateValueAtIndex(pair.Value.key, pair.Value.value, out resultMessage) ?? true)) {
            return false;
          }
        }
      }

      Value = value;
      return true;
    }

    /// <summary>
    /// Memberwise clone to copy
    /// </summary>
    /// <returns></returns>
    public virtual DataField Copy(View toNewView = null, bool withCurrentValuesAsNewDefaults = false) {
      var newField = MemberwiseClone() as DataField;
      newField.View = toNewView;
      newField._validations = _validations?.ToList();
      newField.DefaultValue = withCurrentValuesAsNewDefaults ? Value : DefaultValue;

      return newField;
    }

    /// <summary>
    /// Reset the value of this field to it's default
    /// </summary>
    public void ResetValueToDefault()
      => Value = DefaultValue;

    ///<summary><inheritdoc/></summary>
    IUxViewElement IUxViewElement.Copy(View toNewView)
      => Copy(toNewView);

    /// <summary>
    /// Make a new field that fits your needs.
    /// Some field types require attribute data.
    /// </summary>
    public static DataField Make(
      DisplayType type,
      string title = null,
      string tooltip = null,
      object value = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enabledIf = null,
      string dataKey = null,
      Dictionary<Type, Attribute> attributes = null,
      params Func<DataField, object, (bool success, string message)>[] validations
    ) => Make(type, title, tooltip, value, isReadOnly, enabledIf, dataKey, attributes, validations);

    /// <summary>
    /// Make a new field that fits your needs.
    /// Some field types require attribute data.
    /// </summary>
    public static DataField Make(
      DisplayType type, 
      string title = null,
      string tooltip = null, 
      object value = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enabledIf = null,
      string dataKey = null,
      Dictionary<Type, Attribute> attributes = null,
      IEnumerable<Func<DataField, object, (bool success, string message)>> validations = null
    ) {
      switch(type) {
        case DisplayType.Text:
          if(isReadOnly) {
            return new ReadOnlyTextField(
              title: title,
              tooltip: tooltip,
              text: value,
              dataKey: dataKey
            );
          } else
            return new TextField(
              name: title,
              validations: validations?.Select(func => func.CastMiddleType<object, string>()),
              tooltip: tooltip,
              value: value,
              enabledIf: enabledIf,
              dataKey: dataKey
            );

        case DisplayType.Toggle:
          bool boolValue = value is bool asBool
            ? asBool
            : float.TryParse(value.ToString(), out float parsedAsFloat) && parsedAsFloat > 0;
          return new ToggleField(
            name: title,
            validations: validations?.Select(func => func.CastMiddleType<object, bool>()),
            tooltip: tooltip,
            value: boolValue,
            enabledIf: enabledIf,
            dataKey: dataKey
          );

        case DisplayType.RangeSlider:
          RangeSliderAttribute rangeSliderAttribute
            = attributes.TryGetValue(typeof(RangeSliderAttribute), out var foundrsa)
              ? foundrsa as RangeSliderAttribute
              : null;

          bool clamped = rangeSliderAttribute?._isClampedToInt ?? false;
          (float min, float max)? minAndMax = rangeSliderAttribute is not null
            ? (rangeSliderAttribute._min, rangeSliderAttribute._max)
            : null;

          float? floatValue = value is double asFloat
            ? (float)asFloat
            : float.TryParse(value.ToString(), out float parsedFloat)
             ? parsedFloat
             : null;

          return new RangeSliderField(
            name: title,
            min: minAndMax?.min ?? 0,
            max: minAndMax?.max ?? 1,
            clampedToWholeNumbers: clamped,
            tooltip: tooltip,
            value: floatValue,
            enabledIf: enabledIf,
            dataKey: dataKey,
            validations: validations?.Select(func => func.CastMiddleType<object, double>())
          );

        case DisplayType.KeyValueFieldList:
          return new DataFieldKeyValueSet(
            name: title,
            rows: value as Dictionary<string, object>,
            entryValidations: validations?.Select(func => func.CastMiddleType<object, KeyValuePair<string, object>>()),
            tooltip: tooltip,
            dataKey: dataKey,
            childFieldAttributes: attributes.Values,
            isReadOnly: isReadOnly,
            enable: enabledIf
          );

        case DisplayType.Dropdown:
          DropdownAttribute selectableData = attributes.TryGetValue(typeof(DropdownAttribute), out var found)
            ? found as DropdownAttribute
            : null;

          Dictionary<string, object> options = selectableData?._options;
          return new DropdownSelectField(
            name: title,
            options: options ?? throw new ArgumentNullException(nameof(options)),
            tooltip: tooltip,
            multiselectIsAllowed: selectableData?._isMultiselect ?? false,
            alreadySelectedOptionKeys: value as string[],
            dataKey: dataKey,
            isReadOnly: isReadOnly,
            enabledIf: enabledIf,
            validations: validations?.Select(func => func.CastMiddleType<object, KeyValuePair<string, object>>())
          );

        case DisplayType.FieldList:
        case DisplayType.Executeable:
        case DisplayType.ColorPicker:
        case DisplayType.Image:
        case DisplayType.Button:
          throw new NotImplementedException(type.ToString());
        default:
          return new DataField(
            name: title,
            type: type,
            validations: validations,
            tooltip: tooltip,
            value: value,
            enable: enabledIf
        );
      }
    }
  }
}
