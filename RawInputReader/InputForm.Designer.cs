namespace RawInputReader
{
    partial class InputForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            propertyGridMain = new System.Windows.Forms.PropertyGrid();
            panelButtons = new System.Windows.Forms.Panel();
            buttonRun = new System.Windows.Forms.Button();
            tableLayoutPanelMain.SuspendLayout();
            panelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.ColumnCount = 1;
            tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanelMain.Controls.Add(propertyGridMain, 0, 0);
            tableLayoutPanelMain.Controls.Add(panelButtons, 0, 1);
            tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.RowCount = 2;
            tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanelMain.Size = new System.Drawing.Size(457, 212);
            tableLayoutPanelMain.TabIndex = 0;
            // 
            // propertyGridMain
            // 
            propertyGridMain.Dock = System.Windows.Forms.DockStyle.Fill;
            propertyGridMain.HelpVisible = false;
            propertyGridMain.Location = new System.Drawing.Point(3, 3);
            propertyGridMain.Name = "propertyGridMain";
            propertyGridMain.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            propertyGridMain.Size = new System.Drawing.Size(451, 176);
            propertyGridMain.TabIndex = 2;
            propertyGridMain.ToolbarVisible = false;
            // 
            // panelButtons
            // 
            panelButtons.Controls.Add(buttonRun);
            panelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            panelButtons.Location = new System.Drawing.Point(0, 182);
            panelButtons.Margin = new System.Windows.Forms.Padding(0);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new System.Drawing.Size(457, 30);
            panelButtons.TabIndex = 1;
            // 
            // buttonRun
            // 
            buttonRun.Anchor = System.Windows.Forms.AnchorStyles.Right;
            buttonRun.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonRun.Location = new System.Drawing.Point(379, 3);
            buttonRun.Name = "buttonRun";
            buttonRun.Size = new System.Drawing.Size(75, 23);
            buttonRun.TabIndex = 0;
            buttonRun.Text = "Run";
            buttonRun.UseVisualStyleBackColor = true;
            // 
            // InputForm
            // 
            AcceptButton = buttonRun;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(457, 212);
            Controls.Add(tableLayoutPanelMain);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Read Raw Input";
            tableLayoutPanelMain.ResumeLayout(false);
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.PropertyGrid propertyGridMain;
    }
}