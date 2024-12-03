namespace CourseManagementService.Common
{
    public class ResponseInfo
    {
        public int StatusCode { get; set; }

        public string Error { get; set; }

        public string Message { get; set; }

        public Dictionary<string, dynamic> Data { get; set; }

        public bool IsSuccess
        {
            get
            {
                return StatusCode == StatusCodes.Status200OK
                    || StatusCode == StatusCodes.Status201Created;
            }
        }

        public ResponseInfo()
        {
            StatusCode = StatusCodes.Status200OK;
            Message = "Success";
            Data = [];
        }

        public ResponseInfo(string resource, dynamic data, int statusCode = StatusCodes.Status200OK)
        {
            StatusCode = statusCode;
            Message = statusCode switch
            {
                StatusCodes.Status200OK => "Success",
                StatusCodes.Status201Created => "Created",
                _ => "Error"
            };

            Data = new Dictionary<string, dynamic>
            {
                { resource, data }
            };
        }
    }
}