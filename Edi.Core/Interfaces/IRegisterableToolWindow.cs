﻿namespace Edi.Core.Interfaces
{
	public interface IRegisterableToolWindow
	{
		/// <summary>
		/// Set the document parent handling object to deactivation and activation
		/// of documents with content relevant to this tool window viewmodel.
		/// </summary>
		/// <param name="parent"></param>
		void SetDocumentParent(IDocumentParent parent);

		/// <summary>
		/// Set the document parent handling object and visibility
		/// to enable tool window to react on deactivation and activation
		/// of documents with content relevant to this tool window viewmodel.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="isVisible"></param>
		void SetToolWindowVisibility(IDocumentParent parent,
                                        bool isVisible = true);
	}
}
