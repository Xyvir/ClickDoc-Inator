using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text.Json;
using System.Windows.Automation;
using System.Windows.Forms;

namespace Better_Steps_Recorder
{
    public partial class Form1 : Form
    {
        public System.Windows.Forms.Timer activityTimer;
        private const int DefaultActivityDelay = 5000;
        private int ActivityDelay = DefaultActivityDelay;
        private Point _mouseDownLocation;
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


        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!Program.IsRecording)
            {
                // Get the mouse click coordinates relative to the PictureBox
                MouseEventArgs me = (MouseEventArgs)e;
                int x = me.X;
                int y = me.Y;

                if (pictureBox1.Image != null)
                {
                    // Calculate the scaling factors
                    float scaleX = (float)pictureBox1.Image.Width / pictureBox1.ClientSize.Width;
                    float scaleY = (float)pictureBox1.Image.Height / pictureBox1.ClientSize.Height;

                    // Adjust the click coordinates based on the scaling factors
                    int adjustedX = (int)(x * scaleX);
                    int adjustedY = (int)(y * scaleY);

                    if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
                    {
                        // Update the mouse coordinates of the selected event
                        selectedEvent.MouseCoordinates = new WindowHelper.POINT { X = adjustedX, Y = adjustedY };

                        // Reload the step's image from the BSR file
                        if (!string.IsNullOrEmpty(selectedEvent.Screenshotb64))
                        {
                            try
                            {
                                byte[] imageBytes = Convert.FromBase64String(selectedEvent.Screenshotb64);
                                using (MemoryStream ms = new MemoryStream(imageBytes))
                                {
                                    pictureBox1.Image = new Bitmap(ms);
                                }

                                // Draw crosshairs at the new mouse coordinates
                                DrawCrosshairs(pictureBox1, adjustedX, adjustedY);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Failed to load image from Base64 string: {ex.Message}");
                                pictureBox1.Image = null;
                            }
                        }

                        // Refresh the PropertyGrid to show the updated coordinates
                        propertyGrid_RecordEvent.SelectedObject = null;
                        propertyGrid_RecordEvent.SelectedObject = selectedEvent;
                    }
                }
            }
        }

        private void DrawCrosshairs(PictureBox pictureBox, int x, int y)
        {
            if (pictureBox.Image == null)
                return;

            // Create a copy of the image to draw on
            Bitmap bitmap = new Bitmap(pictureBox.Image);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Define the crosshair properties
                Pen pen = new Pen(Color.HotPink, 2);
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
            EnableDisable_exportToolStripMenuItem();
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

                // Check if Screenshotb64 is not null or empty
                if (!string.IsNullOrEmpty(selectedEvent.Screenshotb64))
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
                    pictureBox1.Image = null; // Clear the image if there's no Base64 string
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
            }
            else
            {
                Program.HookMouseOperations();
                ToolStripMenuItem_Recording.Text = "Pause Recording";
                ToolStripMenuItem_Recording.BackColor = Color.IndianRed;
                ToolStripMenuItem_Recording.Image = Properties.Resources.RecordPauseTiny;
                ActivityDelay = 15000;
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
                EnableRecording();
                Program.zip = new ZipFileHandler(zipFilePath);
                Program._recordEvents = new List<RecordEvent>();
                Listbox_Events.Items.Clear();
                Program.EventCounter = 1;
                EnableDisable_exportToolStripMenuItem();
                propertyGrid_RecordEvent.SelectedObject = null;
                pictureBox1.Image = null;
                richTextBox_stepText.Text = null;
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
                toolStripMenuItem1_SaveAs.Enabled = true;
            }
            else
            {
                exportToolStripMenuItem.Enabled = false;
                toolStripMenuItem1_SaveAs.Enabled = true;
            }

        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.zip?.SaveToZip();
            // Set up the save file dialog to specify the output path for the Word document
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
                    Program.ExportToRTF(docPath);
                }
            }
        }

        private void exportToolMDStripMenuItem_Click(object sender, EventArgs e)
        {
            //Program.zip?.SaveToZip();
          
            //using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            //{
            //    saveFileDialog.Filter = "Rich Text Format|*.rtf";
            //    saveFileDialog.Title = "Export to RTF Document";
            //    if (Program.zip?.zipFilePath != null)
            //    {
            //        saveFileDialog.FileName = Path.GetFileNameWithoutExtension(Program.zip.zipFilePath) + ".rtf";
            //    }
            //    if (saveFileDialog.ShowDialog() == DialogResult.OK)
            //    {
            //        string docPath = saveFileDialog.FileName;
            //        Program.ExportToRTF(docPath);
            //    }
            //}
        }


        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Listbox_Events.SelectedItems.Count > 0)
            {
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
            FileDialogHelper.SaveAs();
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
    }
}
