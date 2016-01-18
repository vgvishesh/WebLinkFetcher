namespace CreateHtmlFakePages
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = @"http://yourstory.com/2015/11/10-steps-avoid-giving/";
            var linkFetcher = new LinkFetcher(20, url);
        }
    }
}

