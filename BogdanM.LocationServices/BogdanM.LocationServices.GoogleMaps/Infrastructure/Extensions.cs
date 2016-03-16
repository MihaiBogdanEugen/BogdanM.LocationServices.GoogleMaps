using BogdanM.LocationServices.Core;

namespace BogdanM.LocationServices.GoogleMaps.Infrastructure
{
    public static class Extensions
    {
        public static bool IsPointInside(this LatLng[] polygon, LatLng point)
        {
            var result = false;
            var j = polygon.Length - 1;
            for (var i = 0; i < polygon.Length; i++)
            {
                if (polygon[i].Lng < point.Lng && polygon[j].Lng >= point.Lng || polygon[j].Lng < point.Lng && polygon[i].Lng >= point.Lng)
                {
                    if (polygon[i].Lat + (point.Lng - polygon[i].Lng) / (polygon[j].Lng - polygon[i].Lng) * (polygon[j].Lat - polygon[i].Lat) < point.Lat)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
    }
}
