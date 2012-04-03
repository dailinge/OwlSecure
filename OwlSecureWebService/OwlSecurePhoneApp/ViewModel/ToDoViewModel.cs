using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

// Directive for the data model.
using LocalDatabaseSample.Model;


namespace LocalDatabaseSample.ViewModel
{
    public class ToDoViewModel : INotifyPropertyChanged
    {
        // LINQ to SQL data context for the local database.
        private ToDoDataContext toDoDB;

        // Class constructor, create the data context object.
        public ToDoViewModel(string toDoDBConnectionString)
        {
            toDoDB = new ToDoDataContext(toDoDBConnectionString);
        }

        //
        // TODO: Add collections, list, and methods here.
        //

        // Write changes in the data context to the database.
        public void SaveChangesToDB()
        {
            toDoDB.SubmitChanges();
        }

        // All to-do items.
        private ObservableCollection<ToDoItem> _allToDoItems;
        public ObservableCollection<ToDoItem> AllToDoItems
        {
            get { return _allToDoItems; }
            set
            {
                _allToDoItems = value;
                NotifyPropertyChanged("AllToDoItems");
            }
        }

        // To-do items associated with the home category.
        private ObservableCollection<ToDoItem> _accountEntryItems;
        public ObservableCollection<ToDoItem> AccountEntryItems
        {
            get { return _accountEntryItems; }
            set
            {
                _accountEntryItems = value;
                NotifyPropertyChanged("AccountEntryItems");
            }
        }

        // To-do items associated with the work category.
        private ObservableCollection<ToDoItem> _personalEntryItems;
        public ObservableCollection<ToDoItem> PersonalEntryItems
        {
            get { return _personalEntryItems; }
            set
            {
                _personalEntryItems = value;
                NotifyPropertyChanged("PersonalEntryItems");
            }
        }

        // To-do items associated with the hobbies category.
        private ObservableCollection<ToDoItem> _customEntryItems;
        public ObservableCollection<ToDoItem> CustomEntryItems
        {
            get { return _customEntryItems; }
            set
            {
                _customEntryItems = value;
                NotifyPropertyChanged("CustomEntryItems");
            }
        }

        // A list of all categories, used by the add task page.
        private List<ToDoCategory> _categoriesList;
        public List<ToDoCategory> CategoriesList
        {
            get { return _categoriesList; }
            set
            {
                _categoriesList = value;
                NotifyPropertyChanged("CategoriesList");
            }
        }
        // Query database and load the collections and list used by the pivot pages.
        public void LoadCollectionsFromDatabase()
        {

            // Specify the query for all to-do items in the database.
            var toDoItemsInDB = from ToDoItem todo in toDoDB.Items
                                select todo;

            // Query the database and load all to-do items.
            AllToDoItems = new ObservableCollection<ToDoItem>(toDoItemsInDB);

            // Specify the query for all categories in the database.
            var toDoCategoriesInDB = from ToDoCategory category in toDoDB.Categories
                                     select category;


            // Query the database and load all associated items to their respective collections.
            foreach (ToDoCategory category in toDoCategoriesInDB)
            {
                switch (category.Name)
                {
                    case "Game info":
                        AccountEntryItems = new ObservableCollection<ToDoItem>(category.ToDos);
                        break;
                    case "Finance info":
                        PersonalEntryItems = new ObservableCollection<ToDoItem>(category.ToDos);
                        break;
                    case "Customized info":
                        CustomEntryItems = new ObservableCollection<ToDoItem>(category.ToDos);
                        break;
                    default:
                        break;
                }
            }

            // Load a list of all categories.
            CategoriesList = toDoDB.Categories.ToList();
        }

        // Remove a to-do task item from the database and collections.
        public void DeleteToDoItem(ToDoItem toDoForDelete)
        {

            // Remove the to-do item from the "all" observable collection.
            AllToDoItems.Remove(toDoForDelete);

            // Remove all the fields related to the item
            List<ToDoField> itemFields = LoadItem(toDoForDelete.ItemName);
            toDoDB.Fields.DeleteAllOnSubmit(itemFields);
            // Remove the to-do item from the data context.
            toDoDB.Items.DeleteOnSubmit(toDoForDelete);

            // Remove the to-do item from the appropriate category.   
            switch (toDoForDelete.Category.Name)
            {
                case "Game info":
                    AccountEntryItems.Remove(toDoForDelete);
                    break;
                case "Finance info":
                    PersonalEntryItems.Remove(toDoForDelete);
                    break;
                case "Customized info":
                    CustomEntryItems.Remove(toDoForDelete);
                    break;
                default:
                    break;
            }

            // Save changes to the database.
            toDoDB.SubmitChanges();
        }

        // Add a to-do item to the database and collections.
        public void AddToDoItem(ToDoItem newToDoItem)
        {
            // Add a to-do item to the data context.
            toDoDB.Items.InsertOnSubmit(newToDoItem);

            // Save changes to the database.
            toDoDB.SubmitChanges();

            // Add a to-do item to the "all" observable collection.
            AllToDoItems.Add(newToDoItem);

            // Add a to-do item to the appropriate filtered collection.
            switch (newToDoItem.Category.Name)
            {
                case "Game info":
                    AccountEntryItems.Add(newToDoItem);
                    break;
                case "Finance info":
                    PersonalEntryItems.Add(newToDoItem);
                    break;
                case "Customized info":
                    CustomEntryItems.Add(newToDoItem);
                    break;
                default:
                    break;
            }
        }

        public void AddToDoField(List<ToDoField> newToDoFields)
        {

            // insert a list of fields into the database
            toDoDB.Fields.InsertAllOnSubmit(newToDoFields);
            toDoDB.SubmitChanges();
            
        }

        public List<ToDoField> LoadItem(string itemName)
        {
            var loadItemDB = from ToDoItem item in toDoDB.Items
                             where item.ItemName == itemName
                             select item; 
                                

            List<ToDoItem> loadItem = new List<ToDoItem>(loadItemDB);

            var itemFields = from ToDoField field in toDoDB.Fields
                             where field.Item.Equals(loadItem[0])
                             select field;

            return new List<ToDoField>(itemFields);

        }

        public ToDoItem GetItem(string itemName)
        {
            return toDoDB.Items.First(item => item.ItemName == itemName);
        }

        public void UpdateItem(IDictionary<string, string> itemFieldValue, string itemName)
        {
            ToDoItem currentItem = toDoDB.Items.First(item => item.ItemName == itemName);
            
            foreach (KeyValuePair<string, string> fieldValue in itemFieldValue)
            {
                ToDoField currentField = toDoDB.Fields.First(field => field.Item.Equals(currentItem) && field.FieldName == fieldValue.Key);
                currentField.FieldValue = fieldValue.Value;
            }
            toDoDB.SubmitChanges();
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }



}
