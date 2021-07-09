namespace MoneySaver.Common
{
    public static class GlobalConstants
    {
        public const string SystemName = "MoneySaver";

        public const string AdministratorRoleName = "Administrator";

        public const string NullValueOfCategory = "Category Cannot Be Null";

        public const string BadgeColorNotValid = "Entered Badge Color not valid";

        public const string NullValueOfWallet = "Wallet Cannot Be Null";

        public const string InvalidRecordType = "Invalid Record Type";

        public const string RecordSuccessfullyAdded = "The record is successfully added";

        public const string RecordSuccessfullyUpdated = "The record is successfully updated";

        public const string RecordNotExist = "Record with this Id does not exist";

        public const string SuccessfullyAddedRecord = "Record {0} with type {1} and amount {2} successfully added in category {3}";

        public const string SuccessfullyRemovedRecord = "The record is successfully removed";

        public const string ExistingCategory = "Category with this name already exist";

        public const string SuccessfullyAddedCategory = "Category with name {0} added successfully";

        public const string SuccessfullyRemovedCategory = "Category with name {0} removed successfully";

        public const string CategorySuccessfullyUpdated = "The category is successfully updated";

        public const string UnexistingCategory = "Category do not exist";

        public const string WalletNotExist = "Wallet do not exist";

        public const string WalletAlreadyExist = "Wallet with this name already exist";

        public const string NoPermissionForEditWallet = "You have no permissions to see or modify this wallet!";

        public const string NoPermissionForViewOrEditCategory = "You have no permissions to see or modify this category!";

        public const string NoPermissionForViewOrEditRecord = "You have no permissions to see or modify this record!";

        public const string WalletSuccessfullyAdded = "Wallet with name {0} successfully added!";

        public const string WalletSuccessfullyRemoved = "Wallet with name {0} successfully removed!";

        public const string BudgetAlreadyExists = "Budget with name {0} already exists!";

        public const string BudgetSuccessfullyAdded = "Budget with name {0} successfully added!";

        public const string BudgetSuccessfullyRemoved = "Budget with name {0} successfully removed!";

        public const string InvalidBudgetName = "Budget with name {0} doesn't exist in this wallet!";

        public const string InvalidCurrency = "Please select a valid currency!";

        public const string UserNotExist = "User with this id does not exist";

        public const string CurrencyNotExist = "Currency with this id does not exist";

        public const string ActiveListWithThisNameAlreadyExisting = "List with Active status and this name already exist";

        public const string NoPermissionForEditList = "You have no permissions to see or modify this list!";

        public const string NoPermissionForEditListItem = "You have no permissions to see or modify this list item!";

        public const string ListNotExist = "List not exist!";

        public const string ListItemWithThisIdNotExist = "List item with this Id does not exist";

        public const string ListContainsEmptyItems = "List cannot contains empty items";

        public const string InvalidCompanyTicker = "Company ticker is invalid! Please make sure you've entered the ticker correctly and that it is not empty";

        public const string UserIsNotHolder = "User with this Id has no shares of this company";

        public const string NotEnoughQuantity = "You haven't enough stock quantity in this currency by this company!";
    }
}
