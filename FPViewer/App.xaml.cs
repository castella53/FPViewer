using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FPViewer
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string mes = string.Format(
                "Unhandled Error:\n\n---\n\n{0}\n\n{1}",
                e.Exception.Message, e.Exception.StackTrace);

            Console.WriteLine(mes);
            // 例外を処理したことを通知
            e.Handled = true;
        }
    }
}
