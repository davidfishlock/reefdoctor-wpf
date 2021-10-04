using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FlipViewControl
{
    public class FlipView : Selector
    {
        #region Private Fields
        private ContentControl PART_CurrentItem;
        private ContentControl PART_PreviousItem;
        private ContentControl PART_NextItem;
        private FrameworkElement PART_Root;
        private FrameworkElement PART_Container;
        private double fromValue = 0.0;
        private double elasticFactor = 1.0;
        #endregion

        public static DependencyProperty IsFlipEnabledProperty = DependencyProperty.Register("IsFlipEnabled", typeof(bool), typeof(FlipView), new PropertyMetadata(true));
        public static DependencyProperty HandleTapOnChildProperty = DependencyProperty.Register("HandleTapOnChild", typeof(bool), typeof(FlipView), new PropertyMetadata(true));
        public static DependencyProperty TapCommandProperty = DependencyProperty.Register("TapCommand", typeof(ICommand), typeof(FlipView));
        public static DependencyProperty TapCommandParameterProperty = DependencyProperty.Register("TapCommandParameter", typeof(object), typeof(FlipView));

        public bool IsFlipEnabled
        {
            get
            {
                return (bool)GetValue(IsFlipEnabledProperty);
            }
            set
            {
                SetValue(IsFlipEnabledProperty, value);
            }
        }

        public bool HandleTapOnChild
        {
            get
            {
                return (bool)GetValue(HandleTapOnChildProperty);
            }
            set
            {
                SetValue(HandleTapOnChildProperty, value);
            }
        }

        public ICommand TapCommand
        {
            get
            {
                return (ICommand)GetValue(TapCommandProperty);
            }
            set
            {
                SetValue(TapCommandProperty, value);
            }
        }

        public object TapCommandParameter
        {
            get
            {
                return (ICommand)GetValue(TapCommandParameterProperty);
            }
            set
            {
                SetValue(TapCommandParameterProperty, value);
            }
        }

        #region Constructor
        static FlipView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlipView), new FrameworkPropertyMetadata(typeof(FlipView)));
            SelectedIndexProperty.OverrideMetadata(typeof(FlipView), new FrameworkPropertyMetadata(-1, OnSelectedIndexChanged));
        }

        public FlipView()
        {
            this.CommandBindings.Add(new CommandBinding(NextCommand, this.OnNextExecuted, this.OnNextCanExecute));
            this.CommandBindings.Add(new CommandBinding(PreviousCommand, this.OnPreviousExecuted, this.OnPreviousCanExecute));
        }

        #endregion

        #region Private methods
        private async void OnRootManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (this.IsFlipEnabled)
            {
                this.fromValue = e.TotalManipulation.Translation.X;

                if (this.fromValue != 0)
                {
                    if (e.TotalManipulation.Translation.X > this.ActualWidth / 4 ||
                    e.TotalManipulation.Translation.X < 0 - (this.ActualWidth / 4) ||
                    e.FinalVelocities.LinearVelocity.X < -1 ||
                    e.FinalVelocities.LinearVelocity.X > 1)
                    {
                        if (this.fromValue > 0)
                        {
                            if (this.SelectedIndex > 0)
                            {
                                await this.RunSlideAnimation(this.ActualWidth, fromValue);
                                this.SelectedIndex -= 1;
                            }
                        }
                        else
                        {
                            if (this.SelectedIndex < this.Items.Count - 1)
                            {
                                await this.RunSlideAnimation(-this.ActualWidth, fromValue);
                                this.SelectedIndex += 1;
                            }
                        }
                    }
                    else
                    {
                        await this.RunSlideAnimation(0, fromValue);
                    }
                }

                if (this.elasticFactor < 1)
                {
                    if (this.PART_Root.RenderTransform is MatrixTransform)
                    {
                        await this.RunSlideAnimation(0, ((MatrixTransform)this.PART_Root.RenderTransform).Matrix.OffsetX);
                    }
                }
                this.elasticFactor = 1.0;
            }

            try
            {
                if (fromValue == 0 && this.TapCommand != null)
                {
                    if (this.HandleTapOnChild)
                    {
                        Image foundImage;

                        var contentPresenter = (UIElement)VisualTreeHelper.GetChild(this.PART_CurrentItem, 0);
                        var rootContent = (UIElement)VisualTreeHelper.GetChild(contentPresenter, 0);

                        if (rootContent is Image)
                        {
                            foundImage = (Image)rootContent;
                        }
                        else
                        {
                            var firstContentChild = (UIElement)VisualTreeHelper.GetChild(rootContent, 0);
                            foundImage = (Image)firstContentChild;
                        }

                        if (foundImage != null)
                        {
                            var translatePoint = foundImage.TranslatePoint(new Point(0, 0), this.PART_Root);

                            if (e.ManipulationOrigin.X >= translatePoint.X &&
                            e.ManipulationOrigin.Y >= translatePoint.Y &&
                            e.ManipulationOrigin.X <= translatePoint.X + foundImage.ActualWidth &&
                            e.ManipulationOrigin.Y <= translatePoint.Y + foundImage.ActualHeight)
                            {
                                this.TapCommand.Execute(this.SelectedItem);
                            }
                        }
                    }
                    else
                    {
                        this.TapCommand.Execute(this.SelectedItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private void OnRootManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (this.IsFlipEnabled)
            {
                if (!(this.PART_Root.RenderTransform is MatrixTransform))
                {
                    this.PART_Root.RenderTransform = new MatrixTransform();
                }

                Matrix matrix = ((MatrixTransform)this.PART_Root.RenderTransform).Matrix;
                var delta = e.DeltaManipulation;

                if ((this.SelectedIndex == 0 && delta.Translation.X > 0 && this.elasticFactor > 0)
                    || (this.SelectedIndex == this.Items.Count - 1 && delta.Translation.X < 0 && this.elasticFactor > 0))
                {
                    this.elasticFactor -= 0.05;
                }

                matrix.Translate(delta.Translation.X * elasticFactor, 0);
                this.PART_Root.RenderTransform = new MatrixTransform(matrix);

                e.Handled = true;
            }
        }

        private void OnRootManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            if (this.IsFlipEnabled)
            {
                if (e.ManipulationContainer.GetType() == typeof(FlipViewPanel))
                {
                    e.ManipulationContainer = this.PART_Container;
                    e.Handled = true;
                }
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RefreshViewPort(this.SelectedIndex);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.SelectedIndex > -1)
            {
                this.RefreshViewPort(this.SelectedIndex);
            }
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FlipView;

            control.OnSelectedIndexChanged(e);
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!this.EnsureTemplateParts())
            {
                return;
            }

            Task.Run(async () => {
                await Task.Delay(100);
                Dispatcher.BeginInvoke(new Action(() => { this.RefreshViewPort(this.SelectedIndex); }));
            });
        }

        private void RefreshViewPort(int selectedIndex)
        {
            if (!this.EnsureTemplateParts())
            {
                return;
            }

            Canvas.SetLeft(this.PART_PreviousItem, -this.ActualWidth);
            Canvas.SetLeft(this.PART_NextItem, this.ActualWidth);
            this.PART_Root.RenderTransform = new TranslateTransform();

            var currentItem = this.GetItemAt(selectedIndex);
            var nextItem = this.GetItemAt(selectedIndex + 1);
            var previousItem = this.GetItemAt(selectedIndex - 1);

            this.PART_CurrentItem.Content = currentItem;
            this.PART_NextItem.Content = nextItem;
            this.PART_PreviousItem.Content = previousItem;
        }

        public async Task RunSlideAnimation(double toValue, double fromValue = 0)
        {
            if (!(this.PART_Root.RenderTransform is TranslateTransform))
            {
                this.PART_Root.RenderTransform = new TranslateTransform();
            }

            await AnimationFactory.Instance.AnimateTo(this.PART_Root, toValue, fromValue);
        }

        private object GetItemAt(int index)
        {
            if (index < 0 || index >= this.Items.Count)
            {
                return null;
            }

            return this.Items[index];
        }

        private bool EnsureTemplateParts()
        {
            return this.PART_CurrentItem != null &&
                this.PART_NextItem != null &&
                this.PART_PreviousItem != null &&
                this.PART_Root != null;
        }

        private void OnPreviousCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectedIndex > 0 && this.IsFlipEnabled;
        }

        private async void OnPreviousExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await this.RunSlideAnimation(this.ActualWidth, fromValue);
            this.SelectedIndex -= 1;
        }

        private void OnNextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectedIndex < (this.Items.Count - 1) && this.IsFlipEnabled;
        }

        private async void OnNextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await this.RunSlideAnimation(-this.ActualWidth, fromValue);
            this.SelectedIndex += 1;
        }
        #endregion

        #region Commands

        public static RoutedUICommand NextCommand = new RoutedUICommand("Next", "Next", typeof(FlipView));
        public static RoutedUICommand PreviousCommand = new RoutedUICommand("Previous", "Previous", typeof(FlipView));

        #endregion

        #region Override methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_PreviousItem = this.GetTemplateChild("PART_PreviousItem") as ContentControl;
            this.PART_NextItem = this.GetTemplateChild("PART_NextItem") as ContentControl;
            this.PART_CurrentItem = this.GetTemplateChild("PART_CurrentItem") as ContentControl;
            this.PART_Root = this.GetTemplateChild("PART_Root") as FrameworkElement;
            this.PART_Container = this.GetTemplateChild("PART_Container") as FrameworkElement;

            this.Loaded += this.OnLoaded;
            this.SizeChanged += this.OnSizeChanged;

            this.PART_Root.ManipulationStarting += this.OnRootManipulationStarting;
            this.PART_Root.ManipulationDelta += this.OnRootManipulationDelta;
            this.PART_Root.ManipulationCompleted += this.OnRootManipulationCompleted;

            if (this.HandleTapOnChild)
            {
                this.PART_CurrentItem.MouseLeftButtonUp += this.OnContentClicked;
            }
            else
            {
                this.PART_Root.MouseLeftButtonUp += this.OnContentClicked;
            }
            
        }

        private void OnContentClicked(object sender, MouseButtonEventArgs e)
        {
            if (this.TapCommand != null)
            {
                this.TapCommand.Execute(this.SelectedItem);
            }
        }
        #endregion
    }
}
