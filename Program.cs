
using System.Diagnostics;

using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

using System.Windows.Automation;

using System.Text.Json;
using System.IO.Compression;
//using Xceed.Document.NET;
//using Xceed.Words.NET;


namespace Better_Steps_Recorder
{
    internal static class Program
    {
        public static ZipFileHandler? zip;
        private static IntPtr _hookID = IntPtr.Zero;
        private static LowLevelMouseProc _proc = HookCallback;
        public static List<RecordEvent> _recordEvents = new List<RecordEvent>();

        private static Form1? _form1Instance;
        public static int EventCounter = 1;
        public static bool IsRecording = false;

        public static LinkHeading _linkHeading = new LinkHeading();



        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            _form1Instance = new Form1();

            System.Windows.Forms.Application.Run(_form1Instance);


    
        }

        public static void HookMouseOperations()
        {
            _hookID = SetHook(_proc);
            IsRecording = true;
        }
        public static void UnHookMouseOperations()
        {
            WindowHelper.UnhookWindowsHookEx(_hookID);
            IsRecording = false;
        }

        public static void LoadRecordEventsFromFile(string filePath)
        {
            Debug.WriteLine($"LoadRecordEventsFromFile called with filePath: {filePath}");

            if (File.Exists(filePath))
            {
                try
                {
                    using (ZipArchive archive = ZipFile.OpenRead(filePath))
                    {
                        Debug.WriteLine("Zip file opened successfully.");

                        _recordEvents = new List<RecordEvent>();
                        _form1Instance?.Invoke((Action)(() => _form1Instance.ClearListBox()));

                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            Debug.WriteLine($"Processing entry: {entry.FullName}");

                            if (entry.FullName == "additional_attributes.json")
                            {
                                // Load additional attributes if the entry exists
                                try
                                {
                                    using (StreamReader reader = new StreamReader(entry.Open()))
                                    {
                                        string jsonContent = reader.ReadToEnd();
                                        var additionalAttributes = JsonSerializer.Deserialize<AdditionalAttributes>(jsonContent);

                                        if (additionalAttributes != null)
                                        {
                                            Program._linkHeading.HyperlinkURL = additionalAttributes.HyperlinkURL;
                                            Program._linkHeading.HyperlinkText = additionalAttributes.HyperlinkText;
                                            Program._linkHeading.SpoilerTitle = additionalAttributes.SpoilerTitle;
                                            Program._linkHeading.SpoilerText = additionalAttributes.SpoilerText;

                                            Debug.WriteLine("Additional attributes loaded successfully.");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Failed to load additional attributes: {ex.Message}");
                                }
                            }
                            else if (Path.GetDirectoryName(entry.FullName) == "events" && entry.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                            {
                                // Load record events
                                try
                                {
                                    using (StreamReader reader = new StreamReader(entry.Open()))
                                    {
                                        string jsonContent = reader.ReadToEnd();
                                        var recordEvent = JsonSerializer.Deserialize<RecordEvent>(jsonContent);

                                        if (recordEvent != null)
                                        {
                                            _recordEvents.Add(recordEvent);
                                            EventCounter++;
                                            Debug.WriteLine($"Record event loaded: {recordEvent.ID}");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Failed to load record event: {ex.Message}");
                                }
                            }
                        }

                        // Sort the events by the Step attribute
                        _recordEvents.Sort((x, y) => x.Step.CompareTo(y.Step));
                        Debug.WriteLine("Record events sorted by step.");

                        // Update the UI with the sorted list
                        foreach (var recordEvent in _recordEvents)
                        {
                            _form1Instance?.Invoke((Action)(() => _form1Instance.AddRecordEventToListBox(recordEvent)));
                        }
                        Debug.WriteLine("UI updated with sorted record events.");
                    }
                }
                catch (JsonException ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Invalid JSON format: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine($"Invalid JSON format: {ex.Message}");
                }
                catch (IOException ex)
                {
                    System.Windows.Forms.MessageBox.Show($"File I/O error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine($"File I/O error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine($"An unexpected error occurred: {ex.Message}");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("File does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("File does not exist.");
            }
        }


        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule? curModule = curProcess.MainModule)
            {
                if (curModule != null)
                {
                    return SetWindowsHookEx(WindowHelper.WH_MOUSE_LL, proc, WindowHelper.GetModuleHandle(curModule.ModuleName), 0);
                }
                else
                {
                    // Handle the case where MainModule is null
                    // You can either return an error code, throw an exception, or handle it appropriately
                    throw new InvalidOperationException("The process does not have a main module.");
                }
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (!IsRecording)
                return WindowHelper.CallNextHookEx(_hookID, nCode, wParam, lParam);

            if (nCode >= 0 && (WindowHelper.MouseMessages.WM_LBUTTONDOWN == (WindowHelper.MouseMessages)wParam || WindowHelper.MouseMessages.WM_RBUTTONDOWN == (WindowHelper.MouseMessages)wParam))
            {
                WindowHelper.POINT cursorPos;
                if (WindowHelper.GetCursorPos(out cursorPos))
                {
                    IntPtr hwnd = WindowHelper.WindowFromPoint(cursorPos);
                    if (hwnd != IntPtr.Zero)
                    {
                        // Get window title
                        string? windowTitle = WindowHelper.GetTopLevelWindowTitle(hwnd);
                        // Get ApplicationName
                        string? applicationName=WindowHelper.GetApplicationName(hwnd);

                        // Get UI Element coordinates and size
                        WindowHelper.GetWindowRect(hwnd, out WindowHelper.RECT UIrect);
                        int UIWidth = UIrect.Right - UIrect.Left;
                        int UIHeight = UIrect.Bottom - UIrect.Top;

                        // Get window coordinates and size
                       
                        WindowHelper.RECT rect = WindowHelper.GetTopLevelWindowRect(hwnd);
                        int windowWidth = rect.Right - rect.Left;
                        int windowHeight = rect.Bottom - rect.Top;

                        // Get UI element under cursor
                        AutomationElement? element = GetElementFromCursor(new System.Windows.Point(cursorPos.X, cursorPos.Y));
                        string? elementName = null;
                        string? elementType = null;
                        string? tooltipText = null;
                        if (element != null)
                        {
                            elementName = element.Current.Name;
                            elementType = element.Current.LocalizedControlType;
                        }

                        // Get the tooltip text if available DOESN'T WORK
                        //AutomationElement? tooltipElement = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolTip));
                        //if (tooltipElement != null)
                        //{
                        //    tooltipText = tooltipElement.Current.Name;
                        //}

                        // Determine click type
                        string clickType = WindowHelper.MouseMessages.WM_LBUTTONDOWN == (WindowHelper.MouseMessages)wParam ? "Left Click" : "Right Click";

                        //Skip record if its to pause recording
                        if (elementName != "Pause Recording" && applicationName != "Better Steps Recorder")
                        {
                         
                    
                            // Create a record event object and add it to the list
                            RecordEvent recordEvent = new RecordEvent
                            {
                                WindowTitle = windowTitle,
                                ApplicationName=applicationName,
                                WindowCoordinates = new WindowHelper.RECT { Left = rect.Left, Top = rect.Top, Bottom = rect.Bottom, Right = rect.Right },
                                WindowSize = new WindowHelper.Size { Width = windowWidth, Height = windowHeight },
                                UICoordinates = new WindowHelper.RECT { Left = UIrect.Left, Top = UIrect.Top, Bottom = UIrect.Bottom, Right = UIrect.Right },
                                UISize = new WindowHelper.Size { Width= UIWidth, Height= UIHeight },
                                UIElement = element,
                                ElementName= elementName,
                                ElementType= elementType,
                                OGMouseCoordinates = new WindowHelper.POINT { X = cursorPos.X-rect.Left, Y = cursorPos.Y-rect.Top },
                                MouseCoordinates = new WindowHelper.POINT { X = cursorPos.X-rect.Left, Y = cursorPos.Y-rect.Top },
                                EventType = clickType,
                                //TODO Make this work with timed linstening event
                                TooltipText = tooltipText,
                                _StepText = $"In {applicationName}, {clickType} on  {elementType} {elementName}",
                                Step = _recordEvents.Count + 1
                            };
                            _recordEvents.Add(recordEvent);

                            // Take screenshot of the window
                            //string screenshotPath = SaveWindowScreenshot(hwnd, recordEvent.ID);
                            string? screenshotb64 = SaveScreenRegionScreenshot(rect.Left, rect.Top, windowWidth, windowHeight, recordEvent.ID);
                            if (screenshotb64 != null)
                            {
                                recordEvent.Screenshotb64 = screenshotb64;
                            }

                            // Update ListBox in Form1
                            _form1Instance?.Invoke((Action)(() => _form1Instance.AddRecordEventToListBox(recordEvent)));
                            _form1Instance?.Invoke((Action)(() => _form1Instance.activityTimer.Stop()));
                            _form1Instance?.Invoke((Action)(() => _form1Instance.activityTimer.Start()));
                            //zip?.SaveToZip();

                        }
                    }
                }
            }
            return WindowHelper.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static AutomationElement? GetElementFromCursor(System.Windows.Point point)
        {
            try
            {
                AutomationElement element = AutomationElement.FromPoint(new System.Windows.Point(point.X, point.Y));
                if (element != null)
                {
                    return element;
                }
                return null;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                // Handle the specific COM exception that may occur
                Console.WriteLine($"COM Exception: {ex.Message}");
                return null;
            }
        }

        public static string? SaveScreenRegionScreenshot(int x, int y, int width, int height, Guid eventId)
        {
            try
            {
                // Create a bitmap of the specified size
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                // Create graphics object from the bitmap
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    // Copy the specified screen area to the bitmap
                    gfx.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);


                }

                // Convert the bitmap to a memory stream
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    byte[] imageBytes = ms.ToArray();

                    // Convert byte array to Base64 string
                    string base64String = Convert.ToBase64String(imageBytes);

                    // Dispose of the bitmap
                    bmp.Dispose();

                    // Return the Base64 string
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to capture screenshot: {ex.Message}");
                return null;
            }
        }



        public static void ExportDoc(string docPath, string kind)
        {
            try
            {
                using (var writer = new StreamWriter(docPath))
                {
                    // Extract the filename without the extension
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(docPath);

                    string displayName = fileNameWithoutExtension.Replace("_", " ");

                    // Start the document based on the kind
                    switch (kind)
                    {
                        case "RTF":
                            writer.WriteLine("{\\rtf1\\ansi\\deff0");
                            writer.WriteLine($"\\b {fileNameWithoutExtension} \\b0\\par");
                            writer.WriteLine("\\par");
                            break;

                        case "MD":
                            writer.WriteLine($"---");
                            writer.WriteLine($"layout : default");
                            writer.WriteLine($"title: {displayName}");
                            writer.WriteLine($"---");

                            // Write additional properties
                            if (!string.IsNullOrEmpty(Program._linkHeading.HyperlinkText) && Program._linkHeading.HyperlinkURL != null)
                            {
                                writer.WriteLine($"[{Program._linkHeading.HyperlinkText}]({Program._linkHeading.HyperlinkURL})");
                            }

                            if (!string.IsNullOrEmpty(Program._linkHeading.SpoilerTitle) && !string.IsNullOrEmpty(Program._linkHeading.SpoilerText))
                            {
                                writer.WriteLine();
                                writer.WriteLine("<details>");
                                writer.WriteLine($"  <summary>{Program._linkHeading.SpoilerTitle}</summary>");
                                writer.WriteLine($"  {Program._linkHeading.SpoilerText}");
                                writer.WriteLine("</details>");
                            }

                            writer.WriteLine();
                            writer.WriteLine($"| {displayName} ||");
                            writer.WriteLine($"|-|-|");
                            break;

                        case "IMGs":
                            //don't trigger fault
                            break;

                        default:
                            throw new NotSupportedException($"The document type '{kind}' is not supported.");
                    }

                    // Initialize the list index
                    int stepNumber = 1;

                    // Create a new folder for images if exporting to Markdown
                    string imageFolderName = $"{fileNameWithoutExtension}-img";
                    string imageFolderPath = Path.Combine(Path.GetDirectoryName(docPath), imageFolderName);

                    if (kind == "MD")
                    {
                        if (Directory.Exists(imageFolderPath))
                        {
                            // Delete all image contents in the folder
                            DirectoryInfo di = new DirectoryInfo(imageFolderPath);
                            foreach (FileInfo file in di.GetFiles())
                            {
                                file.Delete();
                            }
                            foreach (DirectoryInfo dir in di.GetDirectories())
                            {
                                dir.Delete(true);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(imageFolderPath);
                        }
                    }

                    // Iterate through each record event and add to the document
                    foreach (var recordEvent in Program._recordEvents)
                    {
                        //generate display step number
                        string filenameStep = stepNumber < 10 ? $"0{stepNumber}" : stepNumber.ToString();
                        string printStep;
                        string tmpStepText;

                                if (recordEvent._StepText.Contains("::"))
                                {
             

                                    
                                    // Extract text up to "::" and append ":"
                                    int delimiterIndex = recordEvent._StepText.IndexOf("::");
                                    printStep = recordEvent._StepText.Substring(0, delimiterIndex);
                                    
                                    // Add Callout to Filname
                                    filenameStep += printStep;

                                    printStep += ":";

                                    // Extract the remaining text after "::"
                                    tmpStepText = recordEvent._StepText.Substring(delimiterIndex + 2);

                                   

                                }
                                else
                                {
                                     // Default behavior if "::" is not found
                                     printStep = $"#{stepNumber}";
                                     tmpStepText = recordEvent._StepText;

                                     // Increment stepNumber
                                     stepNumber++;
                                 }

                        // Write the step number and text
                        switch (kind)
                        {
                            case "RTF":
                                writer.WriteLine($"\\b Step {stepNumber}: \\b0 {recordEvent._StepText}\\par");
                                break;

                            case "MD":
                             


                                // Define the image filename and path
                                string imageFilename;
                                string imageFullPath;

                                if (recordEvent.Screenshotb64.StartsWith("iVBO"))
                                {
                                    imageFilename = $"{filenameStep}-{fileNameWithoutExtension}.png";
                                    imageFullPath = $"{imageFolderName}/{imageFilename}";
                                }
                                else
                                {
                                    imageFullPath = recordEvent.Screenshotb64;
                                }

                                // Write the Markdown table lines
                                writer.WriteLine($"| {printStep} ||");
                                writer.WriteLine($"| {tmpStepText} |![]({imageFullPath})|");
                                break;
                        }

     

                        // Decode the base64 screenshot
                        if (!string.IsNullOrEmpty(recordEvent.Screenshotb64) && recordEvent.Screenshotb64.StartsWith("iVBO"))
                        {
                            byte[] imageBytes = Convert.FromBase64String(recordEvent.Screenshotb64);
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                using (Image image = Image.FromStream(ms))
                                {
                                    using (MemoryStream rtfImageStream = GetRtfImage(image, recordEvent.MouseCoordinates.X, recordEvent.MouseCoordinates.Y, kind))
                                    {
                                        switch (kind)
                                        {

                                            case "IMGs":
                                                // Get stepTextImgName
                                                string stepTextImgName = recordEvent._StepText.Length > 25 
                                                ? recordEvent._StepText.Substring(0, 25) 
                                                : recordEvent._StepText;

                                                // Replace spaces with underscores and force lowercase
                                                stepTextImgName = stepTextImgName.Replace(" ", "_").ToLower();


                                                // Save the image to a file in the subdirectory
                                                string imageFilename2 = $"{stepTextImgName}.png";
                                                string imageFullPath2 = Path.Combine(Path.GetDirectoryName(docPath), imageFilename2);
                                                using (FileStream fileStream = new FileStream(imageFullPath2, FileMode.Create, FileAccess.Write))
                                                {
                                                    rtfImageStream.WriteTo(fileStream);
                                                }
                                                break;

                                            case "MD":
                                                // Save the image to a file in the subdirectory
                                                string imageFilename = $"{filenameStep}-{fileNameWithoutExtension}.png";
                                                string imageFullPath = Path.Combine(imageFolderPath, imageFilename);
                                                using (FileStream fileStream = new FileStream(imageFullPath, FileMode.Create, FileAccess.Write))
                                                {
                                                    rtfImageStream.WriteTo(fileStream);
                                                }
                                                break;

                                            case "RTF":
                                                // Convert the stream back to a string and write it
                                                rtfImageStream.Position = 0; // Reset the stream position to the beginning
                                                using (StreamReader reader = new StreamReader(rtfImageStream))
                                                {
                                                    string rtfImage = reader.ReadToEnd();
                                                    // Insert the image into the document
                                                    writer.WriteLine(rtfImage);
                                                }
                                                break;
                                        
                                            // If adding additional img export cases do not forget to update the logic in GetRTFImage as 'kind' is passed there too.
                                        }
                                    }
                                }
                            }
                        }


                        // Add two line breaks after each event
                        switch (kind)
                        {
                            case "RTF":
                                writer.WriteLine("\\par");
                                writer.WriteLine("\\par");
                                break;

                            case "MD":
                                break;
                        }
                    }

                    // End the RTF document if the kind is "RTF"
                    if (kind == "RTF")
                    {
                        writer.WriteLine("}");
                    }
                    
                }

                System.Windows.Forms.MessageBox.Show($"Exported {kind} at {docPath}", "Export completed successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException ioEx)
            {
                System.Windows.Forms.MessageBox.Show($"Failed to save the document. {ioEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private static MemoryStream GetRtfImage(Image image, int cursorX, int cursorY1, string kind)
        {
            const int cropWidth = 325;
            const int cropHeight = 250;
            int cursorY = Math.Abs(cursorY1);

            using (Bitmap bitmap = new Bitmap(image))
            {
                // Get the bounds of the active window
                int windowWidth = bitmap.Width;
                int windowHeight = bitmap.Height;

                // Calculate the cropping rectangle centered around the cursor
                int cropX = cursorX - cropWidth / 2;
                int cropY = cursorY - cropHeight / 2;

                // Adjust the cropping rectangle if it goes beyond the image boundaries
                if (cropX < 0) cropX = 0;
                if (cropY < 0) cropY = 0;
                if (cropX + cropWidth > windowWidth) cropX = windowWidth - cropWidth;
                if (cropY + cropHeight > windowHeight) cropY = windowHeight - cropHeight;

                Rectangle cropRect = new Rectangle(cropX, cropY, cropWidth, cropHeight);

                // Crop the image to the specified rectangle
                using (Bitmap croppedBitmap = new Bitmap(cropWidth, cropHeight))
                {
                    using (Graphics g = Graphics.FromImage(croppedBitmap))
                    {
                        g.DrawImage(bitmap, new Rectangle(0, 0, cropWidth, cropHeight), cropRect, GraphicsUnit.Pixel);

                        // Draw an arrow pointing at the cursor
                        if (cursorY1 > 0)
                        {
                            int arrowX = cursorX - cropX; // Cursor position relative to cropped image
                            int arrowY = cursorY - cropY;
                            DrawArrowAtCursor(g, arrowX, arrowY);
                        }
                    }

                    // Convert the cropped image to PNG format
                    MemoryStream stream = new MemoryStream();
                    croppedBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                    if (kind == "MD" || kind == "IMGs")
                    {
                        // Return the stream immediately for MD
                        return stream;
                    }

                    byte[] bytes = stream.ToArray();
                    int hexLength = bytes.Length;

                    // Create the RTF image content
                    StringBuilder sb = new StringBuilder();
                    sb.Append(@"{\pict\pngblip\picw");
                    sb.Append(cropWidth * 20); // Image width in twips
                    sb.Append(@"\pich");
                    sb.Append(cropHeight * 20); // Image height in twips
                    sb.Append(@"\picwgoal");
                    sb.Append(cropWidth * 20); // Target width in twips
                    sb.Append(@"\pichgoal");
                    sb.Append(cropHeight * 20); // Target height in twips
                    sb.Append(" ");
                    for (int i = 0; i < hexLength; i++)
                    {
                        sb.AppendFormat("{0:X2}", bytes[i]);
                    }
                    sb.Append("}");

                    return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
                }
            }
        }





        private static void DrawArrowAtCursor(Graphics gfx, int cursorX, int cursorY)
        {
            // Enable anti-aliasing for smoother lines
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Define the larger arrow properties
            Pen largeArrowPen = new Pen(Color.Magenta, 4);
            largeArrowPen.EndCap = System.Drawing.Drawing2D.LineCap.Custom;
            largeArrowPen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(5, 5); // Bigger arrow head

            // Define the smaller arrow properties
            Pen smallArrowPen = new Pen(Color.Black, 1);
            smallArrowPen.EndCap = System.Drawing.Drawing2D.LineCap.Custom;
            smallArrowPen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(8, 8); // Smaller arrow head

            // Define the length of the arrows
            int largeArrowLength = 35;
            int smallArrowLength = largeArrowLength - 4; // Smaller arrow is 3 pixels shorter

            // Constants for the angle and length of the arrow
            double angleDegrees = 215.0; // Angle in degrees (left of Y-axis)
            double angleRadians = Math.PI * angleDegrees / 180.0; // Convert to radians

            // Calculate the end point for the larger arrow based on the angle
            int largeEndX = cursorX - (int)(largeArrowLength * Math.Sin(angleRadians));
            int largeEndY = cursorY - (int)(largeArrowLength * Math.Cos(angleRadians));

            // Calculate the end point for the smaller arrow based on the angle
            int smallEndX = cursorX - (int)(smallArrowLength * Math.Sin(angleRadians));
            int smallEndY = cursorY - (int)(smallArrowLength * Math.Cos(angleRadians));

            // Draw a small (5 pixel) 20% transparent circle at the cursor position
            using (Brush transparentBrush = new SolidBrush(Color.FromArgb(90, Color.Gray))) // 20% transparent red
            {
                gfx.FillEllipse(transparentBrush, cursorX - 4, cursorY - 4, 10, 10); // 5 pixel circle centered at cursor
            }

            // Draw the larger arrow pointing at the given coordinates at a 35-degree angle left off the Y-axis
            gfx.DrawLine(largeArrowPen, largeEndX, largeEndY, cursorX, cursorY);

            // Draw the smaller arrow inside the larger arrow
            int smalloffset = -2;
            gfx.DrawLine(smallArrowPen, smallEndX - smalloffset, smallEndY - smalloffset, cursorX - smalloffset, cursorY - smalloffset);
        }
        public static Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(ms, true);
            }
        }

        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                return Convert.ToBase64String(imageBytes);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
    }
    public class AdditionalAttributes
    {
        public string HyperlinkURL { get; set; }
        public string HyperlinkText { get; set; }
        public string SpoilerTitle { get; set; }
        public string SpoilerText { get; set; }
    }
}