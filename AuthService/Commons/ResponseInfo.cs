namespace AuthService.Commons
{
    public class ResponseInfo
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public Dictionary<string, dynamic> Data { get; set; }

        public ResponseInfo()
        {
            StatusCode = HttpStatusCode.OK;
            Message = "Success";
            Data = [];
        }
    }
}