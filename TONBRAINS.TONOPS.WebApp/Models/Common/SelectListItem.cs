namespace TONBRAINS.TONOPS.WebApp.Common
{
    public class SelectListItem<T>
    {
        public T Id { get; set; }

        public string Title { get; set; }

        public SelectListItem(T id , string title)
        {
            Id = id;
            Title = title;
        }
    }
}
