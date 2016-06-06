using System;
using System.Collections.Generic;
using System.Text;

using Linn;
using Linn.Ascii;

namespace Linn.Control.Http
{
    class HttpError : Exception
    {
        public HttpError()
        {
        }
    }

    public enum EMethod
    {
        eOptions,
        eGet,
        eHead,
        ePost,
        ePut,
        eDelete,
        eTrace,
        eConnect,
        eExtension
    };

    public enum EVersion
    {
        eHttp09 = 9,    // HTTP/0.9
        eHttp10 = 10,   // HTTP/1.0
        eHttp11 = 11,   // HTTP/1.1
    };

    public class Status
    {
        public Status(uint aCode, string aReason)
        {
            iCode = aCode;
            iReason = aReason;
        }

        public uint Code()
        {
            return (iCode);
        }

        public string Reason()
        {
            return (iReason);
        }

        private uint iCode;
        private string iReason;
    }

    class Http
    {
        // Informational codes
        public static readonly Status kContinue =  new Status(100, "Continue");
        public static readonly Status kSwitchingProtocols =  new Status(101, "Switching Protocols");
        // Success codes
        public static readonly Status kOk =  new Status(200, "OK");
        public static readonly Status kCreated =  new Status(201, "Created");
        public static readonly Status kAccepted =  new Status(202, "Accepted");
        public static readonly Status kNonAuthoritativeInformation =  new Status(203, "Non-Authoritative Information");
        public static readonly Status kNoContent =  new Status(204, "No Content");
        public static readonly Status kResetContent =  new Status(205, "Reset Content");
        public static readonly Status kPartialContent =  new Status(206, "Partial Content");
        // Redirection codes
        public static readonly Status kMultipleChoices =  new Status(300, "Multiple Choices");
        public static readonly Status kMovedPermanently =  new Status(301, "Moved Permanently");
        public static readonly Status kFound =  new Status(302, "Found");
        public static readonly Status kSeeOther =  new Status(303, "See Other");
        public static readonly Status kNotModified =  new Status(304, "Not Modified");
        public static readonly Status kUseProxy =  new Status(305, "Use Proxy");
        public static readonly Status kTemporaryRedirect =  new Status(307, "Temporary Redirect");
        // Client error codes
        public static readonly Status kBadRequest =  new Status(400, "Bad Request");
        public static readonly Status kUnauthorized =  new Status(401, "Unauthorized");
        public static readonly Status kPaymentRequired =  new Status(402, "Payment Required");
        public static readonly Status kForbidden =  new Status(403, "Forbidden");
        public static readonly Status kNotFound =  new Status(404, "Not Found");
        public static readonly Status kMethodNotAllowed =  new Status(405, "Method Not Allowed");
        public static readonly Status kNotAcceptable =  new Status(406, "Not Acceptable");
        public static readonly Status kProxyAuthenticationRequired =  new Status(407, "Proxy Authentication Required");
        public static readonly Status kRequestTimeout =  new Status(408, "Request Timeout");
        public static readonly Status kConflict =  new Status(409, "Conflict");
        public static readonly Status kGone =  new Status(410, "Gone");
        public static readonly Status kLengthRequired =  new Status(411, "Length Required");
        public static readonly Status kPreconditionFailed =  new Status(412, "Precondition Failed");
        public static readonly Status kRequestEntityTooLarge =  new Status(413, "Request Entity Too Large");
        public static readonly Status kRequestUriTooLarge =  new Status(414, "Request URI Too Large");
        public static readonly Status kUnsupportedMediaType =  new Status(415, "Unsupported Media Type");
        public static readonly Status kRequestedRangeNotSatisfiable =  new Status(416, "Request Range Not Satisfiable");
        public static readonly Status kExpectationFailure =  new Status(417, "Expectation Failure");
        // Server error codes
        public static readonly Status kInternalServerError =  new Status(500, "Internal Server Error");
        public static readonly Status kNotImplemented =  new Status(501, "Not Implemented");
        public static readonly Status kBadGateway =  new Status(502, "Bad Gateway");
        public static readonly Status kServiceUnavailable =  new Status(503, "Service Unavailable");
        public static readonly Status kGatewayTimeout =  new Status(504, "Gateway Timeout");
        public static readonly Status kHttpVersionNotSupported = new Status(505, "HTTP Version Not Supported");

        private const string kHttpMethodOptions = "OPTIONS";
        private const string kHttpMethodGet = "GET";
        private const string kHttpMethodHead = "HEAD";
        private const string kHttpMethodPost = "POST";
        private const string kHttpMethodPut = "PUT";
        private const string kHttpMethodDelete = "DELETE";
        private const string kHttpMethodTrace = "TRACE";
        private const string kHttpMethodConnect = "CONNECT";
        private const string kHttpMethodExtension = "EXTENSION";

        private const string kHttpVersion09 = "HTTP/0.9";
        private const string kHttpVersion10 = "HTTP/1.0";
        private const string kHttpVersion11 = "HTTP/1.1";

        public const string kHttpClose = "close";
        public const string kHttpKeepAlive = "Keep-Alive";
        public const string kHttpSpace = " ";
        public const string kHttpNewline = "\r\n";

        public const string kHeaderFieldSeparator = ": ";

        public const string kHeaderGeneralCacheControl = "Cache-Control";
        public const string kHeaderGeneralConnection = "Connection";
        public const string kHeaderGeneralDate = "Date";
        public const string kHeaderGeneralPragma = "Pragma";
        public const string kHeaderGeneralTrailer = "Trailer";
        public const string kHeaderGeneralTransferEncoding = "Transfer-Encoding";
        public const string kHeaderGeneralUpgrade = "Upgrade";
        public const string kHeaderGeneralVia = "Via";
        public const string kHeaderGeneralWarning = "Warning";
        public const string kHeaderRequestAccept = "Accept";
        public const string kHeaderRequestAcceptCharset = "Accept-Charset";
        public const string kHeaderRequestAcceptEncoding = "Accept-Encoding";
        public const string kHeaderRequestAcceptLanguage = "Accept-Language";
        public const string kHeaderRequestAuthorization = "Authorization";
        public const string kHeaderRequestExpect = "Expect";
        public const string kHeaderRequestFrom = "From";
        public const string kHeaderRequestHost = "Host";
        public const string kHeaderRequestIfMatch = "If-Match";
        public const string kHeaderRequestIfModifiedSince = "If-Modified-Since";
        public const string kHeaderRequestIfNoneMatch = "If-None-Match";
        public const string kHeaderRequestIfRange = "If-Range";
        public const string kHeaderRequestIfUnmodifiedSince = "If-Unmodified-Since";
        public const string kHeaderRequestMaxForwards = "Max-Forwards";
        public const string kHeaderRequestProxyAuthorization = "Proxy-Authorization";
        public const string kHeaderRequestRange = "Range";
        public const string kHeaderRequestReferer = "Referer";
        public const string kHeaderRequestTe = "Te";
        public const string kHeaderRequestUserAgent = "User-Agent";
        public const string kHeaderResponseAcceptRanges = "Accept-Ranges";
        public const string kHeaderResponseAge = "Age";
        public const string kHeaderResponseETag = "ETag";
        public const string kHeaderResponseLocation = "Location";
        public const string kHeaderResponseProxyAuthenticate = "Proxy-Authenticate";
        public const string kHeaderResponseRetryAfter = "Retry-After";
        public const string kHeaderResponseServer = "Server";
        public const string kHeaderResponseVary = "Vary";
        public const string kHeaderResponseWwwAuthenticate = "WWW-Authenticate";
        public const string kHeaderEntityAllow = "Allow";
        public const string kHeaderEntityContentEncoding = "Content-Encoding";
        public const string kHeaderEntityContentLanguage = "Content-Language";
        public const string kHeaderEntityContentLength = "Content-Length";
        public const string kHeaderEntityContentLocation = "Content-Location";
        public const string kHeaderEntityContentMd5 = "Content-Md5";
        public const string kHeaderEntityContentRange = "Content-Range";
        public const string kHeaderEntityContentType = "Content-Type";
        public const string kHeaderEntityExpires = "Expires";
        public const string kHeaderEntityLastModified = "Last-Modified";

        public static byte[] ByteArray(string aString)
        {
            return (ASCIIEncoding.UTF8.GetBytes(aString));
        }

        public static EMethod Method(byte[] aMethod)
        {
            if (aMethod == ASCIIEncoding.UTF8.GetBytes(kHttpMethodOptions))
            {
                return (EMethod.eOptions);
            }
            if (aMethod == ASCIIEncoding.UTF8.GetBytes(kHttpMethodGet))
            {
                return (EMethod.eGet);
            }
            if (aMethod == ASCIIEncoding.UTF8.GetBytes(kHttpMethodHead))
            {
                return (EMethod.eHead);
            }
            if (aMethod == ASCIIEncoding.UTF8.GetBytes(kHttpMethodPost))
            {
                return (EMethod.ePost);
            }
            if (aMethod == ASCIIEncoding.UTF8.GetBytes(kHttpMethodPut))
            {
                return (EMethod.ePut);
            }
            if (aMethod == ASCIIEncoding.UTF8.GetBytes(kHttpMethodDelete))
            {
                return (EMethod.eDelete);
            }
            if (aMethod == ASCIIEncoding.UTF8.GetBytes(kHttpMethodTrace))
            {
                return (EMethod.eTrace);
            }
            if (aMethod == ASCIIEncoding.UTF8.GetBytes(kHttpMethodConnect))
            {
                return (EMethod.eConnect);
            }
            if (aMethod == ASCIIEncoding.UTF8.GetBytes(kHttpMethodExtension))
            {
                return (EMethod.eExtension);
            }
            throw (new HttpError());
        }

        public static string Method(EMethod aMethod)
        {
            switch (aMethod)
            {
                case (EMethod.eOptions): return (kHttpMethodOptions);
                case EMethod.eGet: return (kHttpMethodGet);
                case EMethod.eHead: return (kHttpMethodHead);
                case EMethod.ePost: return (kHttpMethodPost);
                case EMethod.ePut: return (kHttpMethodPut);
                case EMethod.eDelete: return (kHttpMethodDelete);
                case EMethod.eTrace: return (kHttpMethodTrace);
                case EMethod.eConnect: return (kHttpMethodConnect);
                case EMethod.eExtension: return (kHttpMethodExtension);
                default: break;
            }
            Assert.Check(false);
            return ("");
        }

        public static EVersion Version(byte[] aVersion)
        {
            String version = ASCIIEncoding.UTF8.GetString(aVersion, 0, aVersion.Length);

            if (version == kHttpVersion11)
            {
                return (EVersion.eHttp11);
            }
            if (version == kHttpVersion10)
            {
                return (EVersion.eHttp10);
            }
            if (version == kHttpVersion09)
            {
                return (EVersion.eHttp09);
            }
            throw (new HttpError());
        }

        public static string Version(EVersion aVersion)
        {
            switch (aVersion) {
                case EVersion.eHttp09: return (kHttpVersion09);
                case EVersion.eHttp10: return (kHttpVersion10);
                case EVersion.eHttp11: return (kHttpVersion11);
                default: break;
            }
            Assert.Check(false);
            return ("");
        }
    }

    public interface IWriterMethod
    {
        void WriteMethod(byte[] aMethod, byte[] aUri, EVersion aVersion);
    }

    public interface IWriterStatus
    {
        void WriteStatus(Status aStatus, EVersion aVersion);
    }

    public interface IWriterHeader : IWriter
    {
        void WriteHeader(byte[] aField, byte[] aValue);
    }

    public interface IWriterHeaderExtended : IWriterHeader 
    {
        IWriterAscii WriteHeaderField(byte[] aField);
        void WriteHeaderTerminator();
    }

    public interface IWriterRequest : IWriterHeader, IWriterMethod
    {
    }

    public interface IWriterResponse : IWriterHeader, IWriterStatus
    {    
    }

    // ReaderRequest

    class ReaderRequest
    {
        public ReaderRequest(IReader aReader, IWriterRequest aWriter)
        {
            iReader = aReader;
            iWriter = aWriter;
        }

        public void Read()
        {
            iReader.ReadFlush();

            uint count = 0;

            while (true)
            {
                byte[] line;

                line = Ascii.Ascii.Trim(iReader.ReadUntil(Ascii.Ascii.kAsciiLf));

                int bytes = line.Length;

                if (bytes == 0)
                {
                    if (count == 0)
                    {
                        continue; // a blank line before first header - ignore (RFC 2616 section 4.1)
                    }
                    iWriter.WriteFlush();
                    return;
                }

                if (Ascii.Ascii.IsWhitespace(line[0]))
                {
                    continue; // a line starting with spaces is a continuation line
                }

                Parser parser = new Parser(line);

                if (count == 0)
                { // method
                    byte[] method = parser.Next();
                    byte[] uri = parser.Next();
                    byte[] version = Ascii.Ascii.Trim(parser.Remaining());
                    iWriter.WriteMethod(method, uri, Http.Version(version));
                }
                else
                { // header
                    byte[] field = parser.Next(Ascii.Ascii.kAsciiColon);
                    byte[] value = Ascii.Ascii.Trim(parser.Remaining());
                    iWriter.WriteHeader(field, value);
                }
                count++;
            }
        }

        /*
        public void Shutdown()
        {
            iReader.ReadShutdown();
        }
        */

        private IReader iReader;
        private IWriterRequest iWriter;
    };

    // IHeader

    public interface IHeader
    {
	    void Reset();
	    bool Recognise(byte[] aHeader);
	    void Process(byte[] aValue);
    };

    // ReaderHeader

    class ReaderHeader
    {
        protected ReaderHeader()
        {
            iHeaders = new List<IHeader>();
        }

        protected IHeader Header
        {
            get
            {
                Assert.Check(iHeader != null);
    	        return (iHeader);
            }
        }

        protected void ResetHeaders()
        {
            foreach (IHeader h in iHeaders)
            {
                h.Reset();
            }
        }

	    protected void ProcessHeader(byte[] aField, byte[] aValue)
        {
            foreach (IHeader h in iHeaders)
            {
                if (h.Recognise(aField))
                {
                    iHeader = h;
                    h.Process(aValue);
                    return;
                }
            }
        }

        public void AddHeader(IHeader aHeader)
        {
            iHeaders.Add(aHeader);
        }

        private IHeader iHeader;
        private List<IHeader> iHeaders;
    };

    // ReaderResponse

    class ReaderResponse2 : ReaderHeader
    {
        public ReaderResponse2(IReader aReader)
        {
            iReader = aReader;
        }

        public void Read()
        {
            iReader.ReadFlush();
            
            uint count = 0;
            
            while (true) {
                byte[] line = Ascii.Ascii.Trim(iReader.ReadUntil(Ascii.Ascii.kAsciiLf));

                Trace.WriteLine(Trace.kUpnp, "ReaderResponse   " + ASCIIEncoding.UTF8.GetString(line,0,line.Length));

                int bytes = line.Length;
                
                if (bytes == 0) {
                    if (count == 0) {
                        continue; // a blank line before first header - ignore (RFC 2616 section 4.1)
                    }
                    
                    return;
                }
                
                if (Ascii.Ascii.IsWhitespace(line[0])) {
                    continue; // a line starting with spaces is a continuation line
                }
            
                Parser parser = new Parser(line);
                
                if (count == 0) { // status
                    byte[] version = parser.Next();
                    byte[] code = parser.Next();
                    byte[] description = Ascii.Ascii.Trim(parser.Remaining());
                    ProcessStatus(version, code, description);
                }
                else { // header
                    byte[] field = parser.Next(Ascii.Ascii.kAsciiColon);
                    byte[] value = Ascii.Ascii.Trim(parser.Remaining());
                    ProcessHeader(field, value);
                }
                count++;
            }
        }

        public void Flush()
        {
            iReader.ReadFlush();
        }

        /*
        public void Close()
        {
            iReader.ReadShutdown();
        }
        */

        public EVersion Version
        {
            get
            {
                return (iVersion);
            }
        }

        public uint Code
        {
            get
            {
                return (iCode);
            }
        }

        public byte[] Description
        {
            get
            {
                return (iDescription);
            }
        }

        private void ProcessStatus(byte[] aVersion, byte[] aCode, byte[] aDescription)
        {
            iVersion = Http.Version(aVersion);
        	
	        try {
		        iCode = Ascii.Ascii.Uint(aCode);
	        }
	        catch (AsciiError) {
                throw (new HttpError());
	        }
        		
            iDescription = aDescription;
        }

        private IReader iReader;
        private EVersion iVersion;
        private uint iCode;
        private byte[] iDescription;
    }

    /*
class ReaderResponse : public ReaderHeader
{
	static const TUint kMaxDescriptionBytes = 100;
public:    
    ReaderResponse(IReader& aReader);
    void Read();
    void Flush();
    void Close();
	EVersion Version() const;
	TUint Code() const;
	const Brx& Description() const;
private:
	void ProcessStatus(const Brx& aVersion, const Brx& aCode, const Brx& aDescription);
private:
    IReader& iReader;
	EVersion iVersion;
    TUint iCode;
    Bws<kMaxDescriptionBytes> iDescription;
};
     */




    // ReaderResponse

    class ReaderResponse
    {
        public ReaderResponse(IReader aReader, IWriterResponse aWriter)
        {
            iReader = aReader;
            iWriter = aWriter;
        }

        public void Read()
        {
            iReader.ReadFlush();
            
            uint count = 0;
            
            while (true) {
                byte[] line;
                
                line = Ascii.Ascii.Trim(iReader.ReadUntil(Ascii.Ascii.kAsciiLf));
                
                int bytes = line.Length;
                
                if (bytes == 0) {
                    if (count == 0) {
                        continue; // a blank line before first header - ignore (RFC 2616 section 4.1)
                    }
                    iWriter.WriteFlush();
                    return;
                }
                
                if (Ascii.Ascii.IsWhitespace(line[0])) {
                    continue; // a line starting with spaces is a continuation line
                }
            
                Parser parser = new Parser(line);
                
                if (count == 0) { // status
                    EVersion version = Http.Version(parser.Next());
                    
                    uint code;
                    
                    try {
                        code = Ascii.Ascii.Uint(parser.Next());
                    }
                    catch (Ascii.AsciiError) {
                        throw (new HttpError());
                    }

                    byte[] temp = Ascii.Ascii.Trim(parser.Remaining());
                    string reason = ASCIIEncoding.UTF8.GetString(temp, 0, temp.Length);
                    Status status =  new Status(code, reason);
                    
                    iWriter.WriteStatus(status, version);
                }
                else { // header
                    byte[] field = parser.Next(Ascii.Ascii.kAsciiColon);
                    byte[] value = Ascii.Ascii.Trim(parser.Remaining());
                    iWriter.WriteHeader(field, value);
                }
                count++;
            }
        }

        /*
        public void Shutdown()
        {
            iReader.ReadShutdown();
        }
        */

        private IReader iReader;
        private IWriterResponse iWriter;
    };

    // WriterHeader

    class WriterHeader : IWriterHeaderExtended
    {
        protected WriterHeader(IWriter aWriter)
        {
            iWriter = new WriterAscii(aWriter);
        }

        // IWriter

        public void Write(byte aValue)
        {
            Assert.Check(false);
        }

        public void Write(byte[] aBuffer)
        {
            Assert.Check(false);
        }

        public void WriteFlush()
        {
            iWriter.WriteNewline();
            iWriter.WriteFlush();
        }

        // IWriterHeader

        public void WriteHeader(byte[] aField, byte[] aValue)
        {
            iWriter.Write(aField);
            iWriter.Write(Http.ByteArray(Http.kHeaderFieldSeparator));
            iWriter.Write(aValue);
            iWriter.WriteNewline();
        }

        // IWriterHeaderExtended

        public IWriterAscii WriteHeaderField(byte[] aField)  // returns a stream for writing the value
        {
            iWriter.Write(aField);
            iWriter.Write(Http.ByteArray(Http.kHeaderFieldSeparator));
            return (iWriter);
        }

        public void WriteHeaderTerminator()
        {
            iWriter.WriteNewline();
        }

        protected WriterAscii iWriter;
    }

    // WriterRequest

    class WriterRequest : WriterHeader, IWriterMethod 
    {
        public WriterRequest(IWriter aWriter) : base(aWriter)
        {
        }

        public void WriteMethod(byte[] aMethod, byte[] aUri, EVersion aVersion)
        {
            iWriter.Write(aMethod);
            iWriter.WriteSpace();
            iWriter.Write(aUri);
            iWriter.WriteSpace();
            iWriter.Write(Http.ByteArray(Http.Version(aVersion)));
            iWriter.WriteNewline();
        }
    }


    // WriterResponse

    class WriterResponse : WriterHeader, IWriterStatus 
    {
        public WriterResponse(IWriter aWriter) : base(aWriter)
        {
        }

        public void WriteStatus(Status aStatus, EVersion aVersion)
        {
            iWriter.Write(Http.ByteArray(Http.Version(aVersion)));
            iWriter.WriteSpace();
            iWriter.WriteUint(aStatus.Code());
            iWriter.WriteSpace();
            iWriter.Write(Http.ByteArray(aStatus.Reason()));
            iWriter.WriteNewline();
        }
    }
}
