using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace March17.Data
{
    public class DBManager
    {
        private string _conection;

        public DBManager(string conection)
        {
            _conection = conection;
        }
        public List<Simcha> GetAllSimchos()
        {
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT s.Id, s.Name, s.Date, COUNT(c.ContributorId) AS  [Contributor Count], SUM(c.amount) AS [Total]
                                    FROM Simchos s
                                    LEFT JOIN Contributions c
                                    ON s.Id = c.SimchaId
                                    GROUP BY s.Id, s.Name, s.Date";
            connection.Open();
            List<Simcha> simchos = new();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Simcha s = new()
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    ContributorCount = (int)reader["Contributor Count"],

                };
                if (s.ContributorCount != 0)
                {
                    s.Total = (decimal)reader["Total"];
                };
                simchos.Add(s);
            }
            return simchos;
        }
        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT c1.*, d.Amount, SUM(c2.Amount) AS Contributed FROM Contributions c2
                                    JOIN Contributors c1
                                    ON c1.Id = c2.ContributorId
                                    LEFT JOIN Deposits d
                                    ON d.ContributorId = c1.Id
                                    GROUP BY c1.Id, c1.FirstName, c1.LastName, c1.CellNumber, c1.AlwaysInclude, d.Amount";
            connection.Open();
            List<Contributor> contributors = new();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                contributors.Add(new Contributor
                {
                    Id = reader.Get<int>("Id"),
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    AlwaysInclude = reader.Get<bool>("AlwaysInclude"),
                    Balance = reader.Get<decimal>("Amount") + reader.Get<decimal>("Contributed")
                });
            }
            return contributors;
        }
        public List<Contributor> GetContributorsBySimcha(int id)
        {
            List<Contributor> contributors = GetContributors();
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT c1.id, ISNULL(c2.Amount, 0) AS 'AMOUNT' FROM Contributors c1
                                    JOIN Contributions c2
                                    ON c1.Id = c2.ContributorId
                                    WHERE SimchaId = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            List<Contributor> contributed = new();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int cId = reader.Get<int>("Id");
                Contributor c = contributors.Find(x => x.Id == cId);
                c.Contributed = true;
                c.AmountContributed = reader.Get<decimal>("Amount");
            }
            return contributors;
        }

        public int GetContributorCount()
        {
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Contributors";
            connection.Open();
            return (int)command.ExecuteScalar();
        }
        public List<Transaction> GetTransactionsById(int id)
        {
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Deposits 
                                    WHERE ContributorId=@id;
                                    SELECT * FROM Contributions c 
                                    JOIN Simchos s 
                                    ON s.Id = c.SimchaId
                                    WHERE ContributorId = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            List<Transaction> transactions = new();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                transactions.Add(new Transaction
                {
                    Name = "Deposit",
                    Date = reader.Get<DateTime>("Date"),
                    Amount = reader.Get<decimal>("Amount")
                });
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        transactions.Add(new Transaction
                        {
                            Name = reader.Get<string>("Name"),
                            Date = reader.Get<DateTime>("Date"),
                            Amount = reader.Get<decimal>("Amount")
                        });
                    }
                }
            }
            return transactions;
        }
        public void DeleteContributions(int id)
        {
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM Contributions
                                    WHERE SimchaId = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public string GetSimchaName(int id)
        {
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Name FROM Simchos WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return (string)command.ExecuteScalar();
        }
        public string GetContributorName(int id)
        {
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT FirstName, LastName FROM Contributors WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return $"{reader.Get<string>("FirstName")} {reader.Get<string>("LastName")}";
        }
        public void EditContributor(Contributor c)
        {
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE Contributors
                                    SET FirstName = @first, LastName = @last,
                                    CellNumber = @cell, Date = @date,
                                    AlwaysInclude = @always
                                    WHERE Id = @id";
            command.Parameters.AddWithValue("@first", c.FirstName);
            command.Parameters.AddWithValue("@last", c.LastName);
            command.Parameters.AddWithValue("@cell", c.CellNumber);
            command.Parameters.AddWithValue("@always", c.AlwaysInclude);
            command.Parameters.AddWithValue("@date", c.Date);
            command.Parameters.AddWithValue("@id", c.Id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public void AddSimcha(Simcha s)
        {
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Simchos  VALUES(@name, @date)";
            command.Parameters.AddWithValue("@name", s.Name);
            command.Parameters.AddWithValue("@date", s.Date);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public int AddContributor(Contributor c)
        {
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Contributors  
                                    VALUES(@firstName, @lastName, @cell, @include, @date)
                                    SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@firstName", c.FirstName);
            command.Parameters.AddWithValue("@lastName", c.LastName);
            command.Parameters.AddWithValue("@cell", c.CellNumber);
            command.Parameters.AddWithValue("@include", c.AlwaysInclude);
            command.Parameters.AddWithValue("@date", c.Date);
            connection.Open();
            return (int)(decimal)command.ExecuteScalar();

        }
        public void AddDeposit(Transaction t)
        {
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Deposits
                                    VALUES(@amount, @date, @id)";
            command.Parameters.AddWithValue("@amount", t.Amount);
            command.Parameters.AddWithValue("@date", t.Date);
            command.Parameters.AddWithValue("@id", t.ContributorId);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public void AddContributions(List<Transaction> transactions, int id)
        {
            using var connection = new SqlConnection(_conection);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Contributions
                                    VALUES(@cid, @sid, @amount)";
            connection.Open();
            foreach (var t in transactions)
            {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@cid", t.ContributorId);
                command.Parameters.AddWithValue("@sid", id);
                command.Parameters.AddWithValue("@amount", -t.Amount);
                command.ExecuteNonQuery();
            }

        }
    }
}
