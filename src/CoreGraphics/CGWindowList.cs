#if MONOMAC
using System;
using System.Runtime.InteropServices;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreFoundation;

namespace XamCore.CoreGraphics
{
	public static class CGWindowList
	{
#if !COREBUILD
		[DllImport (Constants.CoreGraphicsLibrary)]
		internal static extern IntPtr CGWindowListCreateImage (CGRect screenBounds, CGWindowListOption windowOption, uint windowID, CGWindowImageOption imageOption);

		public static CGImage ScreenImage (uint windownumber, CGRect bounds)
		{
			IntPtr imageRef = CGWindowListCreateImage (bounds, CGWindowListOption.IncludingWindow, windownumber,
								  CGWindowImageOption.Default);
			if (imageRef == IntPtr.Zero)
				return null;
			return new CGImage (imageRef, true);
		}

		public static CGImage ScreenImage (CGRect bounds, CGWindowListOption windowOption, uint windownumber, CGWindowImageOption imageOption)
		{
			IntPtr imageRef = CGWindowListCreateImage (bounds, windowOption, windownumber, imageOption);

			if (imageRef == IntPtr.Zero)
				return null;

			return new CGImage (imageRef, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGWindowListCreateImage (CGRect bounds, IntPtr windowArray, CGWindowImageOption imageOption);

		public static CGImage ScreenImage (CGRect bounds, uint [] windows, CGWindowImageOption imageOption)
		{
			var numberArray = new NSNumber [windows.Length];

			if (windows.Length > 0) {
				for (int i = 0; i < windows.Length; i++) {
					numberArray [i] = NSNumber.FromNFloat ((nfloat)windows [i]);
				}
			}

			var array = NSArray.FromNSObjects (numberArray);

			IntPtr imageRef = CGWindowListCreateImage (bounds, array.Handle, imageOption);

			if (imageRef == IntPtr.Zero)
				return null;

			return new CGImage (imageRef, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr /* CFArrayRef */ CGWindowListCopyWindowInfo (CGWindowListOption option, /* CGWindowID*/ uint relativeToWindow);

		public static NSDictionary CopyWindowInfo (CGWindowListOption option, int relativeToWindow)
		{ 
			IntPtr cfArrayRef = CGWindowListCopyWindowInfo (option, (uint)relativeToWindow);

			if (cfArrayRef == IntPtr.Zero || CFArray.GetCount (cfArrayRef) == 0)
				return null;

			return new NSDictionary (cfArrayRef);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CFArrayRef */ IntPtr CGWindowListCreate (CGWindowListOption option, uint relativeToWindow);

		public static uint [] GetWindowList (CGWindowListOption option, int relativeToWindow)
		{
			IntPtr cfArrayRef = CGWindowListCreate (option, (uint)relativeToWindow);

			if (cfArrayRef == IntPtr.Zero)
				return new uint [0];

			var array = new CFArray (cfArrayRef);

			if (array == null)
				return new uint [0];

			nint length = CFArray.GetCount (cfArrayRef);
			uint [] idList = new uint [length];

			for (uint i = 0; i < length; i++)
				idList [i] = (uint)array.GetValue ((int)i);

			return idList;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CFArrayRef */ IntPtr CGWindowListCreateDescriptionFromArray (IntPtr /* CFArrayRef */ windowArray);

		public static NSDictionary [] GetDescription (uint[] windows)
		{
				var intPtrArray = new IntPtr [windows.Length];

				for (int i = 0; i < windows.Length; i++)
					intPtrArray [i] = (IntPtr) windows [i];

				var arrayRef = CFArray.CreateRawValues (intPtrArray);

				IntPtr cfArrayRef = CGWindowListCreateDescriptionFromArray (arrayRef);

				if (cfArrayRef == IntPtr.Zero)
					return new NSDictionary [0];

				var descriptions = NSArray.ArrayFromHandle<NSDictionary> (cfArrayRef);

				return descriptions;
		}
#endif
	}
}
#endif