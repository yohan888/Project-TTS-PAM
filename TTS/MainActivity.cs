using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Java.Util;
using Android.Content;

namespace TTS
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextView txt1, txt2, txt3, txt4;
        Button translate;
        EditText text;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            
            txt1 = (TextView)FindViewById(Resource.Id.textView1);
            txt2 = (TextView)FindViewById(Resource.Id.textView2);
            txt3 = (TextView)FindViewById(Resource.Id.textView3);
            txt4 = (TextView)FindViewById(Resource.Id.textView4);
            translate = (Button)FindViewById(Resource.Id.button1);
            text = (EditText)FindViewById(Resource.Id.editText1);

            var client = new RestClient("https://google-translate1.p.rapidapi.com/language/translate/v2/languages");
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept-encoding", "application/gzip");
            request.AddHeader("x-rapidapi-key", "eccaa7b855msh5edf8fa0cce8ea1p1fa097jsn138f9ae4ecfa");
            request.AddHeader("x-rapidapi-host", "google-translate1.p.rapidapi.com");
            IRestResponse response = client.Execute(request);
            JObject j = JObject.Parse(response.Content);
            JToken bahasa = j["data"]["languages"];
            string l = "";


            /*for (int i=0; i<111; i++)
            {
                l += bahasa[i]["language"].ToString();
                txt1.SetText(l, null);
            }*/

            txt1.SetText("id", null);
            txt2.SetText("en", null);


            translate.Click += delegate
            {
                var client = new RestClient("https://google-translate1.p.rapidapi.com/language/translate/v2");
                var request = new RestRequest(Method.POST);
                string kalimat = text.Text.ToString();
                string source = txt1.Text.ToString();
                string target = txt2.Text.ToString();
                kalimat.Replace(" ", "%20");
                string data = "q=saya%20adalah%20manusia" + "&source=" + source + "&target=" + target;
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddHeader("accept-encoding", "application/gzip");
                request.AddHeader("x-rapidapi-key", "eccaa7b855msh5edf8fa0cce8ea1p1fa097jsn138f9ae4ecfa");
                request.AddHeader("x-rapidapi-host", "google-translate1.p.rapidapi.com");
                request.AddParameter("application/x-www-form-urlencoded", data, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                JObject j1 = JObject.Parse(response.Content);
                JToken bahasa1 = j1["data"]["translations"];
                Intent i = new Intent(this, typeof(ActivityTranslate));
                i.PutExtra("awal", kalimat);
                i.PutExtra("akhir", bahasa1[0]["translatedText"].ToString());
                StartActivity(i);
                txt3.SetText(kalimat, null);
                txt4.SetText(bahasa1[0]["translatedText"].ToString(), null);
            };
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}