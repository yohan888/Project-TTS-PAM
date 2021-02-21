using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Android.Content;
using System.IO;
using SQLite;

namespace TTS
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private ImageButton translate, hapus, swit;
        private EditText text;
        private ListView lv;
        private Spinner spin, spin2;
        private ArrayAdapter adapterSpinner;
        private ArrayAdapter<string> adapter;
        private List<string> Llist = new List<string>();
        private string bahasaSource, bahasaTarget;
        private int indexSource, indexTarget;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //Pembuatan database
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db = new SQLiteConnection(dbPath);
            db.CreateTable<History>();

            //inisialisasi komponen 
            lv = (ListView)FindViewById(Resource.Id.listView1);
            translate = (ImageButton)FindViewById(Resource.Id.btnTerjemahkan);
            text = (EditText)FindViewById(Resource.Id.editText1);
            hapus = (ImageButton)FindViewById(Resource.Id.buttonHapus);
            spin = (Spinner)FindViewById(Resource.Id.spinner1);
            spin2 = (Spinner)FindViewById(Resource.Id.spinner2);
            swit = (ImageButton)FindViewById(Resource.Id.buttonSwitch);

            //API list kode bahasa (111 bahasa)
            var client = new RestClient("https://google-translate1.p.rapidapi.com/language/translate/v2/languages");
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept-encoding", "application/gzip");
            request.AddHeader("x-rapidapi-key", "33645fc5e6mshc258f82b30b53cdp102e45jsn29f2da353c97");
            request.AddHeader("x-rapidapi-host", "google-translate1.p.rapidapi.com");
            IRestResponse response = client.Execute(request);
            JObject j = JObject.Parse(response.Content);
            JToken bahasa = j["data"]["languages"];
            for (int i = 0; i < 111; i++)
            {
                Llist.Add(bahasa[i]["language"].ToString());
            }

            //Set spinner berdasarkan data dari API
            adapterSpinner = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, Llist);
            adapterSpinner.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spin.Adapter = adapterSpinner;
            spin.ItemSelected += spiner_Click;
            spin2.Adapter = adapterSpinner;
            spin2.ItemSelected += spiner_Click2;

            //Mengisi listview history
            var listData = db.Table<History>();
            int index = 0;
            string[] history = new string[listData.Count()];
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line);
            if (listData == null) { }
            else
            {
                foreach (var s in listData)
                {
                    adapter.Add(s.Awal + " => " + s.Akhir);
                    lv.Adapter = adapter;
                }
            }

            //Tombol Terjemahkan
            translate.Click += delegate
            {
                //proteksi jika pemilihan bahasa sama
                if (bahasaSource == bahasaTarget)
                {
                    Toast.MakeText(this, "Bahasa tidak boleh sama", ToastLength.Short).Show();
                }
                //proteksi jika teks masih kosong
                if (text.Text.ToString().Equals("") | text.Text.ToString().Equals(null))
                {
                    Toast.MakeText(this, "Masukkan kalimat", ToastLength.Short).Show();
                }
                else
                {
                    //API translate
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

                    //Memasukkan translate tadi ke history
                    string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
                    var db = new SQLiteConnection(dbPath);
                    History history = new History();
                    history.Awal = kalimat;
                    history.Akhir = bahasa1[0]["translatedText"].ToString();
                    db.Insert(history);

                    //Data untuk translate dikirim ke activity lain
                    Intent i = new Intent(this, typeof(ActivityTranslate));
                    i.PutExtra("awal", kalimat);
                    i.PutExtra("akhir", bahasa1[0]["translatedText"].ToString());
                    i.PutExtra("bahasaAwal", bahasaSource);
                    i.PutExtra("bahasaAkhir", bahasaTarget);
                    StartActivity(i);
                }
            };

            //Tombol hapus history
            hapus.Click += delegate
            {
                string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
                var db = new SQLiteConnection(dbPath);
                db.DeleteAll<History>();
                StartActivity(typeof(MainActivity));
            };

            //Tombol switch bahasa
            swit.Click += delegate
            {
                spin.SetSelection(indexTarget);
                spin2.SetSelection(indexSource);
            };
        }

        //Method saat spinner dipilih
        private void spiner_Click(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            bahasaSource = spin.GetItemAtPosition(e.Position).ToString();
            indexSource = e.Position;
        }

        //Method saat spinner dipilih
        private void spiner_Click2(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            bahasaTarget = spin2.GetItemAtPosition(e.Position).ToString();
            indexTarget = e.Position;
        }

        //Method bawaan
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}