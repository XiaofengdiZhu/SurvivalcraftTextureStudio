using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Windows.Threading;

namespace SurvivalcraftTextureStudio
{
    public static class NotifyIcon
    {
        public static System.Windows.Forms.NotifyIcon notifyIcon;
        public static System.Windows.Controls.ContextMenu NotifyIconMenu;
        private static System.Drawing.Icon Icon;
        private static System.Drawing.Icon BlankIcon;
        private static bool isBlinking;
        private static bool isBlank;
        private static DispatcherTimer BlinkDT;
        public static bool IsInitialized;

        public static void Initiate()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Icon = new System.Drawing.Icon((Stream)new ResourceManager(assembly.GetName().Name + ".g", assembly).GetResourceSet(CultureInfo.CurrentCulture, true, true).GetObject("favicon.ico", true));
            BlankIcon = new System.Drawing.Icon((Stream)new ResourceManager(assembly.GetName().Name + ".g", assembly).GetResourceSet(CultureInfo.CurrentCulture, true, true).GetObject("blank.ico", true));
            notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                BalloonTipTitle = "欢迎使用",
                BalloonTipText = "从Excel批量生成Word",
                Text = "从Excel批量生成Word",
                Icon = Icon,
                BalloonTipIcon = ToolTipIcon.None,
                Visible = true
            };
            notifyIcon.MouseDoubleClick += new MouseEventHandler((s, e) =>
            {
                MainWindow.MW.RecoverWindow();
            });
            NotifyIconMenu = (System.Windows.Controls.ContextMenu)MainWindow.MW.FindResource("NotifyIconMenu");
            notifyIcon.MouseClick += new MouseEventHandler((s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    NotifyIconMenu.IsOpen = true;
                }
            });
            BlinkDT = new DispatcherTimer(
                            TimeSpan.FromMilliseconds(500),
                            DispatcherPriority.Normal,
                            new EventHandler((o, ev) =>
                            {
                                isBlank = !isBlank;
                                notifyIcon.Icon = isBlank ? BlankIcon : Icon;
                            }), MainWindow.MW.Dispatcher)
            { IsEnabled = false };
            IsInitialized = true;
        }

        public static void ShowNotification(string content, string title = "从Excel批量生成Word", ToolTipIcon tipIcon = ToolTipIcon.None)
        {
            notifyIcon.ShowBalloonTip(0, content, title, tipIcon);
        }

        public static bool IsBlinking
        {
            get { return isBlinking; }
            set
            {
                isBlinking = value;
                if (value)
                {
                    BlinkDT.Start();
                }
                else
                {
                    BlinkDT.Stop();
                    if (isBlank)
                    {
                        notifyIcon.Icon = Icon;
                    }
                }
            }
        }
    }
}