using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace TTS
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextView txt;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            txt = (TextView)FindViewById(Resource.Id.textView1);
            var client = new RestClient("https://google-translate1.p.rapidapi.com/language/translate/v2/languages");
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept-encoding", "application/gzip");
            request.AddHeader("x-rapidapi-key", "eccaa7b855msh5edf8fa0cce8ea1p1fa097jsn138f9ae4ecfa");
            request.AddHeader("x-rapidapi-host", "google-translate1.p.rapidapi.com");
            IRestResponse response = client.Execute(request);
            JObject j = JObject.Parse(response.Content);
            JToken bahasa = j["data"]["languages"];
            for(int i=0; i<111; i++)
            {
                txt.SetText((string)bahasa[i]["language"].ToString(), null);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}