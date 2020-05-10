using System;
using System.Net;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using TPClient.Models;

namespace TPClient.Classes
{
    public class DweetClient : IDisposable
    {
        private Action<Dweet> _dweetHandler;
        private CustomWebClient _webClient;

        private readonly string _thing;
        private bool _isListening;

        public DweetClient(string thing)
        {
            _thing = thing;
        }

        public bool ListenForDweets(Action<Dweet> dweetHandler)
        {
            _dweetHandler = dweetHandler;

            Log.Instance.Debug("Initialize CustomWebClient...");

            _webClient = new CustomWebClient
            {
                KeepAlive = true,
                Timeout = new TimeSpan(0, 0, 0, 3)
            };

            var uri = "https://dweet.io/listen/for/dweets/from/" + _thing;
            Log.Instance.Debug("Begin listening dweets from: {0}", uri);

            var wr = (HttpWebRequest)WebRequest.Create(uri);
            wr.ProtocolVersion = Version.Parse("1.0");

            var response = (HttpWebResponse)wr.GetResponse();

            try
            {
                _webClient.OpenReadCompleted += WebClient_OpenReadCompleted;
                _webClient.OpenReadAsync(new Uri(uri));
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }

            // If web client is busy, then connection is open
            _isListening = _webClient.IsBusy;

            return _isListening;
        }

        public void StopListeningForDweets()
        {
            Log.Instance.Debug("Stop listening dweets");

            try
            {
                if (_webClient != null)
                {
                    _webClient.OpenReadCompleted -= WebClient_OpenReadCompleted;
                    _webClient.CancelAsync();
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }

            _isListening = false;
        }

        public bool IsListeningForDweets()
        {
            return _isListening;
        }

        public void GetLastDweet()
        {
            var uri = "https://dweet.io/get/latest/dweet/for/" + _thing;
            Log.Instance.Debug("Get last dweet from: {0}", uri);

            try
            {
                using (var wc = new CustomWebClient())
                {
                    wc.OpenReadCompleted += WebClient_OpenReadCompleted;
                    wc.OpenReadAsync(new Uri(uri));
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }
        }

        protected void WebClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                    throw e.Error;

                using (var reader = new StreamReader(e.Result))
                {
                    while (!reader.EndOfStream)
                    {
                        try
                        {
                            var ln = reader.ReadLine();

                            // Starts with a quote so assume it's a JSON encoded string
                            if (ln.StartsWith("\""))
                            {
                                ln = JsonConvert.DeserializeObject<String>(ln);
                            }

                            // Json object so parse as a dweet
                            if (ln.StartsWith("{") || ln.StartsWith("["))
                            {
                                Log.Instance.Debug("Recieved dweet! Trying to deserialize...");

                                if (ln.Contains("this") && ln.Contains("by") && ln.Contains("the") && ln.Contains("with"))
                                {
                                    var lastDweet = JsonConvert.DeserializeObject<DweetLast>(ln);

                                    if (_dweetHandler != null)
                                    {
                                        Log.Instance.Debug("Dweet type is <DweetLast>");

                                        var dweet = null as Dweet;

                                        if (lastDweet.With.Count > 0)
                                            dweet = lastDweet.With[0];

                                        _dweetHandler(dweet);
                                    }
                                    else
                                    {
                                        Log.Instance.Warn("Unknown dweet type");
                                    }
                                }
                                else
                                {
                                    var dweet = JsonConvert.DeserializeObject<Dweet>(ln);

                                    if (_dweetHandler != null)
                                    {
                                        Log.Instance.Debug("Dweet type is <Dweet>");
                                        _dweetHandler(dweet);
                                    }
                                    else
                                    {
                                        Log.Instance.Warn("Unknown dweet type");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.Warn(ex);
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                Log.Instance.Error(ex2);
            }
            finally
            {
                if (_webClient == (CustomWebClient)sender)
                {
                    ListenForDweets(_dweetHandler);
                }
            }
        }

        public void Dispose()
        {
            StopListeningForDweets();

            if (_webClient != null)
                _webClient.Dispose();
        }
    }
}