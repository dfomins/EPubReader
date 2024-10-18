using EPubReader.Models;

namespace EPubReader.ViewModel
{
    public class BookInfoViewModel
    {
        public Book CurrentBook { get; set; }

        //private Book _currentBook;
        //public Book CurrentBook
        //{
        //    get => _currentBook;
        //    set
        //    {
        //        _currentBook = value;
        //    }
        //}

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
