using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BabyBrother
{
    public sealed partial class ImageToggle : UserControl
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public ImageSource ImageSelected
        {
            get { return (ImageSource)GetValue(ImageSelectedProperty); }
            set { SetValue(ImageSelectedProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public bool IsImageFlipped
        {
            get { return (bool)GetValue(IsImageFlippedProperty); }
            set { SetValue(IsImageFlippedProperty, value); }
        }

        private static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ImageToggle),
            new PropertyMetadata(null, OnCommandPropertyChanged));

        private static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageToggle),
            new PropertyMetadata(null, OnImagePropertyChanged));

        private static readonly DependencyProperty ImageSelectedProperty = DependencyProperty.Register("ImageSelected", typeof(ImageSource), typeof(ImageToggle),
            new PropertyMetadata(null, OnImagePropertyChanged));

        private static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ImageToggle),
            new PropertyMetadata(null, OnTextPropertyChanged));

        private static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ImageToggle),
            new PropertyMetadata(false, OnIsSelectedPropertyChanged));

        private static readonly DependencyProperty IsImageFlippedProperty = DependencyProperty.Register("IsImageFlipped", typeof(bool), typeof(ImageToggle),
            new PropertyMetadata(false, OnIsImageFlippedPropertyChanged));

        public ImageToggle()
        {
            this.InitializeComponent();
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ImageToggle;
            self.RootButton.Command = e.NewValue as ICommand;
        }

        private static void OnImagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ImageToggle;
            self.SetImage();
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ImageToggle;
            self.ButtonText.Text = e.NewValue as string;
        }

        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ImageToggle;
            self.SetImage();
        }

        private static void OnIsImageFlippedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ImageToggle;
            var isFlipped = (bool)e.NewValue;
            self.ImageFlipTransform.ScaleX = isFlipped ? -1 : 1;
        }

        private void SetImage()
        {
            ButtonImage.Source = IsSelected ? ImageSelected : Image;
        }
    }
}
