using System;

namespace kifaro.Models
{
    public class Wish
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? PictureUrl { get; set; }
        public int Price { get; set; }
        public int Rank { get; set; }
        public int UserID { get; set; }

        public Wish()
        {
            this.Rank = 1;
        }
    }
}