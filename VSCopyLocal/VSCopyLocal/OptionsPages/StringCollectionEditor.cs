using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace CodeValue.VSCopyLocal.OptionsPages
{
    public class StringCollectionEditor : CollectionEditor
    {
        public StringCollectionEditor(Type type)
            : base(typeof (List<string>))
        {
        }

        protected override Type CreateCollectionItemType()
        {
            return typeof (string);
        }

        protected override object CreateInstance(Type itemType)
        {
            string newString = string.Empty;
            return newString;
        }

        protected override string GetDisplayText(object value)
        {
            return value.ToString();
        }
    }
}