using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AutoCoder.Windows.Input
{
  public class MoreDelegateCommand : IMoreDelegateCommand
  {
    Action<object> execute;
    Func<object, bool> canExecute;

    // Event required by ICommand
    public event EventHandler CanExecuteChanged;
    public MoreDelegateCommand Group { get; set; }
    private List<IMoreDelegateCommand> _GroupMembers;
    public List<IMoreDelegateCommand> GroupMembers
    {
      get
      {
        if (_GroupMembers == null)
          _GroupMembers = new List<IMoreDelegateCommand>();
        return _GroupMembers;
      }
      set { _GroupMembers = value; }
    }

    // Two constructors
    public MoreDelegateCommand(Action<object> execute, Func<object, bool> canExecute, 
      MoreDelegateCommand group = null)
    {
      this.execute = execute;
      this.canExecute = canExecute;
      this.AddToGroup(group);
    }
    public MoreDelegateCommand(Action<object> execute, MoreDelegateCommand group = null)
    {
      this.execute = execute;
      this.canExecute = this.AlwaysCanExecute;
      this.AddToGroup(group);
    }

    // Methods required by ICommand
    public void Execute(object param)
    {
      execute(param);
    }
    public bool CanExecute(object param)
    {
      return canExecute(param);
    }

    // Method required by IDelegateCommand
    public void RaiseCanExecuteChanged()
    {
      if (CanExecuteChanged != null)
        CanExecuteChanged(this, EventArgs.Empty);
    }

    private void AddToGroup( IMoreDelegateCommand member )
    {
      this.GroupMembers.Add(member);
    }

    private void SetGroup( MoreDelegateCommand group)
    {
      this.Group = group;
      if ( this.Group != null)
      {
        this.Group.AddToGroup(this);
      }
    }

    // Default CanExecute method
    bool AlwaysCanExecute(object param)
    {
      return true;
    }
  }
}


