public readonly struct WaypointData
{
    public string WaypointName { get; }
    public double Latitude { get; }
    public double Longitude { get; }
    public double Height { get; }
    public bool IsValid { get; }
    public bool IsBaseStation { get; }

    public WaypointData(
        string waypointName, 
        double latitude, 
        double longitude, 
        double height, 
        bool isValid,
        bool isBaseStation)
    {
        this.WaypointName = waypointName;
        this.Latitude = latitude;
        this.Longitude = longitude;
        this.Height = height;
        this.IsValid = isValid;
        this.IsBaseStation = isBaseStation;
    }
}
