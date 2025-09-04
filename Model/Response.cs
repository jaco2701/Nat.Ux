namespace Applet.Nat.Ux.Models

{
    public class Response
    {
        public int ivnumStatus { get; set; }
        public Object? ioData { get; set; }
    }

    public class ResponseException
    {
        public string? ivstrMsg{ get; set; }
        public string? ivstrStack { get; set; }

    }
    public class StringResponse
    {
        public string ivstr { get; set; }
    }
}

