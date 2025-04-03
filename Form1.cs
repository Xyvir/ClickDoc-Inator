using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text.Json;
using System.Windows.Automation;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;

namespace Better_Steps_Recorder
{
    public partial class Form1 : Form
    {
        public System.Windows.Forms.Timer activityTimer;
        private const int DefaultActivityDelay = 5000;
        private int ActivityDelay = DefaultActivityDelay;
        private Point _mouseDownLocation;

        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID_MOVE_UP = 1;
        private const int HOTKEY_ID_MOVE_DOWN = 2;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public Form1()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("Loaded");
            Listbox_Events.KeyDown += new KeyEventHandler(ListBox1_KeyDown);

            activityTimer = new System.Windows.Forms.Timer();
            activityTimer.Interval = ActivityDelay;
            activityTimer.Tick += activityTimer_Tick;

            // Add the Click event handler for pictureBox1
            pictureBox1.Click += pictureBox1_Click;

            // Set the Multiline property to false
            richTextBox_stepText.Multiline = true;

            // Handle the KeyPress event to prevent the Enter key from being processed
            richTextBox_stepText.KeyPress += richTextBox_stepText_KeyPress;

            // Handle the KeyDown event to prevent Shift+Enter and Ctrl+Enter
            richTextBox_stepText.KeyDown += richTextBox_stepText_KeyDown;

            // Handle the TextChanged event to remove any pasted newlines
            richTextBox_stepText.TextChanged += richTextBox_stepText_TextChanged;

            // Register hotkeys
            RegisterHotKey(this.Handle, HOTKEY_ID_MOVE_UP, 0x0001, (uint)Keys.Up); // ALT + Up
            RegisterHotKey(this.Handle, HOTKEY_ID_MOVE_DOWN, 0x0001, (uint)Keys.Down); // ALT + Down
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == HOTKEY_ID_MOVE_UP)
                {
                    MoveSelectedEventUp();
                }
                else if (id == HOTKEY_ID_MOVE_DOWN)
                {
                    MoveSelectedEventDown();
                }
            }
            base.WndProc(ref m);
        }

        private void MoveSelectedEventUp()
        {
            var selectedIndex = Listbox_Events.SelectedIndex;
            if (selectedIndex > 0)
            {
                Listbox_Events.ClearSelected();
                Listbox_Events.SetSelected(selectedIndex - 1, true);
                Listbox_Events_SelectedIndexChanged(Listbox_Events, EventArgs.Empty);
            }
            richTextBox_stepText.Focus();
        }

        private void MoveSelectedEventDown()
        {
            var selectedIndex = Listbox_Events.SelectedIndex;
            if (selectedIndex < Listbox_Events.Items.Count - 1)
            {
                Listbox_Events.ClearSelected();
                Listbox_Events.SetSelected(selectedIndex + 1, true);
                Listbox_Events_SelectedIndexChanged(Listbox_Events, EventArgs.Empty);
            }
            richTextBox_stepText.Focus();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID_MOVE_UP);
            UnregisterHotKey(this.Handle, HOTKEY_ID_MOVE_DOWN);
            base.OnFormClosing(e);
        }

        private void richTextBox_stepText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // Prevent the Enter key from being processed
            }
        }

        private void richTextBox_stepText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && (e.Modifiers == Keys.Shift || e.Modifiers == Keys.Control))
            {
                e.SuppressKeyPress = true; // Prevent Shift+Enter and Ctrl+Enter
            }
        }

        
        private void editHyperlinkHeadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new EditHyperlinkHeadingForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Changes were saved
                    //MessageBox.Show("Hyperlink heading updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left)
            {
                if (!Program.IsRecording)
                {
                    // Get the mouse click coordinates relative to the PictureBox
                    int x = me.X;
                    int y = me.Y;

                    if (pictureBox1.Image != null)
                    {
                        // Calculate the aspect ratios
                        float imageAspectRatio = (float)pictureBox1.Image.Width / pictureBox1.Image.Height;
                        float pictureBoxAspectRatio = (float)pictureBox1.ClientSize.Width / pictureBox1.ClientSize.Height;

                        // Calculate the actual displayed size of the image within the PictureBox
                        int displayedWidth, displayedHeight;
                        if (imageAspectRatio > pictureBoxAspectRatio)
                        {
                            // Image is wider than the PictureBox
                            displayedWidth = pictureBox1.ClientSize.Width;
                            displayedHeight = (int)(pictureBox1.ClientSize.Width / imageAspectRatio);
                        }
                        else
                        {
                            // Image is taller than the PictureBox
                            displayedWidth = (int)(pictureBox1.ClientSize.Height * imageAspectRatio);
                            displayedHeight = pictureBox1.ClientSize.Height;
                        }

                        // Calculate the offsets if the image is centered within the PictureBox
                        int offsetX = (pictureBox1.ClientSize.Width - displayedWidth) / 2;
                        int offsetY = (pictureBox1.ClientSize.Height - displayedHeight) / 2;

                        // Adjust the click coordinates based on the displayed size and offsets
                        int adjustedX = (int)((x - offsetX) * ((float)pictureBox1.Image.Width / displayedWidth));
                        int adjustedY = (int)((y - offsetY) * ((float)pictureBox1.Image.Height / displayedHeight));

                        if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
                        {
                            // Update the mouse coordinates of the selected event
                            selectedEvent.MouseCoordinates = new WindowHelper.POINT { X = adjustedX, Y = adjustedY };

                            // Draw crosshairs at the new mouse coordinates
                            DrawCrosshairs(pictureBox1, adjustedX, adjustedY);

                            // Refresh the PropertyGrid to show the updated coordinates
                            propertyGrid_RecordEvent.SelectedObject = null;
                            propertyGrid_RecordEvent.SelectedObject = selectedEvent;
                        }
                    }
                }
            }
            else if (me.Button == MouseButtons.Right)
            {
                if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
                {
                    // Change the Y coordinate to a negative value
                    selectedEvent.MouseCoordinates = new WindowHelper.POINT { X = selectedEvent.MouseCoordinates.X, Y = -selectedEvent.MouseCoordinates.Y };

                    // Draw crosshairs at the new mouse coordinates
                    DrawCrosshairs(pictureBox1, selectedEvent.MouseCoordinates.X, selectedEvent.MouseCoordinates.Y);

                    // Refresh the PropertyGrid to show the updated coordinates
                    propertyGrid_RecordEvent.SelectedObject = null;
                    propertyGrid_RecordEvent.SelectedObject = selectedEvent;
                }
            }
            else if (me.Button == MouseButtons.Middle)
            {
                if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
                {
                    // Set MouseCoordinates to OGMouseCoordinates
                    selectedEvent.MouseCoordinates = selectedEvent.OGMouseCoordinates;

                    // Draw crosshairs at the new mouse coordinates
                    DrawCrosshairs(pictureBox1, selectedEvent.MouseCoordinates.X, selectedEvent.MouseCoordinates.Y);

                    // Refresh the PropertyGrid to show the updated coordinates
                    propertyGrid_RecordEvent.SelectedObject = null;
                    propertyGrid_RecordEvent.SelectedObject = selectedEvent;
                }
            }
        }

        private void DrawCrosshairs(PictureBox pictureBox, int x, int y1)
        {
            if (pictureBox.Image == null || Listbox_Events.SelectedItem is not RecordEvent selectedEvent)
                return;

            // Reload the step's image from the BSR file
            if (!string.IsNullOrEmpty(selectedEvent.Screenshotb64))
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(selectedEvent.Screenshotb64);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        pictureBox.Image = new Bitmap(ms);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to load image from Base64 string: {ex.Message}");
                    pictureBox.Image = null;
                    return;
                }
            }

            int y = Math.Abs(y1);

            // Create a copy of the image to draw on
            Bitmap bitmap = new Bitmap(pictureBox.Image);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Define the crosshair properties
                Pen pen = new Pen(y1 < 0 ? Color.Yellow : Color.HotPink, 2);
                int crosshairSize = 40;

                // Draw horizontal line
                g.DrawLine(pen, x - crosshairSize, y, x + crosshairSize, y);

                // Draw vertical line
                g.DrawLine(pen, x, y - crosshairSize, x, y + crosshairSize);
            }

            // Set the modified image back to the PictureBox
            pictureBox.Image = bitmap;
        }
        private void ListBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the Delete key was pressed
            if (e.KeyCode == Keys.Delete)
            {
                deleteToolStripMenuItem_Click(sender, e);
            }
        }
        public void AddRecordEventToListBox(RecordEvent recordEvent)
        {
            Listbox_Events.Items.Add(recordEvent);
            EnableDisable_exportToolStripMenuItem();
        }
        public void ClearListBox()
        {
            Listbox_Events.Items.Clear();  
            propertyGrid_RecordEvent.SelectedObject = null;
            pictureBox1.Image = null;
            richTextBox_stepText.Text = null;

        }

        // This is the logic for whenever the 'active' event is changed.
        private void Listbox_Events_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
            {
                propertyGrid_RecordEvent.SelectedObject = selectedEvent;

                if (selectedEvent.HasPng())
                {
                    try
                    {
                        // Convert the Base64 string back to a byte array
                        byte[] imageBytes = Convert.FromBase64String(selectedEvent.Screenshotb64);

                        // Create a MemoryStream from the byte array
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            // Create a Bitmap from the MemoryStream and set it to the PictureBox
                            pictureBox1.Image = new Bitmap(ms);
                        }

                        // Draw crosshairs at the mouse coordinates
                        DrawCrosshairs(pictureBox1, selectedEvent.MouseCoordinates.X, selectedEvent.MouseCoordinates.Y);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to load image from Base64 string: {ex.Message}");
                        pictureBox1.Image = null; // Clear the image if there was an error
                    }
                }
                else
                {
                    // Get the directory of the current .bsr file
                    string bsrDirectory = Path.GetDirectoryName(Program.zip?.zipFilePath);

                    // Combine the directory with the relative path
                    string fullPath = Path.Combine(bsrDirectory, selectedEvent.Screenshotb64);

                    try
                    {
                        // Attempt to load the image from the full path
                        pictureBox1.Image = Image.FromFile(fullPath);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to load image from relative path: {ex.Message}");

                        // Display a message in the PictureBox if the image cannot be loaded
                        Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.Clear(Color.White);
                            g.DrawString($"No Image could be loaded from \n \"{fullPath}\"",
                                         new Font("Arial", 12), Brushes.Black, new PointF(10, 10));
                        }
                        pictureBox1.Image = bitmap;
                    }
                }

                // Set the step text
                richTextBox_stepText.Text = selectedEvent._StepText;
            }
            else
            {
                // if not a record event
            }
        }


        private void ToolStripMenuItem_Recording_Click(object sender, EventArgs e)
        {
            if (Program.IsRecording)
            {
                Program.UnHookMouseOperations();
                ToolStripMenuItem_Recording.Text = "Start Recording";
                ToolStripMenuItem_Recording.BackColor = SystemColors.Control;
                ToolStripMenuItem_Recording.Image = Properties.Resources.RecordTiny;
                ActivityDelay = DefaultActivityDelay;
                activityTimer_Tick(sender, e);

                EnableDisable_exportToolStripMenuItem();
            }
            else
            {
                Program.HookMouseOperations();
                ToolStripMenuItem_Recording.Text = "Pause Recording";
                ToolStripMenuItem_Recording.BackColor = Color.IndianRed;
                ToolStripMenuItem_Recording.Image = Properties.Resources.RecordPauseTiny;
                ActivityDelay = 15000;


                ClearListBox();
                UpdateListItems();

            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.zip != null)
            {
                Program.zip?.SaveToZip();
            }
            string zipFilePath = FileDialogHelper.ShowSaveFileDialog();
            if (zipFilePath != null && zipFilePath != "")
            {
                // Extract the directory and filename and remove spaces
                string directory = Path.GetDirectoryName(zipFilePath);
                string filename = Path.GetFileName(zipFilePath).Replace(" ", "_");
                string newFilePath = Path.Combine(directory, filename);

                // Output debug information
                Debug.WriteLine($"Directory: {directory}");
                Debug.WriteLine($"Filename: {filename}");
                Debug.WriteLine($"NewFilePath: {newFilePath}");

                EnableRecording();
                Program.zip = new ZipFileHandler(newFilePath);
                Program._recordEvents = new List<RecordEvent>();
                Listbox_Events.Items.Clear();
                Program.EventCounter = 1;
                EnableDisable_exportToolStripMenuItem();
                propertyGrid_RecordEvent.SelectedObject = null;
                pictureBox1.Image = null;
                richTextBox_stepText.Text = null;

                // Clear the additional attributes
                Program._linkHeading.HyperlinkURL = new Uri("https://www.youtube.com/watch?v=###########");
                Program._linkHeading.HyperlinkText = "Watch the Video";
                Program._linkHeading.SpoilerTitle = "Video Transcript";
                Program._linkHeading.SpoilerText = "Default Spoiler Text";
                // Note these are originally set under the object definition in linkheading.cs

                // Enable the edit hyperlink heading menu item
                editHyperlinkHeadingToolStripMenuItem.Enabled = true;
                toolStripMenuItem1_SaveAs.Enabled = true;

                // Update the title bar text
                UpdateTitleBar(newFilePath);
            }
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.zip != null)
            {
                Program.zip?.SaveToZip();
            }
            string zipFilePath = FileDialogHelper.ShowOpenFileDialog();
            if (zipFilePath != null && zipFilePath != "")
            {
                propertyGrid_RecordEvent.SelectedObject = null;
                pictureBox1.Image = null;
                richTextBox_stepText.Text = null;
                EnableRecording();
                Program.zip = new ZipFileHandler(zipFilePath);
                Program.LoadRecordEventsFromFile(zipFilePath);
                EnableDisable_exportToolStripMenuItem();

                // Update the title bar text
                UpdateTitleBar(zipFilePath);

                // Enable the edit hyperlink heading menu item
                editHyperlinkHeadingToolStripMenuItem.Enabled = true;
                toolStripMenuItem1_SaveAs.Enabled = true;

                // Check if the filename contains "template" (case insensitive)
                if (Path.GetFileName(zipFilePath).IndexOf("template", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Show the "Save As" dialog
                    string newZipFilePath = FileDialogHelper.ShowSaveFileDialog();
                    if (newZipFilePath != null && newZipFilePath != "")
                    {
                        // Save the loaded file to the new path
                        Program.zip.SaveToZip();

                        // Update the title bar text with the new file path
                        UpdateTitleBar(newZipFilePath);
                    }
                }
            }
        }

        private void richTextBox_stepText_TextChanged(object sender, EventArgs e)
        {


            if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
            {
                var recordEvent = Program._recordEvents.Find(ev => ev.ID == selectedEvent.ID);
                if (recordEvent != null)
                {
                    if (recordEvent._StepText != richTextBox_stepText.Text)
                    {
                        recordEvent._StepText = richTextBox_stepText.Text;
                        activityTimer.Stop();
                        activityTimer.Start();
                    }

                    string text = richTextBox_stepText.Text;
                    if (text.Contains("\n") || text.Contains("\r"))
                    {
                        text = text.Replace("\r", "").Replace("\n", "");
                        richTextBox_stepText.Text = text;
                        richTextBox_stepText.SelectionStart = text.Length; // Move the cursor to the end
                    }

                }
                else
                {
                    // Handle the case where the event is not found, if necessary
                    // This might be logging an error, notifying the user, etc.
                }
                // UpdateListItems();
            }


        }
        private void activityTimer_Tick(object sender, EventArgs e)
        {
            Program.zip?.SaveToZip();
            activityTimer.Stop();
        }


        private void EnableRecording()
        {
            ToolStripMenuItem_Recording.Enabled = true;
            ToolStripMenuItem_Recording.Visible = true;
        }
        private void DisableRecording()
        {
            ToolStripMenuItem_Recording.Enabled = false;
            ToolStripMenuItem_Recording.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DisableRecording();
        }

        private void EnableDisable_exportToolStripMenuItem()
        {
            if (Listbox_Events.Items.Count > 0)
            {
                exportToolStripMenuItem.Enabled = true;
                exportToolMDStripMenuItem.Enabled = true;



            }
            else
            {
                Listbox_Events.Items.Add("<No events saved to currently open *.BSR file>");
                exportToolStripMenuItem.Enabled = false;
                exportToolMDStripMenuItem.Enabled = false;
                
                
            }

        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.zip?.SaveToZip();
            // Set up the save file dialog to specify the output path for the Rtf document
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Rich Text Format|*.rtf";
                saveFileDialog.Title = "Export to RTF Document";
                if (Program.zip?.zipFilePath != null)
                {
                    saveFileDialog.FileName = Path.GetFileNameWithoutExtension(Program.zip.zipFilePath) + ".rtf";
                }
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string docPath = saveFileDialog.FileName;
                    Program.ExportDoc(docPath, "RTF");
                }
            }
        }

        private void exportToolMDStripMenuItem_Click(object sender, EventArgs e)
        {
            // Use FolderBrowserDialog to select a folder
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select the folder to save the Markdown document";
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderBrowserDialog.SelectedPath;
                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        // Generate the Markdown file path
                        string fileName = "ExportedDocument.md";
                        if (Program.zip?.zipFilePath != null)
                        {
                            fileName = Path.GetFileNameWithoutExtension(Program.zip.zipFilePath) + ".md";
                        }
                        string docPath = Path.Combine(folderPath, fileName);

                        // Export to Markdown
                        Program.ExportDoc(docPath, "MD");
                    }
                }
            }
        }


        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Listbox_Events.SelectedItems.Count > 0)
            {
                // Check if the Shift key is held down
                if (!Control.ModifierKeys.HasFlag(Keys.Shift))
                {
                    // Show a confirmation dialog
                    var result = MessageBox.Show("Are you sure you want to delete the selected events?\n(This cannot be Undo'd.)", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result != DialogResult.Yes)
                    {
                        // If the user chooses not to delete, exit the method
                        return;
                    }
                }

                // Create a list to store the selected events to remove them safely
                List<RecordEvent> selectedEvents = new List<RecordEvent>();

                // Collect all selected events
                foreach (var item in Listbox_Events.SelectedItems)
                {
                    if (item is RecordEvent selectedEvent)
                    {
                        selectedEvents.Add(selectedEvent);
                    }
                }

                // Store the step number of the first selected event
                int stepNumberToSelect = selectedEvents.First().Step;

                // Remove each selected event
                foreach (var selectedEvent in selectedEvents)
                {
                    Listbox_Events.Items.Remove(selectedEvent);

                    var recordEvent = Program._recordEvents.Find(e => e.ID == selectedEvent.ID);
                    if (recordEvent != null)
                    {
                        Program._recordEvents.Remove(recordEvent);
                    }
                    else
                    {
                        // Handle the case where the event is not found, if necessary
                        MessageBox.Show("One or more selected events were not found in the record events list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // Update the display of list items after removal
                UpdateListItems();
                EnableDisable_exportToolStripMenuItem();

                // Re-select the event with the same step number
                var eventToSelect = Program._recordEvents.FirstOrDefault(e => e.Step == stepNumberToSelect);
                if (eventToSelect != null)
                {
                    Listbox_Events.SelectedItem = eventToSelect;
                }
            }
        }





        private void Listbox_Events_MouseDown(object sender, MouseEventArgs e)
        {
            // Store the mouse down location for potential dragging
            _mouseDownLocation = e.Location;

            if (e.Button == MouseButtons.Right)
            {
                // Get the index of the item under the mouse cursor
                int index = Listbox_Events.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    // Select the item
                    Listbox_Events.SelectedIndex = index;

                    // Show the context menu at the mouse position
                    contextMenu_ListBox_Events.Show(Listbox_Events, e.Location);
                }
            }
            
        }

        private void toolStripMenuItem1_SaveAs_Click(object sender, EventArgs e)
        {
            Program.zip?.SaveToZip();
            string zipFilePath = FileDialogHelper.ShowSaveFileDialog();
            if (zipFilePath != null && zipFilePath != "")
            {
               
                // Extract the directory and filename and replaces spaces
                string directory = Path.GetDirectoryName(zipFilePath);
                string filename = Path.GetFileName(zipFilePath).Replace(" ", "_");
                string newFilePath = Path.Combine(directory, filename);

                // Output debug information
                Debug.WriteLine($"Directory: {directory}");
                Debug.WriteLine($"Filename: {filename}");
                Debug.WriteLine($"NewFilePath: {newFilePath}");

                Program.zip = new ZipFileHandler(newFilePath);
                Program.zip.SaveToZip();
                UpdateTitleBar(newFilePath);
            }
        }

        private void newExternalImageStepStripMenuItem_Click(object sender, EventArgs e)
        {
            string prompt = "Please enter relative path, for example \"foo/bar.png\" without quotes";
            string title = "Enter Relative Path";
            string defaultValue = "shared/";

            // Show the InputBox and get the user input
            string relativePath = Interaction.InputBox(prompt, title, defaultValue);

            if (!string.IsNullOrEmpty(relativePath))
            {
                // Create a new RecordEvent with the specified properties
                var newEvent = new RecordEvent
                {
                    Step = Program._recordEvents.Count + 1,
                    Screenshotb64 = relativePath,
                    ID = Guid.NewGuid(),
                    CreationTime = DateTime.Now,
                    WindowTitle = " ",
                    ApplicationName = " ",
                    WindowCoordinates = new WindowHelper.RECT(),
                    WindowSize = new WindowHelper.Size(),
                    UICoordinates = new WindowHelper.RECT(),
                    UISize = new WindowHelper.Size(),
                    MouseCoordinates = new WindowHelper.POINT(),
                    OGMouseCoordinates = new WindowHelper.POINT(),
                    TooltipText = " ",
                    EventType = " ",
                    _StepText = " ",
                    ElementName = " ",
                    ElementType = " "
                };

                // Add the new event to the list and update the UI
                Program._recordEvents.Add(newEvent);
                AddRecordEventToListBox(newEvent);
                EnableDisable_exportToolStripMenuItem();
            }
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the selected indices and sort them
            var selectedIndices = Listbox_Events.SelectedIndices.Cast<int>().ToList();
            selectedIndices.Sort();

            // Move selected items up
            foreach (int index in selectedIndices)
            {
                if (index > 0) // Ensure there's an item above to swap with
                {
                    // Swap the selected item with the one above it
                    var temp = Program._recordEvents[index];
                    Program._recordEvents[index] = Program._recordEvents[index - 1];
                    Program._recordEvents[index - 1] = temp;

                    // Update the ListBox
                    Listbox_Events.Items[index] = Program._recordEvents[index];
                    Listbox_Events.Items[index - 1] = Program._recordEvents[index - 1];
                }
            }

            // Update the selected indices
            Listbox_Events.ClearSelected();
            foreach (var index in selectedIndices)
            {
                Listbox_Events.SetSelected(index - 1, true);
            }

            // Update the display
            UpdateListItems();
        }


        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the selected indices and sort them in descending order
            var selectedIndices = Listbox_Events.SelectedIndices.Cast<int>().ToList();
            selectedIndices.Sort((x, y) => y.CompareTo(x));

            // Move selected items down
            foreach (int index in selectedIndices)
            {
                if (index < Program._recordEvents.Count - 1) // Ensure there's an item below to swap with
                {
                    // Swap the selected item with the one below it
                    var temp = Program._recordEvents[index];
                    Program._recordEvents[index] = Program._recordEvents[index + 1];
                    Program._recordEvents[index + 1] = temp;

                    // Update the ListBox
                    Listbox_Events.Items[index] = Program._recordEvents[index];
                    Listbox_Events.Items[index + 1] = Program._recordEvents[index + 1];
                }
            }

            // Update the selected indices
            Listbox_Events.ClearSelected();
            foreach (var index in selectedIndices)
            {
                Listbox_Events.SetSelected(index + 1, true);
            }

            // Update the display
            UpdateListItems();
        }

        private void UpdateListItems()
        {
            ClearListBox();
            // Update the Step property based on the new order in the list
            for (int i = 0; i < Program._recordEvents.Count; i++)
            {
                Program._recordEvents[i].Step = i + 1;
                AddRecordEventToListBox(Program._recordEvents[i]);
                //Debug.WriteLine(Program._recordEvents[i].ToString());

            }

            // Optionally, update the display to reflect new step numbers if shown
            Listbox_Events.Refresh();
            activityTimer.Stop();
            activityTimer.Start();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpPopup helpPopup = new HelpPopup();
            helpPopup.Show();
        }

        private void Listbox_Events_DragOver(object sender, DragEventArgs e)
        {
            // Get the mouse position relative to the ListBox
            Point point = Listbox_Events.PointToClient(new Point(e.X, e.Y));

            // Determine the height of the ListBox and the threshold for scrolling
            int scrollRegionHeight = 20; // Adjust this value as needed for sensitivity
            int scrollSpeed = 1; // Number of items to scroll at a time

            // Scroll up if the mouse is near the top of the ListBox
            if (point.Y < scrollRegionHeight)
            {
                int newTopIndex = Math.Max(Listbox_Events.TopIndex - scrollSpeed, 0);
                Listbox_Events.TopIndex = newTopIndex;
            }
            // Scroll down if the mouse is near the bottom of the ListBox
            else if (point.Y > Listbox_Events.Height - scrollRegionHeight)
            {
                int newTopIndex = Math.Min(Listbox_Events.TopIndex + scrollSpeed, Listbox_Events.Items.Count - 1);
                Listbox_Events.TopIndex = newTopIndex;
            }

            // Set the drag effect to indicate a move operation
            e.Effect = DragDropEffects.Move;
        }

        private void Listbox_Events_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Listbox_Events_DragDrop(object sender, DragEventArgs e)
        {
            // Get the index of the item where the drop occurred
            Point point = Listbox_Events.PointToClient(new Point(e.X, e.Y));
            int targetIndex = Listbox_Events.IndexFromPoint(point);

            if (targetIndex < 0) targetIndex = Listbox_Events.Items.Count - 1;

            // Get the dragged item from the data
            var draggedEvent = e.Data.GetData(typeof(RecordEvent)) as RecordEvent;

            if (draggedEvent != null)
            {
                // Find the original index of the dragged item
                int originalIndex = Listbox_Events.Items.IndexOf(draggedEvent);

                // Check if the item was dragged to a new position
                if (originalIndex != targetIndex)
                {
                    // Remove the item from its original position in the data source
                    Program._recordEvents.RemoveAt(originalIndex);

                    // Insert the item into the new position in the data source
                    Program._recordEvents.Insert(targetIndex, draggedEvent);

                    // Remove the item from the ListBox
                    Listbox_Events.Items.RemoveAt(originalIndex);

                    // Insert the item into the new position in the ListBox
                    Listbox_Events.Items.Insert(targetIndex, draggedEvent);

                    // Set the new selected index
                    Listbox_Events.SelectedIndex = targetIndex;

                    // Update the step numbers and other necessary UI elements
                    UpdateListItems();
                }
            }
        }

        private void richTextBox_stepText_Leave(object sender, EventArgs e)
        {
            UpdateListItems();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.zip?.SaveToZip();
        }

        private void Listbox_Events_MouseMove(object sender, MouseEventArgs e)
        {
            // Start the drag-and-drop operation if the left button is held and the mouse moved
            if (e.Button == MouseButtons.Left)
            {
                int index = Listbox_Events.IndexFromPoint(_mouseDownLocation);
                if (index != ListBox.NoMatches)
                {
                    // Determine if the drag threshold has been met
                    if (Math.Abs(e.X - _mouseDownLocation.X) > SystemInformation.DragSize.Width ||
                        Math.Abs(e.Y - _mouseDownLocation.Y) > SystemInformation.DragSize.Height)
                    {
                        // Start the drag-and-drop operation
                        Listbox_Events.DoDragDrop(Listbox_Events.Items[index], DragDropEffects.Move);
                    }
                }
            }
        }
        private void UpdateTitleBar(string filePath)
        {
            string parentFolder = Path.GetFileName(Path.GetDirectoryName(filePath));
            string fileName = Path.GetFileName(filePath);
            this.Text = $"ClickDoc-Inator - ...\\{parentFolder}\\{fileName}";
        }
    }
}
