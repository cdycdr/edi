namespace Edi.Core.Interfaces.DocType
{
	using System.Collections.Generic;

	public interface IDocumentTypeItem
	{
		List<string> DocFileTypeExtensions { get; }   // eg: *.*
		string Description { get; }        // eg: 'All Files'

		int SortPriority { get; }
	}
}
