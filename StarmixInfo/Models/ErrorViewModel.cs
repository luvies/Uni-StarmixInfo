namespace StarmixInfo.Models
{
    public class ErrorViewModel
    {
        /// <summary>
        /// Base code message.
        /// </summary>
        public string BaseMessage { get; set; } = "Error";
        /// <summary>
        /// Format string for error messages.
        /// {0} - <see cref="BaseMessage"/>. 
        /// </summary>
        public string FormatMessageOnly { get; set; } = "{0}";
        /// <summary>
        /// Format string for error messages with a code and no code message.
        /// {0} - <see cref="BaseMessage"/>.
        /// {1} - Code.
        /// </summary>
        public string FormatMessageCode { get; set; } = "{0} {1}";
        /// <summary>
        /// Format string for error messages with a code and code message.
        /// {0} - <see cref="BaseMessage"/>.
        /// {1} - Code.
        /// {2} - Code message.
        /// </summary>
        public string FormatMessageCodeMessage { get; set; } = "{0} {1}: {2}";

        public string Code { get; set; }
        public string Message
        {
            get
            {
                if (Code == null)
                    return string.Format(FormatMessageOnly, BaseMessage);
                if (int.TryParse(Code, out int c))
                {
                    string msg = ErrorCodeMessage(c);
                    if (msg == null)
                        return string.Format(FormatMessageCode, BaseMessage, Code);
                    return string.Format(FormatMessageCodeMessage, BaseMessage, Code, msg);
                }
                return string.Format(FormatMessageCode, BaseMessage, Code);
            }
        }
        public string RequestId { get; set; }

        public static string ErrorCodeMessage(int code)
        {
            switch (code)
            {
                // client errors
                case 400:
                    return "Bad Request";
                case 401:
                    return "Unauthorized";
                case 402:
                    return "Payment Required";
                case 403:
                    return "Forbidden";
                case 404:
                    return "Not Found";
                case 405:
                    return "Method Not Allowed";
                case 406:
                    return "Not Acceptable";
                case 407:
                    return "Proxy Authentication Required";
                case 408:
                    return "Request Timeout";
                case 409:
                    return "Conflict";
                case 410:
                    return "Gone";
                case 411:
                    return "Length Required";
                case 412:
                    return "Precondition Failed";
                case 413:
                    return "Payload Too Large";
                case 414:
                    return "URI Too Long";
                case 415:
                    return "Unsupported Media Type";
                case 416:
                    return "Range Not Satisfiable";
                case 417:
                    return "Expectation Failed";
                case 418:
                    return "I'm a teapot"; // joke code
                case 421:
                    return "Misdirected Request";
                case 422:
                    return "Unprocessable Entity";
                case 423:
                    return "Locked";
                case 424:
                    return "Failed Dependency";
                case 426:
                    return "Upgrade Required";
                case 428:
                    return "Precondition Required";
                case 429:
                    return "Too Many Requests";
                case 431:
                    return "Request Header Fields Too Large";
                case 451:
                    return "Unavailable For Legal Reasons";

                // server errors
                case 500:
                    return "Internal Server Error";
                case 501:
                    return "Not Implemented";
                case 502:
                    return "Bad Gateway";
                case 503:
                    return "Service Unavailable";
                case 504:
                    return "Gateway Timeout";
                case 505:
                    return "HTTP Version Not Supported";
                case 506:
                    return "Variant Also Negates";
                case 507:
                    return "Insufficient Storage";
                case 508:
                    return "Loop Detected";
                case 510:
                    return "Not Extended";
                case 511:
                    return "Network Authentication Required";

                default:
                    return null;
            }
        }
    }
}