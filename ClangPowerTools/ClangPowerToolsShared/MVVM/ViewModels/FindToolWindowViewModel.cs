﻿using ClangPowerTools;
using ClangPowerTools.Views;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using ClangPowerToolsShared.MVVM.Models.ToolWindowModels;
using ClangPowerTools.MVVM.Command;
using System.Threading.Tasks;
using ClangPowerTools.Commands;
using ClangPowerToolsShared.MVVM.Controllers;
using System.Collections.ObjectModel;
using ClangPowerToolsShared.Commands;
using ClangPowerToolsShared.MVVM.Interfaces;

namespace ClangPowerToolsShared.MVVM.ViewModels
{

  public class FindToolWindowViewModel : FindController
  {
    private List<string> filesPaths = new();

    public List<IViewMatcher> ViewMatchers
    {
      get { return FindToolWindowModel.ViewMatchers;  }
    }

    public FindToolWindowViewModel(FindToolWindowView findToolWindowView)
    {
      this.findToolWindowView = findToolWindowView;
    }

    public void OpenToolWindow(List<string> filesPath)
    {
      filesPaths = filesPath;
    }

    public void RunQuery()
    {
      if (!RunController.StopCommandActivated)
      {
        SelectCommandToRun(findToolWindowModel.CurrentViewMatcher);
        RunPowershellQuery(filesPaths);
      }
      AfterCommand();
    }

    public void SelectCommandToRun(IViewMatcher viewMatcher)
    {
      findToolWindowModel.UpdateUiToSelectedModel(viewMatcher);
      FindToolWindowModel = findToolWindowModel;
    }

    public void RunCommandFromView()
    {
      BeforeCommand();
      LaunchCommand();
      CommandControllerInstance.CommandController.LaunchCommandAsync(CommandIds.kClangFindRun, CommandUILocation.ContextMenu);

    }
  } 
}