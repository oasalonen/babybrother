using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace BabyBrother
{
    public sealed partial class FormEntry : UserControl
    {
        public string Header
        {
            get { return (string) GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public UIElement Field
        {
            get { return (UIElement)GetValue(FieldProperty); }
            set { SetValue(FieldProperty, value); }
        }

        private static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(FormEntry),
            new PropertyMetadata(null, OnHeaderPropertyChanged));

        private static readonly DependencyProperty FieldProperty = DependencyProperty.Register("Field", typeof(UIElement), typeof(FormEntry), 
            new PropertyMetadata(null, OnFieldPropertyChanged));

        public FormEntry()
        {
            this.InitializeComponent();
        }

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
 	        var self = d as FormEntry;
            var header = e.NewValue as string;
            self.HeaderText.Text = header;
        }

        private static void OnFieldPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as FormEntry;
            var field = e.NewValue as UIElement;
            self.EntryContent.Content = field;
        } 
    }
}
