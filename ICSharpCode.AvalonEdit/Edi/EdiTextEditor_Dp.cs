﻿namespace ICSharpCode.AvalonEdit.Edi
{
  using System.Windows;
  using System.Windows.Media;

  /// <summary>
  /// This part of the AvalonEdit extension contains additional
  /// dependendency properties and their OnChanged...() method (if any)
  /// </summary>
  public partial class EdiTextEditor : TextEditor
  {
    #region fields
    #region EditorCurrentLineBackground
    // Style the background color of the current editor line
    private static readonly DependencyProperty EditorCurrentLineBackgroundProperty =
        DependencyProperty.Register("EditorCurrentLineBackground",
                                     typeof(SolidColorBrush),
                                     typeof(EdiTextEditor),
                                     new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(33, 33, 33, 33)),
                                     EdiTextEditor.OnCurrentLineBackgroundChanged));
    #endregion EditorCurrentLineBackground

    #region CaretPosition
    private static readonly DependencyProperty ColumnProperty =
        DependencyProperty.Register("Column", typeof(int), typeof(EdiTextEditor), new UIPropertyMetadata(1));


    public static readonly DependencyProperty LineProperty =
        DependencyProperty.Register("Line", typeof(int), typeof(EdiTextEditor), new UIPropertyMetadata(1));
    #endregion CaretPosition

    #region EditorStateProperties
    /// <summary>
    /// Editor selection start
    /// </summary>
    private static readonly DependencyProperty EditorSelectionStartProperty =
        DependencyProperty.Register("EditorSelectionStart", typeof(int), typeof(EdiTextEditor),
                                    new FrameworkPropertyMetadata(0,
                                                  FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Editor selection length
    /// </summary>
    private static readonly DependencyProperty EditorSelectionLengthProperty =
        DependencyProperty.Register("EditorSelectionLength", typeof(int), typeof(EdiTextEditor),
                                    new FrameworkPropertyMetadata(0));

    /// <summary>
    /// Selected text (if any) in text editor
    /// </summary>
    private static readonly DependencyProperty EditorSelectedTextProperty =
        DependencyProperty.Register("EditorSelectedText", typeof(string), typeof(EdiTextEditor),
                                            new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// TextEditor caret position
    /// </summary>
    private static readonly DependencyProperty EditorCaretOffsetProperty =
        DependencyProperty.Register("EditorCaretOffset", typeof(int), typeof(EdiTextEditor),
                                    new FrameworkPropertyMetadata(0));

    /// <summary>
    /// Determine whether current selection is a rectangle or not
    /// </summary>
    private static readonly DependencyProperty EditorIsRectangleSelectionProperty =
        DependencyProperty.Register("EditorIsRectangleSelection", typeof(bool), typeof(EdiTextEditor), new UIPropertyMetadata(false));

    #region EditorScrollOffsetXY
    /// <summary>
    /// Current editor view scroll X position
    /// </summary>
    public static readonly DependencyProperty EditorScrollOffsetXProperty =
        DependencyProperty.Register("EditorScrollOffsetX", typeof(double), typeof(EdiTextEditor), new UIPropertyMetadata(0.0));

    /// <summary>
    /// Current editor view scroll Y position
    /// </summary>
    public static readonly DependencyProperty EditorScrollOffsetYProperty =
        DependencyProperty.Register("EditorScrollOffsetY", typeof(double), typeof(EdiTextEditor), new UIPropertyMetadata(0.0));
    #endregion EditorScrollOffsetXY
    #endregion EditorStateProperties
    #endregion

    #region properties
    #region EditorCurrentLineBackground
    /// <summary>
    /// Style the background color of the current editor line
    /// </summary>
    public SolidColorBrush EditorCurrentLineBackground
    {
      get { return (SolidColorBrush)GetValue(EditorCurrentLineBackgroundProperty); }
      set { SetValue(EditorCurrentLineBackgroundProperty, value); }
    }
    #endregion EditorCurrentLineBackground

    #region CaretPosition
    public int Column
    {
      get
      {
        return (int)GetValue(ColumnProperty);
      }

      set
      {
        SetValue(ColumnProperty, value);
      }
    }

    public int Line
    {
      get
      {
          return (int)GetValue(LineProperty);
      }

      set
      {
        SetValue(LineProperty, value);
      }
    }
    #endregion CaretPosition

    #region EditorStateProperties
    /// <summary>
    /// Dependency property to allow ViewModel binding
    /// </summary>
    public int EditorSelectionStart
    {
      get
      {
        return (int)GetValue(EdiTextEditor.EditorSelectionStartProperty);
      }

      set
      {
        SetValue(EdiTextEditor.EditorSelectionStartProperty, value);
      }
    }

    /// <summary>
    /// Dependency property to allow ViewModel binding
    /// </summary>
    public int EditorSelectionLength
    {
      get
      {
        return (int)GetValue(EdiTextEditor.EditorSelectionLengthProperty);
      }

      set
      {
        SetValue(EdiTextEditor.EditorSelectionLengthProperty, value);
      }
    }

    /// <summary>
    /// Selected text (if any) in text editor
    /// </summary>
    public string EditorSelectedText
    {
      get
      {
        return (string)GetValue(EditorSelectedTextProperty);
      }

      set
      {
        SetValue(EditorSelectedTextProperty, value);
      }
    }

    /// <summary>
    /// Dependency property to allow ViewModel binding
    /// </summary>
    public int EditorCaretOffset
    {
      get
      {
        return (int)GetValue(EdiTextEditor.EditorCaretOffsetProperty);
      }

      set
      {
        SetValue(EdiTextEditor.EditorCaretOffsetProperty, value);
      }
    }

    /// <summary>
    /// Get property to determine whether ot not rectangle selection was used or not.
    /// </summary>
    public bool EditorIsRectangleSelection
    {
      get
      {
        return (bool)GetValue(EditorIsRectangleSelectionProperty);
      }

      set
      {
        SetValue(EditorIsRectangleSelectionProperty, value);
      }
    }

    #region EditorScrollOffsetXY
    public double EditorScrollOffsetX
    {
      get
      {
        return (double)GetValue(EditorScrollOffsetXProperty);
      }

      set
      {
        SetValue(EditorScrollOffsetXProperty, value);
      }
    }

    public double EditorScrollOffsetY
    {
      get
      {
        return (double)GetValue(EditorScrollOffsetYProperty);
      }

      set
      {
        SetValue(EditorScrollOffsetYProperty, value);
      }
    }
    #endregion EditorScrollOffsetXY
    #endregion EditorStateProperties
    #endregion properties

    #region methods
    /// <summary>
    /// The dependency property for has changed.
    /// Chnage the <seealso cref="SolidColorBrush"/> to be used for highlighting the current editor line
    /// in the particular <seealso cref="EdiView"/> control.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnCurrentLineBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      EdiTextEditor view = d as EdiTextEditor;

      if (view != null && e != null)
      {
        SolidColorBrush newValue = e.NewValue as SolidColorBrush;

        if (newValue != null)
        {
          view.AdjustCurrentLineBackground(newValue);
        }
      }
    }
    #endregion methods
  }
}