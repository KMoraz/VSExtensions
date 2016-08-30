using CodeValue.VSCopyLocal.Annotations;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CodeValue.VSCopyLocal.OptionsPages
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    public sealed class OptionsPage : DialogPage, INotifyPropertyChanged
    {
        private bool _copyLocalFlag;
        private bool _previewMode;

        public OptionsPage()
        {
            CopyLocalFlag = false;
            PreviewMode = false;
            ProjectsToSkipMaskList = new List<string> { "*test*", "*setup*"};
        }

        [Category("VSCopyLocal")]
        [DisplayName(@"Default CopyLocal Value")]
        [Description("Set the default Copy Local flag")]
        public bool CopyLocalFlag
        {
            get { return _copyLocalFlag; }
            set
            {
                _copyLocalFlag = value;
                OnPropertyChanged();
            }
        }

        [Category("VSCopyLocal")]
        [DisplayName(@"Preview Mode")]
        [Description("Preview mode does not modify the projects but writes the results to the output window.")]
        public bool PreviewMode
        {
            get { return _previewMode; }
            set
            {
                _previewMode = value;
                OnPropertyChanged();
            }
        }

        [Category("VSCopyLocal")]
        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(StringCollectionConverter))]
        [DisplayName(@"Project Names to Skip")]
        [Description("List of project names to skip from processing. Wildcards are supported.")]
        public List<string> ProjectsToSkipMaskList { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Skip(string name)
        {
            bool wildCardStart = false;
            bool wildCardEnd = false;

            foreach (var s in ProjectsToSkipMaskList)
            {
                if (s.StartsWith("*")) wildCardStart = true;
                if (s.EndsWith("*")) wildCardEnd = true;

                var st = s.Trim('*').ToLowerInvariant();
                name = name.ToLowerInvariant();

                if (wildCardStart && wildCardEnd && name.Contains(st))
                    return true;
                if (wildCardStart && name.EndsWith(st))
                    return true;
                if (wildCardEnd && name.StartsWith(st))
                    return true;
                if (st.Equals(name))
                    return true;
            }

            return false;
        }
    }
}