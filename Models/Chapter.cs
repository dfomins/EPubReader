namespace EPubReader.Models
{
    public class Chapter
    {
        public string Title { get; set; }
        public string Key { get; set; }
        public List<Chapter> SubChapter { get; set; }

        public Chapter(string Title, string Key)
        {
            this.Title = Title;
            this.Key = Key;
            SubChapter = new List<Chapter>();
        }
    }
}
