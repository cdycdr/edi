// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing
{
	class ImeSupport : IDisposable
	{
		TextArea textArea;
		IntPtr currentContext;
		IntPtr previousContext;
		HwndSource hwndSource;
		
		public ImeSupport(TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			this.textArea = textArea;
			InputMethod.SetIsInputMethodSuspended(this.textArea, true);
			textArea.GotKeyboardFocus += TextAreaGotKeyboardFocus;
			textArea.LostKeyboardFocus += TextAreaLostKeyboardFocus;
			textArea.OptionChanged += TextAreaOptionChanged;
			currentContext = IntPtr.Zero;
			previousContext = IntPtr.Zero;
		}

		void TextAreaOptionChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "EnableImeSupport" && textArea.IsKeyboardFocusWithin) {
				CreateContext();
			}
		}
		
		public void Dispose()
		{
			if (textArea != null) {
				textArea.GotKeyboardFocus -= TextAreaGotKeyboardFocus;
				textArea.LostKeyboardFocus -= TextAreaLostKeyboardFocus;
				textArea.OptionChanged -= TextAreaOptionChanged;
				textArea = null;
			}
			ClearContext();
		}

		void ClearContext()
		{
			if (hwndSource != null) {
				hwndSource.RemoveHook(WndProc);
				ImeNativeWrapper.AssociateContext(hwndSource, previousContext);
				previousContext = IntPtr.Zero;
				ImeNativeWrapper.ReleaseContext(hwndSource, currentContext);
				hwndSource = null;
				currentContext = IntPtr.Zero;
			}
		}
		
		void TextAreaGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (this.textArea == null)
				return;
			if (e.OriginalSource != this.textArea)
				return;
			CreateContext();
		}

		void CreateContext()
		{
			if (this.textArea == null)
				return;
      //Dirkster99 BugFix
      if (this.textArea.Options == null)
        return;

			if (!textArea.Options.EnableImeSupport)
				return;
			hwndSource = (HwndSource)PresentationSource.FromVisual(this.textArea);
			if (hwndSource != null) {
				currentContext = ImeNativeWrapper.GetContext(hwndSource);
				previousContext = ImeNativeWrapper.AssociateContext(hwndSource, currentContext);
//				ImeNativeWrapper.SetCompositionFont(hwndSource, currentContext, textArea);
				hwndSource.AddHook(WndProc);
			}
		}
		
		void TextAreaLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (e.OriginalSource != this.textArea)
				return;
			if (currentContext != IntPtr.Zero)
				ImeNativeWrapper.NotifyIme(currentContext);
			ClearContext();
		}
		
		IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg) {
				case ImeNativeWrapper.WM_INPUTLANGCHANGE:
					ClearContext();
					CreateContext();
					break;
				case ImeNativeWrapper.WM_IME_COMPOSITION:
					UpdateCompositionWindow();
					break;
			}
			return IntPtr.Zero;
		}
		
		public void UpdateCompositionWindow()
		{
			if (currentContext != IntPtr.Zero && textArea != null) {
				ImeNativeWrapper.SetCompositionWindow(hwndSource, currentContext, textArea);
			}
		}
	}
}