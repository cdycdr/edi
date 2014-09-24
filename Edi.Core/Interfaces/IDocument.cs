﻿namespace Edi.Core.Interfaces
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Edi.Core.ViewModels.Events;

	/// <summary>
	/// Inteface that is supported by document related viewmodels.
	/// </summary>
	public interface IDocument
	{
		#region events
		/// <summary>
		/// This event is fired when a document tells the framework that is wants to be closed.
		/// The framework can then close it and clean-up whatever is left to clean-up.
		/// </summary>
		event EventHandler<FileBaseEvent> DocumentEvent;
		#endregion events

		#region properties
		string FilePath
		{
			get;
		}

		/// <summary>
		/// Gets/sets whether a given document has been changed since loading
		/// from filesystem, or not.
		/// </summary>
		bool IsDirty
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the currently assigned name of the file in the file system.
		/// </summary>
		string FileName
		{
			get;
		}

		/// <summary>
		/// Get/set whether a given file path is a real existing path or not.
		/// 
		/// This is used to identify files that have never been saved and can
		/// those not be remembered in an MRU etc...
		/// </summary>
		bool IsFilePathReal
		{
			get;
			set;
		}

		/// <summary>
		/// Get whether edited data can be saved or not.
		/// A type of document does not have a save
		/// data implementation if this property returns false.
		/// (this is document specific and should always be overriden by descendents)
		/// </summary>
		bool CanSaveData { get; }
		#endregion properties

		#region methods
		/// <summary>
		/// Indicate whether document can be closed or not.
		/// </summary>
		/// <returns></returns>
		bool CanClose();

		/// <summary>
		/// Indicate whether document can be saved in the currennt state.
		/// </summary>
		/// <returns></returns>
		bool CanSave();

		/// <summary>
		/// Indicate whether document can be saved as.
		/// </summary>
		/// <returns></returns>
		bool CanSaveAs();

		/// <summary>
		/// Save this document as.
		/// </summary>
		/// <returns></returns>
		bool SaveFile(string filePath);
		#endregion methods
	}
}
