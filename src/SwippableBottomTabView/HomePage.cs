﻿using SwippableBottomTabView.Controls;
using SwippableBottomTabView.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace SwippableBottomTabView
{
    public class HomePage : ContentPage
    {
        private View _tabs;

        private RelativeLayout _relativeLayout;

        private CarouselLayout.IndicatorStyleEnum _indicatorStyle;

        private TabbedPageViewModel _viewModel;

        public HomePage(CarouselLayout.IndicatorStyleEnum indicatorStyle)
        {
            _indicatorStyle = indicatorStyle;

            _viewModel = new TabbedPageViewModel();
            BindingContext = _viewModel;

            BackgroundColor = Color.Black;

            Title = _indicatorStyle.ToString();

            _relativeLayout = new RelativeLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var pagesCarousel = CreatePagesCarousel();
            var dots = CreatePagerIndicatorContainer();
            _tabs = CreateTabs();

            switch (pagesCarousel.IndicatorStyle)
            {
                case CarouselLayout.IndicatorStyleEnum.Dots:
                    _relativeLayout.Children.Add(pagesCarousel,
                        Constraint.RelativeToParent((parent) => { return parent.X; }),
                        Constraint.RelativeToParent((parent) => { return parent.Y; }),
                        Constraint.RelativeToParent((parent) => { return parent.Width; }),
                        Constraint.RelativeToParent((parent) => { return parent.Height / 2; })
                    );

                    _relativeLayout.Children.Add(dots,
                        Constraint.Constant(0),
                        Constraint.RelativeToView(pagesCarousel,
                            (parent, sibling) => { return sibling.Height - 18; }),
                        Constraint.RelativeToParent(parent => parent.Width),
                        Constraint.Constant(18)
                    );
                    break;

                case CarouselLayout.IndicatorStyleEnum.Tabs:
                    var tabsHeight = 50;
                    _relativeLayout.Children.Add(_tabs,
                        Constraint.Constant(0),
                        Constraint.RelativeToParent((parent) => { return parent.Height - tabsHeight; }),
                        Constraint.RelativeToParent(parent => parent.Width),
                        Constraint.Constant(tabsHeight)
                    );

                    _relativeLayout.Children.Add(pagesCarousel,
                        Constraint.RelativeToParent((parent) => { return parent.X; }),
                        Constraint.RelativeToParent((parent) => { return parent.Y; }),
                        Constraint.RelativeToParent((parent) => { return parent.Width; }),
                        Constraint.RelativeToView(_tabs, (parent, sibling) => { return parent.Height - (sibling.Height); })
                    );
                    break;

                default:
                    _relativeLayout.Children.Add(pagesCarousel,
                        Constraint.RelativeToParent((parent) => { return parent.X; }),
                        Constraint.RelativeToParent((parent) => { return parent.Y; }),
                        Constraint.RelativeToParent((parent) => { return parent.Width; }),
                        Constraint.RelativeToParent((parent) => { return parent.Height; })
                    );
                    break;
            }

            Content = _relativeLayout;
        }

        private CarouselLayout CreatePagesCarousel()
        {
            var carousel = new CarouselLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IndicatorStyle = _indicatorStyle,
                ItemTemplate = new DataTemplate(typeof(DynamicTemplateLayout))
            };
            carousel.SetBinding(CarouselLayout.ItemsSourceProperty, "Pages");
            carousel.SetBinding(CarouselLayout.SelectedItemProperty, "CurrentPage", BindingMode.TwoWay);

            return carousel;
        }

        private View CreatePagerIndicatorContainer()
        {
            return new StackLayout
            {
                Children = { CreatePagerIndicators() }
            };
        }

        private View CreatePagerIndicators()
        {
            var pagerIndicator = new PagerIndicatorDots() { DotSize = 5, DotColor = Color.Black };
            pagerIndicator.SetBinding(PagerIndicatorDots.ItemsSourceProperty, "Pages");
            pagerIndicator.SetBinding(PagerIndicatorDots.SelectedItemProperty, "CurrentPage");
            return pagerIndicator;
        }

        private View CreateTabsContainer()
        {
            return new StackLayout
            {
                Children = { CreateTabs() }
            };
        }

        private View CreateTabs()
        {
            var pagerIndicator = new PagerIndicatorTabs() { HorizontalOptions = LayoutOptions.CenterAndExpand };
            pagerIndicator.RowDefinitions.Add(new RowDefinition() { Height = 50 });
            pagerIndicator.SetBinding(PagerIndicatorTabs.ColumnDefinitionsProperty, "Pages", BindingMode.Default, new SpacingConverter());
            pagerIndicator.SetBinding(PagerIndicatorTabs.ItemsSourceProperty, "Pages");
            pagerIndicator.SetBinding(PagerIndicatorTabs.SelectedItemProperty, "CurrentPage");

            return pagerIndicator;
        }
    }

    public class SpacingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = value as IEnumerable<ICarouselViewModel>;

            var collection = new ColumnDefinitionCollection();
            foreach (var item in items)
            {
                collection.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            return collection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}