﻿namespace ICSharpCode.AvalonEdit.BracketRenderer
{
  public class DefaultBracketSearcher : IBracketSearcher
  {
    public static readonly DefaultBracketSearcher DefaultInstance = new DefaultBracketSearcher();

    public BracketSearchResult SearchBracket(ICSharpCode.AvalonEdit.Document.TextDocument document, int offset)
    {
      return null;
    }
  }
}
