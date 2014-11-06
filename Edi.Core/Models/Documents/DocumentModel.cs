namespace Edi.Core.Models.Documents
{
	using System;
	using Edi.Core.Interfaces.Documents;

	/// <summary>
	/// Class models the basic properties and behaviours of a low level file stored on harddisk.
	/// </summary>
	public class DocumentModel : IDocumentModel
	{
		#region constructors
		/// <summary>
		/// Hidden standard class constructor
		/// </summary>
		public DocumentModel()
		{
			this.SetDefaultDocumentModel();
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		public DocumentModel(DocumentModel copyThis)
			: this()
		{
			if (copyThis == null)
				return;

			this.IsReadonly = copyThis.IsReadonly;
			this.IsReal = copyThis.IsReal;
			this.FileNamePath = copyThis.FileNamePath;
		}
		#endregion constructors

		#region properties
		/// <summary>
		/// Gets whether the file content on storake (harddisk) can be changed
		/// or whether file is readonly through file properties.
		/// </summary>
		public bool IsReadonly { get; private set; }

		/// <summary>
		/// Determines whether a document has ever been stored on disk or whether
		/// the current path and other file properties are currently just initialized
		/// in-memory with defaults.
		/// </summary>
		public bool IsReal { get; private set; }

		/// <summary>
		/// Gets the complete path and file name for this document.
		/// </summary>
		public string FileNamePath { get; private set; }

		/// <summary>
		/// Gets the name of a file.
		/// </summary>
		public string FileName
		{
			get
			{
				return System.IO.Path.GetFileName(this.FileNamePath);
			}
		}

		/// <summary>
		/// Gets the path of a file.
		/// </summary>
		public string Path
		{
			get
			{
				return System.IO.Path.GetFullPath(this.FileNamePath);
			}
		}

		/// <summary>
		/// Gets the file extension of the document represented by this path.
		/// </summary>
		public string FileExtension
		{
			get
			{
				return System.IO.Path.GetExtension(this.FileNamePath);
			}
		}
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
		public void SetFileNamePath(string fileNamePath, bool isReal)
		{
			this.FileNamePath = fileNamePath;
			this.IsReal = isReal;

			if (this.IsReal == true)
				this.QueryFileProperies();
		}

		/// <summary>
		/// Resets the IsReal property to adjust model when a new document has been saved
		/// for the very first time.
		/// </summary>
		/// <param name="IsReal">Determines whether file exists on disk
		/// (file open -> properties are refreshed from persistence) or not
		/// (properties are reset to default).</param>
		public void SetIsReal(bool isReal)
		{
			this.IsReal = isReal;

			if (this.IsReal == true)
				this.QueryFileProperies();
		}

		/// <summary>
		/// Query sub-system for basic properties if this file is supposed to exist in persistence.
		/// </summary>
		private void QueryFileProperies()
		{
			System.IO.FileInfo f = new System.IO.FileInfo(this.FileNamePath);
			this.IsReadonly = f.IsReadOnly;
		}

		/// <summary>
		/// Resets all document property values to their defaults.
		/// </summary>
		private void SetDefaultDocumentModel()
		{
			this.IsReadonly = true;
			this.IsReal = false;
			this.FileNamePath = string.Empty;
		}
		#endregion methods
	}
}
