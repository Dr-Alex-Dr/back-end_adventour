using MySql.Data;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;


namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();
            ServerConnection server = new ServerConnection();
            MySqlConnection connection = new MySqlConnection("server=localhost; database=AdvenTour; user=root; password=root; port=3306");
            try
            {
                connection.Open();
                Console.WriteLine("Connection succes");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection unsucces");
            }

            if (server.CheckConnection(connection))
            {
                app.MapGet("/", () => (ServerConnection.GetInformation(connection)));
            }
            else
            {
                app.MapGet("/", () => ("DataBase connection unsuccess"));
            }
            app.Run();
        }
    }

    public class PlacePointGet
    {
        public string name { get; set; }
        public string? category { get; set; }
        public int price { get; set; }
        public int time { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string? description { get; set; }
    }

    public class PlacePointPost
    {
        public string? category { get; set; }
        public int price { get; set; }
        public int time { get; set; }

        public PlacePointPost(string? category, int price, int time)
        {
            this.category = category;
            this.price = price;
            this.time = time;
        }
    }
    public class ServerConnection
    {
        public static List<PlacePointGet> GetInformation(MySqlConnection connection)
        {
            List<PlacePointGet> placePoints = new List<PlacePointGet>();

                using (var command = new MySqlCommand("SELECT * FROM Points", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PlacePointGet placePoint = new PlacePointGet()
                            {
                                name = reader["name"] as string,
                                category = reader["category"] as string,
                                price = (int)reader["price"],
                                time = (int)reader["time"],
                                latitude = (double)reader["latitude"],
                                longitude = (double)reader["longitude"],
                                description = (string)reader["description"]
                            };

                            placePoints.Add(placePoint);
                        }
                    }
                }
            return placePoints;
        }

        public bool CheckConnection(MySqlConnection connection)
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class Distance
    {
        public static double GetDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371d;
            var dLat = Deg2Rad(lat2 - lat1);
            var dLon = Deg2Rad(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2d) * Math.Sin(dLat / 2d) +
                Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
                Math.Sin(dLon / 2d) * Math.Sin(dLon / 2d);
            var c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1d - a));
            var d = R * c;
            return d;
        }

        public static double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180d);
        }
    }
}
