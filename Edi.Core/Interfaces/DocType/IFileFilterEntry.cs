namespace Edi.Core.Interfaces.DocType
{
	using System;
	using Edi.Core.ViewModels;

	public interface IFileFilterEntry
	{
		string FileFilter { get; }

		FileOpenDelegate FileOpenMethod { get; }
	}
}
