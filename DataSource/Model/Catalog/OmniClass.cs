using DataSource.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DataSource.Model.Catalog
{
    public class OmniClass : ACatalog, IEquatable<OmniClass>
    {
        public static IList<int> GetId(string idAsString)
        {
            if (string.IsNullOrWhiteSpace(idAsString) || idAsString.Contains(Constant.Point) == false) { return null; }

            var splitedId = idAsString.Split(Constant.PointChar);
            var intIds = new int[splitedId.Length];
            for (var idx = 0; idx < splitedId.Length; idx++)
            {
                var stringId = splitedId[idx];
                if (int.TryParse(stringId, out var id) == false) { continue; }

                intIds[idx] = id;
            }
            return intIds;
        }

        public static string GetId(ICollection<int> idArray)
        {
            return string.Join(Constant.Point, idArray);
        }

        [JsonIgnore]
        public override string Id
        {
            get { return GetId(IdArray); }
            set { IdArray = GetId(value); }
        }


        [JsonProperty(nameof(Id))]
        public IList<int> IdArray { get; set; }

        [JsonIgnore]
        public string NumberAndName
        {
            get
            {
                if (IdArray is null || IdArray.Count == 0 || (IdArray.Count == 1 && IdArray[0] == 0))
                {
                    return string.Concat(Name);
                }
                return string.Concat(Id, Constant.Space, Name);
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as OmniClass);
        }

        public bool Equals(OmniClass other)
        {
            return other != null &&
                   NumberAndName == other.NumberAndName;
        }

        public override int GetHashCode()
        {
            var hashCode = 1744613352;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NumberAndName);
            return hashCode;
        }

        public static bool operator ==(OmniClass left, OmniClass right)
        {
            return EqualityComparer<OmniClass>.Default.Equals(left, right);
        }

        public static bool operator !=(OmniClass left, OmniClass right)
        {
            return !(left == right);
        }
    }
}
