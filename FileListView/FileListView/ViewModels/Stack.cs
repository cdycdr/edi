namespace FileListView.ViewModels
{
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// Class implements a stack whos items can be observed via data binding.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Stack<T> : List<T>
  {
    /// <summary>
    /// Add a new element on top of the stack.
    /// </summary>
    /// <param name="item"></param>
    public virtual void Push(T item)
    {
      this.Add(item);
    }

    /// <summary>
    /// Remove the current element from the stacks top position and return it.
    /// </summary>
    /// <returns>The element that was pooped or null.</returns>
    public virtual T Pop()
    {
      T item = this.Last();
      this.RemoveAt(this.Count - 1);

      return item;
    }
  }
}
