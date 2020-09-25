using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControls
{
    /// <summary>
    /// PagerControl.xaml 的交互逻辑
    /// </summary>
    public partial class PagerControl : UserControl
    {
        public PagerControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register("Total",
            typeof(int),
            typeof(PagerControl),
            new FrameworkPropertyMetadata(0,
              FrameworkPropertyMetadataOptions.None,
              new PropertyChangedCallback(OnTotalChanged)));

        public static void OnTotalChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var pagerControl = (PagerControl)dp;
            pagerControl.BuidNumerButton();
            pagerControl.totalTb.Text = e.NewValue.ToString();
        }

        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex",
            typeof(int),
            typeof(PagerControl),
            new FrameworkPropertyMetadata(1,
              FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
              new PropertyChangedCallback(OnPageIndexChanged)));

        public static void OnPageIndexChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var pagerControl = (PagerControl)dp;
            pagerControl.BuidNumerButton();
        }

        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize",
            typeof(int),
            typeof(PagerControl),
            new FrameworkPropertyMetadata(0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnPageSizeChanged)));

        public static void OnPageSizeChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var pagerControl = (PagerControl)dp;
            pagerControl.BuidNumerButton();

            pagerControl.UpdateComboBoxText(e.NewValue.ToString());
        }

        public static readonly DependencyProperty NumberButtonCountProperty =
          DependencyProperty.Register("NumberButtonCount",
          typeof(int),
          typeof(PagerControl),
          new FrameworkPropertyMetadata(4,
              FrameworkPropertyMetadataOptions.None,
              new PropertyChangedCallback(OnNumberButtonCountChanged)));

        public static void OnNumberButtonCountChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (int)e.NewValue;
            if (newValue < 4)
            {
                ((PagerControl)dp).NumberButtonCount = 4;
            }
        }
        /// <summary>
        /// 总条数
        /// </summary>
        public int Total
        {
            get { return (int)GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        /// <summary>
        /// 要显示数值按钮个数，默认值为4， 最少3个
        /// </summary>
        public int NumberButtonCount
        {
            get { return (int)GetValue(NumberButtonCountProperty); }
            set { SetValue(NumberButtonCountProperty, value); }
        }

        /// <summary>
        /// 生成数字按钮
        /// </summary>
        private void BuidNumerButton()
        {
            if (PageSize == 0) return;

            // 最大页数
            int maxPageNumber = Total / PageSize + 1;
            if (Total % PageSize == 0) maxPageNumber--;

            // 需要显示fill
            bool needFill = maxPageNumber > NumberButtonCount + PageIndex - 1;

            // 需要的按钮数
            int needButtonCount = needFill ? NumberButtonCount : (maxPageNumber <= NumberButtonCount ? maxPageNumber : NumberButtonCount);

            // 已存在的按钮数
            int existButtonCount = 0;
            for (int i = 0; i < numberBtnStackPanel.Children.Count; i++)
            {
                if (numberBtnStackPanel.Children[i] is Button) existButtonCount++;
            }

            // 计算开始页码
            int startPageNumber = 1;
            if (!needFill)
            {
                startPageNumber = maxPageNumber - needButtonCount + 1;
            }
            else
            {
                if (numberBtnStackPanel.Children.Count > 0)
                {
                    startPageNumber = (int)((Button)numberBtnStackPanel.Children[0]).Content;
                }

                // 页码在前面
                if (PageIndex <= startPageNumber)
                {
                    if (PageIndex <= (needButtonCount - 1))
                    {
                        startPageNumber = 1;
                    }
                    else
                    {
                        startPageNumber = PageIndex + 1 - (needButtonCount - 2);
                    }
                }
                // 页码在后面
                else if (PageIndex >= startPageNumber + existButtonCount - 2)
                {
                    if (maxPageNumber - PageIndex > NumberButtonCount)
                        startPageNumber = PageIndex - 1;
                    else startPageNumber = maxPageNumber - NumberButtonCount + 1;
                }
            }

            // 生成数字按钮
            if (existButtonCount != needButtonCount)
            {
                var temp = new List<UIElement>();
                //var numberPageBtnStyle = (Style)this.FindResource("NumberPageBtnStyle");
                for (int i = 0; i < needButtonCount; i++)
                {
                    var button = new Button();// { Style = numberPageBtnStyle };
                    button.Click += PageBtn_Click;
                    temp.Add(button);
                }

                numberBtnStackPanel.Children.Clear();
                temp.ForEach(element => numberBtnStackPanel.Children.Add(element));
            }


            // 生成Fill 文本框
            UIElement fillElement = null;
            for (int i = 0; i < numberBtnStackPanel.Children.Count; i++)
            {
                var child = numberBtnStackPanel.Children[i];
                if (child is TextBlock) { fillElement = child; break; }
            }

            if (needFill && fillElement == null)
            {
                var fillTextBlack = new TextBlock() { Text = "..." };
                numberBtnStackPanel.Children.Insert(needButtonCount - 1, fillTextBlack);
            }
            else if (!needFill && fillElement != null)
            {
                numberBtnStackPanel.Children.Remove(fillElement);
            }


            //更新按钮上的数字和选中状态
            for (int i = 0; i < numberBtnStackPanel.Children.Count; i++)
            {
                if (numberBtnStackPanel.Children[i] is Button)
                {
                    var button = (Button)numberBtnStackPanel.Children[i];
                    button.Tag = false;
                    button.Content = startPageNumber + i;
                    // 最后一个一定是按钮，数字一定是最大页数
                    if (i == numberBtnStackPanel.Children.Count - 1)
                    {
                        button.Content = maxPageNumber;
                    }
                    // 高亮
                    if ((int)button.Content == PageIndex)
                    {
                        button.Tag = true;
                    }
                }
            }
        }

        /// <summary>
        /// 页码按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PageBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            PageIndex = (int)button.Content;
        }

        private void FirstPageBtn_Click(object sender, RoutedEventArgs e)
        {
            PageIndex = 1;
        }

        private void PrePageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PageIndex > 1)
                PageIndex--;
        }

        private void NextPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PageIndex * PageSize < Total)
                PageIndex++;
        }

        private void LastPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PageSize == 0) return;

            if (Total % PageSize == 0)
                PageIndex = Total / PageSize;
            else
                PageIndex = Total / PageSize + 1;
        }

        private void GotoPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key > Key.D9 || e.Key < Key.D0)
            {
                e.Handled = true;
            }

            if (e.Key == Key.Enter)
            {
                var textBox = (TextBox)sender;
                int gotoPageIndex = 0;
                if (int.TryParse(textBox.Text, out gotoPageIndex))
                {
                    if ((gotoPageIndex - 1) * PageSize < Total)
                        PageIndex = gotoPageIndex;
                }
            }
        }


        private bool enableSelectionChanged = true;

        public void UpdateComboBoxText(string pageSize)
        {
            enableSelectionChanged = false;
            pageSizeCombo.Text = pageSize;
            enableSelectionChanged = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!enableSelectionChanged) return;
            var comboBox = (ComboBox)sender;
            var comboBoxItem = (ComboBoxItem)comboBox.SelectedValue;
            PageSize = int.Parse(comboBoxItem.Content.ToString());
        }

    }
}
