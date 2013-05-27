namespace ICSharpCode.AvalonEdit.Edi
{
  using System;
  using System.Collections.ObjectModel;
  using System.Windows;
  using System.Windows.Media;
  using System.Windows.Threading;

  using ICSharpCode.AvalonEdit.Editing;
  using ICSharpCode.AvalonEdit.Rendering;
  using ICSharpCode.AvalonEdit.Utils;
using System.Windows.Input;

  /// <summary>
  /// </summary>
  public partial class EdiTextEditor : TextEditor
  {
    #region constructor
    /// <summary>
    /// Static class constructor to register style key for usage in XAML
    /// </summary>
    static EdiTextEditor()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(EdiTextEditor), new FrameworkPropertyMetadata(typeof(EdiTextEditor)));
      FocusableProperty.OverrideMetadata(typeof(EdiTextEditor),
                                         new FrameworkPropertyMetadata(Boxes.True));
    }

    /// <summary>
    /// Class Constructor
    /// (review the OnApplyTemplate() method for most construction routines)
    /// </summary>
    public EdiTextEditor()
      : base()
    {
      // This is default list of viewing scales that is bound by the default XAML skin in EdiTextEditor.xaml
      // Consumers can use other lists by binding to their viewmodel when re-styling the control in the
      // applications resource dictionary
      this.ScaleList = new ObservableCollection<string>(){ "20 %",
                                                           "50 %",
                                                           "75 %",
                                                           "100 %",
                                                           "125 %",
                                                           "150 %",
                                                           "175 %",
                                                           "200 %",
                                                           "300 %",
                                                           "400 %"};
    }
    #endregion constructor

    #region properties
    #region ScaleViewOutput
    /// <summary>
    /// Get/Set viewmodel to display a customizable font size
    /// This is default list of viewing scales that is bound by the default XAML skin in EdiTextEditor.xaml
    /// Consumers can use other lists by binding to their viewmodel when re-styling the control in the
    /// applications resource dictionary
    /// </summary>
    public ObservableCollection<string> ScaleList { get; set; }
    #endregion ScaleViewOutput
    #endregion properties

    #region methods
    /// <summary>
    /// Standard <seealso cref="System.Windows.Controls.Control"/> method to be executed when the
    /// framework applies the XAML definition of the lookless control.
    /// </summary>
		public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      // Highlight current line in editor (even if editor is not focused) via themable dp-property
      this.AdjustCurrentLineBackground(this.EditorCurrentLineBackground);

      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);

      // Highlight current line in editor (even if editor is not focused) via themable dp-property
      this.AdjustCurrentLineBackground(this.EditorCurrentLineBackground);

      // Update highlighting of current line when caret position is changed
      this.TextArea.Caret.PositionChanged += (sender, e) =>
      {
        this.TextArea.TextView.InvalidateLayer(KnownLayer.Background); //Update current line highlighting

        if (this.TextArea != null)
        {
          this.Column = this.TextArea.Caret.Column;
          this.Line = this.TextArea.Caret.Line;
        }
        else
          this.Column = this.Line = 0;
      };
    }

    /// <summary>
    /// Hock event handlers and restore editor states (if any) or defaults
    /// when the control is fully loaded.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    private void OnLoaded(object obj, RoutedEventArgs args)
    {
      // Call folding once upon loading so that first run is executed right away
      this.foldingUpdateTimer_Tick(null, null);

      this.mFoldingUpdateTimer = new DispatcherTimer();
      mFoldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
      mFoldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
      mFoldingUpdateTimer.Start();

      // Connect CompletionWindow Listners
      this.TextArea.TextEntering += TextEditorTextAreaTextEntering;
      this.TextArea.TextEntered += TextEditorTextAreaTextEntered;
      this.Focus();
      this.ForceCursor = true;

      // Restore cusor position for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
      this.ScrollToHorizontalOffset(this.EditorScrollOffsetX);
      this.ScrollToVerticalOffset(this.EditorScrollOffsetY);

      try
      {
        if (this.EditorIsRectangleSelection == true)
        {
          this.CaretOffset = this.EditorCaretOffset;

          this.SelectionStart = this.EditorSelectionStart;
          this.SelectionLength = this.EditorSelectionLength;

          // See OnMouseCaretBoxSelection in Editing\CaretNavigationCommandHandler.cs
          // Convert normal selection to rectangle selection
          this.TextArea.Selection = new ICSharpCode.AvalonEdit.Editing.RectangleSelection(this.TextArea,
                                                                                          this.TextArea.Selection.StartPosition,
                                                                                          this.TextArea.Selection.EndPosition);
        }
        else
        {
          this.CaretOffset = this.EditorCaretOffset;

          // http://www.codeproject.com/Articles/42490/Using-AvalonEdit-WPF-Text-Editor?msg=4388281#xx4388281xx
          this.Select(this.EditorSelectionStart, this.EditorSelectionLength);
        }
      }
      catch
      {
      }

      // Attach mouse wheel CTRL-key zoom support
      this.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(textEditor_PreviewMouseWheel);
    }

    /// <summary>
    /// Unhock event handlers and save editor states (to be recovered later)
    /// when the control is unloaded.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    private void OnUnloaded(object obj, RoutedEventArgs args)
    {
      if (this.mFoldingUpdateTimer != null)
        this.mFoldingUpdateTimer = null;

      this.Loaded -= this.OnLoaded;
      this.Unloaded -= this.OnUnloaded;

      this.TextArea.TextEntering -= TextEditorTextAreaTextEntering;
      this.TextArea.TextEntered -= TextEditorTextAreaTextEntered;

      // Save cusor position for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
      this.EditorCaretOffset = this.CaretOffset;
      this.EditorSelectionStart = this.SelectionStart;
      this.EditorSelectionLength = this.SelectionLength;
      this.EditorIsRectangleSelection = this.TextArea.Selection is RectangleSelection;

      // http://stackoverflow.com/questions/11863273/avalonedit-how-to-get-the-top-visible-line
      this.EditorScrollOffsetX = this.TextArea.TextView.ScrollOffset.X;
      this.EditorScrollOffsetY = this.TextArea.TextView.ScrollOffset.Y;

      // Detach mouse wheel CTRL-key zoom support
      // This does not work when doing mouse zoom and CTRL-TAB between two documents and trying to do mouse zoom???
      ////this.PreviewMouseWheel -= textEditor_PreviewMouseWheel;
    }

    /// <summary>
    /// Reset the <seealso cref="SolidColorBrush"/> to be used for highlighting the current editor line.
    /// </summary>
    /// <param name="newValue"></param>
    private void AdjustCurrentLineBackground(SolidColorBrush newValue)
    {
      if (newValue != null)
      {
        this.TextArea.TextView.BackgroundRenderers.Clear();

        this.TextArea.TextView.BackgroundRenderers.Add(
                new HighlightCurrentLineBackgroundRenderer(this, newValue.Clone()));
      }
    }

    /// <summary>
    /// This method is triggered on a MouseWheel preview event to check if the user
    /// is also holding down the CTRL Key and adjust the current font size if so.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void textEditor_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
      if (Keyboard.Modifiers == ModifierKeys.Control)
      {
        double fontSize = this.FontSize + e.Delta / 25.0;

        if (fontSize < 6)
          this.FontSize = 6;
        else
        {
          if (fontSize > 200)
            this.FontSize = 200;
          else
            this.FontSize = fontSize;
        }

        e.Handled = true;
      }
    }
    #endregion methods
  }
}
