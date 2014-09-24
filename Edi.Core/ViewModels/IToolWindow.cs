namespace Edi.Core.ViewModels
{

	public interface IToolWindow
	{
		string Name
		{
			get;
		}

		bool IsVisible
		{
			get;
		}

		void SetToolWindowVisibility(bool isVisible = true);
	}
}
