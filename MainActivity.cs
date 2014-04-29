using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AndroidSMSExporter
{
    [Activity(Label = "AndroidSMSExporter", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            Init();
        }

        async void Init()
        {
            var progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            var textStatus = FindViewById<TextView>(Resource.Id.textStatus);

            try
            {
                var cursor = ContentResolver.Query(Android.Net.Uri.Parse("content://sms"), null,
                    null, null, null);

                List<string> rows = new List<string>();
                var columns = cursor.GetColumnNames();
                var rowsCount = cursor.Count;
                var rowsDone = 0;

                progressBar.Max = rowsCount;

                var client = new HttpClient();
                var uri = new Uri("http://192.168.1.9:1337/sms");

                while (cursor.MoveToNext())
                {
                    var sb = new StringBuilder();
                    using (var writer = new JsonTextWriter(new StringWriter(sb)))
                    {
                        writer.WriteStartObject();
                        for (int i = 0; i < columns.Length; i++)
                        {
                            writer.WritePropertyName(columns[i]);
                            writer.WriteValue(cursor.GetString(i));
                        }
                        writer.WriteEndObject();
                    }

                    var json = sb.ToString();
                    var data = Encoding.UTF8.GetBytes(json);
                    var content = new ByteArrayContent(data);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    await client.PostAsync(uri, content);

                    progressBar.Progress = ++rowsDone;
                    textStatus.Text = String.Format("{0} of {1} posted.", rowsDone, rowsCount);
                }
            }
            catch (Exception ex)
            {
                textStatus.Text = ex.Message;
            }
        }
    }
}

