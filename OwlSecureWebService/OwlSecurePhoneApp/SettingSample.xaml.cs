/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace PhoneApp1
{
    public partial class SettingSample : PhoneApplicationPage
    {
        PhotoChooserTask photoChooserTask;

        //AppSettings settings = new AppSettings();

        public SettingSample()
        {
            InitializeComponent();
            slider.Value = App.settings.SyncTimeSettings;
            synctime.Text = Convert.ToInt32(slider.Value).ToString();
            this.photoChooserTask = new PhotoChooserTask();
            this.photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed); 
        }
        
        // Event handler to show the slider value in text block
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider != null)
            {
                synctime.Text = Convert.ToInt32(slider.Value).ToString();
                App.settings.SyncTimeSettings = Convert.ToInt32(slider.Value);
            }
        }

        // Show the photo chooser when browse button is clicked
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            photoChooserTask.Show(); 
        }

        // Change the background of the phone when photo is chosen
        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {            
            //App.settings.BackGroundSettings = e.ChosenPhoto;
            App.SetBackground(e.ChosenPhoto);
        } 

    }
}
