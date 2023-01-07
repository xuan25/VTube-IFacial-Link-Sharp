using Microsoft.UI.Xaml;

namespace VTube_IFacial_Link.UI
{
    internal class ThemeShadow : Microsoft.UI.Xaml.Media.ThemeShadow
    {
        public static readonly DependencyProperty ReceiverProperty = DependencyProperty.Register(nameof(Receiver), typeof(UIElement), typeof(ThemeShadow), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            ThemeShadow s = (ThemeShadow)d;
            s.Receivers.Clear();
            UIElement receiver = (UIElement)e.NewValue;
            if (receiver != null)
            {
                s.Receivers.Add(receiver);
            }
        }));
        public UIElement Receiver
        {
            get => (UIElement)GetValue(ReceiverProperty);
            set => SetValue(ReceiverProperty, value);
        }
    }
}
