using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;

namespace TTS
{
    [Activity(Label = "ActivityTranslate")]
    public class ActivityTranslate : Activity
    {
        private TextView txt1, txt2, languageAwal, languageAkhir;
        private ImageButton back;
        private ListView lv;
        private ArrayAdapter<string> adapter;
        private string awal, akhir;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_translate);

            //Inisialisasi komponen
            languageAwal = (TextView)FindViewById(Resource.Id.textView1);
            languageAkhir = (TextView)FindViewById(Resource.Id.textView2);
            txt1 = (TextView)FindViewById(Resource.Id.textViewSource);
            txt2 = (TextView)FindViewById(Resource.Id.textView4);
            back = (ImageButton)FindViewById(Resource.Id.buttonKembali);
            lv = (ListView)FindViewById(Resource.Id.listView1);
            //Data dari activity sebelah
            awal = Intent.GetStringExtra("awal");
            akhir = Intent.GetStringExtra("akhir");
            string bahasaAwal = Intent.GetStringExtra("bahasaAwal");
            string bahasaAkhir = Intent.GetStringExtra("bahasaAkhir");

            //Bahasa awal dan akhir
            txt1.SetText(awal.ToString(), null);
            txt2.SetText(akhir.ToString(), null);

            //API menerjemahkan kode bahasa ke nama bahasa
            RestClient restClient = new RestClient("https://restcountries.eu/rest/v2");
            RestRequest restRequest = new RestRequest("/alpha/" + bahasaAwal);
            RestRequest restRequest2 = new RestRequest("/alpha/" + bahasaAkhir);
            IRestResponse restResponse = restClient.Execute(restRequest);
            IRestResponse restResponse2 = restClient.Execute(restRequest2);
            JObject jobject = JObject.Parse(restResponse.Content);
            JToken jtoken = jobject["languages"];
            JObject jobject2 = JObject.Parse(restResponse2.Content);
            JToken jtoken2 = jobject2["languages"];
            //proteksi bila API tidak menemukan nama bahasa awal 
            try
            {
                languageAwal.SetText(jtoken[0]["nativeName"].ToString(), null);
            }
            catch (NullReferenceException e)
            {
                languageAwal.SetText(bahasaAwal, null);
            }
            //proteksi bila API tidak menemukan nama bahasa akhir
            try
            {
                languageAkhir.SetText(jtoken2[0]["nativeName"].ToString(), null);
            }
            catch (NullReferenceException e)
            {
                languageAkhir.SetText(bahasaAkhir, null);
            }

            //API alternatif translate
            var client = new RestClient("https://translated-mymemory---translation-memory.p.rapidapi.com/api/get?langpair=" + bahasaAwal.ToString() + "%7C" + bahasaAkhir.ToString() + "&q=" + akhir.Replace(" ", "%20") + "&mt=1&onlyprivate=0&de=a%40b.c");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-key", "33645fc5e6mshc258f82b30b53cdp102e45jsn29f2da353c97");
            request.AddHeader("x-rapidapi-host", "translated-mymemory---translation-memory.p.rapidapi.com");
            IRestResponse response = client.Execute(request);
            JObject j = JObject.Parse(response.Content);
            JToken bahasa = j["matches"];
            var panjang = JObject.Parse(response.Content);

            JArray items = (JArray)panjang["matches"];
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line);

            //perulangan sebanyak data alternatif translate dan memasukkannya ke listview
            for (int i = 0; i < items.Count; i++)
            {
                if (bahasa[i]["translation"].ToString().Equals(null) | bahasa[i]["translation"].ToString().Equals(""))
                {
                    adapter.Add("Tidak ada alternatif");
                }
                else
                {
                    adapter.Add(bahasa[i]["translation"].ToString());
                }
                lv.Adapter = adapter;
            }

            //Tombol kembali
            back.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };
        }
    }
}