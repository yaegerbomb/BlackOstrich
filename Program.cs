using System;
using Gtk;
using GLib;
using Gdk;
using System.Drawing;
using System.Windows.Forms;

namespace BlackOstrich
{
	public 

	class MainClass
	{
		//mouse tracking variables
		private Gdk.Point mouseStart = new Gdk.Point(0,0); //top left
		private Gdk.Point mouseEnd = new Gdk.Point(0,0); //bottom right
		private Gdk.Point topRightCoords = new Gdk.Point(0,0); //top right
		private Gdk.Point bottomLeftCoords = new Gdk.Point(0,0);
		private ImageClass imageClass = new ImageClass();

		//gui tracking variables for clean up
		private MainWindow mainWindowTracker;
		private HBox hbox;
		private VBox vbox;
		private ImageMenuItem newi;
		private MenuBar menuBar;
		private Gtk.Menu filemenu;
		private Gtk.MenuItem file;
		private AccelGroup agr;
		private SeparatorMenuItem sep;
		private ImageMenuItem exit;
		private Gtk.Button uploadButton;
		private EventBox notificationEventBox;
		private EventBox toCropBox;
		private Gtk.Menu popupMenu;
		private ImageMenuItem menuItemQuit;
		private Gtk.Image appimg;
		private Gtk.Label notificationMessage;
		private StatusIcon trayIcon;
		private Gdk.Color RectangleColor = new Gdk.Color (51,255,0);

		//display our newly cropped image to send off
		void DisplayCroppedImage(){
			//clear our rectangle and such
			toCropBox.GdkWindow.Clear ();

			mainWindowTracker.HideAll ();

			//crop our image
			imageClass.GetScreenshot ();
			imageClass.cropimage (mouseStart, mouseEnd);

			mainWindowTracker.Unfullscreen ();
			mainWindowTracker.Opacity = 1;
			mainWindowTracker.Resize (imageClass.imageWidth, imageClass.imageHeight);
			mainWindowTracker.Decorated = true;
			mainWindowTracker.GdkWindow.Cursor = new Gdk.Cursor(CursorType.Arrow);

			//add cropped image to event box
			toCropBox.Add (imageClass.mainImage);

			//remove our widget from window
			mainWindowTracker.Remove(toCropBox);

			//remove button drag events
			toCropBox.ButtonPressEvent -= DragStart;
			toCropBox.ButtonReleaseEvent -= DragEnd;
			toCropBox.MotionNotifyEvent -= Dragging;

			BuildMenuBar ();
		}

		//build menubar
		void BuildMenuBar(){
			//create new menu bar
			menuBar = new MenuBar();

			//create 'file' dropdown
			filemenu = new Gtk.Menu();
			file = new Gtk.MenuItem("File");
			file.Submenu = filemenu;


			agr = new AccelGroup();
			mainWindowTracker.AddAccelGroup(agr);

			//add new button to file dropdown
			newi = new ImageMenuItem(Stock.New, agr);
			newi.AddAccelerator("activate", agr, new AccelKey(
				Gdk.Key.n, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
			newi.Activated += NewCrop;
			filemenu.Append(newi);

			//add hr line
			sep = new SeparatorMenuItem();
			filemenu.Append(sep);

			//add close button
			exit = new ImageMenuItem(Stock.Quit, agr);
			exit.AddAccelerator("activate", agr, new AccelKey(
				Gdk.Key.q, Gdk.ModifierType.ControlMask, AccelFlags.Visible));

			//set up exit button
			exit.Activated += OnActivated;
			filemenu.Append(exit);

			//add to menu bar
			menuBar.Append(file);

			//add new vbox
			vbox = new VBox(false, 2);
			vbox.PackStart(menuBar, true, true, 0);

			//add our event box that contains our image
			vbox.PackStart (toCropBox);

			//add our upload button
			hbox = new HBox();
			uploadButton = new Gtk.Button ();
			uploadButton.Label = "Upload Image";
			uploadButton.Clicked += UploadCroppedImage;
			hbox.PackEnd (uploadButton, false, false, 2);
			vbox.PackEnd (hbox, false, false, 5);

			//finally add to main window
			mainWindowTracker.Add(vbox);

			mainWindowTracker.ShowAll ();
		}

		//destroy everything but our tray icon, main window, and our notification bubble
		private void DestroyMostlyEverything(){
			if (hbox != null) {
				hbox.Destroy ();
			}

			if (vbox != null) {
				vbox.Destroy();
			}

			if (newi != null) {
				newi.Destroy ();
			}

			if (menuBar != null) {
				menuBar.Destroy ();
			}

			if (filemenu != null) {
				filemenu.Destroy();
			}

			if(file != null){
				file.Destroy();
			}

			if (sep != null) {
				sep.Destroy ();
			}

			if (exit != null) {
				exit.Destroy ();
			}

			if (uploadButton != null) {
				uploadButton.Destroy ();
			}

			if (toCropBox != null) {
				toCropBox.Destroy ();
			}

			if (agr != null) {
				agr.Dispose ();
			}
		}

		//literally, destroy all our gui stuff
		private void DestroyEverything(){
			if (hbox != null) {
				hbox.Destroy ();
			}

			if (vbox != null) {
				vbox.Destroy();
			}

			if (newi != null) {
				newi.Destroy ();
			}

			if (menuBar != null) {
				menuBar.Destroy ();
			}

			if (filemenu != null) {
				filemenu.Destroy();
			}

			if(file != null){
				file.Destroy();
			}

			if (sep != null) {
				sep.Destroy ();
			}

			if (exit != null) {
				exit.Destroy ();
			}

			if (uploadButton != null) {
				uploadButton.Destroy ();
			}

			if (toCropBox != null) {
				toCropBox.Destroy ();
			}

			if (appimg != null) {
				appimg.Destroy ();
			}

			if (menuItemQuit != null) {
				menuItemQuit.Destroy ();
			}

			if (popupMenu != null) {
				popupMenu.Destroy ();
			}

			if (notificationMessage != null) {
				notificationMessage.Destroy ();
			}

			if (notificationEventBox != null) {
				notificationEventBox.Destroy ();
			}

			if (mainWindowTracker != null) {
				mainWindowTracker.Destroy ();
			}

			if (trayIcon != null) {
				trayIcon.Dispose ();
			}

			if (agr != null) {
				agr.Dispose ();
			}
		}

		void DisplayUploadNotification(MainWindow mainWindow){
			//set our window width
			int frameWidth = 250;
			int frameHeight = 100;
			int screenWidth = Gdk.Screen.Default.Width;

			//need to dispose of all our stuff in the window to handle memory
			DestroyMostlyEverything();


			//add event box
			notificationEventBox = new EventBox ();

			//add click event to close app on click
			notificationEventBox.ButtonPressEvent += OnActivated;

			//add label to eb
			notificationMessage = new Gtk.Label("Image Uploaded. Link copied to clipboard");
			notificationMessage.ModifyFg(StateType.Normal, new Gdk.Color (255, 255, 255)); //change font color to white
			notificationEventBox.Add (notificationMessage);

			mainWindow.Decorated = false;
			mainWindow.Resize (frameWidth, frameHeight);
			mainWindow.Move ((screenWidth - frameWidth), 0);

			//change background color to black
			notificationEventBox.ModifyBg(StateType.Normal, new Gdk.Color(67,67,67));

			mainWindow.Add(notificationEventBox);
			mainWindow.ShowAll ();

			//fadeout timer function
			GLib.Timeout.Add (100, new GLib.TimeoutHandler (update_opacity));
		}

		//this will handle our notification fade
		bool update_opacity ()
		{
			// returning true means that the timeout routine should be invoked
			// again after the timeout period expires.   Returning false would
			// terminate the timeout.
			if (mainWindowTracker.Opacity > 0) {
				mainWindowTracker.Opacity -= .05;
				return true;
			} else {
				mainWindowTracker.HideAll ();
				//delete our notification stuff
				notificationEventBox.Destroy ();
				notificationMessage.Destroy ();
				return false;
			}
		}

		void initStart(){
			//add event box so we can track clicking
			toCropBox = new EventBox ();
			mainWindowTracker.Add (toCropBox);

			//set rectangle color
			toCropBox.ModifyBase(StateType.Normal, RectangleColor); 

			//window decoration
			mainWindowTracker.Decorated = false;
			mainWindowTracker.Opacity = .5;
			mainWindowTracker.Fullscreen ();
			mainWindowTracker.GdkWindow.Cursor = new Gdk.Cursor(CursorType.Plus);


			//add our key events
			mainWindowTracker.KeyPressEvent += EntryKeyPressEvent;
			mainWindowTracker.DeleteEvent += delete_event;
			toCropBox.ButtonPressEvent += DragStart;
			toCropBox.ButtonReleaseEvent += DragEnd;
			toCropBox.MotionNotifyEvent += Dragging;

			mainWindowTracker.ShowAll();

		}

		#region Events

		//upload button event
		void UploadCroppedImage(object sender, EventArgs args){
			mainWindowTracker.HideAll ();
			imageClass.UploadImageToImgur ();

			//remove all our chidlren as we are done with them
			mainWindowTracker.Remove (mainWindowTracker.Child);

			//display success
			DisplayUploadNotification (mainWindowTracker);
		}

		//event for when we are creating our rectangle of cropness
		void Dragging(object sender, Gtk.MotionNotifyEventArgs args)
		{
			toCropBox.GdkWindow.Clear ();

			//draw horizontal lines
			toCropBox.GdkWindow.DrawLine(toCropBox.Style.BaseGC(StateType.Normal), mouseStart.X, mouseStart.Y, topRightCoords.X, topRightCoords.Y);
			toCropBox.GdkWindow.DrawLine(toCropBox.Style.BaseGC(StateType.Normal), bottomLeftCoords.X, bottomLeftCoords.Y, mouseEnd.X, mouseEnd.Y);

			//draw vertical lines
			toCropBox.GdkWindow.DrawLine(toCropBox.Style.BaseGC(StateType.Normal), mouseStart.X, mouseStart.Y, bottomLeftCoords.X, bottomLeftCoords.Y);
			toCropBox.GdkWindow.DrawLine(toCropBox.Style.BaseGC(StateType.Normal), topRightCoords.X, topRightCoords.Y, mouseEnd.X, mouseEnd.Y);

			//set our new coords
			mouseEnd = new Gdk.Point((int)Math.Floor(args.Event.X),(int)Math.Floor(args.Event.Y));
			topRightCoords = new Gdk.Point (mouseEnd.X, mouseStart.Y);
			bottomLeftCoords = new Gdk.Point (mouseStart.X, mouseEnd.Y);

			Console.WriteLine("-------------------------------------");
			Console.WriteLine("Mouse Moved");
			Console.WriteLine ("Mouse End Point: ({0}, {1})", mouseEnd.X, mouseEnd.Y);
			Console.WriteLine ("Top Right Point: ({0}, {1})", topRightCoords.X, topRightCoords.Y);
			Console.WriteLine ("Bottom Left Point: ({0}, {1})", bottomLeftCoords.X, bottomLeftCoords.Y);
			Console.WriteLine("-------------------------------------");
		}

		//when we have let go of our mouse button
		void DragEnd(object sender, Gtk.ButtonReleaseEventArgs args)
		{
			mouseEnd = new Gdk.Point((int)Math.Floor(args.Event.X),(int)Math.Floor(args.Event.Y));
			topRightCoords = new Gdk.Point (mouseEnd.X, mouseStart.Y);
			bottomLeftCoords = new Gdk.Point (mouseStart.X, mouseEnd.Y);

			Console.WriteLine("-------------------------------------");
			Console.WriteLine("Mouse Released");
			Console.WriteLine ("Mouse End Point: ({0}, {1})", mouseEnd.X, mouseEnd.Y);
			Console.WriteLine ("Top Right Point: ({0}, {1})", topRightCoords.X, topRightCoords.Y);
			Console.WriteLine ("Bottom Left Point: ({0}, {1})", bottomLeftCoords.X, bottomLeftCoords.Y);
			Console.WriteLine("-------------------------------------");

			//all about dat crop
			DisplayCroppedImage();
		}

		//when we click our mouse button
		void DragStart(object sender, Gtk.ButtonPressEventArgs args)
		{
			// single click
			if (args.Event.Type == EventType.ButtonPress)
			{
				mouseStart = new Gdk.Point((int)Math.Floor(args.Event.X),(int)Math.Floor(args.Event.Y));
				Console.WriteLine("-------------------------------------");
				Console.WriteLine("Mouse Clicked");
				Console.WriteLine ("Mouse Start Point: ({0}, {1})", mouseStart.X, mouseStart.Y);
				Console.WriteLine("-------------------------------------");
			}
		}

		//close program is we hit escape
		void EntryKeyPressEvent(object o, Gtk.KeyPressEventArgs args)
		{
			Console.WriteLine("DEBUG: KeyValue: " + args.Event.KeyValue);

			if (args.Event.KeyValue == 65307) {
				DestroyMostlyEverything ();
				mainWindowTracker.HideAll ();
			}
		}

		//quit application if we close the window
		void delete_event (object obj, DeleteEventArgs args)
		{
			DestroyMostlyEverything ();
			mainWindowTracker.HideAll ();
			args.RetVal = true;
			Gtk.Application.Run ();
		}


		//menu bar exit
		void OnActivated(object sender, EventArgs args)
		{
			DestroyEverything ();
			Gtk.Application.Quit();
		}

		//menu bar new
		void NewCrop(object sender, EventArgs args)
		{
			DestroyMostlyEverything ();
			initStart ();
		}

		// Create the popup menu, on right click.
		void OnTrayIconPopup (object o, EventArgs args) {
			popupMenu = new Gtk.Menu();
			menuItemQuit = new ImageMenuItem ("Quit");
			appimg = new Gtk.Image(Stock.Quit, IconSize.Menu);
			menuItemQuit.Image = appimg;
			popupMenu.Add(menuItemQuit);

			// Quit the application when quit has been clicked.
			menuItemQuit.Activated += OnActivated;
			popupMenu.ShowAll();
			popupMenu.Popup();
		}

		void ShowOrHideApplication(object o, EventArgs args){
			if(mainWindowTracker.Visible){
				mainWindowTracker.HideAll();
			}else{
				mainWindowTracker.ShowAll();
			}
		}

		void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
		{
			initStart();
			Console.WriteLine("Hot Key Pressed");
		}
		#endregion

		//start it up yo
		public static void Main (string[] args)
		{
			//declare main window
			MainClass main = new MainClass ();
			Gtk.Application.Init ();
			MainWindow win = new MainWindow ();

			//so we can reference this in our class
			main.mainWindowTracker = win;

			//create tray icon
			main.trayIcon = new StatusIcon(new Pixbuf ("trayicon.png"));
			main.trayIcon.Visible = true;

			// Show/Hide the window (even from the Panel/Taskbar) when the TrayIcon has been clicked.
			main.trayIcon.Activate += main.ShowOrHideApplication;
			// Show a pop up menu when the icon has been right clicked.
			main.trayIcon.PopupMenu += main.OnTrayIconPopup;

			// A Tooltip for the Icon
			main.trayIcon.Tooltip = "Black Ostrich";


			#region events
			//set up close window quit event
			HotKeyManager.RegisterHotKey(Keys.Oemtilde, KeyModifiers.Control);
			HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(main.HotKeyManager_HotKeyPressed);
			#endregion

			win.HideAll ();

			//finally show all and run
			Gtk.Application.Run ();
		}
	}
}
