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
    public partial class MainWindow
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

        /* CORE FUNCTIONS */

        private void OpenSearchWindow()
        {
            this.searchWindow.Show();
        }

        /// <summary>
        ///     Asking to the user if he want to save his current file, if save is cancelled nothing gonna happen, BUT, If save is failed/successfull,
        ///     the method in param will be executed
        /// </summary>
        /// <param name="methodName">Method to execute</param>
        private void AskForSave(Action methodName)
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



        /* UNUSED FUNCTIONS */

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

    }
}
