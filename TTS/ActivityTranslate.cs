using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTS
{
    [Activity(Label = "ActivityTranslate")]
    public class ActivityTranslate : Activity
    {
        TextView txt1, txt2, languageAwal, languageAkhir;
        Button btn;
        ImageButton back;
        ListView lv;
        private ArrayAdapter<string> adapter;
        public string awal, akhir;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_translate);

            languageAwal = (TextView)FindViewById(Resource.Id.textView1);
            languageAkhir = (TextView)FindViewById(Resource.Id.textView2);
            txt1 = (TextView)FindViewById(Resource.Id.textViewSource);
            txt2 = (TextView)FindViewById(Resource.Id.textView4);
            btn = (Button)FindViewById(Resource.Id.button1);
            back = (ImageButton)FindViewById(Resource.Id.buttonKembali);
            awal = Intent.GetStringExtra("awal");
            akhir = Intent.GetStringExtra("akhir");
            string bahasaAwal = Intent.GetStringExtra("bahasaAwal");
            string bahasaAkhir = Intent.GetStringExtra("bahasaAkhir");
            txt1.SetText(awal.ToString(), null);
            txt2.SetText(akhir.ToString(), null);
            languageAwal.SetText(bahasaAwal.ToString(), null);
            languageAkhir.SetText(bahasaAkhir.ToString(), null);
            lv = (ListView)FindViewById(Resource.Id.listView1);
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line);

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
            
            for (int i=0; i<items.Count; i++)
            {
                if(bahasa[i]["translation"].ToString().Equals(null) | bahasa[i]["translation"].ToString().Equals(""))
                {
                    adapter.Add("Tidak ada alternatif");
                }
                else
                {
                    adapter.Add(bahasa[i]["translation"].ToString());
                }
                lv.Adapter = adapter;
            }

            back.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };
        }

        public void bahasaPilihan()
        {
            var client = new RestClient("https://microsoft-translator-text.p.rapidapi.com/languages?api-version=3.0");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-key", "33645fc5e6mshc258f82b30b53cdp102e45jsn29f2da353c97");
            request.AddHeader("x-rapidapi-host", "microsoft-translator-text.p.rapidapi.com");
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content);
        }
    }
}