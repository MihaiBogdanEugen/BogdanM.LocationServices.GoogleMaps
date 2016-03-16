namespace BogdanM.LocationServices.GoogleMaps.Dtos
{
    public class GoogleMapsRoutePart
    {
        public GoogleMapsRectangle Bounds { get; set; }
        public string Copyrights { get; set; }
        public GoogleMapsLegs[] Legs { get; set; }
        public GoogleMapsPolyline OverviewPolyline { get; set; }
        public string Summary { get; set; }
        public string[] Warnings { get; set; }
    }
}