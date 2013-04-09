namespace ICSharpCode.AvalonEdit.Edi.Intellisense
{
  using ICSharpCode.AvalonEdit.CodeCompletion;

  public interface ICompletionWindowResolver
	{
		CompletionWindow Resolve();
	}
}