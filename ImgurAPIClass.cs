using System;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace BlackOstrich
{
	public class ImgurAPIClass
	{
		private string ClientID = "**Removed**";
		private string ClientSecret = "**Removed**";
		private string BaseURL = "https://api.imgur.com/3/";
		private string Pin { get; set; }
		private ImageClass imageClass { get; set;}

		public ImgurAPIClass (ImageClass ic)
		{
			imageClass = ic;
		}

		private void AuthorizeRequests(){
			string OAuth2Url = "https://api.imgur.com/oauth2/authorize?client_id={0}&response_type={1}&state={2}";
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(OAuth2Url, ClientID, "pin", "notneeded"));
			request.Method = "GET";
			request.ContentType = @"application/json";

			try {				
				using( HttpWebResponse rs = (HttpWebResponse)request.GetResponse()){
					using (var reader = new StreamReader(rs.GetResponseStream())) {
						JavaScriptSerializer js = new JavaScriptSerializer();
						var objText = reader.ReadToEnd();
						string Pin = (string)js.Deserialize(objText,typeof(string));
					}
				}
			}
			catch (WebException ex) {
				//figure it out
			}
		}

		public ImageModel UploadImage(){
			//AuthorizeRequests ();
			string url = BaseURL + "upload";

			//set up json object
			string Json = new JavaScriptSerializer().Serialize(new
			{
				image = imageClass.GetBase64OfCroppedImage () ,
				type = "base64"
			});
			
			HttpWebResponse response = POST (url, Json);
			using (HttpWebResponse rs = response) {
				using (var reader = new StreamReader(rs.GetResponseStream())) {
					JavaScriptSerializer js = new JavaScriptSerializer();
					var objText = reader.ReadToEnd();
					BasicImageModel BasicImageModel = (BasicImageModel)js.Deserialize(objText,typeof(BasicImageModel));
					return BasicImageModel.data;
				}
			}
		}


		// POST a JSON string
		private HttpWebResponse POST(string url, string jsonContent) 
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";

			request.Headers[HttpRequestHeader.Authorization] = "Client-ID " + ClientID;


			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
			Byte[] byteArray = encoding.GetBytes(jsonContent);

			request.ContentLength = byteArray.Length;
			request.ContentType = @"application/json";
			using (var streamWriter = new StreamWriter(request.GetRequestStream()))
			{
				streamWriter.Write(jsonContent);
				streamWriter.Flush();
				streamWriter.Close();
			}
			try {				
				return  (HttpWebResponse)request.GetResponse();
			}
			catch (WebException ex) {
				return null;
			}
		}
	}

	public class ImageModel{
		public string id { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public int datetime { get; set;}
		public string type { get; set; }
		public bool animated { get; set; }
		public int width { get; set; }
		public int height { get; set; }
		public int size { get; set; }
		public int views { get; set; }
		public int bandwidth{ get; set; }
		public string deletehash { get; set; }
		public string name { get; set; }
		public string section { get; set; }
		public string link { get; set; }
		public string gifv { get; set; }
		public string mp4 { get; set; }
		public string webm { get; set; }
		public bool? looping { get; set; }
		public bool favorite { get; set; }
		public bool? nsfw { get; set; }
		public string vote { get; set; }
		public string account_url { get; set; }
		public int account_id { get; set; }
	}

	public class BasicImageModel{
		public ImageModel data {get;set;}
		public int status {get;set;}
		public bool success {get;set;}
	}
}

