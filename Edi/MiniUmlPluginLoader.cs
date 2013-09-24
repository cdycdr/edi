namespace Edi
{
  using System;
  using System.IO;
  using System.Reflection;
  using System.Windows;
  using MiniUML.Framework;
  using MiniUML.Model;
  using MiniUML.Model.Model;
  using MiniUML.Model.ViewModels;
  using MiniUML.Model.ViewModels.Document;
  using MsgBox;

  public partial class App : Application
  {
    /// <summary>
    /// This class load MiniUML Plug-Ins at run-time from the specified folder.
    /// </summary>
    private static class MiniUmlPluginLoader
    {
      #region methods
      public static void LoadPlugins(string pluginDirectory, IMiniUMLDocument windowViewModel)
      {
        string[] assemblyFiles = { };

        try
        {
          try
          {
            // Get the names of all assembly files in the plugin directory.
            assemblyFiles = Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories);
          }
          catch (DirectoryNotFoundException ex)
          {
            // Plugin directory not was not found; create it.
            Directory.CreateDirectory(pluginDirectory);

            MsgBox.Msg.Show(ex, Util.Local.Strings.STR_MSG_DIRCREATED_NO_PLuginLoaded,
                                Util.Local.Strings.STR_MSG_DIRCREATED_NO_PLuginLoaded_Caption);

            return;
          }
        }
        catch (Exception ex)
        {
          MsgBox.Msg.Show(ex, Util.Local.Strings.STR_MSG_ACCESS_PLuginDir_Caption,
                              Util.Local.Strings.STR_MSG_ACCESS_PLuginDir_Caption);

          return;
        }

        // Try to load plugins from each assembly.
        foreach (string assemblyFile in assemblyFiles)
          loadPluginAssembly(assemblyFile, windowViewModel);

        if (PluginManager.PluginModels.Count != assemblyFiles.Length)
          MsgBox.Msg.Show(Util.Local.Strings.STR_MSG_UML_PLugin_NOTALL_Loaded,
                          Util.Local.Strings.STR_MSG_UML_PLugin_NOTALL_Loaded_Caption,
                          MsgBoxButtons.OK, MsgBoxImage.Error);
      }

      private static void loadPluginAssembly(string assemblyFile, IMiniUMLDocument windowViewModel)
      {
        Assembly assembly;

        try
        {
          // Load the plugin assembly.
          assembly = Assembly.LoadFrom(assemblyFile);

          // Add an instance of each PluginModel found in the assembly to the plugin collection 
          // and merge its resources into the plugin resource dictionary.
          foreach (Type type in assembly.GetTypes())
          {
            if (!type.IsAbstract && typeof(PluginModelBase).IsAssignableFrom(type))
            {
              try
              {
                // Create PluginModel instance.
                PluginModelBase pluginModel = Activator.CreateInstance(type, windowViewModel) as PluginModelBase;

                // Plugin names must be unique
                foreach (PluginModelBase p in PluginManager.PluginModels)
                {
                  if (p.Name == pluginModel.Name)
                    throw new Exception(Util.Local.Strings.STR_MSG_UML_PLugin_Duplicate);
                }

                // Get the shared resources from the plugin.
                ResourceDictionary sharedResources = pluginModel.Resources;

                // If we got any resources, merge them into our plugin resource dictionary.
                if (sharedResources != null)
                  PluginManager.PluginResources.MergedDictionaries.Add(sharedResources);

                // Add the plugin into the collection of plugins
                PluginManager.AddPluginModel(pluginModel);

              }
              catch (Exception ex)
              {
                MsgBox.Msg.Show(ex, string.Format(Util.Local.Strings.STR_MSG_ErrorLoadingPlugin, assemblyFile),
                                MsgBoxButtons.OK, MsgBoxImage.Error);

                ////ExceptionManager.Register(ex,
                ////    "Plugin not loaded.",
                ////    "An error occured while initializing a plugin found in assembly " + assemblyFile + ".");
              }
            }
          }
        }
        catch (Exception ex)
        {
          MsgBox.Msg.Show(ex, Util.Local.Strings.STR_MSG_PluginNotLoaded,
                          string.Format(Util.Local.Strings.STR_MSG_ErrorWhileLoadingPlugin, assemblyFile));

          return;
        }
      }
      #endregion methods
    }
  }
}
