using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTS
{
    [Activity(Label = "ActivityTranslate")]
    public class ActivityTranslate : Activity
    {
        TextView txt1, txt2;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_translate);
            // Create your application here
            txt1 = (TextView)FindViewById(Resource.Id.textViewSource);
            txt2 = (TextView)FindViewById(Resource.Id.textViewTarget);

            var awal = Intent.GetStringExtra("awal");
            var akhir = Intent.GetStringExtra("akhir");
            txt1.SetText(awal.ToString(), null);
            txt2.SetText(akhir.ToString(), null);

        }
    }
}