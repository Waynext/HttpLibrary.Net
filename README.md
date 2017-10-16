# HttpLibrary.Net
HttpLibrary.Net PCL is an open source, minimal library to allow .NET and Mono applications to access web resouce by http request and response.
By inheriting Request and Response classes, function can be implemented in unified way. Multithread access is supported. Request can be classify by priority.

## Usage
### Create RequestQueue

	var libHelper = DependencyService.Get<IHttpLibraryHelper>(DependencyFetchTarget.GlobalInstance);
	RequestQueue reqQueue = new RequestQueue(libHelper.Platform);

### Send request

	var req = new ProjectRomeListDeviceRequest();

	await reqQueue.SendRequestAsync(req);

	var response = req.Response as ProjectRomeListDeviceResponse;
	if(response != null && response.IsSucceeded)
	{
		return response.Devices.Where(d => d.Status.Equals("online", StringComparison.OrdinalIgnoreCase));
	}	
		
### Create Request
	
	//Let's take MSGraph ProjectRome RESTAPI as an example
	class MSGraphSettings
    {
        private static MSGraphSettings _instance = new MSGraphSettings();

        public static MSGraphSettings Instance
        {
            get
            {
                return _instance;
            }
        }

        public string Token
        {
            get;
            set;
        }

        public IHttpLibraryPlatform Platform
        {
            get;
            set;
        }

    }
	
	enum HttpMethod {
        GET, POST, PATCH, PUT, DELETE
    }

    class BasicRequest : Request
    {
        public const string basicUriTemplate = "https://graph.microsoft.com/{0}/{1}";

        protected HttpMethod _method;
        protected JObject _jsonObject;
        public BasicRequest(string resource, JObject jsonObject = null, HttpMethod method = HttpMethod.GET, string query = null, string version = "beta", RequestPriority priority = RequestPriority.Normal) : base(MSGraphSettings.Instance.Platform, priority)
        {
            _method = method;

            Uri = string.Format(basicUriTemplate, version, resource);
            if(string.IsNullOrEmpty(query))
            {
                Uri += "?" + query;
            }

            _jsonObject = jsonObject;

            if(_jsonObject != null)
            {
                Debug.WriteLine("Json request: " + jsonObject);
            }

            if (requestHeaders == null)
                requestHeaders = new List<KeyValuePair<string, string>>();

            if(string.IsNullOrWhiteSpace(MSGraphSettings.Instance.Token))
            {
                throw new InvalidOperationException("MS Account token is empty");
            }

            requestHeaders.Add(new KeyValuePair<string, string>("Authorization", "Bearer " + MSGraphSettings.Instance.Token));
        }

        protected override void Encode()
        {
            base.Encode();

            if(_jsonObject != null)
            {
                HttpRequest = new JSonRequest(HttpLibPlatform, Uri, _jsonObject, _method.ToString());
            }
            else
            {
                HttpRequest = new HttpLibrary.Http.HttpRequest(HttpLibPlatform, Uri, _method.ToString());
            }

            if(requestHeaders != null)
            {
                HttpRequest.Headers = requestHeaders;
            }
        }


        protected override void CreateResponse()
        {
            Response = new BasicResponse();
        }
    }

    class BasicResponse: Response
    {
        public Object Data
        {
            get;
            set;
        }

        protected override void Decode()
        {
            base.Decode();

            if (!IsSucceeded)
            {
                Debug.WriteLine("MS Graph return failure: ");
                if(Exception != null)
                {
                    Debug.WriteLine(Exception);
                }

            }

            JSonResponse response = HttpResponse as JSonResponse;

            if (response != null)
            {
                Data = response.Object;

                Debug.WriteLine("Json request: " + Data);
            }
        }
    }
	
	class ProjectRomeListDeviceRequest : BasicRequest
    {
        private const string resource = "me/devices";
        public ProjectRomeListDeviceRequest() : base(resource)
        {
        }

        protected override void CreateResponse()
        {
            Response = new ProjectRomeListDeviceResponse();
        }
    }

    class PRDevice
    {
        public string Name{get;set;}
        public string id { get; set; }
        public string Status { get; set; }
        public string Platform{get;set;}
        public string Kind{get;set;}
        public string Image{get;set;}
        public string Model{get;set;}
        public string Manufacturer{get;set;}
    }

    class ProjectRomeListDeviceResponse : BasicResponse
    {
        public IEnumerable<PRDevice> Devices
        {
            get;
            private set;
        }
        
        protected override void Decode()
        {
            base.Decode();

            var jData = Data as JObject;

            if (IsSucceeded && jData != null)
            {
                JToken jToken;
                if(jData.TryGetValue("value", out jToken))
                {
                    var devicesTemp = new List<PRDevice>();

                    var array = jToken as JArray;
                    foreach(var jItem in array)
                    {
                        var device = jItem.ToObject<PRDevice>();
                        devicesTemp.Add(device);
                        Devices = devicesTemp;
                    }
                }
                
            }
        }
    }
	
### Create httpLibrary helper in UWP, Android or iOS project

	[assembly: Dependency(typeof(NeroShareDemo.UWP.HttpLibraryHelperUWP))]
	namespace NeroShareDemo.UWP
	{
		class HttpLibraryHelperUWP : IHttpLibraryHelper
		{
			private static IHttpLibraryPlatform _platform = new HttpLibraryPlatformUWP();
			public IHttpLibraryPlatform Platform => _platform;
		}
	}

