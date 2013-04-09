﻿//Copyright (c) 2007-2012, Adolfo Marinucci
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the 
//following conditions are met:

//* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

//* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following 
//disclaimer in the documentation and/or other materials provided with the distribution.

//* Neither the name of Adolfo Marinucci nor the names of its contributors may be used to endorse or promote products
//derived from this software without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
//IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
//EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
//STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
//EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows;
using System.Diagnostics;
using AvalonDock.Layout;
using System.Windows.Media;
using System.Windows.Threading;

namespace AvalonDock.Controls
{
    internal static class FocusElementManager
    {
      private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      
      #region Focus Management
        static List<DockingManager> _managers = new List<DockingManager>();
        internal static void SetupFocusManagement(DockingManager manager)
        {
            if (_managers.Count == 0)
            {
                //InputManager.Current.EnterMenuMode += new EventHandler(InputManager_EnterMenuMode);
                //InputManager.Current.LeaveMenuMode += new EventHandler(InputManager_LeaveMenuMode);
                _windowHandler = new WindowHookHandler();
                _windowHandler.FocusChanged += new EventHandler<FocusChangeEventArgs>(WindowFocusChanging);
                //_windowHandler.Activate += new EventHandler<WindowActivateEventArgs>(WindowActivating);
                _windowHandler.Attach();

                if (Application.Current != null)
                    Application.Current.Exit += new ExitEventHandler(Current_Exit);
            }

            manager.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(manager_PreviewGotKeyboardFocus);
            _managers.Add(manager);
        }

        internal static void FinalizeFocusManagement(DockingManager manager)
        {
            manager.PreviewGotKeyboardFocus -= new KeyboardFocusChangedEventHandler(manager_PreviewGotKeyboardFocus);
            _managers.Remove(manager);

            if (_managers.Count == 0)
            {
                //InputManager.Current.EnterMenuMode -= new EventHandler(InputManager_EnterMenuMode);
                //InputManager.Current.LeaveMenuMode -= new EventHandler(InputManager_LeaveMenuMode);
                if (_windowHandler != null)
                {
                    _windowHandler.FocusChanged -= new EventHandler<FocusChangeEventArgs>(WindowFocusChanging);
                    //_windowHandler.Activate -= new EventHandler<WindowActivateEventArgs>(WindowActivating);
                    _windowHandler.Detach();
                    _windowHandler = null;
                }
            }

        }

        private static void Current_Exit(object sender, ExitEventArgs e)
        {
            Application.Current.Exit -= new ExitEventHandler(Current_Exit);
            if (_windowHandler != null)
            {
                _windowHandler.FocusChanged -= new EventHandler<FocusChangeEventArgs>(WindowFocusChanging);
                //_windowHandler.Activate -= new EventHandler<WindowActivateEventArgs>(WindowActivating);
                _windowHandler.Detach();
                _windowHandler = null;
            }
        }

        static void manager_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
          logger.Info("-> manager_PreviewGotKeyboardFocus");

            var focusedElement = e.NewFocus as Visual;
            if (focusedElement != null &&
                !(focusedElement is LayoutAnchorableTabItem || focusedElement is LayoutDocumentTabItem) &&
                !(focusedElement is ICommandSource))//Avoid tracking focus for elements like this
            {
                var parentAnchorable = focusedElement.FindVisualAncestor<LayoutAnchorableControl>();
                if (parentAnchorable != null)
                {
                  logger.Info("-> parentAnchorable");
                  _modelFocusedElement[parentAnchorable.Model] = e.NewFocus;
                }
                else
                {
                  logger.Info("-> parentDocument");
                  var parentDocument = focusedElement.FindVisualAncestor<LayoutDocumentControl>();
                  if (parentDocument != null)
                  {
                      _modelFocusedElement[parentDocument.Model] = e.NewFocus;
                  }
                }
            }

            logger.Info("<- manager_PreviewGotKeyboardFocus");
        }

        static FullWeakDictionary<ILayoutElement, IInputElement> _modelFocusedElement = new FullWeakDictionary<ILayoutElement, IInputElement>();
        static WeakDictionary<ILayoutElement, IntPtr> _modelFocusedWindowHandle = new WeakDictionary<ILayoutElement, IntPtr>();

        /// <summary>
        /// Get the input element that was focused before user left the layout element
        /// </summary>
        /// <param name="model">Element to look for</param>
        /// <returns>Input element </returns>
        internal static IInputElement GetLastFocusedElement(ILayoutElement model)
        {
            IInputElement objectWithFocus;
            if (_modelFocusedElement.GetValue(model, out objectWithFocus))
                return objectWithFocus;

            return null;
        }


        /// <summary>
        /// Get the last window handle focused before user left the element passed as argument
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static IntPtr GetLastWindowHandle(ILayoutElement model)
        {
            IntPtr handleWithFocus;
            if (_modelFocusedWindowHandle.GetValue(model, out handleWithFocus))
                return handleWithFocus;

            return IntPtr.Zero;
        }
        static WeakReference _lastFocusedElement;

        /// <summary>
        /// Given a layout element tries to set the focus of the keyword where it was before user moved to another element
        /// </summary>
        /// <param name="model"></param>
        internal static void SetFocusOnLastElement(ILayoutElement model)
        {
          logger.InfoFormat("SetFocusOnLastElement(model={0})", (model == null ? "(null)" : model.ToString()));
          
          bool focused = false;
          IInputElement objectToFocus;
          if (_modelFocusedElement.GetValue(model, out objectToFocus))
          {
              focused = objectToFocus == Keyboard.Focus(objectToFocus);
              logger.InfoFormat("_modelFocusedElement.GetValue: (focused={0})", focused);
          }

          IntPtr handleToFocus;
          if (_modelFocusedWindowHandle.GetValue(model, out handleToFocus))
          {
            focused = IntPtr.Zero != Win32Helper.SetFocus(handleToFocus);
            logger.InfoFormat("_modelFocusedWindowHandle.GetValue: (focused={0})", focused);
          }

          logger.InfoFormat("SetFocusOnLastElement(focused={0}, model={1}, element={2})", focused, model, handleToFocus == IntPtr.Zero ? (objectToFocus == null ? "" : objectToFocus.ToString()) : handleToFocus.ToString());
           
          if (focused)
          {
              _lastFocusedElement = new WeakReference(model);
          }

        }

        static WindowHookHandler _windowHandler = null;

        static void WindowFocusChanging(object sender, FocusChangeEventArgs e)
        {
            foreach (var manager in _managers)
            {
                var hostContainingFocusedHandle = manager.FindLogicalChildren<HwndHost>().FirstOrDefault(hw => Win32Helper.IsChild(hw.Handle, e.GotFocusWinHandle));

                if (hostContainingFocusedHandle != null)
                {
                    var parentAnchorable = hostContainingFocusedHandle.FindVisualAncestor<LayoutAnchorableControl>();
                    if (parentAnchorable != null)
                    {
                        _modelFocusedWindowHandle[parentAnchorable.Model] = e.GotFocusWinHandle;
                        if (parentAnchorable.Model != null)
                            parentAnchorable.Model.IsActive = true;
                    }
                    else
                    {
                        var parentDocument = hostContainingFocusedHandle.FindVisualAncestor<LayoutDocumentControl>();
                        if (parentDocument != null)
                        {
                            _modelFocusedWindowHandle[parentDocument.Model] = e.GotFocusWinHandle;
                            if (parentDocument.Model != null)
                                parentDocument.Model.IsActive = true;
                        }
                    }
                }


            }
        }

        static DispatcherOperation _setFocusAsyncOperation;

        static void WindowActivating(object sender, WindowActivateEventArgs e)
        {
          logger.Info("WindowActivating");

            if (Keyboard.FocusedElement == null && 
                _lastFocusedElement != null && 
                _lastFocusedElement.IsAlive)
            {
                var elementToSetFocus = _lastFocusedElement.Target as ILayoutElement;
                if (elementToSetFocus != null)
                {
                    var manager = elementToSetFocus.Root.Manager;
                    if (manager == null)
                        return;

                    IntPtr parentHwnd;
                    if (!manager.GetParentWindowHandle(out parentHwnd))
                        return;

                    if (e.HwndActivating != parentHwnd)
                        return;

                    _setFocusAsyncOperation = Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            SetFocusOnLastElement(elementToSetFocus);
                        }
                        finally
                        {
                            _setFocusAsyncOperation = null;
                        }
                    }), DispatcherPriority.Background);
                }
            }
        }


        static WeakReference _lastFocusedElementBeforeEnterMenuMode = null;
        static void InputManager_EnterMenuMode(object sender, EventArgs e)
        {
            if (Keyboard.FocusedElement == null)
                return;

            var lastfocusDepObj = Keyboard.FocusedElement as DependencyObject;
            if (lastfocusDepObj.FindLogicalAncestor<DockingManager>() == null)
            {
                _lastFocusedElementBeforeEnterMenuMode = null;
                return;
            }

            _lastFocusedElementBeforeEnterMenuMode = new WeakReference(Keyboard.FocusedElement);
        }
        static void InputManager_LeaveMenuMode(object sender, EventArgs e)
        {
            if (_lastFocusedElementBeforeEnterMenuMode != null &&
                _lastFocusedElementBeforeEnterMenuMode.IsAlive)
            {
                var lastFocusedInputElement = _lastFocusedElementBeforeEnterMenuMode.GetValueOrDefault<UIElement>();
                if (lastFocusedInputElement != null)
                {
                    if (lastFocusedInputElement != Keyboard.Focus(lastFocusedInputElement))
                      logger.Info("Unable to activate the element");
                }
            }
        }

        #endregion

    }
}