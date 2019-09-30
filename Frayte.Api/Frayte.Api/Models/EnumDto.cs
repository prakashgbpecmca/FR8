using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
    public static class ApiPackageType
    {
        public const string Single = "Single";
        public const string Multiple = "Multiple";
    }

    public static class ApiDocType
    {
        public const string Doc = "Doc";
        public const string Nondoc = "Nondoc";
        public const string Letter = "Letter";
        public const string Parcel = "Parcel";
        public const string LetterDisplay = "Letter (Document)";
        public const string ParcelDisplay = "Parcel (Non Doc)";
    }

    public static class ApiAddressType
    {
        public const string B2B = "B2B";
    }

    public static class EncriptionKey
    {
        public const string PrivateKey = "IRAS-Soft-Techno";
    }
}