using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
namespace MiniUML.View.Windows
{
  using MiniUML.Framework;
  using MiniUML.Model.ViewModels;
  using MiniUML.View.Utilities;

  /// <summary>
  /// Interaction logic for NewDocumentWindow.xaml
  /// </summary>
  public partial class NewDocumentWindow : Window
  {
    public NewDocumentWindow()
    {
      InitializeComponent();
      Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
      Title = (string)Application.Current.Resources["ApplicationName"];
    }

    private bool getValues(out Size pageSize, out Thickness pageMargins)
    {
      pageSize = new Size(); pageMargins = new Thickness();

      DiuToCentimetersConverter converter = new DiuToCentimetersConverter();

      try
      {
        double pageWidth = (double)converter.ConvertBack(_pageWidthTextBox.Text, typeof(bool), null, CultureInfo.CurrentCulture);
        double pageHeight = (double)converter.ConvertBack(_pageHeightTextBox.Text, typeof(bool), null, CultureInfo.CurrentCulture);
        double pageMarginTop = (double)converter.ConvertBack(_pageMarginTopTextBox.Text, typeof(bool), null, CultureInfo.CurrentCulture);
        double pageMarginBottom = (double)converter.ConvertBack(_pageMarginBottomTextBox.Text, typeof(bool), null, CultureInfo.CurrentCulture);
        double pageMarginLeft = (double)converter.ConvertBack(_pageMarginLeftTextBox.Text, typeof(bool), null, CultureInfo.CurrentCulture);
        double pageMarginRight = (double)converter.ConvertBack(_pageMarginRightTextBox.Text, typeof(bool), null, CultureInfo.CurrentCulture);

        if (pageWidth < 0 || pageHeight < 0)
        {
          MsgBox.Msg.Show(string.Format(MiniUML.Framework.Local.Strings.STR_MSG_PAGE_HEIGHT_WIDTH_NEGATIVE, pageWidth, pageHeight),
                          MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                          MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Warning);

          return false;
        }

        if (pageMarginTop < 0 || pageMarginRight < 0 || pageMarginLeft < 0 || pageMarginBottom < 0)
        {
          MsgBox.Msg.Show(string.Format(MiniUML.Framework.Local.Strings.STR_MSG_PAGE_MARGINS_NEGATIVE),
                          MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                          MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Warning);

          return false;
        }

        if (pageMarginTop + pageMarginBottom > pageHeight || pageMarginLeft + pageMarginRight > pageWidth)
        {
          MsgBox.Msg.Show(string.Format(MiniUML.Framework.Local.Strings.STR_MSG_PAGE_MARGIN_LARGER_THAN_PAGESIZE),
                          MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                          MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Warning);

          return false;
        }

        pageSize = new Size(pageWidth, pageHeight);
        pageMargins = new Thickness(pageMarginLeft, pageMarginTop, pageMarginRight, pageMarginBottom);
        return true;
      }
      catch (FormatException)
      {
        MsgBox.Msg.Show(string.Format(MiniUML.Framework.Local.Strings.STR_MSG_PAGE_DEFINITION_FIELD_INVALID),
                        MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                        MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Warning);
      }
      catch (OverflowException)
      {
        MsgBox.Msg.Show(string.Format(MiniUML.Framework.Local.Strings.STR_MSG_PAGE_DEFINITION_FIELD_INVALID),
                        MiniUML.Framework.Local.Strings.STR_MSG_Warning_Caption,
                        MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Warning);
      }
      return false;
    }

    private void okButton_Click(object sender, RoutedEventArgs e)
    {
      NewDocumentWindowViewModel viewModel = DataContext as NewDocumentWindowViewModel;

      Size pageSize;
      Thickness pageMargins;
      if (getValues(out pageSize, out pageMargins))
      {
        viewModel.prop_PageSize = pageSize;
        viewModel.prop_PageMargins = pageMargins;
        this.DialogResult = true;
        this.Close();
      }
    }

    private void setDefaultButton_Click(object sender, RoutedEventArgs e)
    {
      Size pageSize;
      Thickness pageMargins;
      if (getValues(out pageSize, out pageMargins))
      {
        // TODO XXX SettingsManager.Settings["DefaultPageSize"] = pageSize;
        // TODO XXX SettingsManager.Settings["DefaultPageMargins"] = pageMargins;
        // TODO XXX SettingsManager.SaveSettings();
      }
    }
  }

  public class NewDocumentWindowFactory : IFactory
  {
    public object CreateObject()
    {
      return new NewDocumentWindow();
    }
  }
}
