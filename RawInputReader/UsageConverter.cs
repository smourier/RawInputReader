using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using RawInputReader.Utilities;

namespace RawInputReader
{
    public class UsageConverter : TypeConverter
    {
        private static IEnumerable<object> GetValues(Type type)
        {
            foreach (var value in Enum.GetValues(type))
            {
                yield return value;
            }
        }

        private static Type? GetType(HID_USAGE_PAGE usage)
        {
            switch (usage)
            {
                case HID_USAGE_PAGE.HID_USAGE_PAGE_GENERIC:
                    return typeof(HID_USAGE_GENERIC);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_SIMULATION:
                    return typeof(HID_USAGE_SIMULATION);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_VR:
                    return typeof(HID_USAGE_VR);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_SPORT:
                    return typeof(HID_USAGE_SPORT);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_GAME:
                    return typeof(HID_USAGE_GAME);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_GENERIC_DEVICE:
                    return typeof(HID_USAGE_GENERIC_DEVICE);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_KEYBOARD:
                    return typeof(HID_USAGE_KEYBOARD);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_LED:
                    return typeof(HID_USAGE_LED);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_TELEPHONY:
                    return typeof(HID_USAGE_TELEPHONY);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_CONSUMER:
                    return typeof(HID_USAGE_CONSUMER);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_DIGITIZER:
                    return typeof(HID_USAGE_DIGITIZER);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_HAPTICS:
                    return typeof(HID_USAGE_HAPTICS);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_ALPHANUMERIC:
                    return typeof(HID_USAGE_ALPHANUMERIC);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_LIGHTING_ILLUMINATION:
                    return typeof(HID_USAGE_CAMERA);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_CAMERA_CONTROL:
                    return typeof(HID_USAGE_CAMERA);

                case HID_USAGE_PAGE.HID_USAGE_PAGE_MICROSOFT_BLUETOOTH_HANDSFREE:
                    return typeof(HID_USAGE_MS_BTH_HF);

                default:
                    return null;
            }
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value?.Equals((ushort)0) == true)
                return "<INVALID>";

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (context?.Instance is RawInput input)
            {
                var type = GetType(input.Page);
                if (type != null)
                {
                    if (Conversions.TryParseEnum(type, value, out var evalue))
                        return evalue;

                    if (ushort.TryParse(value.ToString(), out var us))
                        return us;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;
        public override StandardValuesCollection? GetStandardValues(ITypeDescriptorContext? context)
        {
            var list = new List<object>();
            if (context?.Instance is RawInput input)
            {
                var type = GetType(input.Page);
                if (type != null)
                {
                    list.AddRange(GetValues(type));
                }
            }

            return new StandardValuesCollection(list);
        }
    }
}
