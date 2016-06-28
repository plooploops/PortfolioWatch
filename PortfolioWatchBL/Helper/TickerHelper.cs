using PortfolioWatchBO.Containers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioWatchBL.Helper
{
    public class TickerHelper
    {
        #region Constants

        /// <summary>
        /// TICKER, MONTH (Ordinal), DAY, YEAR
        /// </summary>
        public const string FROM_DATE_YAHOO_CSV_SOURCE_URL = @"http://ichart.yahoo.com/table.csv?s={0}&a={1}&b={2}&c={3}";

        /// <summary>
        /// TICKER
        /// </summary>
        public const string ALL_HISTORY_YAHOO_CSV_SOURCE_URL = @"http://ichart.yahoo.com/table.csv?s={0}";

        /// <summary>
        /// ERROR FETCHING DATA.
        /// </summary>
        public const string ERROR_FETCHING_DATA = "Error fetching data.  Please ensure proper connectivity and that the tickers are correct!";

        #endregion

        #region Members

        object sync = new object();
        HttpWebRequest myRequest;
        string responseData;

        #endregion

        #region Helper Methods

        public List<DailyTicker> ReadTickerData(string tickerName, DateTime? dt = null)
        {
            List<DailyTicker> ret = null;
            try
            {
                //This assumes that the ticker is valid.
                var results = FetchTickerData(tickerName, dt);
                if (results == ERROR_FETCHING_DATA)
                {
                    return ret;
                }

                var tickerData = results.Split('\n').Where(_ => !String.IsNullOrEmpty(_) && !_.ToLower().StartsWith("date"));
                ret = tickerData.Select(_ =>
                {
                    var tickerDataParts = _.Split(',').Where(lp => !String.IsNullOrEmpty(lp)).ToList();
                    return new DailyTicker()
                    {
                        Ticker = tickerName,
                        Date = DateTime.Parse(tickerDataParts[0]),
                        Open = decimal.Parse(tickerDataParts[1]),
                        High = decimal.Parse(tickerDataParts[2]),
                        Low = decimal.Parse(tickerDataParts[3]),
                        Close = decimal.Parse(tickerDataParts[4]),
                        Volume = decimal.Parse(tickerDataParts[5]),
                        AdjClose = decimal.Parse(tickerDataParts[6]),
                        MarketPrice = decimal.Parse(tickerDataParts[6]) //assumes market price is adjusted close.
                };
                }).ToList();
                //convert to containers.
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return ret;
        }

        public string FetchTickerData(string tickerName, DateTime? dt = null)
        {
            try
            {
                lock (sync)
                {
                    responseData = string.Empty;
                    Task parentTask = Task.Run(() =>
                        {
                            try
                            {
                                HttpWebRequest request = WebRequest.CreateHttp(FormatUrl(tickerName, dt));
                                request.Method = "GET";
                                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                                {
                                    using (Stream streamResponse = response.GetResponseStream())
                                    {
                                        using (StreamReader streamRead = new StreamReader(streamResponse))
                                        {
                                            responseData = streamRead.ReadToEnd();
                                            // Close the stream object
                                            // Release the HttpWebResponse
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                responseData = ERROR_FETCHING_DATA;
                            }
                        }
                    );
                    parentTask.Wait();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                responseData = ERROR_FETCHING_DATA;
            }
            return responseData;
        }

        protected string FormatUrl(string tickerName, DateTime? date = null)
        {
            if (date != null && date.HasValue)
            {
                DateTime dt = date.Value;
                return String.Format(FROM_DATE_YAHOO_CSV_SOURCE_URL, tickerName, (dt.Month - 1).ToString(), dt.Day.ToString(), dt.Year.ToString());
            }
            else
            {
                return String.Format(ALL_HISTORY_YAHOO_CSV_SOURCE_URL, tickerName);
            }
        }

        protected void GetRequest(string url)
        {
            myRequest = WebRequest.CreateHttp(url);
            myRequest.Method = "GET";
            myRequest.BeginGetResponse(GetResponseCallback, myRequest);
        }

        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                WebResponse resp = myRequest.EndGetResponse(asynchronousResult);
                HttpWebResponse response = (HttpWebResponse)resp;
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);
                string responseString = streamRead.ReadToEnd();
                // Close the stream object
                streamResponse.Close();
                streamRead.Close();
                // Release the HttpWebResponse
                response.Close();

                //Do whatever you want with the returned "responseString"
                Console.WriteLine(responseString);

                responseData = responseString;
            }
            catch (Exception ex)
            {
                //issue with getting the response.
            }
        }

        #endregion
    }
}
