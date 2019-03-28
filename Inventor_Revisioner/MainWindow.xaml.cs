using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using Inventor;

namespace Inventor_Revisioner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Inventor.Application _invApp;
        private InventorObject _currentInventorObject;


        public MainWindow()
        {
            InitializeComponent();
            try
            {
                _invApp = (Inventor.Application)Marshal.GetActiveObject("Inventor.Application");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Inventor ist nicht gestartet. Bitte zuerst Inventor starten.");
                this.Close();
            }
        }

        private void cmdRevisionize_Click(object sender, RoutedEventArgs e)
        {
            this._currentInventorObject?.CopyAndReplace();
            this.Close();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Topmost = true;
            if (Utility.DocumentChecker(Utility.DataTypes.Assembly, _invApp))
            {
                this._currentInventorObject = new Assembly(_invApp);
                cmdRevisionize.Content = "Revisionier Baugruppe";
            }
            else if (Utility.DocumentChecker(Utility.DataTypes.Part, _invApp))
            {
                this._currentInventorObject = new Part(_invApp);
                cmdRevisionize.Content = "Revisionier Bauteil";
            }
            else
            {
                MessageBox.Show("Kann nur in einer Baugruppe oder Bauteil ausgeführt werden.");
                this.Close();
                return;
            }
            this._currentInventorObject.UpdateInformation();

            // Label assignments
            lblDocumentName.Text = _currentInventorObject.DocumentName;
            lblIsDrawing.Text = _currentInventorObject.HasDrawing ? "Ja" : "Nein";
            lblHasRevision.Text = _currentInventorObject.HasRevision ? "Ja" : "Nein";
            lblNextRevision.Text = _currentInventorObject.NextRevisionNumber;

        }
    }
}
