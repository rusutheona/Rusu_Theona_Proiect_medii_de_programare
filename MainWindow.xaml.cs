using ChloeShopModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rusu_Theona_Proiect_medii_de_programare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        ChloeShopEntitiesModel ctx = new ChloeShopModel.ChloeShopEntitiesModel();
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerViewSource;
        CollectionViewSource customerOrdersViewSource;

        Binding nameTextBoxBinding = new Binding();
        Binding sizeTextBoxBinding = new Binding();
        Binding colorTextBoxBinding = new Binding();
        Binding priceTextBoxBinding = new Binding();
        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.ProductId
                 equals inv.ProductId
                             select new
                             {
                                 ord.OrderId,
                                 ord.ProductId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 cust.Address,
                                 cust.Email,
                                 cust.PhoneNumber,
                                 inv.Name,
                                 inv.Color,
                                 inv.Size,
                                 inv.Price

                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }



        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            nameTextBoxBinding.Source = inventoryViewSource;
            nameTextBoxBinding.Path = new PropertyPath("Name");
            nameTextBoxBinding.Mode = BindingMode.TwoWay;
            nameTextBox.SetBinding(TextBox.TextProperty, nameTextBoxBinding);

            sizeTextBoxBinding.Source = inventoryViewSource;
            sizeTextBoxBinding.Path = new PropertyPath("Size");
            sizeTextBoxBinding.Mode = BindingMode.TwoWay;
            sizeTextBox.SetBinding(TextBox.TextProperty, sizeTextBoxBinding);

            colorTextBoxBinding.Source = inventoryViewSource;
            colorTextBoxBinding.Path = new PropertyPath("Color");
            colorTextBoxBinding.Mode = BindingMode.TwoWay;
            colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);

            priceTextBoxBinding.Source = inventoryViewSource;
            priceTextBoxBinding.Path = new PropertyPath("Price");
            priceTextBoxBinding.Mode = BindingMode.TwoWay;
            priceTextBox.SetBinding(TextBox.TextProperty, priceTextBoxBinding);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            inventoryViewSource = (System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource"));
            inventoryViewSource.Source = ctx.Inventories.Local;

            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;

            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));

            //customerOrdersViewSource.Source = ctx.Orders.Local;
            BindDataGrid();
            // Load data by setting the CollectionViewSource.Source property:
            // inventoryViewSource.Source = [generic data source]
            ctx.Inventories.Load();
            ctx.Customers.Load();
            ctx.Orders.Load();

            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";
            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Name";
            cmbInventory.SelectedValuePath = "ProductId";


        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {

                    inventory = new Inventory()
                    {
                        Name = nameTextBox.Text.Trim(),
                        Size = sizeTextBox.Text.Trim(),
                        Color = colorTextBox.Text.Trim(),
                        Price = priceTextBox.Text.Trim()
                    };

                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();

                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
       if (action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Name = nameTextBox.Text.Trim();
                    inventory.Size = sizeTextBox.Text.Trim();
                    inventory.Color = colorTextBox.Text.Trim();
                    inventory.Price = priceTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                inventoryViewSource.View.MoveCurrentTo(inventory);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }


        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            nameTextBox.IsEnabled = true;
            sizeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            priceTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(nameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(sizeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(priceTextBox, TextBox.TextProperty);
            nameTextBox.Text = "";
            sizeTextBox.Text = "";
            colorTextBox.Text = "";
            priceTextBox.Text = "";
            Keyboard.Focus(colorTextBox);
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
            nameTextBox.IsEnabled = false;
            sizeTextBox.IsEnabled = false;
            colorTextBox.IsEnabled = false;
            priceTextBox.IsEnabled = false;
            nameTextBox.SetBinding(TextBox.TextProperty, nameTextBoxBinding);
            sizeTextBox.SetBinding(TextBox.TextProperty, sizeTextBoxBinding);
            colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
            priceTextBox.SetBinding(TextBox.TextProperty, priceTextBoxBinding);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempName = nameTextBox.Text.ToString();
            string tempSize = sizeTextBox.Text.ToString();
            string tempColor = colorTextBox.Text.ToString();
            string tempPrice = priceTextBox.Text.ToString();
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            nameTextBox.IsEnabled = true;
            sizeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            priceTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(nameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(sizeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(priceTextBox, TextBox.TextProperty);

            nameTextBox.Text = tempName;
            sizeTextBox.Text = tempSize;
            colorTextBox.Text = tempColor;
            priceTextBox.Text = tempPrice;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempName = nameTextBox.Text.ToString();
            string tempSize = sizeTextBox.Text.ToString();
            string tempColor = colorTextBox.Text.ToString();
            string tempPrice = priceTextBox.Text.ToString();
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            BindingOperations.ClearBinding(nameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(sizeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(priceTextBox, TextBox.TextProperty);
            nameTextBox.Text = tempName;
            sizeTextBox.Text = tempSize;
            colorTextBox.Text = tempColor;
            priceTextBox.Text = tempPrice;
        }

        private void btnSave1_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {

                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim(),
                        Address = addressTextBox.Text.Trim(),
                        Email = emailTextBox.Text.Trim(),
                        PhoneNumber = phoneNumberTextBox.Text.Trim(),
                    };
                    //adaugam entitatea nou creata in context

                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            else
                if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    customer.Address = addressTextBox.Text.Trim();
                    customer.Email = emailTextBox.Text.Trim();
                    customer.PhoneNumber = phoneNumberTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {

                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();

                customerViewSource.View.MoveCurrentTo(customer);
            }
            else if (action == ActionState.Delete)
            {
                try
                {

                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {

                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
            }

        }

        private void btnCancel1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew1.IsEnabled = true;
            btnEdit1.IsEnabled = true;
            btnDelete1.IsEnabled = true;
            btnSave1.IsEnabled = false;
            btnCancel1.IsEnabled = false;
            btnPrev1.IsEnabled = true;
            btnNext1.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;
            addressTextBox.IsEnabled = false;
            emailTextBox.IsEnabled = false;
            phoneNumberTextBox.IsEnabled = false;

        }

        private void btnDelete1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();
            string tempAddress = addressTextBox.Text.ToString();
            string tempEmail = emailTextBox.Text.ToString();
            string tempPhoneNumber = phoneNumberTextBox.Text.ToString();

            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(addressTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(emailTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(phoneNumberTextBox, TextBox.TextProperty);

            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            addressTextBox.Text = tempAddress;
            emailTextBox.Text = tempEmail;
            phoneNumberTextBox.Text = tempPhoneNumber;

        }

        private void btnNew1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;

            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            addressTextBox.IsEnabled = true;
            emailTextBox.IsEnabled = true;
            phoneNumberTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(addressTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(emailTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(phoneNumberTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            addressTextBox.Text = "";
            emailTextBox.Text = "";
            phoneNumberTextBox.Text = "";

            Keyboard.Focus(firstNameTextBox);
        }

        private void btnEdit1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();
            string tempAddress = addressTextBox.Text.ToString();
            string tempEmail = emailTextBox.Text.ToString();
            string tempPhoneNumber = phoneNumberTextBox.Text.ToString();
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            addressTextBox.IsEnabled = true;
            emailTextBox.IsEnabled = true;
            phoneNumberTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(addressTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(emailTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(phoneNumberTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            addressTextBox.Text = tempAddress;
            emailTextBox.Text = tempEmail;
            phoneNumberTextBox.Text = tempPhoneNumber;
        }

        private void btnPrev1_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnSave2_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                    //instantiem Order entity
                    order = new Order()
                    {

                        CustId = customer.CustId,
                        ProductId = inventory.ProductId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    
                    //salvam modificarile
                    ctx.SaveChanges();
                    BindDataGrid();
                    customerOrdersViewSource.View.Refresh();



                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.ProductId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(selectedOrder);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnPrev2_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext2_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }

        private void btnEdit2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            nameTextBox.IsEnabled = true;
            sizeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            priceTextBox.IsEnabled = true;
            Customer customer = (Customer)cmbCustomers.SelectedItem;
            Inventory inventory = (Inventory)cmbInventory.SelectedItem;
            customerOrdersViewSource.View.Refresh();
        }

        private void btnNew2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;

            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            addressTextBox.IsEnabled = true;
            emailTextBox.IsEnabled = true;

            Customer customer = (Customer)cmbCustomers.SelectedItem;
            Inventory inventory = (Inventory)cmbInventory.SelectedItem;

            customerOrdersViewSource.View.Refresh();
        }

        private void btnDelete2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;

        }

        private void btnCancel2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew2.IsEnabled = true;
            btnEdit2.IsEnabled = true;
            btnDelete2.IsEnabled = true;
            btnSave2.IsEnabled = false;
            btnCancel2.IsEnabled = false;
            btnPrev2.IsEnabled = true;
            btnNext2.IsEnabled = true;
            colorTextBox.IsEnabled = false;
            priceTextBox.IsEnabled = false;
        }
    }
}
