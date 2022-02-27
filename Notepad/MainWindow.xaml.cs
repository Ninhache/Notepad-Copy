using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using System.Windows.Input;
using System.ComponentModel;

namespace Notepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _changed = false;
        private string _appname = "Bloc-Notes";
        private string _filepath  = "";
        private string _filename = "Sans titre";

        public TextBox textbox { get; }
        private Research searchWindow;

        public MainWindow()
        {
            InitializeComponent();
            this.textbox = (TextBox)this.FindName("actualFile");
            this.searchWindow = new Research(this);
            this.ResetTitle();
        }

        /* EVENT HANDELING */

        /// <summary>
        ///     Event called on windows close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (actualFile.Text.Length != 0 && _changed)
            {
                e.Cancel = true;
                AskForSave(() =>
                {
                    e.Cancel = false;
                });
            }
        }
        
        /// <summary>
        ///     Event called on text input (Also changing title name)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Text_Changed(object sender, TextChangedEventArgs? e)
        {
            if(!_changed)
            {
                this.Title += "*";
                _changed = true;
            }
        }

        /* KEYBIND HANDELING */

        /// <summary>
        ///     Ctrl + N
        ///     Execute OpenNewFile() function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Keybind_new(Object sender, ExecutedRoutedEventArgs e)
        {
            this.OpenNewFile();
        }

        /// <summary>
        ///     F5
        ///     Execute OpenNewFile() function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Keybind_date(Object sender, ExecutedRoutedEventArgs e)
        {
            this.AddDate();
        }

        /// <summary>
        ///     Ctrl + S
        ///     Execute SaveFile() function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Keybind_save(Object sender, ExecutedRoutedEventArgs e)
        {
            this.SaveFile();
        }

        /// <summary>
        ///     Ctrl + L
        ///     Execute Close() function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Keybind_exit(Object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        ///     Ctrl + F
        ///     Execute OpenSearchWindow() function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Keybind_search(Object sender, ExecutedRoutedEventArgs e)
        {
            this.OpenSearchWindow();
        }


        /* CLICK HANDELING */

        /// <summary>
        ///     Execute the AddDate() function on trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Date(object sender, RoutedEventArgs? e)
        {
            this.AddDate();
        }

        /// <summary>
        ///     Execute the CutText() function on trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Cut(object sender, RoutedEventArgs? e)
        {
            this.CutText();
        }

        /// <summary>
        ///     Execute the CopyText() function on trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Copy(object sender, RoutedEventArgs? e)
        {
            this.CopyText();
        }

        /// <summary>
        ///     Execute the PasteText() function on trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Paste(object sender, RoutedEventArgs? e)
        {
            this.PasteText();
        }

        /// <summary>
        ///     Execute the OpenFile() function on trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Open(object sender, RoutedEventArgs? e)
        {
            this.OpenFile();
        }

        /// <summary>
        ///     Execute the SaveFile() function on trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Save(object sender, RoutedEventArgs? e)
        {
            this.SaveFile();
        }

        /// <summary>
        ///     Execute the SelectAll() function on trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_SelectAll(object sender, RoutedEventArgs? e)
        {
            this.SelectAll();
        }

        /// <summary>
        ///     Shows off the search window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_SearchWord(object sender, RoutedEventArgs? e)
        {
            this.OpenSearchWindow();
        }

        /// <summary>
        ///    Execute .Close function, which one, trigger CloseEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Exit(object sender, RoutedEventArgs? e)
        {
            this.Close();
        }

        /// <summary>
        ///     Execute OpenNewFile() function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_NewFile(object sender, RoutedEventArgs? e)
        {
            this.OpenNewFile();
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Edition(object sender, RoutedEventArgs? e)
        { 
            ((MenuItem) this.FindName("menuitem_Paste")).IsEnabled = Clipboard.ContainsText();
            ((MenuItem)this.FindName("menuitem_Cut")).IsEnabled = ((MenuItem)this.FindName("menuitem_Copy")).IsEnabled = textbox.SelectedText.Length > 0;
        }

        /* CORE FUNCTIONS */

        /// <summary>
        ///     Open the search window, this window is able to search occurences of word you gave her
        /// </summary>
        private void OpenSearchWindow()
        {
            this.searchWindow.Show();
        }

        /// <summary>
        ///     Asking to the user if he want to save his current file, if save is cancelled nothing gonna happen, BUT, If save is failed/successfull,
        ///     the method in param will be executed
        /// </summary>
        /// <param name="methodName">Method to execute</param>
        private MessageBoxResult AskForSave(Action methodName)
        {
            var result = MessageBox.Show("Voulez-vous enregistrer les modifications de " + _filename, "Enregistrer", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    SaveStatement save = SaveFile();
                    if (save == SaveStatement.Saved)
                    {
                        methodName();
                    }
                    break;
                case MessageBoxResult.No:
                    methodName();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
            return result;
        }

        /// <summary>
        ///     Open a save file dialog, and permit to user to save his actual file.
        ///     If the user, already choose a path, and the program had the file in memory, the functions wont call the save file dialog, and will directly
        ///     save file inside the saved path
        /// </summary>
        /// <returns></returns>
        private SaveStatement SaveFile()
        {
            SaveStatement save = SaveStatement.NotSaved;
            Stream myStream;

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            if(_filepath.Equals(""))
            {
                if (dialog.ShowDialog() == true)
                {
                    if ((myStream = dialog.OpenFile()) != null)
                    {
                        FileStream fs = (FileStream)myStream;
                        using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                        {
                            sw.WriteLine(this.textbox.Text);
                        }
                        fs.Close();
                        myStream.Close();
                        save = SaveStatement.Saved;
                        this.UpdateTitle(fs);
                        this.ResetTitle();
                        _changed = false;
                    }
                    else
                    {
                        save = SaveStatement.Cancel;
                    }
                }
            } else
            {
                FileStream fs = File.Open(_filepath, FileMode.Open);
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(this.textbox.Text);
                }
                fs.Close();
                this.UpdateTitle(fs);
                this.ResetTitle();
                _changed = false;
            }
            return save;
        }

        /// <summary>
        /// Open
        /// </summary>
        private void OpenNewFile()
        {
            if (this.textbox == null)
            {
                MessageBox.Show("ActualFile equals null");
            }
            else
            {
                if (actualFile.Text.Length != 0 && _changed)
                {
                    AskForSave(() =>
                    {
                        this.ClearText();
                        this._filepath = "";
                        this._filename = "Sans titre";
                        this.ResetTitle();
                        _changed = false;
                    });
                }
            }
        }

        /// <summary>
        ///     Update title name after a save
        /// </summary>
        /// <param name="fs"></param>
        void UpdateTitle(FileStream fs)
        {
            _filepath = fs.Name.ToString();
            string[] splitedVersion = _filepath.Split("\\");

            _filename = splitedVersion[splitedVersion.Length - 1];
        }

        /// <summary>
        ///     Insert today's date at current cursor position
        /// </summary>
        private void AddDate()
        {
            DateTime date = DateTime.Now;
            this.InsertText(date.ToString());
        }

        /// <summary>
        ///     Insert text at the current cursor position, carret is handeled to be set at the end of the inserted text
        /// </summary>
        /// <param name="text">Text to insert</param>
        private void InsertText(String text)
        {
            int caretPos = this.textbox.CaretIndex;
            this.textbox.Text = this.textbox.Text.Insert(this.textbox.CaretIndex, text);
            this.textbox.CaretIndex = caretPos + text.Length;
        }

        /// <summary>
        ///     Select whole text
        /// </summary>
        private void SelectAll()
        {
            this.textbox.SelectAll();
        }

        /// <summary>
        ///     Clear textbox's text
        /// </summary>
        private void ClearText()
        {
            if (this.textbox != null)
            {
                this.textbox.Clear();
            }
        }

        /// <summary>
        ///     Set title to correspond to the file
        ///     For exemple :
        ///     If you open a file called "Foo.md", Windows's Title will be : {AppName} - Foo.md
        /// </summary>
        private void ResetTitle()
        {
            this.Title = _appname + " - " + _filename;
        }


        /// <summary>
        ///     Open a file in the system, the function will ask the user if a file is currently open (and changed)
        /// </summary>
        private void OpenFile()
        {
            Stream myStream;


            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            var result = MessageBoxResult.None;

            if(_changed)
            {
                result = AskForSave(() => { });
                if(result == MessageBoxResult.Cancel || result == MessageBoxResult.None)
                {
                    return;
                }
            }

            if (dialog.ShowDialog() == true)
            {
                
                if ((myStream = dialog.OpenFile()) != null)
                {
                    FileStream fs = (FileStream)myStream;
                    using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8))
                    {
                        this.textbox.Text = sr.ReadToEnd();
                        this.UpdateTitle(fs);
                        this.ResetTitle();
                        _changed = false;
                        sr.Close();
                    }
                    fs.Close();
                }
            }
        }

        /// <summary>
        ///     Copy the selectioned text in the textbox
        /// </summary>
        private void CopyText()
        {
            this.textbox.Copy();
        }

        /// <summary>
        ///     Cut the selectioned text in the textbox
        /// </summary>
        private void CutText()
        {
            this.textbox.Cut();
        }

        /// <summary>
        ///     Paste the current clipboard text in the textbox ONLY TEXT !
        /// </summary>
        private void PasteText()
        {
            this.textbox.Paste();
        }


        /* UNUSED FUNCTIONS */

        private void replace()
        {
            throw new Exception("Not yet implemented");
        }

        private void ReplaceAll()
        {
            this.textbox.Text.Replace(this.textbox.SelectedText.ToString(), "TODO");
        }

    }
}