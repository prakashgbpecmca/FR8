using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Frayte.Api.Models;

namespace Frayte.Api
{
    public class CryptoEngine
    {
        public static string Encrypt(string input, string PrivateKey)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(PrivateKey);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string input, string PrivateKey)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(PrivateKey);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }

    public class ShipmentType
    {
        public static string LetterType(decimal TotalWeight, string ParcelType)
        {
            if (TotalWeight <= 2.0m)
            {
                if (ParcelType == ApiDocType.Doc)
                {
                    return ApiDocType.Letter;
                }
                else if (ParcelType == ApiDocType.Nondoc)
                {
                    return ApiDocType.Parcel;
                }
                else
                {
                    return ApiDocType.Letter;
                }
            }
            else
            {
                return ApiDocType.Parcel;
            }
        }

        public static string DocType(decimal TotalWeight, string ParcelType)
        {
            if (TotalWeight <= 2.0m)
            {
                if (ParcelType == ApiDocType.Doc)
                {
                    return ApiDocType.Doc;
                }
                else if(ParcelType == ApiDocType.Nondoc)
                {
                    return ApiDocType.Nondoc;
                }
                else
                {
                    return ApiDocType.Doc;
                }
            }
            else
            {
                return ApiDocType.Nondoc;
            }
        }

        public static string DocDescription(decimal TotalWeight, string ParcelType)
        {
            if (TotalWeight <= 2.0m)
            {
                if (ParcelType == ApiDocType.Doc)
                {
                    return ApiDocType.LetterDisplay;
                }
                else if(ParcelType == ApiDocType.Nondoc)
                {
                    return ApiDocType.ParcelDisplay;
                }
                else
                {
                    return ApiDocType.LetterDisplay;
                }
            }
            else
            {
                return ApiDocType.ParcelDisplay;
            }
        }

        public static string PackageType(int PackageCount)
        {
            if (PackageCount == 1)
            {
                return ApiPackageType.Single;
            }
            else
            {
                return ApiPackageType.Multiple;
            }
        }
    }
}