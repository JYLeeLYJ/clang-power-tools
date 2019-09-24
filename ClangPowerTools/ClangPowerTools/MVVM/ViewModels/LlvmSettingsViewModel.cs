﻿using ClangPowerTools.MVVM.Commands;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ClangPowerTools
{
  public class LlvmSettingsViewModel : INotifyPropertyChanged
  {
    #region Members
    public event PropertyChangedEventHandler PropertyChanged;

    private const string exeParameters = "/S /D=";
    private const string arguments = @"/C reg delete HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\LLVM /f &";
    private const string processFileName = "cmd.exe";
    private const string processVerb = "runas";

    Process process;
    private string executableName = "test.exe";
    private string destinationFile;
    private string appdDataPath;
    private LlvmSettingsModel llvmModel = new LlvmSettingsModel();
    private ICommand dowloadCommand;
    private ICommand deleteCommand;
    private ICommand stopCommand;

    #endregion

    #region Constructor
    public LlvmSettingsViewModel()
    {
      var settingsPathBuilder = new SettingsPathBuilder();
      appdDataPath = settingsPathBuilder.GetPath("");
      destinationFile = string.Concat(appdDataPath, "\\", executableName);
    }
    #endregion

    #region Properties
    public LlvmSettingsModel LlvmModel
    {
      get
      {
        return llvmModel;
      }
      set
      {
        llvmModel = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LlvmModel"));
      }
    }


    public bool CanExecute
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region Commands
    public ICommand DownloadCommand
    {
      get => dowloadCommand ?? (dowloadCommand = new RelayCommand(() => DownloadLlvmVersion(), () => CanExecute));
    }

    public ICommand DeleteCommand
    {
      get => deleteCommand ?? (deleteCommand = new RelayCommand(() => DeleteLlvmVersion("8.0.0"), () => CanExecute));
    }

    public ICommand StopCommand
    {
      get => stopCommand ?? (stopCommand = new RelayCommand(() => StopDownload(), () => CanExecute));
    }


    #endregion

    #region Methods
    private void DownloadLlvmVersion()
    {
      using (var client = new WebClient())
      {
        client.DownloadProgressChanged += DownloadProgressChanged;
        client.DownloadFileAsync(new Uri("https://github.com/llvm/llvm-project/releases/download/llvmorg-8.0.1/LLVM-8.0.1-win32.exe"), destinationFile);
      }
    }
    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      llvmModel.DownloadProgress = e.ProgressPercentage;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LlvmModel"));
    }


    private void InstallLlVmVersion()
    {
      var startInfoArguments = string.Concat(arguments, " ", destinationFile, " ", exeParameters, appdDataPath, "\\Folder");

      try
      {
        process = new Process();
        process.StartInfo.FileName = processFileName;
        process.StartInfo.Arguments = startInfoArguments;
        process.StartInfo.Verb = processVerb;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.EnableRaisingEvents = true;
        process.Exited += new EventHandler(ProcessExited);
        process.Start();
      }
      catch (Exception e)
      {

        MessageBox.Show(e.Message, "Installation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void ProcessExited(object sender, EventArgs e)
    {
      process.Dispose();
    }


    private void DeleteLlvmVersion(string version)
    {

    }


    private void StopDownload()
    {

    }
    #endregion
  }
}
