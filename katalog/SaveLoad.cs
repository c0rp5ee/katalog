using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace katalog
{
    public class SaveLoad
    {
        public List<Meme> Memes { get; set; }  // список объектов типа Meme
        public CategoryManager CategoryManager { get; set; }  // управление категориями мемов

        public SaveLoad(List<Meme> memes, CategoryManager categoryManager)    // конструктор 
        {
            Memes = memes;
            CategoryManager = categoryManager;
        }

    }
}
