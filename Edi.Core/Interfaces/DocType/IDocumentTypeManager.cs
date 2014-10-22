namespace Edi.Core.Interfaces.DocType
{
	using System;
	using System.Collections.ObjectModel;
	using Edi.Core.ViewModels;

	public delegate ViewModels.FileBaseViewModel FileOpenDelegate(string pathFilename, object settingsManager);

	public interface IDocumentTypeManager
	{
		#region properties
		ObservableCollection<IDocumentType> DocumentTypes { get; }
		#endregion properties

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Key"></param>
		/// <param name="FileFilterName"></param>
		/// <param name="DefaultFilter"></param>
		/// <param name="FileOpenMethod">Is a static method that returns <seealso cref="FileBaseViewModel"/>
		/// and takes a string (path) and ISettingsManager as parameter.</param>
		/// <param name="t"></param>
		/// <returns></returns>
		IDocumentType RegisterDocumentType(string Key,
																			 string Name,
																			 string FileFilterName,
																			 string DefaultFilter,               // eg: 'log4j'
																			 FileOpenDelegate FileOpenMethod,
																			 Type t,
																			 int sortPriority = 0
																			 );

		/// <summary>
		/// Finds a document type that can handle a file
		/// with the given file extension eg ".txt" or "txt"
		/// when the original file name was "Readme.txt".
		/// 
		/// Always returns the 1st document type handler that matches the extension.
		/// </summary>
		/// <param name="fileExtension"></param>
		/// <param name="trimPeriod">Determines if an additional '.' character is removed
		/// from the given extension string or not.</param>
		/// <returns></returns>
		IDocumentType FindDocumentTypeByExtension(string fileExtension,
																							bool trimPeriod = false);

		IDocumentType FindDocumentTypeByKey(string typeOfDoc);

		/// <summary>
		/// Goes through all file/document type definitions and returns a filter string
		/// object that can be used in conjunction with FileOpen and FileSave dialog filters.
		/// </summary>
		/// <param name="key">Get entries for this viewmodel only,
		/// or all entries if key parameter is not set.</param>
		/// <returns></returns>
		IFileFilterEntries GetFileFilterEntries(string key = "");
	}
}
