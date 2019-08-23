using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Chigusa.AzureHelper
{
    /// <summary>
    /// REST 向けユーティリティー
    /// </summary>
    public class ApiAppREST
    {
        /// <summary>
        /// サーバーと共通のキー（自作）
        /// </summary>
        readonly string HashKey = "";

        /// <summary>
        /// エンドポイント
        /// </summary>
        readonly public string EndPoint = "";

        /// <summary>
        /// バージョン
        /// </summary>
        public const string ChigusaVersion = "2019-08-21";

        /// <summary>
        /// キーサイズ
        /// </summary>
        const int KeySize = 2048;

        /// <summary>
        /// XML形式の公開鍵
        /// </summary>
        const string accessPublicKey = "<RSAKeyValue><Modulus>qC+Br1QZDbrScpITTya4vBKDzDkmNR9ud5ZpL9xkn2M6x989Ffh7MKTZPh+qpKux2obJ8UFfTUKK/rhtWTwCV4BdwAeqdPKyHNKETV/xCNa7TuFZ+WHjXa7V5it3HQL372Okb1se5Gq6x0aQ96QQ64I+SEsa+ZdIFO5gQ3R9+G9Hutv/WWOnBgAdkFV/mMkLw7hoSHgZJG6vHO6PvVJRQraAMSVtMnfoD9bdPXyFylBzMHAYda3Y0KFk3ZXAysFkGb92Z63LtUbHqtlfVwSBYt/s0vEphZFNgafVyCnzr/KJzOgTAqPXcyBouPEl3On2a45Bk7YY2G5OtIXOU0dHWw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        /// <summary>
        /// XML形式の公開鍵
        /// <para/>送受信用
        /// </summary>
        const string clientPublicKey = "<RSAKeyValue><Modulus>6PWU2ZkwP7QGIPCA3tYbzTWYOe4bkfScwdQxqNrrQVZ5QOLZlXGpBIhYoSyeOD8inH3KsTILsfjBKMovFYmunvq6g1vHdkDB6Eyl4N1SIa/nrL1c8QR6GAAfbipHpFOwLxoC292k6+VshzoM2xiZllxGw/p4PMvDoA2pFq3jV3gl0RFNKKUbKFZKPvmqie8Z2LKe0n3uw2FXFHWqcJUMrgv28bUlu7VNt+Ou4kTTBNNMPsfOhlZqg/fb0fHGoLGffLW3ZTWSNNH8qiceAP3IVLJRSPHmyzDUE92rqxdCPhrmCiapm9z0rleTH1ZJi1ox1uWg87KfJMQJMQcpvxP44Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        /// <summary>
        /// XML形式の秘密鍵
        /// <para/>送受信用
        /// </summary>
        const string clientPrivateKey = "<RSAKeyValue><Modulus>6PWU2ZkwP7QGIPCA3tYbzTWYOe4bkfScwdQxqNrrQVZ5QOLZlXGpBIhYoSyeOD8inH3KsTILsfjBKMovFYmunvq6g1vHdkDB6Eyl4N1SIa/nrL1c8QR6GAAfbipHpFOwLxoC292k6+VshzoM2xiZllxGw/p4PMvDoA2pFq3jV3gl0RFNKKUbKFZKPvmqie8Z2LKe0n3uw2FXFHWqcJUMrgv28bUlu7VNt+Ou4kTTBNNMPsfOhlZqg/fb0fHGoLGffLW3ZTWSNNH8qiceAP3IVLJRSPHmyzDUE92rqxdCPhrmCiapm9z0rleTH1ZJi1ox1uWg87KfJMQJMQcpvxP44Q==</Modulus><Exponent>AQAB</Exponent><P>+OcZ0w2Ycs/KqMYKXZufDTmdxGcrXpJIyZugqjL0keG1uW2NnEMGI/K1qHdZ1yMNYxAlYtYYGDtLphD3mSswALeCK9xy3enf+UKD9yueWayPzdtXxm/Nsc9KJPeTZDwtTmrsSOKjLlb8jhgWTrDCkQBeQpw0hAOquNd9l1Ki7hs=</P><Q>75oZbHbPd//YRZPq1HpA1hntA6eCJ+9Lp8uqhy7aFvsLX/25o2ABD+GxkxtBEc9KprlkIGoB1ImcbQAnhnqjafFfpeWDF/A6pOPI4V4kaxsJfDBnDybxocV3CztmuBea8gw28XShpmKihr8RS2A8MsbzSUl6TbfXQY2z1uD+NLM=</Q><DP>pbeoV/6rS3XRpoEEkcJ1KSb9RbCzDWo0EBcP54G5mA9BIM4yBKITSofkLuAX7scluJkdayrELA3+lfiiAVbhxPhpMK67w8hdGOYSWtStv2LG8/ZgAHyb5RDweqBjf88ZEybZXsWWg9nimPCsmYPSZxxppcu+o06Vsi+3LLMWS5U=</DP><DQ>ttIoUACf8XpANWbWKeZWjocduEoaIAqQ+amHprpzIlHPriDVgvmAFfQqIIsNLV+0IF8ZLTp1xwxxVSJnBk+RXQcV6mmji6J7vNEpt/yzYR4yMJZmLMOUX9FiMinTCOjKC6KSUc6igWiFhrdHpPH7POtdOzBbp+18y8Ip1O28Sc0=</DQ><InverseQ>KHvK1GC5G/FyPlXq29MMHKGzaH0lJsHFn+8pCDUqjZRWx3JLb5qaJCF1xanpJuTfxT5PyjUB0m92VSZzHYrlZwCxSY/84LmYspUbf0hg9WKVKn997I5zrYH8/7DmYx62P6DNZvmfbj8XG2uD+DF4FJ4+O+L1BJNumv6oMVFjy+I=</InverseQ><D>YDzj6yjPt+FlDsKMWoJVJedYJcZp+Vf2UlSY10rpGSriGJ4eiRBVZJv3EU5fZpkecUu+KomkLze2hGVSIjGow+CMoKuPsRh7gr33YPfNcEE9Ei2AcckJr1SNp3Mr+YThKfQy7iWYdiNnPE4M9Y4qFOW2Dkww09zRyhMaoVXgr0cXeSEgvZLRoDMoaWn5YOjP+x5EKMy15UkCMRF0xVfL8LBe56QLBHMYGtRUxrpJCq2E/O5pJnDi+cVJh+7+MHEPndrR6SjkLM3H6+yF+gWVJ/sI9I1eNC4XiWt4zlwsR6sw35TxqIK34N8+vA8dSwzuYN0p/Yowl6VEe4ymV6tvKQ==</D></RSAKeyValue>";



        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="hashKey">サーバーとのハッシュキー</param>
        /// <param name="endPoint">サーバーのエンドポイント</param>
        public ApiAppREST(string hashKey, string endPoint)
        {
            HashKey = hashKey;
            EndPoint = endPoint;
        }

        /// <summary>
        /// 権限付与ヘッダーの作成
        /// </summary>
        /// <param name="canonicalizedString">正規化された文字列</param>
        /// <returns>ヘッダー文字列</returns>
        private String CreateAuthorizationHeader(String canonicalizedString, string commonId)
        {
            if (String.IsNullOrEmpty(canonicalizedString))
            {
                throw new ArgumentNullException("canonicalizedString");
            }

            String signature = CreateHmacSignature(canonicalizedString, Convert.FromBase64String(HashKey));
            String authorizationHeader = String.Format(CultureInfo.InvariantCulture, "{0} {1}", commonId, signature);

            return authorizationHeader;
        }


        /// <summary>
        /// ハッシュキーで署名文字列を作成する
        /// </summary>
        /// <param name="unsignedString">署名前の文字列</param>
        /// <param name="key">ハッシュキー</param>
        /// <returns>署名済み文字列</returns>
        private static String CreateHmacSignature(String unsignedString, Byte[] key)
        {
            if (String.IsNullOrEmpty(unsignedString))
            {
                throw new ArgumentNullException("unsignedString");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(unsignedString);
            using (HMACSHA256 hmacSha256 = new HMACSHA256(key))
            {
                return Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
            }
        }


        /// <summary>
        /// 要求する情報から権限付与ヘッダーの作成
        /// </summary>
        /// <param name="requestMethod">メソッド</param>
        /// <param name="requestDate">日付</param>
        /// <param name="requestVersion">バージョン</param>
        /// <param name="requestAccountName">アカウント名</param>
        /// <returns>権限付与ヘッダー</returns>
        public string GetAuthorizationParameter(string requestMethod, string requestDate, string requestVersion, string requestAccountName, string addCanonicalizedHeaders = "")
        {
            if (requestMethod == null || requestDate == null || requestVersion == null || requestAccountName == null)
                throw new ArgumentNullException("args");

            String canonicalizedHeaders = String.Format("x-chigusa-date:{0}\nx-chigusa-version:{1}", requestDate, requestVersion);
            if (!String.IsNullOrEmpty(addCanonicalizedHeaders))
                canonicalizedHeaders += "\n" + addCanonicalizedHeaders;
            String canonicalizedResource = String.Format("/{0}", requestAccountName);
            String stringToSign = String.Format("{0}\n{1}\n{2}", requestMethod, canonicalizedHeaders, canonicalizedResource);
            String authorizationHeader = CreateAuthorizationHeader(stringToSign, requestAccountName);
            return authorizationHeader;
        }



        #region Encrypt and Decrypt
        /// <summary>
        /// 公開鍵を使って文字列を暗号化する
        /// </summary>
        /// <param name="str">暗号化する文字列</param>
        /// <returns>暗号化された文字列</returns>
        public static string Encrypt(string str)
        {
            try
            {
                //RSACryptoServiceProviderオブジェクトの作成
                var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(KeySize);

                //公開鍵を指定
                rsa.FromXmlString(accessPublicKey);

                //暗号化する文字列をバイト配列に
                byte[] data = System.Text.Encoding.UTF8.GetBytes(str);

                //  データサイズのチェック
                if (data.Length > rsa.KeySize / 8 - 11)
                    throw new Exception();

                //暗号化する
                //（XP以降の場合のみ2項目にTrueを指定し、OAEPパディングを使用できる）
                //  RSACryptoServiceProvider.KeySize プロパティで取得できるキーサイズが1024ビットだとすると、暗号化されるデータの最大長は、 1024/8-11=117バイト となります。
                byte[] encryptedData = rsa.Encrypt(data, false);

                //Base64で結果を文字列に変換
                return System.Convert.ToBase64String(encryptedData);
            }
            catch (Exception)
            {
            }

            return null;
        }


        /// <summary>
        /// 公開鍵を使って文字列を暗号化する
        /// </summary>
        /// <param name="str">暗号化する文字列</param>
        /// <returns>暗号化された文字列</returns>
        public static string ClientEncrypt(string str)
        {
            try
            {
                //RSACryptoServiceProviderオブジェクトの作成
                var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(KeySize);

                //公開鍵を指定
                rsa.FromXmlString(clientPublicKey);

                //暗号化する文字列をバイト配列に
                byte[] data = System.Text.Encoding.UTF8.GetBytes(str);

                //  データサイズのチェック
                if (data.Length > rsa.KeySize / 8 - 11)
                    throw new Exception();

                //暗号化する
                //（XP以降の場合のみ2項目にTrueを指定し、OAEPパディングを使用できる）
                //  RSACryptoServiceProvider.KeySize プロパティで取得できるキーサイズが1024ビットだとすると、暗号化されるデータの最大長は、 1024/8-11=117バイト となります。
                byte[] encryptedData = rsa.Encrypt(data, false);

                //Base64で結果を文字列に変換
                return System.Convert.ToBase64String(encryptedData);
            }
            catch (Exception)
            {
            }

            return null;
        }


        /// <summary>
        /// 秘密鍵を使って文字列を復号化する
        /// </summary>
        /// <param name="str">Encryptメソッドにより暗号化された文字列</param>
        /// <returns>復号化された文字列</returns>
        public static string ClientDecrypt(string str)
        {
            try
            {
                //RSACryptoServiceProviderオブジェクトの作成
                var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(KeySize);

                //秘密鍵を指定
                rsa.FromXmlString(clientPrivateKey);

                //復号化する文字列をバイト配列に
                byte[] data = System.Convert.FromBase64String(str);
                //復号化する
                byte[] decryptedData = rsa.Decrypt(data, false);

                //結果を文字列に変換
                return System.Text.Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception)
            {
            }

            return null;
        }
        #endregion

    }

}
