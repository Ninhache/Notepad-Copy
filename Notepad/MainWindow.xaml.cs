using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using System.Windows.Input;

namespace Notepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public TextBox textbox { get; }
        private Research searchWindow;

        public MainWindow()
        {
            InitializeComponent();
            this.textbox = (TextBox)this.FindName("actualFile");
            this.searchWindow = new Research(this);
        }

        public void Keybind_new(Object sender, ExecutedRoutedEventArgs e)
        {
            this.Click_newFile(sender, null);
        }

        public void Keybind_date(Object sender, ExecutedRoutedEventArgs e)
        {
            this.AddDate();
        }

        public void Keybind_exit(Object sender, ExecutedRoutedEventArgs e)
        {
            this.Click_Exit(sender, null);
        }
        public void Keybind_search(Object sender, ExecutedRoutedEventArgs e)
        {
            this.Click_searchWord(sender, null);
        }
        
        private void Click_Date(object sender, RoutedEventArgs? e)
        {
            this.AddDate();
        }

        private void Click_selectAll(object sender, RoutedEventArgs? e)
        {
            this.SelectAll();
        }

        private void Click_searchWord(object sender, RoutedEventArgs? e)
        {
            this.searchWindow.Show();
        }

        private void Click_Exit(object sender, RoutedEventArgs? e)
        {

            var result = MessageBox.Show("TU VEUX SAUVEGARDER???",
                        "Save file", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            
            // File isnt empty, ask to 
            if (this.textbox.Text.Length != 0)
            {
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        if (saveFile(this.textbox.Text))
                        {
                            Application.Current.Shutdown();
                        }
                        break;
                    case MessageBoxResult.No:
                        Application.Current.Shutdown();
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }
            }
            
        }
        private void Click_newFile(object sender, RoutedEventArgs? e)
        {
            

            if (this.textbox == null)
            {
                MessageBox.Show("ActualFile equals null");
            }
            else
            {

                // File isnt empty, ask to save
                if(actualFile.Text.Length != 0)
                {
                    var result = MessageBox.Show("TU VEUX SAUVEGARDER???",
                        "Save file", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            if(saveFile(this.textbox.Text))
                            {
                                this.ClearText();
                            }
                            break;
                        case MessageBoxResult.No:
                            this.ClearText();
                            break;
                        case MessageBoxResult.Cancel:
                            break;
                    }
                } 
                else
                {
                    this.ClearText();
                }
            }
        }

        private bool saveFile(String text)
        {

            bool textSaved = false;
            Stream myStream;

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };
            
            if (dialog.ShowDialog() == true)
            {
                if ((myStream = dialog.OpenFile()) != null)
                { 
                    using (StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine(text);
                    }

                    myStream.Close();
                    textSaved = true;
                }
            }
            return textSaved;
        }

        private void AddDate()
        {
            DateTime date = DateTime.Now;
            this.InsertText(date.ToString());
        }

        private void InsertText(String text)
        {
            int caretPos = this.textbox.CaretIndex;
            this.textbox.Text = this.textbox.Text.Insert(this.textbox.CaretIndex, text);
            this.textbox.CaretIndex = caretPos + text.Length;
        }

        private void replace()
        {
            throw new Exception("Not yet implemented");
        }

        private void ReplaceAll()
        {
            this.textbox.Text.Replace(this.textbox.SelectedText.ToString(), "TODO");
        }
        private void CopyText()
        {
            this.textbox.Copy();
        }

        private void CutText()
        {         
            this.textbox.Cut();
        }

        private void PasteText()
        {
            this.textbox.Paste();
        }

        private void SelectAll()
        {
            this.textbox.SelectAll();
        }

        private void ClearText()
        {
            if (this.textbox != null)
            {
                this.textbox.Clear();
            }
        }

        
    }
}
