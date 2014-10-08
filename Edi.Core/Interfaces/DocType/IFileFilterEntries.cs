namespace Edi.Core.Interfaces.DocType
{
	public interface IFileFilterEntries
	{
		string GetFilterString();

		FileOpenDelegate GetFileOpenMethod(int idx);
	}
}
