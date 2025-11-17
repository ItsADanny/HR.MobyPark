using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public static class CDatabaseParkingLot
{
    public static List<ParkingLot> GetAll()
    {
        var results = new List<ParkingLot>();

        using (var conn = new MySqlConnection(connectionString))
        using (var cmd = new MySqlCommand("SELECT * FROM ParkingLots", conn))
        {
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(MapParkingLot(reader));
                }
            }
        }

        return results;
    }

    public static ParkingLot GetById(int id)
    {
        using (var conn = new MySqlConnection(connectionString))
        using (var cmd = new MySqlCommand("SELECT * FROM ParkingLots WHERE ID = @id;", conn))
        {
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();

            using (var reader = cmd.ExecuteReader())
            {
                return reader.Read() ? MapParkingLot(reader) : null;
            }
        }
    }

    public static bool Delete(int id)
    {
        using (var conn = new MySqlConnection(connectionString))
        using (var cmd = new MySqlCommand("DELETE FROM ParkingLots WHERE ID = @id;", conn))
        {
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            int affected = cmd.ExecuteNonQuery();
            return affected > 0;
        }
    }

    public static int Insert(string name, string location, string address, int capacity, decimal tariff, decimal dayTariff, double lat, double lon)
    {
        using (var conn = new MySqlConnection(connectionString))
        using (var cmd = new MySqlCommand(
            @"INSERT INTO ParkingLots (Name, Location, Address, Capacity, Tariff, DayTariff, Lat, Lon)
              VALUES (@name, @location, @address, @capacity, @tariff, @dayTariff, @lat, @lon);
              SELECT LAST_INSERT_ID();", conn))
        {
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@location", location);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@capacity", capacity);
            cmd.Parameters.AddWithValue("@tariff", tariff);
            cmd.Parameters.AddWithValue("@dayTariff", dayTariff);
            cmd.Parameters.AddWithValue("@lat", lat);
            cmd.Parameters.AddWithValue("@lon", lon);

            try
            {
                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Log.WriteErrorLine(ex, $"Error inserting parking lot '{name}'.");
                return -1;
            }
        }
    }

    private static ParkingLot MapParkingLot(MySqlDataReader reader)
    {
        return new ParkingLot
        {
            Id = reader.GetInt32("ID"),
            Name = reader.GetString("Name"),
            Location = reader.GetString("Location"),
            Address = reader.GetString("Address"),
            Capacity = reader.GetInt32("Capacity"),
            Tariff = reader.GetDouble("Tariff"),
            DayTariff = reader.GetDouble("DayTariff"),
            Latitude = reader.GetDouble("Lat"),
            Longitude = reader.GetDouble("Lon")
        };
    }
}
