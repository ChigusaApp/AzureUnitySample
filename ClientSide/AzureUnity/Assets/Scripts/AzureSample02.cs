using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class AzureSample02 : MonoBehaviour
{
    public Text debugText;



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

    private void Start()
    {
        //  Mono が HTTPS 接続でエラーを出すので、事前に回避できるようにするコールバック
        System.Net.ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
    }

    public void OnClick()
    {
        GetDoIt();
    }

    /// <summary>
    /// 処理
    /// </summary>
    public bool GetDoIt()
    {
        int serverIndex = 1;
        string addHeader = "x-chigusa-serverid:" + serverIndex;
        addHeader += "\n" + "x-chigusa-userid:" + "DammyId";
        addHeader += "\n" + "x-chigusa-appversion:" + Chigusa.AzureHelper.ApiAppREST.ChigusaVersion;
        var apiApp = new UnityApiApp(serverIndex);
        var request = apiApp.BuildGetRequest("GET", "values", "accountName", addHeader);
        request.SetRequestHeader("x-chigusa-serverid", serverIndex.ToString());
        request.SetRequestHeader("x-chigusa-userid", "DammyId");
        request.SetRequestHeader("x-chigusa-appversion", Chigusa.AzureHelper.ApiAppREST.ChigusaVersion);

        request.SendWebRequest().completed += new System.Action<AsyncOperation>(_ =>
        {
            Debug.Log(request.responseCode);
            Debug.Log(request.downloadHandler?.text);
            debugText.text = request.downloadHandler?.text;
        });

        return true;
    }

}
