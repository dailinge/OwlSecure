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
// Directive for the ViewModel.
using LocalDatabaseSample.Model;

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            // Set the page DataContext property to the ViewModel.
            this.DataContext = App.ViewModel;
        }

        // Add a new item to the database
        private void newTaskAppBarButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/NewTaskPage.xaml", UriKind.Relative));
        }

        // Delete an item from the database
        private void deleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            // Cast the parameter as a button.
            var button = sender as Button;

            if (button != null)
            {
                // Get a handle for the to-do item bound to the button.
                ToDoItem toDoForDelete = button.DataContext as ToDoItem;

                App.ViewModel.DeleteToDoItem(toDoForDelete);
            }

            // Put the focus back to the main page.
            this.Focus();
        }

        // Navigate to setting page
        void settingButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingSample.xaml", UriKind.Relative));
        }

        // Save the changes to database when leaving the page
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Save changes to the database.
            App.ViewModel.SaveChangesToDB();
        }

        // Go to the item page when tapping on an item
        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // Has to pass the item name when go to the item page so that the item can be loaded correctly.
            NavigationService.Navigate(new Uri("/NewTaskPage.xaml?itemInfo=" + ((TextBlock)sender).Text, UriKind.Relative));  
        }

    }
}