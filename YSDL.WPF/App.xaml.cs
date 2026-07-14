using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace YSDL.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mutex = new Mutex(true, AppDomain.CurrentDomain.FriendlyName);

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                MessageBox.Show("程序已经打开，不能重复启动！");
                Application.Current.Shutdown();
                return;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//.net 运行时注册其他编码

            #region 添加异常处理
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // 对于UI线程的未处理异常
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            // 对于非UI线程抛出的未处理异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; ;
            #endregion

            base.OnStartup(e);
        }

        /// <summary>
        /// 非UI线程中异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                //ShowError(ex);
            }
            else
            {
                //ShowError(new Exception(e.Dump()));
            }
        }

        /// <summary>
        ///  UI 线程中异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is COMException com)
            {
                if (e.Exception.Message.Contains("UCEERR_RENDERTHREADFAILURE") ||
                    com.ErrorCode == unchecked((int)0x800401D0))
                {
                    e.Handled = true;
                }
            }
            else
            {
                // ShowError(e.Exception);
                //todo log
            }
        }


        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            //todo set log 
            e.SetObserved();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            mutex?.ReleaseMutex();
            mutex?.Dispose();
            base.OnExit(e);
        }
    }

}
