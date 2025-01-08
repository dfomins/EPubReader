using EPubReader.Core;

namespace EPubReader.Models
{
    public class Book : ObservableObject
    {
        string bookCoversDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "book-covers");

        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Path { get; set; } = string.Empty;
        public string ImagePath
        {
            get { return System.IO.Path.Combine(bookCoversDirectory, $"{Id}.png"); }
        }
        public string AddingDate { get; set; } = string.Empty;
        private bool _isBookmark;
        public bool IsBookmark
        {
            get { return _isBookmark; }
            set
            {
                if (_isBookmark != value)
                {
                    _isBookmark = value;
                    OnPropertyChanged(nameof(IsBookmark));
                }
            }
        }
    }
}
