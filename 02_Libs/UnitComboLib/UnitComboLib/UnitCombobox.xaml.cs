namespace UnitComboLib
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// This class implements a lookless combobox control with
  /// a unit drop-down selection that can be used to pre-select
  /// a list of useful combobox entries.
  /// </summary>
  public class UnitCombobox : ComboBox
  {
    #region constructor
    /// <summary>
    /// Static class constructor to register look-less <seealso cref="UnitCombobox"/> class
    /// control with the dependency property system.
    /// </summary>
    static UnitCombobox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(UnitCombobox), new FrameworkPropertyMetadata(typeof(UnitCombobox)));
    }

    /// <summary>
    /// Standard public class constructor.
    /// </summary>
    public UnitCombobox() : base()
    {
    }
    #endregion constructor
  }
}
