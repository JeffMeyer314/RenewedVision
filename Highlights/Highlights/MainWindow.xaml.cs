using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Highlights
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public static StateEnum state = StateEnum.Normal;
        Timer t = new Timer(2000);

        public MainWindow()
        {
            InitializeComponent();
            rtb.TextChanged += Rtb_TextChanged;
        }


        string GetString(RichTextBox rtb)
        {
            var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text;
        }

        private void CheckKeyword(RichTextBox rtb)
        {
            string msg = GetString(rtb);
            HighlightField hf = new HighlightField();
            List<HighlightField> fields = hf.Parse(msg);
            if (fields.Count > 0)
            {
                rtb.Document.Blocks.Clear();
                Paragraph p = new Paragraph();
                rtb.Document.Blocks.Add(p);

                foreach (HighlightField curField in fields)
                {
                    Run r = new Run(curField.Text);
                    switch (curField.State)
                    {
                        case StateEnum.User: r.Foreground = Brushes.Blue; break;
                        case StateEnum.Hash: r.Foreground = Brushes.Red; break;
                        default: r.Foreground = Brushes.Black; break;
                    }
                    p.Inlines.Add(r);
                }
            }
        }

        private void Rtb_TextChanged(object sender, TextChangedEventArgs e)
        {
            rtb.Dispatcher.BeginInvoke(new Action(() =>
            {
                int totalChars = 0;
                int offset = 0;
                foreach ( TextChange change in e.Changes)
                {
                    totalChars = change.AddedLength - change.RemovedLength;
                    offset = change.Offset;
                }
                int savePos = SavePosition(totalChars, offset);
                rtb.TextChanged -= Rtb_TextChanged;
                CheckKeyword(rtb);
                rtb.TextChanged += Rtb_TextChanged;
                getPos(savePos);
            }));

        }

        int SavePosition(int adj, int offset)
        {
            int  so;
            if (rtb.CaretPosition.IsAtInsertionPosition)
            {
                so = offset + adj;
            }
            else
            {
                so = rtb.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward).GetOffsetToPosition(rtb.CaretPosition);
            }
            

            return so;
        }

        void getPos(int so)
        {
            try
            {
                TextPointer tp;
                tp  = rtb.CaretPosition.GetPositionAtOffset(so);
                if (tp.IsAtInsertionPosition)
                {
                    rtb.CaretPosition = tp;
                }
                else
                {
                    rtb.CaretPosition = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                }
            }
            catch (Exception exc)
            {
                string x = exc.Message;
                rtb.CaretPosition = rtb.CaretPosition.DocumentEnd;
            }
        }
    }
}
