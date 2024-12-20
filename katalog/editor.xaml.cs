using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace katalog
{
    public partial class editor : Window
    {
        public Meme Meme { get; private set; }
        private readonly CategoryManager categoryManager;

        public editor(Meme meme, CategoryManager categoryManager)
        {
            InitializeComponent();
            Meme = meme;
            this.categoryManager = categoryManager;
            InitializeFields();
        }

        //инициализация полей для выбранного мема
        private void InitializeFields()
        {
            UpdateCategoriesList();
            NameTextBox.Text = Meme.Name;
            SelectedImagePath.Text = Meme.ImagePath;

            categoriesBox.SelectedIndex = categoryManager.Categories
                .Cast<string>()
                .ToList()
                .IndexOf(Meme.Category);
        }

        //обновление категорий
        private void UpdateCategoriesList()
        {
            categoriesBox.ItemsSource = categoryManager.Categories;
        }

        //кнока обновления мема
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Meme = CreateMemeFromInputs();
            DialogResult = true;
            Close();
        }

        //создание нового мема
        private Meme CreateMemeFromInputs()
        {
            return new Meme
            {
                Name = NameTextBox.Text,
                Category = categoriesBox.SelectedItem.ToString(),
                ImagePath = SelectedImagePath.Text,
            };
        }

        //кнокпка отмены
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        //вызов диалогового окна для выбора мема
        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedImagePath.Text = openFileDialog.FileName;
            }
        }

        //вызов окна редактрования категорий
        private void EditCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            var categoriesEditWindow = new categoriesEdit(categoryManager);
            if (categoriesEditWindow.ShowDialog() == true)
            {
                UpdateCategoriesList();
            }
        }
    }
}