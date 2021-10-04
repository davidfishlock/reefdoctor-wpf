using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FlipViewControl
{
    public class AnimationFactory
    {
        public static AnimationFactory Instance
        {
            get
            {
                return new AnimationFactory();
            }
        }

        public Task AnimateTo(FrameworkElement targetControl, double toValue, double fromValue)
        {
            var tcs = new TaskCompletionSource<bool>();

            if (!(targetControl.RenderTransform is TranslateTransform))
            {
                targetControl.RenderTransform = new TranslateTransform();
            }

            var story = this.GetAnimation(targetControl, toValue, fromValue);

            story.Completed += (s, e) =>
            {
                tcs.SetResult(true);
            };

            story.Begin();

            return tcs.Task;
        }

        public Storyboard GetAnimation(DependencyObject target, double to, double from)
        {
            Storyboard story = new Storyboard();
            Storyboard.SetTargetProperty(story, new PropertyPath("(TextBlock.RenderTransform).(TranslateTransform.X)"));
            Storyboard.SetTarget(story, target);

            var doubleAnimation = new DoubleAnimationUsingKeyFrames();

            EasingDoubleKeyFrame fromFrame = new EasingDoubleKeyFrame(from);
            fromFrame.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut };
            fromFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));

            EasingDoubleKeyFrame toFrame = new EasingDoubleKeyFrame(to);
            toFrame.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut };
            toFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200));

            doubleAnimation.KeyFrames.Add(fromFrame);
            doubleAnimation.KeyFrames.Add(toFrame);
            story.Children.Add(doubleAnimation);

            return story;
        }
    }
}
