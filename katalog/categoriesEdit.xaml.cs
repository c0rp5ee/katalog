using System;
using System.Windows;

namespace katalog
{
    public partial class categoriesEdit : Window
    {
        private CategoryManager _categoryManager;

        public categoriesEdit(CategoryManager categoryManager)
        {
            InitializeComponent();
            _categoryManager = categoryManager;
            UpdateCategoriesList();
        }

        //обновляем список категорий
        private void UpdateCategoriesList()
        {
            CList.ItemsSource = null;
            CList.ItemsSource = _categoryManager.Categories;
        }

        //обновление списка категорий(удаление или добавление)
        private void UpdateCategory(bool isAdding)
        {
            if (isAdding && !string.IsNullOrEmpty(CText.Text))
            {
                _categoryManager.Categories.Add(CText.Text);
                CText.Clear();
            }
            else if (!isAdding && CList.SelectedItem != null)
            {
                _categoryManager.Categories.RemoveAt(CList.SelectedIndex);
            }
            UpdateCategoriesList();
        }

        //кнопка добавления мема
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateCategory(true);
        }

        //кнопка удания мема
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateCategory(false);
        }

        //кнопка отмены
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}