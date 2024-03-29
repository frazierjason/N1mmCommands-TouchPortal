﻿/* This project published at https://github.com/frazierjason/N1mmCommands-TouchPortal
 * under the MIT license.
 */

using System;
using TouchPortalExtension.Attributes;

namespace N1mmCommands.Touchportal
{
    public class PluginSetting
    {
        public string? SettingID { get; set; }
        public string? Name { get; set; } = null;
        public string? Default { get; set; } = null;
        public DataType ValueType { get; set; } = DataType.Text;
        public int MaxLength { get; set; } = int.MinValue;
        public double MinValue { get; set; } = double.NaN;
        public double MaxValue { get; set; } = double.NaN;
        public string? TouchPortalStateId { get; set; } = null;

        private dynamic? _value = null;
        public dynamic? Value
        {
            get
            {
                if (_value == null && Default != null)
                    SetValueFromString(Default);
                return _value;
            }
            set
            {
                if (null == value)
                {
                    // let it be corrected on the get() call above
                    _value = null;
                }
                else if (ValueType == DataType.Number)
                {
                    if (!value.GetType().IsAssignableTo(typeof(double)))
                    {
                        _value = null;
                    }
                    else
                    {
                        double realVal = (double)value;
                        if (!double.IsNaN(MinValue))
                            realVal = Math.Max(realVal, MinValue);
                        if (!double.IsNaN(MaxValue))
                            realVal = Math.Min(realVal, MaxValue);
                        _value = realVal;
                    }
                }
                // string
                else
                {
                    if (!value.GetType().IsAssignableTo(typeof(string)))
                    {
                        _value = null;
                    }
                    else
                    {
                        string strVal = (string)value;
                        if (MaxLength > 0 && strVal.Length > MaxLength)
                            strVal = strVal.Remove(MaxLength + 1);
                        _value = strVal;
                    }
                }
            }
        }

        public void SetValueFromString(string? value)
        {
            if (ValueType == DataType.Number)
            {
                if (double.TryParse(value, out var numVal))
                    Value = numVal;
            }
            else
            {
                Value = value;
            }
        }

        public int ValueAsInt() => (Value == null || ValueType != DataType.Number) ? 0 : (int)Value;
        public double ValueAsDbl() => Value == null ? double.NaN : (double)Value;
        public string ValueAsStr() => Value == null ? string.Empty : Value.ToString();

        public PluginSetting(string id, DataType type = DataType.Text) { SetProperties(id, null, null, type); }
        public PluginSetting(string id, double minValue, double maxValue, string? defaultValue = null) { SetProperties(id, null, defaultValue, DataType.Number, minValue, maxValue); }
        public PluginSetting(string id, int maxLength, string? defaultValue = null) { SetProperties(id, null, defaultValue, DataType.Text, double.NaN, double.NaN, maxLength); }
        public PluginSetting(string id, string defaultValue, DataType type = DataType.Text) { SetProperties(id, default, defaultValue, type); }
        public PluginSetting(string id, string name, string defaultValue, DataType type = DataType.Text) { SetProperties(id, name, defaultValue, type); }

        private void SetProperties(string? id, string? name, string? defaultValue, DataType type, double min = double.NaN, double max = double.NaN, int maxLen = int.MinValue)
        {
            SettingID = id;
            ValueType = type;
            Name = name;
            Default = defaultValue;
            ValueType = type;
            MinValue = min;
            MaxValue = max;
            MaxLength = maxLen;
            if (Value == null && defaultValue != null)
                SetValueFromString(defaultValue);
        }
    }
}