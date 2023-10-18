using System.Windows.Forms;

namespace RawInputReader
{
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
            Input = new RawInput();
            Input.PropertyChanged += (s, e) => propertyGridMain.Refresh();
            propertyGridMain.PropertyValueChanged += OnPropertyValueChanged;
            propertyGridMain.SelectedObject = Input;
            buttonRun.Enabled = false;
        }

        private void OnPropertyValueChanged(object? s, PropertyValueChangedEventArgs e)
        {
            buttonRun.Enabled = Input.Page != HID_USAGE_PAGE.HID_USAGE_PAGE_UNDEFINED && Input.Usage != 0;
        }

        public RawInput Input { get; }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
    }
}
