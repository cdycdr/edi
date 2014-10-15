namespace Edi.Core.View.Pane
{
	using System;
	using System.ComponentModel.Composition;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using Edi.Core.Interfaces.Enums;
	using Edi.Core.ViewModels;
	using Xceed.Wpf.AvalonDock.Layout;

	/// <summary>
	/// Initialize the AvalonDock Layout. Methods in this class
	/// are called before and after the layout is changed.
	/// 
	/// See Source:
	/// https://github.com/tgjones/gemini/blob/master/src/Gemini/Modules/Shell/Controls/LayoutInitializer.cs
	/// </summary>
	public class LayoutInitializer : ILayoutUpdateStrategy
	{
		public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
		{
			var tool = anchorableToShow.Content as IToolWindow;
			if (tool != null)
			{
				var preferredLocation = tool.PreferredLocation;
				string paneName = GetPaneName(preferredLocation);

				var toolsPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == paneName);

				if (toolsPane == null)
				{
					switch (preferredLocation)
					{
						case PaneLocation.Left:
							toolsPane = CreateAnchorablePane(layout, Orientation.Horizontal, paneName, InsertPosition.Start);
							break;

						case PaneLocation.Right:
							toolsPane = CreateAnchorablePane(layout, Orientation.Horizontal, paneName, InsertPosition.End);
							break;

						case PaneLocation.Bottom:
							toolsPane = CreateAnchorablePane(layout, Orientation.Vertical, paneName, InsertPosition.End);
							break;

						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				toolsPane.Children.Add(anchorableToShow);
				return true;
			}

			return false;
		}

		private static string GetPaneName(PaneLocation location)
		{
			switch (location)
			{
				case PaneLocation.Left:
					return "LeftPane";

				case PaneLocation.Right:
					return "RightPane";

				case PaneLocation.Bottom:
					return "BottomPane";

				default:
					throw new ArgumentOutOfRangeException("location");
			}
		}

		private static LayoutAnchorablePane CreateAnchorablePane(LayoutRoot layout,
																														 Orientation orientation,
																														 string paneName,
																														 InsertPosition position)
		{
			try
			{
				LayoutPanel parent = null;

				try
				{
					parent = layout.Descendents().OfType<LayoutPanel>().First(d => d.Orientation == orientation);
				}
				catch (Exception)
				{
					parent = layout.GetRoot().RootPanel;
				}

				var toolsPane = new LayoutAnchorablePane { Name = paneName };

				if (position == InsertPosition.Start)
					parent.InsertChildAt(0, toolsPane);
				else
					parent.Children.Add(toolsPane);

				return toolsPane;
			}
			catch (Exception exp)
			{
				throw new Exception(string.Format("Failed to initialize layout for: {0}, {1}, {2}",
																														orientation, paneName, position), exp);
			}
		}

		private enum InsertPosition
		{
			Start,
			End
		}

		public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
		{
			// If this is the first anchorable added to this pane, then use the preferred size.
			var tool = anchorableShown.Content as IToolWindow;
			if (tool != null)
			{
				var anchorablePane = anchorableShown.Parent as LayoutAnchorablePane;
				if (anchorablePane != null && anchorablePane.ChildrenCount == 1)
				{
					switch (tool.PreferredLocation)
					{
						case PaneLocation.Left:
						case PaneLocation.Right:
							anchorablePane.DockWidth = new GridLength(tool.PreferredWidth, GridUnitType.Pixel);
							break;
						case PaneLocation.Bottom:
							anchorablePane.DockHeight = new GridLength(tool.PreferredHeight, GridUnitType.Pixel);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
		}

		public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
		{
			return false;
		}

		public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
		{
		}
	}
}
