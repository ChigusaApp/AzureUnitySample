using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;

namespace Chigusa.AzureHelper
{
    /// <summary>
    /// ApiApp向けクラス
    /// </summary>
    public class ApiApp
    {
        /// <summary>
        /// REST ユーティリティー
        /// </summary>
        protected readonly ApiAppREST rest = null;
        /// <summary>
        /// サーバーと共通のキー（自作）
        /// </summary>
        private readonly static string HashKeys = "sOPaWjT89lMARUR4cnYClFLRuUglbt7PbS4t46WnV/gkyKxsB4YL4ILWDyQx/ZWkC3R8K4Dgu2XZHuFWQ07bBA==";
        /// <summary>
        /// エンドポイント
        /// </summary>
        private readonly static string[] EndPoint = { "https://localhost:44374/api/", "http://azureunitysample01.azurewebsites.net/api/", };


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="serverIndex">サーバー番号</param>
        public ApiApp(int serverIndex)
        {
            rest = new ApiAppREST(HashKeys, EndPoint[serverIndex]);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="serverIndex">サーバー番号</param>
        public ApiApp(string endPoint)
        {
            rest = new ApiAppREST(HashKeys, endPoint);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="hashKey">サーバーとのハッシュキー</param>
        /// <param name="endPoint">サーバーのエンドポイント</param>
        public ApiApp(string hashKey, string endPoint)
        {
            rest = new ApiAppREST(hashKey, endPoint);
        }



        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="hashKey">サーバーとのハッシュキー</param>
        /// <param name="endPoint">サーバーのエンドポイント</param>
        public string GetEndPoint()
        {
            if (rest == null)
                return "";
            return rest.EndPoint;
        }



        /// <summary>
        /// 基本的なリクエストを作成する
        /// </summary>
        /// <param name="requestMethod">GET, POST, PUT, DELETE メソッド</param>
        /// <param name="appName">app名</param>
        /// <param name="accountName">アカウント名</param>
        /// <returns>リクエスト</returns>
        public HttpWebRequest BuildBaseRequest(string requestMethod, string appName, string accountName, string addHeader = "")
        {
            if (rest == null)
                return null;

            //  要求する情報から権限付与ヘッダーの作成
            String dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            String authorizationHeader = rest.GetAuthorizationParameter(requestMethod, dateInRfc1123Format, ApiAppREST.ChigusaVersion, accountName, addHeader);

            //  リクエストの作成
            string urlPath = appName + "/" + accountName;
            Uri uri = new Uri(rest.EndPoint + urlPath);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = requestMethod;
            request.Headers.Add("x-chigusa-date", dateInRfc1123Format);
            request.Headers.Add("x-chigusa-version", ApiAppREST.ChigusaVersion);
            request.Headers.Add("Authorization", authorizationHeader);
            request.Headers.Add("Accept-Charset", "UTF-8");
            request.Accept = "application/json";
            //request.Timeout = 60 * 1000;
            return request;
        }

    }

}
