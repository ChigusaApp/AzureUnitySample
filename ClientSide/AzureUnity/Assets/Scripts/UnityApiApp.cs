using Chigusa.AzureHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityApiApp : ApiApp
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="serverIndex">サーバー番号</param>
    public UnityApiApp(int serverIndex) : base(serverIndex)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="serverIndex">サーバー番号</param>
    public UnityApiApp(string endPoint) : base(endPoint)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="hashKey">サーバーとのハッシュキー</param>
    /// <param name="endPoint">サーバーのエンドポイント</param>
    public UnityApiApp(string hashKey, string endPoint) : base(hashKey, endPoint)
    {
    }



    public UnityEngine.Networking.UnityWebRequest BuildGetRequest(string requestMethod, string appName, string accountName, string addHeader = "")
    {
        if (rest == null)
            return null;

        //  要求する情報から権限付与ヘッダーの作成
        string dateInRfc1123Format = System.DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
        string authorizationHeader = rest.GetAuthorizationParameter(requestMethod, dateInRfc1123Format, Chigusa.AzureHelper.ApiAppREST.ChigusaVersion, accountName, addHeader);

        //  リクエストの作成
        string urlPath = appName + "/" + accountName;
        System.Uri uri = new System.Uri(rest.EndPoint + urlPath);
        var request = UnityEngine.Networking.UnityWebRequest.Get(uri);
        request.SetRequestHeader("x-chigusa-date", dateInRfc1123Format);
        request.SetRequestHeader("x-chigusa-version", Chigusa.AzureHelper.ApiAppREST.ChigusaVersion);
        request.SetRequestHeader("Authorization", authorizationHeader);
        return request;
    }


}
