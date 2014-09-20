namespace EdiViews.View.Behavior
{
	using System;
	using System.Reflection;
	using System.Windows;
	using System.Windows.Input;
	using System.Windows.Interactivity;

	/// <summary>
	/// Behavior that will connect an UI event to a viewmodel Command,
	/// allowing the event arguments to be passed as the CommandParameter.
	/// </summary>
	public class EventToCommandBehavior : Behavior<FrameworkElement>
	{
		public static readonly DependencyProperty EventProperty = DependencyProperty.Register("Event", typeof(string), typeof(EventToCommandBehavior), new PropertyMetadata(null, OnEventChanged));

		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(EventToCommandBehavior), new PropertyMetadata(null));
		public static readonly DependencyProperty CommandParameterProperty =
				DependencyProperty.Register("CommandParameter",
													typeof(object),
													typeof(EventToCommandBehavior), new PropertyMetadata(null));

		public static readonly DependencyProperty PassArgumentsProperty = DependencyProperty.Register("PassArguments", typeof(bool), typeof(EventToCommandBehavior), new PropertyMetadata(false));

		private Delegate _handler;
		private EventInfo _oldEvent;

		/// <summary>
		/// Gets a string
		/// </summary>
		public string Event
		{
			get
			{
				return (string)GetValue(EventProperty);
			}

			set
			{
				this.SetValue(EventProperty, value);
			}
		}

		/// <summary>
		/// Gets an ICommand
		/// </summary>
		public ICommand Command
		{
			get
			{
				return (ICommand)GetValue(CommandProperty);
			}
			set
			{
				this.SetValue(CommandProperty, value);
			}
		}

		public object CommandParameter
		{
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		/// <summary>
		/// Gets whether to pass arguments (default: false)
		/// </summary>
		public bool PassArguments
		{
			get
			{
				return (bool)GetValue(PassArgumentsProperty);
			}

			set
			{
				this.SetValue(PassArgumentsProperty, value);
			}
		}

		protected override void OnAttached()
		{
			this.AttachHandler(this.Event); // initial set
		}

		private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var beh = (EventToCommandBehavior)d;

			// is not yet attached at initial load
			if (beh.AssociatedObject != null)
				beh.AttachHandler((string)e.NewValue);
		}

		/// <summary>
		/// Attaches the handler to the event
		/// </summary>
		/// <param name="eventName"></param>
		private void AttachHandler(string eventName)
		{
			// detach old event
			if (this._oldEvent != null)
				this._oldEvent.RemoveEventHandler(this.AssociatedObject, this._handler);

			// attach new event
			if (!string.IsNullOrEmpty(eventName))
			{
				EventInfo ei = this.AssociatedObject.GetType().GetEvent(eventName);
				if (ei != null)
				{
					MethodInfo mi = this.GetType().GetMethod("ExecuteCommand", BindingFlags.Instance | BindingFlags.NonPublic);
					this._handler = Delegate.CreateDelegate(ei.EventHandlerType, this, mi);
					ei.AddEventHandler(this.AssociatedObject, this._handler);
					this._oldEvent = ei; // store to detach in case the Event property changes
				}
				else
					throw new ArgumentException(string.Format("The event '{0}' was not found on type '{1}'", eventName, this.AssociatedObject.GetType().Name));
			}
		}

		/// <summary>
		/// Executes the Command
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExecuteCommand(object sender, EventArgs e)
		{
			object parameter = this.PassArguments ? e : this.CommandParameter;
			if (this.Command != null)
			{
				if (this.Command.CanExecute(parameter))
					this.Command.Execute(parameter);
			}
		}
	}
}
