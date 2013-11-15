namespace EdiViews.ViewModel.Documents
{
  using System;
  using System.Collections.ObjectModel;
  using System.Globalization;
  using System.IO;
  using System.Text;
  using System.Windows.Input;
  using ICSharpCode.AvalonEdit.Document;
  using ICSharpCode.AvalonEdit.Edi.TextBoxControl;
  using ICSharpCode.AvalonEdit.Highlighting;
  using ICSharpCode.AvalonEdit.Utils;
  using MsgBox;
  using Settings.ProgramSettings;
  using SimpleControls.Command;
  using UnitComboLib.Unit.Screen;
  using UnitComboLib.ViewModel;

  /// <summary>
  /// This viewmodel class represents the business logic of the text editor.
  /// Each text editor document instance is associated with a <seealso cref="EdiViewModel"/> instance.
  /// </summary>
  public class EdiViewModel : EdiViews.ViewModel.Base.FileBaseViewModel, EdiViews.FindReplace.ViewModel.IEditor
  {
    #region Fields
    private ICSharpCode.AvalonEdit.TextEditorOptions mTextOptions;
    private IHighlightingDefinition mHighlightingDefinition;
    private static int iNewFileCounter = 1;
    private string defaultFileType = "txt";
    private readonly static string defaultFileName = Util.Local.Strings.STR_FILE_DEFAULTNAME;

    private object lockThis = new object();

    private bool mWordWrap = false;            // Toggle state command
    private bool mShowLineNumbers = true;     // Toggle state command
    #endregion Fields

    #region constructor
    /// <summary>
    /// Standard constructor. See also static <seealso cref="LoadFile"/> method
    /// for construction from file saved on disk.
    /// </summary>
    public EdiViewModel()
    {
      // Copy text editor settings from settingsmanager by default
      this.TextOptions = new ICSharpCode.AvalonEdit.TextEditorOptions(Settings.SettingsManager.Instance.SettingData.EditorTextOptions);
      this.WordWrap = Settings.SettingsManager.Instance.SettingData.WordWarpText;
      var items = new ObservableCollection<ListItem>(Options.GenerateScreenUnitList());

      this.SizeUnitLabel = new UnitViewModel(items, new ScreenConverter(), 0);

      this.TxtControl = new TextBoxController();
      
      this.FilePath = string.Format(CultureInfo.InvariantCulture, "{0} {1}.{2}",
                                    EdiViewModel.defaultFileName, EdiViewModel.iNewFileCounter++, this.defaultFileType);

      this.IsDirty = false;
      this.mHighlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(this.mFilePath));;

      this.mDocument = new TextDocument();

      this.TextEditorSelectionStart = 0;
      this.TextEditorSelectionLength = 0;
    }
    #endregion constructor

    #region properties
    #region FilePath
    private string mFilePath = null;

    /// <summary>
    /// Get/set complete path including file name to where this stored.
    /// This string is never null or empty.
    /// </summary>
    public override string FilePath
    {
      get
      {
        if (this.mFilePath == null || this.mFilePath == String.Empty)
          return string.Format(CultureInfo.CurrentCulture, "{0}.{1}", EdiViewModel.defaultFileName, this.defaultFileType);

        return this.mFilePath;
      }

      protected set
      {
        if (this.mFilePath != value)
        {
          this.mFilePath = value;

          this.NotifyPropertyChanged(() => this.FilePath);
          this.NotifyPropertyChanged(() => this.FileName);
          this.NotifyPropertyChanged(() => this.Title);

          this.HighlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(this.mFilePath));
        }
      }
    }
    #endregion

    #region Title
    /// <summary>
    /// Title is the string that is usually displayed - with or without dirty mark '*' - in the docking environment
    /// </summary>
    public override string Title
    {
      get
      {
        return this.FileName + (this.IsDirty == true ? "*" : string.Empty);
      }
    }
    #endregion

    #region FileName
    /// <summary>
    /// FileName is the string that is displayed whenever the application refers to this file, as in:
    /// string.Format(CultureInfo.CurrentCulture, "Would you like to save the '{0}' file", FileName)
    /// 
    /// Note the absense of the dirty mark '*'. Use the Title property if you want to display the file
    /// name with or without dirty mark when the user has edited content.
    /// </summary>
    public override string FileName
    {
      get
      {
        // This option should never happen - its an emergency break for those cases that never occur
        if (FilePath == null || FilePath == String.Empty)
          return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", EdiViewModel.defaultFileName, this.defaultFileType);

        return System.IO.Path.GetFileName(FilePath);
      }
    }

    public override Uri IconSource
    {
      get
      {
        // This icon is visible in AvalonDock's Document Navigator window
        return new Uri("pack://application:,,,/EdiViews;component/Images/Documents/document.png", UriKind.RelativeOrAbsolute);
      }
    }
    #endregion FileName

    #region IsReadOnly
    private bool mIsReadOnly = false;
    public bool IsReadOnly
    {
      get
      {
        return this.mIsReadOnly;
      }

      protected set
      {
        if (this.mIsReadOnly != value)
        {
          this.mIsReadOnly = value;
          this.NotifyPropertyChanged(() => this.IsReadOnly);
        }
      }
    }

    private string mIsReadOnlyReason = string.Empty;
    public string IsReadOnlyReason
    {
      get
      {
        return this.mIsReadOnlyReason;
      }

      protected set
      {
        if (this.mIsReadOnlyReason != value)
        {
          this.mIsReadOnlyReason = value;
          this.NotifyPropertyChanged(() => this.IsReadOnlyReason);
        }
      }
    }
    #endregion IsReadOnly

    #region TextContent
    TextDocument mDocument;

    /// <summary>
    /// This property wraps the document class provided by AvalonEdit. The actual text is inside
    /// the document and can be accessed at save, load or other processing times.
    /// 
    /// The Text property itself cannot be bound in AvalonEdit since binding this would result
    /// in updating the text (via binding) each time a user enters a key on the keyboard
    /// (which would be a design error resulting in huge performance problems)
    /// </summary>
    public TextDocument Document
    {
      get { return mDocument; }
      set
      {
        if (mDocument != value)
        {
          mDocument = value;
          RaisePropertyChanged("Document");
        }
      }
    }
    #endregion

    #region IsDirty
    private bool _isDirty = false;

    /// <summary>
    /// IsDirty indicates whether the file currently loaded
    /// in the editor was modified by the user or not.
    /// </summary>
    override public bool IsDirty
    {
      get
      {
        return _isDirty;
      }
      
      set
      {
        if (_isDirty != value)
        {
          _isDirty = value;

          this.NotifyPropertyChanged(() => this.IsDirty);
          this.NotifyPropertyChanged(() => this.Title);
        }
      }
    }
    #endregion

    #region CanSaveData
    /// <summary>
    /// Get whether edited data can be saved or not.
    /// This type of document does not have a save
    /// data implementation if this property returns false.
    /// (this is document specific and should always be overriden by descendents)
    /// </summary>
    override public bool CanSaveData
    {
      get
      {
        return true;
      }
    }
    #endregion CanSaveData

    #region AvalonEdit properties
    /// <summary>
    /// AvalonEdit exposes a Highlighting property that controls whether keywords,
    /// comments and other interesting text parts are colored or highlighted in any
    /// other visual way. This property exposes the highlighting information for the
    /// text file managed in this viewmodel class.
    /// </summary>
    public IHighlightingDefinition HighlightingDefinition
    {
      get
      {
        lock (lockThis)
        {
          return this.mHighlightingDefinition;
        }
      }

      set
      {
        lock (lockThis)
        {
          if (this.mHighlightingDefinition != value)
          {
            this.mHighlightingDefinition = value;

            this.NotifyPropertyChanged(() => this.HighlightingDefinition);
          }
        }
      }
    }

    /// <summary>
    /// Get/set whether word wrap is currently activated or not.
    /// </summary>
    public bool WordWrap
    {
      get
      {
        return this.mWordWrap;
      }

      set
      {
        if (this.mWordWrap != value)
        {
          this.mWordWrap = value;
          this.NotifyPropertyChanged(() => this.WordWrap);
        }
      }
    }
    
    /// <summary>
    /// Get/set whether line numbers are currently shown or not.
    /// </summary>
    public bool ShowLineNumbers
    {
      get
      {
        return this.mShowLineNumbers;
      }

      set
      {
        if (this.mShowLineNumbers != value)
        {
          this.mShowLineNumbers = value;
          this.NotifyPropertyChanged(() => this.ShowLineNumbers);
        }
      }
    }

    /// <summary>
    /// Get/set whether the end of each line is currently shown or not.
    /// </summary>
    public bool ShowEndOfLine               // Toggle state command
    {
      get
      {
        return this.TextOptions.ShowEndOfLine;
      }

      set
      {
        if (this.TextOptions.ShowEndOfLine != value)
        {
          this.TextOptions.ShowEndOfLine = value;
          this.NotifyPropertyChanged(() => this.ShowEndOfLine);
        }
      }
    }

    /// <summary>
    /// Get/set whether the spaces are highlighted or not.
    /// </summary>
    public bool ShowSpaces               // Toggle state command
    {
      get
      {
        return this.TextOptions.ShowSpaces;
      }

      set
      {
        if (this.TextOptions.ShowSpaces != value)
        {
          this.TextOptions.ShowSpaces = value;
          this.NotifyPropertyChanged(() => this.ShowSpaces);
        }
      }
    }

    /// <summary>
    /// Get/set whether the tabulator characters are highlighted or not.
    /// </summary>
    public bool ShowTabs               // Toggle state command
    {
      get
      {
        return this.TextOptions.ShowTabs;
      }

      set
      {
        if (this.TextOptions.ShowTabs != value)
        {
          this.TextOptions.ShowTabs = value;
          this.NotifyPropertyChanged(() => this.ShowTabs);
        }
      }
    }

    /// <summary>
    /// Get/Set texteditor options frmo <see cref="AvalonEdit"/> editor as <see cref="TextEditorOptions"/> instance.
    /// </summary>
    public ICSharpCode.AvalonEdit.TextEditorOptions TextOptions
    {
      get
      {
        return this.mTextOptions;
      }

      set
      {
        if (this.mTextOptions != value)
        {
          this.mTextOptions = value;
          this.NotifyPropertyChanged(() => this.TextOptions);
        }
      }
    }
    #endregion AvalonEdit properties

    #region LoadFile
    public static EdiViewModel LoadFile(string filePath)
    {
      bool IsFilePathReal = false;

      try 
	    {	        
        IsFilePathReal = File.Exists(filePath);
	    }
	    catch
	    {
	    }

      if (IsFilePathReal == false)
        return null;

      EdiViewModel vm = new EdiViewModel();

      if (vm.OpenFile(filePath) == true)
        return vm;

      return null;
    }

    /// <summary>
    /// Attempt to open a file and load it into the viewmodel if it exists.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>True if file exists and was succesfully loaded. Otherwise false.</returns>
    protected bool OpenFile(string filePath)
    {
      try
      {
        if ((this.IsFilePathReal = File.Exists(filePath)) == true)
        {
          this.FilePath = filePath;
          this.ContentId = this.mFilePath;
          IsDirty = false; // Mark document loaded from persistence as unedited copy (display without dirty mark '*' in name)

          // Check file attributes and set to read-only if file attributes indicate that
          if ((System.IO.File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0)
          {
            this.IsReadOnly = true;
            this.IsReadOnlyReason = Util.Local.Strings.STR_FILE_READONLY_REASON_NO_WRITE_PERMISSION;
          }

          try
          {
          	using (FileStream fs = new FileStream(this.mFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
              using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
              {
                this.mDocument = new TextDocument(reader.ReadToEnd());
                this.FileEncoding = reader.CurrentEncoding; // assign encoding after ReadToEnd() so that the StreamReader can autodetect the encoding
              }
            }
          }
          catch                 // File may be blocked by another process
          {                    // Try read-only shared method and set file access to read-only
            try
            {
              this.IsReadOnly = true;  // Open file in readonly mode
              this.IsReadOnlyReason = Util.Local.Strings.STR_FILE_READONLY_REASON_USED_BY_OTHER_PROCESS;

              using (FileStream fs = new FileStream(this.mFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
              {
                using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
                {
                  this.mDocument = new TextDocument(reader.ReadToEnd());
                  this.FileEncoding = reader.CurrentEncoding; // assign encoding after ReadToEnd() so that the StreamReader can autodetect the encoding
                }
              }
            }
            catch (Exception ex)
            {
              MsgBox.Msg.Show(ex.Message, Util.Local.Strings.STR_FILE_OPEN_ERROR_MSG_CAPTION, MsgBoxButtons.OK);

              return false;
            }
          }
        }
        else
          return false;
      }
      catch (Exception exp)
      {
        MsgBox.Msg.Show(exp.Message, Util.Local.Strings.STR_FILE_OPEN_ERROR_MSG_CAPTION, MsgBoxButtons.OK);

        return false;
      }

      return true;
    }
    #endregion LoadFile

    #region SaveCommand
    /// <summary>
    /// Save the document viewed in this viewmodel.
    /// </summary>
    override public bool CanSave()
    {
      return true;  // IsDirty
    }

    /// <summary>
    /// Write text content to disk and (re-)set associated properties
    /// </summary>
    /// <param name="filePath"></param>
    override public bool SaveFile(string filePath)
    {
      try
      {
        File.WriteAllText(filePath, this.Document.Text);

        this.IsFilePathReal = true;
        this.FilePath = filePath;
        this.ContentId = filePath;
        this.IsDirty = false;

        return true;
      }
      catch (Exception)
      {
        throw;
      }
    }
    #endregion

    #region SaveAsCommand
    override public bool CanSaveAs()
    {
      return true;  // IsDirty
    }
    #endregion

    #region CloseCommand
    RelayCommand<object> _closeCommand = null;

    /// <summary>
    /// This command cloases a single file. The binding for this is in the AvalonDock LayoutPanel Style.
    /// </summary>
    override public ICommand CloseCommand
    {
      get
      {
        if (_closeCommand == null)
        {
          _closeCommand = new RelayCommand<object>((p) => this.OnClose(), (p) => this.CanClose());
        }

        return _closeCommand;
      }
    }

    override public bool CanClose()
    {
      return true;
    }
    #endregion

    #region Encoding
    private Encoding mFileEncoding = Encoding.UTF8;
    /// <summary>
    /// Get/set file encoding of current text file.
    /// </summary>
    public Encoding FileEncoding
    {
      get
      {
        return this.mFileEncoding;
      }

      set
      {
        if (this.mFileEncoding != value)
        {
          this.mFileEncoding = value;
          this.NotifyPropertyChanged(() => this.mFileEncoding);
        }
      }
    }
    #endregion Encoding

    #region ScaleView
    /// <summary>
    /// Scale view of text in percentage of font size
    /// </summary>
    public UnitViewModel SizeUnitLabel { get; set; }
    #endregion ScaleView

    #region CaretPosition
    // These properties are used to display the current column/line
    // of the cursor in the user interface
    private int mLine = 0;
    public int Line
    {
      get
      {
        return this.mLine;
      }

      set
      {
        if (this.mLine != value)
        {
          this.mLine = value;
          this.NotifyPropertyChanged(() => this.Line);
        }
      }
    }

    private int mColumn = 0;
    public int Column
    {
      get
      {
        return this.mColumn;
      }

      set
      {
        if (this.mColumn != value)
        {
          this.mColumn = value;
          this.NotifyPropertyChanged(() => this.Column);
        }
      }
    }
    #endregion CaretPosition

    #region EditorStateProperties
    // These properties are used to save and restore the editor state when CTRL+TABing between documents
    private int mTextEditorCaretOffset = 0;
    private int mTextEditorSelectionStart = 0;
    private int mTextEditorSelectionLength = 0;
    private bool mTextEditorIsRectangularSelection = false;
    private double mTextEditorScrollOffsetX = 0;
    private double mTextEditorScrollOffsetY = 0;

    /// <summary>
    /// Get/set editor carret position
    /// for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
    /// </summary>
    public int TextEditorCaretOffset
    {
      get
      {
        return this.mTextEditorCaretOffset;
      }

      set
      {
        if (this.mTextEditorCaretOffset != value)
        {
          this.mTextEditorCaretOffset = value;
          this.NotifyPropertyChanged(() => this.TextEditorCaretOffset);
        }
      }
    }

    /// <summary>
    /// Get/set editor start of selection
    /// for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
    /// </summary>
    public int TextEditorSelectionStart
    {
      get
      {
        return this.mTextEditorSelectionStart;
      }

      set
      {
        if (this.mTextEditorSelectionStart != value)
        {
          this.mTextEditorSelectionStart = value;
          this.NotifyPropertyChanged(() => this.TextEditorSelectionStart);
        }
      }
    }

    /// <summary>
    /// Get/set editor length of selection
    /// for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
    /// </summary>
    public int TextEditorSelectionLength
    {
      get
      {
        return this.mTextEditorSelectionLength;
      }

      set
      {
        if (this.mTextEditorSelectionLength != value)
        {
          this.mTextEditorSelectionLength = value;
          this.NotifyPropertyChanged(() => this.TextEditorSelectionLength);
        }
      }
    }

    public bool TextEditorIsRectangularSelection
    {
      get
      {
        return this.mTextEditorIsRectangularSelection;
      }

      set
      {
        if (this.mTextEditorIsRectangularSelection != value)
        {
          this.mTextEditorIsRectangularSelection = value;
          this.NotifyPropertyChanged(() => this.TextEditorIsRectangularSelection);
        }
      }
    }

    #region EditorScrollOffsetXY
    /// <summary>
    /// Current editor view scroll X position
    /// </summary>
    public double TextEditorScrollOffsetX
    {
      get
      {
        return this.mTextEditorScrollOffsetX;
      }

      set
      {
        if (this.mTextEditorScrollOffsetX != value)
        {
          this.mTextEditorScrollOffsetX = value;
          this.NotifyPropertyChanged(() => this.TextEditorScrollOffsetX);
        }
      }
    }

    /// <summary>
    /// Current editor view scroll Y position
    /// </summary>
    public double TextEditorScrollOffsetY
    {
      get
      {
        return this.mTextEditorScrollOffsetY;
      }

      set
      {
        if (this.mTextEditorScrollOffsetY != value)
        {
          this.mTextEditorScrollOffsetY = value;
          this.NotifyPropertyChanged(() => this.TextEditorScrollOffsetY);
        }
      }
    }
    #endregion EditorScrollOffsetXY
    #endregion EditorStateProperties

    #region TxtControl
    private TextBoxController mTxtControl = null;
    public TextBoxController TxtControl
    {
      get
      {
        return this.mTxtControl;
      }

      private set
      {
        if (this.mTxtControl != value)
        {
          this.mTxtControl = value;
          this.NotifyPropertyChanged(() => this.TxtControl);
        }
      }
    }
    #endregion TxtControl

    #region IEditorInterface
    public string Text
    {
      get
      {
        if (this.Document == null)
          return string.Empty;

        return this.Document.Text;
      }
    }

    public int SelectionStart
    {
      get
      {
        int start = 0, length = 0;
        bool IsRectSelect = false;

        if (this.TxtControl != null)
          this.TxtControl.CurrentSelection(out start, out length, out IsRectSelect);

        return start;
      }
    }

    public int SelectionLength
    {
      get
      {
        int start=0, length=0;
        bool IsRectSelect = false;

        if (this.TxtControl != null)
          this.TxtControl.CurrentSelection(out start, out length, out IsRectSelect);

        return length;
      }
    }

    /// <summary>
    /// Selects the specified portion of Text and scrolls that part into view.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="length"></param>
    public void Select(int start, int length)
    {
      if (this.TxtControl != null)
        this.TxtControl.SelectText(start, length);
    }

    public void Replace(int start, int length, string ReplaceWith)
    {
      if (this.Document != null)
        this.Document.Replace(start, length, ReplaceWith);
    }

    /// <summary>
    /// This method is called before a replace all operation.
    /// </summary>
    public void BeginChange()
    {
      if (this.TxtControl != null)
        this.TxtControl.BeginChange();
    }

    /// <summary>
    /// This method is called after a replace all operation.
    /// </summary>
    public void EndChange()
    {
      if (this.TxtControl != null)
        this.TxtControl.EndChange();
    }
    #endregion IEditorInterface
    #endregion properties

    #region methods
    /// <summary>
    /// Get the path of the file or empty string if file does not exists on disk.
    /// </summary>
    /// <returns></returns>
    override public string GetFilePath()
    {
      try
      {
        if (System.IO.File.Exists(this.FilePath))
          return System.IO.Path.GetDirectoryName(this.FilePath);
      }
      catch
      {
      }

      return string.Empty;
    }

    /// <summary>
    /// Switch off text highlighting to display the current document in regular
    /// black and white or white and black foreground/background colors.
    /// </summary>
    public void DisableHighlighting()
    {
      this.HighlightingDefinition = null;
    }

    #region ScaleView methods
    /// <summary>
    /// Initialize scale view of content to indicated value and unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="defaultValue"></param>
    public void InitScaleView(ZoomUnit unit, double defaultValue)
    {
      var unitList = new ObservableCollection<ListItem>(Options.GenerateScreenUnitList());

      this.SizeUnitLabel = new UnitViewModel(unitList, new ScreenConverter(), (int)unit, defaultValue);
    }
    #endregion ScaleView methods

    #endregion methods
  }
}
