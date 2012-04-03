using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using OwlSecureWebService.Model;

namespace OwlSecureWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWCFService" in both code and config file together.
    [ServiceContract]
    public interface IWCFService
    {
        [OperationContract]
        void DBInitialize();
        [OperationContract]
        List<ToDoItem> LoadAllItemFromDatabase();
        [OperationContract]
        List<ToDoCategory> LoadAllCategoryFromDatabase();
        [OperationContract]
        void DeleteToDoItem(ToDoItem toDoForDelete);
        [OperationContract]
        void AddToDoItem(ToDoItem newToDoItem);
        [OperationContract]
        void SaveChangesToDB();
    }
}
