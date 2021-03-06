﻿using System.Windows;
using System.Windows.Controls;

namespace BEditor.Views.CreatePage
{
    /// <summary>
    /// SceneCreatePage.xaml の相互作用ロジック
    /// </summary>
    public partial class SceneCreatePage : UserControl
    {
        public SceneCreatePage() => InitializeComponent();

        private void CloseClick(object sender, RoutedEventArgs e) => Window.GetWindow(this).Close();
    }
}
