using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CreateKeys : MonoBehaviour
{
    /// <summary>
    /// キーサイズ
    /// </summary>
    const int KeySize = 2048;

    /// <summary>
    /// 公開鍵と秘密鍵とハッシュキーを作成して返す
    /// </summary>
    /// <param name="publicKey">作成された公開鍵(XML形式)</param>
    /// <param name="privateKey">作成された秘密鍵(XML形式)</param>
    /// <param name="hashKey">作成されたハッシュキー(XML形式)</param>
    public static void Create(out string publicKey, out string privateKey, out string hashKey)
    {
        //  RSACryptoServiceProviderオブジェクトの作成
        var rsa = new RSACryptoServiceProvider(KeySize);

        //  公開鍵をXML形式で取得
        publicKey = rsa.ToXmlString(false);
        //  秘密鍵をXML形式で取得
        privateKey = rsa.ToXmlString(true);

        //  ハッシュキー  
        byte[] secretkey = new Byte[64];
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        rng.GetBytes(secretkey);
        hashKey = Convert.ToBase64String(secretkey);
    }

    // Start is called before the first frame update
    void Start()
    {
        Create(out string publicKey, out string privateKey, out string hashKey);
        Debug.Log(publicKey);
        Debug.Log(privateKey);
        Debug.Log(hashKey);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
