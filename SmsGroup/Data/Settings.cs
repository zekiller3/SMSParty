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
			Console.WriteLine(string.Format("Locale: {0} - Language: {1}",
		         NSLocale.CurrentLocale.LocaleIdentifier,
		         NSLocale.PreferredLanguages[0]));
			return localeBundle.LocalizedString(key, comment);
		}
	}
}

