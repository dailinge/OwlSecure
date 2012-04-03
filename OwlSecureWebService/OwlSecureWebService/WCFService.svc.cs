using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.Linq;
using OwlSecureWebService.Model;

namespace OwlSecureWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WCFService" in code, svc and config file together.
    public class WCFService : IWCFService
    {
        private ToDoDataContext toDoDB;
        
        public void DBInitialize()
        {
            // Specify the local database connection string.
            string DBConnectionString = "Data Source=isostore:/ToDo.sdf";

            // Create the database if it does not exist.
            ToDoDataContext db = new ToDoDataContext(DBConnectionString);
            if (db.DatabaseExists() == false)
            {
                // Create the local database.
                db.CreateDatabase();

                // Prepopulate the categories.
                db.Categories.InsertOnSubmit(new ToDoCategory { Name = "Account info" });
                db.Categories.InsertOnSubmit(new ToDoCategory { Name = "Personal info" });
                db.Categories.InsertOnSubmit(new ToDoCategory { Name = "Customized info" });

                // Save categories to the database.
                db.SubmitChanges();
                toDoDB = db;
            }
            
        }

        public List<ToDoItem> LoadAllItemFromDatabase()
        {
            // Specify the query for all to-do items in the database.
            var toDoItemsInDB = from ToDoItem todo in toDoDB.Items
                                select todo;
            return new List<ToDoItem>(toDoItemsInDB);
        }

        public List<ToDoCategory> LoadAllCategoryFromDatabase()
        {
            // Specify the query for all categories in the database.
            var toDoCategoriesInDB = from ToDoCategory category in toDoDB.Categories
                                     select category;

            return new List<ToDoCategory>(toDoCategoriesInDB);
        }

        public void DeleteToDoItem(ToDoItem toDoForDelete)
        {
            // Remove the to-do item from the data context.
            toDoDB.Items.DeleteOnSubmit(toDoForDelete);

            // Save changes to the database.
            toDoDB.SubmitChanges();
        }

        public void AddToDoItem(ToDoItem newToDoItem)
        {
            // Add a to-do item to the data context.
            toDoDB.Items.InsertOnSubmit(newToDoItem);

            // Save changes to the database.
            toDoDB.SubmitChanges();
        }

        public void SaveChangesToDB()
        {
            toDoDB.SubmitChanges();
        }
    }
}
