using NUnit.Framework;
using System;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
#else
using AppKit;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class CGWindowListTests 
	{
		[Test]
		public void CGWindow_ScreenImageTest()
		{
			var image = CGWindowList.ScreenImage (0, new CGRect (0, 0, 200, 200));
			Assert.NotNull (image);
			
			image = CGWindowList.ScreenImage (new CGRect (0, 0, 200, 200), CGWindowListOption.All, 0, CGWindowImageOption.Default);
			Assert.NotNull (image);
			
			image = CGWindowList.ScreenImage (new CGRect (0, 0, 200, 200), new uint [] {0, 1}, CGWindowImageOption.Default);
			Assert.NotNull (image);
		}

		[Test]
		public void CGWindow_CopyWindowInfo ()
		{
			var info = CGWindowList.CopyWindowInfo (CGWindowListOption.All, 0);
			Assert.NotNull (info);
		}
		
		[Test]
		public void CGWindow_GetWindowList ()
		{
			var windowList = CGWindowList.GetWindowList (CGWindowListOption.All, 0);
		}
		
		[Test]
		public void CGWindow_GetDescription ()
		{
			var windowList = CGWindowList.GetWindowList (CGWindowListOption.All, 0);
			var description = CGWindowList.GetDescription (windowList);
			Assert.NotNull (description);
		}
	}
}


