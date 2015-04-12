using System;
using Gtk;
using Gdk;
using System.Drawing;
using System.IO;


namespace BlackOstrich
{
	public class ImageClass
	{
		//our main image that will be cropped
		public Gtk.Image mainImage{ get; set; }
		public int imageWidth { get; set; }
		public int imageHeight { get; set; }
		public string originalImageName = "screenshot.png";
		public string croppedImageName = "cropped.png";
		private ImgurAPIClass imgurAPIClass;

		public ImageClass ()
		{
			imgurAPIClass = new ImgurAPIClass (this);
		}

		//Take screen shot of desktop
		public Gtk.Image GetScreenshot(){
			int width = 0;
			int height = 0;
			Gdk.Window root = Gdk.Global.DefaultRootWindow;
			root.GetSize(out width, out height);
			Pixbuf screenshot = new Pixbuf(Gdk.Colorspace.Rgb, false, 8,width, height).GetFromDrawable(root, Gdk.Colormap.System, 0, 0, 0, 0, width, height);
			Gtk.Image image = new Gtk.Image (screenshot);
			screenshot.Save(originalImageName, "jpeg");
			mainImage = image;
			imageWidth = screenshot.Width;
			imageHeight = screenshot.Height;
			return image;
		}

		public void cropimage(Gdk.Point mouseStart, Gdk.Point mouseEnd){
			System.Drawing.Rectangle cropRect = new System.Drawing.Rectangle (0, 0, imageWidth, imageHeight);
			//determine rectangle direction 
			if (mouseStart != mouseEnd) {
				if (mouseEnd.X >= mouseStart.X && mouseEnd.Y >= mouseStart.Y) {
					//went down + right
					cropRect = new System.Drawing.Rectangle(mouseStart.X, mouseStart.Y, (mouseEnd.X - mouseStart.X), (mouseEnd.Y - mouseStart.Y));

					Console.WriteLine ("-------------------------------------");
					Console.WriteLine ("Rectangle Direction: Down + Right");
					Console.WriteLine ("-------------------------------------");
				} else if (mouseEnd.X <= mouseStart.X && mouseEnd.Y <= mouseStart.Y) {
					//went up left
					cropRect = new System.Drawing.Rectangle(mouseEnd.X, mouseEnd.Y, (mouseStart.X - mouseEnd.X), (mouseStart.Y - mouseEnd.Y));

					Console.WriteLine ("-------------------------------------");
					Console.WriteLine ("Rectangle Direction: Up + Left");
					Console.WriteLine ("-------------------------------------");
				} else if (mouseEnd.X >= mouseStart.X && mouseEnd.Y < mouseStart.Y) {
					//went up right
					//get new start and end
					int startX = mouseStart.X;
					int startY = mouseEnd.Y;
					int endX = mouseEnd.X;
					int endY = mouseStart.Y;
					cropRect = new System.Drawing.Rectangle(startX, startY, (endX - startX), (endY - startY));

					Console.WriteLine ("-------------------------------------");
					Console.WriteLine ("Image Width: {0}, Image Height: {1}", imageWidth, imageHeight);
					Console.WriteLine ("SP: ({0}, {1})     EP: ({2}, {3})", startX, startY, endX, endY);
					Console.WriteLine ("Rectangle Direction: Up + Right");
					Console.WriteLine ("-------------------------------------");
				} else if (mouseEnd.X <= mouseStart.X && mouseEnd.Y >= mouseStart.Y) {
					//went down left
					//get new start and end
					int startX = mouseEnd.X;
					int startY = mouseStart.Y;
					int endX = mouseStart.X;
					int endY = mouseEnd.Y;
					cropRect = new System.Drawing.Rectangle(startX, startY, (endX - startX), (endY - startY));

					Console.WriteLine ("-------------------------------------");
					Console.WriteLine ("Image Width: {0}, Image Height: {1}", imageWidth, imageHeight);
					Console.WriteLine ("SP: ({0}, {1})     EP: ({2}, {3})", startX, startY, endX, endY);
					Console.WriteLine ("Rectangle Direction: Down + Left");
					Console.WriteLine ("-------------------------------------");
				}
			}

			Bitmap src = System.Drawing.Image.FromFile(originalImageName) as Bitmap;
			Bitmap target = src.Clone (cropRect, src.PixelFormat);
			target.Save (croppedImageName);


			var buffer = System.IO.File.ReadAllBytes (croppedImageName);
			Pixbuf cropped = new Gdk.Pixbuf(buffer);
			mainImage = new Gtk.Image (cropped);
			imageWidth = target.Width;
			imageHeight = target.Height;

			//cleanup
			src.Dispose ();
			target.Dispose ();

			//open the new image i guess

		}


		//upload image and copy link to clipboard
		public void UploadImageToImgur(){
			//upload to imgur
			var a = imgurAPIClass.UploadImage();

			//copy link to clipboard
			Gtk.Clipboard clipboard = Gtk.Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
			clipboard.Text = a.link;
		}

		public string GetBase64OfCroppedImage(){
			System.Drawing.Image croppedImage = System.Drawing.Image.FromFile (croppedImageName);
			return ImageToBase64 (croppedImage, System.Drawing.Imaging.ImageFormat.Png);
		}


		public string ImageToBase64(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				// Convert Image to byte[]
				image.Save(ms, format);
				byte[] imageBytes = ms.ToArray();

				// Convert byte[] to Base64 String
				string base64String = Convert.ToBase64String(imageBytes);
				image.Dispose ();
				return base64String;
			}
		}

	}
}

