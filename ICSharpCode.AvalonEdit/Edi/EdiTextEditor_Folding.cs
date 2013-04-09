namespace ICSharpCode.AvalonEdit.Edi
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using ICSharpCode.AvalonEdit.Folding;
  using ICSharpCode.AvalonEdit.Highlighting;
  using ICSharpCode.AvalonEdit.Edi.Folding;
  using System.Windows.Threading;

  /// <summary>
  /// This part of the AvalonEdit extension contains the code
  /// necessary to manage folding strategies for various types of text.
  /// </summary>
  public partial class EdiTextEditor : TextEditor
  {
    #region fields
    FoldingManager mFoldingManager = null;
    AbstractFoldingStrategy mFoldingStrategy = null;

    private bool mInstallFoldingManager = false;
    private DispatcherTimer mFoldingUpdateTimer = null;
    #endregion fields

    #region Methods
    /// <summary>
    /// This method is executed via <seealso cref="TextEditor"/> class when the Highlighting for
    /// a text display is changed durring the live time of the control.
    /// </summary>
    /// <param name="newValue"></param>
		protected override void OnSyntaxHighlightingChanged(IHighlightingDefinition newValue)
		{
      base.OnSyntaxHighlightingChanged(newValue);

      if (newValue != null)
        this.SetFolding(newValue);
    }

    /// <summary>
    /// Determine whether or not highlighting can be
    /// suppported by a particular folding strategy.
    /// </summary>
    /// <param name="syntaxHighlighting"></param>
    public void SetFolding(IHighlightingDefinition syntaxHighlighting)
    {
      if (syntaxHighlighting == null)
      {
        this.mFoldingStrategy = null;
      }
      else
      {
        switch (syntaxHighlighting.Name)
        {
          case "XML":
          case "HTML":
            mFoldingStrategy = new XmlFoldingStrategy() { ShowAttributesWhenFolded = true };
            this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
            break;
          case "C#":
          case "C++":
          case "PHP":
          case "Java":
            this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(this.Options);
            mFoldingStrategy = new BraceFoldingStrategy();
            break;
          case "VBNET":
            this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(this.Options);
            mFoldingStrategy = new VBNetFoldingStrategy();
            break;
          default:
            this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
            mFoldingStrategy = null;
            break;
        }

        if (mFoldingStrategy != null)
        {
          if (this.Document != null)
          {
            if (mFoldingManager == null)
              mFoldingManager = FoldingManager.Install(this.TextArea);

            this.mFoldingStrategy.UpdateFoldings(mFoldingManager, this.Document);
          }
          else
            this.mInstallFoldingManager = true;
        }
        else
        {
          if (mFoldingManager != null)
          {
            FoldingManager.Uninstall(mFoldingManager);
            mFoldingManager = null;
          }
        }
      }
    }

    /// <summary>
    /// Update the folding in the text editor when requested per call on this method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void foldingUpdateTimer_Tick(object sender, EventArgs e)
    {
      if (this.IsVisible == true)
      {
        if (mInstallFoldingManager == true)
        {
          if (this.Document != null)
          {
            if (mFoldingManager == null)
            {
              mFoldingManager = FoldingManager.Install(this.TextArea);

              mInstallFoldingManager = false;
            }
          }
          else
            return;
        }

        if (mFoldingStrategy != null)
        {
          mFoldingStrategy.UpdateFoldings(mFoldingManager, this.Document);
        }
      }
    }
    #endregion
  }
}
