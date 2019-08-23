using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class AzureSample01 : MonoBehaviour
{
    /// <summary>
    /// Mono が HTTPS 接続でエラーを出すので、事前に回避できるようにするコールバック
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="certificate"></param>
    /// <param name="chain"></param>
    /// <param name="policyErrors"></param>
    /// <returns></returns>
    private static bool ValidateRemoteCertificate(
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        System.Net.Security.SslPolicyErrors policyErrors
    )
    {
        if (policyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors)
        {
            if (certificate.Subject == "CN=*.azurewebsites.net")
                return true;
            else if (certificate.Subject == "CN=*.blob.core.windows.net")
                return true;
            else if (certificate.Subject == "CN=*.table.core.windows.net")
                return true;
            else if (certificate.Subject == "CN=*.documents.azure.com")
                return true;
            else if (certificate.Subject.StartsWith("CN=localhost"))
                return true;
        }
        else if (policyErrors == System.Net.Security.SslPolicyErrors.None)
        {
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //  Mono が HTTPS 接続でエラーを出すので、事前に回避できるようにするコールバック
        System.Net.ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;

        GetDoIt();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 処理
    /// </summary>
    public bool GetDoIt()
    {
        int serverIndex = 0;
        string addHeader = "x-chigusa-serverid:" + serverIndex;
        addHeader += "\n" + "x-chigusa-userid:" + "DammyId";
        addHeader += "\n" + "x-chigusa-appversion:" + Chigusa.AzureHelper.ApiAppREST.ChigusaVersion;
        var apiApp = new Chigusa.AzureHelper.ApiApp(serverIndex);
        var request = apiApp.BuildBaseRequest("GET", "values", "accountName", addHeader);
        request.Headers.Add("x-chigusa-serverid", serverIndex.ToString());
        request.Headers.Add("x-chigusa-userid", "DammyId");
        request.Headers.Add("x-chigusa-appversion", Chigusa.AzureHelper.ApiAppREST.ChigusaVersion);
        
        using (var res = request.GetResponse() as System.Net.HttpWebResponse)
        {
            if (res.StatusCode != System.Net.HttpStatusCode.OK)
                return false;
            var sr = new System.IO.StreamReader(res.GetResponseStream());
            var con = sr.ReadToEnd();
            sr.Close();
            Debug.Log(con);
        }
        return true;
    }

}
