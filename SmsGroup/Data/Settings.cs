using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
namespace SmsGroup
{
	public class Settings
	{
		public static int MaxGroupFreeVersion = 5;
		
		private static NSBundle localeBundle = null;
		
		private static string magicExpressionFirstName = "/f";
		public static string MagicExpressionFirstName 
		{
			get {return magicExpressionFirstName;}
			set	{magicExpressionFirstName = value;}
		}
		private static string magicExpressionLastName = "/l";
		public static string MagicExpressionLastName
		{
			get {return magicExpressionLastName;}
			set	{magicExpressionLastName = value;}
		}
		
		private static int defaultMaxRecipient = 10;
		public static int DefaultMaxRecipient
		{
			get {return defaultMaxRecipient;}
			set {defaultMaxRecipient = value;}
		}
		
		private static UIImage background = null;
		public static UIImage Background
		{
			get
			{
				if(background == null)
				{
					background = UIImage.FromBundle("Images/SMSBackground.jpg");
				}
				
				return background;
			}
		}
		
		public static UIView GenerateHeaderFooter(string text, string localizedKey, int policeSize = 14)
		{
			UIFont f = UIFont.BoldSystemFontOfSize (policeSize);
			string transformedText = Settings.GetLocalizedString(text, localizedKey);
			
			var result = new UILabel (new RectangleF (10, 0, 320, 20)){
				    Font = f,
				    BackgroundColor = UIColor.Clear,
					TextColor = UIColor.White,
					Text = transformedText
				};
			//Console.WriteLine("displaying " + transformedText);
			//Console.WriteLine("Size HeaderFooter : {0}x{1}", result.Frame.Size.Height, result.Frame.Size.Width);
			SizeF targetSize = result.StringSize(transformedText, f);
			int line = (int)Math.Ceiling(targetSize.Width / result.Frame.Size.Width);
			//Console.WriteLine("Target Size HeaderFooter : {0}x{1}, should need {2}", targetSize.Height, targetSize.Width, line);
			if(line > 1){
				result.Frame = new RectangleF (10, 0, 320, 50 * line);
				result.Lines = 0;
				result.LineBreakMode = UILineBreakMode.WordWrap;
				result.SizeToFit();
			}
			
			UIView v = new UIView(new RectangleF(0,0, result.Frame.Size.Width, result.Frame.Size.Height));
			v.Add(result);
			return v;
		}
		
		static Settings ()
		{
			string resourcePath = NSBundle.MainBundle.PathForResource(NSLocale.PreferredLanguages[0], "lproj");
			if(string.IsNullOrEmpty(resourcePath))
			{
				resourcePath = NSBundle.MainBundle.PathForResource("en", "lproj");
			}
			
			localeBundle = NSBundle.FromPath(resourcePath);
		}
		
		
		public static string GetLocalizedString(string key, string comment)
		{
//			Console.WriteLine(string.Format("Locale: {0} - Language: {1}",
//		         NSLocale.CurrentLocale.LocaleIdentifier,
//		         NSLocale.PreferredLanguages[0]));
			return localeBundle.LocalizedString(key, comment);
		}
	}
}

