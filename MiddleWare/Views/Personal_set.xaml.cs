﻿using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiddleWare.Views
{
    /// <summary>
    /// Personal_set.xaml 的交互逻辑
    /// </summary>
    public partial class Personal_set : UserControl
    {
        private static PersonalSet personalSet = new PersonalSet
        {
            IsSocketASCII = AppConfig.GetAppConfig("SocketCode") == "ASCII",
            IsSocketUTF8 = AppConfig.GetAppConfig("SocketCode") == "UTF8",
            IsComASCII = AppConfig.GetAppConfig("ComCode") == "ASCII",
            IsComUTF8 = AppConfig.GetAppConfig("ComCode") == "UTF8"
        };
        private static ObservableCollection<LanguageSelect> langSelect = new ObservableCollection<LanguageSelect>
        {
            new LanguageSelect {NAME="简体中文",ID=0 },//简体中文必须放第一个  
            new LanguageSelect {NAME="English",ID=1 }
        };

        public Personal_set()
        {
            InitializeComponent();

            this.grid_personalset.DataContext = personalSet;
            combobox_language.ItemsSource = langSelect;
            version.Text = GlobalVariable.Version;
            if (AppConfig.GetAppConfig("Language") != null) 
            {
                try
                {
                    GlobalVariable.Language = Convert.ToInt16(AppConfig.GetAppConfig("Language"));
                }
                catch
                {
                    GlobalVariable.Language = 0;
                }
                
            }
            else
            {
                GlobalVariable.Language  = 0;//先索引第一个,简体中文
            }

            combobox_language.SelectedValue = GlobalVariable.Language;


            ResourceDictionary dict = new ResourceDictionary();

            if (GlobalVariable.Language == 0)//chinese
            {
                dict.Source = new Uri(@"Resources\zh.xaml", UriKind.Relative);
            }
            else if (GlobalVariable.Language == 1) //english
            {
                dict.Source = new Uri(@"Resources\en.xaml", UriKind.Relative);
            }

            Application.Current.Resources.MergedDictionaries.Add(dict);

            /*LIS重连次数和样本最大重复上传次数 读取样本设置*/
            if (AppConfig.GetAppConfig("ReSendNum") != null)
            {
                try
                {
                    GlobalVariable.ReSendNum = Convert.ToInt16(AppConfig.GetAppConfig("ReSendNum"));
                }
                catch
                {
                    GlobalVariable.ReSendNum = 5;
                }
            }
            else
            {
                GlobalVariable.ReSendNum = 5;
            }
            if (AppConfig.GetAppConfig("ReLisConnectNum") != null)
            {
                try
                {
                    GlobalVariable.ReLisConnectNum = Convert.ToInt16(AppConfig.GetAppConfig("ReLisConnectNum"));
                }
                catch
                {
                    GlobalVariable.ReLisConnectNum = 3;
                }
            }
            else
            {
                GlobalVariable.ReLisConnectNum = 3;
            }
                
        }

        private async void button_languageOK_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                GlobalVariable.Language = (int)combobox_language.SelectedValue;//GlobalVariable.Language对应ID号 

                ResourceDictionary dict = new ResourceDictionary();

                if (GlobalVariable.Language == 1)//english
                {
                    dict.Source = new Uri(@"Resources\en.xaml", UriKind.Relative);

                }
                else//chinese
                {
                    dict.Source = new Uri(@"Resources\zh.xaml", UriKind.Relative);
                }

                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
            catch
            {
                MainWindow mainwin = (MainWindow)System.Windows.Application.Current.MainWindow;
                await mainwin.ShowMessageAsync("通知", "请选择语言");
            }
            /*多语言切换程序 信号量GlobalVariable.Language*/

            /*todo 保存到配置文件*/
            AppConfig.UpdateAppConfig("Language", GlobalVariable.Language.ToString());
        }

        private async void button_resendnum_Click(object sender, RoutedEventArgs e)
        {
            int resendnum = 0;
            MainWindow mainwin = (MainWindow)System.Windows.Application.Current.MainWindow;
            try
            {
                resendnum = Convert.ToInt32(textbox_resendnum.Text);
            }
            catch
            {
                await mainwin.ShowMessageAsync("通知", "请正确输入");
                return;
            }
            if (resendnum > 0 && resendnum < 10) 
            {
                GlobalVariable.ReSendNum = resendnum;
                await mainwin.ShowMessageAsync("通知", "修改成功");
                AppConfig.UpdateAppConfig("ReSendNum", resendnum.ToString());
            }
            else
            {
                await mainwin.ShowMessageAsync("通知", "请输入正常范围");
            }
        }

        private async void button_relisconnectnum_Click(object sender, RoutedEventArgs e)
        {
            int relisconnectnum = 0;
            MainWindow mainwin = (MainWindow)System.Windows.Application.Current.MainWindow;
            try
            {
                relisconnectnum = Convert.ToInt32(textbox_relisconnectnum.Text);
            }
            catch
            {
                await mainwin.ShowMessageAsync("通知", "请正确输入");
                return;
            }
            if (relisconnectnum > 0 && relisconnectnum < 6)
            {
                GlobalVariable.ReLisConnectNum = relisconnectnum;
                await mainwin.ShowMessageAsync("通知", "修改成功");
                AppConfig.UpdateAppConfig("ReLisConnectNum", relisconnectnum.ToString());
            }
            else
            {
                await mainwin.ShowMessageAsync("通知", "请输入正常范围");
            }
        }

        private async void button_clearcache_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwin = (MainWindow)System.Windows.Application.Current.MainWindow;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                if (key != "SocketCode" && key != "ComCode") 
                {
                    config.AppSettings.Settings.Remove(key);
                }
            }
            config.Save();//保存
            ConfigurationManager.RefreshSection("appSettings");//刷新
            await mainwin.ShowMessageAsync("通知", "已成功清除缓存");
            return;
        }
    }

    public class PersonalSet : INotifyPropertyChanged //界面中的绑定元素都可以在这个类内定义
    {
        private bool _IsSocketASCII;
        private bool _IsSocketUTF8;
        private bool _IsComASCII;
        private bool _IsComUTF8;

        public bool IsSocketASCII
        {
            get
            {
                return this._IsSocketASCII;
            }
            set
            {
                if(this._IsSocketASCII!=value)
                {
                    this._IsSocketASCII = value;
                    OnPropertyChanged("IsSocketASCII");
                }
            }
        }
        public bool IsSocketUTF8
        {
            get
            {
                return this._IsSocketUTF8;
            }
            set
            {
                if(this._IsSocketUTF8!=value)
                {
                    this._IsSocketUTF8 = value;
                    OnPropertyChanged("IsSocketUTF8");
                }
            }
        }
        public bool IsComASCII
        {
            get
            {
                return this._IsComASCII;
            }
            set
            {
                if(this._IsComASCII!=value)
                {
                    this._IsComASCII = value;
                    OnPropertyChanged("IsComASCII");
                }
            }
        }
        public bool IsComUTF8
        {
            get
            {
                return this._IsComUTF8;
            }
            set
            {
                if(this._IsComUTF8!=value)
                {
                    this._IsComUTF8 = value;
                    OnPropertyChanged("IsComUTF8");
                }
            }
        }

        public PersonalSet()
        {
            this.PropertyChanged += CodeSetPropertyChanged;
        }
        void CodeSetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSocketASCII")
            {
                GlobalVariable.SocketCode = IsSocketASCII;
                IsSocketUTF8 = !IsSocketASCII;
                AppConfig.UpdateAppConfig("SocketCode", IsSocketASCII == true ? "ASCII" : "UTF8");
            }
            else if (e.PropertyName == "IsSocketUTF8")
            {
                IsSocketASCII = !IsSocketUTF8;
                GlobalVariable.SocketCode = IsSocketASCII;
                AppConfig.UpdateAppConfig("SocketCode", IsSocketASCII == true ? "ASCII" : "UTF8");
            }
            else if (e.PropertyName == "IsComASCII")
            {
                GlobalVariable.ComCode = IsComASCII;
                IsComUTF8 = !IsComASCII;
                AppConfig.UpdateAppConfig("ComCode", IsComASCII == true ? "ASCII" : "UTF8");
            }
            else if (e.PropertyName == "IsComUTF8") 
            {
                IsComASCII = !IsComUTF8;
                GlobalVariable.ComCode = IsComASCII;
                AppConfig.UpdateAppConfig("ComCode", IsComASCII == true ? "ASCII" : "UTF8");
            }

        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }

    public class LanguageSelect
    {
        public string NAME { get; set; }
        public int ID { get; set; }
    }

}
