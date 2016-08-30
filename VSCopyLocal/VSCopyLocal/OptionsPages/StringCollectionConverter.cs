using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace CodeValue.VSCopyLocal.OptionsPages
{
    public class StringCollectionConverter : TypeConverter
    {
        private const string Seperator = ",";

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var str = value as string;
            return string.IsNullOrEmpty(str) ? Enumerable.Empty<string>() : new List<string>(str.Split(new[] { Seperator }, StringSplitOptions.RemoveEmptyEntries));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is IEnumerable<string>)
            {
                var collection = value as IEnumerable<string>;
                return string.Join(Seperator, collection);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}