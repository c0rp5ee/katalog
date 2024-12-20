using System.IO;
using Microsoft.Win32;
using System.Text.Json;
using System.Windows;
using katalog;
using System.Diagnostics.Metrics;
using System.Collections.ObjectModel;

namespace MemeCatalog
{
    public partial class AddMemeDialog : Window
    {
        CategoryManager CategoryManager;
        public Meme Meme { get; private set; }

        public AddMemeDialog(CategoryManager categoryManager)
        {
            InitializeComponent();
            CategoryManager = categoryManager;
            UpdateCategoriesList();
        }

        //вызов диалогового окна для выборка мема
        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedImagePath.Text = openFileDialog.FileName;
                NameTextBox.Text = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
            }
        }

        //обновление категорий
        private void UpdateCategoriesList()
        {
            categoriesBox.ItemsSource = null;
            categoriesBox.ItemsSource = CategoryManager.Categories;
        }

        //создание нового мема
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (categoriesBox.SelectedIndex != -1 && NameTextBox.Text.Length > 0 && SelectedImagePath.Text.Length>0)
            {
                Meme = new Meme
                {
                    Name = NameTextBox.Text,
                    Category = categoriesBox.SelectedItem.ToString(),
                    ImagePath = SelectedImagePath.Text,
                };
                DialogResult = true;
                Close();
            }
            
        }

        //вызок окна редактирования категорий
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            categoriesEdit CE = new categoriesEdit(CategoryManager);
            if (CE.ShowDialog() == true)
            {
                UpdateCategoriesList();
            }
        }

        private void NameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}