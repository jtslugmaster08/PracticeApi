using PracticeModel.Interface.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PracticeService
{
    public class UserService : IUserService
    {
        internal static readonly char[] basechars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789^&".ToCharArray();
        internal static readonly char[] specialChars = "!@#$%*^&".ToCharArray();
        public UserService() { }
        public string GeneratePassword(int size)
        {
            byte[] data = new byte[4 * size];
            byte[] specialdata = new byte[4 * specialChars.Length];
            using(var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }
            using(var crypto2 = RandomNumberGenerator.Create())
            {
                crypto2.GetBytes(specialdata);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++) {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % basechars.Length;
                result.Append(basechars[idx]);
            }
            if(!result.ToString().Contains('&') && !result.ToString().Contains('^'))
            {
                Random r = new Random();
                do
                {
                    int i = r.Next(specialChars.Length);
                    var rnd = BitConverter.ToUInt32(specialdata, i * 4);
                    var idx = rnd % specialChars.Length;
                    result.Remove(i, 1);
                    result.Insert(i, specialChars[idx]);
                }
                while (!result.ToString().Any(o => specialChars.Contains(o)));
            }
            return result.ToString();
        }
    }
}
