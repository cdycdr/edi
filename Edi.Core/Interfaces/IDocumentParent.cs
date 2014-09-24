namespace Edi.Core.Interfaces
{
	using System;
	using Edi.Core.ViewModels;
	using Edi.Core.ViewModels.Base;

	public interface IDocumentParent
	{
		/// <summary>
		/// This event is raised when the active document changes
		/// (a new/different or no document becomes active).
		/// </summary>
		event DocumentChangedEventHandler ActiveDocumentChanged;

		/// <summary>
		/// Gets the viewmodel of the currently selected document.
		/// </summary>
		IDocument ActiveDocument
		{
			get;
			set;
		}

		////void Close(IDocument fileToClose);
		////void Save(IDocument fileToSave, bool saveAsFlag = false);
	}
}
