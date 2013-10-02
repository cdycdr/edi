namespace Settings
{
  using System;
  using System.IO;
  using System.Xml;
  using System.Xml.Serialization;
  using Settings.ProgramSettings;
  using Settings.UserProfile;

  /// <summary>
  /// This class keeps track of program options and user profile (session) data.
  /// Both data items can be added and are loaded on application start to restore
  /// the program state of the last user session or to implement the default
  /// application state when starting the application for the very first time.
  /// </summary>
  public class SettingsManager
  {
    #region fields
    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private Options mSettingData = null;
    private Profile mSessionData = null;
    #endregion fields

    #region constructor
    public SettingsManager()
    {
      this.SettingData = new Options();
      this.SessionData = new Profile();
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Gets the default <seealso cref="ThemesManager"/> instance.
    /// The default <seealso cref="ThemesManager"/> comes with the built-in WPF andHighlighting themes.
    /// </summary>
    public static SettingsManager Instance
    {
      get
      {
        return DefaultSettingsManager.Instance;
      }
    }

    public Options SettingData
    {
      get
      {
        if (this.mSettingData == null)
          this.mSettingData = new Options();

        return this.mSettingData;
      }
      
      private set
      {
        if (this.mSettingData != value)
          this.mSettingData = value;
      }
    }

    public Profile SessionData
    {
      get
      {
        if (this.mSessionData == null)
          this.mSessionData = new Profile();

        return this.mSessionData;
      }
      
      private set
      {
        if (this.mSessionData != value)
          this.mSessionData = value;
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Determine whether program options are valid and corrext
    /// settings if they appear to be invalid on current system
    /// </summary>
    public void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft,
                                    double SystemParameters_VirtualScreenTop)
    {
      //// Dirkster: Not sure whether this is working correctly yet...
      //// this.SessionData.CheckSettingsOnLoad(SystemParameters_VirtualScreenLeft,
      ////                                      SystemParameters_VirtualScreenTop);
    }

    #region Load Save ProgramOptions
    /// <summary>
    /// Save program options into persistence.
    /// See <seealso cref="SaveOptions"/> to save program options on program end.
    /// </summary>
    /// <param name="settingsFileName"></param>
    /// <returns></returns>
    public void LoadOptions(string settingsFileName)
    {
      Options loadedModel = null;

      try
      {
        if (System.IO.File.Exists(settingsFileName))
        {
          // Create a new file stream for reading the XML file
          using (FileStream readFileStream = new System.IO.FileStream(settingsFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            try
            {
              // Create a new XmlSerializer instance with the type of the test class
              XmlSerializer serializerObj = new XmlSerializer(typeof(Options));

              // Load the object saved above by using the Deserialize function
              loadedModel = (Options)serializerObj.Deserialize(readFileStream);
            }
            catch (Exception e)
            {
              logger.Error(e);
            }

            // Cleanup
            readFileStream.Close();
          }
        }
      }
      catch (Exception exp)
      {
        logger.Error(exp);
      }
      finally
      {
        if (loadedModel == null)
          loadedModel = new Options();  // Just get the defaults if serilization wasn't working here...
      }

      this.SettingData = loadedModel;
    }

    /// <summary>
    /// Save program options into persistence.
    /// See <seealso cref="LoadOptions"/> to load program options on program start.
    /// </summary>
    /// <param name="settingsFileName"></param>
    /// <param name="vm"></param>
    /// <returns></returns>
    public bool SaveOptions(string settingsFileName, Options vm)
    {
      try
      {
        XmlWriterSettings xws = new XmlWriterSettings();
        xws.NewLineOnAttributes = true;
        xws.Indent = true;
        xws.IndentChars = "  ";
        xws.Encoding = System.Text.Encoding.UTF8;

        // Create a new file stream to write the serialized object to a file
        using (XmlWriter xw = XmlWriter.Create(settingsFileName, xws))
        {
          // Create a new XmlSerializer instance with the type of the test class
          XmlSerializer serializerObj = new XmlSerializer(typeof(Options));

          serializerObj.Serialize(xw, vm);

          xw.Close(); // Cleanup

          return true;
        }
      }
      catch
      {
        throw;
      }
    }
    #endregion Load Save ProgramOptions

    #region Load Save UserSessionData
    /// <summary>
    /// Save program options into persistence.
    /// See <seealso cref="SaveOptions"/> to save program options on program end.
    /// </summary>
    /// <param name="sessionDataFileName"></param>
    /// <returns></returns>
    public void LoadSessionData(string sessionDataFileName)
    {
      Profile profileDataModel = null;

      try
      {
        if (System.IO.File.Exists(sessionDataFileName))
        {
          // Create a new file stream for reading the XML file
          using (FileStream readFileStream = new System.IO.FileStream(sessionDataFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            try
            {
              // Create a new XmlSerializer instance with the type of the test class
              XmlSerializer serializerObj = new XmlSerializer(typeof(Profile));

              // Load the object saved above by using the Deserialize function
              profileDataModel = (Profile)serializerObj.Deserialize(readFileStream);
            }
            catch (Exception e)
            {
              logger.Error(e);
            }

            // Cleanup
            readFileStream.Close();
          }
        }

        this.SessionData = profileDataModel;
      }
      catch (Exception exp)
      {
        logger.Error(exp);
      }
      finally
      {
        if (profileDataModel == null)
          profileDataModel = new Profile();  // Just get the defaults if serilization wasn't working here...
      }
    }

    /// <summary>
    /// Save program options into persistence.
    /// See <seealso cref="LoadOptions"/> to load program options on program start.
    /// </summary>
    /// <param name="sessionDataFileName"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool SaveSessionData(string sessionDataFileName, Profile model)
    {
      try
      {
        XmlWriterSettings xws = new XmlWriterSettings();
        xws.NewLineOnAttributes = true;
        xws.Indent = true;
        xws.IndentChars = "  ";
        xws.Encoding = System.Text.Encoding.UTF8;

        // Create a new file stream to write the serialized object to a file
        using (XmlWriter xw = XmlWriter.Create(sessionDataFileName, xws))
        {
          // Create a new XmlSerializer instance with the type of the test class
          XmlSerializer serializerObj = new XmlSerializer(typeof(Profile));

          serializerObj.Serialize(xw, model);

          xw.Close(); // Cleanup

          return true;
        }
      }
      catch
      {
        throw;
      }
    }
    #endregion Load Save UserSessionData
    #endregion methods

    #region internal classes
    /// <summary>
    /// Internal implementation class of the static DefaultSettingsManager.Instance property.
    /// </summary>
    internal sealed class DefaultSettingsManager : SettingsManager
    {
      public new static readonly DefaultSettingsManager Instance = new DefaultSettingsManager();
    }
    #endregion internal classes
  }
}
