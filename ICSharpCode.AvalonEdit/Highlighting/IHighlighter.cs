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
using System.Collections.Generic;
#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#else
using ICSharpCode.AvalonEdit.Document;
#endif
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// Represents a highlighted document.
	/// </summary>
	/// <remarks>This interface is used by the <see cref="HighlightingColorizer"/> to register the highlighter as a TextView service.</remarks>
	public interface IHighlighter
	{
		/// <summary>
		/// Gets the underlying text document.
		/// </summary>
		TextDocument Document { get; }
		
			/// <summary>
		/// Gets the span stack at the end of the specified line.
		/// -> GetSpanStack(1) returns the spans at the start of the second line.
		/// </summary>
		/// <remarks>GetSpanStack(0) is valid and will always return the empty stack.</remarks>
		ImmutableStack<HighlightingSpan> GetSpanStack(int lineNumber);
		
		/// <summary>
		/// Highlights the specified document line.
		/// </summary>
		/// <param name="lineNumber">The line to highlight.</param>
		/// <returns>A <see cref="HighlightedLine"/> line object that represents the highlighted sections.</returns>
		HighlightedLine HighlightLine(int lineNumber);
	}
}
