using System;
using System.Windows.Input;
using System.Xml.Linq;
using MiniUML.Framework;
using MiniUML.Framework.Command;

namespace MiniUML.Model.ViewModels
{
  public class XmlViewModel : ViewModel
  {
    #region fields
    private RelayCommand<object> mUpdateDesignerCommand = null;
    private bool _documentChanged;
    #endregion fields

    #region constructor
    public XmlViewModel(DocumentViewModel documentViewModel)
    {
      // Store a reference to the parent view model.
      _DocumentViewModel = documentViewModel;
    }
    #endregion constructor

    #region properties

    public bool prop_DocumentChanged
    {
      get { return _documentChanged; }
      set
      {
        _documentChanged = value;
        base.SendPropertyChanged("prop_DocumentChanged");
      }
    }

    public DocumentViewModel _DocumentViewModel { get; private set; }

    #region command
    /// <summary>
    /// Update the canvas with text input from XML (in XML editor).
    /// </summary>
    public ICommand UpdateDesignerCommand
    {
      get
      {
        if (this.mUpdateDesignerCommand == null)
          this.mUpdateDesignerCommand = new RelayCommand<object>((p) => this.OnUpdateDesignerCommand_Execute(p),
                                                                 (p) => this.OnUpdateDesignerCommand_CanExecute());

        return mUpdateDesignerCommand;
      }
    }
    #endregion command
    #endregion properties

    #region methods
    #region Update Canvas from XML command
    private bool OnUpdateDesignerCommand_CanExecute()
    {
      return (this._DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
              this._DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid) &&
              this.prop_DocumentChanged;
    }

    private void OnUpdateDesignerCommand_Execute(object p)
    {
      try
      {
        string xmlText = p as string;

        this._DocumentViewModel.dm_DocumentDataModel.DocumentRoot = XElement.Parse(xmlText);
      }
      catch (Exception)
      {
        this._DocumentViewModel.dm_DocumentDataModel.State = DataModel.ModelState.Invalid;
        this._DocumentViewModel.dm_DocumentDataModel.DocumentRoot = new XElement("InvalidDocument");
      }
    }
    #endregion Update Canvas from XML command
    #endregion methods
  }
}
