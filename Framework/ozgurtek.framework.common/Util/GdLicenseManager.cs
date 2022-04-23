using System;
using System.Globalization;

namespace ozgurtek.framework.common.Util
{
    public sealed class GdLicenseManager
    {
        private static GdLicenseManager _instance;
        private static readonly object Padlock = new object();
        private string _key;

        private GdLicenseManager()
        {
        }

        public static GdLicenseManager Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ?? (_instance = new GdLicenseManager());
                }
            }
        }

        public string Key
        {
            get => _key;
            set => _key = value;
        }

        public bool CheckValid()
        {
            if (_key == "Developer Licence")
                return true;

            if (string.IsNullOrWhiteSpace(_key))
                throw new Exception("Licence key required use -> GdLicenceManager.Instance.Key = xyz");

            string encryptAes = GdCrypto.DecryptAes(_key);
            string[] values = encryptAes.Split(';');
            string licenceOwner = values[0];
            DateTime expireTime = DateTime.Parse(values[1], CultureInfo.InvariantCulture);
            if (expireTime <= DateTime.Now)
                throw new Exception($"Licence expired: {licenceOwner}-{expireTime}" );

            return true;
        }
    }
}