public class ShortUrlDto
{
    public string id { get; set;}
    public string url { get; set;}
    public string shortUrl { get; set; }
    public int timesClicked {get; set; }
    public DateTime dateCreated { get; set; }
    public DateTime expirationDate { get; set; }
}