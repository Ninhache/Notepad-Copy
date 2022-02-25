using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Notepad
{
    /// <summary>
    /// Logique d'interaction pour Research.xaml
    /// </summary>
    public partial class Research : Window
    {

        private MainWindow parent;
        private TextBox searchBox;
        public Research(Window parent)
        {
            InitializeComponent();
            this.searchBox = (TextBox)FindName("SearchedWord");
            this.parent = (MainWindow)parent;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        { 
            e.Cancel = true;
            this.Hide();
        }

        private void OnShowing(object sender, EventArgs e)
        {
            this.searchBox.Text = this.parent.textbox.SelectedText.ToString();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void Click_NextOccurence(object sender, RoutedEventArgs? e)
        {
            
        }
    }
}
