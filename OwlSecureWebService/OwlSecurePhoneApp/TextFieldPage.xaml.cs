using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using LocalDatabaseSample.Model;

namespace PhoneApp1
{
    public partial class TextFieldPage : PhoneApplicationPage
    {

        public TextFieldPage()
        {
            InitializeComponent();
            // Clean the temp field when load the page.
            //App.cleanTempField();
        }

        // Save new field and go back to item page
        private void appBarOkButton_Click(object sender, EventArgs e)
        {
            // Confirm there is some text in the field name box.
            if (fieldNameBox.Text.Length > 0)
            {
                App.tempField.FieldName = fieldNameBox.Text;
                App.tempField.FieldValue = fieldValueBox.Text;
                
                // Return to the previous page.
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            
        }

        // Cancel field and go back to item page.
        private void appBarCancelButton_Click(object sender, EventArgs e)
        {
            // Return to the item page.
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}