namespace MiniUML.View.Windows
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Markup;

  using MiniUML.Framework;
  using MiniUML.Model.ViewModels;
  using MsgBox;

  /// <summary>
  /// Interaction logic for ExportDocumentWindow.xaml
  /// </summary>
  public partial class ExportDocumentWindow : Window
  {
    public ExportDocumentWindow()
    {
      InitializeComponent();
      Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
      Title = (string)Application.Current.Resources["ApplicationName"];
    }

    private void okButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        double resolution = Double.Parse(_dpiTextBox.Text,
            NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint,
            CultureInfo.CurrentCulture);

        if (resolution <= 0)
        {
          Msg.Show("Invalid resolution.", "Warning", MsgBoxButtons.OK, MsgBoxImage.Warning);
          return;
        }

        ExportDocumentWindowViewModel viewModel = DataContext as ExportDocumentWindowViewModel;
        viewModel.prop_Resolution = resolution;

        this.DialogResult = true;
        this.Close();
      }
      catch (SystemException)
      {
        Msg.Show("One or more page definition fields are not valid.", "Warning", MsgBoxButtons.OK, MsgBoxImage.Warning);
      }
    }
  }

  public class ExportDocumentWindowFactory : IFactory
  {
    public object CreateObject()
    {
      return new ExportDocumentWindow();
    }
  }
}
