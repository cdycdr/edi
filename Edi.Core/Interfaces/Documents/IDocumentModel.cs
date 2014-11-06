﻿namespace Edi.Core.Interfaces.Documents
{
	/// <summary>
	/// Interface defines properties and methods of a base class for modelling
	/// file access on persistent storage.
	/// </summary>
	public interface IDocumentModel
	{
		#region properties
		/// <summary>
		/// Gets whether the file content on storake (harddisk) can be changed
		/// or whether file is readonly through file properties.
		/// </summary>
		bool IsReadonly { get; }

		/// <summary>
		/// Determines whether a document has ever been stored on disk or whether
		/// the current path and other file properties are currently just initialized
		/// in-memory with defaults.
		/// </summary>
		bool IsReal { get; }

		/// <summary>
		/// Gets the complete path and file name for this document.
		/// </summary>
		string FileNamePath { get; }

		/// <summary>
		/// Gets the name of a file.
		/// </summary>
		string FileName { get; }

		/// <summary>
		/// Gets the path of a file.
		/// </summary>
		string Path { get; }

		/// <summary>
		/// Gets the file extension of the document represented by this path.
		/// </summary>
		string FileExtension { get; }
		#endregion properties

		#region methods
		/// <summary>
		/// Assigns a filename and path to this document model. This will also
		/// refresh all properties (IsReadOnly etc..) that can be queried for this document.
		/// </summary>
		/// <param name="fileNamePath"></param>
		/// <param name="isReal">Determines whether file exists on disk
		/// (file open -> properties are refreshed from persistence) or not
		/// (properties are reset to default).</param>
		void SetFileNamePath(string fileNamePath, bool isReal);

		/// <summary>
		/// Resets the IsReal property to adjust model when a new document has been saved
		/// for the very first time.
		/// </summary>
		/// <param name="isReal">Determines whether file exists on disk
		/// (file open -> properties are refreshed from persistence) or not
		/// (properties are reset to default).</param>
		void SetIsReal(bool isReal);
		#endregion methods
	}
}
