using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTS
{
    [Table("History")]
    public class History
    {

        private int id;
        private string awal;
        private string akhir;
        [PrimaryKey, AutoIncrement, Column("_id")]

        public int Id { get; set; }
        public string Awal { get; set; }
        public string Akhir { get; set; }
    }
}