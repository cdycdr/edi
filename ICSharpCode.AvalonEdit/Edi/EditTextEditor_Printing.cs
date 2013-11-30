namespace ICSharpCode.AvalonEdit.Edi
{
  using System.Windows.Input;
  using ICSharpCode.AvalonEdit.Edi.PrintEngine;

  public partial class EdiTextEditor : TextEditor
  {
    /// <summary>
    /// Executes the collapse all folds command (which folds all text foldings but the first).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void PrintDocument(object sender, ExecutedRoutedEventArgs e)
    {
      EdiTextEditor edi = sender as EdiTextEditor;

      if (edi == null)
        return;

      edi.PrintPreviewDocument();
    }

    /// <summary>
    /// Determines whether a folding command can be executed or not and sets correspondind
    /// <paramref name="e"/>.CanExecute property value.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void PrintDocumentCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = false;
      e.Handled = true;

      EdiTextEditor edi = sender as EdiTextEditor;

      if (edi == null)
        return;

      e.CanExecute = true;
    }

    private void PrintPreviewDocument()
    {
      // Printing.PageSetupDialog();              // .NET dialog

      Printing.PrintPreviewDialog(this, "Print Document");           // WPF print preview dialog

      /* Printing.PrintPreviewDialog(filename);   // WPF print preview dialog, filename as document title

      Printing.PrintDialog();                   // WPF print dialog

      Printing.PrintDialog(filename);          // WPF print dialog, filename as document title

      Printing.PrintDirect();                   // prints to default or previously selected printer

      Printing.PrintDirect(filename);           // prints to default or previously selected printer, filename as document title
       */
    }
  }
}
