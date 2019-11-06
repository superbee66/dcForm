using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using dCForm.Client.Util;
using dCForm.Core;

namespace dCForm.Core.Util
{
    /// <summary>
    ///     Hey Greg,
    ///     Not relevant to production activities between us anymore; I have a full blow version of that “Rand/Mock” poco
    ///     property filling factory class working. I wanted to test it out & show you it’s results as something relevant
    ///     between us. Can you paste or attach your poco to me?
    /// </summary>
    public class Rand
    {
        private readonly Dictionary<string, List<string>> getSetText = new Dictionary<string, List<string>>();

        public bool bit(object seed = null)
        {
            return int32(seed) > 0;
        }

        public byte byte_(object seed = null, byte min = byte.MinValue, byte max = byte.MaxValue)
        {
            return (byte)int16(seed, min, max);
        }

        public DateTime date(object seed = null, DateTime? min = null, DateTime? max = null)
        {
            return datetime(seed, min, max).Date;
        }

        public DateTime datetime(object seed = null, DateTime? min = null, DateTime? max = null)
        {
            DateTime low = min ?? DateTime.MinValue;
            DateTime top = max ?? DateTime.MaxValue;

            return new DateTime(int64(seed, low.Ticks, top.Ticks));
        }

        //public string text(object seed = null, int min = 1, int max = 50) {
        //    int minWordCount = int32(new[] { seed }, min, max);
        //    int maxWordCount = int32(new[] { seed, seed }, minWordCount, max);
        //    return Regex.Matches(RandomText, string.Format(@"(\w+[^\w]+){{{0},{1}}}", minWordCount, maxWordCount)).Cast<Match>().OrderBy(m => int32(new[] { seed, m })).FirstOrDefault().ToString();
        //}

        public Guid guid(object seed = null)
        {
            return new Guid(
                int32(seed),
                int16(seed),
                int16(seed),
                byte_(seed),
                byte_(seed),
                byte_(seed),
                byte_(seed),
                byte_(seed),
                byte_(seed),
                byte_(seed),
                byte_(seed));
        }

        public Int16 int16(object seed = null, int min = Int16.MinValue, Int16 max = Int16.MaxValue)
        {
            return (Int16)new Random(seed == null ? 0 : seed.GetHashCode()).Next(min, max);
        }

        public int int32(object seed = null, int min = int.MinValue, int max = int.MaxValue)
        {
            return new Random(seed == null ? 0 : seed.GetHashCode()).Next(min, max);
        }

        public long int64(object seed = null, long min = long.MinValue, long max = long.MaxValue)
        {
            float r = max - (float)min;
            r *= percent(seed);
            r += min;

            return (long)r;
        }

        public float percent(object seed = null)
        {
            return Math.Abs((float)int32(seed)) / int.MaxValue;
        }

        //private static readonly string RandomText = string.Join(" ", Directory.EnumerateFiles(Environment.ExpandEnvironmentVariables("%WINDIR%"), "*.txt").Take(5).Select(f => File.ReadAllText(f)).ToArray());

        public T obj<T>(T src, object seed = null, int min = 2, int max = 7)
        {
            foreach (PropertyInfo _PropertyInfo in src.GetType().GetProperties())
                if (_PropertyInfo.CanRead)
                    if (_PropertyInfo.CanWrite)
                        //TODO:need to centralize this logic as there are many places though out the code trying to detect if we are dealing with a collection of something
                        if (_PropertyInfo.PropertyType != typeof(byte[])
                            && _PropertyInfo.PropertyType != typeof(string)
                            && !typeof(IDictionary).IsAssignableFrom(_PropertyInfo.PropertyType)
                            && typeof(IEnumerable).IsAssignableFrom(_PropertyInfo.PropertyType))
                        {
                            int i = int32(new[]
                            {
                                seed, _PropertyInfo.Name
                            }, min, max);
                            IList _IList = (IList)Activator.CreateInstance(_PropertyInfo.PropertyType, i);
                            while (i-- != 0)
                                _IList.Add(obj(Activator.CreateInstance(_PropertyInfo.PropertyType.GetEnumeratedType()), new[]
                                {
                                    seed, _PropertyInfo.Name
                                }, min, max));
                            _PropertyInfo.SetValue(src, _IList, null);
                        }
                        else
                        {
                            MethodInfo _MethodInfo = GetType().GetMethods().OrderBy(m => int32(m)).FirstOrDefault(m => m.ReturnType == (Nullable.GetUnderlyingType(_PropertyInfo.PropertyType) ?? _PropertyInfo.PropertyType));
                            
                            if (_PropertyInfo.PropertyType == typeof(byte[]))
                                _PropertyInfo.SetValue(src, null, null);
                            else if (_PropertyInfo.PropertyType == typeof(string))
                                _PropertyInfo.SetValue(src, string.Format("{0} string property placeholder", StringTransform.Wordify(_PropertyInfo.Name)), null);
                            else if (_MethodInfo != null)
                                _PropertyInfo.SetValue(src, _MethodInfo.Invoke(this, _MethodInfo.GetParameters().Select(p => p.Name == "seed" ? new[]
                                {
                                        seed, p.Name
                                    } : p.DefaultValue).ToArray()), null);
                        }
            return src;
        }
    }
}