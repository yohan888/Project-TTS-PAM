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
using System.IO;
using SQLite;
using System;
using Android.Speech.Tts;
using Newtonsoft.Json;
using System.Linq;

namespace TTS
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextView txt1, txt2, txt3, txt4, tester;
        ImageButton translate, hapus, swit;
        EditText text;
        ListView lv;
        Spinner spin, spin2;
        TextToSpeech textToSpeech;
        private ArrayAdapter<string> adapter;
        private List<string> Llist = new List<string>();
        private List<string> Blist = new List<string>();
        public static string bahasaSource, bahasaTarget;
        public static int indexSource, indexTarget;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db = new SQLiteConnection(dbPath);
            db.CreateTable<History>();
            lv = (ListView)FindViewById(Resource.Id.listView1);
            tester = (TextView)FindViewById(Resource.Id.textView4);
            translate = (ImageButton)FindViewById(Resource.Id.btnTerjemahkan);
            text = (EditText)FindViewById(Resource.Id.editText1);
            hapus = (ImageButton)FindViewById(Resource.Id.buttonHapus);
            spin = (Spinner)FindViewById(Resource.Id.spinner1);
            spin2 = (Spinner)FindViewById(Resource.Id.spinner2);
            swit = (ImageButton)FindViewById(Resource.Id.buttonSwitch);
            var client = new RestClient("https://google-translate1.p.rapidapi.com/language/translate/v2/languages");
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept-encoding", "application/gzip");
            request.AddHeader("x-rapidapi-key", "33645fc5e6mshc258f82b30b53cdp102e45jsn29f2da353c97");
            request.AddHeader("x-rapidapi-host", "google-translate1.p.rapidapi.com");
            IRestResponse response = client.Execute(request);
            JObject j = JObject.Parse(response.Content);
            JToken bahasa = j["data"]["languages"];
            //tester.SetText(bahasa[0]["language"].ToString(), null);
            for (int i=0; i<111; i++)
            {
                //fetchJSON(bahasa[i]["language"].ToString());
                Llist.Add(bahasa[i]["language"].ToString());
            }

            for (int i = 0; i < 111; i++)
            {
                /*var client2 = new RestClient("https://restcountries.eu/rest/v2/alpha/" + Llist[i]);
                var request2 = new RestRequest(Method.GET);
                IRestResponse response2 = client2.Execute(request2);
                JObject j2 = JObject.Parse(response.Content);
                JToken namaBahasa = j2["languages"];
                Blist.Add(namaBahasa[0]["nativeName"].ToString());*/
            }

            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, Llist);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spin.Adapter = adapter;
            spin.ItemSelected += spiner_Click;
            spin2.Adapter = adapter;
            spin2.ItemSelected += spiner_Click2;

            var listData = db.Table<History>();
            int index = 0;
            string[] history = new string[listData.Count()];
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line);
            if(listData == null){}
            else
            {
                foreach (var s in listData)
                {
                    adapter.Add(s.Awal + " => " + s.Akhir);
                    lv.Adapter = adapter;
                }
            }

            translate.Click += delegate
            {
                if(bahasaSource == bahasaTarget)
                {
                    Toast.MakeText(this, "Bahasa tidak boleh sama", ToastLength.Short).Show();
                }
                if (text.Text.ToString().Equals("") | text.Text.ToString().Equals(null))
                {
                    Toast.MakeText(this, "Masukkan kalimat", ToastLength.Short).Show();
                }
                else
                {
                    var client1 = new RestClient("https://google-translate1.p.rapidapi.com/language/translate/v2");
                    var request1 = new RestRequest(Method.POST);
                    string kalimat = text.Text.ToString();
                    kalimat.Replace(" ", "%20");
                    string data = "q=" + kalimat + "&source=" + bahasaSource + "&target=" + bahasaTarget;
                    request1.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request1.AddHeader("accept-encoding", "application/gzip");
                    request1.AddHeader("x-rapidapi-key", "33645fc5e6mshc258f82b30b53cdp102e45jsn29f2da353c97");
                    request1.AddHeader("x-rapidapi-host", "google-translate1.p.rapidapi.com");
                    request1.AddParameter("application/x-www-form-urlencoded", data, ParameterType.RequestBody);
                    IRestResponse response1 = client1.Execute(request1);
                    JObject j1 = JObject.Parse(response1.Content);
                    JToken bahasa1 = j1["data"]["translations"];
                    Intent i = new Intent(this, typeof(ActivityTranslate));
                    string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
                    var db = new SQLiteConnection(dbPath);
                    History history = new History();
                    history.Awal = kalimat;
                    history.Akhir = bahasa1[0]["translatedText"].ToString();
                    db.Insert(history);
                    i.PutExtra("awal", kalimat);
                    i.PutExtra("akhir", bahasa1[0]["translatedText"].ToString());
                    i.PutExtra("bahasaAwal", bahasaSource);
                    i.PutExtra("bahasaAkhir", bahasaTarget);
                    StartActivity(i);
                }     
            };

            hapus.Click += delegate
            {
                string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
                var db = new SQLiteConnection(dbPath);
                db.DeleteAll<History>();
                StartActivity(typeof(MainActivity));
            };

            swit.Click += delegate
            {
                spin.SetSelection(indexTarget);
                spin2.SetSelection(indexSource);
            };

            
           
        }

        private async void fetchJSON(string data)
        {
            try
            {
                RestClient restClient = new RestClient("https://restcountries.eu/rest/v2");
                RestRequest restRequest = new RestRequest("/alpha/" + data);
                IRestResponse restResponse = await restClient.ExecuteTaskAsync(restRequest);
                JObject jobject = JObject.Parse(restResponse.Content);
                JToken jGenre = jobject["languages"];
                Blist.Add(jGenre[0]["nativeName"].ToString());
            }
            catch(Exception e)
            {
                Blist.Add(data);
            }
            
        }

        private void spiner_Click(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            bahasaSource = spin.GetItemAtPosition(e.Position).ToString();
            indexSource = e.Position;
        }

        private void spiner_Click2(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            bahasaTarget = spin2.GetItemAtPosition(e.Position).ToString();
            indexTarget = e.Position;
        }

        public void createDB()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db = new SQLiteConnection(dbPath);
            db.CreateTable<History>();
        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public static void CustomerInfo(string json)
        {
            JArray a = JArray.Parse(json);
        }
    }
}