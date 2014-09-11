namespace Edi
{
  using System.ComponentModel.Composition.Hosting;
  using System.Windows;
  using Edi.ViewModel;
  using Microsoft.Practices.Prism.MefExtensions;
  using Microsoft.Practices.Prism.Modularity;

  public class Bootstapper : MefBootstrapper
  {
    #region fields
    private ApplicationViewModel appVM = null;
    private MainWindow mMainWindow = null;
    #endregion fields

    #region Methods
    protected override void ConfigureAggregateCatalog()
    {
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ApplicationViewModel).Assembly));
      this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstapper).Assembly));
    }

    protected override DependencyObject CreateShell()
    {
			this.appVM = this.Container.GetExportedValue<ApplicationViewModel>();

      this.mMainWindow = this.Container.GetExportedValue<MainWindow>();

      return this.mMainWindow;
    }

    protected override void InitializeShell()
    {
      base.InitializeShell();

      Application.Current.MainWindow = (MainWindow)this.Shell;
      Application.Current.MainWindow.Show();
    }

    protected override Microsoft.Practices.Prism.Regions.IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
    {
      var factory = base.ConfigureDefaultRegionBehaviors();
      return factory;
    }

    /// <summary>
    /// Creates the <see cref="IModuleCatalog"/> used by Prism.
    /// 
    /// The base implementation returns a new ModuleCatalog.
    /// </summary>
    /// <returns>
    /// A ConfigurationModuleCatalog.
    /// </returns>
    protected override IModuleCatalog CreateModuleCatalog()
    {
      // When using MEF, the existing Prism ModuleCatalog is still the place to configure modules via configuration files.
      return new ConfigurationModuleCatalog();
    }
    #endregion Methods
  }
}
