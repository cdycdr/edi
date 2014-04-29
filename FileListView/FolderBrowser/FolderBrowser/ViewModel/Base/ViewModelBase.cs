﻿namespace FolderBrowser.ViewModel.Base
{
  using System;
  using System.ComponentModel;
  using System.Linq.Expressions;

  /// <summary>
  /// Base of Viewmodel classes implemented in this assembly.
  /// </summary>
  public class ViewModelBase : INotifyPropertyChanged
  {
    #region constructor
    /// <summary>
    /// Standard <seealso cref="ViewModelBase"/> class constructor
    /// </summary>
    public ViewModelBase()
    {
    }
    #endregion constructor

    public event PropertyChangedEventHandler PropertyChanged;

    #region methods
    protected virtual void RaisePropertyChanged(string propName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propName));
    }

    /// <summary>
    /// Tell bound controls (via WPF binding) to refresh their display.
    /// 
    /// Sample call: this.OnPropertyChanged(() => this.IsSelected);
    /// where 'this' is derived from <seealso cref="BaseViewModel"/>
    /// and IsSelected is a property.
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="property"></param>
    protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
    {
      if (propertyExpression.Body.NodeType == ExpressionType.MemberAccess)
      {
        var memberExpr = propertyExpression.Body as MemberExpression;
        string propertyName = memberExpr.Member.Name;
        this.RaisePropertyChanged(propertyName);
      }
    }
    #endregion methods
  }
}
