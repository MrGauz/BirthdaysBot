using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace BirthdaysBot
{
    public class Database
    {
        private static SqliteConnection sqlite;

        public static void Initialize(string dbName)
        {
            bool newFile = false;
            if (!File.Exists(dbName + ".sqlite"))
            {
                newFile = true;
            }

            string connectionString = $"Data Source={dbName}.sqlite";
            sqlite = new SqliteConnection(connectionString);
            sqlite.Open();
            var query = sqlite.CreateCommand();

            if (newFile)
            {
                query.CommandText = @"CREATE TABLE birthdays(
                                        id INTEGER PRIMARY KEY,
                                        user_id BIGINT, 
                                        text TEXT,
                                        month INT,
                                        day INT
                                    );";
                try
                {
                    query.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine($"{ex.Message} (sqlite code: {ex.SqliteErrorCode})\n" + ex.StackTrace);
                }
            }
        }

        public static void NewBirthday(Int64 userId, string text, int month, int day)
        {
            var query = sqlite.CreateCommand();
            query.CommandText = $"INSERT INTO birthdays ('user_id', 'text', 'month', 'day') VALUES ({userId}, '{text.Replace("'", "''")}', {month}, {day});";
            try
            {
                query.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"{ex.Message} (sqlite code: {ex.SqliteErrorCode})\n" + ex.StackTrace);
            }
        }

        public static List<KeyValuePair<Int64, string>> GetTodaysBirthdays()
        {
            var birthdays = new List<KeyValuePair<Int64, string>>();
            var query = sqlite.CreateCommand();
            query.CommandText = $"SELECT user_id, text FROM birthdays WHERE month = {DateTime.Now.Month} AND day = {DateTime.Now.Day};";
            try
            {
                var result = query.ExecuteReader();
                while (result.Read())
                {
                    birthdays.Add(new KeyValuePair<Int64, string>(result.GetInt64(0), result.GetString(1)));
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"{ex.Message} (sqlite code: {ex.SqliteErrorCode})\n" + ex.StackTrace);
            }

            return birthdays;
        }

        public static void DeleteUserBirthdays(Int64 userId)
        {
            var query = sqlite.CreateCommand();
            query.CommandText = $"DELETE FROM birthdays WHERE user_id = {userId};";
            try
            {
                query.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"{ex.Message} (sqlite code: {ex.SqliteErrorCode})\n" + ex.StackTrace);
            }
        }
    }
}
