﻿namespace WPFNotifications.ViewModel
{
  using System.Windows.Input;
  using UserNotification.Events;
  using WPFNotifications.Commands;

  /// <summary>
  /// Implements a viewmodel that drives the logic for binding commands
  /// to the view and invoking events that trigger the next notification.
  /// </summary>
  public class TestViewModel : Base.ViewModelBase
  {
    #region fields
    private RelayCommand mNotification1 = null;
    private RelayCommand mNotification2 = null;
    private RelayCommand mNotification3 = null;
    private RelayCommand mNotification4 = null;
    #endregion fields

    #region events
    /// <summary>
    /// Expose an event that is triggered when the viewmodel tells its view:
    /// Here is another notification message please show it to the user.
    /// </summary>
    public event ShowNotificationEventHandler ShowNotificationMessage;
    #endregion events

    #region commands
    /// <summary>
    /// Expose a command binding to show a sample notification message #1.
    /// </summary>
    public ICommand Notification1
    {
      get
      {
        if (this.mNotification1 == null)
          this.mNotification1 = new RelayCommand(() => Notification1_Executed());

        return this.mNotification1;
      }
    }

    /// <summary>
    /// Expose a command binding to show a sample notification message #2.
    /// </summary>
    public ICommand Notification2
    {
      get
      {
        if (this.mNotification2 == null)
          this.mNotification2 = new RelayCommand(() => Notification2_Executed());

        return this.mNotification2;
      }
    }

    /// <summary>
    /// Expose a command binding to show a sample notification message #3.
    /// </summary>
    public ICommand Notification3
    {
      get
      {
        if (this.mNotification3 == null)
          this.mNotification3 = new RelayCommand(() => Notification3_Executed());

        return this.mNotification3;
      }
    }

    /// <summary>
    /// Expose a command binding to show a sample notification message #4.
    /// </summary>
    public ICommand Notification4
    {
      get
      {
        if (this.mNotification4 == null)
          this.mNotification4 = new RelayCommand(() => Notification4_Executed());

        return this.mNotification4;
      }
    }
    #endregion commands

    #region methods
    private void Notification1_Executed()
    {
      // Invoke another notification event to tell the view: Lets display another notification.
      if (this.ShowNotificationMessage != null)
      {
        this.ShowNotificationMessage(this, new ShowNotificationEvent
         (
          "Mesage #1",
          "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
          "pack://application:,,,/Resources/notification-icon.png"
         ));
      }
    }

    private void Notification2_Executed()
    {
      // Invoke another notification event to tell the view: Lets display another notification.
      if (this.ShowNotificationMessage != null)
      {
        this.ShowNotificationMessage(this, new ShowNotificationEvent
         (
          "Mesage #2",
          "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
          "pack://application:,,,/Resources/microsoft-windows-8-logo.png"
         ));
      }
    }

    private void Notification3_Executed()
    {
      // Invoke another notification event to tell the view: Lets display another notification.
      if (this.ShowNotificationMessage != null)
      {
        this.ShowNotificationMessage(this, new ShowNotificationEvent
         (
          "Mesage #3",
          "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
          "pack://application:,,,/Resources/facebook-button.png"
         ));
      }
    }

    private void Notification4_Executed()
    {
      // Invoke another notification event to tell the view: Lets display another notification.
      if (this.ShowNotificationMessage != null)
      {
        this.ShowNotificationMessage(this, new ShowNotificationEvent
         (
          "Mesage #4",
          "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
          "pack://application:,,,/Resources/Radiation_warning_symbol.png"
         ));
      }
    }
    #endregion methods
  }
}
