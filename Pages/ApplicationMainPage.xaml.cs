using MySklad.Helpers.Connecting;
using MySklad.Models;
using System;
using System.Collections.Generic;
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

namespace MySklad.Pages
{
    /// <summary>
    /// Логика взаимодействия для ApplicationMainPage.xaml
    /// </summary>
    public partial class ApplicationMainPage : Page
    {
        private List<OrganizationItems> organizationItems;
        private List<Category> categoriesList;
        private List<Organization> organizationList;

        private string inventoryNumber;
        private long organizationId;
        private int itemCategoryId;
        public ApplicationMainPage()
        {
            InitializeComponent();

            LoadDataItems();
            LoadDataCategories();
            LoadDataOrganization();


            LoadDataCmbOrganizations();
            LoadDataCmbCategories();

        }

        private void LoadDataCmbOrganizations()
        {
            CmbOrganization.ItemsSource = Connecting.conn.Organization.ToList();
        }
        private void LoadDataCmbCategories()
        {
            CmbCategory.ItemsSource = categoriesList.ToList();
        }

        private void LoadDataItems()
        {
            try
            {
                organizationItems = Connecting.conn.OrganizationItems.ToList();
                DgItems.ItemsSource = organizationItems;
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void TxbSearrch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string txtSearch = TxbSearrch.Text.ToLower();
                if (txtSearch != null && txtSearch != "")
                {
                    var result = organizationItems.Where(x => x.Item.InventoryNmber.ToLower().Contains(txtSearch) || x.Item.Name.ToLower().Contains(txtSearch)).ToList();
                    DgItems.ItemsSource = result;
                }
                else
                {
                    LoadDataItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            
        }
        #region
        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result =  CheckInputData();

                if (result != false)
                {
                    inventoryNumber = TxbInventoryNumber.Text;
                    organizationId = Convert.ToInt64(CmbOrganization.SelectedValue);
                    itemCategoryId = Convert.ToInt32(CmbCategory.SelectedValue);

                    var result2 = Connecting.conn.OrganizationItems.FirstOrDefault(x => x.OrganizationId == organizationId && x.Item.InventoryNmber == inventoryNumber);

                    if (result2 != null)
                    {
                        MessageBox.Show("У этой организаци уже есть предмет с таким инвертарным номером.\nВведите другой инвентарный номер", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Item item = new Item()
                        {
                            InventoryNmber = TxbInventoryNumber.Text,
                            Name = TxbItemName.Text,
                            CategoryId = itemCategoryId,

                        };
                        Connecting.conn.Item.Add(item);
                        Connecting.conn.SaveChanges();
                        var itemId = item.Id;


                        OrganizationItems organizationItem = new OrganizationItems()
                        {
                            ItemId = itemId,
                            OrganizationId = organizationId,
                        };
                        Connecting.conn.OrganizationItems.Add(organizationItem);
                        Connecting.conn.SaveChanges();

                        MessageBox.Show("Тест успешно добавлен", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadDataItems();
                    }
                }
                else
                {
                    return;
                }
               
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion #
        private void BtnDeleteNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var note = DgItems.SelectedItem as OrganizationItems;

                if (note != null)
                {
                    var result = MessageBox.Show("Вы действительно хотите удалить выбранную запись?", "Подтвердите", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Connecting.conn.OrganizationItems.Remove(note);
                        //Connecting.conn.Items.Remove(note.InventaryNumber);

                        Connecting.conn.SaveChanges();

                        MessageBox.Show("Запись успешно удалена", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDataItems();
                    }
                    return;
                }
                else
                {
                    MessageBox.Show("Удаление невозможно.\nНе выбрана строка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private bool CheckInputData()
        {
            if (TxbItemName.Text != null && TxbItemName.Text != "" && 
                TxbInventoryNumber.Text != null && TxbInventoryNumber.Text != "" &&  
                CmbCategory.SelectedIndex == -1 && CmbOrganization.SelectedIndex == -1)
            {
                MessageBox.Show("Заполнены не все поля", "Ошибка", MessageBoxButton.OK,MessageBoxImage.Warning);
                return false;
            }
            else
            {
                return true;
            }
        }

        private void LoadDataCategories()
        {
            try
            {
                var data = Connecting.conn.Category.ToList();
                categoriesList = data;
                CategoriesList.ItemsSource = categoriesList;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }

        private void LoadDataOrganization() 
        {
            try
            {
                var data = Connecting.conn.Organization.ToList();
                organizationList = data;
                DgOrganization.ItemsSource = organizationList;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }

        private void TxbSearchOrganization_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string txtSearch = TxbSearchOrganization.Text.ToLower();
                if (txtSearch != null)
                {
                    var result = organizationList.Where(x =>
                    x.Name.ToLower().Contains(txtSearch) ||
                    x.Address.ToLower().Contains(txtSearch) ||
                    x.Phone.Contains(txtSearch)).ToList();
                    DgOrganization.ItemsSource = result;
                }
                else
                {
                    LoadDataOrganization();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }

        private void BtnAddOrganization_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string organizationName = TxbOrganizationName.Text;
                string organizationAddress = TxbOrganizationAddress.Text;
                string oganizationPhone = TxbOrganizationPhone.Text;

                if (organizationAddress != null && organizationAddress != "" && 
                    organizationName != null && organizationName != "" &&
                    oganizationPhone != null && oganizationPhone != "")
                {
                    Organization organization = new Organization()
                    {
                        Name = organizationName,
                        Address = organizationAddress,
                        Phone = oganizationPhone
                    };
                    Connecting.conn.Organization.Add(organization);
                    Connecting.conn.SaveChanges();

                    MessageBox.Show("Разработчик успешно добавлен");

                    LoadDataOrganization();
                    LoadDataItems();
                    LoadDataCmbOrganizations();                    
                }
                else
                {
                    MessageBox.Show("Заполнены не все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteOrganization_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = DgOrganization.SelectedItem as Organization;
                if (data != null)
                {
                    var result = MessageBox.Show("Вы действительно хотите удалить выбранную запись?", "Подтвердите", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Connecting.conn.Organization.Remove(data);
                        Connecting.conn.SaveChanges();

                        MessageBox.Show("Запись успешно удалена", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadDataOrganization();
                        LoadDataItems();
                        LoadDataCmbOrganizations();
                    }
                    return;
                }
                else
                {
                    MessageBox.Show("Удаление невозможно.\nНе выбрана строка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }


        private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string categoryName = TxbCategory.Text;
                if (categoryName != null && categoryName != "")
                {
                    var result = Connecting.conn.Category.FirstOrDefault(x => x.Name == categoryName);
                    if (result == null)
                    {
                        Category category = new Category()
                        {
                            Name = categoryName
                        };
                        Connecting.conn.Category.Add(category);
                        Connecting.conn.SaveChanges();

                        MessageBox.Show("Категория успешно добавлена");

                        LoadDataCategories();
                        LoadDataCmbCategories();
                    }
                    else
                    {
                        MessageBox.Show("Такая категория уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Вы не ввели название категории.\nВведите название категории и повторите попытку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

           
        }


        private void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = (sender as Button).DataContext as Category;
                var result = MessageBox.Show("Вы дейстительно хотите удалить данную категорию?", "Подтвержждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Connecting.conn.Category.Remove(data);
                    Connecting.conn.SaveChanges();

                    MessageBox.Show("Запись успешно удалена");

                    LoadDataCategories();
                    LoadDataCmbCategories();
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }
    }
}
