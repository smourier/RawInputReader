using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace RawInputReader.Utilities
{
    public class EnumEditor : UITypeEditor
    {
        private const string _defaultZeroName = "None";

        private bool _cancelled;
        private bool _inEvent;
        private int _index = -1;
        private readonly ToolTip _toolTip = new();
        private IWindowsFormsEditorService? _editorService;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;
        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            _editorService = provider?.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            if (value != null && _editorService != null)
            {
                var uvalue = Conversions.EnumToUInt64(value);
                string? zeroName = null;
                var zeroMode = false;
                var type = value.GetType();
                var values = Enum.GetValues(type);
                var names = Enum.GetNames(type);

                var excludedMembers = new HashSet<string>();
                var displayNames = new Dictionary<string, string>();
                foreach (var fi in type.GetFields())
                {
                    foreach (var att in fi.GetCustomAttributes<BrowsableAttribute>())
                    {
                        if (!att.Browsable)
                        {
                            excludedMembers.Add(fi.Name);
                        }
                    }

                    foreach (var att in fi.GetCustomAttributes<EditorBrowsableAttribute>())
                    {
                        if (att.State == EditorBrowsableState.Never)
                        {
                            excludedMembers.Add(fi.Name);
                        }
                    }

                    foreach (var att in fi.GetCustomAttributes<DescriptionAttribute>())
                    {
                        displayNames[fi.Name] = att.Description;
                    }
                }

                var isFlags = Conversions.IsFlagsEnum(type);
                if (isFlags)
                {
                    var bitsCount = (ulong)Conversions.GetEnumMaxPower(type) - 1;

                    var listBox = new EnumEditorCheckedListBox
                    {
                        IntegralHeight = true
                    };
                    listBox.ItemCheck += OnListBoxItemCheck;
                    listBox.MouseHover += OnListBoxMouseHover;
                    listBox.MouseMove += OnListBoxMouseMove;
                    listBox.KeyDown += OnListBoxKeyDown;
                    listBox.PreviewKeyDown += OnListBoxPreviewKeyDown;
                    listBox.CheckOnClick = true;

                    for (var i = 0; i < names.Length; i++)
                    {
                        var name = names[i];
                        if (excludedMembers.Contains(name))
                            continue;

                        var nameValue = Conversions.EnumToUInt64(values.GetValue(i)!);
                        var item = new Item
                        {
                            EnumName = name,
                            EnumValue = nameValue
                        };

                        if (displayNames.TryGetValue(name, out var dn))
                        {
                            item.DisplayName = dn;
                        }

                        var isChecked = true;
                        ulong b = 1;
                        for (ulong bit = 1; bit < bitsCount; bit++)
                        {
                            var bitName = Enum.GetName(type, b);
                            if (bitName != null && name != bitName && (nameValue & b) != 0 && !excludedMembers.Contains(bitName))
                            {
                                if (item.DisplayNames == null)
                                {
                                    item.DisplayNames = [];
                                    item.Values = [];
                                }
                                if (displayNames.TryGetValue(bitName, out dn))
                                {
                                    item.DisplayNames.Add(dn);
                                }
                                else
                                {
                                    item.DisplayNames.Add(bitName);
                                }
                                item.Values!.Add(b);

                                if ((uvalue & b) == 0)
                                {
                                    isChecked = false;
                                }
                            }
                            b *= 2;
                        }

                        if (item.DisplayNames == null)
                        {
                            isChecked = (uvalue & nameValue) != 0;
                        }

                        if (nameValue == 0)
                        {
                            item.IsZeroName = true;
                        }

                        item.Name = name;
                        listBox.Items.Add(item, isChecked);
                    }

                    if (listBox.Items.Count == 0)
                    {
                        zeroName ??= _defaultZeroName;

                        var zero = new Item
                        {
                            Name = zeroName,
                            IsZeroName = true
                        };
                        listBox.Items.Add(zero, false);
                        zeroMode = true;
                    }

                    if (listBox.Items.Count <= 20)
                    {
                        listBox.Height = listBox.PreferredHeight + listBox.ItemHeight;
                    }

                    var width = listBox.Width;
                    for (var i = 0; i < listBox.Items.Count; i++)
                    {
                        var rc = listBox.GetItemRectangle(i);
                        if (rc.Width > width)
                        {
                            width = rc.Width;
                        }
                    }
                    listBox.Width = width;

                    _editorService.DropDownControl(listBox);
                    if (_cancelled)
                    {
                        _cancelled = false;
                        return value;
                    }

                    if (zeroMode)
                    {
                        return 0;
                    }

                    ulong newValue = 0;
                    foreach (Item item in listBox.CheckedItems)
                    {
                        newValue |= Conversions.ChangeType<ulong>(Enum.Parse(type, item.EnumName!));
                    }

                    return Conversions.EnumToObject(type, newValue);
                }
                else
                {
                    var listBox = new EnumEditorListBox
                    {
                        IntegralHeight = true,
                        Sorted = true,
                        DisplayMember = "DisplayName",
                        SelectionMode = SelectionMode.One
                    };

                    listBox.MouseHover += OnListBoxMouseHover;
                    listBox.MouseMove += OnListBoxMouseMove;
                    listBox.MouseClick += OnListBoxMouseClick;
                    listBox.KeyDown += OnListBoxKeyDown;
                    listBox.PreviewKeyDown += OnListBoxPreviewKeyDown;

                    for (var i = 0; i < names.Length; i++)
                    {
                        if (excludedMembers.Contains(names[i]))
                            continue;

                        var item = new Item
                        {
                            Name = names[i],
                            EnumName = names[i]
                        };
                        if (displayNames.TryGetValue(item.EnumName, out var dn))
                        {
                            item.DisplayName = dn;
                        }

                        var index = listBox.Items.Add(item);
                        if (i < values.Length && uvalue == Conversions.EnumToUInt64(values.GetValue(i)!))
                        {
                            listBox.SetSelected(index, true);
                        }
                    }

                    if (listBox.Items.Count == 0)
                    {
                        zeroName ??= _defaultZeroName;

                        var zero = new Item
                        {
                            Name = zeroName,
                            IsZeroName = true
                        };
                        listBox.Items.Add(zero);
                        zeroMode = true;
                    }

                    if (listBox.SelectedItems.Count == 0)
                    {
                        listBox.SetSelected(0, true);
                    }

                    if (listBox.Items.Count <= 20)
                    {
                        listBox.Height = listBox.PreferredHeight;
                    }

                    _editorService.DropDownControl(listBox);
                    if (_cancelled)
                    {
                        _cancelled = false;
                        return value;
                    }

                    if (zeroMode)
                        return 0;

                    if (listBox.SelectedItems.Count == 0)
                        return 0;

                    var selected = (Item)listBox.SelectedItem!;
                    return Enum.Parse(type, selected.EnumName!);
                }
            }
            return base.EditValue(context, provider!, value);
        }

        private void OnListBoxMouseMove(object? sender, MouseEventArgs e)
        {
            var index = ((ListBox)sender!).IndexFromPoint(e.Location);
            if (_index != index)
            {
                ShowToolTip((ListBox)sender);
            }
        }

        private void OnListBoxMouseHover(object? sender, EventArgs e) => ShowToolTip((ListBox)sender!);
        private void ShowToolTip(ListBox listBox)
        {
            var pos = listBox.PointToClient(Cursor.Position);
            _index = listBox.IndexFromPoint(pos);
            if (_index > -1)
            {
                var obj = listBox.Items[_index];
                if (obj != null)
                {
                    _toolTip.SetToolTip(listBox, obj.ToString());
                }
            }
        }

        private void OnListBoxKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _editorService?.CloseDropDown();
            }
        }

        private void OnListBoxPreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _cancelled = true;
                _editorService?.CloseDropDown();
            }
        }

        private void OnListBoxMouseClick(object? sender, MouseEventArgs e)
        {
            var index = ((ListBox)sender!).IndexFromPoint(e.Location);
            if (index >= 0)
            {
                _editorService?.CloseDropDown();
            }
        }

        private static void SetZeroState(CheckedListBox listBox, CheckState state)
        {
            for (var i = 0; i < listBox.Items.Count; i++)
            {
                if (((Item)listBox.Items[i]).IsZeroName)
                {
                    listBox.SetItemCheckState(i, state);
                }
            }
        }

        private static ulong GetNewValue(CheckedListBox listBox, ItemCheckEventArgs e)
        {
            ulong value = 0;
            for (var i = 0; i < listBox.Items.Count; i++)
            {
                if (listBox.GetItemChecked(i))
                {
                    value |= ((Item)listBox.Items[i]).EnumValue;
                }
            }
            if (e.NewValue == CheckState.Checked)
            {
                value |= ((Item)listBox.Items[e.Index]).EnumValue;
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                value &= ~((Item)listBox.Items[e.Index]).EnumValue;
            }
            return value;
        }

        private void OnListBoxItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (_inEvent)
                return;

            _inEvent = true;
            try
            {
                var listBox = (CheckedListBox)sender!;
                var item = (Item)listBox.Items[e.Index];

                if (e.NewValue == CheckState.Checked && item.EnumValue == 0)
                {
                    for (var i = 0; i < listBox.Items.Count; i++)
                    {
                        listBox.SetItemChecked(i, false);
                    }
                }
                else
                {
                    if (e.NewValue == CheckState.Checked && item.EnumValue != 0)
                    {
                        SetZeroState(listBox, CheckState.Unchecked);
                    }

                    var value = GetNewValue(listBox, e);
                    for (var i = 0; i < listBox.Items.Count; i++)
                    {
                        var lbItem = (Item)listBox.Items[i];
                        if (lbItem.Values != null)
                        {
                            var check = true;
                            foreach (var v in lbItem.Values)
                            {
                                if ((v & value) == 0)
                                {
                                    check = false;
                                    break;
                                }
                            }
                            listBox.SetItemChecked(i, check);
                        }
                        else
                        {
                            listBox.SetItemChecked(i, ((lbItem.EnumValue & value) != 0));
                        }
                    }
                }
            }
            finally
            {
                _inEvent = false;
            }
        }

        private sealed class Item
        {
            private string? _displayName;
            public string? DisplayName { get => string.IsNullOrEmpty(_displayName) ? Name : _displayName; set => _displayName = value; }
            public string? Name;
            public string? EnumName;
            public ulong EnumValue;
            public bool IsZeroName;
            public List<string>? DisplayNames;
            public List<ulong>? Values;

            public override string ToString()
            {
                var name = new StringBuilder(DisplayName);
                if (DisplayNames != null)
                {
                    name.Append(": ");
                    for (var i = 0; i < DisplayNames.Count; i++)
                    {
                        if (i > 0)
                        {
                            name.Append(", ");
                        }
                        name.Append(DisplayNames[i]);
                    }
                }
                return name.ToString();
            }
        }

        private sealed class EnumEditorCheckedListBox : CheckedListBox
        {
            protected override CreateParams CreateParams
            {
                get
                {
                    var createParams = base.CreateParams;
                    createParams.Style &= ~EnumEditorListBox.WS_BORDER;
                    createParams.ExStyle &= ~EnumEditorListBox.WS_EX_CLIENTEDGE;
                    return createParams;
                }
            }
        }

        private sealed class EnumEditorListBox : ListBox
        {
            internal const int WS_EX_CLIENTEDGE = 0x00000200;
            internal const int WS_BORDER = 0x00800000;

            protected override CreateParams CreateParams
            {
                get
                {
                    var createParams = base.CreateParams;
                    createParams.Style &= ~WS_BORDER;
                    createParams.ExStyle &= ~WS_EX_CLIENTEDGE;
                    return createParams;
                }
            }
        }
    }
}
