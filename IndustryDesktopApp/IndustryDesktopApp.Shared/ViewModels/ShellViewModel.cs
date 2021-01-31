using IndustryDesktopApp.Core.Mvvm;
using Prism.Commands;
using Prism.Mvvm;
using System;

namespace IndustryDesktopApp.Shared.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            Title = "Main Page";
        }
    }
}
