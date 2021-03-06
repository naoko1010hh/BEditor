﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BEditor.WPF.Controls
{
    public class BasePropertyView : Control
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(BasePropertyView));
        public static readonly DependencyProperty ResetCommandProperty = DependencyProperty.Register(nameof(ResetCommand), typeof(ICommand), typeof(BasePropertyView));
        public static readonly DependencyProperty BindCommandProperty = DependencyProperty.Register(nameof(BindCommand), typeof(ICommand), typeof(BasePropertyView));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public ICommand ResetCommand
        {
            get => (ICommand)GetValue(ResetCommandProperty);
            set => SetValue(ResetCommandProperty, value);
        }
        public ICommand BindCommand
        {
            get => (ICommand)GetValue(BindCommandProperty);
            set => SetValue(BindCommandProperty, value);
        }
    }
}
