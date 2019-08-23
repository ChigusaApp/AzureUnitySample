using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Chigusa.AzureApiApp
{
    /// <summary>
    /// REST 向けユーティリティー
    /// </summary>
    public class REST
    {
        /// <summary>
        /// クライアントと共通のキー（自作）
        /// <para>
        /// byte[] secretkey = new Byte[64];<para/>
        /// RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();<para/>
        /// rng.GetBytes(secretkey);<para/>
        /// string key = Convert.ToBase64String(secretkey);<para/>
        /// </para>
        /// </summary>
        const string HashKey = "sOPaWjT89lMARUR4cnYClFLRuUglbt7PbS4t46WnV/gkyKxsB4YL4ILWDyQx/ZWkC3R8K4Dgu2XZHuFWQ07bBA==";

        /// <summary>
        /// アカウント名
        /// </summary>
        const string AccountName = "PublicUser";

        /// <summary>
        /// バージョン
        /// </summary>
        const string CurrentVersion = "2019-08-21";

        /// <summary>
        /// 最低限のバージョン
        /// </summary>
        const string MinimumVersion = "2019-08-21";

        /// <summary>
        /// キーサイズ
        /// </summary>
        const int KeySize = 2048;

        /// <summary>
        /// XML形式の公開鍵
        /// <para/>内部用
        /// </summary>
        const string publicKey = "<RSAKeyValue><Modulus>ps6AWJTwxNmaeE2zRneswOjlBGUtFSxTiwzq4aBGILlY0z9F6bmsnHK+5RZo+bdfK5GsLESu9KXRIFjfEaBFOXDht2bVkaQ9afHhP+JNxLNqNWtx/1JN3dhLu9wSeUJQpDx1iI5AIXFyS8Z9mrkhSKVS5OF9O54qtIG9/WwxyfkIrQqUWNlqbpBWmAADBhCc9gtIXW8CAu69rDVianZqAgaTQYZoaQh4FPNd7DnMrBkTh6QqS8h63woOmkIysTM2+mPw4NFy1z7TtMyaVuSGe1IbvEopUsWXhrH9mAM/YUMituyzVpwHy5ssnvMcjm3hPICwZ5xyIz4rMR0ytl0myw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        /// <summary>
        /// XML形式の秘密鍵
        /// <para/>内部用
        /// </summary>
        const string privateKey = "<RSAKeyValue><Modulus>ps6AWJTwxNmaeE2zRneswOjlBGUtFSxTiwzq4aBGILlY0z9F6bmsnHK+5RZo+bdfK5GsLESu9KXRIFjfEaBFOXDht2bVkaQ9afHhP+JNxLNqNWtx/1JN3dhLu9wSeUJQpDx1iI5AIXFyS8Z9mrkhSKVS5OF9O54qtIG9/WwxyfkIrQqUWNlqbpBWmAADBhCc9gtIXW8CAu69rDVianZqAgaTQYZoaQh4FPNd7DnMrBkTh6QqS8h63woOmkIysTM2+mPw4NFy1z7TtMyaVuSGe1IbvEopUsWXhrH9mAM/YUMituyzVpwHy5ssnvMcjm3hPICwZ5xyIz4rMR0ytl0myw==</Modulus><Exponent>AQAB</Exponent><P>rMmq/Y97R0mZaM+VzRyiYLu9USnAi3BR12FaUibP/gb4y1H1qDev82aaSs+ih67CupwIOK53Iq3J5FROitlg91loXVR3SUy6Ct2EYPtC1k+wzPPRHp/u7uzf8xqBg3ylHHD6Y9o/m3yVkK1NcKB0f61FjYWESjNs6uJ7V3AQS9c=</P><Q>9yNxx1VVDWXFE489WDrQS6IKNunAiu2TceutJTPhE4xH1RIQNsYf/CSQkGHBaOplSjVX3DWr9M2HUGdvnr75H3Smdm052b03IM1LeITfb0exzOje6J87JEa50rhZ22QzxHgsTVqalVOBNYpKLKzKrFaUPvJoPN+4Pyti8U20fi0=</Q><DP>kQPUcWhpY6Qej7uhEvtUQHFwa1zqT1zeIzB+mHIgoCeDyUipLxxFnLdhCSaIVu3SWsG3mLK8JCvNRMeI/7l/SzkLgUDxuGAfyBywDKD0MF/Dl8+Nk/FQm8/MCQEW756+CEe9re4u8jU2Za09UCgqfjmTJTqIk6njYKLiN7lCxBM=</DP><DQ>04GqJoaJO+YVPCAiyBfNnG0ZgNWcy0l4Hs8sZ+J2hlxQPbIv3jDBvvEsJ+UBueCgv1nXz8a/W8tqOw3LIXkfjCQxMrlYAVlGjPQl2wcckggxysShwURFQU6mSgksHe8rvsF6aNlZ4uKMel3YDVP0QHn4dcbQHwj4jld7Xji6eCE=</DQ><InverseQ>NX+4xa3J3RAgls/1Gft3rq3tIbdgSx151UYurQ8aOxyOhytCUJ5FGunXrKwJ7YmScvAImMHRVJpPEW/8HvEjZMH1t2qNgRxu4OXN9A+QkeItKFbIn5ped04NVhja1Gj3qmm/Ay1Iyx4fu1SBGAj0hSHv/mlCLMFCno+CcR5FWy0=</InverseQ><D>JgSg2g+wRJ14CH1lI+V42Ur7j6HjwZSlweaCzkPyV9kGtrcQHzIzdkDs41fVOz0hEtQT0s8gtR/p5x56UREfVfU1bNCiUpyFZtU7So0wvmlKBrzjcZb3ql12/W0uRLbpnFUvUJ1jXnv12fF2hUI2pNERcJs4KNNm1ldPzyfOvUKGkvwHsWG0tCwPmcJxEDBtOrcOZ4szjxz1ig/0KFcJTc4OCZAXQfnTnNu3eA/wJd0NRluEKWhXm83w0jdzv1cVbyVDCuoZZiiR6RzTct85XPoktqrxo1hps2qso2HwWDRfpR/BMqJ0dkT59gqzmU6k/TGEeAe3YqbgTIE1YHSRiQ==</D></RSAKeyValue>";

        /// <summary>
        /// XML形式の公開鍵
        /// <para/>送受信用
        /// </summary>
        const string accessPublicKey = "<RSAKeyValue><Modulus>qC+Br1QZDbrScpITTya4vBKDzDkmNR9ud5ZpL9xkn2M6x989Ffh7MKTZPh+qpKux2obJ8UFfTUKK/rhtWTwCV4BdwAeqdPKyHNKETV/xCNa7TuFZ+WHjXa7V5it3HQL372Okb1se5Gq6x0aQ96QQ64I+SEsa+ZdIFO5gQ3R9+G9Hutv/WWOnBgAdkFV/mMkLw7hoSHgZJG6vHO6PvVJRQraAMSVtMnfoD9bdPXyFylBzMHAYda3Y0KFk3ZXAysFkGb92Z63LtUbHqtlfVwSBYt/s0vEphZFNgafVyCnzr/KJzOgTAqPXcyBouPEl3On2a45Bk7YY2G5OtIXOU0dHWw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        /// <summary>
        /// XML形式の秘密鍵
        /// <para/>送受信用
        /// </summary>
        const string accessPrivateKey = "<RSAKeyValue><Modulus>qC+Br1QZDbrScpITTya4vBKDzDkmNR9ud5ZpL9xkn2M6x989Ffh7MKTZPh+qpKux2obJ8UFfTUKK/rhtWTwCV4BdwAeqdPKyHNKETV/xCNa7TuFZ+WHjXa7V5it3HQL372Okb1se5Gq6x0aQ96QQ64I+SEsa+ZdIFO5gQ3R9+G9Hutv/WWOnBgAdkFV/mMkLw7hoSHgZJG6vHO6PvVJRQraAMSVtMnfoD9bdPXyFylBzMHAYda3Y0KFk3ZXAysFkGb92Z63LtUbHqtlfVwSBYt/s0vEphZFNgafVyCnzr/KJzOgTAqPXcyBouPEl3On2a45Bk7YY2G5OtIXOU0dHWw==</Modulus><Exponent>AQAB</Exponent><P>6S2Jr4lTipH5j5ORnpd3lIAc72ptEOvifZSNC+u747flWzGBSI+JRFrNByr7HbK9T2rU7R9eXb96TVjQCpikiz9f3yGWpuGYLVKXnmW2gzrl2+vXpv2ZE+bRya6DlPVIUbe7q64HPmAkAvfz9K9NjiZiseMg/1ZfSievdi/P//s=</P><Q>uKWIi63i8UkeWSVB0TwzoBKkO8+GbznPqGuoYThtE60BiDn9WBgvPTbWJo/PTcTdzeaGZkCXSqk7x4071vQJaE8JEkjI87N4cFWFRAXxRYU2lc7Z5NNEtbK44ms9yvkYRhoHP6rpuN2nwXjY3oq1qkhugPF+7NKUBFfT9d4bWCE=</Q><DP>Sw4cXoM+f8EC6mMeoUSx+fvO1IWWO7chvTheujKT1PWxfswNiDSjg3wSuLbuA54v72s8xGKvdk+W52Sp+m8tr6CjlGf0XR0KZbRLF3I38D+6EzLzxE2mw1AtwNRKiFz3fU49u0IWFM7PaKE1RlJTDWlvpUCts2Ky42SoxssXEZc=</DP><DQ>ItFkDCK/9ou5I0o5PQFDIt4hBf499V9LXxDd8Wc4ektXTJ7SvPfigIKEo0Te7GVBBgCAO2vWm4eJ9DkXnZLq6zZsGXyMGBgxj80wkgEk290gy+Lzh4inHjQTVO6v+kQ6ZY2m7ESISgnSBlOJYX16gB+kZsAjFPkoXCFLoMM/GME=</DQ><InverseQ>bbZCotxGF7nIDNyFfWgLvqyo3GCpdIRARqW6MZYs09py1arHNdxFzrnxrFpK2qW+SJTa/ZaFeVvF4goS+wIta3J4U2v2PvpQIkM3Zy8iq0gLU56lPCgtfriL3uPmSECYaabrJ9E1WXbStpulBW8Klv5wsQERb7NJ8B6aeHzpBuQ=</InverseQ><D>JGEiTCqEwNOIcf/m7VgS3Zujpqk1+oex/WwrlI63tUJhlJpVPEtjWkJOO4HeB4CcGAjP5b+ePbgHGNIL1SGF4CeEH+WImYNlw5ZFx/bYzHCmU4mrTKY/6CPJYOBGA6CAwOdsyimLpRzxCenZsbdrcYXaswiL7TEsBlsJt42bMgGFwX2DT2E11TIlO/iGML0jrxPLwTgtX+yoO+5P7ZFkPvaJkrdhxyFfqizD6iFcUEepK3fyqaAN2Gyt/31zLJpH3dv5KFGhjrYooC7lVoPX7F0cY0JfepbaEXs9aNx2GYjYLtr6mVp35RFJ99DjjxkaP3Eln2/MeqjAgrbhSt94AQ==</D></RSAKeyValue>";

        /// <summary>
        /// XML形式の公開鍵
        /// <para/>送受信用
        /// </summary>
        const string clientPublicKey = "<RSAKeyValue><Modulus>6PWU2ZkwP7QGIPCA3tYbzTWYOe4bkfScwdQxqNrrQVZ5QOLZlXGpBIhYoSyeOD8inH3KsTILsfjBKMovFYmunvq6g1vHdkDB6Eyl4N1SIa/nrL1c8QR6GAAfbipHpFOwLxoC292k6+VshzoM2xiZllxGw/p4PMvDoA2pFq3jV3gl0RFNKKUbKFZKPvmqie8Z2LKe0n3uw2FXFHWqcJUMrgv28bUlu7VNt+Ou4kTTBNNMPsfOhlZqg/fb0fHGoLGffLW3ZTWSNNH8qiceAP3IVLJRSPHmyzDUE92rqxdCPhrmCiapm9z0rleTH1ZJi1ox1uWg87KfJMQJMQcpvxP44Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        ///// <summary>
        ///// XML形式の秘密鍵
        ///// <para/>送受信用
        ///// </summary>
        //const string clientPrivateKey = "<RSAKeyValue><Modulus>6PWU2ZkwP7QGIPCA3tYbzTWYOe4bkfScwdQxqNrrQVZ5QOLZlXGpBIhYoSyeOD8inH3KsTILsfjBKMovFYmunvq6g1vHdkDB6Eyl4N1SIa/nrL1c8QR6GAAfbipHpFOwLxoC292k6+VshzoM2xiZllxGw/p4PMvDoA2pFq3jV3gl0RFNKKUbKFZKPvmqie8Z2LKe0n3uw2FXFHWqcJUMrgv28bUlu7VNt+Ou4kTTBNNMPsfOhlZqg/fb0fHGoLGffLW3ZTWSNNH8qiceAP3IVLJRSPHmyzDUE92rqxdCPhrmCiapm9z0rleTH1ZJi1ox1uWg87KfJMQJMQcpvxP44Q==</Modulus><Exponent>AQAB</Exponent><P>+OcZ0w2Ycs/KqMYKXZufDTmdxGcrXpJIyZugqjL0keG1uW2NnEMGI/K1qHdZ1yMNYxAlYtYYGDtLphD3mSswALeCK9xy3enf+UKD9yueWayPzdtXxm/Nsc9KJPeTZDwtTmrsSOKjLlb8jhgWTrDCkQBeQpw0hAOquNd9l1Ki7hs=</P><Q>75oZbHbPd//YRZPq1HpA1hntA6eCJ+9Lp8uqhy7aFvsLX/25o2ABD+GxkxtBEc9KprlkIGoB1ImcbQAnhnqjafFfpeWDF/A6pOPI4V4kaxsJfDBnDybxocV3CztmuBea8gw28XShpmKihr8RS2A8MsbzSUl6TbfXQY2z1uD+NLM=</Q><DP>pbeoV/6rS3XRpoEEkcJ1KSb9RbCzDWo0EBcP54G5mA9BIM4yBKITSofkLuAX7scluJkdayrELA3+lfiiAVbhxPhpMK67w8hdGOYSWtStv2LG8/ZgAHyb5RDweqBjf88ZEybZXsWWg9nimPCsmYPSZxxppcu+o06Vsi+3LLMWS5U=</DP><DQ>ttIoUACf8XpANWbWKeZWjocduEoaIAqQ+amHprpzIlHPriDVgvmAFfQqIIsNLV+0IF8ZLTp1xwxxVSJnBk+RXQcV6mmji6J7vNEpt/yzYR4yMJZmLMOUX9FiMinTCOjKC6KSUc6igWiFhrdHpPH7POtdOzBbp+18y8Ip1O28Sc0=</DQ><InverseQ>KHvK1GC5G/FyPlXq29MMHKGzaH0lJsHFn+8pCDUqjZRWx3JLb5qaJCF1xanpJuTfxT5PyjUB0m92VSZzHYrlZwCxSY/84LmYspUbf0hg9WKVKn997I5zrYH8/7DmYx62P6DNZvmfbj8XG2uD+DF4FJ4+O+L1BJNumv6oMVFjy+I=</InverseQ><D>YDzj6yjPt+FlDsKMWoJVJedYJcZp+Vf2UlSY10rpGSriGJ4eiRBVZJv3EU5fZpkecUu+KomkLze2hGVSIjGow+CMoKuPsRh7gr33YPfNcEE9Ei2AcckJr1SNp3Mr+YThKfQy7iWYdiNnPE4M9Y4qFOW2Dkww09zRyhMaoVXgr0cXeSEgvZLRoDMoaWn5YOjP+x5EKMy15UkCMRF0xVfL8LBe56QLBHMYGtRUxrpJCq2E/O5pJnDi+cVJh+7+MHEPndrR6SjkLM3H6+yF+gWVJ/sI9I1eNC4XiWt4zlwsR6sw35TxqIK34N8+vA8dSwzuYN0p/Yowl6VEe4ymV6tvKQ==</D></RSAKeyValue>";



        /// <summary>
        /// 権限付与ヘッダーの作成
        /// </summary>
        /// <param name="canonicalizedString">正規化された文字列</param>
        /// <returns>ヘッダー文字列</returns>
        public static String CreateAuthorizationHeader(String canonicalizedString, string commonId)
        {
            if (String.IsNullOrEmpty(canonicalizedString))
            {
                throw new ArgumentNullException(nameof(canonicalizedString));
            }

            //  ハッシュキーで署名文字列を取得する
            String signature = CreateHmacSignature(canonicalizedString, Convert.FromBase64String(HashKey));
            //  ヘッダー文字列にする
            String authorizationHeader = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} {1}", commonId, signature);

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
                throw new ArgumentNullException(nameof(unsignedString));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
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
        public static string GetAuthorizationParameter(string requestMethod, string requestDate, string requestVersion, string requestAccountName, string addCanonicalizedHeader = "")
        {
            if (requestMethod == null || requestDate == null || requestVersion == null || requestAccountName == null)
                throw new ArgumentNullException("args");

            String canonicalizedHeaders = String.Format("x-chigusa-date:{0}\nx-chigusa-version:{1}", requestDate, requestVersion);
            if (!String.IsNullOrEmpty(addCanonicalizedHeader))
                canonicalizedHeaders += "\n" + addCanonicalizedHeader;
            String canonicalizedResource = String.Format("/{0}", requestAccountName);
            String stringToSign = String.Format("{0}\n{1}\n{2}", requestMethod, canonicalizedHeaders, canonicalizedResource);
            String authorizationHeader = CreateAuthorizationHeader(stringToSign, requestAccountName);
            return authorizationHeader;
        }



        /// <summary>
        /// 公開鍵と秘密鍵を作成して返す
        /// </summary>
        /// <param name="publicKey">作成された公開鍵(XML形式)</param>
        /// <param name="privateKey">作成された秘密鍵(XML形式)</param>
        public static void CreateKeys(out string publicKey, out string privateKey)
        {
            //RSACryptoServiceProviderオブジェクトの作成
            var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(KeySize);

            //公開鍵をXML形式で取得
            publicKey = rsa.ToXmlString(false);
            //秘密鍵をXML形式で取得
            privateKey = rsa.ToXmlString(true);
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
                rsa.FromXmlString(publicKey);

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
        public static string Decrypt(string str)
        {
            try
            {
                //RSACryptoServiceProviderオブジェクトの作成
                var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(KeySize);

                //秘密鍵を指定
                rsa.FromXmlString(privateKey);

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



        /// <summary>
        /// 公開鍵を使って暗号化する
        /// </summary>
        /// <param name="str">暗号化するデータ</param>
        /// <returns>暗号化されたデータ</returns>
        public static string EncryptBigData(string str)
        {
            try
            {
                byte[] targetData = System.Text.Encoding.UTF8.GetBytes(str);
                var resultData = EncryptBigData(targetData);
                //Base64で結果を文字列に変換
                return System.Convert.ToBase64String(resultData);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 公開鍵を使って暗号化する
        /// </summary>
        /// <param name="str">暗号化するデータ</param>
        /// <returns>暗号化されたデータ</returns>
        public static byte[] EncryptBigData(byte[] targetData)
        {
            try
            {
                //RSACryptoServiceProviderオブジェクトの作成
                var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(KeySize);

                //公開鍵を指定
                rsa.FromXmlString(publicKey);

                int blockNumber = targetData.Length;
                blockNumber /= rsa.KeySize / 8 - 11;
                if (targetData.Length % (rsa.KeySize / 8 - 11) != 0)
                    blockNumber++;
                int totalSize = blockNumber * (rsa.KeySize / 8);

                byte[] resultData = new byte[totalSize];

                for (int index = 0; index < blockNumber; index++)
                {
                    //暗号化する
                    //（XP以降の場合のみ2項目にTrueを指定し、OAEPパディングを使用できる）
                    //  RSACryptoServiceProvider.KeySize プロパティで取得できるキーサイズが1024ビットだとすると、暗号化されるデータの最大長は、 1024/8-11=117バイト となります。
                    int tempSize = rsa.KeySize / 8 - 11;
                    if (targetData.Length - index * tempSize < tempSize)
                        tempSize = targetData.Length - index * tempSize;
                    byte[] tempData = new byte[tempSize];
                    Buffer.BlockCopy(targetData, index * (rsa.KeySize / 8 - 11), tempData, 0, tempSize);
                    byte[] encryptedData = rsa.Encrypt(tempData, false);
                    Buffer.BlockCopy(encryptedData, 0, resultData, index * (rsa.KeySize / 8), encryptedData.Length);
                }

                return resultData;
            }
            catch (Exception)
            {
            }

            return null;
        }



        /// <summary>
        /// 秘密鍵を使って復号化する
        /// </summary>
        /// <param name="str">復号化するデータ</param>
        /// <returns>復号化されたデータ</returns>
        public static string DecryptBigData(string str)
        {
            try
            {
                byte[] targetData = System.Convert.FromBase64String(str);
                var resultData = DecryptBigData(targetData);
                return System.Text.Encoding.UTF8.GetString(resultData);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 公開鍵を使って復号化する
        /// </summary>
        /// <param name="str">復号化するデータ</param>
        /// <returns>復号化されたデータ</returns>
        public static byte[] DecryptBigData(byte[] targetData)
        {
            try
            {
                //RSACryptoServiceProviderオブジェクトの作成
                var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(KeySize);

                //秘密鍵を指定
                rsa.FromXmlString(privateKey);

                int blockNumber = targetData.Length;
                blockNumber /= rsa.KeySize / 8;
                if (targetData.Length % (rsa.KeySize / 8) != 0)
                    blockNumber++;

                byte[] resultData = new byte[targetData.Length];
                int resultOffset = 0;
                for (int index = 0; index < blockNumber; index++)
                {
                    //復号化する
                    int tempSize = rsa.KeySize / 8;
                    if (targetData.Length - index * tempSize < tempSize)
                        tempSize = targetData.Length - index * tempSize;
                    byte[] tempData = new byte[tempSize];
                    Buffer.BlockCopy(targetData, index * (rsa.KeySize / 8), tempData, 0, tempSize);
                    byte[] decryptedData = rsa.Decrypt(tempData, false);
                    Buffer.BlockCopy(decryptedData, 0, resultData, resultOffset, decryptedData.Length);
                    resultOffset += decryptedData.Length;
                }
                Array.Resize(ref resultData, resultOffset);

                //結果を文字列に変換
                return resultData;
            }
            catch (Exception ex)
            {
                string exe = ex.Message;
            }

            return null;
        }



        /// <summary>
        /// 公開鍵を使って文字列を暗号化する
        /// </summary>
        /// <param name="str">暗号化する文字列</param>
        /// <returns>暗号化された文字列</returns>
        public static string AccessEncrypt(string str)
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
        /// 秘密鍵を使って文字列を復号化する
        /// </summary>
        /// <param name="str">Encryptメソッドにより暗号化された文字列</param>
        /// <returns>復号化された文字列</returns>
        public static string AccessDecrypt(string str)
        {
            try
            {
                //RSACryptoServiceProviderオブジェクトの作成
                var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(KeySize);

                //秘密鍵を指定
                rsa.FromXmlString(accessPrivateKey);

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
        #endregion


        /// ハッシュのチェック
        /// </summary>
        /// <param name="requestMethod">GET, POST, PUT, DELETE のメソッド</param>
        /// <returns>結果</returns>
        public static bool CheckHash(string requestMethod, string commonId, System.Net.Http.HttpRequestMessage request, string addHeader = "")
        {
            if (string.IsNullOrEmpty(requestMethod) || string.IsNullOrEmpty(commonId) || request == null || request.Headers == null)
                return false;

            try
            {
                //  IDは同じ？
                if (commonId != request.Headers.Authorization.Scheme)
                    throw new Exception("CreateArgException");

                //  5分以内まで
                string requestDate = GetHeaderValue("x-chigusa-date", request.Headers);
                string[] expectedFormats = { "ddd, d MMM yyyy HH':'mm':'ss zzz", "r" };
                DateTime localTime = System.DateTime.ParseExact(requestDate,
                            expectedFormats,
                            System.Globalization.DateTimeFormatInfo.InvariantInfo,
                            System.Globalization.DateTimeStyles.None);
                TimeSpan timeDifference = DateTime.UtcNow - localTime;
                if (Math.Abs(timeDifference.TotalMinutes) > 5)
                    throw new Exception("CreateBadTimeException");

                //  ハッシュで検査
                string requestVersion = GetHeaderValue("x-chigusa-version", request.Headers);
                string requestAuthorizationHeader = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} {1}",
                                                           request.Headers.Authorization.Scheme, request.Headers.Authorization.Parameter);
                string authorization = GetAuthorizationParameter(requestMethod, requestDate, requestVersion, request.Headers.Authorization.Scheme, addHeader);
                if (authorization == null || authorization != requestAuthorizationHeader)
                    throw new Exception("CreateArgException");

                //  バージョンチェック
                //  Comments... 比較バージョンをリスト化して特定のもののみ有効に変更する？
                if (string.Compare(MinimumVersion, requestVersion) > 0)
                    throw new Exception("CreateArgException");
            }
            //catch (System.Web.Http.HttpResponseException ex)
            //{
            //    throw ex;
            //}
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// ヘッダーから特定の値を取り出す
        /// </summary>
        /// <param name="name">対象の名前</param>
        /// <returns>結果</returns>
        public static string GetHeaderValue(string name, System.Net.Http.Headers.HttpRequestHeaders requestheaders)
        {
            if (requestheaders == null)
                return null;
            foreach (string tempValue in requestheaders.GetValues(name))
            {
                return tempValue;
            }
            return null;
        }

    }

}
