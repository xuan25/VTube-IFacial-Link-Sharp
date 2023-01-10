// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using VTube_IFacial_Link.Models;
using VTube_IFacial_Link.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace VTube_IFacial_Link.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Nav

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("home", typeof(HomePage)),
            ("data", typeof(DataPage)),
            ("globals", typeof(GlobalsPage)),
            ("parameters", typeof(ParametersPage)),
        };

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                throw new NotImplementedException("No Setting Page");
                //_page = typeof(SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, null, transitionInfo);
            }
        }

        #endregion

        #region Common

        public MainPage()
        {
            ViewModel = new MainViewModel(this);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            InitializeComponent();
        }

        #endregion

        #region Property

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.IsBusy):
                    TimeSpan animationTime = new(250 * 10000);
                    if (ViewModel.IsBusy)
                    {
                        TimeSpan holdTime = new(1000 * 10000);
                        Storyboard storyboard = new();
                        {
                            DoubleAnimation doubleAnimation = new()
                            {
                                EnableDependentAnimation = true,
                                BeginTime = holdTime,
                                Duration = animationTime,
                                From = 1.15,
                                To = 1,
                                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                            };
                            Storyboard.SetTarget(doubleAnimation, this);
                            Storyboard.SetTargetProperty(doubleAnimation, nameof(BusyMessageScale));
                            storyboard.Children.Add(doubleAnimation);
                        }
                        {
                            DoubleAnimation doubleAnimation = new()
                            {
                                EnableDependentAnimation = true,
                                BeginTime = holdTime,
                                Duration = animationTime,
                                From = 0,
                                To = 1,
                                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                            };
                            Storyboard.SetTarget(doubleAnimation, this);
                            Storyboard.SetTargetProperty(doubleAnimation, nameof(BusyMessageOpacity));
                            storyboard.Children.Add(doubleAnimation);
                        }
                        storyboard.Begin();
                    }
                    else
                    {
                        Storyboard storyboard = new();
                        {
                            DoubleAnimation doubleAnimation = new()
                            {
                                EnableDependentAnimation = true,
                                Duration = animationTime,
                                To = 1.15,
                                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
                            };
                            Storyboard.SetTarget(doubleAnimation, this);
                            Storyboard.SetTargetProperty(doubleAnimation, nameof(BusyMessageScale));
                            storyboard.Children.Add(doubleAnimation);
                        }
                        {
                            DoubleAnimation doubleAnimation = new()
                            {
                                EnableDependentAnimation = true,
                                Duration = animationTime,
                                To = 0,
                                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
                            };
                            Storyboard.SetTarget(doubleAnimation, this);
                            Storyboard.SetTargetProperty(doubleAnimation, nameof(BusyMessageOpacity));
                            storyboard.Children.Add(doubleAnimation);
                        }
                        storyboard.Begin();
                    }
                    break;

            }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(MainViewModel), typeof(MainPage), new PropertyMetadata(null));
        internal MainViewModel ViewModel
        {
            get => (MainViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty BusyMessageOpacityProperty = DependencyProperty.Register(nameof(BusyMessageOpacity), typeof(double), typeof(MainPage), new PropertyMetadata(0d));
        public double BusyMessageOpacity
        {
            get => (double)GetValue(BusyMessageOpacityProperty);
            set => SetValue(BusyMessageOpacityProperty, value);
        }

        public static readonly DependencyProperty BusyMessageScaleProperty = DependencyProperty.Register(nameof(BusyMessageScale), typeof(double), typeof(MainPage), new PropertyMetadata(1d));
        public double BusyMessageScale
        {
            get => (double)GetValue(BusyMessageScaleProperty);
            set => SetValue(BusyMessageScaleProperty, value);
        }

        #endregion

    }
}
