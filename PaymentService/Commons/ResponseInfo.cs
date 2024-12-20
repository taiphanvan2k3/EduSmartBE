namespace PaymentService.Commons
{
    public class ResponseInfo
    {
        public int StatusCode { get; set; }

        public string Error { get; set; }

        public string Message { get; set; }

        public Dictionary<string, dynamic> Data { get; set; }

        public ResponseInfo()
        {
            StatusCode = StatusCodes.Status200OK;
            Message = "Success";
            Data = [];
        }

        public bool IsSuccess
        {
            get
            {
                return StatusCode == StatusCodes.Status200OK || StatusCode == StatusCodes.Status201Created;
            }
        }
    }
}