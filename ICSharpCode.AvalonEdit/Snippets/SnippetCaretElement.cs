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
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// Sets the caret position after interactive mode has finished.
	/// </summary>
	[Serializable]
	public class SnippetCaretElement : SnippetElement
	{
		/// <inheritdoc/>
		public override void Insert(InsertionContext context)
		{
			if (!string.IsNullOrEmpty(context.SelectedText))
				SetCaret(context);
		}
		
		internal static void SetCaret(InsertionContext context)
		{
			TextAnchor pos = context.Document.CreateAnchor(context.InsertionPosition);
			pos.MovementType = AnchorMovementType.BeforeInsertion;
			pos.SurviveDeletion = true;
			context.Deactivated += (sender, e) => {
				if (e.Reason == DeactivateReason.ReturnPressed || e.Reason == DeactivateReason.NoActiveElements) {
					context.TextArea.Caret.Offset = pos.Offset;
				}
			};
		}
	}
}
