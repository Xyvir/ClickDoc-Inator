
namespace Better_Steps_Recorder
{
    public partial class EditHyperlinkHeadingForm : Form
    {
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
           
        public EditHyperlinkHeadingForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            textBoxHyperlinkURL = new TextBox();
            textBoxHyperlinkText = new TextBox();
            textBoxSpoilerTitle = new TextBox();
            textBoxSpoilerText = new TextBox();
            buttonSave = new Button();
            buttonCancel = new Button();
            labelHyperlinkURL = new Label();
            labelHyperlinkText = new Label();
            labelSpoilerTitle = new Label();
            labelSpoilerText = new Label();
            SuspendLayout();
            // 
            // textBoxHyperlinkURL
            // 
            textBoxHyperlinkURL.Location = new Point(14, 40);
            textBoxHyperlinkURL.Margin = new Padding(3, 4, 3, 4);
            textBoxHyperlinkURL.Name = "textBoxHyperlinkURL";
            textBoxHyperlinkURL.Size = new Size(511, 27);
            textBoxHyperlinkURL.TabIndex = 0;
            // 
            // textBoxHyperlinkText
            // 
            textBoxHyperlinkText.Location = new Point(14, 99);
            textBoxHyperlinkText.Margin = new Padding(3, 4, 3, 4);
            textBoxHyperlinkText.Name = "textBoxHyperlinkText";
            textBoxHyperlinkText.Size = new Size(511, 27);
            textBoxHyperlinkText.TabIndex = 1;
            // 
            // textBoxSpoilerTitle
            // 
            textBoxSpoilerTitle.Location = new Point(14, 157);
            textBoxSpoilerTitle.Margin = new Padding(3, 4, 3, 4);
            textBoxSpoilerTitle.Name = "textBoxSpoilerTitle";
            textBoxSpoilerTitle.Size = new Size(511, 27);
            textBoxSpoilerTitle.TabIndex = 2;
            // 
            // textBoxSpoilerText
            // 
            textBoxSpoilerText.Location = new Point(14, 216);
            textBoxSpoilerText.Margin = new Padding(3, 4, 3, 4);
            textBoxSpoilerText.Multiline = true;
            textBoxSpoilerText.Name = "textBoxSpoilerText";
            textBoxSpoilerText.ScrollBars = ScrollBars.Vertical;
            textBoxSpoilerText.Size = new Size(511, 199);
            textBoxSpoilerText.TabIndex = 3;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(109, 424);
            buttonSave.Margin = new Padding(3, 4, 3, 4);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(86, 31);
            buttonSave.TabIndex = 4;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(339, 424);
            buttonCancel.Margin = new Padding(3, 4, 3, 4);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(86, 31);
            buttonCancel.TabIndex = 5;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // labelHyperlinkURL
            // 
            labelHyperlinkURL.AutoSize = true;
            labelHyperlinkURL.Location = new Point(14, 16);
            labelHyperlinkURL.Name = "labelHyperlinkURL";
            labelHyperlinkURL.Size = new Size(105, 20);
            labelHyperlinkURL.TabIndex = 6;
            labelHyperlinkURL.Text = "Hyperlink URL:";
            // 
            // labelHyperlinkText
            // 
            labelHyperlinkText.AutoSize = true;
            labelHyperlinkText.Location = new Point(14, 75);
            labelHyperlinkText.Name = "labelHyperlinkText";
            labelHyperlinkText.Size = new Size(106, 20);
            labelHyperlinkText.TabIndex = 7;
            labelHyperlinkText.Text = "Hyperlink Text:";
            // 
            // labelSpoilerTitle
            // 
            labelSpoilerTitle.AutoSize = true;
            labelSpoilerTitle.Location = new Point(14, 133);
            labelSpoilerTitle.Name = "labelSpoilerTitle";
            labelSpoilerTitle.Size = new Size(92, 20);
            labelSpoilerTitle.TabIndex = 8;
            labelSpoilerTitle.Text = "Spoiler Title:";
            // 
            // labelSpoilerText
            // 
            labelSpoilerText.AutoSize = true;
            labelSpoilerText.Location = new Point(14, 192);
            labelSpoilerText.Name = "labelSpoilerText";
            labelSpoilerText.Size = new Size(90, 20);
            labelSpoilerText.TabIndex = 9;
            labelSpoilerText.Text = "Spoiler Text:";
            // 
            // EditHyperlinkHeadingForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(539, 471);
            Controls.Add(labelSpoilerText);
            Controls.Add(labelSpoilerTitle);
            Controls.Add(labelHyperlinkText);
            Controls.Add(labelHyperlinkURL);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(textBoxSpoilerText);
            Controls.Add(textBoxSpoilerTitle);
            Controls.Add(textBoxHyperlinkText);
            Controls.Add(textBoxHyperlinkURL);
            Margin = new Padding(3, 4, 3, 4);
            Name = "EditHyperlinkHeadingForm";
            Text = "Edit Hyperlink Heading";
            Load += EditHyperlinkHeadingForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private void EditHyperlinkHeadingForm_Load(object sender, EventArgs e)
        {
            // Load existing data
            textBoxHyperlinkURL.Text = Program._linkHeading.HyperlinkURL?.ToString() ?? string.Empty;
            textBoxHyperlinkText.Text = Program._linkHeading.HyperlinkText;
            textBoxSpoilerTitle.Text = Program._linkHeading.SpoilerTitle;
            textBoxSpoilerText.Text = Program._linkHeading.SpoilerText;

            // Set default values if any of the text boxes are empty
            if (string.IsNullOrEmpty(textBoxHyperlinkURL.Text))
            {
                textBoxHyperlinkURL.Text = "https://www.youtube.com/watch?v=###########";
            }
            if (string.IsNullOrEmpty(textBoxHyperlinkText.Text))
            {
                textBoxHyperlinkText.Text = "Watch the Video";
            }
            if (string.IsNullOrEmpty(textBoxSpoilerTitle.Text))
            {
                textBoxSpoilerTitle.Text = "Video Transcript";
            }
            if (string.IsNullOrEmpty(textBoxSpoilerText.Text))
            {
                textBoxSpoilerText.Text = "Default Spoiler Text";
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Save the changes
            Program._linkHeading.HyperlinkURL = textBoxHyperlinkURL.Text;
            Program._linkHeading.HyperlinkText = textBoxHyperlinkText.Text;
            Program._linkHeading.SpoilerTitle = textBoxSpoilerTitle.Text;
            Program._linkHeading.SpoilerText = textBoxSpoilerText.Text;

            //Call savetozip here
            Program.zip?.SaveToZip();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Discard the changes
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
