using katalog;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;
using WpfAnimatedGif;
using System.ComponentModel;
using System.Windows.Threading;

namespace MemeCatalog
{
    public partial class MainWindow : Window           //https://a.d-cd.net/6YAAAgERq-A-960.jpg
    {
        private DispatcherTimer timer;
        private List<Meme> memes = new List<Meme>();
        List<string> categories;
        private CategoryManager _categoryManager;
        bool VideIsPlaying;

        public MainWindow()
        {
            InitializeComponent();
            LoadCategories();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
        }

        //загрузка начальных категорий
        private void LoadCategories()
        {
            string jsonName = "categories.json";
            string jsonPath = System.IO.Path.Combine(Environment.CurrentDirectory, jsonName);
            string json = File.ReadAllText(jsonPath);

            if (File.Exists(jsonPath))
            {
                var categories = JsonSerializer.Deserialize<List<string>>(json);
                _categoryManager = new CategoryManager(categories);

                if (_categoryManager.Categories != null)
                {
                    CategoryComboBox.ItemsSource = _categoryManager.Categories;
                    CategoryComboBox.SelectedIndex = 0;
                }

                _categoryManager.PropertyChanged += CategoryManager_PropertyChanged;
            }
        }

        //обновления списка категорий при его изменении (событие)
        private void CategoryManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CategoryManager.Categories))
            {
                UpdateCategoriesBox();

                if (_categoryManager.Categories.Any())
                {
                    CategoryComboBox.SelectedIndex = 0;
                }
            }
        }

        //обновлеине списка мемов для листбокса
        private void UpdateMemeList()
        {
            MemeListBox.ItemsSource = null;
            MemeListBox.ItemsSource = memes;
        }

        //обновление коллекции категорий для комбобокса
        private void UpdateCategoriesBox()
        {
            CategoryComboBox.ItemsSource = _categoryManager.Categories;
        }

        //выбор мема из списка и показ его
        private void MemeListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MemeListBox.SelectedItem is Meme selectedMeme)
            {
                string extension = Path.GetExtension(selectedMeme.ImagePath).ToLower();

                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".webp")
                {
                    showImage(selectedMeme);
                }
                else if (extension == ".mp4" || extension == ".avi" || extension == ".mov" || extension == ".wmv")
                {
                    ShowVideo(selectedMeme);
                }
                else
                {
                    MessageBox.Show("Неподдерживаемый формат файла: " + selectedMeme.ImagePath);
                    MemeImage.Source = null;
                    videoPlayer.Visibility = Visibility.Collapsed;
                    MemeImage.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                MemeImage.Source = null; // если выбрали не мем, то все скрывается 
                videoPlayer.Stop();
                videoPlayer.Visibility = Visibility.Collapsed;
                MemeImage.Visibility = Visibility.Collapsed;
            }
        }

        //показ картинки
        private void showImage(Meme selectedMeme)
        {
            var bitmapImage = new BitmapImage(); //новый объектик который используется для загрузки изображ
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(selectedMeme.ImagePath, UriKind.Absolute); //инициализация
            bitmapImage.EndInit();
            MemeImage.Source = bitmapImage; // установка источника для отображения мема
            MemeNameText.Text = selectedMeme.Name; //обновление имени мема

            ImageBehavior.SetAnimatedSource(MemeImage, bitmapImage); //установка анимации

            VisibilitySwitch(false); // плей и пауза не видны
        }

        //показ видео
        private void ShowVideo(Meme selectedMeme)
        {
            videoSlider.Value = 0; //видео с самого начала
            videoPlayer.Source = new Uri(selectedMeme.ImagePath, UriKind.Absolute); //установка источника для воспроизведения видео

            MemeNameText.Text = selectedMeme.Name;

            VisibilitySwitch(true); // плей и пауза видны
        }


        //переключение отображения элементов в зависимтости выбрано изображение или видео
        private void VisibilitySwitch(bool isVideo)
        {
            if (isVideo)
            {
                videoPlayer.Visibility = Visibility.Visible;  // если видео, то все кнопочки(пауза, громкость и тд) становятс явилимыми
                videoSlider.Visibility = Visibility.Visible;
                VolSlider.Visibility = Visibility.Visible;
                playButton.Visibility = Visibility.Visible;
                MemeImage.Visibility = Visibility.Collapsed;
            }
            else
            {
                MemeImage.Visibility = Visibility.Visible; // в другом случае MemeImage становится видимым, а все штучки для отображения видео скрываются 

                videoPlayer.Stop();
                videoPlayer.Visibility = Visibility.Collapsed;
                VideIsPlaying = false;
                timer.Stop();
                videoSlider.Visibility = Visibility.Hidden;
                VolSlider.Visibility = Visibility.Hidden;
                playButton.Visibility = Visibility.Hidden;
            }
        }

        //вызов окна добавления мемов
        private void AddMemeButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddMemeDialog(_categoryManager); // создается новое окошко 
            if (dialog.ShowDialog() == true) // Открывается окошко и ждет закрытия
            {
                memes.Add(dialog.Meme);
                UpdateMemeList();
            }
        }

        //удаление мема
        private void DeleteMemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemeListBox.SelectedItem is Meme selectedMeme)
            {
                memes.Remove(selectedMeme);
                UpdateMemeList();

                MemeImage.Source = null;
            }
        }

        //сохранение списка мемов и категорий
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                FileName = "memes.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var dataToSave = new SaveLoad(memes, _categoryManager);

                var json = JsonSerializer.Serialize(dataToSave);
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }

        //загрузка списка мемов и категорий
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                FileName = "memes.json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var json = File.ReadAllText(openFileDialog.FileName);
                var dataToLoad = JsonSerializer.Deserialize<SaveLoad>(json);

                memes = dataToLoad.Memes;
                _categoryManager = dataToLoad.CategoryManager;
                UpdateMemeList();
                UpdateCategoriesBox();
            }
        }

        //вызов окна редактирования мема
        private void EditMemeButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new editor(memes[MemeListBox.SelectedIndex], _categoryManager);
            if (dialog.ShowDialog() == true)
            {
                memes.RemoveAt(MemeListBox.SelectedIndex);
                memes.Insert(MemeListBox.SelectedIndex, dialog.Meme);
                UpdateMemeList();
            }

        }

        //поиск мема по имени
        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var searchText = SearchTextBox.Text.ToLower();
            MemeListBox.ItemsSource = memes.Where(m => m.Name.ToLower().Contains(searchText)).ToList();
        }

        //отобраение мемов по категориям
        private void CategoryComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedCategory = CategoryComboBox.SelectedItem as string;

            if (selectedCategory == "Все мемасы")
            {
                MemeListBox.ItemsSource = memes.ToList();
            }
            else
            {
                MemeListBox.ItemsSource = memes.Where(m => m.Category == selectedCategory).ToList();
            }
        }

        

        //логика ползунка видео
        private void videoSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (videoPlayer.NaturalDuration.HasTimeSpan)
            {
                videoPlayer.Position = TimeSpan.FromSeconds((videoSlider.Value/100) * videoPlayer.NaturalDuration.TimeSpan.TotalSeconds);
            }
        }

        //логика ползунка громкости
        private void VolSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            videoPlayer.Volume = VolSlider.Value;
        }

        //таймер
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (videoPlayer.NaturalDuration.HasTimeSpan)
            {
                videoTime.Text = $"{videoPlayer.Position.Hours:D2}:{videoPlayer.Position.Minutes:D2}:{videoPlayer.Position.Seconds:D2} / {videoPlayer.NaturalDuration.TimeSpan.Hours:D2}:{videoPlayer.NaturalDuration.TimeSpan.Minutes:D2}:{videoPlayer.NaturalDuration.TimeSpan.Seconds:D2}";
                videoSlider.Value = videoPlayer.Position.TotalSeconds / videoPlayer.NaturalDuration.TimeSpan.TotalSeconds * 100;
            }
        }

        //логика кнопки play\pause
        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideIsPlaying)
            {
                VideIsPlaying = false;
                videoPlayer.Stop();
                timer.Stop();
            }
            else
            {
                VideIsPlaying = true;
                videoPlayer.Play();
                timer.Start();
            }
        }
    }
}