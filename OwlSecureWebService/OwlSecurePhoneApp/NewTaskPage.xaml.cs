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

// Directive for the data model.
using LocalDatabaseSample.Model;
using System.Windows.Data;

namespace PhoneApp1
{
    public partial class NewTaskPage : PhoneApplicationPage
    {
        // A list of fields under this item.
        List<ToDoField> fields = new List<ToDoField>();
        // Item information when navigate from tapping on main page.
        // It is the name to load an existing item.
        string LoadedItemInfo;
        // Item loaded from database.
        ToDoItem LoadedItem;
        // The boolean value to indicate whether this page has already been loaded
        bool IsLoaded;
        // The dictionary that maps the names of the existing fields associated with this item in the database 
        // to their textbox
        Dictionary<string, TextBox> UpdateFieldDictionary;

        // The dictionary that maps the names of the new fields to their textbox
        Dictionary<string, TextBox> NewFieldDictionary;

        public NewTaskPage()
        {
            InitializeComponent();
            // Set the page DataContext property to the ViewModel.
            this.DataContext = App.ViewModel; 
        }

        // When navigate from main page, needs to check if it is from adding a new item
        // or loading an existing item. Also needs to see if it is coming back from adding field
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!IsLoaded)
            {
                UpdateFieldDictionary = new Dictionary<string, TextBox>();
                NewFieldDictionary = new Dictionary<string, TextBox>();
                NavigationContext.QueryString.TryGetValue("itemInfo", out LoadedItemInfo);

                if (LoadedItemInfo != null)
                    loadItemPage();

                IsLoaded = true;
            }
  
            else
            {
                //add the new textbox for the new field created and add the field to the new field dictionary
                if (App.tempField.FieldName != null)
                {
                    string newFieldName = App.tempField.FieldName;
                    string newFieldValue = App.tempField.FieldValue;
                    App.cleanTempField();
                    
                    TextBlock newFieldNameBlock = new TextBlock();
                    newFieldNameBlock.Text = newFieldName;
                    ContentPanel.Children.Add(newFieldNameBlock);
                    
                    TextBox newFieldValueBox = new TextBox();
                    newFieldValueBox.Text = newFieldValue;
                    ContentPanel.Children.Add(newFieldValueBox);

                    NewFieldDictionary.Add(newFieldName, newFieldValueBox);

                }
            }
            
        }

        // Add a field box to the view
        private void addFieldBox(ToDoField f)
        {
            TextBlock newFieldNameBlock = new TextBlock();
            newFieldNameBlock.Text = f.FieldName;
            ContentPanel.Children.Add(newFieldNameBlock);
            TextBox newFieldValueBox = new TextBox();
            newFieldValueBox.Text = f.FieldValue;
            ContentPanel.Children.Add(newFieldValueBox);
            UpdateFieldDictionary.Add(f.FieldName, newFieldValueBox);
            //Here is a link on bindings
            // http://www.windowsphonegeek.com/articles/Talking-about-Data-Binding-in-WP7--Coding4fun-TextBoxBinding-helper-in-depth
        }

        // Load the existing item when navigating from main page
        public void loadItemPage()
        {
            PageTitle.Text = LoadedItemInfo;
            newEntryNameTextBox.Text = LoadedItemInfo;

            // Get the list of fields from the load item method. And then fill in the text boxes with 
            // the information of the fields. 

            LoadedItem = App.ViewModel.GetItem(LoadedItemInfo);

            List<ToDoField> itemFields = App.ViewModel.LoadItem(LoadedItemInfo);
            UpdateFieldDictionary.Add("username", newUserNameTextBox);
            UpdateFieldDictionary.Add("password", newPasswordTextBox);
            UpdateFieldDictionary.Add("note", newNoteTextBox);
            foreach (ToDoField field in itemFields)
            {
                switch (field.FieldName)
                {
                    case "username":
                        newUserNameTextBox.Text = field.FieldValue;
                        
                        break;
                    case "password":
                        newPasswordTextBox.Text = field.FieldValue;
                        LoginPassword.Password = newPasswordTextBox.Text;
                        break;
                    case "note":
                        newNoteTextBox.Text = field.FieldValue;
                        break;
                    default:
                        addFieldBox(field);
                        break;
                }
            }
            
        }

        // When ok is clicked, save the item and add to database
        // Make sure all the fields are also stored
        private void appBarOkButton_Click(object sender, EventArgs e)
        {            
            // Confirm there is some text in the text box.
            if (newEntryNameTextBox.Text.Length > 0)
            {
                // Check if this is a new item or existing item
                if (LoadedItemInfo == null)
                {
                    List<ToDoField> NewFields = new List<ToDoField>();
                    // Create a new to-do item.
                    LoadedItem = new ToDoItem
                    {
                        ItemName = newEntryNameTextBox.Text,
                        Category = (ToDoCategory)categoriesListPicker.SelectedItem
                    };

                    ToDoField UsernameField = new ToDoField
                    {
                        FieldName = "username",
                        FieldValue = newUserNameTextBox.Text,
                        Item = LoadedItem
                    };

                    ToDoField PasswordField = new ToDoField
                    {
                        FieldName = "password",
                        FieldValue = newPasswordTextBox.Text,
                        Item = LoadedItem
                    };
                    ToDoField NoteField = new ToDoField
                    {
                        FieldName = "note",
                        FieldValue = newNoteTextBox.Text,
                        Item = LoadedItem
                    };

                    NewFields.Add(UsernameField);
                    NewFields.Add(PasswordField);
                    NewFields.Add(NoteField);

                    // Add the item to the ViewModel.
                    App.ViewModel.AddToDoItem(LoadedItem);
                    App.ViewModel.AddToDoField(NewFields);
                }
                else
                {
                    Dictionary<string, string> fieldValueDict = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, TextBox> updateField in UpdateFieldDictionary) 
                    {
                        fieldValueDict.Add(updateField.Key, updateField.Value.Text);
                    }
                    //fieldValueDict.Add("username", newUserNameTextBox.Text);
                    //fieldValueDict.Add("password", newPasswordTextBox.Text);
                    //fieldValueDict.Add("note", newNoteTextBox.Text);
                    App.ViewModel.UpdateItem(fieldValueDict, LoadedItemInfo);
                }

                List<ToDoField> NewFieldsDup = new List<ToDoField>();
                foreach (KeyValuePair<string, TextBox> newFieldInfo in NewFieldDictionary)
                {
                    ToDoField newField = new ToDoField
                    {
                        FieldName = newFieldInfo.Key,
                        FieldValue = newFieldInfo.Value.Text,
                        Item = LoadedItem
                    };
                    NewFieldsDup.Add(newField);

                }
                App.ViewModel.AddToDoField(NewFieldsDup);


                // Return to the main page.
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        // Go back to main page when cancling adding the item
        private void appBarCancelButton_Click(object sender, EventArgs e)
        {
            // Return to the main page.
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        // Based on different category the display of field names are different
        private void categoriesListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string catagory = ((ToDoCategory)categoriesListPicker.SelectedItem).Name;
            switch (catagory)
            {
                case "Game info":
                    userNameTitle.Text = "Account Name";
                    dataTitle.Text = "Password";
                    break;
                case "Finance info":
                    userNameTitle.Text = "Info Type";
                    dataTitle.Text = "Info Detail";
                    break;
                case "Customized info":
                    userNameTitle.Text = "Data Type";
                    dataTitle.Text = "Data Value";
                    break;
                default:
                    userNameTitle.Text = "Data Type";
                    dataTitle.Text = "Data Value";
                    break;
            }

        }

        // Mask the data value when box is checked
        private void ShowCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (ShowCheck.IsChecked == true)
            {
                newPasswordTextBox.Visibility = System.Windows.Visibility.Visible;
                LoginPassword.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                newPasswordTextBox.Visibility = System.Windows.Visibility.Collapsed;
                LoginPassword.Visibility = System.Windows.Visibility.Visible;
            }
        }

        // Navigate to a new page when adding a field
        private void appBaraAddButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/TextFieldPage.xaml", UriKind.Relative));
        }

        // Show data when box is unchecked
        private void ShowCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ShowCheck.IsChecked == true)
            {
                newPasswordTextBox.Visibility = System.Windows.Visibility.Visible;
                LoginPassword.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                newPasswordTextBox.Visibility = System.Windows.Visibility.Collapsed;
                LoginPassword.Visibility = System.Windows.Visibility.Visible;
            }
        
        }

        // Update all the field boxes in view.
        // Problem: cannot detect change if text box is edited
        // Should try binding
        private void updateFieldBoxes()
        {
            foreach (ToDoField f in fields)
            {
                addFieldBox(f);
            }
        }
    }
}