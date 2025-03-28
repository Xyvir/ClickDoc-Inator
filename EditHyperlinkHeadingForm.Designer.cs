namespace Better_Steps_Recorder
{
    partial class EditHyperlinkHeading : Form
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox textBoxHyperlinkURL;
        private TextBox textBoxHyperlinkText;
        private TextBox textBoxSpoilerTitle;
        private TextBox textBoxSpoilerText;
        private Button buttonSave;
        private Button buttonCancel;

        private Label labelHyperlinkURL;
        private Label labelHyperlinkText;
        private Label labelSpoilerTitle;
        private Label labelSpoilerText;

        // protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        private void InitializeComponent()
        {
            this.textBoxHyperlinkURL = new TextBox();
            this.textBoxHyperlinkText = new TextBox();
            this.textBoxSpoilerTitle = new TextBox();
            this.textBoxSpoilerText = new TextBox();
            this.buttonSave = new Button();
            this.buttonCancel = new Button();
            this.labelHyperlinkURL = new Label();
            this.labelHyperlinkText = new Label();
            this.labelSpoilerTitle = new Label();
            this.labelSpoilerText = new Label();
            this.SuspendLayout();
            // 
            // labelHyperlinkURL
            // 
            this.labelHyperlinkURL.AutoSize = true;
            this.labelHyperlinkURL.Location = new Point(12, 12);
            this.labelHyperlinkURL.Name = "labelHyperlinkURL";
            this.labelHyperlinkURL.Size = new Size(85, 15);
            this.labelHyperlinkURL.TabIndex = 6;
            this.labelHyperlinkURL.Text = "Hyperlink URL:";
            // 
            // textBoxHyperlinkURL
            // 
            this.textBoxHyperlinkURL.Location = new Point(12, 30);
            this.textBoxHyperlinkURL.Name = "textBoxHyperlinkURL";
            this.textBoxHyperlinkURL.Size = new Size(360, 23);
            this.textBoxHyperlinkURL.TabIndex = 0;
            // 
            // labelHyperlinkText
            // 
            this.labelHyperlinkText.AutoSize = true;
            this.labelHyperlinkText.Location = new Point(12, 56);
            this.labelHyperlinkText.Name = "labelHyperlinkText";
            this.labelHyperlinkText.Size = new Size(82, 15);
            this.labelHyperlinkText.TabIndex = 7;
            this.labelHyperlinkText.Text = "Hyperlink Text:";
            // 
            // textBoxHyperlinkText
            // 
            this.textBoxHyperlinkText.Location = new Point(12, 74);
            this.textBoxHyperlinkText.Name = "textBoxHyperlinkText";
            this.textBoxHyperlinkText.Size = new Size(360, 23);
            this.textBoxHyperlinkText.TabIndex = 1;
            // 
            // labelSpoilerTitle
            // 
            this.labelSpoilerTitle.AutoSize = true;
            this.labelSpoilerTitle.Location = new Point(12, 100);
            this.labelSpoilerTitle.Name = "labelSpoilerTitle";
            this.labelSpoilerTitle.Size = new Size(70, 15);
            this.labelSpoilerTitle.TabIndex = 8;
            this.labelSpoilerTitle.Text = "Spoiler Title:";
            // 
            // textBoxSpoilerTitle
            // 
            this.textBoxSpoilerTitle.Location = new Point(12, 118);
            this.textBoxSpoilerTitle.Name = "textBoxSpoilerTitle";
            this.textBoxSpoilerTitle.Size = new Size(360, 23);
            this.textBoxSpoilerTitle.TabIndex = 2;
            // 
            // labelSpoilerText
            // 
            this.labelSpoilerText.AutoSize = true;
            this.labelSpoilerText.Location = new Point(12, 144);
            this.labelSpoilerText.Name = "labelSpoilerText";
            this.labelSpoilerText.Size = new Size(70, 15);
            this.labelSpoilerText.TabIndex = 9;
            this.labelSpoilerText.Text = "Spoiler Text:";
            // 
            // textBoxSpoilerText
            // 
            this.textBoxSpoilerText.Location = new Point(12, 162);
            this.textBoxSpoilerText.Multiline = true;
            this.textBoxSpoilerText.Name = "textBoxSpoilerText";
            this.textBoxSpoilerText.Size = new Size(360, 150);
            this.textBoxSpoilerText.TabIndex = 3;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new Point(216, 318);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new Size(75, 23);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new Point(297, 318);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new EventHandler(this.buttonCancel_Click);
            // 
            // EditHyperlinkHeading
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(384, 353);
            this.Controls.Add(this.labelSpoilerText);
            this.Controls.Add(this.labelSpoilerTitle);
            this.Controls.Add(this.labelHyperlinkText);
            this.Controls.Add(this.labelHyperlinkURL);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxSpoilerText);
            this.Controls.Add(this.textBoxSpoilerTitle);
            this.Controls.Add(this.textBoxHyperlinkText);
            this.Controls.Add(this.textBoxHyperlinkURL);
            this.Name = "EditHyperlinkHeading";
            this.Text = "Edit Hyperlink Heading";
            this.Load += new EventHandler(this.EditHyperlinkHeading_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Add your save logic here
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Add your cancel logic here
        }

        private void EditHyperlinkHeading_Load(object sender, EventArgs e)
        {
            // Add your load logic here
        }
    }
}