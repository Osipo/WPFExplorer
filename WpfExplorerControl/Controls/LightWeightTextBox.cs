using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfExplorerControl.Extensions;

namespace WpfExplorerControl.Control
{ 
    public class LightWeightTextBox : System.Windows.Controls.TextBox, IScrollInfo
    {

        static LightWeightTextBox()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in appropirate theme
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(LightWeightTextBox), new FrameworkPropertyMetadata(typeof(LightWeightTextBox)));
        }

      
        public LightWeightTextBox() : base()
        {
            this.ClearEventHandlers();
        }



        #region IScrollInfo implementation
        /// <summary>
        /// Size of the document, in pixels.
        /// </summary>
        Size scrollExtent;

        /// <summary>
        /// Offset of the scroll position.
        /// </summary>
        Vector scrollOffset;

        /// <summary>
        /// Size of the viewport.
        /// </summary>
        Size scrollViewport;

        void ClearScrollData()
        {
            SetScrollData(new Size(), new Size(), new Vector());
        }

        bool SetScrollData(Size viewport, Size extent, Vector offset)
        {
            if (!(viewport.IsClose(this.scrollViewport)
                  && extent.IsClose(this.scrollExtent)
                  && offset.IsClose(this.scrollOffset)))
            {
                this.scrollViewport = viewport;
                this.scrollExtent = extent;
                SetScrollOffset(offset);
                this.OnScrollChange();
                return true;
            }
            return false;
        }

        void OnScrollChange()
        {
            ScrollViewer scrollOwner = ((IScrollInfo)this).ScrollOwner;
            if (scrollOwner != null)
            {
                scrollOwner.InvalidateScrollInfo();
            }
        }

        bool canVerticallyScroll;
        bool IScrollInfo.CanVerticallyScroll
        {
            get { return canVerticallyScroll; }
            set
            {
                if (canVerticallyScroll != value)
                {
                    canVerticallyScroll = value;
                    InvalidateMeasure(DispatcherPriority.Normal);
                }
            }
        }
        bool canHorizontallyScroll;
        bool IScrollInfo.CanHorizontallyScroll
        {
            get { return canHorizontallyScroll; }
            set
            {
                if (canHorizontallyScroll != value)
                {
                    canHorizontallyScroll = value;
                    //ClearVisualLines();
                    InvalidateMeasure(DispatcherPriority.Normal);
                }
            }
        }

        double IScrollInfo.ExtentWidth
        {
            get { return scrollExtent.Width; }
        }

        double IScrollInfo.ExtentHeight
        {
            get { return scrollExtent.Height; }
        }

        double IScrollInfo.ViewportWidth
        {
            get { return scrollViewport.Width; }
        }

        double IScrollInfo.ViewportHeight
        {
            get { return scrollViewport.Height; }
        }

        /// <summary>
        /// Gets the horizontal scroll offset.
        /// </summary>
        public double HorizontalOffset
        {
            get { return scrollOffset.X; }
        }

        /// <summary>
        /// Gets the vertical scroll offset.
        /// </summary>
        public double VerticalOffset
        {
            get { return scrollOffset.Y; }
        }

        /// <summary>
        /// Gets the scroll offset;
        /// </summary>
        public Vector ScrollOffset
        {
            get { return scrollOffset; }
        }

        /// <summary>
        /// Occurs when the scroll offset has changed.
        /// </summary>
        public event EventHandler ScrollOffsetChanged;

        void SetScrollOffset(Vector vector)
        {
            if (!canHorizontallyScroll)
                vector.X = 0;
            if (!canVerticallyScroll)
                vector.Y = 0;

            if (!scrollOffset.IsClose(vector))
            {
                scrollOffset = vector;
                if (ScrollOffsetChanged != null)
                    ScrollOffsetChanged(this, EventArgs.Empty);
            }
        }

        ScrollViewer IScrollInfo.ScrollOwner { get; set; }

        void IScrollInfo.LineUp()
        {
            ((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y - DefaultLineHeight);
        }

        void IScrollInfo.LineDown()
        {
            ((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y + DefaultLineHeight);
        }

        void IScrollInfo.LineLeft()
        {
            ((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X - WideSpaceWidth);
        }

        void IScrollInfo.LineRight()
        {
            ((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X + WideSpaceWidth);
        }

        void IScrollInfo.PageUp()
        {
            ((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y - scrollViewport.Height);
        }

        void IScrollInfo.PageDown()
        {
            ((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y + scrollViewport.Height);
        }

        void IScrollInfo.PageLeft()
        {
            ((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X - scrollViewport.Width);
        }

        void IScrollInfo.PageRight()
        {
            ((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X + scrollViewport.Width);
        }

        void IScrollInfo.MouseWheelUp()
        {
            ((IScrollInfo)this).SetVerticalOffset(
                scrollOffset.Y - (SystemParameters.WheelScrollLines * DefaultLineHeight));
            OnScrollChange();
        }

        void IScrollInfo.MouseWheelDown()
        {
            ((IScrollInfo)this).SetVerticalOffset(
                scrollOffset.Y + (SystemParameters.WheelScrollLines * DefaultLineHeight));
            OnScrollChange();
        }

        void IScrollInfo.MouseWheelLeft()
        {
            ((IScrollInfo)this).SetHorizontalOffset(
                scrollOffset.X - (SystemParameters.WheelScrollLines * WideSpaceWidth));
            OnScrollChange();
        }

        void IScrollInfo.MouseWheelRight()
        {
            ((IScrollInfo)this).SetHorizontalOffset(
                scrollOffset.X + (SystemParameters.WheelScrollLines * WideSpaceWidth));
            OnScrollChange();
        }

        bool defaultTextMetricsValid;
        double wideSpaceWidth; // Width of an 'x'. Used as basis for the tab width, and for scrolling.
        double defaultLineHeight; // Height of a line containing 'x'. Used for scrolling.
        double defaultBaseline; // Baseline of a line containing 'x'. Used for TextTop/TextBottom calculation.

        /// <summary>
        /// Gets the width of a 'wide space' (the space width used for calculating the tab size).
        /// </summary>
        /// <remarks>
        /// This is the width of an 'x' in the current font.
        /// We do not measure the width of an actual space as that would lead to tiny tabs in
        /// some proportional fonts.
        /// For monospaced fonts, this property will return the expected value, as 'x' and ' ' have the same width.
        /// </remarks>
        public double WideSpaceWidth
        {
            get
            {
                CalculateDefaultTextMetrics();
                return wideSpaceWidth;
            }
        }

        /// <summary>
        /// Gets the default line height. This is the height of an empty line or a line containing regular text.
        /// Lines that include formatted text or custom UI elements may have a different line height.
        /// </summary>
        public double DefaultLineHeight
        {
            get
            {
                CalculateDefaultTextMetrics();
                return defaultLineHeight;
            }
        }

        /// <summary>
        /// Gets the default baseline position. This is the difference between <see cref="VisualYPosition.TextTop"/>
        /// and <see cref="VisualYPosition.Baseline"/> for a line containing regular text.
        /// Lines that include formatted text or custom UI elements may have a different baseline.
        /// </summary>
        public double DefaultBaseline
        {
            get
            {
                CalculateDefaultTextMetrics();
                return defaultBaseline;
            }
        }

        void InvalidateDefaultTextMetrics()
        {
            defaultTextMetricsValid = false;
        }

        void CalculateDefaultTextMetrics()
        {
            if (defaultTextMetricsValid)
                return;

            defaultTextMetricsValid = true;
            wideSpaceWidth = FontSize / 2;
            defaultBaseline = FontSize;
            defaultLineHeight = FontSize + 3;
        }

        static double ValidateVisualOffset(double offset)
        {
            if (double.IsNaN(offset))
                throw new ArgumentException("offset must not be NaN");
            if (offset < 0)
                return 0;
            else
                return offset;
        }

        void IScrollInfo.SetHorizontalOffset(double offset)
        {
            offset = ValidateVisualOffset(offset);
            if (!scrollOffset.X.IsClose(offset))
            {
                SetScrollOffset(new Vector(offset, scrollOffset.Y));
                InvalidateVisual();
            }
        }

        void IScrollInfo.SetVerticalOffset(double offset)
        {
            offset = ValidateVisualOffset(offset);
            if (!scrollOffset.Y.IsClose(offset))
            {
                SetScrollOffset(new Vector(scrollOffset.X, offset));
                InvalidateMeasure(DispatcherPriority.Normal);
            }
        }

        Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle)
        {
            if (rectangle.IsEmpty || visual == null || visual == this || !this.IsAncestorOf(visual))
            {
                return Rect.Empty;
            }
            // Convert rectangle into our coordinate space.
            GeneralTransform childTransform = visual.TransformToAncestor(this);
            rectangle = childTransform.TransformBounds(rectangle);

            MakeVisible(Rect.Offset(rectangle, scrollOffset));

            return rectangle;
        }

        /// <summary>
        /// Scrolls the text view so that the specified rectangle gets visible.
        /// </summary>
        public virtual void MakeVisible(Rect rectangle)
        {
            Rect visibleRectangle = new Rect(scrollOffset.X, scrollOffset.Y,
                                             scrollViewport.Width, scrollViewport.Height);
            Vector newScrollOffset = scrollOffset;
            if (rectangle.Left < visibleRectangle.Left)
            {
                if (rectangle.Right > visibleRectangle.Right)
                {
                    newScrollOffset.X = rectangle.Left + rectangle.Width / 2;
                }
                else
                {
                    newScrollOffset.X = rectangle.Left;
                }
            }
            else if (rectangle.Right > visibleRectangle.Right)
            {
                newScrollOffset.X = rectangle.Right - scrollViewport.Width;
            }
            if (rectangle.Top < visibleRectangle.Top)
            {
                if (rectangle.Bottom > visibleRectangle.Bottom)
                {
                    newScrollOffset.Y = rectangle.Top + rectangle.Height / 2;
                }
                else
                {
                    newScrollOffset.Y = rectangle.Top;
                }
            }
            else if (rectangle.Bottom > visibleRectangle.Bottom)
            {
                newScrollOffset.Y = rectangle.Bottom - scrollViewport.Height;
            }
            newScrollOffset.X = ValidateVisualOffset(newScrollOffset.X);
            newScrollOffset.Y = ValidateVisualOffset(newScrollOffset.Y);
            if (!scrollOffset.IsClose(newScrollOffset))
            {
                SetScrollOffset(newScrollOffset);
                this.OnScrollChange();
                InvalidateMeasure(DispatcherPriority.Normal);
            }
        }
        #endregion

        #region InvalidateMeasure(DispatcherPriority)
        DispatcherOperation invalidateMeasureOperation;

        void InvalidateMeasure(DispatcherPriority priority)
        {
            if (priority >= DispatcherPriority.Render)
            {
                if (invalidateMeasureOperation != null)
                {
                    invalidateMeasureOperation.Abort();
                    invalidateMeasureOperation = null;
                }
                base.InvalidateMeasure();
            }
            else
            {
                if (invalidateMeasureOperation != null)
                {
                    invalidateMeasureOperation.Priority = priority;
                }
                else
                {
                    invalidateMeasureOperation = Dispatcher.BeginInvoke(
                        priority,
                        new Action(
                            delegate {
                                invalidateMeasureOperation = null;
                                base.InvalidateMeasure();
                            }
                        )
                    );
                }
            }
        }
        #endregion
    }
}
